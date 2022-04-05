/*
 * SPDX-FileCopyrightText: 2019 Daniele Corsini <daniele.corsini@corsinvest.it>
 * SPDX-FileCopyrightText: Copyright Corsinvest Srl
 * SPDX-License-Identifier: GPL-3.0-only
 */

using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace Corsinvest.ProxmoxVE.Api.Metadata
{
    /// <summary>
    /// Class Api
    /// </summary>
    public class ClassApi
    {
        /// <summary>
        /// Constructor
        /// </summary>
        public ClassApi() { IsRoot = true; }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="token"></param>
        /// <param name="parent"></param>
        public ClassApi(JToken token, ClassApi parent)
        {
            Name = token["text"].ToString();
            IndexName = Name.Replace("{", "").Replace("}", "");
            Parent = parent;
            Resource = token["path"].ToString();

            Keys.AddRange(parent.Keys);
            parent.SubClasses.Add(this);

            IsIndexed = token["text"].ToString().StartsWith("{");
            if (IsIndexed) { Keys.Add(IndexName); }

            if (token["info"] != null)
            {
                Methods.AddRange(token["info"].Select(a => new MethodApi(a.Parent[((JProperty)a).Name], this)).ToArray());
            }

            //tree children
            if (token["children"] != null)
            {
                foreach (var child in token["children"]) { _ = new ClassApi(child, this); }
            }
        }

        /// <summary>
        /// Get From Resource
        /// </summary>
        /// <param name="host"></param>
        /// <param name="port"></param>
        /// <param name="resource"></param>
        /// <returns></returns>
        public static async Task<ClassApi> GetFromResource(string host, int port, string resource)
            => GetFromResource(await GeneretorClassApi.Generate(host, port), resource);

        /// <summary>
        /// Get From Resource
        /// </summary>
        /// <param name="classApiRoot"></param>
        /// <param name="resource"></param>
        /// <returns></returns>
        public static ClassApi GetFromResource(ClassApi classApiRoot, string resource)
        {
            var classApi = classApiRoot;
            foreach (var item in resource.Split('/'))
            {
                if (string.IsNullOrWhiteSpace(item)) { continue; }
                var oldClassApi = classApi;
                classApi = classApi.SubClasses.FirstOrDefault(a => a.Resource == classApi.Resource + "/" + item);
                if (classApi == null) { classApi = oldClassApi.SubClasses.FirstOrDefault(a => a.IsIndexed); }
                if (classApi == null) { break; }
            }

            return classApi;
        }

        /// <summary>
        /// Is Root
        /// </summary>
        /// <value></value>
        public bool IsRoot { get; }

        /// <summary>
        /// Name of class
        /// </summary>
        /// <value></value>
        public string Name { get; }

        /// <summary>
        /// Index Name
        /// </summary>
        /// <returns></returns>
        public string IndexName { get; }

        /// <summary>
        /// Parent class
        /// </summary>
        /// <value></value>
        public ClassApi Parent { get; }

        /// <summary>
        /// Sub classes
        /// </summary>
        /// <returns></returns>
        public IList<ClassApi> SubClasses { get; } = new List<ClassApi>();

        /// <summary>
        /// Methods
        /// </summary>
        /// <returns></returns>
        public List<MethodApi> Methods { get; } = new List<MethodApi>();

        /// <summary>
        /// Keys class
        /// </summary>
        /// <returns></returns>
        public List<string> Keys { get; } = new List<string>();

        /// <summary>
        /// Is Index
        /// </summary>
        /// <value></value>
        public bool IsIndexed { get; }

        /// <summary>
        /// Path resource
        /// </summary>
        /// <value></value>
        public string Resource { get; }
    }
}