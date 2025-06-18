using DndTest.Data;
using DndTest.Data.Model;
using DndTest.Data.Model.Content;
using DndTest.Data.Model.CustomFields;
using DndTest.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace DataLoader;

internal class Program
{
    // AOE = free text
    // Book = 

    static async Task Main(string[] args)
    {
        var json = System.IO.File.ReadAllText("Priest.list.json");
        var data = System.Text.Json.JsonSerializer.Deserialize<SpellData>(json);

        var battlefate = data.Spells.Single(s => s.Name == "Battlefate");

        var converter = new ReverseMarkdown.Converter();

        var markdown = converter.Convert(battlefate.Body.Replace("<br />", "<br /><br />"));

        Console.WriteLine(markdown);

        var spellTypeOption = new CustomFieldOption { Name = "Spell" };

        var type = new CustomField()
        {
            Name = "Type",
            Type = CustomFieldType.SingleChoice,
            Options = [
                spellTypeOption,
                new CustomFieldOption { Name = "Rules" },
                new CustomFieldOption { Name = "Lore" },
            ],
        };

        var book = new CustomField()
        {
            Name = "Book",
            Options = data.Spells.Select(s => s.Book).Distinct().Select(v => new CustomFieldOption { Name = v }).ToList(),
            Type = CustomFieldType.SingleChoice,
            Conditions = [
                new CustomFieldCondition
                {
                    DependsOnCustomField = type,
                    RequiredOptions = [spellTypeOption],
                }
            ],
        };

        var schools = new CustomField()
        {
            Name = "Schools",
            Options = data.Spells.SelectMany(s => s.SchoolsSplit).Distinct().Select(v => new CustomFieldOption { Name = v }).ToList(),
            Type = CustomFieldType.MultiChoice,
            Conditions = [
                new CustomFieldCondition
                {
                    DependsOnCustomField = type,
                    RequiredOptions = [spellTypeOption],
                }
            ],
        };

        var spheres = new CustomField()
        {
            Name = "Spheres",
            Options = data.Spells.SelectMany(s => s.SpheresSplit).Distinct().Select(v => new CustomFieldOption { Name = v }).ToList(),
            Type = CustomFieldType.MultiChoice,
            Conditions = [
                new CustomFieldCondition
                {
                    DependsOnCustomField = type,
                    RequiredOptions = [spellTypeOption],
                }
            ],
        };

        var range = new CustomField()
        {
            Name = "Range",
            Options = data.Spells.SelectMany(s => s.SpheresSplit).Distinct().Select(v => new CustomFieldOption { Name = v }).ToList(),
            Type = CustomFieldType.SingleValue,
            Conditions = [
                new CustomFieldCondition
                {
                    DependsOnCustomField = type,
                    RequiredOptions = [spellTypeOption],
                }
            ],
        };

        var components = new CustomField()
        {
            Name = "Components",
            Options = data.Spells.SelectMany(s => s.ComponentsSplit).Distinct().Select(v => new CustomFieldOption { Name = v }).ToList(),
            Type = CustomFieldType.MultiChoice,
            Conditions = [
                new CustomFieldCondition
                {
                    DependsOnCustomField = type,
                    RequiredOptions = [spellTypeOption],
                }
            ],
        };

        var duration = new CustomField()
        {
            Name = "Duration",
            Options = data.Spells.Select(s => s.Duration).Distinct().Select(v => new CustomFieldOption { Name = v }).ToList(),
            Type = CustomFieldType.SingleValue,
            Conditions = [
                new CustomFieldCondition
                {
                    DependsOnCustomField = type,
                    RequiredOptions = [spellTypeOption],
                }
            ],
        };

        var castingTime = new CustomField()
        {
            Name = "Casting Time",
            Options = data.Spells.Select(s => s.CastingTime).Distinct().Select(v => new CustomFieldOption { Name = v }).ToList(),
            Type = CustomFieldType.SingleValue,
            Conditions = [
                new CustomFieldCondition
                {
                    DependsOnCustomField = type,
                    RequiredOptions = [spellTypeOption],
                }
            ],
        };

        var aoe = new CustomField()
        {
            Name = "Area of Effect",
            Options = data.Spells.Select(s => s.AreaOfEffect).Distinct().Select(v => new CustomFieldOption { Name = v }).ToList(),
            Type = CustomFieldType.SingleValue,
            Conditions = [
                new CustomFieldCondition
                {
                    DependsOnCustomField = type,
                    RequiredOptions = [spellTypeOption],
                }
            ],
        };

        var savingThrow = new CustomField()
        {
            Name = "Saving Throw",
            Options = data.Spells.Select(s => s.SavingThrow).Distinct().Select(v => new CustomFieldOption { Name = v }).ToList(),
            Type = CustomFieldType.SingleValue,
            Conditions = [
                new CustomFieldCondition
                {
                    DependsOnCustomField = type,
                    RequiredOptions = [spellTypeOption],
                }
            ],
        };

        var level = new CustomField()
        {
            Name = "Level",
            Type = CustomFieldType.Integer,
            Conditions = [
                new CustomFieldCondition
                {
                    DependsOnCustomField = type,
                    RequiredOptions = [spellTypeOption],
                }
            ],
        };

        var customFields = (CustomField[])[
            // First, others depend on this.
            type,

            // Dependents:
            aoe,
            book,
            castingTime,
            components,
            duration,
            level,
            range,
            savingThrow,
            schools,
            spheres,
        ];

        var notes = data.Spells.Select(spell => new Note()
        {
            Name = spell.Name,
            Content = converter.Convert(spell.Body.Replace("<br />", "<br /><br />")),
            CustomFieldValues = [
                Cf(aoe, spell.AreaOfEffect),
                Cf(book, spell.Book),
                Cf(castingTime, spell.CastingTime),
                Cf(components, spell.ComponentsSplit),
                Cf(duration, spell.Duration),
                new ItemCustomFieldValue() { CustomField = level, ValueInteger = spell.Level },
                Cf(range, spell.Range),
                Cf(savingThrow, spell.SavingThrow),
                Cf(schools, spell.SchoolsSplit),
                Cf(spheres, spell.SpheresSplit),
                new ItemCustomFieldValue() { CustomField = type, Values = [spellTypeOption] },
            ],
        }).ToArray();

        //return;

        await DndTest.Program.Main2(app => Func(app, customFields, notes));

        ItemCustomFieldValue Cf(CustomField customField, params string[] values)
        {
            return new ItemCustomFieldValue
            {
                CustomField = customField,
                CustomFieldId = customField.Id,
                Values = customField.Options
                    .Where(option => values.Contains(option.Name))
                    .ToList(),
            };
        }
    }

    private static async Task Func(WebApplication app, CustomField[] customFields, Note[] notes)
    {
        var scope = app.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<DndDbContext>();

        var tenant = scope.ServiceProvider.GetRequiredService<DndDbContext>().Tenants.Single();
        var customFieldService = scope.ServiceProvider.GetRequiredService<CustomFieldService>();

        foreach (var customField in customFields)
        {
            customField.Tenant = tenant;

            await customFieldService.Create(customField);
        }

        var noteService = scope.ServiceProvider.GetRequiredService<NoteService>();

        foreach (var note in notes)
        {
            note.Tenant = tenant;
            await noteService.Create(note);
        }

        await dbContext.SaveChangesAsync();
    }
}