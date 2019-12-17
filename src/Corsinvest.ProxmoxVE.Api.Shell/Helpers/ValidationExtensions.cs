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

using McMaster.Extensions.CommandLineUtils;

namespace Corsinvest.ProxmoxVE.Api.Shell.Helpers
{
    /// <summary>
    /// Extension methods for adding validation rules to options and arguments.
    /// </summary>
    public static class ValidationExtensions
    {
        /// <summary>
        /// Depend on
        /// </summary>
        /// <param name="option"></param>
        /// <param name="command"></param>
        /// <param name="optionName"></param>
        /// <param name="errorMessage"></param>
        /// <returns></returns>
        public static CommandOption DependOn(this CommandOption option,
                                             CommandLineApplication command,
                                             string optionName,
                                             string errorMessage = null)
        {
            if (!string.IsNullOrWhiteSpace(optionName))
            {
                option.Validators.Add(new OptionRequireDependOnValidator(command, optionName, errorMessage));
            }
            return option;
        }

        /// <summary>
        /// Depend on
        /// </summary>
        /// <param name="option"></param>
        /// <param name="command"></param>
        /// <param name="optionName"></param>
        /// <param name="errorMessage"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static CommandOption<T> DependOn<T>(this CommandOption<T> option,
                                                CommandLineApplication command,
                                                string optionName,
                                                string errorMessage = null)
        {
            if (!string.IsNullOrWhiteSpace(optionName))
            {
                option.Validators.Add(new OptionRequireDependOnValidator(command, optionName, errorMessage));
            }
            return option;
        }
    }
}