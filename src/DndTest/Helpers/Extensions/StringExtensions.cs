using System.Text;

namespace DndTest.Helpers.Extensions;

public static class StringExtensions
{
    public static string XxHash128(this string str)
    {
        str = str.Trim();
        var textBytes = Encoding.UTF8.GetBytes(str);
        var hashBytes = System.IO.Hashing.XxHash128.Hash(textBytes);
        var hashString = Convert.ToBase64String(hashBytes);
        return hashString;
    }
}
