﻿namespace DndTest.Data.Model;

public class ExtractedText
{
    public int Id { get; set; }

    public int FileId { get; set; }
    public Content.File File { get; set; } = default!;

    /// <summary>
    /// Null if the document did not have pages (according to tika).
    /// </summary>
    public int? PageNumber { get; set; }

    public string Text { get; set; } = default!;
}
