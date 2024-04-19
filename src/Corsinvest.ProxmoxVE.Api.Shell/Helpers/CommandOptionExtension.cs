/*
 * SPDX-FileCopyrightText: Copyright Corsinvest Srl
 * SPDX-License-Identifier: GPL-3.0-only
 */

using Corsinvest.ProxmoxVE.Api.Extension.Utils;
using Corsinvest.ProxmoxVE.Api.Shared.Utils;
using Microsoft.Extensions.Logging;
using System;
using System.CommandLine;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Corsinvest.ProxmoxVE.Api.Shell.Helpers;

/// <summary>
/// Command option shell extension.
/// </summary>
public static class CommandOptionExtension
{
    /// <summary>
    /// Add fullName and logo
    /// </summary>
    /// <param name="command"></param>
    public static void AddFullNameLogo(this Command command)
    {
        command.Description = ConsoleHelper.MakeLogoAndTitle(command.Description) + $@"

{command.Name} is a part of suite cv4pve.
For more information visit https://www.corsinvest.it/cv4pve";
    }

    /// <summary>
    /// Get option
    /// </summary>
    /// <param name="command"></param>
    /// <param name="name"></param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public static Option<T> GetOption<T>(this Command command, string name)
        => (Option<T>)command.Options.FirstOrDefault(a => a.Name == name || a.Aliases.Contains(name));

    /// <summary>
    /// Dry run is active
    /// </summary>
    /// <param name="command"></param>
    /// <returns></returns>
    public static bool DryRunIsActive(this Command command) => command.GetOption<bool>(DryRunOptionName).GetValue();

    /// <summary>
    /// Debug is active
    /// </summary>
    /// <param name="command"></param>
    /// <returns></returns>
    public static bool DebugIsActive(this Command command) => command.GetOption<bool>(DebugOptionName).GetValue();

    /// <summary>
    /// Get LogLevel from debug
    /// </summary>
    /// <param name="command"></param>
    /// <returns></returns>
    public static LogLevel GetLogLevelFromDebug(this Command command)
        => command.DebugIsActive()
            ? LogLevel.Trace
            : LogLevel.Warning;

    /// <summary>
    /// Debug option
    /// </summary>
    /// <param name="command"></param>
    /// <returns></returns>
    public static Option<bool> DebugOption(this Command command)
    {
        var opt = new Option<bool>($"--{DebugOptionName}", "Debug application")
        {
            IsHidden = true
        };
        command.AddGlobalOption(opt);
        return opt;
    }

    /// <summary>
    /// Dry run option
    /// </summary>
    /// <param name="command"></param>
    /// <returns></returns>
    public static Option<bool> DryRunOption(this Command command)
    {
        var opt = new Option<bool>($"--{DryRunOptionName}", "Dry run application")
        {
            IsHidden = true,
        };
        command.AddGlobalOption(opt);
        return opt;
    }

    /// <summary>
    /// Get Value from option
    /// </summary>
    /// <param name="option"></param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    private static T GetValue<T>(this Option<T> option) => option.Parse(Environment.GetCommandLineArgs()).GetValueForOption(option);

    // /// <summary>
    // /// Get value
    // /// </summary>
    // /// <param name="argument"></param>
    // /// <typeparam name="T"></typeparam>
    // /// <returns></returns>
    // public static T GetValue<T>(this Argument<T> argument) => argument.Parse(Environment.GetCommandLineArgs()).GetValueForArgument(argument);

    /// <summary>
    /// Return if a option has value
    /// </summary>
    /// <param name="option"></param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public static bool HasValue<T>(this Option<T> option) => option.GetValue() != null;

    /// <summary>
    /// Id or name option
    /// </summary>
    /// <param name="command"></param>
    /// <returns></returns>
    public static Option<string> VmIdOrNameOption(this Command command) => command.AddOption<string>("--vmid", "The id or name VM/CT");

    /// <summary>
    /// Ids or names option
    /// </summary>
    /// <param name="command"></param>
    /// <returns></returns>
    public static Option<string> VmIdsOrNamesOption(this Command command)
    {
        var opt = command.VmIdOrNameOption();
        opt.Description = @"The id or name VM/CT comma separated (eg. 100,101,102,TestDebian)
-vmid,-name,-@node-???,-@tag-?? exclude from list (e.g. @all,-200,-TestUbuntu,-@tag-customer1)
range 100:107,-105,200:204
'@pool-???' for all VM/CT in specific pool (e.g. @pool-customer1),
'@tag-???' for all VM/CT in specific tags (e.g. @tag-customerA),
'@node-???' for all VM/CT in specific node (e.g. @node-pve1, @node-\$(hostname)),
'@all-???' for all VM/CT in specific host (e.g. @all-pve1, @all-\$(hostname)),
'@all' for all VM/CT in cluster";

        return opt;
    }

    /// <summary>
    /// Get Api token option
    /// </summary>
    /// <param name="command"></param>
    /// <returns></returns>
    public static Option<string> GetApiTokenOption(this Command command) => command.GetOption<string>(ApiTokenOptionName);

