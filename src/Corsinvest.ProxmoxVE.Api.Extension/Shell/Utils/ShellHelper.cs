using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using McMaster.Extensions.CommandLineUtils;

namespace Corsinvest.ProxmoxVE.Api.Extension.Shell.Utils
{
    public static class ShellHelper
    {
        public const string EMAIL_SUPPORT = "support@corsinvest.it";
        public const string REPORT_BUGS = "Report bugs to " + EMAIL_SUPPORT;

        public const string LOGO = @"
    ______                _                      __
   / ____/___  __________(_)___ _   _____  _____/ /_
  / /   / __ \/ ___/ ___/ / __ \ | / / _ \/ ___/ __/
 / /___/ /_/ / /  (__  ) / / / / |/ /  __(__  ) /_
 \____/\____/_/  /____/_/_/ /_/|___/\___/____/\__/
";

        public static string MakeLogoAndTitle(string title)
        {
            title += new string(' ', (47 - title.Length));

            return $@"{LOGO}

{title}(Made in Italy)";
        }

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
                var escapedArgs = cmd.Replace("\"", "\\\"");
                startInfo.FileName = "/bin/bash";
                startInfo.Arguments = $"-c \"{escapedArgs}\"";
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                startInfo.FileName = cmd;
            }

            var process = new Process()
            {
                StartInfo = startInfo
            };

            //addiotional variable
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

        public static CommandLineApplication CreateConsoleApp(string name, string description)
        {
            var app = new CommandLineApplication();

            app.Name = name;
            app.Description = description;
            app.AddFullNameLogo();
            app.HelpOption(true);
            app.DebugOption();
            app.DryRunOption();

            return app;
        }

        public static int ExecuteConsoleApp(this CommandLineApplication app, TextWriter stdOut, string[] args)
        {
            //execute this
            app.OnExecute(() =>
            {
                app.ShowHint();
                return 1;
            });

            //execute command
            try
            {
                return app.Execute(args);
            }
            catch (CommandParsingException ex)
            {
                stdOut.WriteLine(ex.Message);
                return 1;
            }
            catch (Exception ex)
            {
                stdOut.WriteLine("================ EXCEPTION ================ ");
                stdOut.WriteLine(ex.GetType().FullName);
                stdOut.WriteLine(ex.Message);
                stdOut.WriteLine(ex.StackTrace);
                return 1;
            }
        }
    }
}