using DndTest.Data;
using DndTest.Data.Model.Content;
using DndTest.Data.Model.CustomFields;
using DndTest.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System.Diagnostics;

namespace DataLoader;

internal class Program
{
    /*
     * Wipe database tables altered by this loader:

TRUNCATE TABLE "CustomFieldOption" RESTART IDENTITY CASCADE;
TRUNCATE TABLE "CustomFieldCondition" RESTART IDENTITY CASCADE;
TRUNCATE TABLE "CustomFields" RESTART IDENTITY CASCADE;
TRUNCATE TABLE "Notes" RESTART IDENTITY CASCADE;
TRUNCATE TABLE "Items" RESTART IDENTITY CASCADE;
TRUNCATE TABLE "SearchChunks" RESTART IDENTITY CASCADE;
    */

    static async Task Main(string[] args)
    {

        return; // Don't run this stuff that's already been done.

        await MoveSpellsToFolders();
        await LoadSpells();
    }

    private static async Task MoveSpellsToFolders()
    {
        await DndTest.Program.Main2(async app =>
        {
            var scope = app.Services.CreateScope();

            var dbContext = scope.ServiceProvider.GetRequiredService<DndDbContext>();

            const int wizardCfValueId = 4;
            const int priestCfValueId = 5;

            const int wizFolder = 927;
            const int priestFolder = 928;

            //var items = await dbContext.Items
            //    .Where(i => i.CustomFieldValues.Any(cf => cf.CustomFieldId == 2 && cf.Values.Any(v => v.Id == wizardCfValueId)))
            //    .ToArrayAsync();
            //
            //Debug.WriteLine("Wait");

            // Wiz
            await dbContext.Items
                .Where(i => i.CustomFieldValues.Any(cf => cf.CustomFieldId == 2 && cf.Values.Any(v => v.Id == wizardCfValueId)))
                .ExecuteUpdateAsync(_ => _.SetProperty(i => i.ParentId, wizFolder));

            await dbContext.Items
                .Where(i => i.CustomFieldValues.Any(cf => cf.CustomFieldId == 2 && cf.Values.Any(v => v.Id == priestCfValueId)))
                .ExecuteUpdateAsync(_ => _.SetProperty(i => i.ParentId, priestFolder));
        });
    }

    private static async Task LoadSpells()
    {
        var priestData = System.Text.Json.JsonSerializer.Deserialize<SpellData>(System.IO.File.ReadAllText("Priest.list.json"))!;
        var wizardData = System.Text.Json.JsonSerializer.Deserialize<SpellData>(System.IO.File.ReadAllText("Wizard.list.json"))!;

        var converter = new ReverseMarkdown.Converter();
        var allSpells = priestData.Spells.Concat(wizardData.Spells);

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

        var wizard = new CustomFieldOption { Name = "Wizard" };
        var priest = new CustomFieldOption { Name = "Priest" };

        var classCf = new CustomField()
        {
            Name = "Class",
            Type = CustomFieldType.SingleChoice,
            Options = [
                wizard,
                priest,
            ],
            Conditions = [
                new CustomFieldCondition
                {
                    DependsOnCustomField = type,
                    RequiredOptions = [spellTypeOption],
                }
            ],
        };

        var book = new CustomField()
        {
            Name = "Book",
            Options = allSpells.Select(s => s.Book).Distinct().Select(v => new CustomFieldOption { Name = v }).ToList(),
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
            Options = allSpells.SelectMany(s => s.SchoolsSplit).Distinct().Select(v => new CustomFieldOption { Name = v }).ToList(),
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
            Options = allSpells.SelectMany(s => s.SpheresSplit ?? []).Distinct().Select(v => new CustomFieldOption { Name = v }).ToList(),
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
            Options = allSpells.Select(s => s.Range).Distinct().Select(v => new CustomFieldOption { Name = v }).ToList(),
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
            Options = allSpells.SelectMany(s => s.ComponentsSplit).Distinct().Select(v => new CustomFieldOption { Name = v }).ToList(),
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
            Options = allSpells.Select(s => s.Duration).Distinct().Select(v => new CustomFieldOption { Name = v }).ToList(),
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
            Options = allSpells.Select(s => s.CastingTime).Distinct().Select(v => new CustomFieldOption { Name = v }).ToList(),
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
            Options = allSpells.Select(s => s.AreaOfEffect).Distinct().Select(v => new CustomFieldOption { Name = v }).ToList(),
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
            Options = allSpells.Select(s => s.SavingThrow).Distinct().Select(v => new CustomFieldOption { Name = v }).ToList(),
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

            // From us - either Wizard or Priest.
            classCf,

            // From JSON files.
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

        var notes = allSpells.Select(spell => new Note()
        {
            Name = spell.Name,
            Content = converter.Convert(spell.Body.Replace("<br />", "<br /><br />")),
            CustomFieldValues = new[] {
                Cf(classCf, priestData.Spells.Contains(spell) ? "Priest" : "Wizard"),
                Cf(aoe, spell.AreaOfEffect),
                Cf(book, spell.Book),
                Cf(castingTime, spell.CastingTime),
                Cf(components, spell.ComponentsSplit),
                Cf(duration, spell.Duration),
                new ItemCustomFieldValue() { CustomField = level, ValueInteger = spell.Level },
                Cf(range, spell.Range),
                Cf(savingThrow, spell.SavingThrow),
                Cf(schools, spell.SchoolsSplit),
                spell.SpheresSplit == null ? null : Cf(spheres, spell.SpheresSplit),
                new ItemCustomFieldValue() { CustomField = type, Values = [spellTypeOption] },
            }.OfType<ItemCustomFieldValue>().ToList(),  // Remove nulls (Spheres may be null).
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