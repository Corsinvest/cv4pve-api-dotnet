namespace Corsinvest.ProxmoxVE.Api.Extension.Utils
{
    public static class StringHelper
    {
        internal static bool IsNumeric(string value) => int.TryParse(value, out var vmId);
    }
}