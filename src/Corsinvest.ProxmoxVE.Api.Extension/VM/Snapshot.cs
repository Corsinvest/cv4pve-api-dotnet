using System;
using System.Collections.Generic;
using Corsinvest.ProxmoxVE.Api.Extension.Utils;

namespace Corsinvest.ProxmoxVE.Api.Extension.VM
{
    /// <summary>
    /// Snapshot
    /// </summary>
    public class Snapshot
    {
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

        /// <summary>
        /// Config
        /// </summary>
        /// <value></value>
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

        /// <summary>
        /// Update
        /// </summary>
        /// <param name="name"></param>
        /// <param name="description"></param>
        /// <returns></returns>
        public Result Update(string name, string description)
        {
            switch (_vm.Type)
            {
                case VMTypeEnum.Qemu: return _vm.QemuApi.Snapshot[Name].Config.SetRest(description);
                case VMTypeEnum.Lxc: return _vm.LxcApi.Snapshot[Name].Config.SetRest(description);
                default: return null;
            }
        }

        /// <summary>
        /// Rollback
        /// </summary>
        /// <param name="wait"></param>
        /// <returns></returns>
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

        /// <summary>
        /// Date
        /// </summary>
        /// <value></value>
        public DateTime Date { get; }

        /// <summary>
        /// Parent
        /// </summary>
        public string Parent => _apiData.parent;

        /// <summary>
        /// Name
        /// </summary>
        public string Name => _apiData.name;

        /// <summary>
        /// Description
        /// </summary>
        public string Description => (_apiData.description as string).TrimEnd();

        /// <summary>
        /// Ram used
        /// </summary>
        public bool Ram => !(_apiData.vmstate == 0);

        /// <summary>
        /// Get title info
        /// </summary>
        /// <param name="showNodeAndVm"></param>
        /// <returns></returns>
        public static string[] GetTitlesInfo(bool showNodeAndVm)
        {
            var data = new List<string>();
            if (showNodeAndVm) { data.AddRange(new string[] { "NODE", "VM" }); }
            data.AddRange(new string[] { "TIME", "PARENT", "NAME", "DESCRIPTION", "RAM" });
            return data.ToArray();
        }

        /// <summary>
        /// Row info
        /// </summary>
        /// <param name="showNodeAndVm"></param>
        /// <returns></returns>
        public string[] GetRowInfo(bool showNodeAndVm)
        {
            var data = new List<string>();
            if (showNodeAndVm) { data.AddRange(new string[] { _vm.Node, _vm.Id, }); }
            data.AddRange(new string[]{ Date.ToString("yy/MM/dd HH:mm:ss"),
                                        Parent,
                                        Name,
                                        Description,
                                        Ram ? "X" : ""});

            return data.ToArray();
        }
    }
}