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
using System.Diagnostics;
using System.IO;
using System.Text;
using Corsinvest.ProxmoxVE.Api.Extension.Helpers;
using Corsinvest.ProxmoxVE.Api.Extension.VM;
using McMaster.Extensions.CommandLineUtils;

namespace Corsinvest.ProxmoxVE.Api.Shell.Helpers
{
    /// <summary>
    /// Command option shell extension.
    /// </summary>
    public static class CommandOptionExtension
    {
        /// <summary>
        /// Add fullname and logo
        /// </summary>
        /// <param name="command"></param>
        public static void AddFullNameLogo(this CommandLineApplication command)
        {
            var parent = command;
            while (parent.Parent != null) { parent = parent.Parent; }

            command.FullName = ShellHelper.MakeLogoAndTitle(parent.Description);
            command.ExtendedHelpText = $@"
{parent.Name} is a part of suite cv4pve-tools.
For more information visit https://www.cv4pve-tools.com";
        }

        /// <summary>
        /// Get option
        /// </summary>
        /// <param name="command"></param>
        /// <param name="optionName"></param>
        /// <param name="searchInParent"></param>
        /// <returns></returns>
        public static CommandOption GetOption(this CommandLineApplication command,
                                              string optionName,
                                              bool searchInParent = false)
        {
            foreach (var option in command.GetOptions())
            {
                if (option.ShortName == optionName || option.LongName == optionName) { return option; }
            }

            //found in parent
            if (searchInParent && command.Parent != null) { return command.Parent.GetOption(optionName, true); }

            return null;
        }

        /// <summary>
        /// Debug is active
        /// </summary>
        /// <param name="command"></param>
        /// <returns></returns>
        public static bool DebugIsActive(this CommandLineApplication command)
            => command.GetOption(DEBUG_OPTION_NAME).HasValue();

        /// <summary>
        /// Debug value
        /// </summary>
        /// <param name="command"></param>
        /// <returns></returns>
        public static int DebugValue(this CommandLineApplication command)
        {
            var ret = 0;
            if (command.DebugIsActive())
            {
                var value = command.GetOption(DEBUG_OPTION_NAME).Value() ?? "99";
                ret = int.Parse(value);
            }
            return ret;
        }

        /// <summary>
        /// Dryrun is active
        /// </summary>
        /// <param name="command"></param>
        /// <returns></returns>
        public static bool DryRunIsActive(this CommandLineApplication command)
            => command.GetOption("dry-run").HasValue();

        /// <summary>
        /// Node option
        /// </summary>
        /// <param name="command"></param>
        /// <returns></returns>
        public static CommandOption NodeOption(this CommandLineApplication command)
            => command.Option("--node", "Node of cluster", CommandOptionType.SingleValue);

        /// <summary>
        /// Node argument
        /// </summary>
        /// <param name="command"></param>
        /// <returns></returns>
        public static CommandArgument NodeArgument(this CommandLineApplication command)
            => command.Argument("node", "Node of cluster").IsRequired();

        /// <summary>
        /// Output type
        /// </summary>
        /// <param name="command"></param>
        /// <returns></returns>
        public static CommandOption<TableOutputType> OutputTypeArgument(this CommandLineApplication command)
            => command.OptionEnum<TableOutputType>("--output|-o", "Type output (default: text)");

        /// <summary>
        /// Debug option
        /// </summary>
        /// <param name="command"></param>
        /// <returns></returns>
        public static CommandOption<int> DebugOption(this CommandLineApplication command)
        {
            var opt = command.Option<int>($"--{DEBUG_OPTION_NAME}",
                                          "Debug application",
                                          CommandOptionType.SingleOrNoValue);
            opt.ShowInHelpText = false;
            opt.Inherited = true;
            return opt;
        }

        /// <summary>
        /// Dry run option
        /// </summary>
        /// <param name="command"></param>
        /// <returns></returns>
        public static CommandOption DryRunOption(this CommandLineApplication command)
        {
            var opt = command.Option("--dry-run", "Dry run application", CommandOptionType.NoValue);
            opt.ShowInHelpText = false;
            opt.Inherited = true;
            return opt;
        }

        /// <summary>
        /// Id or name option
        /// </summary>
        /// <param name="command"></param>
        /// <returns></returns>
        public static CommandOption VmIdOrNameOption(this CommandLineApplication command)
            => command.Option("--vmid", "The id or name VM/CT", CommandOptionType.SingleValue);

