using System.IO.Hashing;

namespace DndTest.Helpers.Extensions;

public static class StreamExtensions
{
    public static string XXHash128(this Stream stream)
    {
        var hasher = new XxHash128();

        var buffer = new byte[8192]; // 8KB buffer
        int bytesRead;

        while ((bytesRead = stream.Read(buffer, 0, buffer.Length)) > 0)
        {
            hasher.Append(new ReadOnlySpan<byte>(buffer, 0, bytesRead));
        }

        var hashBytes = hasher.GetCurrentHash();
        return Convert.ToHexString(hashBytes).ToLowerInvariant();
    }
}
