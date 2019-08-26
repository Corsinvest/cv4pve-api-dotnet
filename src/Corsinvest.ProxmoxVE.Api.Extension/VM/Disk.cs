using System.Linq;
using Corsinvest.ProxmoxVE.Api.Extension.Storage;

namespace Corsinvest.ProxmoxVE.Api.Extension.VM
{
    /// <summary>
    /// Disk
    /// </summary>
    public abstract class Disk
    {
        /// <summary>
        /// Client API.
        /// </summary>
        protected Client Client { get; }

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

        /// <summary>
        /// dEFINITION
        /// </summary>
        /// <value></value>
        public string Definition { get; }

        /// <summary>
        /// Identifier
        /// </summary>
        public string Id { get; }

        /// <summary>
        /// Storage
        /// </summary>
        /// <value></value>
        public string Storage { get; }

        private StorageInfo _storageInfo;
        /// <summary>
        /// Storage info
        /// </summary>
        /// <returns></returns>
        public StorageInfo StorageInfo => _storageInfo ?? (_storageInfo = Client.GetStorages().FirstOrDefault(a => a.Id == Storage));

        /// <summary>
        /// Name
        /// </summary>
        /// <value></value>
        public string Name { get; }

        /// <summary>
        /// Size
        /// </summary>
        public string Size { get; }

        /// <summary>
        /// Backup enabled.
        /// </summary>
        public bool Backup { get; protected set; }
    }
}