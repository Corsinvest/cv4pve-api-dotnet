using System.Globalization;

namespace EnterpriseVE.ProxmoxVE.Api.Extension
{
    internal static class StringExtension
    {
        public static string Capitalize(this string text)
        {
            return CultureInfo.CurrentCulture.TextInfo.ToTitleCase(text.ToLower());
        }
    }
}