using DndTest.Data.Model.Content;
using DndTest.Exceptions;
using DndTest.Services;
using Microsoft.EntityFrameworkCore;

namespace DndTest.Data.Repositories;

/// <summary>
/// Item repository for app api interactions (applies user security context).
/// </summary>
public class ItemAppRepository(
    SecurityContext securityContext,
    DndDbContext dbContext
)
{
    /// <exception cref="ForbiddenNotFoundException">Was found but was not in the current tenancy.</exception>
    public async Task<Item?> GetItem(int id)
    {
        var item = await dbContext.Items
            .Where(i => i.TenantId == securityContext.TenancyId)
            .Where(i => i.Id == id)
            .SingleOrDefaultAsync();

        if (item != null && item.TenantId != securityContext.TenancyId)
        {
            // Item exists but is not in the user's tenancy.
            throw new ForbiddenNotFoundException($"Item {id} not found in tenancy {securityContext.TenancyId}.");
        }

        return item;
    }

    /// <exception cref="NotFoundException"/>
    /// <exception cref="ForbiddenNotFoundException"/>
    public async Task<Item> GetItemOrNotFound(int id)
    {
        return await GetItem(id) ?? throw new NotFoundException($"Item {id} not found.");
    }

    ///// <exception cref="NotFoundException"/>
    ///// <exception cref="ForbiddenNotFoundException"/>
    ///// <exception cref="BadRequestException"/>
    //public async Task<Item> GetItemOrNotFound<T>(int id) where T : Item
    //{
    //    var item = await GetItemOrNotFound(id);
    //
    //    VerifyItemType<T>(item);
    //
    //    return item;
    //}
    //
    //public async Task<Item?> GetItem<T>(int id) where T : Item
    //{
    //    var item = await GetItem(id);
    //
    //    if (item != null)
    //    {
    //        VerifyItemType<T>(item);
    //    }
    //
    //    return item;
    //}
}
