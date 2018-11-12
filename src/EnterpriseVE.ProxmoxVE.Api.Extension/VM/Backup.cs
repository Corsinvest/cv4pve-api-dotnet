namespace EnterpriseVE.ProxmoxVE.Api.Extension.VM
{
    public class Backup
    {
        private VMInfo _vm;

        internal Backup(VMInfo vm) { _vm = vm; }

        //dump/vzdump-qemu-104-

        public Result Info(string volume) { return _vm.NodeApi.Vzdump.Extractconfig.GetRest(volume); }

        public Result Create(string mode,
                             string compress,
                             string mailTo,
                             BackupMailNotificationEnum mailnotification,
                             string storage,
                             bool wait)
        {
            var result = _vm.NodeApi.Vzdump.CreateRest(vmid: _vm.Id,
                                                   mode: mode,
                                                   compress: compress,
                                                   remove: false,
                                                   mailto: mailTo,
                                                   mailnotification: (mailnotification + "").ToLower(),
                                                   storage: storage);

            result.WaitForTaskToFinish(_vm, wait);
            return result;
        }
    }
}