/*
 * SPDX-FileCopyrightText: Copyright Corsinvest Srl
 * SPDX-License-Identifier: GPL-3.0-only
 */

using Microsoft.Extensions.Logging;
using System.CommandLine;
using System.Reflection;

namespace Corsinvest.ProxmoxVE.Api.Console.Helpers;

/// <summary>
/// Console Helper
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
            var keyInfo = System.Console.ReadKey(intercept: true);
            key = keyInfo.Key;

            if (key == ConsoleKey.Backspace && pass.Length > 0)
            {
                System.Console.Write("\b \b");
                pass = pass[0..^1];
            }
            else if (!char.IsControl(keyInfo.KeyChar))
            {
                System.Console.Write("*");
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
            System.Console.Write($"{prompt} {message}");
            System.Console.Write(' ');
            var input = System.Console.ReadLine()?.ToLower()?.Trim();
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
                    System.Console.WriteLine("Invalid response '" + input + "'. Please answer 'y' or 'n' or CTRL+C to exit.");
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
        var rc = new RootCommand(description);
        rc.AddFullNameLogo();
        rc.AddDebugOption();
        rc.AddDryRunOption();
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
    {
        var resultCode = 0;

        try
        {
            resultCode = await rootCommand.Parse(args).InvokeAsync(new()
            {
                EnableDefaultExceptionHandler = false
            });
        }
        catch (Exception ex)
        {
            System.Console.Out.WriteLine("ERROR: " + ex.Message);

            if (rootCommand.DebugIsActive())
            {
                logger.LogError(ex, ex.Message);

                System.Console.Out.WriteLine("================ EXCEPTION ================ ");
                System.Console.Out.WriteLine(ex.GetType().FullName);
                System.Console.Out.WriteLine(ex.Message);
                System.Console.Out.WriteLine(ex.StackTrace);
            }

            resultCode = 1;
        }
        return resultCode;
    }
}