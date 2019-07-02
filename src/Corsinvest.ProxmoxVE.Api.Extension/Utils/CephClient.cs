using System;
using System.Diagnostics;
using System.IO;
using Corsinvest.ProxmoxVE.Api.Extension.Storage;

namespace Corsinvest.ProxmoxVE.Api.Extension.Utils
{
    public class CephClient
    {
        public TextWriter Output { get; }
        public bool DryRun { get; }
        public bool Debug { get; }
        public string CephConfigDirectory { get; }
        public string MonitorHosts { get; }
        public string Pool { get; }
        public string StoreId { get; }
        public string Username { get; }

        public CephClient(TextWriter output,
                          bool dryRun,
                          bool debug,
                          string cephConfigDirectory,
                          string monHosts,
                          string pool,
                          string storeId,
                          string username)
        {
            Output = output;
            DryRun = dryRun;
            Debug = debug;
            CephConfigDirectory = cephConfigDirectory;
            MonitorHosts = monHosts;
            Pool = pool;
            StoreId = storeId;
            Username = username;
            if (string.IsNullOrWhiteSpace(Username)) { Username = "admin"; }
        }

        public static CephClient From(TextWriter output, bool dryRun, bool debug, string cephConfigDirectory, Ceph cephInfo)
            => new CephClient(output,
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

            if (Debug) { Output.WriteLine($"Run command: {cmd}"); }
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

        public bool ExportDiff(string image, string fromSnap, string toSnap, string destFile)
        {
            var cmd = $"{BaseArgs()} export-diff";
            if (!string.IsNullOrWhiteSpace(fromSnap)) { cmd += " --from-snap {image}@{fromSnap}"; }
            cmd += $" {image}@{toSnap} '{destFile}'";
            return Shell(cmd, false).ExitCode == 0;
        }

        public bool ImportDiff(string fileNameImport, string pool, string image)
            => Shell($"{BaseArgs()} import-diff '{fileNameImport}' {image}", false).ExitCode == 0;

        public bool SnapshotCreate(string pool, string image, string snapName)
            => Shell($"{BaseArgs()}  snap create {image}@{snapName}", false).ExitCode == 0;

        public bool SnapshotDelete(string pool, string image, string snapName)
            => Shell($"{BaseArgs()} snap rm {image}@{snapName}", false).ExitCode == 0;

        public bool SnapshotsPurge(string pool, string image)
            => Shell($"{BaseArgs()} purge {image}", false).ExitCode == 0;

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