namespace Corsinvest.ProxmoxVE.Api.Extension.VM
{
    public class DiskLxc : Disk
    {
        internal DiskLxc(Client client, string id, string definition) : base(client, id, definition) { }
    }
}