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

using System.ComponentModel.DataAnnotations;
using McMaster.Extensions.CommandLineUtils;
using McMaster.Extensions.CommandLineUtils.Validation;

namespace Corsinvest.ProxmoxVE.Api.Shell.Helpers
{
    /// <summary>
    /// Option depend validator
    /// </summary>
    public class OptionRequireDependOnValidator : IOptionValidator
    {
        private readonly string _optionName;
        private readonly CommandLineApplication _command;
        private readonly string _errorMessage;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="command"></param>
        /// <param name="optionName"></param>
        /// <param name="errorMessage"></param>
        public OptionRequireDependOnValidator(CommandLineApplication command,
                                              string optionName,
                                              string errorMessage = null)
        {
            _optionName = optionName;
            _command = command;
            _errorMessage = errorMessage;
        }

        /// <summary>
        /// Get validator
        /// </summary>
        /// <param name="option"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        public ValidationResult GetValidationResult(CommandOption option, ValidationContext context)
        {
            var depOpt = _command.GetOption(_optionName);
            if (depOpt == null) { return new ValidationResult($"The option depend on '{_optionName}' not exits!"); }

            if (option.HasValue())
            {
                if (!depOpt.HasValue()) { return new ValidationResult($"The --{_optionName} field is required!"); }
            }
            else
            {
               // return new ValidationResult($"The --{option.LongName} field is required!");
                if (depOpt.HasValue()) { return new ValidationResult($"The --{option.LongName} field is required!"); }
            }

            return ValidationResult.Success;
        }
    }
}