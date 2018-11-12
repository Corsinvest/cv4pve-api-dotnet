namespace EnterpriseVE.ProxmoxVE.Api.Extension.Utils
{
    internal static class StringHelper
    {
        public static bool IsNumeric(string value) { return int.TryParse(value, out var vmId); }
    }
}