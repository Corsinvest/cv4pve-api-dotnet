using System.Linq;

namespace Corsinvest.ProxmoxVE.Api.Extension.VM
{
    /// <summary>
    /// Disk Qemu
    /// </summary>
    public class DiskQemu : Disk
    {
        internal DiskQemu(Client client, string id, string definition) : base(client, id, definition)
        {
            var backup = definition.Split(':')[1].Split(',').Where(a => a.StartsWith("backup=")).FirstOrDefault();
            Backup = backup == null ? true : backup == "1";
        }
    }
}