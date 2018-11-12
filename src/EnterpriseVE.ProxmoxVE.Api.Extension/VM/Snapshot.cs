using System;
using System.Collections.Generic;
using EnterpriseVE.ProxmoxVE.Api;
using EnterpriseVE.ProxmoxVE.Api.Extension.Utils;

namespace EnterpriseVE.ProxmoxVE.Api.Extension.VM
{
    public class Snapshot
    {
        private const string FORMAT = "{0,-17} {1,-25} {2,-25} {3,-25} {4,-25} {5,-1}";
        private const string FORMATEX = "{0,-10} {1,5} {2,-17} {3,-25} {4,-25} {5,-25} {6,-1}";

        private VMInfo _vm;
        private dynamic _apiData;

        internal Snapshot(VMInfo vm, object apiData)
        {
            _apiData = apiData;
            _vm = vm;

            AttributeHelper.NotExistCreate(_apiData, "description", "no-description");
            AttributeHelper.NotExistCreate(_apiData, "parent", "no-parent");
            AttributeHelper.NotExistCreate(_apiData, "snaptime");
            AttributeHelper.NotExistCreate(_apiData, "vmstate", 0);

            Date = _apiData.snaptime == null ?
                   DateTime.Now :
                   DateTimeOffset.FromUnixTimeSeconds(_apiData.snaptime).DateTime;
        }

        public Result Config => (Result)_vm.VMApi.Snapshot[Name].Config.GetRest();

        public Result Update(string name, string description)
        {
            return _vm.VMApi.Snapshot[name].Config.SetRest(description);
        }

        public Result Rollback(bool wait)
        {
            var result = (Result)_vm.VMApi.Snapshot[Name].Rollback.CreateRest();
            result.WaitForTaskToFinish(_vm, wait);
            return result;
        }

        public DateTime Date { get; }
        public string Parent => _apiData.parent;
        public string Name => _apiData.name;
        public string Description => (_apiData.description as string).TrimEnd();
        public bool Ram => !(_apiData.vmstate == 0);

        public static string HeaderInfo(bool showNodeAndVm)
        {
            var data = new List<string>();
            if (showNodeAndVm) { data.AddRange(new string[] { "NODE", "VM" }); }
            data.AddRange(new string[] { "TIME", "PARENT", "NAME", "DESCRIPTION", "RAM" });
            return string.Format(showNodeAndVm ? FORMATEX : FORMAT, data.ToArray());
        }

        public override string ToString() { return RowInfo(true); }

        public string RowInfo(bool showNodeAndVm)
        {
            var data = new List<string>();
            if (showNodeAndVm) { data.AddRange(new string[] { _vm.Node, _vm.Id, }); }
            data.AddRange(new string[]{ Date.ToString("yy/MM/dd HH:mm:ss"),
                                        Parent,
                                        Name,
                                        Description,
                                        Ram ? "X" : ""});

            return string.Format(showNodeAndVm ? FORMATEX : FORMAT, data.ToArray());
        }
    }
}