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

    [Fact]
    public void Qemu_options_absent_from_the_payload_are_the_pve_defaults()
    {
        var qemu = JsonConvert.DeserializeObject<VmConfigQemu>("""{"name":"vm"}""")!;

        // PVE omits an option when it holds the default, so absent must not read as 0/false/null.
        Assert.True(qemu.Reboot);
        Assert.True(qemu.Ciupgrade);
        Assert.Equal("lsi", qemu.ScsiHw);
    }

    [Fact]
    public void Qemu_options_set_to_zero_stay_off()
    {
        var qemu = JsonConvert.DeserializeObject<VmConfigQemu>(
            """{"reboot":0,"ciupgrade":0,"scsihw":"virtio-scsi-single"}""")!;

        Assert.False(qemu.Reboot);
        Assert.False(qemu.Ciupgrade);
        Assert.Equal("virtio-scsi-single", qemu.ScsiHw);
    }

    [Fact]
    public void Lxc_options_absent_from_the_payload_are_the_pve_defaults()
    {
        var lxc = JsonConvert.DeserializeObject<VmConfigLxc>("""{"hostname":"ct"}""")!;

        Assert.True(lxc.Console);
        Assert.Equal(2, lxc.Tty);
        Assert.Equal("tty", lxc.Cmode);
        Assert.Equal("amd64", lxc.Arch);
    }

    [Fact]
    public void Lxc_options_set_explicitly_win_over_the_defaults()
    {
        var lxc = JsonConvert.DeserializeObject<VmConfigLxc>(
            """{"console":0,"tty":0,"cmode":"shell","arch":"arm64"}""")!;

        Assert.False(lxc.Console);
        Assert.Equal(0, lxc.Tty);
        Assert.Equal("shell", lxc.Cmode);
        Assert.Equal("arm64", lxc.Arch);
    }

    [Fact]
    public void Lxc_arch_is_the_same_value_through_the_base_type()
    {
        // Arch is declared on VmConfig and overridden on VmConfigLxc to carry the LXC default:
        // one backing store, so both views agree.
        var lxc = JsonConvert.DeserializeObject<VmConfigLxc>("""{"arch":"riscv64"}""")!;
        Assert.Equal("riscv64", ((VmConfig)lxc).Arch);

        var byDefault = JsonConvert.DeserializeObject<VmConfigLxc>("""{"hostname":"ct"}""")!;
        Assert.Equal("amd64", ((VmConfig)byDefault).Arch);
    }

    [Fact]
    public void Qemu_has_no_arch_option_so_it_stays_null()
    {
        // 'arch' is an LXC-only option: a VM must not claim an architecture it never reported.
        Assert.Null(JsonConvert.DeserializeObject<VmConfigQemu>("""{"name":"vm"}""")!.Arch);
    }
}