    /// <summary>
    /// Get Validate Certificate option
    /// </summary>
    /// <param name="command"></param>
    /// <returns></returns>
    public static Option<bool> GetValidateCertificateOption(this Command command) => command.GetOption<bool>(ValidateCertificateOptionName);

    /// <summary>
    /// Get username option
    /// </summary>
    /// <param name="command"></param>
    /// <returns></returns>
    public static Option<string> GetUsernameOption(this Command command) => command.GetOption<string>(UsernameOptionName);

    /// <summary>
    /// Get password option
    /// </summary>
    /// <param name="command"></param>
    /// <returns></returns>
    public static Option<string> GetPasswordOption(this Command command) => command.GetOption<string>(PasswordOptionName);

    /// <summary>
    /// Get host option
    /// </summary>
    /// <param name="command"></param>
    /// <returns></returns>
    public static Option<string> GetHostOption(this Command command) => command.GetOption<string>(HostOptionName);

    #region Login option
    /// <summary>
    /// Add options login
    /// </summary>
    /// <param name="command"></param>
    public static void AddLoginOptions(this Command command)
    {
        var optApiToken = command.AddOption<string>($"--{ApiTokenOptionName}", "Api token format 'USER@REALM!TOKENID=UUID'. Require Proxmox VE 6.2 or later");
        var optUsername = command.AddOption<string>($"--{UsernameOptionName}", "User name <username>@<realm>");
        var optPassword = command.AddOption<string>($"--{PasswordOptionName}", "The password. Specify 'file:path_file' to store password in file.");
        command.ValidateCertificateOption();

        var optHost = new Option<string>($"--{HostOptionName}",
                                         parseArgument: (e) =>
                                         {
                                             if (e.FindResultFor(optApiToken) == null && e.FindResultFor(optUsername) == null)
                                             {
                                                 e.ErrorMessage = $"Option '--{optUsername.Name}' or '--{optApiToken.Name}' is required!";
                                             }
                                             else if (e.FindResultFor(optUsername) != null && e.FindResultFor(optPassword) == null)
                                             {
                                                 e.ErrorMessage = $"Option '--{optPassword.Name}' is required!";
                                             }

                                             return e.Tokens.Single().Value;
                                         }, description: "The host name host[:port],host1[:port],host2[:port]")
        {
            IsRequired = true
        };
        command.AddOption(optHost);
    }

    /// <summary>
    /// Api Token
    /// </summary>
    public static readonly string ApiTokenOptionName = "api-token";

    /// <summary>
    /// Validate Certificate
    /// </summary>
    public static readonly string ValidateCertificateOptionName = "validate-certificate";

    /// <summary>
    /// Host option
    /// </summary>
    public static readonly string HostOptionName = "host";

    /// <summary>
    /// Username option
    /// </summary>
    public static readonly string UsernameOptionName = "username";

    /// <summary>
    /// Password option
    /// </summary>
    public static readonly string PasswordOptionName = "password";

    /// <summary>
    /// Password option
    /// </summary>
    public static readonly string DebugOptionName = "debug";

    /// <summary>
    /// Dry run
    /// </summary>
    public static readonly string DryRunOptionName = "dry-run";

    /// <summary>
    /// Host option
    /// </summary>
    /// <param name="command"></param>
    /// <returns></returns>
    public static Option<string> HostOption(this Command command)
    {
        var option = command.AddOption<string>($"--{HostOptionName}", "The host name host[:port],host1[:port],host2[:port]");
        option.IsRequired = true;
        return option;
    }

    /// <summary>
    /// Table output enum
    /// </summary>
    /// <param name="command"></param>
    /// <returns></returns>
    public static Option<TableGenerator.Output> TableOutputOption(this Command command)
    {
        var opt = command.AddOption<TableGenerator.Output>("--output|-o", "Type output");
        opt.SetDefaultValue(TableGenerator.Output.Text);
        return opt;
    }

    /// <summary>
    /// username options
    /// </summary>
    /// <param name="command"></param>
    /// <returns></returns>
    public static Option<string> UsernameOption(this Command command) => command.AddOption<string>($"--{UsernameOptionName}", "User name");

    /// <summary>
    /// Verbose
    /// </summary>
    /// <param name="command"></param>
    /// <returns></returns>
    public static Option<bool> VerboseOption(this Command command) => command.AddOption<bool>($"--verbose|-v", "Verbose.");

    /// <summary>
    /// Validate Certificate
    /// </summary>
    /// <param name="command"></param>
    /// <returns></returns>
    public static Option<bool> ValidateCertificateOption(this Command command)
        => command.AddOption<bool>($"--{ValidateCertificateOptionName}", "Validate SSL Certificate Proxmox VE node.");

    /// <summary>
    /// Try login client api
    /// </summary>
    /// <param name="command"></param>
    /// <param name="loggerFactory"></param>
    /// <returns></returns>
    public static async Task<PveClient> ClientTryLoginAsync(this Command command, ILoggerFactory loggerFactory)
    {
        var inApiToken = command.GetApiTokenOption().HasValue();
        return await ClientHelper.GetClientAndTryLoginAsync(command.GetHostOption().GetValue(),
                                                            inApiToken ? string.Empty : command.GetUsernameOption().GetValue(),
                                                            inApiToken ? string.Empty : command.GetPasswordOption().GetValue(),
                                                            inApiToken ? command.GetApiTokenOption().GetValue() : string.Empty,
                                                            command.GetValidateCertificateOption().GetValue(),
                                                            loggerFactory);
    }

