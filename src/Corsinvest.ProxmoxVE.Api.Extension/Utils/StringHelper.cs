namespace Corsinvest.ProxmoxVE.Api.Extension.Utils
{
    /// <summary>
    /// String helper
    /// </summary>
    public static class StringHelper
    {
        /// <summary>
        /// New line Unix
        /// </summary>
        /// <value></value>
        public static char NewLineUnix => '\n';

        /// <summary>
        /// New line Windows
        /// </summary>
        /// <value></value>
        public static string NewLineWindows => "\r\n";

        /// <summary>
        /// Is numeric
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static bool IsNumeric(string value) => int.TryParse(value, out var vmId);
    }
}