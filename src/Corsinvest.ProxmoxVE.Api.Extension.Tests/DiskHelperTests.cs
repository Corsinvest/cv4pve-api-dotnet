/*
 * SPDX-FileCopyrightText: Copyright Corsinvest Srl
 * SPDX-License-Identifier: MIT
 */

using System.ComponentModel;
using System.Net;
using Corsinvest.ProxmoxVE.Api.Extension.Utils;
using Corsinvest.ProxmoxVE.Api.Shared.Models.Vm;
using Newtonsoft.Json.Linq;
using Xunit;

namespace Corsinvest.ProxmoxVE.Api.Extension.Tests;

/// <summary>
/// Unit tests for <see cref="DiskHelper"/> covering slot allocation, limits, attach, detach, and remove unused disks.
/// </summary>
public class DiskHelperTests
{
    private static JObject ParseRequestBody(HttpRequestMessage request)
    {
        var raw = request.Content?.ReadAsStringAsync().GetAwaiter().GetResult();
        return string.IsNullOrWhiteSpace(raw) ? [] : JObject.Parse(raw);
    }

    #region GetFreeSlotAsync

    [Fact]
    public async Task GetFreeSlotAsync_Qemu_DefaultScsi_ReturnsFirstSlotWhenEmpty()
    {
        var handler = new FakeHandler(req =>
        {
            var path = req.RequestUri!.AbsolutePath;
            if (path.EndsWith("/config"))
            {
                return FakeHandler.Json(HttpStatusCode.OK, """{"data":{}}""");
            }
            if (path.EndsWith("/pending"))
            {
                return FakeHandler.Json(HttpStatusCode.OK, """{"data":[]}""");
            }
            return FakeHandler.Json(HttpStatusCode.NotFound, "{}");
        });

        var client = FakeHandler.Client(handler);
        var slot = await DiskHelper.GetFreeSlotAsync(client, "pve01", VmType.Qemu, 100);

        Assert.Equal("scsi0", slot);
    }

    [Fact]
    public async Task GetFreeSlotAsync_Qemu_ConsidersConfigAndPending_ReturnsLowestFreeSlot()
    {
        var handler = new FakeHandler(req =>
        {
            var path = req.RequestUri!.AbsolutePath;
            if (path.EndsWith("/config"))
            {
                return FakeHandler.Json(HttpStatusCode.OK, """
                    {"data":{
                        "scsi0":"local-zfs:vm-100-disk-0,size=20G",
                        "scsi1":"local-zfs:vm-100-disk-1,size=30G"
                    }}
                    """);
            }
            if (path.EndsWith("/pending"))
            {
                return FakeHandler.Json(HttpStatusCode.OK, """
                    {"data":[
                        {"key":"scsi2","value":"local-zfs:vm-100-disk-2,size=10G"}
                    ]}
                    """);
            }
            return FakeHandler.Json(HttpStatusCode.NotFound, "{}");
        });

        var client = FakeHandler.Client(handler);
        var slot = await DiskHelper.GetFreeSlotAsync(client, "pve01", VmType.Qemu, 100, bus: "scsi");

        Assert.Equal("scsi3", slot);
    }

    [Fact]
    public async Task GetFreeSlotAsync_Qemu_ScsiLimit31_ThrowsWhenFull()
    {
        var handler = new FakeHandler(req =>
        {
            var path = req.RequestUri!.AbsolutePath;
            if (path.EndsWith("/config"))
            {
                var configData = new JObject();
                for (var i = 0; i < 31; i++)
                {
                    configData[$"scsi{i}"] = $"local-zfs:vm-100-disk-{i},size=10G";
                }
                return FakeHandler.Json(HttpStatusCode.OK, new JObject { ["data"] = configData }.ToString());
            }
            if (path.EndsWith("/pending"))
            {
                return FakeHandler.Json(HttpStatusCode.OK, """{"data":[]}""");
            }
            return FakeHandler.Json(HttpStatusCode.NotFound, "{}");
        });

        var client = FakeHandler.Client(handler);
        var ex = await Assert.ThrowsAsync<InvalidOperationException>(() =>
            DiskHelper.GetFreeSlotAsync(client, "pve01", VmType.Qemu, 100, bus: "scsi"));

        Assert.Contains("No free SCSI slots available on Qemu 100", ex.Message);
        Assert.Contains("scsi0..scsi30", ex.Message);
    }