        /// <summary>
        /// Ids or names option
        /// </summary>
        /// <param name="command"></param>
        /// <returns></returns>
        public static CommandOption VmIdsOrNamesOption(this CommandLineApplication command)
        {
            var opt = command.VmIdOrNameOption();
            opt.Description = @"The id or name VM/CT comma separated (eg. 100,101,102,TestDebian)
-vmid or -name exclude (e.g. -200,-TestUbuntu)
range 100:107,-105,200:204
'@pool-???' for all VM/CT in specific pool (e.g. @pool-customer1),
'@all-???' for all VM/CT in specific host (e.g. @all-pve1, @all-\$(hostname)),
'@all' for all VM/CT in cluster";

            return opt;
        }

        /// <summary>
        /// Get Api token
        /// </summary>
        /// <param name="command"></param>
        /// <returns></returns>
        public static CommandOption GetApiToken(this CommandLineApplication command) => command.GetOption(API_TOKEN_OPTION_NAME, true);

        /// <summary>
        /// Get username
        /// </summary>
        /// <param name="command"></param>
        /// <returns></returns>
        public static CommandOption GetUsername(this CommandLineApplication command) => command.GetOption(USERNAME_OPTION_NAME, true);

        /// <summary>
        /// Get password
        /// </summary>
        /// <param name="command"></param>
        /// <returns></returns>
        public static CommandOption GetPassword(this CommandLineApplication command) => command.GetOption(PASSWORD_OPTION_NAME, true);

        /// <summary>
        /// Get host
        /// </summary>
        /// <param name="command"></param>
        /// <returns></returns>
        public static CommandOption GetHost(this CommandLineApplication command) => command.GetOption(HOST_OPTION_NAME, true);

        /// <summary>
        /// VM State option
        /// </summary>
        /// <param name="command"></param>
        /// <returns></returns>
        public static CommandOption VmStateOption(this CommandLineApplication command)
            => command.Option("--state", "Save the vmstate", CommandOptionType.NoValue);

        /// <summary>
        /// Script hook option
        /// </summary>
        /// <param name="command"></param>
        /// <returns></returns>
        public static CommandOption ScriptHookOption(this CommandLineApplication command)
        {
            var opt = command.Option("--script", "Use specified hook script", CommandOptionType.SingleValue);
            opt.Accepts().ExistingFile();
            return opt;
        }

        /// <summary>
        /// Timeout operation
        /// </summary>
        /// <param name="command"></param>
        /// <returns></returns>
        public static CommandOption<long> TimeoutOption(this CommandLineApplication command)
            => command.Option<long>("--timeout", "Timeout operation in seconds", CommandOptionType.SingleValue);

        #region Login option
        /// <summary>
        /// Add options login
        /// </summary>
        /// <param name="command"></param>
        public static void AddLoginOptions(this CommandLineApplication command)
        {
            command.HostOption();
            //    .DependOn(command, USERNAME_OPTION_NAME)
            //  .DependOn(command, PASSWORD_OPTION_NAME)

            command.ApiTokenOption();
            //.DependOn(command, HOST_OPTION_NAME);

            command.UsernameRealOption()
                   //.DependOn(command, HOST_OPTION_NAME)
                   .DependOn(command, PASSWORD_OPTION_NAME);

            command.PasswordOption()
                   //.DependOn(command, HOST_OPTION_NAME)
                   .DependOn(command, USERNAME_OPTION_NAME);
        }

        /// <summary>
        /// Api Token
        /// </summary>
        public static readonly string API_TOKEN_OPTION_NAME = "api-token";

        /// <summary>
        /// Host option
        /// </summary>
        public static readonly string HOST_OPTION_NAME = "host";

        /// <summary>
        /// Username option
        /// </summary>
        public static readonly string USERNAME_OPTION_NAME = "username";

        /// <summary>
        /// Password option
        /// </summary>
        public static readonly string PASSWORD_OPTION_NAME = "password";

        /// <summary>
        /// Password option
        /// </summary>
        public static readonly string DEBUG_OPTION_NAME = "debug";

        /// <summary>
        /// Host option
        /// </summary>
        /// <param name="command"></param>
        /// <returns></returns>
        public static CommandOption HostOption(this CommandLineApplication command)
            => command.Option($"--{HOST_OPTION_NAME}",
                              "The host name host[:port],host1[:port],host2[:port]",
                              CommandOptionType.SingleValue);

        /// <summary>
        /// Api token option
        /// </summary>
        /// <param name="command"></param>
        /// <returns></returns>
        public static CommandOption ApiTokenOption(this CommandLineApplication command)
            => command.Option($"--{API_TOKEN_OPTION_NAME}",
                              "Api token format 'USER@REALM!TOKENID=UUID'. Require Proxmox VE 6.2 or later",
                              CommandOptionType.SingleValue);

        /// <summary>
        /// Username real option
        /// </summary>
        /// <param name="command"></param>
        /// <returns></returns>
        public static CommandOption UsernameRealOption(this CommandLineApplication command)
            => command.Option($"--{USERNAME_OPTION_NAME}",
                              "User name <username>@<realm>",
                              CommandOptionType.SingleValue);

