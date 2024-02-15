using System.Text.RegularExpressions;

namespace Indiko.Maui.Controls.Markdown.Utils;
internal static class Validations
{
    internal static bool IsValidBase64String(string input)
    {
        if (string.IsNullOrEmpty(input) || input.Length % 4 != 0)
        {
            return false;
        }
        const string base64Pattern = "^[a-zA-Z0-9\\+/]*={0,3}$";
        return Regex.IsMatch(input, base64Pattern);
    }
}
