using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EnterpriseVE.ProxmoxVE.Api.Extension;
using EnterpriseVE.ProxmoxVE.Api.Utils;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace EnterpriseVE.ProxmoxVE.Api.Utils
{
    /// <summary>
    /// Create client class C#
    /// </summary>
    public class CreatorHelper
    {
        /// <summary>
        /// Read schema JSON and create client c#
        /// </summary>
        /// <param name="hostName"></param>
        /// <param name="port"></param>
        public static string ClientCSharp(string hostName, int port = ClientHelper.DEFAULT_PORT)
        {
            var json = JArray.Parse(ClientHelper.GetJsonSchemaFromApiDoc(hostName, port));
            var data = new StringBuilder();

            data.AppendLine(@"using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Linq;
using System.Dynamic;
using System.Web;

namespace EnterpriseVE.ProxmoxVE.Api {
#pragma warning disable 1591

/// <summary>
/// ProxmocVE Client
/// </summary>
public class Client {
private string _ticketCSRFPreventionToken;
private string _ticketPVEAuthCookie;
private Client _client;
private string _baseUrl;

public Client(string hostName, int port = 8006)
{
    _client = this;
    HostName = hostName;
    Port = port;
    _baseUrl = ""https://"" + hostName + "":"" + port + ""/api2/json"";
}

public string HostName {get; private set;}

public int Port {get; private set;}

public bool ThrowExceptionNoSuccess { get; set; } = false;

/// <summary>
/// Convert object to JSON.
/// </summary>
/// <param name=""obj""></param>
public static string ObjectToJson(object obj) { return JsonConvert.SerializeObject(obj, Formatting.Indented); }

/// <summary>
/// Creation ticket from login.
/// </summary>
/// <param name=""userName""></param>
/// <param name=""password""></param>
/// <param name=""realm""></param>
public bool Login(string userName, string password, string realm = ""pam"")
{
    dynamic ticket = Access.Ticket.CreateTicket(username: userName, password: password, realm: realm);
    _ticketCSRFPreventionToken = ticket.data.CSRFPreventionToken;
    _ticketPVEAuthCookie = ticket.data.ticket;
    return ticket != null;
}

private ExpandoObject Execute(string resource, HttpMethod method, IDictionary<string, object> parameters = null)
{
    using (var handler = new HttpClientHandler()
    {
        CookieContainer = new CookieContainer(),
        ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => { return true; }
    })
    using (var client = new HttpClient(handler))
    {
        client.BaseAddress = new Uri(_baseUrl);

        //load parameters
        var parms = new Dictionary<string, string>();
        if (parameters != null)
        {
            foreach (var parameter in parameters.Where(a => a.Value != null))
            {
                var value = parameter.Value;
                if (value is bool) { value = ((bool)value) ? 1 : 0; }
                parms.Add(parameter.Key, HttpUtility.UrlEncode(value.ToString()));
            }
        }

        var uriString = _baseUrl + resource;
        if (method == HttpMethod.Get && parms.Count > 0)
        {
            uriString += ""?"" + string.Join(""&"", (from a in parms
                                                    select $""{a.Key}={a.Value}""));
        }

        var request = new HttpRequestMessage(method, new Uri(uriString));
        if (method != HttpMethod.Get) { request.Content = new FormUrlEncodedContent(parms); }

        //tiket login
        if (_ticketCSRFPreventionToken != null)
        {
            handler.CookieContainer.Add(request.RequestUri, new Cookie(""PVEAuthCookie"", _ticketPVEAuthCookie));
            request.Headers.Add(""CSRFPreventionToken"", _ticketCSRFPreventionToken);
        }

        var response = client.SendAsync(request).Result;
        if (ThrowExceptionNoSuccess && !response.IsSuccessStatusCode) { throw new Exception(response.ReasonPhrase); }

        var stringContent = response.Content.ReadAsStringAsync().Result;
        dynamic result = JsonConvert.DeserializeObject<ExpandoObject>(stringContent);

        //check in error
        result.InError = ((IDictionary<String, object>)result).ContainsKey(""errors"");

        return result;
    }
}

private static void AddComplexParmeterToDictionary(Dictionary<string, object> parameters, string name, IDictionary<int, string> value)
{
    if (value == null) { return; }
    foreach (var item in value) { parameters.Add(name + item.Key, item.Value); }
}");

            var keys = new List<string>();
            foreach (var item in json) { NavigateItem(item, data, keys); }

            data.AppendLine("}").Append("}");

            return data.ToString();
        }

        private static string FixVarName(string name)
        {
            var ret = new StringBuilder();
            var names = name.Split('_');

            ret.Append(names[0]);
            for (int i = 1; i < names.Length; i++) { ret.Append(names[i].Capitalize()); }

            return ret.ToString();
        }

        private static void NavigateItem(JToken item, StringBuilder data, IList<string> keys)
        {
            var originalName = item["text"].ToString();
            var name = FixClassAndMethodName(originalName);
            var thisKey = originalName.Replace("{", "").Replace("}", "");

            var inIndex = false;
            // if item name
            if (name.StartsWith("{"))
            {
                keys.Add(thisKey);
                name = "Item" + thisKey.Capitalize();
                inIndex = true;
            }

            var className = $"PVE{name}";

            var parmsKeys = new StringBuilder();
            var keysCount = inIndex ? keys.Count - 1 : keys.Count;
            for (int i = 0; i < keysCount; i++) { parmsKeys.Append($",{keys[i]}: _{keys[i]}"); }

            if (!inIndex)
            {
                var varName = $"_{FixVarName(originalName)}";

                data.AppendLine($@"
private {className} {varName};
public {className} {FixClassAndMethodName(thisKey)} {{ get {{ return {varName} ?? ({varName} = new {className}(_client{parmsKeys.ToString()})); }} }}");
            }
            else
            {
                if (keys.Count > 0)
                {
                    //index key
                    parmsKeys.Append($",{keys.Last()}: {thisKey}");

                    data.AppendLine()
                        .AppendLine($"public {className} this[object {thisKey}] {{ get {{ return new {className}(_client{parmsKeys.ToString()}); }} }}");
                }
            }

            data.AppendLine($@"
public class {className} {{
private Client _client;");

            //key class
            foreach (var key in keys) { data.AppendLine($"private object _{key};"); }

            data.Append($"internal {className} (Client client");

            foreach (var key in keys) { data.Append($",object {key}"); }
            data.Append("){ _client = client;");
            if (keys.Count > 0) { data.AppendLine(); }
            foreach (var key in keys) { data.AppendLine($"_{key} = {key};"); }
            data.AppendLine("}");

            CreateMethod(item, data, keys);

            if (item["children"] != null)
            {
                foreach (var itemChild in item["children"]) { NavigateItem(itemChild, data, keys); }
            }

            data.AppendLine("}");

            if (inIndex) { keys.Remove(thisKey); }
        }

        private class Parameter
        {
            private static string FixNameParmameteForCSharp(string name)
            {
                var tmp = name.Replace("[n]", "N").Replace("-", "_");

                //reserved key
                foreach (var key in new string[] { "lock", "base", "default" })
                {
                    if (tmp == key)
                    {
                        tmp += "_";
                        break;
                    }
                }

                return tmp;
            }

            private static string FixCommentForCSharp(string comment)
            {
                return comment.Replace(Environment.NewLine, " ")
                              .Replace("<", "&lt;")
                              .Replace("<", "&gt;")
                              .Replace("&", "&amp;");
            }

            private static string FixTypeForCSharp(string type)
            {
                switch (type)
                {
                    case "boolean": return "bool";
                    case "integer": return "int";
                    case "number": return "int";
                    default: return "string";
                }
            }

            public Parameter(JToken parameter)
            {
                OriginalName = ((JProperty)parameter).Name;
                Name = FixNameParmameteForCSharp(OriginalName);
                Description = parameter.Parent[OriginalName]["description"] + "";
                OriginalType = parameter.Parent[OriginalName]["type"] + "";
                Type = FixTypeForCSharp(OriginalType);
                //    csharpDesc += Environment.NewLine + "/// Format: " + propFormatDef;
                Optional = (parameter.Parent[OriginalName]["optional"] ?? 0).ToString() == "1";

                //if Indexed collection
                if (Indexed) { Type = "IDictionary<int, string>"; }

                ParseFormat(parameter);

                ParseEnum(parameter);
            }

            private void ParseFormat(JToken parameter)
            {
                var parameters = new List<Parameter>();
                var format = parameter.Parent[OriginalName]["format"];
                if (format != null && format.HasValues && !format.ToString().StartsWith("pve-"))
                {
                    parameters = format.Select(a => new Parameter(a)).ToList();
                }
                Parameters = parameters.ToArray();
            }

            private void ParseEnum(JToken parameter)
            {
                var enumValues = new List<string>();
                var enumValue = parameter.Parent[OriginalName]["enum"];
                if (enumValue != null)
                {
                    foreach (var item in enumValue) { enumValues.Add(item.ToString()); }
                }
                EnumValues = enumValues.ToArray();
            }

            public string[] EnumValues { get; private set; }
            public Parameter[] Parameters { get; private set; }
            public string OriginalName { get; private set; }
            public string OriginalType { get; private set; }
            public string Name { get; private set; }
            public string Type { get; private set; }
            public string Description { get; private set; }
            public string Format { get; private set; }
            public bool Optional { get; private set; }
            public bool Indexed { get { return OriginalName.EndsWith("[n]"); } }

            public string GetParameter()
            {
                var type = Type;
                if (!Indexed && Optional && Type != "string") { type += "?"; }
                var ret = $"{type} {Name}";
                if (Optional) { ret += " = null"; }
                return ret;
            }

            public string GetProperty() { return $"public {Type} {Name} {{ get; set; }}"; }
            private string GetComment()
            {
                var comments = new StringBuilder();
                comments.Append(FixCommentForCSharp(Description));

                if (EnumValues.Count() > 0)
                {
                    comments.AppendLine()
                            .Append($"///   Enum: {string.Join(",", EnumValues)}");
                }

                foreach (var property in Parameters)
                {
                    comments.AppendLine()
                            .Append($"/// {property.Name} {property.GetComment()}");
                }
                if (Parameters.Count() > 0) { comments.Append("///"); }

                return comments.ToString();
            }
            public string GetCommentForParameter() { return $"/// <param name=\"{Name}\">{GetComment()}</param>"; }

            public string GetCommentForProperty()
            {
                if (string.IsNullOrWhiteSpace(Description))
                {
                    return "";
                }
                else
                {
                    return $@"
/// <summary>
/// {GetComment()}
/// </summary>";
                }
            }
        }

        private static string FixClassAndMethodName(string name) { return name.Capitalize().Replace("_", ""); }

        private static void CreateMethod(JToken item, StringBuilder data, IList<string> keys)
        {
            var infos = item["info"];
            foreach (var name in infos.Select(a => ((JProperty)a).Name))
            {
                var info = infos[name];
                var methodName = FixClassAndMethodName(info["name"].ToString());

                var returnType = "ExpandoObject";
                var returns = info["returns"];
                if (returns != null)
                {
                    // var createClassResult = false;
                    // if (createClassResult)
                    // {
                    //     var props = new List<Parameter>();
                    //     if (returns["properties"] != null)
                    //     {
                    //         props = returns["properties"].Select(a => new Parameter(a))
                    //                                      .OrderBy(a => a.OrigName)
                    //                                      .ToList();
                    //     }
                    //     else if (returns["items"] != null && returns["items"]["properties"] != null)
                    //     {
                    //         props = returns["items"]["properties"].Select(a => new Parameter(a))
                    //                                               .OrderBy(a => a.OrigName)
                    //                                               .ToList();
                    //     }

                    //     //create result class
                    //     if (props.Count > 0)
                    //     {
                    //         returnType = $"PVEResult{methodName}";

                    //         data.AppendLine($"public class {returnType} {{");

                    //         foreach (var prop in props)
                    //         {
                    //             var comment = prop.GetCommentForProperty();
                    //             if (!string.IsNullOrWhiteSpace(comment)) { data.AppendLine(comment); }
                    //             data.AppendLine(prop.GetProperty());
                    //         }

                    //         data.AppendLine("}");
                    //     }
                    // }

                    switch ((returns["type"] + ""))
                    {
                        //case "array": returnType += "[]"; break;
                        case "null": returnType = "void"; break;
                        default: break;
                    }
                }

                data.AppendLine($@"
/// <summary>
/// {info["description"]}
/// </summary>");

                var resource = item["path"].ToString();
                var parms = new List<Parameter>();
                if (info["parameters"] != null && info["parameters"]["properties"] != null)
                {
                    parms = info["parameters"]["properties"].Select(a => new Parameter(a)).ToList();

                    //remove keys
                    foreach (var key in keys)
                    {
                        resource = resource.Replace($"{{{key}}}", $"{{_{key}}}");
                        parms.Remove(parms.FirstOrDefault(a => a.OriginalName == key));
                    }

                    foreach (var comment in parms.OrderBy(a => a.Optional ? 1 : 0)
                                                 .ThenBy(a => a.OriginalName)
                                                 .Select(a => a.GetCommentForParameter()))
                    {
                        data.AppendLine(comment);
                    }
                }

                data.Append($"public {returnType} {methodName} (")
                    .Append(string.Join(",", parms.OrderBy(a => a.Optional ? 1 : 0)
                                                  .ThenBy(a => a.OriginalName)
                                                  .Select(a => a.GetParameter())))
                    .Append(") {");

                if (parms.Count > 0)
                {
                    data.AppendLine()
                        .AppendLine("var parameters = new Dictionary<string, object>();");

                    foreach (var parm in parms)
                    {
                        if (parm.Indexed)
                        {
                            data.AppendLine($@"AddComplexParmeterToDictionary(parameters, ""{parm.OriginalName.Replace("[n]", "")}"", {parm.Name});");
                        }
                        else
                        {
                            data.AppendLine($@"parameters.Add(""{parm.OriginalName}"", {parm.Name});");
                        }
                    }
                }

                data.Append($@" {(returnType == "void" ? "" : "return")} _client.Execute($""{resource}"",HttpMethod.{name.Capitalize()}");
                if (parms.Count > 0) { data.Append(",parameters"); }
                data.AppendLine(");}");
            }
        }
    }
}