    [Fact]
    public async Task GetFreeSlotAsync_Qemu_Virtio_ConsidersPendingAndAllocatesSlot()
    {
        var handler = new FakeHandler(req =>
        {
            var path = req.RequestUri!.AbsolutePath;
            if (path.EndsWith("/config"))
            {
                return FakeHandler.Json(HttpStatusCode.OK, """
                    {"data":{
                        "virtio0":"local-zfs:vm-100-disk-0"
                    }}
                    """);
            }
            if (path.EndsWith("/pending"))
            {
                return FakeHandler.Json(HttpStatusCode.OK, """
                    {"data":[
                        {"key":"virtio1","value":"local-zfs:vm-100-disk-1"}
                    ]}
                    """);
            }
            return FakeHandler.Json(HttpStatusCode.NotFound, "{}");
        });

        var client = FakeHandler.Client(handler);
        var slot = await DiskHelper.GetFreeSlotAsync(client, "pve01", VmType.Qemu, 100, bus: "virtio");

        Assert.Equal("virtio2", slot);
    }

    [Fact]
    public async Task GetFreeSlotAsync_Qemu_VirtioLimit16_ThrowsWhenFull()
    {
        var handler = new FakeHandler(req =>
        {
            var path = req.RequestUri!.AbsolutePath;
            if (path.EndsWith("/config"))
            {
                var configData = new JObject();
                for (var i = 0; i < 16; i++)
                {
                    configData[$"virtio{i}"] = $"local-zfs:vm-100-disk-{i}";
                }
                return FakeHandler.Json(HttpStatusCode.OK, new JObject { ["data"] = configData }.ToString());
            }
            if (path.EndsWith("/pending"))
            {
                return FakeHandler.Json(HttpStatusCode.OK, """{"data":[]}""");
            }
            return FakeHandler.Json(HttpStatusCode.NotFound, "{}");
        });

        var client = FakeHandler.Client(handler);
        var ex = await Assert.ThrowsAsync<InvalidOperationException>(() =>
            DiskHelper.GetFreeSlotAsync(client, "pve01", VmType.Qemu, 100, bus: "virtio"));

        Assert.Contains("No free VIRTIO slots available on Qemu 100", ex.Message);
        Assert.Contains("virtio0..virtio15", ex.Message);
    }

    [Fact]
    public async Task GetFreeSlotAsync_Qemu_Sata_AllocatesSlot()
    {
        var handler = new FakeHandler(req =>
        {
            var path = req.RequestUri!.AbsolutePath;
            if (path.EndsWith("/config"))
            {
                return FakeHandler.Json(HttpStatusCode.OK, """{"data":{"sata0":"local-zfs:vm-100-disk-0"}}""");
            }
            if (path.EndsWith("/pending"))
            {
                return FakeHandler.Json(HttpStatusCode.OK, """{"data":[]}""");
            }
            return FakeHandler.Json(HttpStatusCode.NotFound, "{}");
        });

        var client = FakeHandler.Client(handler);
        var slot = await DiskHelper.GetFreeSlotAsync(client, "pve01", VmType.Qemu, 100, bus: "sata");

        Assert.Equal("sata1", slot);
    }

    [Fact]
    public async Task GetFreeSlotAsync_Qemu_SataLimit6_ThrowsWhenFull()
    {
        var handler = new FakeHandler(req =>
        {
            var path = req.RequestUri!.AbsolutePath;
            if (path.EndsWith("/config"))
            {
                var configData = new JObject();
                for (var i = 0; i < 6; i++)
                {
                    configData[$"sata{i}"] = $"local-zfs:vm-100-disk-{i}";
                }
                return FakeHandler.Json(HttpStatusCode.OK, new JObject { ["data"] = configData }.ToString());
            }
            if (path.EndsWith("/pending"))
            {
                return FakeHandler.Json(HttpStatusCode.OK, """{"data":[]}""");
            }
            return FakeHandler.Json(HttpStatusCode.NotFound, "{}");
        });

        var client = FakeHandler.Client(handler);
        var ex = await Assert.ThrowsAsync<InvalidOperationException>(() =>
            DiskHelper.GetFreeSlotAsync(client, "pve01", VmType.Qemu, 100, bus: "sata"));

        Assert.Contains("No free SATA slots available on Qemu 100", ex.Message);
        Assert.Contains("sata0..sata5", ex.Message);
    }

