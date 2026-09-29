/*
 * SPDX-FileCopyrightText: Copyright Corsinvest Srl
 * SPDX-License-Identifier: MIT
 */

using Corsinvest.ProxmoxVE.Api.Shared.Models.Vm;
using Newtonsoft.Json;
using Xunit;

namespace Corsinvest.ProxmoxVE.Api.Extension.Tests;

public class VmConfigSpiceTests
{
    [Theory]
    [InlineData("qxl", true)]
    [InlineData("qxl2", true)]
    [InlineData("qxl4,memory=32", true)]
    [InlineData("type=qxl3,memory=64", true)]
    [InlineData("memory=16,type=qxl", true)]
    [InlineData("virtio", true)]
    [InlineData("virtio-gl,clipboard=vnc", true)]
    [InlineData("type=virtio,memory=256", true)]
    [InlineData("std", false)]
    [InlineData("std,clipboard=vnc", false)]
    [InlineData("vmware", false)]
    [InlineData("serial0", false)]
    public void Spice_display_is_a_qxl_or_virtio_vga_type(string vga, bool expected)
        => Assert.Equal(expected, new VmConfigQemu { Vga = vga }.IsSpiceDisplay);

    [Fact]
    public void Vga_not_set_is_not_spice()
        => Assert.False(new VmConfigQemu().IsSpiceDisplay);

    [Theory]
    [InlineData("device=ich9-intel-hda,driver=spice", true)]
    [InlineData("device=ich9-intel-hda", true)]
    [InlineData("driver=spice,device=AC97", true)]
    [InlineData("device=intel-hda,driver=none", false)]
    public void Spice_audio_is_an_audio_device_with_the_spice_driver(string audio0, bool expected)
        => Assert.Equal(expected, new VmConfigQemu { Audio0 = audio0 }.HasSpiceAudio);

    [Fact]
    public void Audio_not_set_is_not_spice()
        => Assert.False(new VmConfigQemu().HasSpiceAudio);

    [Theory]
    [InlineData("""{"usb0":"spice,usb3=1"}""", true)]
    [InlineData("""{"usb0":"host=spice"}""", true)]
    [InlineData("""{"usb2":"SPICE"}""", true)]
    [InlineData("""{"usb0":"host=1234:5678","usb1":"spice"}""", true)]
    [InlineData("""{"usb0":"host=1234:5678"}""", false)]
    [InlineData("""{"usb0":"1-2.3,usb3=1"}""", false)]
    [InlineData("""{"usbx":"spice"}""", false)]
    [InlineData("""{"name":"vm"}""", false)]
    public void Spice_usb_is_a_usb_port_with_host_spice(string json, bool expected)
        => Assert.Equal(expected, JsonConvert.DeserializeObject<VmConfigQemu>(json)!.HasSpiceUsb);

    [Theory]
    [InlineData("qxl", 1)]
    [InlineData("qxl2", 2)]
    [InlineData("type=qxl3", 3)]
    [InlineData("qxl4,memory=64", 4)]
    [InlineData("virtio", 1)]
    [InlineData("std", 0)]
    public void Spice_monitors_follow_the_qxl_type(string vga, int expected)
        => Assert.Equal(expected, new VmConfigQemu { Vga = vga }.SpiceMonitors);

    [Fact]
    public void Vga_not_set_has_no_spice_monitors()
        => Assert.Equal(0, new VmConfigQemu().SpiceMonitors);

    [Theory]
    [InlineData("foldersharing=1", true)]
    [InlineData("foldersharing=on,videostreaming=all", true)]
    [InlineData("videostreaming=filter,foldersharing=1", true)]
    [InlineData("foldersharing=0", false)]
    [InlineData("videostreaming=all", false)]
    public void Spice_folder_sharing_reads_the_enhancements(string enhancements, bool expected)
        => Assert.Equal(expected, new VmConfigQemu { SpiceEnhancements = enhancements }.HasSpiceFolderSharing);

    [Fact]
    public void Enhancements_not_set_have_no_folder_sharing()
        => Assert.False(new VmConfigQemu().HasSpiceFolderSharing);
}
