namespace DndTest.Exceptions;

/// <summary>
/// A security exception (forbidden) but displays as a Not Found (404) within API response.
/// </summary>
public class ForbiddenNotFoundException : Exception
{
    public ForbiddenNotFoundException()
    {
    }

    public ForbiddenNotFoundException(string? message) : base(message)
    {
    }
}
