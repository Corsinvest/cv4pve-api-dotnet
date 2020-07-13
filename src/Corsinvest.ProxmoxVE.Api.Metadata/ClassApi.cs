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

using System.Collections.Generic;
using System.Linq;
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
                foreach (var child in token["children"]) { new ClassApi(child, this); }
            }
        }

        /// <summary>
        /// Get From Resource
        /// </summary>
        /// <param name="host"></param>
        /// <param name="port"></param>
        /// <param name="resource"></param>
        /// <returns></returns>
        public static ClassApi GetFromResource(string host, int port, string resource)
            => GetFromResource(GeneretorClassApi.Generate(host, port), resource);

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
                classApi = classApi.SubClasses.Where(a => a.Resource == classApi.Resource + "/" + item).FirstOrDefault();
                if (classApi == null) { classApi = oldClassApi.SubClasses.Where(a => a.IsIndexed).FirstOrDefault(); }
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