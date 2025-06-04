using Amazon.Auth.AccessControlPolicy;
using DndTest.Data;
using Microsoft.EntityFrameworkCore;

namespace DndTest.Api;

public class CustomFieldApi(
    DndDbContext dbContext
)
{
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
