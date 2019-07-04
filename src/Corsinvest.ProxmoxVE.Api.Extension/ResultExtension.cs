using System;
using System.IO;
using Corsinvest.ProxmoxVE.Api.Extension.VM;

namespace Corsinvest.ProxmoxVE.Api.Extension
{
    public static class ResultExtension
    {
        public static bool InError(this Result result) => result != null && result.ResponseInError;

        public static bool LogInError(this Result result, TextWriter stdOut)
        {
            if (result.InError())
            {
                stdOut.WriteLine(result.GetError());
                return true;
            }
            return false;
        }

        public static void WaitForTaskToFinish(this Result result, VMInfo vm, bool wait)
        {
            if (result != null && !result.ResponseInError && wait)
            {
                vm.Client.WaitForTaskToFinish(vm.Node, result.Response.data, 1000, 20000);
            }
        }

        public static bool IsRunningTask(this Result result, VMInfo vm) => vm.Client.TaskIsRunning(vm.Node, result.Response.data);
        public static string GetExitStatusTask(this Result result, VMInfo vm) => vm.Client.GetExitStatusTask(vm.Node, result.Response.data);
    }
}