using System.Text;

public static class StringExtensions {

    public static string KoreanTo1252(this string str) => Encoding.GetEncoding(1252).GetString(Encoding.GetEncoding(949).GetBytes(str));
    
    public static string Windows1252ToKorean(this string str) => Encoding.GetEncoding(949).GetString(Encoding.GetEncoding(1252).GetBytes(str));

    public static string SanitizeForAddressables(this string str) => str.Replace("[", "_").Replace("]", "_").Replace("\\", "/");
}
