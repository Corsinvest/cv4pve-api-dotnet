using System.Linq;
using Corsinvest.ProxmoxVE.Api.Extension.Storage;

namespace Corsinvest.ProxmoxVE.Api.Extension.VM
{
    public abstract class Disk
    {
        protected Client Client { get; }
        private StorageInfo _storageInfo;

        internal Disk(Client client, string id, string definition)
        {
            Client = client;
            Id = id;
            Definition = definition;
            Backup = true;

            var data = definition.Split(':');
            Storage = data[0];

            data = data[1].Split(',');
            Name = data[0];
            Size = data.Where(a => a.StartsWith("size=")).Select(a => a.Substring(5)).FirstOrDefault();
        }

        public string Definition { get; }
        public string Id { get; }
        public string Storage { get; }
        public StorageInfo StorageInfo => _storageInfo ?? (_storageInfo = Client.GetStorages().FirstOrDefault(a => a.Id == Storage));
        public string Name { get; }
        public string Size { get; }
        public bool Backup { get; protected set; }
    }
}