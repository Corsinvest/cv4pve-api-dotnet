/*
 * SPDX-FileCopyrightText: Copyright Corsinvest Srl
 * SPDX-License-Identifier: MIT
 */

using Corsinvest.ProxmoxVE.Api.Shared.Models.Access;
using Corsinvest.ProxmoxVE.Api.Shared.Models.Node;
using Corsinvest.ProxmoxVE.Api.Shared.Models.Vm;
using Newtonsoft.Json;
using Xunit;

namespace Corsinvest.ProxmoxVE.Api.Extension.Tests;

public class VmConfigDefaultsTests
{
    [Theory]
    [InlineData("1", true)]
    [InlineData("1,fstrim_cloned_disks=1", true)]
    [InlineData("enabled=1", true)]
    [InlineData("enabled=1,fstrim_cloned_disks=1,type=virtio", true)]
    [InlineData("0", false)]
    [InlineData("enabled=0,fstrim_cloned_disks=1", false)]
    [InlineData("fstrim_cloned_disks=1", false)]
    public void Agent_enabled_reads_the_bare_value_and_the_enabled_key(string agent, bool expected)
        => Assert.Equal(expected, new VmConfigQemu { Agent = agent }.AgentEnabled);

    [Fact]
    public void Agent_not_set_is_disabled()
        => Assert.False(new VmConfigQemu().AgentEnabled);

    [Theory]
    [InlineData("local-lvm:vm-100-cloudinit,media=cdrom")]
    [InlineData("local:100/vm-100-cloudinit.qcow2,media=cdrom")]
    [InlineData("nfs01:100/vm-100-cloudinit.raw,media=cdrom")]
    public void Cloud_init_drive_is_recognised_on_block_and_file_storages(string ide2)
    {
        var config = JsonConvert.DeserializeObject<VmConfigQemu>($$"""{"ide2":"{{ide2}}"}""")!;

        Assert.Equal(VmDiskKind.CloudInit, Assert.Single(config.DisksAll).Kind);
    }

    [Fact]
    public void Iso_image_is_still_a_cdrom()
    {
        var config = JsonConvert.DeserializeObject<VmConfigQemu>("""{"ide2":"local:iso/debian-12.iso,media=cdrom"}""")!;

        Assert.Equal(VmDiskKind.Cdrom, Assert.Single(config.DisksAll).Kind);
    }

    [Fact]
    public void Memory_and_swap_not_set_are_the_pve_defaults()
    {
        var lxc = JsonConvert.DeserializeObject<VmConfigLxc>("""{"hostname":"ct"}""")!;
        var qemu = JsonConvert.DeserializeObject<VmConfigQemu>("""{"name":"vm"}""")!;

        Assert.Equal(512, lxc.Memory);
        Assert.Equal(512, lxc.Swap);
        Assert.Equal(512, qemu.Memory);
    }

    [Fact]
    public void Memory_and_swap_set_to_zero_stay_zero()
    {
        var lxc = JsonConvert.DeserializeObject<VmConfigLxc>("""{"memory":0,"swap":0}""")!;

        Assert.Equal(0, lxc.Memory);
        Assert.Equal(0, lxc.Swap);
    }

    [Theory]
    [InlineData("ssd", true)]
    [InlineData("nvme", true)]
    [InlineData("hdd", false)]
    [InlineData("usb", false)]
    public void Nvme_disks_are_solid_state(string type, bool expected)
        => Assert.Equal(expected, new NodeDiskList { Type = type }.IsSsd);

    [Fact]
    public void Tfa_entry_is_enabled_unless_enable_is_false()
    {
        var tfa = JsonConvert.DeserializeObject<AccessTfa>(
            """{"userid":"root@pam","entries":[{"id":"a","type":"totp"},{"id":"b","type":"totp","enable":0},{"id":"c","type":"webauthn","enable":1}]}""")!;

        Assert.Equal(["a", "c"], tfa.Entries.Where(e => e.Enable).Select(e => e.Id));
    }
}