    [Fact]
    public async Task GetFreeSlotAsync_Qemu_Ide_AllocatesSlot()
    {
        var handler = new FakeHandler(req =>
        {
            var path = req.RequestUri!.AbsolutePath;
            if (path.EndsWith("/config"))
            {
                return FakeHandler.Json(HttpStatusCode.OK, """{"data":{"ide0":"local-zfs:vm-100-disk-0"}}""");
            }
            if (path.EndsWith("/pending"))
            {
                return FakeHandler.Json(HttpStatusCode.OK, """{"data":[]}""");
            }
            return FakeHandler.Json(HttpStatusCode.NotFound, "{}");
        });

        var client = FakeHandler.Client(handler);
        var slot = await DiskHelper.GetFreeSlotAsync(client, "pve01", VmType.Qemu, 100, bus: "ide");

        Assert.Equal("ide1", slot);
    }

    [Fact]
    public async Task GetFreeSlotAsync_Qemu_IdeLimit4_ThrowsWhenFull()
    {
        var handler = new FakeHandler(req =>
        {
            var path = req.RequestUri!.AbsolutePath;
            if (path.EndsWith("/config"))
            {
                var configData = new JObject();
                for (var i = 0; i < 4; i++)
                {
                    configData[$"ide{i}"] = $"local-zfs:vm-100-disk-{i}";
                }
                return FakeHandler.Json(HttpStatusCode.OK, new JObject { ["data"] = configData }.ToString());
            }
            if (path.EndsWith("/pending"))
            {
                return FakeHandler.Json(HttpStatusCode.OK, """{"data":[]}""");
            }
            return FakeHandler.Json(HttpStatusCode.NotFound, "{}");
        });

        var client = FakeHandler.Client(handler);
        var ex = await Assert.ThrowsAsync<InvalidOperationException>(() =>
            DiskHelper.GetFreeSlotAsync(client, "pve01", VmType.Qemu, 100, bus: "ide"));

        Assert.Contains("No free IDE slots available on Qemu 100", ex.Message);
        Assert.Contains("ide0..ide3", ex.Message);
    }

    [Fact]
    public async Task GetFreeSlotAsync_Qemu_ThrowsOnInvalidBus()
    {
        var client = FakeHandler.Client(new FakeHandler(req => FakeHandler.Json(HttpStatusCode.OK, "{}")));
        await Assert.ThrowsAsync<ArgumentException>(() =>
            DiskHelper.GetFreeSlotAsync(client, "pve01", VmType.Qemu, 100, bus: "nvme"));
    }

    [Fact]
    public async Task GetFreeSlotAsync_Lxc_DefaultBusMapsToMp_AndFindsSlot()
    {
        var handler = new FakeHandler(req =>
        {
            var path = req.RequestUri!.AbsolutePath;
            if (path.EndsWith("/config"))
            {
                return FakeHandler.Json(HttpStatusCode.OK, """{"data":{"mp0":"local-zfs:subvol-200-disk-0,mp=/mnt/a"}}""");
            }
            if (path.EndsWith("/pending"))
            {
                return FakeHandler.Json(HttpStatusCode.OK, """
                    {"data":[
                        {"key":"mp1","value":"local-zfs:subvol-200-disk-1,mp=/mnt/b"}
                    ]}
                    """);
            }
            return FakeHandler.Json(HttpStatusCode.NotFound, "{}");
        });

        var client = FakeHandler.Client(handler);
        // Default bus is "scsi", which DiskHelper maps to "mp" for LXC
        var slot = await DiskHelper.GetFreeSlotAsync(client, "pve01", VmType.Lxc, 200);

        Assert.Equal("mp2", slot);
    }

    [Fact]
    public async Task GetFreeSlotAsync_Lxc_ExplicitMpBus_FindsSlot()
    {
        var handler = new FakeHandler(req =>
        {
            var path = req.RequestUri!.AbsolutePath;
            if (path.EndsWith("/config"))
            {
                return FakeHandler.Json(HttpStatusCode.OK, """{"data":{}}""");
            }
            if (path.EndsWith("/pending"))
            {
                return FakeHandler.Json(HttpStatusCode.OK, """{"data":[]}""");
            }
            return FakeHandler.Json(HttpStatusCode.NotFound, "{}");
        });

        var client = FakeHandler.Client(handler);
        var slot = await DiskHelper.GetFreeSlotAsync(client, "pve01", VmType.Lxc, 200, bus: "mp");

        Assert.Equal("mp0", slot);
    }

