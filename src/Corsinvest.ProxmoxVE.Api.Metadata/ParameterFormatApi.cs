using System.Collections.Generic;
using Newtonsoft.Json.Linq;

namespace Corsinvest.ProxmoxVE.Api.Metadata
{
    /// <summary>
    /// Format parameter
    /// </summary>
    public class ParameterFormatApi
    {
        /// <summary>
        /// Costructor
        /// </summary>
        /// <param name="token"></param>
        public ParameterFormatApi(JToken token)
        {
            Name = ((JProperty)token.Parent).Name;
            Description = token["description"] + "";
            Optional = (token["optional"] ?? 0).ToString() == "1";
            Type = token["type"] + "";
            Maximum = token["maximum"] == null ? null : (int?)token["maximum"];
            Minimum = token["minimum"] == null ? null : (int?)token["maximum"];
            DefaultKey = token["default_key"] + "";
            FormatDescription = token["format_description"] + "";
            Format = token["format"] + "";
            Alias = token["alias"] + "";
            MaxLength = token["maxLength"] == null ? null : (int?)token["maxLength"];

            #region create enum values
            var enumValues = new List<string>();
            if (token["enum"] != null)
            {
                foreach (var item in token["enum"]) { enumValues.Add(item.ToString()); }
            }
            EnumValues = enumValues.ToArray();
            #endregion
        }

        /// <summary>
        /// Enum values
        /// </summary>
        /// <value></value>
        public string[] EnumValues { get; }

        /// <summary>
        /// Name
        /// </summary>
        /// <value></value>
        public string Name { get; }

        /// <summary>
        /// Type
        /// </summary>
        /// <value></value>
        public string Type { get; }

        /// <summary>
        /// Comment
        /// </summary>
        /// <value></value>
        public string Description { get; }

        /// <summary>
        /// Optional
        /// </summary>
        /// <value></value>
        public bool Optional { get; }

        /// <summary>
        /// Minimum
        /// </summary>
        /// <value></value>
        public int? Minimum { get; }

        /// <summary>
        /// Default Key
        /// </summary>
        /// <value></value>
        public string DefaultKey { get; }

        /// <summary>
        /// Format description
        /// </summary>
        /// <value></value>
        public string FormatDescription { get; }

        /// <summary>
        /// Format
        /// </summary>
        /// <value></value>
        public string Format { get; }

        /// <summary>
        /// Alias
        /// </summary>
        /// <value></value>
        public string Alias { get; }

        /// <summary>
        /// Max length
        /// </summary>
        /// <value></value>
        public int? MaxLength { get; }

        /// <summary>
        /// Massimum
        /// </summary>
        /// <value></value>
        public int? Maximum { get; }
    }
}