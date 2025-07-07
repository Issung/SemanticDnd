using DndTest.Api.Models.Request;
using DndTest.Api.Models.Response;
using DndTest.Data;
using DndTest.Data.Model.Content;
using DndTest.Data.Repositories;
using DndTest.Exceptions;
using DndTest.Services;
using Microsoft.EntityFrameworkCore;
using File = DndTest.Data.Model.Content.File;

namespace DndTest.Api;

public class ItemApi(
    DndDbContext dbContext,
    S3Service s3Service,
    SecurityContext securityContext,
    ItemAppRepository itemRepo,
    TimeProvider timeProvider
)
{
    /// <param name="folderId">Null for root of tenancy.</param>
    /// <exception cref="NotFoundException"/>
    /// <exception cref="BadRequestException"/>
    public async Task<BrowseResponse> Browse(int? folderId)
    {
        Folder? folder = null;

        if (folderId.HasValue)
        {
            folder = await GetItemOfType<Folder>(folderId.Value);
        }

        var query = dbContext.Items
            .Where(i => i.TenantId == securityContext.TenancyId)
            .Where(i => i.ParentId == folderId);

        var count = await query.CountAsync();

        var items = query
            .Include(i => i.CustomFieldValues)
                .ThenInclude(c => c.CustomField)
            .Include(i => i.CustomFieldValues)
                .ThenInclude(c => c.Values)
            .OrderByDescending(i => i is Folder)    // Place folders at top.
            .ThenBy(i => i.Name)
            .Select(i => new ItemSummary(i))
            //.AsSplitQuery()   // Does improve performance by like 50-65%.
            .AsAsyncEnumerable();

        return new BrowseResponse(
            ParentId: folder?.ParentId,
            FolderId: folder?.Id,
            FolderName: folder?.Name ?? "Content",
            FolderDescription: folder?.Description ?? "",
            ItemCount: count,
            Items: items
        );
    }

    public async Task<ItemResponse> Get(int id)
    {
        var item = await dbContext.Items
            .Where(i => i.TenantId == securityContext.TenancyId)
            .Include(i => i.Parent)
            .Include(i => i.CustomFieldValues)
                .ThenInclude(c => c.CustomField)
            .Include(i => i.CustomFieldValues)
                .ThenInclude(c => c.Values)
            .Include(i => i.Bookmarks.Where(b => b.BookmarkCollection.UserId == securityContext.UserId))
            .SingleAsync(d => d.Id == id);

        var fileUrl = await MaybeGetFileUrl(item);

        var model = new Models.Response.Item(item)
        {
            FileAccessUrl = fileUrl,
            Text = item is Note note ? note.Content : null,
        };

        return new(model);
    }

    public async Task<int> PutFile(int? id, FilePutRequest request, IFormFile? binary)
    {
        var now = timeProvider.GetUtcNow().UtcDateTime;

        File file;
        if (id.HasValue)
        {
            file = await GetItemOfType<File>(id.Value);
        }
        else
        {
            file = new File { CreatedAt = now };
            dbContext.Files.Add(file);
        }

        if (request.ParentId.HasValue)
        {
            await ValidateParentId(request.ParentId);
        }

        if (binary != null)
        {
            file.S3ObjectKey = Guid.NewGuid().ToString();
            file.ContentType = binary.ContentType;
            file.SizeBytes = binary.Length;
            file.FileHash = "fakehash" + Guid.NewGuid().ToString();
            await s3Service.Put(file.S3ObjectKey, binary.OpenReadStream(), binary.ContentType);
        }

        if (id.HasValue && file.ParentId != request.ParentId)
        {
            await ValidateParentId(request.ParentId);
        }

        file.Name = request.Name;
        file.Description = request.Description;
        file.UpdatedAt = now;
        file.TenantId = securityContext.TenancyId;
        file.ParentId = request.ParentId;

        await dbContext.SaveChangesAsync();

        return file.Id;
    }

    public async Task PutNote(int? id, NotePutRequest request) => throw new NotImplementedException();
    public async Task PutShortcut(int? id, ShortcutPutRequest request) => throw new NotImplementedException();

    public async Task PutFolder(int? id, FolderPutRequest request)
    {
        var now = timeProvider.GetUtcNow().UtcDateTime;

        var folder = id.HasValue
            ? await GetItemOfType<Folder>(id.Value)
            : new Folder() { CreatedAt = now };

        await ValidateParentId(request.ParentId);

        folder.Name = request.Name;
        folder.Description = request.Description;
        folder.UpdatedAt = now;
        folder.TenantId = securityContext.TenancyId;
        folder.ParentId = request.ParentId;

        if (id == null)
        {
            dbContext.Folders.Add(folder);
        }

        await dbContext.SaveChangesAsync();
    }

    /// <exception cref="NotFoundException"/>
    /// <exception cref="ForbiddenNotFoundException"/>
    public async Task Delete(int id)
    {
        var item = await itemRepo.GetItemOrNotFound(id);

        dbContext.Items.Remove(item);

        // If folder delete all sub children.
        if (item is Folder)
        {
            // TODO: This will actually have to be recursive... lol.
            // TODO: We shouldn't hard delete, instead mark as deleted to allow for recovery...

            // ExecuteDeleteAsync didn't work because "'ExecuteDelete'/'ExecuteUpdate' operations on hierarchies mapped as TPT is not supported".

            // TODO: Move to ItemRepository maybe I guess, I dunno... T-T

            var sql = """
                BEGIN;

                DELETE FROM "Shortcuts" USING "Items"
                WHERE "Shortcuts"."Id" = "Items"."Id" AND "Items"."ParentId" = @id;

                DELETE FROM "Notes" USING "Items"
                WHERE "Notes"."Id" = "Items"."Id" AND "Items"."ParentId" = @id;

                DELETE FROM "Folders" USING "Items"
                WHERE "Folders"."Id" = "Items"."Id" AND "Items"."ParentId" = @id;

                DELETE FROM "Files" USING "Items"
                WHERE "Files"."Id" = "Items"."Id" AND "Items"."ParentId" = @id;

                DELETE FROM "Items"
                WHERE "ParentId" = @id;

                COMMIT;
                """;

            await dbContext.Database.ExecuteSqlRawAsync(sql, new Npgsql.NpgsqlParameter("@id", id));

        }

        await dbContext.SaveChangesAsync();
    }

    /// <summary>
    /// Ensure id used for a ParentId is valid by doing some sanity checks and then checking the database to see if it is a folder.
    /// </summary>
    /// <exception cref="BadRequestException"/>
    private async Task ValidateParentId(int? id)
    {
        if (id == null)
        {
            // Null is okay, it means store at root (within no folder, just the tenancy).
            return;
        }

        if (id <= 0)
        {
            throw new BadRequestException($"ParentId ({id}) must be a positive integer.");
        }

        // Also verifies that it is within this tenancy.
        var item = await itemRepo.GetItem(id.Value);

        if (item is not Folder)
        {
            throw new BadRequestException($"Id used as ParentId ({id}) must be a folder.");
        }
    }

    /// <exception cref="BadRequestException"></exception>
    private async Task<T> GetItemOfType<T>(int id) where T : Data.Model.Content.Item
    {
        var item = await itemRepo.GetItemOrNotFound(id);

        if (item is not T)
        {
            throw new BadRequestException($"Item {id} is not of type {typeof(T).Name}.");
        }

        return (T)item;
    }

    private async Task<Uri?> MaybeGetFileUrl(Data.Model.Content.Item doc)
    {
        if (doc is Data.Model.Content.File file)
        {
            return await s3Service.GetAccessUrl(file.S3ObjectKey);
        }

        return null;
    }
}
