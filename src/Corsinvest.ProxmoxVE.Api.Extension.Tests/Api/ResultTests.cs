/*
 * SPDX-FileCopyrightText: Copyright Corsinvest Srl
 * SPDX-License-Identifier: MIT
 */

using System.Net;
using Newtonsoft.Json;
using Xunit;

namespace Corsinvest.ProxmoxVE.Api.Extension.Tests.Api;

public class ResultTests
{
    private sealed class Guest
    {
        [JsonProperty("vmid")] public long VmId { get; set; }
        [JsonProperty("name")] public string Name { get; set; } = "";
        [JsonProperty("template")] public bool IsTemplate { get; set; }
        [JsonProperty("onboot")] public bool OnBoot { get; set; }
    }

    private static async Task<Result> AnswerAsync(HttpStatusCode status, string body, string? reason = null)
    {
        var client = FakeHandler.Client(new FakeHandler(_ =>
        {
            var response = FakeHandler.Text(status, body);
            if (reason != null) { response.ReasonPhrase = reason; }
            return response;
        }));
        return await client.GetAsync("/test");
    }

    private static Task<Result> OkAsync(string json) => AnswerAsync(HttpStatusCode.OK, json);

    private static async Task<Result> NoAnswerAsync()
        => await FakeHandler.Client(new FakeHandler(_ => throw new HttpRequestException("Connection refused"))).GetAsync("/test");

    [Fact]
    public async Task Data_of_an_object_is_read_by_member_name()
    {
        var data = (await OkAsync("""{"data":{"name":"web01","cores":4}}""")).ToData();

        Assert.Equal("web01", (string)data.name);
        Assert.Equal(4L, (long)data.cores);
    }

    [Fact]
    public async Task Data_of_a_single_value_is_converted()
    {
        Assert.Equal("UPID:pve01:1:2:3:qmstart:100:root@pam:", (await OkAsync("""{"data":"UPID:pve01:1:2:3:qmstart:100:root@pam:"}""")).ToData<string>());
        Assert.Equal(105, (await OkAsync("""{"data":"105"}""")).ToData<int>());
        Assert.Equal(105, (await OkAsync("""{"data":105}""")).ToData<int>());
    }

    [Fact]
    public async Task List_is_enumerated()
    {
        var result = await OkAsync("""{"data":[{"vmid":100,"status":"running"},{"vmid":101,"status":"stopped"}]}""");

        Assert.Equal([100L], result.ToEnumerable().Where(a => a.status == "running").Select(a => (long)a.vmid));
    }

    [Fact]
    public async Task Enumeration_is_empty_when_there_is_no_list()
    {
        Assert.Empty((await OkAsync("""{"data":null}""")).ToEnumerable());
        Assert.Empty((await OkAsync("""{"data":[]}""")).ToEnumerable());
        Assert.Empty((await OkAsync("")).ToEnumerable());
        Assert.Empty((await AnswerAsync(HttpStatusCode.Forbidden, """{"data":null}""")).ToEnumerable());
        Assert.Empty((await NoAnswerAsync()).ToEnumerable());
        Assert.Empty(((Result)null!).ToEnumerable());
    }

    [Fact]
    public async Task Model_is_read_from_the_data()
    {
        var guests = (await OkAsync("""{"data":[{"vmid":100,"name":"web01","template":0,"onboot":1},{"vmid":9000,"name":"tpl","template":1}]}"""))
                        .ToModel<List<Guest>>();

        Assert.Equal(2, guests.Count);
        Assert.Equal(100, guests[0].VmId);
        Assert.Equal("web01", guests[0].Name);
        Assert.False(guests[0].IsTemplate);
        Assert.True(guests[0].OnBoot);
        Assert.True(guests[1].IsTemplate);
        Assert.False(guests[1].OnBoot);
    }

    [Theory]
    [InlineData("1", true)]
    [InlineData("0", false)]
    [InlineData("\"1\"", true)]
    [InlineData("\"0\"", false)]
    [InlineData("\"\"", false)]
    [InlineData("true", true)]
    [InlineData("false", false)]
    [InlineData("\"AA:BB:CC\"", true)]
    public async Task Boolean_of_a_model_accepts_the_forms_proxmox_ve_uses(string json, bool expected)
    {
        var guest = (await OkAsync("{\"data\":{\"template\":" + json + "}}")).ToModel<Guest>();

        Assert.Equal(expected, guest.IsTemplate);
    }

    [Fact]
    public async Task Model_of_a_failed_call_throws_with_the_reason()
    {
        var result = await AnswerAsync(HttpStatusCode.Forbidden, """{"data":null}""", "Permission check failed (/vms/100, VM.Audit)");

        var ex = Assert.Throws<PveResultException>(() => result.ToModel<Guest>());

        Assert.Equal("Permission check failed (/vms/100, VM.Audit)", ex.Message);
        Assert.Same(result, ex.Result);
    }

    [Fact]
    public async Task Model_of_a_call_with_refused_parameters_throws_with_the_parameters()
    {
        var result = await AnswerAsync(HttpStatusCode.BadRequest, """{"data":null,"errors":{"vmid":"invalid format"}}""", "Parameter verification failed.");

        var ex = Assert.Throws<PveResultException>(() => result.ToModel<Guest>());

        Assert.Contains("Parameter verification failed.", ex.Message);
    }

    [Fact]
    public async Task Model_of_a_call_that_got_no_answer_throws_with_the_reason()
    {
        var ex = Assert.Throws<PveResultException>(() => NoAnswerAsync().Result.ToModel<Guest>());

        Assert.Equal("Connection refused", ex.Message);
    }

    [Fact]
    public async Task Log_lines_are_sorted_by_number()
    {
        var result = await OkAsync("""{"data":[{"n":2,"t":"second"},{"n":1,"t":"first"},{"n":3,"t":"third"}]}""");

        Assert.Equal(["first", "second", "third"], result.ToLogs());
    }

    [Fact]
    public async Task Log_is_empty_when_there_is_none()
    {
        Assert.Empty((await OkAsync("""{"data":null}""")).ToLogs());
        Assert.Empty((await OkAsync("""{"data":[]}""")).ToLogs());
        Assert.Empty((await AnswerAsync(HttpStatusCode.Forbidden, """{"data":null}""")).ToLogs());
        Assert.Empty((await NoAnswerAsync()).ToLogs());
        Assert.Empty(((Result)null!).ToLogs());
    }

    [Fact]
    public async Task In_error_is_true_only_for_refused_parameters()
    {
        Assert.True((await AnswerAsync(HttpStatusCode.BadRequest, """{"data":null,"errors":{"vmid":"invalid format"}}""")).InError());
        Assert.False((await AnswerAsync(HttpStatusCode.Forbidden, """{"data":null}""")).InError());
        Assert.False((await OkAsync("""{"data":{}}""")).InError());
        Assert.False((await NoAnswerAsync()).InError());
        Assert.False(((Result)null!).InError());
    }

    [Fact]
    public async Task Response_as_a_dictionary_tells_if_a_member_is_there()
    {
        var result = await OkAsync("""{"data":{"net0":"virtio,bridge=vmbr0"}}""");
        var data = (IDictionary<string, object>)result.ToData();

        Assert.True(result.ResponseToDictionary.ContainsKey("data"));
        Assert.True(data.TryGetValue("net0", out var net0));
        Assert.Equal("virtio,bridge=vmbr0", net0);
        Assert.False(data.ContainsKey("net1"));
    }
}
