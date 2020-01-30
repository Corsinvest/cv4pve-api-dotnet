using System;
using System.IO;

namespace Corsinvest.ProxmoxVE.Api.Extension.Helpers
{
    /// <summary>
    /// Application helper
    /// </summary>
    public class ApplicationHelper
    {
        /// <summary>
        /// Get application data directory. If not exists create.
        /// </summary>
        /// <param name="appName"></param>
        /// <returns></returns>
        public static string GetApplicationDataDirectory(string appName)
        {
            var path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "Corsinvest", appName);
            if (!Directory.Exists(path))
            {
                var dir = Directory.CreateDirectory(path);
                //dir.Attributes = FileAttributes.Directory | FileAttributes.Hidden;
            }

            return path;
        }
    }
}