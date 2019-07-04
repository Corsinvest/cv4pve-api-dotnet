using System;
using System.IO;
using Corsinvest.ProxmoxVE.Api.Extension.VM;

namespace Corsinvest.ProxmoxVE.Api.Extension
{
    /// <summary>
    /// Extension result for client.
    /// </summary>
    public static class ResultExtension
    {
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
        /// <param name="stdOut"></param>
        /// <returns></returns>
        public static bool LogInError(this Result result, TextWriter stdOut)
        {
            if (result.InError())
            {
                stdOut.WriteLine(result.GetError());
                return true;
            }
            return false;
        }

        /// <summary>
        /// Wait until task is finish.
        /// </summary>
        /// <param name="result"></param>
        /// <param name="vm"></param>
        /// <param name="wait"></param>
        public static void WaitForTaskToFinish(this Result result, VMInfo vm, bool wait)
        {
            if (result != null && !result.ResponseInError && wait)
            {
                vm.Client.WaitForTaskToFinish(vm.Node, result.Response.data, 1000, 20000);
            }
        }

        /// <summary>
        /// Check task is running.
        /// </summary>
        /// <param name="result"></param>
        /// <param name="vm"></param>
        /// <returns></returns>
        public static bool IsRunningTask(this Result result, VMInfo vm) => vm.Client.TaskIsRunning(vm.Node, result.Response.data);

        /// <summary>
        /// Get exit status code task.
        /// </summary>
        /// <param name="result"></param>
        /// <param name="vm"></param>
        /// <returns></returns>
        public static string GetExitStatusTask(this Result result, VMInfo vm) => vm.Client.GetExitStatusTask(vm.Node, result.Response.data);
    }
}