using System.Text.RegularExpressions;

namespace OnlineConsulting.BusinessLogic.Concretions.StorageServices.Utilities.FileHelpers;

public static class NameOperation
{
    public static string CharacterRegulatory(string fileName)
    {
        if (string.IsNullOrWhiteSpace(fileName))
            return string.Empty;

        var replacements = new (string, string)[]
        {
        ("\"", ""), ("!", ""), ("'", ""), ("#", ""), ("^", ""), ("+", ""), ("%", ""), ("&", ""),
        ("/", ""), ("(", ""), (")", ""), ("=", ""), ("?", ""), ("_", ""), ("@", ""), ("€", ""),
        ("~", ""), ("`", ""), (".", ""), ("-", ""), ("<", ""), (">", ""), ("|", ""), (";", ""),
        (":", ""), (",", ""), ("ö", "o"), ("ü", "u"), ("ä", "a"), ("Ö", "O"), ("Ü", "U"), ("Ä", "A"),
        ("İ", "I"), ("ı", "i"), ("ğ", "g"), ("Ğ", "G"), ("ş", "s"), ("Ş", "S"), ("ç", "c"), ("Ç", "C")
        };

        foreach (var (oldChar, newChar) in replacements)
        {
            fileName = fileName.Replace(oldChar, newChar);
        }

        fileName = Regex.Replace(fileName, "[^a-zA-Z0-9]", "");

        return fileName;
    }
}