        /// <summary>
        /// username options
        /// </summary>
        /// <param name="command"></param>
        /// <returns></returns>
        public static CommandOption Username(this CommandLineApplication command)
            => command.Option($"--{USERNAME_OPTION_NAME}",
                              "User name",
                              CommandOptionType.SingleValue);

        /// <summary>
        /// Password option
        /// </summary>
        /// <param name="command"></param>
        /// <returns></returns>
        public static CommandOption PasswordOption(this CommandLineApplication command)
            => command.Option($"--{PASSWORD_OPTION_NAME}",
                              "The password. Specify 'file:path_file' to store password in file.",
                              CommandOptionType.SingleValue);

        /// <summary>
        /// Try login client api
        /// </summary>
        /// <param name="command"></param>
        /// <returns></returns>
        public static PveClient ClientTryLogin(this CommandLineApplication command)
        {
            var error = "Problem connection!";
            try
            {
                var client = ClientHelper.GetClientFromHA(command.GetHost().Value(), command.Out);

                //debug level
                client.DebugLevel = command.DebugValue();

                if (command.GetApiToken().HasValue())
                {
                    //use api token
                    client.ApiToken = command.GetApiToken().Value();

                    //check is valid API
                    var ver = client.Version.Version();
                    if (ver.IsSuccessStatusCode) { return client; }
                }
                else
                {
                    //use user and password
                    //try login
                    if (client.Login(command.GetUsername().Value(), GetPasswordFromOption(command)))
                    {
                        return client;
                    }

                }

                if (!client.LastResult.IsSuccessStatusCode) { error += " " + client.LastResult.ReasonPhrase; }
            }
            catch (Exception ex)
            {
                throw new ApplicationException(error, ex);
            }

            throw new ApplicationException(error);
        }

        /// <summary>
        /// Return password from option
        /// </summary>
        /// <param name="command"></param>
        /// <returns></returns>
        public static string GetPasswordFromOption(this CommandLineApplication command)
        {
            const string KEY = "012345678901234567890123";

            var password = command.GetPassword().Value();
            if (!string.IsNullOrWhiteSpace(password))
            {
                password = password.Trim();

                //check if file
                if (password.StartsWith("file:"))
                {
                    var fileName = password.Substring(5);
                    if (File.Exists(fileName))
                    {
                        password = StringHelper.Decrypt(File.ReadAllText(fileName, Encoding.UTF8), KEY, false);
                    }
                    else
                    {
                        password = Prompt.GetPassword("Password:");
                        File.WriteAllText(fileName, StringHelper.Encrypt(password, KEY, false), Encoding.UTF8);
                    }
                }
            }

            return password;
        }
        #endregion

        /// <summary>
        /// Label option
        /// </summary>
        /// <param name="command"></param>
        /// <returns></returns>
        public static CommandOption LabelOption(this CommandLineApplication command)
            => command.Option("--label",
                              "Is usually 'hourly', 'daily', 'weekly', or 'monthly'",
                              CommandOptionType.SingleValue);

        /// <summary>
        /// Keep option
        /// </summary>
        /// <param name="command"></param>
        /// <returns></returns>
        public static CommandOption<int> KeepOption(this CommandLineApplication command)
            => command.Option<int>("--keep",
                                   "Specify the number which should will keep",
                                   CommandOptionType.SingleValue);

        /// <summary>
        /// Mail to option
        /// </summary>
        /// <param name="command"></param>
        /// <returns></returns>
        public static CommandOption MailToOption(this CommandLineApplication command)
            => command.Option("--mail-to",
                                  "Comma-separated list of email addresses that should receive email notifications",
                                  CommandOptionType.SingleValue)
                      .Accepts(v => v.EmailAddress());

        /// <summary>
        /// Vm Id option
        /// </summary>
        /// <param name="command"></param>
        /// <param name="dependOn"></param>
        /// <returns></returns>
        public static CommandOption<int> VmIdOption(this CommandLineApplication command, string dependOn)
            => command.Option<int>("--vmid", "The id VM/CT", CommandOptionType.SingleValue)
                      .DependOn(command, dependOn);

        /// <summary>
        /// Wait option
        /// </summary>
        /// <param name="command"></param>
        /// <returns></returns>
        public static CommandOption WaitOption(this CommandLineApplication command)
            => command.Option("--wait", "Wait for task finish", CommandOptionType.NoValue);

