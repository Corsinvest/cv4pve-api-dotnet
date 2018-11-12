using System.Collections.Generic;

namespace EnterpriseVE.ProxmoxVE.Api.Extension.Utils
{
    internal class AttributeHelper
    {
        public static void NotExistCreate(dynamic obj, string key, object defaultValue = null)
        {
            var dic = (IDictionary<string, object>)obj;
            if (!(dic.ContainsKey(key))) { dic.Add(key, defaultValue); }
        }
    }
}