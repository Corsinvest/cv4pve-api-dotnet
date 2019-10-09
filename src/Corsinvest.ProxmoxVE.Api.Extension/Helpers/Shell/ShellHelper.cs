/*
 * This file is part of the cv4pve-api-dotnet https://github.com/Corsinvest/cv4pve-api-dotnet,
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

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using McMaster.Extensions.CommandLineUtils;

namespace Corsinvest.ProxmoxVE.Api.Extension.Helpers.Shell
{
    /// <summary>
    /// Shell Helper
    /// </summary>
    public static class ShellHelper
    {
        /// <summary>
        /// Email support.
        /// </summary>
        public static readonly string EMAIL_SUPPORT = "support@corsinvest.it";

        /// <summary>
        /// Row shell for support.
        /// </summary>
        public static readonly string REPORT_BUGS = "Report bugs to " + EMAIL_SUPPORT;

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
        /// <param name="stdOut"></param>
        /// <param name="dryRun"></param>
        /// <param name="debug"></param>
        /// <returns></returns>
        public static (string StandardOutput, int ExitCode) Execute(string cmd,
                                                                    bool redirectStandardOutput,
                                                                    IDictionary<string, string> environmentVariables,
                                                                    TextWriter stdOut,
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

            var process = new Process()
            {
                StartInfo = startInfo
            };

            //additional variable
            if (environmentVariables != null)
            {
                if (debug) { stdOut.WriteLine("-------------------------------------------------------"); }
                foreach (var variable in environmentVariables)
                {
                    if (debug) { stdOut.WriteLine($"{variable.Key}: {variable.Value}"); }
                    process.StartInfo.EnvironmentVariables.Add(variable.Key, variable.Value);
                }
                if (debug) { stdOut.WriteLine("-------------------------------------------------------"); }
            }

            if (debug) { stdOut.WriteLine($"Run command: {cmd}"); }

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
        /// Get application data directory. If not exists create.
        /// </summary>
        /// <param name="appName"></param>
        /// <returns></returns>
        public static string GetApplicationDataDirectory(string appName)
        {
            var path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "Corsinvest", appName);
            if (!Directory.Exists(path))
            {
                var dir = Directory.CreateDirectory(path);
                //dir.Attributes = FileAttributes.Directory | FileAttributes.Hidden;
            }

            return path;
        }

        /// <summary>
        /// Create console application.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="description"></param>
        /// <returns></returns>
        public static CommandLineApplication CreateConsoleApp(string name, string description)
        {
            var app = new CommandLineApplication()
            {
                Name = name,
                Description = description,
                UsePagerForHelpText = false,
            };

            app.AddFullNameLogo();
            app.HelpOption(true);
            app.DebugOption();
            app.DryRunOption();
            app.AddLoginOptions();
            //app.SelfUpdateCommand();

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
        /// <param name="stdOut"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        public static int ExecuteConsoleApp(this CommandLineApplication app, TextWriter stdOut, string[] args)
        {
            //execute command
            try
            {
                return app.Execute(args);
            }
            catch (Exception ex)
            {
                if (ex is CommandParsingException ||
                    ex is ApplicationException ||
                    ex is ArgumentException)
                {
                    stdOut.WriteLine(ex.Message);
                }
                else
                {
                    stdOut.WriteLine("================ EXCEPTION ================ ");
                    stdOut.WriteLine(ex.GetType().FullName);
                    stdOut.WriteLine(ex.Message);
                    stdOut.WriteLine(ex.StackTrace);
                }

                return 1;
            }
        }
    }
}