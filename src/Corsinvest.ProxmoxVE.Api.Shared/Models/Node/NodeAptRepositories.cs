/*
 * SPDX-FileCopyrightText: Copyright Corsinvest Srl
 * SPDX-License-Identifier: GPL-3.0-only
 */

using Newtonsoft.Json;

namespace Corsinvest.ProxmoxVE.Api.Shared.Models.Node;

/// <summary>
/// Node Apt Repositories
/// </summary>
public class NodeAptRepositories : ModelBase
{
    /// <summary>
    /// List of errors
    /// </summary>
    [JsonProperty("errors")]
    public List<object> Errors { get; set; }

    /// <summary>
    /// Repository files
    /// </summary>
    [JsonProperty("files")]
    public List<File> Files { get; set; }

    /// <summary>
    /// Standard repositories
    /// </summary>
    [JsonProperty("standard-repos")]
    public List<StandardRepository> StandardRepositories { get; set; }

    /// <summary>
    /// Additional information
    /// </summary>
    [JsonProperty("infos")]
    public List<Info> Infos { get; set; }

    /// <summary>
    /// Repository file
    /// </summary>
    public class File
    {
        /// <summary>
        /// Repositories contained in the file
        /// </summary>
        [JsonProperty("repositories")]
        public List<Repository> Repositories { get; set; }

        /// <summary>
        /// File type
        /// </summary>
        [JsonProperty("file-type")]
        public string FileType { get; set; }

        /// <summary>
        /// File path
        /// </summary>
        [JsonProperty("path")]
        public string Path { get; set; }
    }

    /// <summary>
    /// Additional information
    /// </summary>
    public class Info
    {
        /// <summary>
        /// Information kind
        /// </summary>
        [JsonProperty("kind")]
        public string Kind { get; set; }

        /// <summary>
        /// Message
        /// </summary>
        [JsonProperty("message")]
        public string Message { get; set; }

        /// <summary>
        /// File path
        /// </summary>
        [JsonProperty("path")]
        public string Path { get; set; }

        /// <summary>
        /// Index
        /// </summary>
        [JsonProperty("index")]
        public int Index { get; set; }

        /// <summary>
        /// Property
        /// </summary>
        [JsonProperty("property")]
        public string Property { get; set; }
    }

    /// <summary>
    /// Repository option
    /// </summary>
    public class Option
    {
        /// <summary>
        /// Option values
        /// </summary>
        [JsonProperty("Values")]
        public List<string> Values { get; set; }

        /// <summary>
        /// Option key
        /// </summary>
        [JsonProperty("Key")]
        public string Key { get; set; }
    }

    /// <summary>
    /// Repository
    /// </summary>
    public class Repository
    {
        /// <summary>
        /// Repository suites
        /// </summary>
        [JsonProperty("Suites")]
        public List<string> Suites { get; set; }

        /// <summary>
        /// Repository components
        /// </summary>
        [JsonProperty("Components")]
        public List<string> Components { get; set; }

        /// <summary>
        /// File type
        /// </summary>
        [JsonProperty("FileType")]
        public string FileType { get; set; }

        /// <summary>
        /// Repository types
        /// </summary>
        [JsonProperty("Types")]
        public List<string> Types { get; set; }

        /// <summary>
        /// Enabled
        /// </summary>
        [JsonProperty("Enabled")]
        public bool Enabled { get; set; }

        /// <summary>
        /// Repository URIs
        /// </summary>
        [JsonProperty("URIs")]
        public List<string> URIs { get; set; }

        /// <summary>
        /// Comment
        /// </summary>
        [JsonProperty("Comment")]
        public string Comment { get; set; }

        /// <summary>
        /// Repository options
        /// </summary>
        [JsonProperty("Options")]
        public List<Option> Options { get; set; }
    }

    /// <summary>
    /// Standard repository
    /// </summary>
    public class StandardRepository
    {
        /// <summary>
        /// Repository handle
        /// </summary>
        [JsonProperty("handle")]
        public string Handle { get; set; }

        /// <summary>
        /// Repository status
        /// </summary>
        [JsonProperty("status")]
        public bool Status { get; set; }

        /// <summary>
        /// Repository description
        /// </summary>
        [JsonProperty("description")]
        public string Description { get; set; }

        /// <summary>
        /// Repository name
        /// </summary>
        [JsonProperty("name")]
        public string Name { get; set; }
    }
}
