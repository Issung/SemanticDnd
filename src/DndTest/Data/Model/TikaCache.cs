using DndTest.Services;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json;

namespace DndTest.Data.Model;

[Table("TikaCache")]
[PrimaryKey(nameof(FileHash))]
public class TikaCache
{
    public string FileHash { get; set; } = null!;

    public string TikaResponseJson { get; set; } = null!;

    public TikaResponse TikaResponse => JsonSerializer.Deserialize<TikaResponse>(TikaResponseJson, TikaService.JsonOptions) ?? throw new Exception("Deserialized TikaResponse unexpectedly null.");
}
