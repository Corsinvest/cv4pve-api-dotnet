/*
 * SPDX-FileCopyrightText: 2022 Daniele Corsini <daniele.corsini@corsinvest.it>
 * SPDX-FileCopyrightText: Copyright Corsinvest Srl
 * SPDX-License-Identifier: GPL-3.0-only
 */

using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;

namespace Corsinvest.ProxmoxVE.Api.Shell.Helpers
{
    /// <summary>
    /// Shell Helper
    /// </summary>
    public static class ShellHelper
    {

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
            => Execute(cmd, redirectStandardOutput, environmentVariables, @out, dryRun, debug, false);

        /// <summary>
        /// Execute shell command
        /// </summary>
        /// <param name="cmd"></param>
        /// <param name="redirectStandardOutput"></param>
        /// <param name="environmentVariables"></param>
        /// <param name="out"></param>
        /// <param name="dryRun"></param>
        /// <param name="debug"></param>
        /// <param name="waitForExit"></param>
        /// <returns></returns>
        public static (string StandardOutput, int ExitCode) Execute(string cmd,
                                                                    bool redirectStandardOutput,
                                                                    IDictionary<string, string> environmentVariables,
                                                                    TextWriter @out,
                                                                    bool dryRun,
                                                                    bool debug,
                                                                    bool waitForExit)
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
                var exitCode = 0;
                if (waitForExit)
                {
                    process.WaitForExit();
                    exitCode = process.ExitCode;
                }

                return (standardOutput, exitCode);
            }
        }
    }
}