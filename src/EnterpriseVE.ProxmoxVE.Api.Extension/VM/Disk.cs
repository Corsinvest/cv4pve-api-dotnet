using System.Linq;
using EnterpriseVE.ProxmoxVE.Api;

namespace EnterpriseVE.ProxmoxVE.Api.Extension.VM
{
    public abstract class Disk
    {
        internal Disk(string id, string definition)
        {
            Id = id;
            Definition = definition;
            Backup = true;

            var data = definition.Split(':');
            Storage = data[0];

            data = data[1].Split(',');
            Name = data[0];
            Size = data.Where(a => a.StartsWith("size=")).FirstOrDefault();
        }

        public string Definition { get; }
        public string Id { get; }
        public string Storage { get; protected set; }
        public string Name { get; protected set; }
        public string Size { get; protected set; }
        public bool Backup { get; protected set; }
    }
}