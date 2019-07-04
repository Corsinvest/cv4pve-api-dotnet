namespace Corsinvest.ProxmoxVE.Api.Extension.Utils
{
    internal static class StringHelper
    {
        internal static bool IsNumeric(string value) => int.TryParse(value, out var vmId);
    }
}