    [Fact]
    public async Task GetFreeSlotAsync_Lxc_Limit256_ThrowsWhenFull()
    {
        var handler = new FakeHandler(req =>
        {
            var path = req.RequestUri!.AbsolutePath;
            if (path.EndsWith("/config"))
            {
                var configData = new JObject();
                for (var i = 0; i < 256; i++)
                {
                    configData[$"mp{i}"] = $"local-zfs:subvol-200-disk-{i},mp=/mnt/dir{i}";
                }
                return FakeHandler.Json(HttpStatusCode.OK, new JObject { ["data"] = configData }.ToString());
            }
            if (path.EndsWith("/pending"))
            {
                return FakeHandler.Json(HttpStatusCode.OK, """{"data":[]}""");
            }
            return FakeHandler.Json(HttpStatusCode.NotFound, "{}");
        });

        var client = FakeHandler.Client(handler);
        var ex = await Assert.ThrowsAsync<InvalidOperationException>(() =>
            DiskHelper.GetFreeSlotAsync(client, "pve01", VmType.Lxc, 200, bus: "mp"));

        Assert.Contains("No free MP slots available on Lxc 200", ex.Message);
        Assert.Contains("mp0..mp255", ex.Message);
    }

    [Fact]
    public async Task GetFreeSlotAsync_Lxc_ThrowsOnInvalidBus()
    {
        var client = FakeHandler.Client(new FakeHandler(req => FakeHandler.Json(HttpStatusCode.OK, "{}")));
        await Assert.ThrowsAsync<ArgumentException>(() =>
            DiskHelper.GetFreeSlotAsync(client, "pve01", VmType.Lxc, 200, bus: "virtio"));
    }

    [Fact]
    public async Task GetFreeSlotAsync_ThrowsOnInvalidVmType()
    {
        var client = FakeHandler.Client(new FakeHandler(req => FakeHandler.Json(HttpStatusCode.OK, "{}")));
        await Assert.ThrowsAsync<InvalidEnumArgumentException>(() =>
            DiskHelper.GetFreeSlotAsync(client, "pve01", (VmType)999, 100));
    }

    [Fact]
    public async Task GetFreeSlotAsync_ThrowsOnNullOrEmptyBus()
    {
        var client = FakeHandler.Client(new FakeHandler(req => FakeHandler.Json(HttpStatusCode.OK, "{}")));
        await Assert.ThrowsAsync<ArgumentException>(() =>
            DiskHelper.GetFreeSlotAsync(client, "pve01", VmType.Qemu, 100, bus: "   "));
    }

    #endregion

    #region AttachDiskAsync

    [Fact]
    public async Task AttachDiskAsync_Qemu_AutoAssignsSlot_AndPostsPayload()
    {
        var handler = new FakeHandler(req =>
        {
            var path = req.RequestUri!.AbsolutePath;
            if (req.Method == HttpMethod.Get && path.EndsWith("/config"))
            {
                return FakeHandler.Json(HttpStatusCode.OK, """{"data":{"scsi0":"local-zfs:vm-100-disk-0"}}""");
            }
            if (req.Method == HttpMethod.Get && path.EndsWith("/pending"))
            {
                return FakeHandler.Json(HttpStatusCode.OK, """{"data":[]}""");
            }
            if (req.Method == HttpMethod.Post && path.EndsWith("/config"))
            {
                var body = ParseRequestBody(req);
                Assert.Equal("local-zfs:vm-100-disk-1,ssd=1", (string)body["scsi1"]!);
                Assert.Equal("digestQemu", (string)body["digest"]!);
                return FakeHandler.Json(HttpStatusCode.OK, """{"data":"UPID:pve01:task"}""");
            }
            return FakeHandler.Json(HttpStatusCode.BadRequest, "{}");
        });

        var client = FakeHandler.Client(handler);
        var result = await DiskHelper.AttachDiskAsync(client, "pve01", VmType.Qemu, 100,
            volume: "local-zfs:vm-100-disk-1",
            options: "ssd=1",
            digest: "digestQemu");

        Assert.True(result.IsSuccessStatusCode);
    }

    [Fact]
    public async Task AttachDiskAsync_Qemu_ExplicitSlot_Sata()
    {
        var handler = new FakeHandler(req =>
        {
            Assert.Equal(HttpMethod.Post, req.Method);
            Assert.Equal("/api2/json/nodes/pve01/qemu/100/config", req.RequestUri!.AbsolutePath);

            var body = ParseRequestBody(req);
            Assert.Equal("local-zfs:vm-100-disk-5", (string)body["sata2"]!);
            return FakeHandler.Json(HttpStatusCode.OK, """{"data":"UPID:pve01:task"}""");
        });

        var client = FakeHandler.Client(handler);
        var result = await DiskHelper.AttachDiskAsync(client, "pve01", VmType.Qemu, 100,
            volume: "local-zfs:vm-100-disk-5",
            slot: "sata2");

        Assert.True(result.IsSuccessStatusCode);
        Assert.Single(handler.Requests);
    }

