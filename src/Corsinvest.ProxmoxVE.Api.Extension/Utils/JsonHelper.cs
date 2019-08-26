using System.Collections.Generic;

namespace Corsinvest.ProxmoxVE.Api.Extension.Utils
{
    internal class JsonHelper
    {
        public static void GetValueOrCreate(dynamic obj, string key, object defaultValue = null)
        {     
            var dic = (IDictionary<string, object>)obj;
            if (!(dic.ContainsKey(key))) { dic.Add(key, defaultValue); }
        }
    }
}