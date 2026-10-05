/*
 * SPDX-FileCopyrightText: Copyright Corsinvest Srl
 * SPDX-License-Identifier: MIT
 */

using System.Globalization;
using Corsinvest.ProxmoxVE.Api.Extension.Utils;
using Corsinvest.ProxmoxVE.Api.Shared.Models.Node;
using Corsinvest.ProxmoxVE.Api.Shared.Models.Vm;
using Corsinvest.ProxmoxVE.Api.Shared.Utils;
using Xunit;

namespace Corsinvest.ProxmoxVE.Api.Extension.Tests.Utils;

public class SmallHelpersTests
{
    [Fact]
    public void Names_with_numbers_are_sorted_as_a_person_would()
    {
        string?[] names = ["net10", "net2", "Net3", "net1", "vm", null, "vm1", "disk-2-b", "disk-2-a", "disk-10-a"];

        Assert.Equal([null, "disk-2-a", "disk-2-b", "disk-10-a", "net1", "net2", "Net3", "net10", "vm", "vm1"],
                     names.Order(NaturalStringComparer.Instance));
    }

    [Theory]
    [InlineData("a1", "a01", 0)]
    [InlineData("a1", "a1", 0)]
    [InlineData("a2", "a10", -1)]
    [InlineData("a10", "a2", 1)]
    [InlineData("abc", "ABC", 0)]
    [InlineData("a", "b", -1)]
    [InlineData("", "a", -1)]
    [InlineData(null, "a", -1)]
    [InlineData("a", null, 1)]
    [InlineData(null, null, 0)]
    [InlineData("12345678901234567890123", "12345678901234567890124", -1)]
    public void Natural_comparison(string? x, string? y, int expected)
        => Assert.Equal(expected, Math.Sign(NaturalStringComparer.Instance.Compare(x, y)));

    [Fact]
    public void Console_of_a_guest_depends_on_its_type()
    {
        Assert.Equal("kvm", NoVncHelper.GetConsoleType(VmType.Qemu));
        Assert.Equal("lxc", NoVncHelper.GetConsoleType(VmType.Lxc));
    }

    [Fact]
    public void Console_url_has_the_options_asked()
    {
        Assert.Equal("https://pve01:8006/?console=kvm&vmid=100&vmname=web01&node=pve01",
                     NoVncHelper.GetConsoleUrl("pve01", 8006, "kvm", "pve01", 100, "web01", false, false));
        Assert.Equal("https://pve01:8006/?console=lxc&vmid=200&vmname=ct01&node=pve02&novnc=1&resize=scale",
                     NoVncHelper.GetConsoleUrl("pve01", 8006, "lxc", "pve02", 200, "ct01", true, false, "&resize=scale"));
        Assert.EndsWith("&xtermjs=1", NoVncHelper.GetConsoleUrl("pve01", 8006, "lxc", "pve02", 200, "ct01", false, true));
    }

    [Fact]
    public void Vnc_websocket_url_of_a_guest_and_of_a_node()
    {
        Assert.Equal("wss://pve01:8006/api2/json/nodes/pve02/qemu/100/vncwebsocket?port=5900&vncticket=TICKET",
                     NoVncHelper.GetWebsocketUrl("pve01", 8006, "pve02", "qemu", 100, 5900, "TICKET"));
        Assert.Equal("wss://pve01:8006/api2/json/nodes/pve02/vncwebsocket?port=5901&vncticket=TICKET",
                     NoVncHelper.GetWebsocketUrl("pve01", 8006, "pve02", 5901, "TICKET"));
    }

    [Fact]
    public void Paths_of_the_web_interface()
    {
        Assert.Equal("nodes/pve01", PveWebUrlHelper.GetWebUrlNode("pve01"));
        Assert.Equal("nodes/pve01/qemu/100", PveWebUrlHelper.GetWebUrlQemu("pve01", 100));
        Assert.Equal("nodes/pve01/lxc/200", PveWebUrlHelper.GetWebUrlLxc("pve01", 200));
        Assert.Equal("nodes/pve01/storage/local", PveWebUrlHelper.GetWebUrlStorage("pve01", "local"));
        Assert.Equal("pool/customer1", PveWebUrlHelper.GetWebUrlPool("customer1"));
    }

