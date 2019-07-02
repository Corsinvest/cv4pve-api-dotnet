using System;
using System.Collections.Generic;
using Corsinvest.ProxmoxVE.Api.Extension.Utils;

namespace Corsinvest.ProxmoxVE.Api.Extension.VM
{
    public class Snapshot
    {
        private const string FORMAT = "{0,-17} {1,-25} {2,-25} {3,-25} {4,-1}";
        private const string FORMATEX = "{0,-10} {1,5} {2,-17} {3,-25} {4,-25} {5,-25} {6,-1}";

        private VMInfo _vm;
        private dynamic _apiData;

        internal Snapshot(VMInfo vm, object apiData)
        {
            _apiData = apiData;
            _vm = vm;

            JsonHelper.GetValueOrCreate(_apiData, "description", "no-description");
            JsonHelper.GetValueOrCreate(_apiData, "parent", "no-parent");
            JsonHelper.GetValueOrCreate(_apiData, "snaptime");
            JsonHelper.GetValueOrCreate(_apiData, "vmstate", 0);

            Date = _apiData.snaptime == null ?
                   DateTime.Now :
                   DateTimeOffset.FromUnixTimeSeconds(_apiData.snaptime).DateTime;
        }

        public Result Config
        {
            get
            {
                switch (_vm.Type)
                {
                    case VMTypeEnum.Qemu: return _vm.QemuApi.Snapshot[Name].Config.GetRest();
                    case VMTypeEnum.Lxc: return _vm.LxcApi.Snapshot[Name].Config.GetRest();
                    default: return null;
                }
            }
        }

        public Result Update(string name, string description)
        {
            switch (_vm.Type)
            {
                case VMTypeEnum.Qemu: return _vm.QemuApi.Snapshot[Name].Config.SetRest(description);
                case VMTypeEnum.Lxc: return _vm.LxcApi.Snapshot[Name].Config.SetRest(description);
                default: return null;
            }
        }

        public Result Rollback(bool wait)
        {
            Result result = null;
            switch (_vm.Type)
            {
                case VMTypeEnum.Qemu: result = _vm.QemuApi.Snapshot[Name].Rollback.CreateRest(); break;
                case VMTypeEnum.Lxc: result = _vm.LxcApi.Snapshot[Name].Rollback.CreateRest(); break;
            }
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

        public override string ToString() => RowInfo(true);

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