namespace Corsinvest.ProxmoxVE.Api.Extension.VM
{
    /// <summary>
    /// Backup function
    /// </summary>
    public class Backup
    {
        private VMInfo _vm;

        internal Backup(VMInfo vm) => _vm = vm;

        /// <summary>
        /// Information backup.
        /// </summary>
        /// <param name="volume"></param>
        /// <returns></returns>
        public Result Info(string volume) => _vm.NodeApi.Vzdump.Extractconfig.GetRest(volume);

        /// <summary>
        /// Create backup.
        /// </summary>
        /// <param name="mode"></param>
        /// <param name="compress"></param>
        /// <param name="mailTo"></param>
        /// <param name="mailnotification"></param>
        /// <param name="storage"></param>
        /// <param name="wait"></param>
        /// <returns></returns>
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