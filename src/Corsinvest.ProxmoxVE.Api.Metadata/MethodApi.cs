using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json.Linq;

namespace Corsinvest.ProxmoxVE.Api.Metadata
{
    /// <summary>
    /// Method Api
    /// </summary>
    public class MethodApi
    {
        /// <summary>
        /// Costructor
        /// </summary>
        /// <param name="token"></param>
        /// <param name="classApi"></param>
        public MethodApi(JToken token, ClassApi classApi)
        {
            MethodType = token["method"].ToString();
            MethodName = token["name"].ToString();
            Comment = token["description"] + "";
            ClassApi = classApi;

            var returns = token["returns"];
            if (returns != null)
            {
                ReturnType = returns["type"] + "";

                if (returns["properties"] != null)
                {
                    ReturnParameters.AddRange(returns["properties"]
                                    .Select(a => new ParameterApi(a.Parent[((JProperty)a).Name]))
                                    .ToArray());
                }
                else if (returns["items"] != null && returns["items"]["properties"] != null)
                {
                    ReturnParameters.AddRange(returns["items"]["properties"]
                                    .Select(a => new ParameterApi(a.Parent[((JProperty)a).Name]))
                                    .ToArray());
                }

                if (returns["links"] != null)
                {
                    ReturnLinkHRef = returns["links"][0]["href"].ToString();
                    ReturnLinkRel = returns["links"][0]["rel"].ToString();
                }
            }

            if (token["parameters"] != null && token["parameters"]["properties"] != null)
            {
                Parameters.AddRange(token["parameters"]["properties"]
                          .Select(a => new ParameterApi(a.Parent[((JProperty)a).Name]))
                          .ToArray());
            }

            ReturnIsArray = ReturnType == "array";
            ReturnIsNull = ReturnType == "null";
        }

        /// <summary>
        /// Href
        /// </summary>
        /// <value></value>
        public string ReturnLinkHRef { get; }

        /// <summary>
        /// Rel
        /// </summary>
        /// <value></value>
        public string ReturnLinkRel { get; }

        /// <summary>
        /// Parameter
        /// </summary>
        /// <returns></returns>
        public List<ParameterApi> Parameters { get; } = new List<ParameterApi>();

        /// <summary>
        /// Retrun parameter
        /// </summary>
        /// <returns></returns>
        public List<ParameterApi> ReturnParameters { get; } = new List<ParameterApi>();

        /// <summary>
        /// Method Type
        /// </summary>
        /// <value></value>
        public string MethodType { get; }

        /// <summary>
        /// Is Get
        /// </summary>
        /// <returns></returns>
        public bool IsGet => MethodType.ToLower() == "get";

        /// <summary>
        /// Is Post
        /// </summary>
        /// <returns></returns>
        public bool IsPost => MethodType.ToLower() == "post";


        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public string GetMethodTypeHumanized()
        {
            var name = MethodType.ToLower();
            switch (name)
            {
                case "post": return "create";
                case "put": return "set";
                default: return name;
            }
        }

        /// <summary>
        /// Return type
        /// </summary>
        /// <value></value>
        public string ReturnType { get; }

        /// <summary>
        /// 
        /// </summary>
        public bool ReturnIsArray { get; }

        /// <summary>
        /// 
        /// </summary>
        public bool ReturnIsNull { get; }

        /// <summary>
        /// Method name
        /// </summary>
        /// <value></value>
        public string MethodName { get; }

        /// <summary>
        /// Comment
        /// </summary>
        /// <value></value>
        public string Comment { get; }

        /// <summary>
        /// Class Api
        /// </summary>
        /// <value></value>
        public ClassApi ClassApi { get; }
    }
}