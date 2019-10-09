/*
 * This file is part of the cv4pve-botgram https://github.com/Corsinvest/cv4pve-botgram,
 * Copyright (C) 2016 Corsinvest Srl
 *
 * This program is free software: you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation, either version 3 of the License, or
 * (at your option) any later version.
 *
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 *
 * You should have received a copy of the GNU General Public License
 * along with this program.  If not, see <http://www.gnu.org/licenses/>.
 */
 
using System.Linq;
using System.Text.RegularExpressions;

namespace Corsinvest.ProxmoxVE.Api.Extension.Shell
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