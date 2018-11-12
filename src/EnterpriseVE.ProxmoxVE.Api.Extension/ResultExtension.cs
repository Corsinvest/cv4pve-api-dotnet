using EnterpriseVE.ProxmoxVE.Api.Extension.Utils;
using EnterpriseVE.ProxmoxVE.Api.Extension.VM;

namespace EnterpriseVE.ProxmoxVE.Api.Extension
{
    public static class ResultExtension
    {
        public static bool InError(this Result result) { return (result != null && result.ResponseInError); }

        public static void WaitForTaskToFinish(this Result result, VMInfo vm, bool wait)
        {
            if (result != null && !result.ResponseInError && wait)
            {
                vm.Client.WaitForTaskToFinish(vm.Node, result.Response.data, 1000, 200000);
            }
        }
    }
}