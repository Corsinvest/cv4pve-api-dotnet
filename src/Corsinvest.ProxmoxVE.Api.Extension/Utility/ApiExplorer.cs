/*
 * This file is part of the cv4pve-api-dotnet https://github.com/Corsinvest/cv4pve-api-dotnet,
 *
 * This source file is available under two different licenses:
 * - GNU General Public License version 3 (GPLv3)
 * - Corsinvest Enterprise License (CEL)
 * Full copyright and license information is available in
 * LICENSE.md which is distributed with this source code.
 *
 * Copyright (C) 2016 Corsinvest Srl	GPLv3 and CEL
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Corsinvest.ProxmoxVE.Api.Extension.Helpers;
using Corsinvest.ProxmoxVE.Api.Metadata;
using Newtonsoft.Json;

namespace Corsinvest.ProxmoxVE.Api.Extension.Utility
{
    /// <summary>
    /// Api Explorer
    /// </summary>
    public static class ApiExplorer
    {
        /// <summary>
        /// Output type
        /// </summary>
        public enum OutputType
        {
            /// <summary>
            /// Text
            /// </summary>
            Text,

            /// <summary>
            /// Unicode
            /// </summary>
            Unicode,

            /// <summary>
            /// Unicode
            /// </summary>
            UnicodeAlt,

            /// <summary>
            /// Markdown
            /// </summary>
            Markdown,

            /// <summary>
            /// Json
            /// </summary>
            Json,

            /// <summary>
            /// Json pretty
            /// </summary>
            JsonPretty,

            /// <summary>
            /// Html
            /// </summary>
            Html,

            /// <summary>
            /// PNG image
            /// </summary>
            Png
        }

        /// <summary>
        /// Create parameter resource split ':'
        /// </summary>
        /// <param name="items"></param>
        /// <returns></returns>
        public static Dictionary<string, object> CreateParameterResource(IEnumerable<string> items)
        {
            var parameters = new Dictionary<string, object>();
            foreach (var item in items)
            {
                var pos = item.IndexOf(":");
                parameters.Add(item.Substring(0, pos), item.Substring(pos + 1));
            }
            return parameters;
        }

        private static TableOutputType DecodeOutputType(OutputType output)
            => output switch
            {
                OutputType.Text => TableOutputType.Text,
                OutputType.Html => TableOutputType.Html,
                OutputType.Unicode => TableOutputType.Unicode,
                OutputType.UnicodeAlt => TableOutputType.UnicodeAlt,
                OutputType.Markdown => TableOutputType.Markdown,
                _ => TableOutputType.Text,
            };

        /// <summary>
        /// Execute methods
        /// </summary>
        /// <param name="client"></param>
        /// <param name="classApiRoot"></param>
        /// <param name="resource"></param>
        /// <param name="methodType"></param>
        /// <param name="parameters"></param>
        /// <param name="wait"></param>
        /// <param name="outputType"></param>
        /// <param name="verbose"></param>
        public static (int ResultCode, string ResultText) Execute(PveClient client,
                                                                  ClassApi classApiRoot,
                                                                  string resource,
                                                                  MethodType methodType,
                                                                  Dictionary<string, object> parameters,
                                                                  bool wait = false,
                                                                  OutputType outputType = OutputType.Unicode,
                                                                  bool verbose = false)
        {
            var currResponseType = client.ResponseType;
            if (outputType == OutputType.Png) { client.ResponseType = ResponseType.Png; }

            //create result
            Result result = null;
            switch (methodType)
            {
                case MethodType.Get: result = client.Get(resource, parameters); break;
                case MethodType.Set: result = client.Set(resource, parameters); break;
                case MethodType.Create: result = client.Create(resource, parameters); break;
                case MethodType.Delete: result = client.Delete(resource, parameters); break;
            }

            //restore prev ResponseType
            currResponseType = client.ResponseType;

            var resultText = new StringBuilder();
            if (result != null && !result.IsSuccessStatusCode)
            {
                resultText.AppendLine(result.ReasonPhrase);
                resultText.AppendLine(verbose
                                        ? JsonConvert.SerializeObject((string)result.Response.errors, Formatting.Indented)
                                        : result.GetError());
            }
            else if (result.InError())
            {
                resultText.AppendLine(result.ReasonPhrase);
            }
            else
            {
                //print result
                if (verbose)
                {
                    //verbose full response json
                    resultText.AppendLine(JsonConvert.SerializeObject(result.Response, Formatting.Indented));
                }
                else
                {
                    switch (outputType)
                    {
                        case OutputType.Png: resultText.AppendLine(result.Response); break;

                        case OutputType.Json:
                        case OutputType.JsonPretty:
                            resultText.AppendLine(JsonConvert.SerializeObject(result.Response.data,
                                                                              outputType == OutputType.JsonPretty
                                                                                ? Formatting.Indented
                                                                                : Formatting.None));
                            break;

                        case OutputType.Text:
                        case OutputType.Html:
                        case OutputType.Unicode:
                        case OutputType.UnicodeAlt:
                        case OutputType.Markdown:
                            var data = result.Response.data;

                            var tableOutput = DecodeOutputType(outputType);
                            var classApi = ClassApi.GetFromResource(classApiRoot, resource);
                            if (classApi == null)
                            {
                                resultText.AppendLine($"no such resource '{resource}'");
                            }
                            else if (data != null)
                            {
                                var returnParameters = classApi.Methods
                                                               .Where(a => a.IsGet)
                                                               .FirstOrDefault()
                                                               ?.ReturnParameters;

                                if (returnParameters == null || returnParameters.Count == 0)
                                {
                                    //no return defined
                                    resultText.Append(TableHelper.Create(data,
                                                                         null,
                                                                         false,
                                                                         tableOutput,
                                                                         null));
                                }
                                else
                                {
                                    var keys = returnParameters.OrderBy(a => a.Optional)
                                                               .ThenBy(a => a.Name)
                                                               .Select(a => a.Name)
                                                               .ToArray();

                                    resultText.Append(TableHelper.Create(data,
                                                                         keys,
                                                                         false,
                                                                         tableOutput,
                                                                         returnParameters));
                                }
                            }
                            break;

                        default: break;
                    }
                }

                if (wait)
                {
                    var task = (string)result.Response.data;
                    client.WaitForTaskToFinish(task.Split(':')[1], task, 1000, 30000);
                }
            }

            return ((int)result.StatusCode, resultText.ToString());
        }

        private static void CreateTable(IEnumerable<ParameterApi> parameters,
                                        StringBuilder resultText,
                                        OutputType outputType)
        {
            if (parameters.Count() > 0)
            {
                var values = new List<object[]>();
                foreach (var param in parameters)
                {
                    var partsComment = JoinWord(param.Description
                                                     .Replace(StringHelper.NewLineUnix + "", " ")
                                                     .Trim()
                                                     .Split(new[] { " " }, StringSplitOptions.None), 45, " ");

                    //type
                    var partsType = new[] { param.Type };
                    if (!string.IsNullOrWhiteSpace(param.TypeText))
                    {
                        //explicit text
                        partsType = JoinWord(param.TypeText.Split(' '), 18, "");
                    }
                    else if (param.EnumValues.Count() > 0)
                    {
                        //enums
                        partsType = JoinWord(param.EnumValues, 18, ",");
                    }

                    for (var i = 0; i < Math.Max(partsType.Length, partsComment.Length); i++)
                    {
                        values.Add(new[] { i == 0 ? param.Name : "",
                                           i < partsType.Length ? partsType[i] : "",
                                           i < partsComment.Length ? partsComment[i] : "" });
                    }
                }

                resultText.Append(TableHelper.Create(new[] { "param", "type", "description" },
                                                     values,
                                                     DecodeOutputType(outputType),
                                                     false));
            }
        }

        /// <summary>
        /// Usage resource
        /// </summary>
        /// <param name="classApiRoot"></param>
        /// <param name="resource"></param>
        /// <param name="outputType"></param>
        /// <param name="returnsType"></param>
        /// <param name="command"></param>
        /// <param name="verbose"></param>
        /// <returns></returns>
        public static string Usage(ClassApi classApiRoot,
                                   string resource,
                                   OutputType outputType,
                                   bool returnsType = false,
                                   string command = null,
                                   bool verbose = false)
        {
            var ret = new StringBuilder();
            var classApi = ClassApi.GetFromResource(classApiRoot, resource);
            if (classApi == null)
            {
                ret.AppendLine($"no such resource '{resource}'");
            }
            else
            {
                foreach (var method in classApi.Methods.OrderBy(a => a.MethodType))
                {
                    //exclude other command
                    if (!string.IsNullOrWhiteSpace(command) && method.GetMethodTypeHumanized().ToLower() != command)
                    {
                        continue;
                    }

                    ret.Append($"USAGE: {method.GetMethodTypeHumanized()} {resource}");

                    //only parameters no keys
                    var parameters = method.Parameters.Where(a => !classApi.Keys.Contains(a.Name));

                    var opts = string.Join("", parameters.Where(a => !a.Optional).Select(a => $" {a.Name}:<{a.Type}>"));
                    if (!string.IsNullOrWhiteSpace(opts)) { ret.Append(opts); }

                    //optional parameter
                    if (parameters.Where(a => a.Optional).Count() > 0) { ret.Append(" [OPTIONS]"); }

                    ret.AppendLine();

                    if (verbose)
                    {
                        ret.AppendLine().AppendLine("  " + method.Comment);
                        CreateTable(parameters, ret, outputType);
                    }

                    if (returnsType)
                    {
                        //show returns
                        ret.AppendLine("RETURNS:");
                        CreateTable(method.ReturnParameters, ret, outputType);
                    }

                    if (verbose) { ret.AppendLine(); }
                }
            }

            return ret.ToString();
        }

        private static string[] JoinWord(string[] words, int numChar, string separator)
        {
            var ret = new List<string>();
            var line = "";
            foreach (var item in words)
            {
                if (!string.IsNullOrWhiteSpace(line)) { line += separator; }
                line += item;
                if (line.Length >= numChar)
                {
                    ret.Add(line.Trim());
                    line = "";
                }
            }

            if (!string.IsNullOrWhiteSpace(line)) { ret.Add(line.Trim()); }
            return ret.ToArray();
        }

        /// <summary>
        /// List values resource
        /// </summary>
        /// <param name="client"></param>
        /// <param name="classApiRoot"></param>
        /// <param name="resource"></param>
        /// <returns></returns>
        public static (IEnumerable<(string Attribute, string Value)> Values, string Error) ListValues(PveClient client,
                                                                                                      ClassApi classApiRoot,
                                                                                                      string resource)
        {
            var values = new List<(string Attribute, string Value)>();
            var error = "";

            var classApi = ClassApi.GetFromResource(classApiRoot, resource);
            if (classApi == null)
            {
                error = $"no such resource '{resource}'";
            }
            else
            {
                if (classApi.SubClasses.Count == 0)
                {
                    error = $"resource '{resource}' does not define child links";
                }
                else
                {
                    string key = null;
                    foreach (var subClass in classApi.SubClasses.OrderBy(a => a.Name))
                    {
                        var attribute = string.Join("",
                                                    new[] { subClass.SubClasses.Count > 0 ? "D" : "-",
                                                            "r--",
                                                            subClass.Methods.Any(a => a.IsPost)? "c" : "-"});

                        if (subClass.IsIndexed)
                        {
                            var result = client.Get(resource);
                            if (result.InError())
                            {
                                error = result.GetError();
                            }
                            else
                            {
                                if (key == null)
                                {
                                    var returnLinkHRef = classApi.Methods.Where(a => a.IsGet)
                                                                         .FirstOrDefault()
                                                                         .ReturnLinkHRef;
                                    if (!string.IsNullOrWhiteSpace(returnLinkHRef))
                                    {
                                        key = returnLinkHRef.Replace("{", "").Replace("}", "");
                                    }
                                }

                                if (result.Response.data != null && !string.IsNullOrWhiteSpace(key))
                                {
                                    var data = new List<object>();
                                    foreach (IDictionary<string, object> item in result.Response.data) { data.Add(item[key]); }
                                    foreach (var item in data.OrderBy(a => a)) { values.Add((attribute, item + "")); }
                                }
                            }
                        }
                        else
                        {
                            values.Add((attribute, subClass.Name));
                        }
                    }
                }
            }

            return (values, error);
        }

        /// <summary>
        /// List structure
        /// </summary>
        /// <param name="client"></param>
        /// <param name="classApiRoot"></param>
        /// <param name="resource"></param>
        /// <returns></returns>
        public static string List(PveClient client, ClassApi classApiRoot, string resource)
        {
            var (Values, Error) = ListValues(client, classApiRoot, resource);
            return string.Join(Environment.NewLine, Values.Select(a => $"{a.Attribute}        {a.Value}")) +
                   (string.IsNullOrWhiteSpace(Error) ? "" : Environment.NewLine + Error) +
                   Environment.NewLine;
        }
    }
}