    [Fact]
    public async Task AttachDiskAsync_Qemu_ExplicitSlot_Virtio_And_Ide()
    {
        var handler = new FakeHandler(req =>
        {
            var body = ParseRequestBody(req);
            if (body.ContainsKey("virtio3"))
            {
                Assert.Equal("local-zfs:vm-100-disk-v,backup=0", (string)body["virtio3"]!);
                return FakeHandler.Json(HttpStatusCode.OK, """{"data":"UPID:pve01:task1"}""");
            }
            if (body.ContainsKey("ide2"))
            {
                Assert.Equal("local-zfs:vm-100-disk-i", (string)body["ide2"]!);
                return FakeHandler.Json(HttpStatusCode.OK, """{"data":"UPID:pve01:task2"}""");
            }
            return FakeHandler.Json(HttpStatusCode.BadRequest, "{}");
        });

        var client = FakeHandler.Client(handler);

        var rVirtio = await DiskHelper.AttachDiskAsync(client, "pve01", VmType.Qemu, 100,
            volume: "local-zfs:vm-100-disk-v",
            slot: "virtio3",
            options: "backup=0");
        Assert.True(rVirtio.IsSuccessStatusCode);

        var rIde = await DiskHelper.AttachDiskAsync(client, "pve01", VmType.Qemu, 100,
            volume: "local-zfs:vm-100-disk-i",
            slot: "ide2");
        Assert.True(rIde.IsSuccessStatusCode);
    }

    [Fact]
    public async Task AttachDiskAsync_Qemu_ThrowsOnInvalidSlot()
    {
        var client = FakeHandler.Client(new FakeHandler(req => FakeHandler.Json(HttpStatusCode.OK, "{}")));

        // Invalid format
        await Assert.ThrowsAsync<ArgumentException>(() =>
            DiskHelper.AttachDiskAsync(client, "pve01", VmType.Qemu, 100, "local-zfs:disk", slot: "nvme0"));

        // Index exceeds limit (scsi 31 limit -> 0..30 allowed, scsi31 invalid)
        await Assert.ThrowsAsync<ArgumentException>(() =>
            DiskHelper.AttachDiskAsync(client, "pve01", VmType.Qemu, 100, "local-zfs:disk", slot: "scsi31"));

        // Sata 6 limit -> sata6 invalid
        await Assert.ThrowsAsync<ArgumentException>(() =>
            DiskHelper.AttachDiskAsync(client, "pve01", VmType.Qemu, 100, "local-zfs:disk", slot: "sata6"));
    }

    [Fact]
    public async Task AttachDiskAsync_Lxc_AutoAssignsSlot_AndPutsPayload()
    {
        var handler = new FakeHandler(req =>
        {
            var path = req.RequestUri!.AbsolutePath;
            if (req.Method == HttpMethod.Get && path.EndsWith("/config"))
            {
                return FakeHandler.Json(HttpStatusCode.OK, """{"data":{"mp0":"local-zfs:subvol-200-disk-0,mp=/mnt/a"}}""");
            }
            if (req.Method == HttpMethod.Get && path.EndsWith("/pending"))
            {
                return FakeHandler.Json(HttpStatusCode.OK, """{"data":[]}""");
            }
            if (req.Method == HttpMethod.Put && path.EndsWith("/config"))
            {
                var body = ParseRequestBody(req);
                Assert.Equal("local-zfs:subvol-200-disk-1,mp=/mnt/data,backup=1", (string)body["mp1"]!);
                Assert.Equal("digestLxc", (string)body["digest"]!);
                return FakeHandler.Json(HttpStatusCode.OK, """{"data":"UPID:pve01:task"}""");
            }
            return FakeHandler.Json(HttpStatusCode.BadRequest, "{}");
        });

        var client = FakeHandler.Client(handler);
        var result = await DiskHelper.AttachDiskAsync(client, "pve01", VmType.Lxc, 200,
            volume: "local-zfs:subvol-200-disk-1",
            mountPath: "/mnt/data",
            options: "backup=1",
            digest: "digestLxc");

        Assert.True(result.IsSuccessStatusCode);
    }

