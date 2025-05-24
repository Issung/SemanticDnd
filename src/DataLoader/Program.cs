using DndTest.Data.Model;
using DndTest.Services;
using Microsoft.Extensions.DependencyInjection;

namespace DataLoader;

internal class Program
{
    static async Task Main(string[] args)
    {
        var json = System.IO.File.ReadAllText("Priest.list.json");
        var data = System.Text.Json.JsonSerializer.Deserialize<SpellData>(json);

        return;

        await DndTest.Program.Main2(async app =>
        {
            var scope = app.Services.CreateScope();
            var service = scope.ServiceProvider.GetRequiredService<DocumentService>();

            foreach (var spell in data!.Spells)
            {
                await service.UploadPlainText(spell.Name, Category.Spells, spell.ToString());
            }
        });
    }
}