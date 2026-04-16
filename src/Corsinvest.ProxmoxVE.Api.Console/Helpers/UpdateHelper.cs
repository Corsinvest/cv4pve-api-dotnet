/*
 * SPDX-FileCopyrightText: Copyright Corsinvest Srl
 * SPDX-License-Identifier: MIT
 */

#nullable enable

using Newtonsoft.Json;

namespace Corsinvest.ProxmoxVE.Api.Console.Helpers;

/// <summary>
/// Helper for checking new GitHub releases, with 24h file cache shared across cv4pve tools.
/// </summary>
internal static class UpdateHelper
{
    private static readonly TimeSpan CacheTtl = TimeSpan.FromHours(24);

    private static string CacheFilePath
        => Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile),
                        ".cv4pve", "update-check.json");

    /// <summary>
    /// Starts a background task that checks for a newer GitHub release (if cache is stale).
    /// </summary>
    public static (Task Task, CancellationTokenSource Cts) StartCheck(string appName)
    {
        var cts = new CancellationTokenSource();
        var task = Task.Run(() => RefreshCacheIfStaleAsync(appName, cts.Token), cts.Token);
        return (task, cts);
    }

    /// <summary>
    /// Prints an update notice using the best info available (fresh fetch or cached).
    /// Cancels the background task if still running.
    /// </summary>
    public static void PrintIfNewVersion(string appName, Task checkTask, CancellationTokenSource cts)
    {
        try
        {
            if (!checkTask.IsCompleted) { cts.Cancel(); }

            var latestVersion = ReadCache().GetValueOrDefault(appName)?.LatestVersion;
            if (string.IsNullOrEmpty(latestVersion)) { return; }

            var currentVersion = ConsoleHelper.GetCurrentVersionApp();
            if (latestVersion != currentVersion && !System.Console.IsOutputRedirected)
            {
                System.Console.Out.WriteLine();
                System.Console.Out.WriteLine($"*** New version available: {latestVersion} (current: {currentVersion}) ***");
            }
        }
        catch { }
        finally { cts.Dispose(); }
    }

    private static async Task RefreshCacheIfStaleAsync(string appName, CancellationToken ct)
    {
        var cache = ReadCache();
        if (cache.TryGetValue(appName, out var entry)
            && DateTime.UtcNow - entry.LastCheck < CacheTtl) { return; }

        string? newVersion = null;
        try
        {
            using var client = new HttpClient();
            client.DefaultRequestHeaders.UserAgent.ParseAdd("cv4pve-app");
            var json = await client.GetStringAsync($"https://api.github.com/repos/Corsinvest/{appName}/releases", ct);
            var releases = JsonConvert.DeserializeObject<List<GitHubRelease>>(json);
            var latest = releases?.FirstOrDefault(r => !r.Prerelease);
            if (latest != null) { newVersion = latest.TagName.TrimStart('v'); }
        }
        catch { }

        cache[appName] = new CacheEntry
        {
            LastCheck = DateTime.UtcNow,
            LatestVersion = newVersion ?? entry?.LatestVersion ?? string.Empty
        };
        WriteCache(cache);
    }

    private static Dictionary<string, CacheEntry> ReadCache()
    {
        try
        {
            if (!File.Exists(CacheFilePath)) { return []; }
            var json = File.ReadAllText(CacheFilePath);
            return JsonConvert.DeserializeObject<Dictionary<string, CacheEntry>>(json) ?? [];
        }
        catch { return []; }
    }

    private static void WriteCache(Dictionary<string, CacheEntry> cache)
    {
        try
        {
            var dir = Path.GetDirectoryName(CacheFilePath)!;
            if (!Directory.Exists(dir)) { Directory.CreateDirectory(dir); }
            File.WriteAllText(CacheFilePath, JsonConvert.SerializeObject(cache, Formatting.Indented));
        }
        catch { }
    }

    private sealed class CacheEntry
    {
        public DateTime LastCheck { get; set; }
        public string LatestVersion { get; set; } = string.Empty;
    }

    private sealed class GitHubRelease
    {
        [JsonProperty("tag_name")]
        public string TagName { get; set; } = string.Empty;

        [JsonProperty("prerelease")]
        public bool Prerelease { get; set; }
    }
}