    [Fact]
    public async Task AttachDiskAsync_Lxc_ExplicitSlot_AndPutsPayload()
    {
        var handler = new FakeHandler(req =>
        {
            Assert.Equal(HttpMethod.Put, req.Method);
            Assert.Equal("/api2/json/nodes/pve01/lxc/200/config", req.RequestUri!.AbsolutePath);

            var body = ParseRequestBody(req);
            Assert.Equal("local-zfs:subvol-200-disk-5,mp=/var/log", (string)body["mp5"]!);
            return FakeHandler.Json(HttpStatusCode.OK, """{"data":"UPID:pve01:task"}""");
        });

        var client = FakeHandler.Client(handler);
        var result = await DiskHelper.AttachDiskAsync(client, "pve01", VmType.Lxc, 200,
            volume: "local-zfs:subvol-200-disk-5",
            slot: "mp5",
            mountPath: "/var/log");

        Assert.True(result.IsSuccessStatusCode);
        Assert.Single(handler.Requests);
    }

    [Fact]
    public async Task AttachDiskAsync_Lxc_ThrowsWhenMountPathMissing()
    {
        var client = FakeHandler.Client(new FakeHandler(req => FakeHandler.Json(HttpStatusCode.OK, "{}")));
        await Assert.ThrowsAsync<ArgumentException>(() =>
            DiskHelper.AttachDiskAsync(client, "pve01", VmType.Lxc, 200, "local-zfs:disk", slot: "mp0"));
    }

    [Fact]
    public async Task AttachDiskAsync_Lxc_ThrowsOnInvalidSlot()
    {
        var client = FakeHandler.Client(new FakeHandler(req => FakeHandler.Json(HttpStatusCode.OK, "{}")));

        // Invalid format
        await Assert.ThrowsAsync<ArgumentException>(() =>
            DiskHelper.AttachDiskAsync(client, "pve01", VmType.Lxc, 200, "local-zfs:disk", slot: "scsi0", mountPath: "/mnt"));

        // Exceeds limit (256 limit -> 0..255 allowed, mp256 invalid)
        await Assert.ThrowsAsync<ArgumentException>(() =>
            DiskHelper.AttachDiskAsync(client, "pve01", VmType.Lxc, 200, "local-zfs:disk", slot: "mp256", mountPath: "/mnt"));
    }

    [Fact]
    public async Task AttachDiskAsync_ThrowsOnNullOrEmptyVolume()
    {
        var client = FakeHandler.Client(new FakeHandler(req => FakeHandler.Json(HttpStatusCode.OK, "{}")));
        await Assert.ThrowsAsync<ArgumentException>(() =>
            DiskHelper.AttachDiskAsync(client, "pve01", VmType.Qemu, 100, "   "));
    }

    [Fact]
    public async Task AttachDiskAsync_ThrowsOnInvalidVmType()
    {
        var client = FakeHandler.Client(new FakeHandler(req => FakeHandler.Json(HttpStatusCode.OK, "{}")));
        await Assert.ThrowsAsync<InvalidEnumArgumentException>(() =>
            DiskHelper.AttachDiskAsync(client, "pve01", (VmType)999, 100, "local-zfs:disk"));
    }

    #endregion

    #region DetachDiskAsync

    [Fact]
    public async Task DetachDiskAsync_Qemu_SendsDeleteParameter()
    {
        var handler = new FakeHandler(req =>
        {
            Assert.Equal(HttpMethod.Post, req.Method);
            Assert.Equal("/api2/json/nodes/pve01/qemu/100/config", req.RequestUri!.AbsolutePath);

            var body = ParseRequestBody(req);
            Assert.Equal("scsi1", (string)body["delete"]!);
            Assert.Equal("digest999", (string)body["digest"]!);
            return FakeHandler.Json(HttpStatusCode.OK, """{"data":"UPID:pve01:task"}""");
        });

        var client = FakeHandler.Client(handler);
        var result = await DiskHelper.DetachDiskAsync(client, "pve01", VmType.Qemu, 100, "scsi1", digest: "digest999");

        Assert.True(result.IsSuccessStatusCode);
        Assert.Single(handler.Requests);
    }

    [Fact]
    public async Task DetachDiskAsync_Lxc_SendsDeleteParameter()
    {
        var handler = new FakeHandler(req =>
        {
            Assert.Equal(HttpMethod.Put, req.Method);
            Assert.Equal("/api2/json/nodes/pve01/lxc/200/config", req.RequestUri!.AbsolutePath);

            var body = ParseRequestBody(req);
            Assert.Equal("mp2", (string)body["delete"]!);
            Assert.Equal("digestLxc", (string)body["digest"]!);
            return FakeHandler.Json(HttpStatusCode.OK, """{"data":"UPID:pve01:task"}""");
        });

        var client = FakeHandler.Client(handler);
        var result = await DiskHelper.DetachDiskAsync(client, "pve01", VmType.Lxc, 200, "mp2", digest: "digestLxc");

        Assert.True(result.IsSuccessStatusCode);
        Assert.Single(handler.Requests);
    }

