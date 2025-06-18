using DndTest.Data;
using DndTest.Data.Model.CustomFields;

namespace DndTest.Services;

public class CustomFieldService(
    DndDbContext dbContext
)
{
    public async Task Create(CustomField customField)
    {
        dbContext.CustomFields.Add(customField);
        await dbContext.SaveChangesAsync();
    }
}
