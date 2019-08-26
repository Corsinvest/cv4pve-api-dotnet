namespace Corsinvest.ProxmoxVE.Api.Extension.VM
{
    /// <summary>
    /// Disk LXC
    /// </summary>
    public class DiskLxc : Disk
    {
        internal DiskLxc(Client client, string id, string definition) : base(client, id, definition) { }
    }
}