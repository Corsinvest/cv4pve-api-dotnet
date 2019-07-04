using System;
using System.Diagnostics;
using System.IO;
using Corsinvest.ProxmoxVE.Api.Extension.Storage;

namespace Corsinvest.ProxmoxVE.Api.Extension.Utils
{
    /// <summary>
    /// Ceph client.
    /// </summary>
    public class CephClient
    {
        /// <summary>
        /// Standard Outout
        /// </summary>
        /// <value></value>
        public TextWriter StdOut { get; }

        /// <summary>
        /// Dry run
        /// </summary>
        /// <value></value>
        public bool DryRun { get; }

        /// <summary>
        /// DEbug
        /// </summary>
        /// <value></value>
        public bool Debug { get; }

        /// <summary>
        /// Ceph Config Directory
        /// </summary>
        /// <value></value>
        public string CephConfigDirectory { get; }

        /// <summary>
        /// Monitors hosts
        /// </summary>
        /// <value></value>
        public string MonitorHosts { get; }

        /// <summary>
        /// Pool
        /// </summary>
        /// <value></value>
        public string Pool { get; }

        /// <summary>
        /// StoreId
        /// </summary>
        /// <value></value>
        public string StoreId { get; }

        /// <summary>
        /// Username
        /// </summary>
        /// <value></value>
        public string Username { get; }

        /// <summary>
        /// ICostructor
        /// </summary>
        /// <param name="stdOut"></param>
        /// <param name="dryRun"></param>
        /// <param name="debug"></param>
        /// <param name="cephConfigDirectory"></param>
        /// <param name="monHosts"></param>
        /// <param name="pool"></param>
        /// <param name="storeId"></param>
        /// <param name="username"></param>
        public CephClient(TextWriter stdOut,
                          bool dryRun,
                          bool debug,
                          string cephConfigDirectory,
                          string monHosts,
                          string pool,
                          string storeId,
                          string username)
        {
            StdOut = stdOut;
            DryRun = dryRun;
            Debug = debug;
            CephConfigDirectory = cephConfigDirectory;
            MonitorHosts = monHosts;
            Pool = pool;
            StoreId = storeId;
            Username = username;
            if (string.IsNullOrWhiteSpace(Username)) { Username = "admin"; }
        }

        /// <summary>
        /// Get Client from info
        /// </summary>
        /// <param name="stdOut"></param>
        /// <param name="dryRun"></param>
        /// <param name="debug"></param>
        /// <param name="cephConfigDirectory"></param>
        /// <param name="cephInfo"></param>
        /// <returns></returns>
        public static CephClient From(TextWriter stdOut, bool dryRun, bool debug, string cephConfigDirectory, Ceph cephInfo)
            => new CephClient(stdOut,
                              dryRun,
                              debug,
                              cephConfigDirectory,
                              cephInfo.MonitorHosts,
                              cephInfo.Pool,
                              cephInfo.Id,
                              cephInfo.Username);

        private string BaseArgs()
        {
            var args = $"-p {Pool} -m {MonitorHosts}";

            var fullNameKeyring = Path.Combine(CephConfigDirectory, $"{StoreId}.keyring");
            if (File.Exists(fullNameKeyring))
            {
                args += $" -n client.{Username} --keyring {fullNameKeyring} --auth_supported cephx";
            }
            else
            {
                args += " --auth_supported none";
            }

            var fullNameCephConfig = Path.Combine(CephConfigDirectory, $"{StoreId}.conf");
            if (File.Exists(fullNameCephConfig)) { args += $" -c {fullNameCephConfig}"; }

            return args;
        }

        private (string StandardOutput, int ExitCode) Shell(string cmd, bool redirectStandardOutput)
        {
            var startInfo = new ProcessStartInfo()
            {
                UseShellExecute = false,
                CreateNoWindow = true,
                RedirectStandardOutput = redirectStandardOutput,
            };

            var escapedArgs = cmd.Replace("\"", "\\\"");
            startInfo.FileName = "/bin/bash";
            startInfo.Arguments = $"-c \"{escapedArgs}\"";

            var process = new Process()
            {
                StartInfo = startInfo
            };

            if (Debug) { StdOut.WriteLine($"Run command: {cmd}"); }
            if (DryRun)
            {
                return ("", 0);
            }
            else
            {
                process.Start();
                var standardOutput = redirectStandardOutput ? process.StandardOutput.ReadToEnd() : "";
                process.WaitForExit();

                return (standardOutput, process.ExitCode);
            }
        }

