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

using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Corsinvest.ProxmoxVE.Api.Extension.VM;

namespace Corsinvest.ProxmoxVE.Api.Extension
{
    /// <summary>
    /// Extension result for client.
    /// </summary>
    public static class ResultExtension
    {
        /// <summary>
        /// Default timeout
        /// </summary>
        public readonly static long DEFAULT_TIMEOUT = 30000;

        /// <summary>
        /// Enumerable result for Linq.
        /// </summary>
        /// <param name="result"></param>
        /// <returns></returns>
        public static IEnumerable<dynamic> ToEnumerable(this Result result)
            => ((IEnumerable)result.Response.data).Cast<dynamic>();

        /// <summary>
        /// Enumerable result for Linq.
        /// </summary>
        /// <param name="result"></param>
        /// <returns></returns>
        public static IEnumerable<dynamic> ToEnumerableOrDefault(this Result result)
            => result.IsSuccessStatusCode ? result.ToEnumerable() : new List<dynamic>();

        /// <summary>
        /// Check result in error.
        /// </summary>
        /// <param name="result"></param>
        /// <returns></returns>
        public static bool InError(this Result result) => result != null && result.ResponseInError;

        /// <summary>
        /// Log error if exists
        /// </summary>
        /// <param name="result"></param>
        /// <param name="out"></param>
        /// <returns></returns>
        public static bool LogInError(this Result result, TextWriter @out)
        {
            if (result.InError())
            {
                @out.WriteLine(result.GetError());
                return true;
            }
            return false;
        }

        /// <summary>
        /// Wait until task is finish.
        /// </summary>
        /// <param name="result"></param>
        /// <param name="vm"></param>
        /// <param name="timeout"></param>
        public static void WaitForTaskToFinish(this Result result, VMInfo vm, long timeout)
        {
            if (result != null && !result.ResponseInError && timeout > 0)
            {
                vm.Client.WaitForTaskToFinish(vm.Node, result.Response.data, 1000, timeout);
            }
        }

        /// <summary>
        /// Check task is running.
        /// </summary>
        /// <param name="result"></param>
        /// <param name="vm"></param>
        /// <returns></returns>
        public static bool IsRunningTask(this Result result, VMInfo vm)
            => vm.Client.TaskIsRunning(vm.Node, result.Response.data);

        /// <summary>
        /// Get exit status code task.
        /// </summary>
        /// <param name="result"></param>
        /// <param name="vm"></param>
        /// <returns></returns>
        public static string GetExitStatusTask(this Result result, VMInfo vm)
            => vm.Client.GetExitStatusTask(vm.Node, result.Response.data);
    }
}