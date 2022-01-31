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
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
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
            => ((IEnumerable)result.ToData()).Cast<dynamic>();

        /// <summary>
        /// Enumerable result data.
        /// </summary>
        /// <param name="result"></param>
        /// <returns></returns>
        public static dynamic ToData(this Result result) => result.Response.data;

        /// <summary>
        /// Enumerable result data.
        /// </summary>
        /// <param name="result"></param>
        /// <returns></returns>
        public static T ToData<T>(this Result result) => (T)Convert.ChangeType(result.ToData(), typeof(T));

        /// <summary>
        /// Enumerable result for Linq.
        /// </summary>
        /// <param name="result"></param>
        /// <returns></returns>
        public static IEnumerable<dynamic> ToEnumerableOrDefault(this Result result)
            => result.IsSuccessStatusCode
                ? result.ToEnumerable()
                : new List<dynamic>();

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
        /// <param name="output"></param>
        /// <returns></returns>
        public static bool LogInError(this Result result, TextWriter output)
        {
            if (result.InError())
            {
                output.WriteLine(result.GetError());
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
        public static async Task<bool> WaitForTaskToFinish(this Result result, VMInfo vm, long timeout)
            => result != null && !result.ResponseInError && timeout > 0
                ? await vm.Client.WaitForTaskToFinish(vm.Node, result.ToData(), 1000, timeout)
                : true;

        /// <summary>
        /// Check task is running.
        /// </summary>
        /// <param name="result"></param>
        /// <param name="vm"></param>
        /// <returns></returns>
        public static async Task<bool> IsRunningTask(this Result result, VMInfo vm)
            => await vm.Client.TaskIsRunning(vm.Node, result.ToData());

        /// <summary>
        /// Get exit status code task.
        /// </summary>
        /// <param name="result"></param>
        /// <param name="vm"></param>
        /// <returns></returns>
        public static async Task<string> GetExitStatusTask(this Result result, VMInfo vm)
            => await vm.Client.GetExitStatusTask(vm.Node, result.ToData());
    }
}