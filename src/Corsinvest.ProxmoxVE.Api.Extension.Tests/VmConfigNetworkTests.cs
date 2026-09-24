/*
 * SPDX-FileCopyrightText: Copyright Corsinvest Srl
 * SPDX-License-Identifier: MIT
 */

using Corsinvest.ProxmoxVE.Api.Shared.Models.Vm;
using Newtonsoft.Json;
using Xunit;

namespace Corsinvest.ProxmoxVE.Api.Extension.Tests;

public class VmConfigNetworkTests
{
    [Fact]
    public void Lxc_network_reads_trunks()
    {
        var config = JsonConvert.DeserializeObject<VmConfigLxc>(
            """{"net1":"name=eth1,bridge=vmbr2,hwaddr=BC:24:11:AA:BB:CC,tag=10,trunks=20;30-40,firewall=1,link_down=1,type=veth"}""")!;

        var net = Assert.Single(config.Networks);
        Assert.Equal("vmbr2", net.Bridge);
        Assert.Equal(10, net.Tag);
        Assert.Equal("20;30-40", net.Trunks);
        Assert.True(net.Firewall);
        Assert.True(net.LinkDown);
    }

    [Fact]
    public void Lxc_common_properties_are_the_same_through_the_base_type()
    {
        var lxc = JsonConvert.DeserializeObject<VmConfigLxc>(
            """{"arch":"amd64","memory":4096,"ostype":"debian","tags":"a;b","onboot":1,"lock":"backup","protection":1}""")!;
        VmConfig config = lxc;

        // Code that handles any guest through VmConfig must see the container's real values.
        Assert.Equal("amd64", config.Arch);
        Assert.Equal(4096, config.Memory);
        Assert.Equal("debian", config.OsType);
        Assert.Equal("a;b", config.Tags);
        Assert.True(config.OnBoot);
        Assert.Equal("backup", config.Lock);
        Assert.True(config.IsLocked);
        Assert.True(config.Protection);

        // And through the derived type, as before.
        Assert.True(lxc.OnBoot);
        Assert.Equal(4096, lxc.Memory);
    }

    [Fact]
    public void Qemu_network_reads_trunks()
    {
        var config = JsonConvert.DeserializeObject<VmConfigQemu>(
            """{"net1":"virtio=BC:24:11:AA:BB:CC,bridge=vmbr1,trunks=1-4095"}""")!;

        var net = Assert.Single(config.Networks);
        Assert.Equal("vmbr1", net.Bridge);
        Assert.Equal("1-4095", net.Trunks);
    }
}
