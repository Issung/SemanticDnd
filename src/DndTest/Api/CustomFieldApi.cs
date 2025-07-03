using DndTest.Api.Models.Response;
using DndTest.Data;
using DndTest.Services;
using Microsoft.EntityFrameworkCore;

namespace DndTest.Api;

public class CustomFieldApi(
    SecurityContext securityContext,
    DndDbContext dbContext
)
{
    public async Task<CustomFieldsResponse> GetAll()
    {
        var customFields = await dbContext.CustomFields
            .Where(cf => cf.TenantId == securityContext.TenancyId)
            .Include(cf => cf.Options)
            .Include(cf => cf.Conditions)
            .ThenInclude(c => c.RequiredOptions)
            .AsSplitQuery() // TODO: Probably better. Benchmark it. Well really this functionality will eventually be split up into multiple APIs.
            .ToArrayAsync();

        return new()
        {
            CustomFields = customFields.Select(cf => new CustomFieldsResponse.CustomField()
            {
                Id = cf.Id,
                Name = cf.Name,
                Type = cf.Type,
                Options = cf.Options.Select(o => new CustomFieldsResponse.CustomFieldOption()
                {
                    Id = o.Id,
                    Name = o.Name
                }),
                Conditions = cf.Conditions.Select(c => new CustomFieldsResponse.CustomFieldCondition()
                {
                    RequiredOptionIds = c.RequiredOptions.Select(ro => ro.Id),
                    DependsOnCustomFieldId = c.DependsOnCustomFieldId,
                })
            })
        };
    }

    public async Task<object> GetCustomFieldSummaries()
    {
        return await dbContext.CustomFields
            .Select(cf => new
            {
                cf.Id,
                cf.Name,
                cf.Type,
            })
            .ToListAsync();
    }

    public async Task<object> GetCustomFieldOptions(int customFieldId)
    {
        var customField = await dbContext.CustomFields
            .Include(cf => cf.Options)
            .SingleAsync(cf => cf.Id == customFieldId);
        return customField.Options.Select(o => new { o.Id, o.Name }).ToList();
    }

    public async Task<object> GetCustomField(int id)
    {
        var customField = await dbContext.CustomFields
            .Include(cf => cf.Options)
            .Include(cf => cf.Conditions)
                .ThenInclude(con => con.DependsOnCustomField)
            .Include(cf => cf.Conditions)
                .ThenInclude(con => con.RequiredOptions)
            .SingleAsync(cf => cf.Id == id);

        return new
        {
            customField.Id,
            customField.Name,
            customField.Type,
            Options = customField.Options.Select(o => new { o.Id, o.Name }),
            Conditions = customField.Conditions.Select(c => new
            {
                c.Id,
                c.CustomFieldId,
                c.DependsOnCustomFieldId,
                RequiredOptions = c.RequiredOptions.Select(o => new { o.Id, o.Name })
            }),
        };
    }
}