        /// <summary>
        /// Option from enum
        /// </summary>
        public static CommandOption<TEnum> OptionEnum<TEnum>(this CommandLineApplication command,
                                                      string template,
                                                      string description) where TEnum : struct, IConvertible
        {
            if (!typeof(TEnum).IsEnum) { throw new ArgumentException("T must be an enumerated type"); }

            var ret = command.Option<TEnum>(template,
                                            description + " " + string.Join(",", Enum.GetNames(typeof(TEnum))),
                                            CommandOptionType.SingleValue);
            ret.Accepts().Enum<TEnum>(true);
            return ret;
        }

        /// <summary>
        /// Get value form enum option
        /// </summary>
        /// <param name="command"></param>
        /// <typeparam name="TEnum"></typeparam>
        /// <returns></returns>
        public static TEnum GetEnumValue<TEnum>(this CommandOption command)
            where TEnum : struct, IConvertible
        {
            if (!typeof(TEnum).IsEnum) { throw new ArgumentException("T must be an enumerated type"); }

            Enum.TryParse<TEnum>(command.Value(), true, out var value);
            return value;
        }

        /// <summary>
        /// Option form string values
        /// </summary>
        /// <param name="command"></param>
        /// <param name="template"></param>
        /// <param name="description"></param>
        /// <param name="allowedValues"></param>
        /// <returns></returns>
        public static CommandOption OptionEnum(this CommandLineApplication command,
                                               string template,
                                               string description,
                                               params string[] allowedValues)
        {
            var ret = command.Option(template,
                                     description + " " + string.Join(",", allowedValues),
                                     CommandOptionType.SingleValue);

            ret.Accepts().Values(true, allowedValues);
            return ret;
        }

        /// <summary>
        /// VM type option
        /// </summary>
        /// <param name="command"></param>
        /// <returns></returns>
        public static CommandOption<VMTypeEnum> VMTypeOption(this CommandLineApplication command)
            => command.OptionEnum<VMTypeEnum>("--vmType", "VM type");

        /// <summary>
        /// Snapshot name option
        /// </summary>
        /// <param name="command"></param>
        /// <param name="dependOn"></param>
        /// <returns></returns>
        public static CommandOption SnapshotNameOption(this CommandLineApplication command, string dependOn)
            => command.Option("--snapname", "The name of the snapshot", CommandOptionType.SingleValue)
                      .DependOn(command, dependOn);

        /// <summary>
        /// Description option
        /// </summary>
        /// <param name="command"></param>
        /// <param name="dependOn"></param>
        /// <returns></returns>
        public static CommandOption DescriptionOption(this CommandLineApplication command, string dependOn)
            => command.Option("--description", "A textual description or comment", CommandOptionType.SingleValue)
                      .DependOn(command, dependOn);

        /// <summary>
        /// Command check update application
        /// </summary>
        /// <param name="app"></param>
        public static void CheckUpdateApp(this CommandLineApplication app)
        {
            app.Command("app-check-update", cmd =>
            {
                cmd.Description = "Check update application";
                cmd.AddFullNameLogo();

                cmd.OnExecute(() => app.Out.WriteLine(UpdateHelper.GetInfo(app.Name).Info));
            });
        }

        /// <summary>
        /// Upgrade application
        /// </summary>
        /// <param name="app"></param>
        public static void UpgradeApp(this CommandLineApplication app)
        {
            const string APP_UPGRADE_FINISH = "app-upgrade-finish";

            app.Command("app-upgrade", cmd =>
            {
                cmd.Description = "Upgrade application";
                cmd.AddFullNameLogo();
                var optQuiet = cmd.Option("--quiet|-q",
                                          "Non-interactive mode, does not request confirmation",
                                          CommandOptionType.NoValue);

                cmd.OnExecute(() =>
                {
                    var (Info, IsNewVersion, BrowserDownloadUrl) = UpdateHelper.GetInfo(app.Name);
                    app.Out.WriteLine(Info);

                    if (IsNewVersion)
                    {
                        if (!optQuiet.HasValue())
                        {
                            if (!Prompt.GetYesNo("Confirm upgrade application?", false))
                            {
                                app.Out.WriteLine("Upgrade abort!");
                                return 1;
                            }
                        }

                        app.Out.WriteLine($"Download {BrowserDownloadUrl} ....");

                        var fileNameNew = UpdateHelper.UpgradePrepare(BrowserDownloadUrl,
                                                                      Process.GetCurrentProcess().MainModule.FileName);

                        Process.Start(fileNameNew, APP_UPGRADE_FINISH);
                    }

                    return 0;
                });
            });

            //finish upgrade application
            app.Command(APP_UPGRADE_FINISH, cmd =>
            {
                cmd.ShowInHelpText = false;
                cmd.OnExecute(() =>
                {
                    var fileNameNew = Process.GetCurrentProcess().MainModule.FileName;
                    UpdateHelper.UpgradeFinish(fileNameNew);

                    app.Out.WriteLine("Upgrade completed!");

                    return 0;
                });
            });
        }
    }
}