    [Fact]
    public void Download_of_a_file_of_a_backup_encodes_volume_and_path()
    {
        var url = BackupHelper.GetDownloadFileUrl("pve01", 8006, "pve02", "pbs01", "pbs01:backup/vm/100/2026-10-01T00:00:00Z", "/drive-scsi0.img.fidx/etc/host name");

        Assert.StartsWith("https://pve01:8006/api2/json/nodes/pve02/storage/pbs01/file-restore/download?", url);
        var query = System.Web.HttpUtility.ParseQueryString(new Uri(url).Query);
        Assert.Equal("pbs01:backup/vm/100/2026-10-01T00:00:00Z", query["volume"]);
        Assert.Equal("/drive-scsi0.img.fidx/etc/host name", query["filepath"]);
    }

    [Theory]
    [InlineData("d", "etc", "etc.zip")]
    [InlineData("f", "hosts", "hosts")]
    [InlineData("v", "drive-scsi0.img", "drive-scsi0.img")]
    public void Folder_of_a_backup_is_downloaded_as_a_zip(string type, string name, string expected)
        => Assert.Equal(expected, BackupHelper.GetDownloadFileName(type, name));

    [Theory]
    [InlineData("c", NodeLevel.Community)]
    [InlineData("b", NodeLevel.Basic)]
    [InlineData("s", NodeLevel.Standard)]
    [InlineData("p", NodeLevel.Premium)]
    [InlineData("", NodeLevel.None)]
    [InlineData(null, NodeLevel.None)]
    [InlineData("x", NodeLevel.None)]
    public void Subscription_level_is_decoded(string? level, NodeLevel expected)
        => Assert.Equal(expected, NodeHelper.DecodeLevelSupport(level!));

    private const string V2 = "fpu sse sse2 cx16 lahf_lm popcnt sse4_1 sse4_2 ssse3 aes";
    private const string V3 = V2 + " avx avx2 bmi1 bmi2 fma f16c abm movbe";
    private const string V4 = V3 + " avx512f avx512bw avx512cd avx512dq avx512vl";

    [Theory]
    [InlineData(null, "x86-64-v1")]
    [InlineData("", "x86-64-v1")]
    [InlineData("fpu sse sse2", "x86-64-v1")]
    [InlineData(V2, "x86-64-v2-AES")]
    [InlineData(V2 + " avx avx2", "x86-64-v2-AES")]
    [InlineData(V3, "x86-64-v3")]
    [InlineData(V3 + " avx512f", "x86-64-v3")]
    [InlineData(V4, "x86-64-v4")]
    [InlineData("CX16 LAHF_LM POPCNT SSE4_1 SSE4_2 SSSE3", "x86-64-v2-AES")]
    public void Cpu_level_is_read_from_the_flags(string? flags, string expected)
        => Assert.Equal(expected, NodeHelper.GetCpuX86Level(flags!).ToString());

    [Fact]
    public void Cpu_levels_are_ordered_so_the_lowest_of_a_cluster_can_be_taken()
    {
        CpuX86Level[] levels = [CpuX86Level.V3, CpuX86Level.V4, CpuX86Level.V2Aes];

        Assert.Same(CpuX86Level.V2Aes, levels.Min());
        Assert.Equal(2, CpuX86Level.V2Aes.Level);
        Assert.True(CpuX86Level.V1.CompareTo(null!) > 0);
    }

    [Fact]
    public void Format_provider_writes_the_values_of_the_api_as_text()
    {
        var culture = CultureInfo.CurrentCulture;
        CultureInfo.CurrentCulture = CultureInfo.InvariantCulture;
        try
        {
            var provider = new PveFormatProvider();

            Assert.Equal("1.5 GB", string.Format(provider, FormatHelper.DataFormatBytes, 1_500_000_000));
            Assert.Equal("1 KB", string.Format(provider, "{0:" + FormatHelper.FormatBits + "}", 1_000));
            Assert.Equal("1 days 01:01:01", string.Format(provider, FormatHelper.DataFormatUptimeUnixTime, 90_061));
            Assert.Equal("Thu, 01 Jan 2026 00:00:00 GMT", string.Format(provider, FormatHelper.DataFormatUnixTime, 1_767_225_600));
            Assert.Equal("007", string.Format(provider, "{0:000}", 7));
            Assert.Same(provider, provider.GetFormat(typeof(ICustomFormatter)));
            Assert.Null(provider.GetFormat(typeof(NumberFormatInfo)));
        }
        finally
        {
            CultureInfo.CurrentCulture = culture;
        }
    }
}
