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
 
using System.Linq;
using System.Text.RegularExpressions;

namespace Corsinvest.ProxmoxVE.Api.Shell.Utility
{
    /// <summary>
    /// Alias command.
    /// </summary>
    public class AliasDef
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="name"></param>
        /// <param name="description"></param>
        /// <param name="command"></param>
        /// <param name="system"></param>
        public AliasDef(string name, string description, string command, bool system)
        {
            Name = name;
            Description = description;
            Command = command;
            System = system;
        }

        /// <summary>
        /// Name
        /// </summary>
        /// <value></value>
        public string Name { get; }

        /// <summary>
        /// Description
        /// </summary>
        /// <value></value>
        public string Description { get; }

        /// <summary>
        /// Command
        /// </summary>
        /// <value></value>
        public string Command { get; }

        /// <summary>
        /// System
        /// </summary>
        /// <value></value>
        public bool System { get; }

        /// <summary>
        /// Check exists name or alias
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public bool Exists(string name)
        {
            foreach (var name1 in name.Split(','))
            {
                if (Names.Contains(name1)) { return true; }
            }

            return false;
        }

        /// <summary>
        /// Name alias
        /// </summary>
        /// <returns></returns>
        public string[] Names => Name.Split(',');

        /// <summary>
        /// Check name is valid
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public static bool IsValid(string name) => new Regex("^[a-zA-Z0-9,_-]*$").IsMatch(name);
    }
}