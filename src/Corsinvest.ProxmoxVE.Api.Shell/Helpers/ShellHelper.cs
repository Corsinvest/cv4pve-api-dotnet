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
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using McMaster.Extensions.CommandLineUtils;

namespace Corsinvest.ProxmoxVE.Api.Shell.Helpers
{
    /// <summary>
    /// Shell Helper
    /// </summary>
    public static class ShellHelper
    {
        /// <summary>
        /// Logo Corsinvest art ascii.
        /// </summary>
        /// <returns></returns>
        public static readonly string LOGO = @"
    ______                _                      __
   / ____/___  __________(_)___ _   _____  _____/ /_
  / /   / __ \/ ___/ ___/ / __ \ | / / _ \/ ___/ __/
 / /___/ /_/ / /  (__  ) / / / / |/ /  __(__  ) /_
 \____/\____/_/  /____/_/_/ /_/|___/\___/____/\__/
";

        /// <summary>
        /// Remember these things
        /// </summary>
        public static readonly string REMEMBER_THESE_THINGS = @"Remember these things:
- Think before typing.
- From great power comes great responsibility.

Good job";

        /// <summary>
        /// Make string logo and title.
        /// </summary>
        /// <param name="title"></param>
        /// <returns></returns>
        public static string MakeLogoAndTitle(string title)
        {
            title += new string(' ', 47 - title.Length);

            return $@"{LOGO} 

{title}(Made in Italy)";
        }

        /// <summary>
        /// Execute shell command
        /// </summary>
        /// <param name="cmd"></param>
        /// <param name="redirectStandardOutput"></param>
        /// <param name="environmentVariables"></param>
        /// <param name="out"></param>
        /// <param name="dryRun"></param>
        /// <param name="debug"></param>
        /// <returns></returns>
        public static (string StandardOutput, int ExitCode) Execute(string cmd,
                                                                    bool redirectStandardOutput,
                                                                    IDictionary<string, string> environmentVariables,
                                                                    TextWriter @out,
                                                                    bool dryRun,
                                                                    bool debug)
        {
            var startInfo = new ProcessStartInfo()
            {
                UseShellExecute = false,
                CreateNoWindow = true,
                RedirectStandardOutput = redirectStandardOutput,
            };

            if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux) || RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
            {
                startInfo.FileName = "/bin/bash";
                startInfo.Arguments = $"-c \"{cmd.Replace("\"", "\\\"")}\"";
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                startInfo.FileName = cmd;
            }

            var process = new Process
            {
                StartInfo = startInfo
            };

            //additional variable
            if (environmentVariables != null)
            {
                if (debug) { @out.WriteLine("-------------------------------------------------------"); }
                foreach (var variable in environmentVariables)
                {
                    if (debug) { @out.WriteLine($"{variable.Key}: {variable.Value}"); }
                    process.StartInfo.EnvironmentVariables.Add(variable.Key, variable.Value);
                }
                if (debug) { @out.WriteLine("-------------------------------------------------------"); }
            }

            if (debug) { @out.WriteLine($"Run command: {cmd}"); }

            if (dryRun)
            {
                return ("", 0);
            }
            else
            {
                process.Start();
                var standardOutput = redirectStandardOutput ? process.StandardOutput.ReadToEnd() : "";
                process.WaitForExit();

                return (standardOutput, process.ExitCode);
            }
        }

        /// <summary>
        /// Get current version application
        /// </summary>
        /// <returns></returns>
        public static string GetCurrentVersionApp()
        {
            return Assembly.GetEntryAssembly()
                  .GetCustomAttribute<AssemblyInformationalVersionAttribute>()
                  .InformationalVersion;
        }

        /// <summary>
        /// Create console application.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="description"></param>
        /// <returns></returns>
        public static CommandLineApplication CreateConsoleApp(string name, string description)
        {
            var app = new CommandLineApplication
            {
                Name = name,
                Description = description,
                UsePagerForHelpText = false,                
            };

            app.AddFullNameLogo();
            app.HelpOption(true);
            app.VersionOption("--version", GetCurrentVersionApp());
            app.DebugOption();
            app.DryRunOption();
            app.AddLoginOptions();
            app.CheckUpdateApp();
            app.UpgradeApp();

            //execute this
            app.OnExecute(() =>
            {
                app.ShowHint();
                return 1;
            });

            return app;
        }

        /// <summary>
        /// Execute console application.
        /// </summary>
        /// <param name="app"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        public static int ExecuteConsoleApp(this CommandLineApplication app, string[] args)
        {
            //execute command
            try
            {
                Version newVersion = null;

                //check new version available
                var taskInfo = new Task(() =>
                {
                    try
                    {
                        var version = UpdateHelper.GetInfoLastReleaseAssetFromGitHub(app.Name).Version;
                        if (version.ToString() != GetCurrentVersionApp())
                        {
                            newVersion = version;
                        }
                    }
                    catch { }
                });
                taskInfo.Start();

                var ret = app.Execute(args);

                taskInfo.Wait(1000); 

                if ((app.OptionHelp.HasValue() || app.OptionVersion.HasValue()) && newVersion != null)
                {
                    app.Out.WriteLine("====================================");
                    app.Out.WriteLine($"New version available: {newVersion}");
                    app.Out.WriteLine("====================================");
                }

                return ret;
            }
            catch (Exception ex)
            {
                if (ex is CommandParsingException ||
                    ex is ApplicationException ||
                    ex is ArgumentException)
                {
                    app.Out.WriteLine(ex.Message);
                }
                else
                {
                    app.Out.WriteLine("================ EXCEPTION ================ ");
                    app.Out.WriteLine(ex.GetType().FullName);
                    app.Out.WriteLine(ex.Message);
                    app.Out.WriteLine(ex.StackTrace);
                }

                return 1;
            }
        }
    }
}