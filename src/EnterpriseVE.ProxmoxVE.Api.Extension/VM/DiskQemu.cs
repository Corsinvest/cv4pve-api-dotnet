using System.Linq;

namespace EnterpriseVE.ProxmoxVE.Api.Extension.VM
{
    public class DiskQemu : Disk
    {
        internal DiskQemu(string id, string definition) : base(id, definition)
        {
            var backup = definition.Split(':')[1].Split(',').Where(a => a.StartsWith("backup=")).FirstOrDefault();
            Backup = backup == null ? true : backup == "1";
        }
    }
}