    [Fact]
    public async Task DetachDiskAsync_ThrowsOnNullOrEmptySlot()
    {
        var client = FakeHandler.Client(new FakeHandler(req => FakeHandler.Json(HttpStatusCode.OK, "{}")));
        await Assert.ThrowsAsync<ArgumentException>(() =>
            DiskHelper.DetachDiskAsync(client, "pve01", VmType.Qemu, 100, "  "));
    }

    [Fact]
    public async Task DetachDiskAsync_ThrowsOnInvalidVmType()
    {
        var client = FakeHandler.Client(new FakeHandler(req => FakeHandler.Json(HttpStatusCode.OK, "{}")));
        await Assert.ThrowsAsync<InvalidEnumArgumentException>(() =>
            DiskHelper.DetachDiskAsync(client, "pve01", (VmType)999, 100, "scsi0"));
    }

    #endregion

    #region RemoveUnusedDiskAsync

    [Fact]
    public async Task RemoveUnusedDiskAsync_Qemu_MatchesExactVolume_AndDoesNotMatchSubstring()
    {
        var handler = new FakeHandler(req =>
        {
            var path = req.RequestUri!.AbsolutePath;
            if (req.Method == HttpMethod.Get && path.EndsWith("/config"))
            {
                // unused0 has vm-100-disk-10 (should NOT match vm-100-disk-1)
                // unused1 has vm-100-disk-1 (exact match!)
                return FakeHandler.Json(HttpStatusCode.OK, """
                    {"data":{
                        "unused0":"local-zfs:vm-100-disk-10",
                        "unused1":"local-zfs:vm-100-disk-1"
                    }}
                    """);
            }
            if (req.Method == HttpMethod.Post && path.EndsWith("/config"))
            {
                var body = ParseRequestBody(req);
                Assert.Equal("unused1", (string)body["delete"]!);
                Assert.Equal("digestPurge", (string)body["digest"]!);
                return FakeHandler.Json(HttpStatusCode.OK, """{"data":"UPID:pve01:task"}""");
            }
            return FakeHandler.Json(HttpStatusCode.BadRequest, "{}");
        });

        var client = FakeHandler.Client(handler);
        var result = await DiskHelper.RemoveUnusedDiskAsync(client, "pve01", VmType.Qemu, 100, "vm-100-disk-1", digest: "digestPurge");

        Assert.True(result.IsSuccessStatusCode);
    }

    [Fact]
    public async Task RemoveUnusedDiskAsync_Lxc_MatchesExactFullVolumeId()
    {
        var handler = new FakeHandler(req =>
        {
            var path = req.RequestUri!.AbsolutePath;
            if (req.Method == HttpMethod.Get && path.EndsWith("/config"))
            {
                return FakeHandler.Json(HttpStatusCode.OK, """
                    {"data":{
                        "unused0":"local-zfs:subvol-200-disk-0"
                    }}
                    """);
            }
            if (req.Method == HttpMethod.Put && path.EndsWith("/config"))
            {
                var body = ParseRequestBody(req);
                Assert.Equal("unused0", (string)body["delete"]!);
                return FakeHandler.Json(HttpStatusCode.OK, """{"data":"UPID:pve01:task"}""");
            }
            return FakeHandler.Json(HttpStatusCode.BadRequest, "{}");
        });

        var client = FakeHandler.Client(handler);
        var result = await DiskHelper.RemoveUnusedDiskAsync(client, "pve01", VmType.Lxc, 200, "local-zfs:subvol-200-disk-0");

        Assert.True(result.IsSuccessStatusCode);
    }

    [Fact]
    public async Task RemoveUnusedDiskAsync_ThrowsWhenVolumeNotFound()
    {
        var handler = new FakeHandler(req =>
        {
            return FakeHandler.Json(HttpStatusCode.OK, """
                {"data":{
                    "unused0":"local-zfs:vm-100-disk-5"
                }}
                """);
        });

        var client = FakeHandler.Client(handler);
        var ex = await Assert.ThrowsAsync<ArgumentException>(() =>
            DiskHelper.RemoveUnusedDiskAsync(client, "pve01", VmType.Qemu, 100, "vm-100-disk-1"));

        Assert.Contains("Unused disk 'vm-100-disk-1' not found on Qemu 100", ex.Message);
    }

