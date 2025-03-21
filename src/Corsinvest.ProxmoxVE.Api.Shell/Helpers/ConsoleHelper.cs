﻿/*
 * SPDX-FileCopyrightText: Copyright Corsinvest Srl
 * SPDX-License-Identifier: GPL-3.0-only
 */

using Microsoft.Extensions.Logging;
using System.CommandLine;
using System.CommandLine.Builder;
using System.CommandLine.Parsing;
using System.Reflection;

namespace Corsinvest.ProxmoxVE.Api.Shell.Helpers;

/// <summary>
/// Shell Helper
/// </summary>
public static class ConsoleHelper
{
    /// <summary>
    /// Logo Corsinvest art ascii.
    /// </summary>
    /// <returns></returns>
    public static readonly string Logo = @"
   ______                _                      __
  / ____/___  __________(_)___ _   _____  _____/ /_
 / /   / __ \/ ___/ ___/ / __ \ | / / _ \/ ___/ __/
/ /___/ /_/ / /  (__  ) / / / / |/ /  __(__  ) /_
\____/\____/_/  /____/_/_/ /_/|___/\___/____/\__/
";

    /// <summary>
    /// Remember these things
    /// </summary>
    public static readonly string RememberTheseThings = @"Remember these things:
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
        var space = new string(' ', title.Length < 47
                                    ? 47 - title.Length
                                    : 1);

        return $@"{Logo}

{title}{space}(Made in Italy)";
    }

    /// <summary>
    /// Read password from console
    /// </summary>
    /// <returns></returns>
    public static string ReadPassword()
    {
        var pass = string.Empty;
        ConsoleKey key;
        do
        {
            var keyInfo = Console.ReadKey(intercept: true);
            key = keyInfo.Key;

            if (key == ConsoleKey.Backspace && pass.Length > 0)
            {
                Console.Write("\b \b");
                pass = pass[0..^1];
            }
            else if (!char.IsControl(keyInfo.KeyChar))
            {
                Console.Write("*");
                pass += keyInfo.KeyChar;
            }
        } while (key != ConsoleKey.Enter);

        return pass;
    }

    /// <summary>
    /// Get yes or no from console
    /// </summary>
    /// <param name="prompt"></param>
    /// <param name="defaultAnswer"></param>
    /// <returns></returns>
    public static bool ReadYesNo(string prompt, bool defaultAnswer)
    {
        var message = defaultAnswer
                        ? "[Y/n]"
                        : "[y/N]";

        while (true)
        {
            Console.Write($"{prompt} {message}");
            Console.Write(' ');
            var input = Console.ReadLine()?.ToLower()?.Trim();
            if (string.IsNullOrEmpty(input)) { break; }

            switch (input)
            {
                case "n":
                case "no":
                    return false;

                case "y":
                case "yes":
                    return true;

                default:
                    Console.WriteLine("Invalid response '" + input + "'. Please answer 'y' or 'n' or CTRL+C to exit.");
                    break;
            }
        }

        return defaultAnswer;
    }

    /// <summary>
    /// Get current version application
    /// </summary>
    /// <returns></returns>
    public static string GetCurrentVersionApp()
        => Assembly.GetEntryAssembly()
                   .GetCustomAttribute<AssemblyInformationalVersionAttribute>()
                   .InformationalVersion;

    /// <summary>
    /// Create console application.
    /// </summary>
    /// <param name="name"></param>
    /// <param name="description"></param>
    /// <returns></returns>
    public static RootCommand CreateApp(string name, string description)
    {
        var rc = new RootCommand
        {
            Name = name,
            Description = description,
        };

        rc.AddFullNameLogo();
        rc.DebugOption();
        rc.DryRunOption();
        rc.AddLoginOptions();

        return rc;
    }

    /// <summary>
    /// Create LoggerFactory
    /// </summary>
    /// <param name="logLevel"></param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public static ILoggerFactory CreateLoggerFactory<T>(LogLevel logLevel = LogLevel.Warning)
        => LoggerFactory.Create(builder =>
           {
               builder.AddFilter("Microsoft", LogLevel.Warning)
                      .AddFilter("System", LogLevel.Warning)
                      .AddFilter(typeof(PveClientBase).FullName, logLevel)
                      .AddFilter(typeof(T).FullName, logLevel)
                      .AddConsole();
           });

    /// <summary>
    /// Execute console application.
    /// </summary>
    /// <param name="rootCommand"></param>
    /// <param name="args"></param>
    /// <param name="logger"></param>
    /// <returns></returns>
    public static async Task<int> ExecuteAppAsync(this RootCommand rootCommand, string[] args, ILogger logger)
        => await new CommandLineBuilder(rootCommand)
                        .UseDefaults()
                        .UseExceptionHandler((ex, context) =>
                        {
                            var exTmp = ex;
                            while (exTmp != null)
                            {
                                Console.Out.WriteLine("ERROR: " + exTmp.Message);
                                exTmp = exTmp.InnerException;
                            }

                            if (rootCommand.DebugIsActive())
                            {
                                logger.LogError(ex, ex.Message);

                                Console.Out.WriteLine("================ EXCEPTION ================ ");
                                Console.Out.WriteLine(ex.GetType().FullName);
                                Console.Out.WriteLine(ex.Message);
                                Console.Out.WriteLine(ex.StackTrace);
                            }

                            context.ExitCode = 1;
                        })
                        .Build()
                        .InvokeAsync(args);
}