        /// <summary>
        /// Get pools
        /// </summary>
        /// <returns></returns>
        public string[] GetPools()
        {
            var ret = Shell($"rados {BaseArgs()} lspools", true);
            if (ret.ExitCode == 0)
            {
                return ret.StandardOutput.Split(new[] { Environment.NewLine }, StringSplitOptions.None);
            }
            else
            {
                throw new Exception("Problem check pool in Ceph!");
            }
        }

        // public static bool Export(string config, string pool, string image, string fromSnap, string destFile)
        // {
        //     var cmd = BaseCmd(config, pool) +
        //               " export --rbd-concurrent-management-ops 20 --from-snap {image}@{fromSnap} '{destFile}'";
        //     return Shell(cmd, false).ExitCode == 0;
        // }

        // public static bool Import(string config, string fileNameImport, string pool, string image)
        // {
        //     var cmd = BaseCmd(config, pool) + $" import --image-format 2 '{fileNameImport}' {image}";
        //     return Shell(cmd, false).ExitCode == 0;
        // }

        /// <summary>
        /// Export diff
        /// </summary>
        /// <param name="image"></param>
        /// <param name="fromSnap"></param>
        /// <param name="toSnap"></param>
        /// <param name="destFile"></param>
        /// <returns></returns>
        public bool ExportDiff(string image, string fromSnap, string toSnap, string destFile)
        {
            var cmd = $"{BaseArgs()} export-diff";
            if (!string.IsNullOrWhiteSpace(fromSnap)) { cmd += " --from-snap {image}@{fromSnap}"; }
            cmd += $" {image}@{toSnap} '{destFile}'";
            return Shell(cmd, false).ExitCode == 0;
        }

        /// <summary>
        /// Import diff
        /// </summary>
        /// <param name="fileNameImport"></param>
        /// <param name="pool"></param>
        /// <param name="image"></param>
        /// <returns></returns>
        public bool ImportDiff(string fileNameImport, string pool, string image)
            => Shell($"{BaseArgs()} import-diff '{fileNameImport}' {image}", false).ExitCode == 0;

        /// <summary>
        /// Create snapshot
        /// </summary>
        /// <param name="pool"></param>
        /// <param name="image"></param>
        /// <param name="snapName"></param>
        /// <returns></returns>
        public bool SnapshotCreate(string pool, string image, string snapName)
            => Shell($"{BaseArgs()}  snap create {image}@{snapName}", false).ExitCode == 0;

        /// <summary>
        /// Delete snapshot
        /// </summary>
        /// <param name="pool"></param>
        /// <param name="image"></param>
        /// <param name="snapName"></param>
        /// <returns></returns>
        public bool SnapshotDelete(string pool, string image, string snapName)
            => Shell($"{BaseArgs()} snap rm {image}@{snapName}", false).ExitCode == 0;

        /// <summary>
        /// Purge snaposhots
        /// </summary>
        /// <param name="pool"></param>
        /// <param name="image"></param>
        /// <returns></returns>
        public bool SnapshotsPurge(string pool, string image)
            => Shell($"{BaseArgs()} purge {image}", false).ExitCode == 0;

        /// <summary>
        /// Get images
        /// </summary>
        /// <param name="pool"></param>
        /// <returns></returns>
        public string[] GetImages(string pool)
        {
            var ret = Shell($"{BaseArgs()} list", true);
            if (ret.ExitCode == 0)
            {
                return ret.StandardOutput.Split(new[] { Environment.NewLine }, StringSplitOptions.None);
            }
            else
            {
                throw new Exception("Problem check pool in Ceph!");
            }
        }
    }
}