    [Fact]
    public async Task RemoveUnusedDiskAsync_ThrowsOnNullOrEmptyVolume()
    {
        var client = FakeHandler.Client(new FakeHandler(req => FakeHandler.Json(HttpStatusCode.OK, "{}")));
        await Assert.ThrowsAsync<ArgumentException>(() =>
            DiskHelper.RemoveUnusedDiskAsync(client, "pve01", VmType.Qemu, 100, "  "));
    }

    [Fact]
    public async Task RemoveUnusedDiskAsync_ThrowsOnInvalidVmType()
    {
        var client = FakeHandler.Client(new FakeHandler(req => FakeHandler.Json(HttpStatusCode.OK, "{}")));
        await Assert.ThrowsAsync<InvalidEnumArgumentException>(() =>
            DiskHelper.RemoveUnusedDiskAsync(client, "pve01", (VmType)999, 100, "local-zfs:disk"));
    }

    [Fact]
    public async Task RemoveUnusedDiskAsync_MatchesDirectoryStorageVolumePath()
    {
        var handler = new FakeHandler(req =>
        {
            var path = req.RequestUri!.AbsolutePath;
            if (req.Method == HttpMethod.Get && path.EndsWith("/config"))
            {
                return FakeHandler.Json(HttpStatusCode.OK, """
                    {"data":{
                        "unused0":"local:100/vm-100-disk-1.qcow2"
                    }}
                    """);
            }
            if (req.Method == HttpMethod.Post && path.EndsWith("/config"))
            {
                var body = ParseRequestBody(req);
                Assert.Equal("unused0", (string)body["delete"]!);
                return FakeHandler.Json(HttpStatusCode.OK, """{"data":"UPID:pve01:task"}""");
            }
            return FakeHandler.Json(HttpStatusCode.BadRequest, "{}");
        });

        var client = FakeHandler.Client(handler);
        var result = await DiskHelper.RemoveUnusedDiskAsync(client, "pve01", VmType.Qemu, 100, "vm-100-disk-1.qcow2");

        Assert.True(result.IsSuccessStatusCode);
    }

    #endregion

    #region Additional Edge Cases

    [Fact]
    public async Task GetFreeSlotAsync_Qemu_SlotHole_ReturnsLowestFreeSlot()
    {
        var handler = new FakeHandler(req =>
        {
            var path = req.RequestUri!.AbsolutePath;
            if (path.EndsWith("/config"))
            {
                return FakeHandler.Json(HttpStatusCode.OK, """
                    {"data":{
                        "scsi0":"local-zfs:vm-100-disk-0",
                        "scsi2":"local-zfs:vm-100-disk-2"
                    }}
                    """);
            }
            if (path.EndsWith("/pending"))
            {
                return FakeHandler.Json(HttpStatusCode.OK, """{"data":[]}""");
            }
            return FakeHandler.Json(HttpStatusCode.NotFound, "{}");
        });

        var client = FakeHandler.Client(handler);
        // scsi0 and scsi2 exist, so scsi1 is the lowest free hole
        var slot = await DiskHelper.GetFreeSlotAsync(client, "pve01", VmType.Qemu, 100, bus: "scsi");

        Assert.Equal("scsi1", slot);
    }

    [Fact]
    public async Task AttachDiskAsync_NormalizesWhitespaceAndLeadingCommaOptions_Qemu()
    {
        var handler = new FakeHandler(req =>
        {
            var body = ParseRequestBody(req);
            Assert.Equal("local-zfs:vm-100-disk-9,ssd=1,backup=0", (string)body["scsi3"]!);
            return FakeHandler.Json(HttpStatusCode.OK, """{"data":"UPID:pve01:task"}""");
        });

        var client = FakeHandler.Client(handler);
        var result = await DiskHelper.AttachDiskAsync(client, "pve01", VmType.Qemu, 100,
            volume: "  local-zfs:vm-100-disk-9  ",
            slot: "  scsi3  ",
            options: " ,ssd=1,backup=0 ");

        Assert.True(result.IsSuccessStatusCode);
    }

    [Fact]
    public async Task DetachDiskAsync_TrimsSlotWhitespace()
    {
        var handler = new FakeHandler(req =>
        {
            var body = ParseRequestBody(req);
            Assert.Equal("scsi3", (string)body["delete"]!);
            return FakeHandler.Json(HttpStatusCode.OK, """{"data":"UPID:pve01:task"}""");
        });

        var client = FakeHandler.Client(handler);
        var result = await DiskHelper.DetachDiskAsync(client, "pve01", VmType.Qemu, 100, "  scsi3  ");

        Assert.True(result.IsSuccessStatusCode);
    }

    #endregion
}
