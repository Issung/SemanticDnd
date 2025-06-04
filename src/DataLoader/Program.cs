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

        var battlefate = data.Spells.Single(s => s.Name == "Battlefate");

        var converter = new ReverseMarkdown.Converter();

        var markdown = converter.Convert(battlefate.Body.Replace("<br />", "<br /><br />"));

        Console.WriteLine(markdown);

        return;

        await DndTest.Program.Main2(async app =>
        {
            var scope = app.Services.CreateScope();
            var service = scope.ServiceProvider.GetRequiredService<NoteService>();

            foreach (var spell in data!.Spells)
            {
                await service.UploadPlainText(spell.Name, spell.ToString());
            }
        });
    }
}