    /// <summary>
    /// Return password from option
    /// </summary>
    /// <param name="command"></param>
    /// <returns></returns>
    public static string GetPasswordFromOption(this Command command)
    {
        const string key = "012345678901234567890123";

        var password = command.GetPasswordOption().GetValue();
        if (!string.IsNullOrWhiteSpace(password))
        {
            password = password.Trim();

            //check if file
            if (password.StartsWith("file:"))
            {
                var fileName = password[5..];
                if (File.Exists(fileName))
                {
                    password = StringHelper.Decrypt(File.ReadAllText(fileName, Encoding.UTF8), key);
                }
                else
                {
                    Console.WriteLine("Password:");
                    password = ConsoleHelper.ReadPassword();
                    File.WriteAllText(fileName, StringHelper.Encrypt(password, key), Encoding.UTF8);
                }
            }
        }

        return password;
    }
    #endregion

    /// <summary>
    /// Vm Id option
    /// </summary>
    /// <param name="command"></param>
    /// <returns></returns>
    public static Option<int> VmIdOption(this Command command) => command.AddOption<int>("--vmid", "The id VM/CT");

    /// <summary>
    /// Add argument
    /// </summary>
    /// <param name="command"></param>
    /// <param name="name"></param>
    /// <param name="description"></param>
    /// <returns></returns>
    public static Argument<string> AddArgument(this Command command, string name, string description)
        => command.AddArgument<string>(name, description);

    /// <summary>
    /// Add argument
    /// </summary>
    /// <param name="command"></param>
    /// <param name="name"></param>
    /// <param name="description"></param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public static Argument<T> AddArgument<T>(this Command command, string name, string description)
    {
        var argument = new Argument<T>(name, description);
        command.AddArgument(argument);
        return argument;
    }

    /// <summary>
    /// Add command
    /// </summary>
    /// <param name="command"></param>
    /// <param name="name"></param>
    /// <param name="description"></param>
    /// <returns></returns>
    public static Command AddCommand(this Command command, string name, string description)
    {
        var cmd = CreateObject<Command>(name, description);
        command.AddCommand(cmd);
        return cmd;
    }

    private static T CreateObject<T>(string name, string description) where T : IdentifierSymbol
    {
        var names = name.Split("|");
        var obj = (T)Activator.CreateInstance(typeof(T), [names[0], description]);
        for (int i = 1; i < names.Length; i++) { obj.AddAlias(names[i]); }
        return obj;
    }

    /// <summary>
    /// Add option
    /// </summary>
    /// <param name="command"></param>
    /// <param name="name"></param>
    /// <param name="description"></param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public static Option<T> AddOption<T>(this Command command, string name, string description)
    {
        var option = CreateObject<Option<T>>(name, description);
        command.AddOption(option);
        return option;
    }

    /// <summary>
    /// Add validator range
    /// </summary>
    /// <param name="option"></param>
    /// <param name="min"></param>
    /// <param name="max"></param>
    /// <returns></returns>
    public static Option<T> AddValidatorRange<T>(this Option<T> option, T min, T max) where T : INumber<T>
    {
        option.AddValidator(e =>
        {
            var range = e.GetValueOrDefault<T>();
            if (range < min || range > max)
            {
                e.ErrorMessage = $"Option {e.Token.Value} whit value '{range}' is not in range!";
            }
        });

        return option;
    }

    /// <summary>
    /// Add validator exist file
    /// </summary>
    /// <param name="option"></param>
    public static Option<string> AddValidatorExistFile(this Option<string> option)
    {
        option.AddValidator(e =>
        {
            var fileName = e.GetValueOrDefault<string>();
            if (!File.Exists(fileName))
            {
                e.ErrorMessage = $"Option {e.Token.Value} whit value '{fileName}' is not a valid file!";
            }
        });

        return option;
    }

    /// <summary>
    /// Add validator exist directory
    /// </summary>
    /// <param name="option"></param>
    public static Option<string> AddValidatorExistDirectory(this Option<string> option)
    {
        option.AddValidator(e =>
        {
            var directoryName = e.GetValueOrDefault<string>();
            if (!Directory.Exists(directoryName))
            {
                e.ErrorMessage = $"Option {e.Token.Value} whit value '{directoryName}' is not a valid file!";
            }
        });

        return option;
    }

    /// <summary>
    /// Script file option
    /// </summary>
    /// <param name="command"></param>
    /// <returns></returns>
    public static Option<string> ScriptFileOption(this Command command)
        => command.AddOption<string>("--script", "Use specified hook script")
                  .AddValidatorExistFile();

    /// <summary>
    /// Timeout operation
    /// </summary>
    /// <param name="command"></param>
    /// <returns></returns>
    public static Option<long> TimeoutOption(this Command command) => command.AddOption<long>("--timeout", "Timeout operation in seconds");
}