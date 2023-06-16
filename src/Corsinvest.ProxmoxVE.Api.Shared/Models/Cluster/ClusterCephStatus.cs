/*
 * SPDX-FileCopyrightText: Copyright Corsinvest Srl
 * SPDX-License-Identifier: GPL-3.0-only
 */

using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace Corsinvest.ProxmoxVE.Api.Shared.Models.Cluster
{
    public class ClusterCephStatus
    {
        [JsonProperty("fsmap")]
        public Fsmap_ Fsmap { get; set; }

        [JsonProperty("servicemap")]
        public Servicemap_ Servicemap { get; set; }

        [JsonProperty("progress_events")]
        public ProgressEvents_ ProgressEvents { get; set; }

        [JsonProperty("quorum")]
        public List<int> Quorum { get; set; }

        [JsonProperty("osdmap")]
        public Osdmap_ Osdmap { get; set; }

        [JsonProperty("quorum_age")]
        public int QuorumAge { get; set; }

        [JsonProperty("fsid")]
        public string Fsid { get; set; }

        [JsonProperty("monmap")]
        public Monmap_ Monmap { get; set; }

        [JsonProperty("pgmap")]
        public Pgmap_ Pgmap { get; set; }

        [JsonProperty("election_epoch")]
        public int ElectionEpoch { get; set; }

        [JsonProperty("quorum_names")]
        public List<string> QuorumNames { get; set; }

        [JsonProperty("mgrmap")]
        public Mgrmap_ Mgrmap { get; set; }

        [JsonProperty("health")]
        public Health_ Health { get; set; }

        public class Active
        {
            [JsonProperty("tags")]
            public List<object> Tags { get; set; }

            [JsonProperty("see_also")]
            public List<object> SeeAlso { get; set; }

            [JsonProperty("desc")]
            public string Desc { get; set; }

            [JsonProperty("default_value")]
            public string DefaultValue { get; set; }

            [JsonProperty("flags")]
            public int Flags { get; set; }

            [JsonProperty("max")]
            public string Max { get; set; }

            [JsonProperty("type")]
            public string Type { get; set; }

            [JsonProperty("min")]
            public string Min { get; set; }

            [JsonProperty("enum_allowed")]
            public List<object> EnumAllowed { get; set; }

            [JsonProperty("level")]
            public string Level { get; set; }

            [JsonProperty("long_desc")]
            public string LongDesc { get; set; }

            [JsonProperty("name")]
            public string Name { get; set; }
        }

        public class ActiveAddrs
        {
            [JsonProperty("addrvec")]
            public List<Addrvec> Addrvec { get; set; }
        }

        public class ActiveClient
        {
            [JsonProperty("addrvec")]
            public List<Addrvec> Addrvec { get; set; }
        }

        public class Address
        {
            [JsonProperty("see_also")]
            public List<object> SeeAlso { get; set; }

            [JsonProperty("tags")]
            public List<object> Tags { get; set; }

            [JsonProperty("default_value")]
            public string DefaultValue { get; set; }

            [JsonProperty("desc")]
            public string Desc { get; set; }

            [JsonProperty("flags")]
            public int Flags { get; set; }

            [JsonProperty("min")]
            public string Min { get; set; }

            [JsonProperty("max")]
            public string Max { get; set; }

            [JsonProperty("type")]
            public string Type { get; set; }

            [JsonProperty("enum_allowed")]
            public List<object> EnumAllowed { get; set; }

            [JsonProperty("level")]
            public string Level { get; set; }

            [JsonProperty("long_desc")]
            public string LongDesc { get; set; }

            [JsonProperty("name")]
            public string Name { get; set; }
        }

        public class Addrvec
        {
            [JsonProperty("type")]
            public string Type { get; set; }

            [JsonProperty("addr")]
            public string Addr { get; set; }

            [JsonProperty("nonce")]
            public int Nonce { get; set; }
        }

        public class AllowMGranularity
        {
            [JsonProperty("long_desc")]
            public string LongDesc { get; set; }

            [JsonProperty("name")]
            public string Name { get; set; }

            [JsonProperty("min")]
            public string Min { get; set; }

            [JsonProperty("max")]
            public string Max { get; set; }

            [JsonProperty("type")]
            public string Type { get; set; }

            [JsonProperty("enum_allowed")]
            public List<object> EnumAllowed { get; set; }

            [JsonProperty("level")]
            public string Level { get; set; }

            [JsonProperty("flags")]
            public int Flags { get; set; }

            [JsonProperty("tags")]
            public List<object> Tags { get; set; }

            [JsonProperty("see_also")]
            public List<object> SeeAlso { get; set; }

            [JsonProperty("default_value")]
            public string DefaultValue { get; set; }

            [JsonProperty("desc")]
            public string Desc { get; set; }
        }

        public class AlwaysOnModules
        {
            [JsonProperty("nautilus")]
            public List<string> Nautilus { get; set; }

            [JsonProperty("last_failure_osd_epoch")]
            public int LastFailureOsdEpoch { get; set; }

            [JsonProperty("active_clients")]
            public List<ActiveClient> ActiveClients { get; set; }

            [JsonProperty("pacific")]
            public List<string> Pacific { get; set; }

            [JsonProperty("octopus")]
            public List<string> Octopus { get; set; }
        }

        public class AvailableModule
        {
            [JsonProperty("error_string")]
            public string ErrorString { get; set; }

            [JsonProperty("can_run")]
            public bool CanRun { get; set; }

            [JsonProperty("name")]
            public string Name { get; set; }

            [JsonProperty("module_options")]
            public ModuleOptions ModuleOptions { get; set; }
        }

        public class BatchSize
        {
            [JsonProperty("flags")]
            public int Flags { get; set; }

            [JsonProperty("default_value")]
            public string DefaultValue { get; set; }

            [JsonProperty("desc")]
            public string Desc { get; set; }

            [JsonProperty("see_also")]
            public List<object> SeeAlso { get; set; }

            [JsonProperty("tags")]
            public List<object> Tags { get; set; }

            [JsonProperty("name")]
            public string Name { get; set; }

            [JsonProperty("long_desc")]
            public string LongDesc { get; set; }

            [JsonProperty("level")]
            public string Level { get; set; }

            [JsonProperty("enum_allowed")]
            public List<object> EnumAllowed { get; set; }

            [JsonProperty("max")]
            public string Max { get; set; }

            [JsonProperty("min")]
            public string Min { get; set; }

            [JsonProperty("type")]
            public string Type { get; set; }
        }

        public class BeginTime
        {
            [JsonProperty("flags")]
            public int Flags { get; set; }

            [JsonProperty("desc")]
            public string Desc { get; set; }

            [JsonProperty("default_value")]
            public string DefaultValue { get; set; }

            [JsonProperty("tags")]
            public List<object> Tags { get; set; }

            [JsonProperty("see_also")]
            public List<object> SeeAlso { get; set; }

            [JsonProperty("name")]
            public string Name { get; set; }

            [JsonProperty("long_desc")]
            public string LongDesc { get; set; }

            [JsonProperty("level")]
            public string Level { get; set; }

            [JsonProperty("enum_allowed")]
            public List<object> EnumAllowed { get; set; }

            [JsonProperty("type")]
            public string Type { get; set; }

            [JsonProperty("max")]
            public string Max { get; set; }

            [JsonProperty("min")]
            public string Min { get; set; }
        }

        public class BeginWeekday
        {
            [JsonProperty("flags")]
            public int Flags { get; set; }

            [JsonProperty("see_also")]
            public List<object> SeeAlso { get; set; }

            [JsonProperty("tags")]
            public List<object> Tags { get; set; }

            [JsonProperty("default_value")]
            public string DefaultValue { get; set; }

            [JsonProperty("desc")]
            public string Desc { get; set; }

            [JsonProperty("long_desc")]
            public string LongDesc { get; set; }

            [JsonProperty("name")]
            public string Name { get; set; }

            [JsonProperty("type")]
            public string Type { get; set; }

            [JsonProperty("max")]
            public string Max { get; set; }

            [JsonProperty("min")]
            public string Min { get; set; }

            [JsonProperty("level")]
            public string Level { get; set; }

            [JsonProperty("enum_allowed")]
            public List<object> EnumAllowed { get; set; }
        }

        public class Cache
        {
            [JsonProperty("enum_allowed")]
            public List<object> EnumAllowed { get; set; }

            [JsonProperty("level")]
            public string Level { get; set; }

            [JsonProperty("max")]
            public string Max { get; set; }

            [JsonProperty("type")]
            public string Type { get; set; }

            [JsonProperty("min")]
            public string Min { get; set; }

            [JsonProperty("name")]
            public string Name { get; set; }

            [JsonProperty("long_desc")]
            public string LongDesc { get; set; }

            [JsonProperty("desc")]
            public string Desc { get; set; }

            [JsonProperty("default_value")]
            public string DefaultValue { get; set; }

            [JsonProperty("see_also")]
            public List<object> SeeAlso { get; set; }

            [JsonProperty("tags")]
            public List<object> Tags { get; set; }

            [JsonProperty("flags")]
            public int Flags { get; set; }
        }

        public class ChannelBasic
        {
            [JsonProperty("max")]
            public string Max { get; set; }

            [JsonProperty("min")]
            public string Min { get; set; }

            [JsonProperty("type")]
            public string Type { get; set; }

            [JsonProperty("level")]
            public string Level { get; set; }

            [JsonProperty("enum_allowed")]
            public List<object> EnumAllowed { get; set; }

            [JsonProperty("long_desc")]
            public string LongDesc { get; set; }

            [JsonProperty("name")]
            public string Name { get; set; }

            [JsonProperty("tags")]
            public List<object> Tags { get; set; }

            [JsonProperty("see_also")]
            public List<object> SeeAlso { get; set; }

            [JsonProperty("desc")]
            public string Desc { get; set; }

            [JsonProperty("default_value")]
            public string DefaultValue { get; set; }

            [JsonProperty("flags")]
            public int Flags { get; set; }
        }

        public class ChannelCrash
        {
            [JsonProperty("enum_allowed")]
            public List<object> EnumAllowed { get; set; }

            [JsonProperty("level")]
            public string Level { get; set; }

            [JsonProperty("max")]
            public string Max { get; set; }

            [JsonProperty("type")]
            public string Type { get; set; }

            [JsonProperty("min")]
            public string Min { get; set; }

            [JsonProperty("name")]
            public string Name { get; set; }

            [JsonProperty("long_desc")]
            public string LongDesc { get; set; }

            [JsonProperty("default_value")]
            public string DefaultValue { get; set; }

            [JsonProperty("desc")]
            public string Desc { get; set; }

            [JsonProperty("tags")]
            public List<object> Tags { get; set; }

            [JsonProperty("see_also")]
            public List<object> SeeAlso { get; set; }

            [JsonProperty("flags")]
            public int Flags { get; set; }
        }

        public class ChannelDevice
        {
            [JsonProperty("see_also")]
            public List<object> SeeAlso { get; set; }

            [JsonProperty("tags")]
            public List<object> Tags { get; set; }

            [JsonProperty("desc")]
            public string Desc { get; set; }

            [JsonProperty("default_value")]
            public string DefaultValue { get; set; }

            [JsonProperty("flags")]
            public int Flags { get; set; }

            [JsonProperty("min")]
            public string Min { get; set; }

            [JsonProperty("max")]
            public string Max { get; set; }

            [JsonProperty("type")]
            public string Type { get; set; }

            [JsonProperty("level")]
            public string Level { get; set; }

            [JsonProperty("enum_allowed")]
            public List<object> EnumAllowed { get; set; }

            [JsonProperty("long_desc")]
            public string LongDesc { get; set; }

            [JsonProperty("name")]
            public string Name { get; set; }
        }

        public class ChannelIdent
        {
            [JsonProperty("desc")]
            public string Desc { get; set; }

            [JsonProperty("default_value")]
            public string DefaultValue { get; set; }

            [JsonProperty("see_also")]
            public List<object> SeeAlso { get; set; }

            [JsonProperty("tags")]
            public List<object> Tags { get; set; }

            [JsonProperty("flags")]
            public int Flags { get; set; }

            [JsonProperty("enum_allowed")]
            public List<object> EnumAllowed { get; set; }

            [JsonProperty("level")]
            public string Level { get; set; }

            [JsonProperty("min")]
            public string Min { get; set; }

            [JsonProperty("max")]
            public string Max { get; set; }

            [JsonProperty("type")]
            public string Type { get; set; }

            [JsonProperty("name")]
            public string Name { get; set; }

            [JsonProperty("long_desc")]
            public string LongDesc { get; set; }
        }

        public class Checks
        {
        }

        public class Contact
        {
            [JsonProperty("tags")]
            public List<object> Tags { get; set; }

            [JsonProperty("see_also")]
            public List<object> SeeAlso { get; set; }

            [JsonProperty("default_value")]
            public string DefaultValue { get; set; }

            [JsonProperty("desc")]
            public string Desc { get; set; }

            [JsonProperty("flags")]
            public int Flags { get; set; }

            [JsonProperty("max")]
            public string Max { get; set; }

            [JsonProperty("type")]
            public string Type { get; set; }

            [JsonProperty("min")]
            public string Min { get; set; }

            [JsonProperty("enum_allowed")]
            public List<object> EnumAllowed { get; set; }

            [JsonProperty("level")]
            public string Level { get; set; }

            [JsonProperty("long_desc")]
            public string LongDesc { get; set; }

            [JsonProperty("name")]
            public string Name { get; set; }
        }

        public class CrushCompatMaxIterations
        {
            [JsonProperty("max")]
            public string Max { get; set; }

            [JsonProperty("min")]
            public string Min { get; set; }

            [JsonProperty("type")]
            public string Type { get; set; }

            [JsonProperty("enum_allowed")]
            public List<object> EnumAllowed { get; set; }

            [JsonProperty("level")]
            public string Level { get; set; }

            [JsonProperty("long_desc")]
            public string LongDesc { get; set; }

            [JsonProperty("name")]
            public string Name { get; set; }

            [JsonProperty("tags")]
            public List<object> Tags { get; set; }

            [JsonProperty("see_also")]
            public List<object> SeeAlso { get; set; }

            [JsonProperty("desc")]
            public string Desc { get; set; }

            [JsonProperty("default_value")]
            public string DefaultValue { get; set; }

            [JsonProperty("flags")]
            public int Flags { get; set; }
        }

        public class CrushCompatMetrics
        {
            [JsonProperty("flags")]
            public int Flags { get; set; }

            [JsonProperty("desc")]
            public string Desc { get; set; }

            [JsonProperty("default_value")]
            public string DefaultValue { get; set; }

            [JsonProperty("see_also")]
            public List<object> SeeAlso { get; set; }

            [JsonProperty("tags")]
            public List<object> Tags { get; set; }

            [JsonProperty("name")]
            public string Name { get; set; }

            [JsonProperty("long_desc")]
            public string LongDesc { get; set; }

            [JsonProperty("level")]
            public string Level { get; set; }

            [JsonProperty("enum_allowed")]
            public List<object> EnumAllowed { get; set; }

            [JsonProperty("max")]
            public string Max { get; set; }

            [JsonProperty("min")]
            public string Min { get; set; }

            [JsonProperty("type")]
            public string Type { get; set; }
        }

        public class CrushCompatStep
        {
            [JsonProperty("name")]
            public string Name { get; set; }

            [JsonProperty("long_desc")]
            public string LongDesc { get; set; }

            [JsonProperty("enum_allowed")]
            public List<object> EnumAllowed { get; set; }

            [JsonProperty("level")]
            public string Level { get; set; }

            [JsonProperty("min")]
            public string Min { get; set; }

            [JsonProperty("max")]
            public string Max { get; set; }

            [JsonProperty("type")]
            public string Type { get; set; }

            [JsonProperty("flags")]
            public int Flags { get; set; }

            [JsonProperty("desc")]
            public string Desc { get; set; }

            [JsonProperty("default_value")]
            public string DefaultValue { get; set; }

            [JsonProperty("tags")]
            public List<object> Tags { get; set; }

            [JsonProperty("see_also")]
            public List<object> SeeAlso { get; set; }
        }

        public class Database
        {
            [JsonProperty("level")]
            public string Level { get; set; }

            [JsonProperty("enum_allowed")]
            public List<object> EnumAllowed { get; set; }

            [JsonProperty("type")]
            public string Type { get; set; }

            [JsonProperty("max")]
            public string Max { get; set; }

            [JsonProperty("min")]
            public string Min { get; set; }

            [JsonProperty("name")]
            public string Name { get; set; }

            [JsonProperty("long_desc")]
            public string LongDesc { get; set; }

            [JsonProperty("desc")]
            public string Desc { get; set; }

            [JsonProperty("default_value")]
            public string DefaultValue { get; set; }

            [JsonProperty("see_also")]
            public List<object> SeeAlso { get; set; }

            [JsonProperty("tags")]
            public List<object> Tags { get; set; }

            [JsonProperty("flags")]
            public int Flags { get; set; }
        }

        public class Description
        {
            [JsonProperty("name")]
            public string Name { get; set; }

            [JsonProperty("long_desc")]
            public string LongDesc { get; set; }

            [JsonProperty("level")]
            public string Level { get; set; }

            [JsonProperty("enum_allowed")]
            public List<object> EnumAllowed { get; set; }

            [JsonProperty("max")]
            public string Max { get; set; }

            [JsonProperty("type")]
            public string Type { get; set; }

            [JsonProperty("min")]
            public string Min { get; set; }

            [JsonProperty("flags")]
            public int Flags { get; set; }

            [JsonProperty("desc")]
            public string Desc { get; set; }

            [JsonProperty("default_value")]
            public string DefaultValue { get; set; }

            [JsonProperty("tags")]
            public List<object> Tags { get; set; }

            [JsonProperty("see_also")]
            public List<object> SeeAlso { get; set; }
        }

        public class DeviceUrl
        {
            [JsonProperty("enum_allowed")]
            public List<object> EnumAllowed { get; set; }

            [JsonProperty("level")]
            public string Level { get; set; }

            [JsonProperty("type")]
            public string Type { get; set; }

            [JsonProperty("max")]
            public string Max { get; set; }

            [JsonProperty("min")]
            public string Min { get; set; }

            [JsonProperty("name")]
            public string Name { get; set; }

            [JsonProperty("long_desc")]
            public string LongDesc { get; set; }

            [JsonProperty("default_value")]
            public string DefaultValue { get; set; }

            [JsonProperty("desc")]
            public string Desc { get; set; }

            [JsonProperty("see_also")]
            public List<object> SeeAlso { get; set; }

            [JsonProperty("tags")]
            public List<object> Tags { get; set; }

            [JsonProperty("flags")]
            public int Flags { get; set; }
        }

        public class DiscoveryInterval
        {
            [JsonProperty("flags")]
            public int Flags { get; set; }

            [JsonProperty("tags")]
            public List<object> Tags { get; set; }

            [JsonProperty("see_also")]
            public List<object> SeeAlso { get; set; }

            [JsonProperty("desc")]
            public string Desc { get; set; }

            [JsonProperty("default_value")]
            public string DefaultValue { get; set; }

            [JsonProperty("long_desc")]
            public string LongDesc { get; set; }

            [JsonProperty("name")]
            public string Name { get; set; }

            [JsonProperty("min")]
            public string Min { get; set; }

            [JsonProperty("max")]
            public string Max { get; set; }

            [JsonProperty("type")]
            public string Type { get; set; }

            [JsonProperty("level")]
            public string Level { get; set; }

            [JsonProperty("enum_allowed")]
            public List<object> EnumAllowed { get; set; }
        }

        public class DumpOnUpdate
        {
            [JsonProperty("flags")]
            public int Flags { get; set; }

            [JsonProperty("default_value")]
            public string DefaultValue { get; set; }

            [JsonProperty("desc")]
            public string Desc { get; set; }

            [JsonProperty("tags")]
            public List<object> Tags { get; set; }

            [JsonProperty("see_also")]
            public List<object> SeeAlso { get; set; }

            [JsonProperty("name")]
            public string Name { get; set; }

            [JsonProperty("long_desc")]
            public string LongDesc { get; set; }

            [JsonProperty("level")]
            public string Level { get; set; }

            [JsonProperty("enum_allowed")]
            public List<object> EnumAllowed { get; set; }

            [JsonProperty("min")]
            public string Min { get; set; }

            [JsonProperty("max")]
            public string Max { get; set; }

            [JsonProperty("type")]
            public string Type { get; set; }
        }

        public class EnableAuth
        {
            [JsonProperty("flags")]
            public int Flags { get; set; }

            [JsonProperty("see_also")]
            public List<object> SeeAlso { get; set; }

            [JsonProperty("tags")]
            public List<object> Tags { get; set; }

            [JsonProperty("default_value")]
            public string DefaultValue { get; set; }

            [JsonProperty("desc")]
            public string Desc { get; set; }

            [JsonProperty("long_desc")]
            public string LongDesc { get; set; }

            [JsonProperty("name")]
            public string Name { get; set; }

            [JsonProperty("max")]
            public string Max { get; set; }

            [JsonProperty("min")]
            public string Min { get; set; }

            [JsonProperty("type")]
            public string Type { get; set; }

            [JsonProperty("enum_allowed")]
            public List<object> EnumAllowed { get; set; }

            [JsonProperty("level")]
            public string Level { get; set; }
        }

        public class Enabled
        {
            [JsonProperty("flags")]
            public int Flags { get; set; }

            [JsonProperty("desc")]
            public string Desc { get; set; }

            [JsonProperty("default_value")]
            public string DefaultValue { get; set; }

            [JsonProperty("tags")]
            public List<object> Tags { get; set; }

            [JsonProperty("see_also")]
            public List<object> SeeAlso { get; set; }

            [JsonProperty("name")]
            public string Name { get; set; }

            [JsonProperty("long_desc")]
            public string LongDesc { get; set; }

            [JsonProperty("level")]
            public string Level { get; set; }

            [JsonProperty("enum_allowed")]
            public List<object> EnumAllowed { get; set; }

            [JsonProperty("min")]
            public string Min { get; set; }

            [JsonProperty("max")]
            public string Max { get; set; }

            [JsonProperty("type")]
            public string Type { get; set; }
        }

        public class EnableMonitoring
        {
            [JsonProperty("max")]
            public string Max { get; set; }

            [JsonProperty("min")]
            public string Min { get; set; }

            [JsonProperty("type")]
            public string Type { get; set; }

            [JsonProperty("enum_allowed")]
            public List<object> EnumAllowed { get; set; }

            [JsonProperty("level")]
            public string Level { get; set; }

            [JsonProperty("long_desc")]
            public string LongDesc { get; set; }

            [JsonProperty("name")]
            public string Name { get; set; }

            [JsonProperty("tags")]
            public List<object> Tags { get; set; }

            [JsonProperty("see_also")]
            public List<object> SeeAlso { get; set; }

            [JsonProperty("desc")]
            public string Desc { get; set; }

            [JsonProperty("default_value")]
            public string DefaultValue { get; set; }

            [JsonProperty("flags")]
            public int Flags { get; set; }
        }

        public class EndTime
        {
            [JsonProperty("level")]
            public string Level { get; set; }

            [JsonProperty("enum_allowed")]
            public List<object> EnumAllowed { get; set; }

            [JsonProperty("max")]
            public string Max { get; set; }

            [JsonProperty("min")]
            public string Min { get; set; }

            [JsonProperty("type")]
            public string Type { get; set; }

            [JsonProperty("name")]
            public string Name { get; set; }

            [JsonProperty("long_desc")]
            public string LongDesc { get; set; }

            [JsonProperty("desc")]
            public string Desc { get; set; }

            [JsonProperty("default_value")]
            public string DefaultValue { get; set; }

            [JsonProperty("tags")]
            public List<object> Tags { get; set; }

            [JsonProperty("see_also")]
            public List<object> SeeAlso { get; set; }

            [JsonProperty("flags")]
            public int Flags { get; set; }
        }

        public class EndWeekday
        {
            [JsonProperty("name")]
            public string Name { get; set; }

            [JsonProperty("long_desc")]
            public string LongDesc { get; set; }

            [JsonProperty("level")]
            public string Level { get; set; }

            [JsonProperty("enum_allowed")]
            public List<object> EnumAllowed { get; set; }

            [JsonProperty("max")]
            public string Max { get; set; }

            [JsonProperty("type")]
            public string Type { get; set; }

            [JsonProperty("min")]
            public string Min { get; set; }

            [JsonProperty("flags")]
            public int Flags { get; set; }

            [JsonProperty("default_value")]
            public string DefaultValue { get; set; }

            [JsonProperty("desc")]
            public string Desc { get; set; }

            [JsonProperty("tags")]
            public List<object> Tags { get; set; }

            [JsonProperty("see_also")]
            public List<object> SeeAlso { get; set; }
        }

        public class FailureDomain
        {
            [JsonProperty("long_desc")]
            public string LongDesc { get; set; }

            [JsonProperty("name")]
            public string Name { get; set; }

            [JsonProperty("max")]
            public string Max { get; set; }

            [JsonProperty("type")]
            public string Type { get; set; }

            [JsonProperty("min")]
            public string Min { get; set; }

            [JsonProperty("level")]
            public string Level { get; set; }

            [JsonProperty("enum_allowed")]
            public List<object> EnumAllowed { get; set; }

            [JsonProperty("flags")]
            public int Flags { get; set; }

            [JsonProperty("see_also")]
            public List<object> SeeAlso { get; set; }

            [JsonProperty("tags")]
            public List<object> Tags { get; set; }

            [JsonProperty("desc")]
            public string Desc { get; set; }

            [JsonProperty("default_value")]
            public string DefaultValue { get; set; }
        }

        public class Features
        {
            [JsonProperty("optional")]
            public List<object> Optional { get; set; }

            [JsonProperty("persistent")]
            public List<string> Persistent { get; set; }
        }

        public class Fsmap_
        {
            [JsonProperty("epoch")]
            public int Epoch { get; set; }

            [JsonProperty("by_rank")]
            public List<object> ByRank { get; set; }

            [JsonProperty("up:standby")]
            public int UpStandby { get; set; }
        }

        public class Health_
        {
            [JsonProperty("mutes")]
            public List<object> Mutes { get; set; }

            [JsonProperty("status")]
            public string Status { get; set; }

            [JsonProperty("checks")]
            public Checks Checks { get; set; }
        }

        public class Hostname
        {
            [JsonProperty("default_value")]
            public string DefaultValue { get; set; }

            [JsonProperty("desc")]
            public string Desc { get; set; }

            [JsonProperty("tags")]
            public List<object> Tags { get; set; }

            [JsonProperty("see_also")]
            public List<object> SeeAlso { get; set; }

            [JsonProperty("flags")]
            public int Flags { get; set; }

            [JsonProperty("enum_allowed")]
            public List<object> EnumAllowed { get; set; }

            [JsonProperty("level")]
            public string Level { get; set; }

            [JsonProperty("min")]
            public string Min { get; set; }

            [JsonProperty("max")]
            public string Max { get; set; }

            [JsonProperty("type")]
            public string Type { get; set; }

            [JsonProperty("name")]
            public string Name { get; set; }

            [JsonProperty("long_desc")]
            public string LongDesc { get; set; }
        }

        public class Identifier
        {
            [JsonProperty("level")]
            public string Level { get; set; }

            [JsonProperty("enum_allowed")]
            public List<object> EnumAllowed { get; set; }

            [JsonProperty("max")]
            public string Max { get; set; }

            [JsonProperty("min")]
            public string Min { get; set; }

            [JsonProperty("type")]
            public string Type { get; set; }

            [JsonProperty("name")]
            public string Name { get; set; }

            [JsonProperty("long_desc")]
            public string LongDesc { get; set; }

            [JsonProperty("default_value")]
            public string DefaultValue { get; set; }

            [JsonProperty("desc")]
            public string Desc { get; set; }

            [JsonProperty("see_also")]
            public List<object> SeeAlso { get; set; }

            [JsonProperty("tags")]
            public List<object> Tags { get; set; }

            [JsonProperty("flags")]
            public int Flags { get; set; }
        }

        public class Interval
        {
            [JsonProperty("see_also")]
            public List<object> SeeAlso { get; set; }

            [JsonProperty("tags")]
            public List<object> Tags { get; set; }

            [JsonProperty("desc")]
            public string Desc { get; set; }

            [JsonProperty("default_value")]
            public string DefaultValue { get; set; }

            [JsonProperty("flags")]
            public int Flags { get; set; }

            [JsonProperty("max")]
            public string Max { get; set; }

            [JsonProperty("min")]
            public string Min { get; set; }

            [JsonProperty("type")]
            public string Type { get; set; }

            [JsonProperty("level")]
            public string Level { get; set; }

            [JsonProperty("enum_allowed")]
            public List<object> EnumAllowed { get; set; }

            [JsonProperty("long_desc")]
            public string LongDesc { get; set; }

            [JsonProperty("name")]
            public string Name { get; set; }
        }

        public class KeyFile
        {
            [JsonProperty("default_value")]
            public string DefaultValue { get; set; }

            [JsonProperty("desc")]
            public string Desc { get; set; }

            [JsonProperty("tags")]
            public List<object> Tags { get; set; }

            [JsonProperty("see_also")]
            public List<object> SeeAlso { get; set; }

            [JsonProperty("flags")]
            public int Flags { get; set; }

            [JsonProperty("level")]
            public string Level { get; set; }

            [JsonProperty("enum_allowed")]
            public List<object> EnumAllowed { get; set; }

            [JsonProperty("max")]
            public string Max { get; set; }

            [JsonProperty("min")]
            public string Min { get; set; }

            [JsonProperty("type")]
            public string Type { get; set; }

            [JsonProperty("name")]
            public string Name { get; set; }

            [JsonProperty("long_desc")]
            public string LongDesc { get; set; }
        }

        public class LastOptRevision
        {
            [JsonProperty("name")]
            public string Name { get; set; }

            [JsonProperty("long_desc")]
            public string LongDesc { get; set; }

            [JsonProperty("enum_allowed")]
            public List<object> EnumAllowed { get; set; }

            [JsonProperty("level")]
            public string Level { get; set; }

            [JsonProperty("max")]
            public string Max { get; set; }

            [JsonProperty("min")]
            public string Min { get; set; }

            [JsonProperty("type")]
            public string Type { get; set; }

            [JsonProperty("flags")]
            public int Flags { get; set; }

            [JsonProperty("default_value")]
            public string DefaultValue { get; set; }

            [JsonProperty("desc")]
            public string Desc { get; set; }

            [JsonProperty("see_also")]
            public List<object> SeeAlso { get; set; }

            [JsonProperty("tags")]
            public List<object> Tags { get; set; }
        }

        public class Leaderboard
        {
            [JsonProperty("flags")]
            public int Flags { get; set; }

            [JsonProperty("tags")]
            public List<object> Tags { get; set; }

            [JsonProperty("see_also")]
            public List<object> SeeAlso { get; set; }

            [JsonProperty("desc")]
            public string Desc { get; set; }

            [JsonProperty("default_value")]
            public string DefaultValue { get; set; }

            [JsonProperty("long_desc")]
            public string LongDesc { get; set; }

            [JsonProperty("name")]
            public string Name { get; set; }

            [JsonProperty("max")]
            public string Max { get; set; }

            [JsonProperty("min")]
            public string Min { get; set; }

            [JsonProperty("type")]
            public string Type { get; set; }

            [JsonProperty("enum_allowed")]
            public List<object> EnumAllowed { get; set; }

            [JsonProperty("level")]
            public string Level { get; set; }
        }

        public class LogLevel
        {
            [JsonProperty("flags")]
            public int Flags { get; set; }

            [JsonProperty("desc")]
            public string Desc { get; set; }

            [JsonProperty("default_value")]
            public string DefaultValue { get; set; }

            [JsonProperty("tags")]
            public List<object> Tags { get; set; }

            [JsonProperty("see_also")]
            public List<object> SeeAlso { get; set; }

            [JsonProperty("name")]
            public string Name { get; set; }

            [JsonProperty("long_desc")]
            public string LongDesc { get; set; }

            [JsonProperty("enum_allowed")]
            public List<string> EnumAllowed { get; set; }

            [JsonProperty("level")]
            public string Level { get; set; }

            [JsonProperty("max")]
            public string Max { get; set; }

            [JsonProperty("type")]
            public string Type { get; set; }

            [JsonProperty("min")]
            public string Min { get; set; }
        }

        public class LogToCluster
        {
            [JsonProperty("see_also")]
            public List<object> SeeAlso { get; set; }

            [JsonProperty("tags")]
            public List<object> Tags { get; set; }

            [JsonProperty("default_value")]
            public string DefaultValue { get; set; }

            [JsonProperty("desc")]
            public string Desc { get; set; }

            [JsonProperty("flags")]
            public int Flags { get; set; }

            [JsonProperty("max")]
            public string Max { get; set; }

            [JsonProperty("type")]
            public string Type { get; set; }

            [JsonProperty("min")]
            public string Min { get; set; }

            [JsonProperty("level")]
            public string Level { get; set; }

            [JsonProperty("enum_allowed")]
            public List<object> EnumAllowed { get; set; }

            [JsonProperty("long_desc")]
            public string LongDesc { get; set; }

            [JsonProperty("name")]
            public string Name { get; set; }
        }

        public class LogToClusterLevel
        {
            [JsonProperty("see_also")]
            public List<object> SeeAlso { get; set; }

            [JsonProperty("tags")]
            public List<object> Tags { get; set; }

            [JsonProperty("desc")]
            public string Desc { get; set; }

            [JsonProperty("default_value")]
            public string DefaultValue { get; set; }

            [JsonProperty("flags")]
            public int Flags { get; set; }

            [JsonProperty("max")]
            public string Max { get; set; }

            [JsonProperty("type")]
            public string Type { get; set; }

            [JsonProperty("min")]
            public string Min { get; set; }

            [JsonProperty("level")]
            public string Level { get; set; }

            [JsonProperty("enum_allowed")]
            public List<string> EnumAllowed { get; set; }

            [JsonProperty("long_desc")]
            public string LongDesc { get; set; }

            [JsonProperty("name")]
            public string Name { get; set; }
        }

        public class LogToFile
        {
            [JsonProperty("name")]
            public string Name { get; set; }

            [JsonProperty("long_desc")]
            public string LongDesc { get; set; }

            [JsonProperty("level")]
            public string Level { get; set; }

            [JsonProperty("enum_allowed")]
            public List<object> EnumAllowed { get; set; }

            [JsonProperty("min")]
            public string Min { get; set; }

            [JsonProperty("max")]
            public string Max { get; set; }

            [JsonProperty("type")]
            public string Type { get; set; }

            [JsonProperty("flags")]
            public int Flags { get; set; }

            [JsonProperty("default_value")]
            public string DefaultValue { get; set; }

            [JsonProperty("desc")]
            public string Desc { get; set; }

            [JsonProperty("see_also")]
            public List<object> SeeAlso { get; set; }

            [JsonProperty("tags")]
            public List<object> Tags { get; set; }
        }

        public class MarkOutThreshold
        {
            [JsonProperty("long_desc")]
            public string LongDesc { get; set; }

            [JsonProperty("name")]
            public string Name { get; set; }

            [JsonProperty("max")]
            public string Max { get; set; }

            [JsonProperty("type")]
            public string Type { get; set; }

            [JsonProperty("min")]
            public string Min { get; set; }

            [JsonProperty("enum_allowed")]
            public List<object> EnumAllowed { get; set; }

            [JsonProperty("level")]
            public string Level { get; set; }

            [JsonProperty("flags")]
            public int Flags { get; set; }

            [JsonProperty("tags")]
            public List<object> Tags { get; set; }

            [JsonProperty("see_also")]
            public List<object> SeeAlso { get; set; }

            [JsonProperty("default_value")]
            public string DefaultValue { get; set; }

            [JsonProperty("desc")]
            public string Desc { get; set; }
        }

        public class MaxCompletedEvents
        {
            [JsonProperty("name")]
            public string Name { get; set; }

            [JsonProperty("long_desc")]
            public string LongDesc { get; set; }

            [JsonProperty("enum_allowed")]
            public List<object> EnumAllowed { get; set; }

            [JsonProperty("level")]
            public string Level { get; set; }

            [JsonProperty("type")]
            public string Type { get; set; }

            [JsonProperty("max")]
            public string Max { get; set; }

            [JsonProperty("min")]
            public string Min { get; set; }

            [JsonProperty("flags")]
            public int Flags { get; set; }

            [JsonProperty("desc")]
            public string Desc { get; set; }

            [JsonProperty("default_value")]
            public string DefaultValue { get; set; }

            [JsonProperty("tags")]
            public List<object> Tags { get; set; }

            [JsonProperty("see_also")]
            public List<object> SeeAlso { get; set; }
        }

        public class MaxConcurrentClones
        {
            [JsonProperty("desc")]
            public string Desc { get; set; }

            [JsonProperty("default_value")]
            public string DefaultValue { get; set; }

            [JsonProperty("see_also")]
            public List<object> SeeAlso { get; set; }

            [JsonProperty("tags")]
            public List<object> Tags { get; set; }

            [JsonProperty("flags")]
            public int Flags { get; set; }

            [JsonProperty("enum_allowed")]
            public List<object> EnumAllowed { get; set; }

            [JsonProperty("level")]
            public string Level { get; set; }

            [JsonProperty("max")]
            public string Max { get; set; }

            [JsonProperty("type")]
            public string Type { get; set; }

            [JsonProperty("min")]
            public string Min { get; set; }

            [JsonProperty("name")]
            public string Name { get; set; }

            [JsonProperty("long_desc")]
            public string LongDesc { get; set; }
        }

        public class MaxConcurrentSnapCreate
        {
            [JsonProperty("max")]
            public string Max { get; set; }

            [JsonProperty("type")]
            public string Type { get; set; }

            [JsonProperty("min")]
            public string Min { get; set; }

            [JsonProperty("enum_allowed")]
            public List<object> EnumAllowed { get; set; }

            [JsonProperty("level")]
            public string Level { get; set; }

            [JsonProperty("long_desc")]
            public string LongDesc { get; set; }

            [JsonProperty("name")]
            public string Name { get; set; }

            [JsonProperty("tags")]
            public List<object> Tags { get; set; }

            [JsonProperty("see_also")]
            public List<object> SeeAlso { get; set; }

            [JsonProperty("desc")]
            public string Desc { get; set; }

            [JsonProperty("default_value")]
            public string DefaultValue { get; set; }

            [JsonProperty("flags")]
            public int Flags { get; set; }
        }

        public class Mgrmap_
        {
            [JsonProperty("active_change")]
            public DateTime ActiveChange { get; set; }

            [JsonProperty("active_name")]
            public string ActiveName { get; set; }

            [JsonProperty("available_modules")]
            public List<AvailableModule> AvailableModules { get; set; }

            [JsonProperty("modules")]
            public List<string> Modules { get; set; }

            [JsonProperty("always_on_modules")]
            public AlwaysOnModules AlwaysOnModules { get; set; }

            [JsonProperty("epoch")]
            public int Epoch { get; set; }

            [JsonProperty("available")]
            public bool Available { get; set; }

            [JsonProperty("standbys")]
            public List<Standby> Standbys { get; set; }

            [JsonProperty("active_addrs")]
            public ActiveAddrs ActiveAddrs { get; set; }

            [JsonProperty("active_addr")]
            public string ActiveAddr { get; set; }

            [JsonProperty("active_gid")]
            public int ActiveGid { get; set; }

            [JsonProperty("active_mgr_features")]
            public long ActiveMgrFeatures { get; set; }

            [JsonProperty("services")]
            public Services Services { get; set; }
        }

        public class MinScore
        {
            [JsonProperty("default_value")]
            public string DefaultValue { get; set; }

            [JsonProperty("desc")]
            public string Desc { get; set; }

            [JsonProperty("see_also")]
            public List<object> SeeAlso { get; set; }

            [JsonProperty("tags")]
            public List<object> Tags { get; set; }

            [JsonProperty("flags")]
            public int Flags { get; set; }

            [JsonProperty("enum_allowed")]
            public List<object> EnumAllowed { get; set; }

            [JsonProperty("level")]
            public string Level { get; set; }

            [JsonProperty("max")]
            public string Max { get; set; }

            [JsonProperty("min")]
            public string Min { get; set; }

            [JsonProperty("type")]
            public string Type { get; set; }

            [JsonProperty("name")]
            public string Name { get; set; }

            [JsonProperty("long_desc")]
            public string LongDesc { get; set; }
        }

        public class MinSize
        {
            [JsonProperty("default_value")]
            public string DefaultValue { get; set; }

            [JsonProperty("desc")]
            public string Desc { get; set; }

            [JsonProperty("tags")]
            public List<object> Tags { get; set; }

            [JsonProperty("see_also")]
            public List<object> SeeAlso { get; set; }

            [JsonProperty("flags")]
            public int Flags { get; set; }

            [JsonProperty("level")]
            public string Level { get; set; }

            [JsonProperty("enum_allowed")]
            public List<object> EnumAllowed { get; set; }

            [JsonProperty("min")]
            public string Min { get; set; }

            [JsonProperty("max")]
            public string Max { get; set; }

            [JsonProperty("type")]
            public string Type { get; set; }

            [JsonProperty("name")]
            public string Name { get; set; }

            [JsonProperty("long_desc")]
            public string LongDesc { get; set; }
        }

        public class MirrorSnapshotSchedule
        {
            [JsonProperty("desc")]
            public string Desc { get; set; }

            [JsonProperty("default_value")]
            public string DefaultValue { get; set; }

            [JsonProperty("see_also")]
            public List<object> SeeAlso { get; set; }

            [JsonProperty("tags")]
            public List<object> Tags { get; set; }

            [JsonProperty("flags")]
            public int Flags { get; set; }

            [JsonProperty("enum_allowed")]
            public List<object> EnumAllowed { get; set; }

            [JsonProperty("level")]
            public string Level { get; set; }

            [JsonProperty("min")]
            public string Min { get; set; }

            [JsonProperty("max")]
            public string Max { get; set; }

            [JsonProperty("type")]
            public string Type { get; set; }

            [JsonProperty("name")]
            public string Name { get; set; }

            [JsonProperty("long_desc")]
            public string LongDesc { get; set; }
        }

        public class Mode
        {
            [JsonProperty("name")]
            public string Name { get; set; }

            [JsonProperty("long_desc")]
            public string LongDesc { get; set; }

            [JsonProperty("level")]
            public string Level { get; set; }

            [JsonProperty("enum_allowed")]
            public List<string> EnumAllowed { get; set; }

            [JsonProperty("min")]
            public string Min { get; set; }

            [JsonProperty("max")]
            public string Max { get; set; }

            [JsonProperty("type")]
            public string Type { get; set; }

            [JsonProperty("flags")]
            public int Flags { get; set; }

            [JsonProperty("default_value")]
            public string DefaultValue { get; set; }

            [JsonProperty("desc")]
            public string Desc { get; set; }

            [JsonProperty("tags")]
            public List<object> Tags { get; set; }

            [JsonProperty("see_also")]
            public List<object> SeeAlso { get; set; }
        }

        public class ModuleOptions
        {
            [JsonProperty("smtp_user")]
            public SmtpUser SmtpUser { get; set; }

            [JsonProperty("smtp_from_name")]
            public SmtpFromName SmtpFromName { get; set; }

            [JsonProperty("log_to_file")]
            public LogToFile LogToFile { get; set; }

            [JsonProperty("smtp_ssl")]
            public SmtpSsl SmtpSsl { get; set; }

            [JsonProperty("interval")]
            public Interval Interval { get; set; }

            [JsonProperty("log_level")]
            public LogLevel LogLevel { get; set; }

            [JsonProperty("smtp_port")]
            public SmtpPort SmtpPort { get; set; }

            [JsonProperty("log_to_cluster_level")]
            public LogToClusterLevel LogToClusterLevel { get; set; }

            [JsonProperty("smtp_destination")]
            public SmtpDestination SmtpDestination { get; set; }

            [JsonProperty("smtp_sender")]
            public SmtpSender SmtpSender { get; set; }

            [JsonProperty("smtp_host")]
            public SmtpHost SmtpHost { get; set; }

            [JsonProperty("log_to_cluster")]
            public LogToCluster LogToCluster { get; set; }

            [JsonProperty("smtp_password")]
            public SmtpPassword SmtpPassword { get; set; }

            [JsonProperty("crush_compat_max_iterations")]
            public CrushCompatMaxIterations CrushCompatMaxIterations { get; set; }

            [JsonProperty("upmap_max_optimizations")]
            public UpmapMaxOptimizations UpmapMaxOptimizations { get; set; }

            [JsonProperty("active")]
            public Active Active { get; set; }

            [JsonProperty("upmap_max_deviation")]
            public UpmapMaxDeviation UpmapMaxDeviation { get; set; }

            [JsonProperty("sleep_interval")]
            public SleepInterval SleepInterval { get; set; }

            [JsonProperty("end_time")]
            public EndTime EndTime { get; set; }

            [JsonProperty("begin_time")]
            public BeginTime BeginTime { get; set; }

            [JsonProperty("pool_ids")]
            public PoolIds PoolIds { get; set; }

            [JsonProperty("begin_weekday")]
            public BeginWeekday BeginWeekday { get; set; }

            [JsonProperty("mode")]
            public Mode Mode { get; set; }

            [JsonProperty("min_score")]
            public MinScore MinScore { get; set; }

            [JsonProperty("crush_compat_step")]
            public CrushCompatStep CrushCompatStep { get; set; }

            [JsonProperty("crush_compat_metrics")]
            public CrushCompatMetrics CrushCompatMetrics { get; set; }

            [JsonProperty("end_weekday")]
            public EndWeekday EndWeekday { get; set; }

            [JsonProperty("retain_interval")]
            public RetainInterval RetainInterval { get; set; }

            [JsonProperty("warn_recent_interval")]
            public WarnRecentInterval WarnRecentInterval { get; set; }

            [JsonProperty("warn_threshold")]
            public WarnThreshold WarnThreshold { get; set; }

            [JsonProperty("mark_out_threshold")]
            public MarkOutThreshold MarkOutThreshold { get; set; }

            [JsonProperty("scrape_frequency")]
            public ScrapeFrequency ScrapeFrequency { get; set; }

            [JsonProperty("pool_name")]
            public PoolName PoolName { get; set; }

            [JsonProperty("enable_monitoring")]
            public EnableMonitoring EnableMonitoring { get; set; }

            [JsonProperty("retention_period")]
            public RetentionPeriod RetentionPeriod { get; set; }

            [JsonProperty("self_heal")]
            public SelfHeal SelfHeal { get; set; }

            [JsonProperty("ssl")]
            public Ssl Ssl { get; set; }

            [JsonProperty("password")]
            public Password Password { get; set; }

            [JsonProperty("batch_size")]
            public BatchSize BatchSize { get; set; }

            [JsonProperty("username")]
            public Username Username { get; set; }

            [JsonProperty("hostname")]
            public Hostname Hostname { get; set; }

            [JsonProperty("database")]
            public Database Database { get; set; }

            [JsonProperty("port")]
            public Port Port { get; set; }

            [JsonProperty("verify_ssl")]
            public VerifySsl VerifySsl { get; set; }

            [JsonProperty("threads")]
            public Threads Threads { get; set; }

            [JsonProperty("num_rep")]
            public NumRep NumRep { get; set; }

            [JsonProperty("min_size")]
            public MinSize MinSize { get; set; }

            [JsonProperty("failure_domain")]
            public FailureDomain FailureDomain { get; set; }

            [JsonProperty("prefix")]
            public Prefix Prefix { get; set; }

            [JsonProperty("pg_num")]
            public PgNum PgNum { get; set; }

            [JsonProperty("subtree")]
            public Subtree Subtree { get; set; }

            [JsonProperty("orchestrator")]
            public Orchestrator Orchestrator { get; set; }

            [JsonProperty("noautoscale")]
            public Noautoscale Noautoscale { get; set; }

            [JsonProperty("enabled")]
            public Enabled Enabled { get; set; }

            [JsonProperty("max_completed_events")]
            public MaxCompletedEvents MaxCompletedEvents { get; set; }

            [JsonProperty("standby_error_status_code")]
            public StandbyErrorStatusCode StandbyErrorStatusCode { get; set; }

            [JsonProperty("server_port")]
            public ServerPort ServerPort { get; set; }

            [JsonProperty("server_addr")]
            public ServerAddr ServerAddr { get; set; }

            [JsonProperty("rbd_stats_pools")]
            public RbdStatsPools RbdStatsPools { get; set; }

            [JsonProperty("stale_cache_strategy")]
            public StaleCacheStrategy StaleCacheStrategy { get; set; }

            [JsonProperty("rbd_stats_pools_refresh_interval")]
            public RbdStatsPoolsRefreshInterval RbdStatsPoolsRefreshInterval { get; set; }

            [JsonProperty("scrape_interval")]
            public ScrapeInterval ScrapeInterval { get; set; }

            [JsonProperty("cache")]
            public Cache Cache { get; set; }

            [JsonProperty("standby_behaviour")]
            public StandbyBehaviour StandbyBehaviour { get; set; }

            [JsonProperty("max_concurrent_snap_create")]
            public MaxConcurrentSnapCreate MaxConcurrentSnapCreate { get; set; }

            [JsonProperty("trash_purge_schedule")]
            public TrashPurgeSchedule TrashPurgeSchedule { get; set; }

            [JsonProperty("mirror_snapshot_schedule")]
            public MirrorSnapshotSchedule MirrorSnapshotSchedule { get; set; }

            [JsonProperty("enable_auth")]
            public EnableAuth EnableAuth { get; set; }

            [JsonProperty("key_file")]
            public KeyFile KeyFile { get; set; }

            [JsonProperty("rwoption3")]
            public Rwoption3 Rwoption3 { get; set; }

            [JsonProperty("testkey")]
            public Testkey Testkey { get; set; }

            [JsonProperty("rwoption2")]
            public Rwoption2 Rwoption2 { get; set; }

            [JsonProperty("rwoption1")]
            public Rwoption1 Rwoption1 { get; set; }

            [JsonProperty("roption1")]
            public Roption1 Roption1 { get; set; }

            [JsonProperty("testlkey")]
            public Testlkey Testlkey { get; set; }

            [JsonProperty("rwoption5")]
            public Rwoption5 Rwoption5 { get; set; }

            [JsonProperty("roption2")]
            public Roption2 Roption2 { get; set; }

            [JsonProperty("rwoption6")]
            public Rwoption6 Rwoption6 { get; set; }

            [JsonProperty("rwoption4")]
            public Rwoption4 Rwoption4 { get; set; }

            [JsonProperty("testnewline")]
            public Testnewline Testnewline { get; set; }

            [JsonProperty("dump_on_update")]
            public DumpOnUpdate DumpOnUpdate { get; set; }

            [JsonProperty("allow_m_granularity")]
            public AllowMGranularity AllowMGranularity { get; set; }

            [JsonProperty("address")]
            public Address Address { get; set; }

            [JsonProperty("last_opt_revision")]
            public LastOptRevision LastOptRevision { get; set; }

            [JsonProperty("organization")]
            public Organization Organization { get; set; }

            [JsonProperty("device_url")]
            public DeviceUrl DeviceUrl { get; set; }

            [JsonProperty("description")]
            public Description Description { get; set; }

            [JsonProperty("channel_ident")]
            public ChannelIdent ChannelIdent { get; set; }

            [JsonProperty("channel_crash")]
            public ChannelCrash ChannelCrash { get; set; }

            [JsonProperty("leaderboard")]
            public Leaderboard Leaderboard { get; set; }

            [JsonProperty("proxy")]
            public Proxy Proxy { get; set; }

            [JsonProperty("channel_basic")]
            public ChannelBasic ChannelBasic { get; set; }

            [JsonProperty("url")]
            public Url Url { get; set; }

            [JsonProperty("channel_device")]
            public ChannelDevice ChannelDevice { get; set; }

            [JsonProperty("contact")]
            public Contact Contact { get; set; }

            [JsonProperty("max_concurrent_clones")]
            public MaxConcurrentClones MaxConcurrentClones { get; set; }

            [JsonProperty("snapshot_clone_delay")]
            public SnapshotCloneDelay SnapshotCloneDelay { get; set; }

            [JsonProperty("discovery_interval")]
            public DiscoveryInterval DiscoveryInterval { get; set; }

            [JsonProperty("zabbix_port")]
            public ZabbixPort ZabbixPort { get; set; }

            [JsonProperty("identifier")]
            public Identifier Identifier { get; set; }

            [JsonProperty("zabbix_host")]
            public ZabbixHost ZabbixHost { get; set; }

            [JsonProperty("zabbix_sender")]
            public ZabbixSender ZabbixSender { get; set; }
        }

        public class Mon
        {
            [JsonProperty("weight")]
            public int Weight { get; set; }

            [JsonProperty("public_addrs")]
            public PublicAddrs PublicAddrs { get; set; }

            [JsonProperty("public_addr")]
            public string PublicAddr { get; set; }

            [JsonProperty("rank")]
            public int Rank { get; set; }

            [JsonProperty("name")]
            public string Name { get; set; }

            [JsonProperty("priority")]
            public int Priority { get; set; }

            [JsonProperty("crush_location")]
            public string CrushLocation { get; set; }

            [JsonProperty("addr")]
            public string Addr { get; set; }
        }

        public class Monmap_
        {
            [JsonProperty("quorum")]
            public List<int> Quorum { get; set; }

            [JsonProperty("fsid")]
            public string Fsid { get; set; }

            [JsonProperty("modified")]
            public DateTime Modified { get; set; }

            [JsonProperty("created")]
            public DateTime Created { get; set; }

            [JsonProperty("stretch_mode")]
            public bool StretchMode { get; set; }

            [JsonProperty("features")]
            public Features Features { get; set; }

            [JsonProperty("disallowed_leaders: ")]
            public string DisallowedLeaders { get; set; }

            [JsonProperty("tiebreaker_mon")]
            public string TiebreakerMon { get; set; }

            [JsonProperty("mons")]
            public List<Mon> Mons { get; set; }

            [JsonProperty("min_mon_release_name")]
            public string MinMonReleaseName { get; set; }

            [JsonProperty("election_strategy")]
            public int ElectionStrategy { get; set; }

            [JsonProperty("epoch")]
            public int Epoch { get; set; }

            [JsonProperty("min_mon_release")]
            public int MinMonRelease { get; set; }
        }

        public class Noautoscale
        {
            [JsonProperty("max")]
            public string Max { get; set; }

            [JsonProperty("min")]
            public string Min { get; set; }

            [JsonProperty("type")]
            public string Type { get; set; }

            [JsonProperty("enum_allowed")]
            public List<object> EnumAllowed { get; set; }

            [JsonProperty("level")]
            public string Level { get; set; }

            [JsonProperty("long_desc")]
            public string LongDesc { get; set; }

            [JsonProperty("name")]
            public string Name { get; set; }

            [JsonProperty("tags")]
            public List<object> Tags { get; set; }

            [JsonProperty("see_also")]
            public List<object> SeeAlso { get; set; }

            [JsonProperty("default_value")]
            public string DefaultValue { get; set; }

            [JsonProperty("desc")]
            public string Desc { get; set; }

            [JsonProperty("flags")]
            public int Flags { get; set; }
        }

        public class NumRep
        {
            [JsonProperty("long_desc")]
            public string LongDesc { get; set; }

            [JsonProperty("name")]
            public string Name { get; set; }

            [JsonProperty("type")]
            public string Type { get; set; }

            [JsonProperty("max")]
            public string Max { get; set; }

            [JsonProperty("min")]
            public string Min { get; set; }

            [JsonProperty("enum_allowed")]
            public List<object> EnumAllowed { get; set; }

            [JsonProperty("level")]
            public string Level { get; set; }

            [JsonProperty("flags")]
            public int Flags { get; set; }

            [JsonProperty("see_also")]
            public List<object> SeeAlso { get; set; }

            [JsonProperty("tags")]
            public List<object> Tags { get; set; }

            [JsonProperty("default_value")]
            public string DefaultValue { get; set; }

            [JsonProperty("desc")]
            public string Desc { get; set; }
        }

        public class Orchestrator
        {
            [JsonProperty("enum_allowed")]
            public List<string> EnumAllowed { get; set; }

            [JsonProperty("level")]
            public string Level { get; set; }

            [JsonProperty("max")]
            public string Max { get; set; }

            [JsonProperty("type")]
            public string Type { get; set; }

            [JsonProperty("min")]
            public string Min { get; set; }

            [JsonProperty("name")]
            public string Name { get; set; }

            [JsonProperty("long_desc")]
            public string LongDesc { get; set; }

            [JsonProperty("desc")]
            public string Desc { get; set; }

            [JsonProperty("default_value")]
            public string DefaultValue { get; set; }

            [JsonProperty("tags")]
            public List<object> Tags { get; set; }

            [JsonProperty("see_also")]
            public List<object> SeeAlso { get; set; }

            [JsonProperty("flags")]
            public int Flags { get; set; }
        }

        public class Organization
        {
            [JsonProperty("long_desc")]
            public string LongDesc { get; set; }

            [JsonProperty("name")]
            public string Name { get; set; }

            [JsonProperty("min")]
            public string Min { get; set; }

            [JsonProperty("max")]
            public string Max { get; set; }

            [JsonProperty("type")]
            public string Type { get; set; }

            [JsonProperty("level")]
            public string Level { get; set; }

            [JsonProperty("enum_allowed")]
            public List<object> EnumAllowed { get; set; }

            [JsonProperty("flags")]
            public int Flags { get; set; }

            [JsonProperty("see_also")]
            public List<object> SeeAlso { get; set; }

            [JsonProperty("tags")]
            public List<object> Tags { get; set; }

            [JsonProperty("desc")]
            public string Desc { get; set; }

            [JsonProperty("default_value")]
            public string DefaultValue { get; set; }
        }

        public class Osdmap_
        {
            [JsonProperty("num_up_osds")]
            public int NumUpOsds { get; set; }

            [JsonProperty("num_osds")]
            public int NumOsds { get; set; }

            [JsonProperty("num_in_osds")]
            public int NumInOsds { get; set; }

            [JsonProperty("num_remapped_pgs")]
            public int NumRemappedPgs { get; set; }

            [JsonProperty("epoch")]
            public int Epoch { get; set; }

            [JsonProperty("osd_up_since")]
            public int OsdUpSince { get; set; }

            [JsonProperty("osd_in_since")]
            public int OsdInSince { get; set; }
        }

        public class Password
        {
            [JsonProperty("long_desc")]
            public string LongDesc { get; set; }

            [JsonProperty("name")]
            public string Name { get; set; }

            [JsonProperty("min")]
            public string Min { get; set; }

            [JsonProperty("max")]
            public string Max { get; set; }

            [JsonProperty("type")]
            public string Type { get; set; }

            [JsonProperty("enum_allowed")]
            public List<object> EnumAllowed { get; set; }

            [JsonProperty("level")]
            public string Level { get; set; }

            [JsonProperty("flags")]
            public int Flags { get; set; }

            [JsonProperty("see_also")]
            public List<object> SeeAlso { get; set; }

            [JsonProperty("tags")]
            public List<object> Tags { get; set; }

            [JsonProperty("default_value")]
            public string DefaultValue { get; set; }

            [JsonProperty("desc")]
            public string Desc { get; set; }
        }

        public class Pgmap_
        {
            [JsonProperty("num_pools")]
            public int NumPools { get; set; }

            [JsonProperty("pgs_by_state")]
            public List<PgsByState> PgsByState { get; set; }

            [JsonProperty("bytes_used")]
            public long BytesUsed { get; set; }

            [JsonProperty("data_bytes")]
            public long DataBytes { get; set; }

            [JsonProperty("read_bytes_sec")]
            public int ReadBytesSec { get; set; }

            [JsonProperty("num_pgs")]
            public int NumPgs { get; set; }

            [JsonProperty("read_op_per_sec")]
            public int ReadOpPerSec { get; set; }

            [JsonProperty("write_bytes_sec")]
            public int WriteBytesSec { get; set; }

            [JsonProperty("num_objects")]
            public int NumObjects { get; set; }

            [JsonProperty("write_op_per_sec")]
            public int WriteOpPerSec { get; set; }

            [JsonProperty("bytes_total")]
            public long BytesTotal { get; set; }

            [JsonProperty("bytes_avail")]
            public long BytesAvail { get; set; }
        }

        public class PgNum
        {
            [JsonProperty("flags")]
            public int Flags { get; set; }

            [JsonProperty("tags")]
            public List<object> Tags { get; set; }

            [JsonProperty("see_also")]
            public List<object> SeeAlso { get; set; }

            [JsonProperty("default_value")]
            public string DefaultValue { get; set; }

            [JsonProperty("desc")]
            public string Desc { get; set; }

            [JsonProperty("long_desc")]
            public string LongDesc { get; set; }

            [JsonProperty("name")]
            public string Name { get; set; }

            [JsonProperty("type")]
            public string Type { get; set; }

            [JsonProperty("max")]
            public string Max { get; set; }

            [JsonProperty("min")]
            public string Min { get; set; }

            [JsonProperty("enum_allowed")]
            public List<object> EnumAllowed { get; set; }

            [JsonProperty("level")]
            public string Level { get; set; }
        }

        public class PgsByState
        {
            [JsonProperty("count")]
            public int Count { get; set; }

            [JsonProperty("state_name")]
            public string StateName { get; set; }
        }

        public class PoolIds
        {
            [JsonProperty("flags")]
            public int Flags { get; set; }

            [JsonProperty("default_value")]
            public string DefaultValue { get; set; }

            [JsonProperty("desc")]
            public string Desc { get; set; }

            [JsonProperty("tags")]
            public List<object> Tags { get; set; }

            [JsonProperty("see_also")]
            public List<object> SeeAlso { get; set; }

            [JsonProperty("name")]
            public string Name { get; set; }

            [JsonProperty("long_desc")]
            public string LongDesc { get; set; }

            [JsonProperty("enum_allowed")]
            public List<object> EnumAllowed { get; set; }

            [JsonProperty("level")]
            public string Level { get; set; }

            [JsonProperty("max")]
            public string Max { get; set; }

            [JsonProperty("min")]
            public string Min { get; set; }

            [JsonProperty("type")]
            public string Type { get; set; }
        }

        public class PoolName
        {
            [JsonProperty("flags")]
            public int Flags { get; set; }

            [JsonProperty("desc")]
            public string Desc { get; set; }

            [JsonProperty("default_value")]
            public string DefaultValue { get; set; }

            [JsonProperty("see_also")]
            public List<object> SeeAlso { get; set; }

            [JsonProperty("tags")]
            public List<object> Tags { get; set; }

            [JsonProperty("name")]
            public string Name { get; set; }

            [JsonProperty("long_desc")]
            public string LongDesc { get; set; }

            [JsonProperty("level")]
            public string Level { get; set; }

            [JsonProperty("enum_allowed")]
            public List<object> EnumAllowed { get; set; }

            [JsonProperty("max")]
            public string Max { get; set; }

            [JsonProperty("min")]
            public string Min { get; set; }

            [JsonProperty("type")]
            public string Type { get; set; }
        }

        public class Port
        {
            [JsonProperty("default_value")]
            public string DefaultValue { get; set; }

            [JsonProperty("desc")]
            public string Desc { get; set; }

            [JsonProperty("see_also")]
            public List<object> SeeAlso { get; set; }

            [JsonProperty("tags")]
            public List<object> Tags { get; set; }

            [JsonProperty("flags")]
            public int Flags { get; set; }

            [JsonProperty("enum_allowed")]
            public List<object> EnumAllowed { get; set; }

            [JsonProperty("level")]
            public string Level { get; set; }

            [JsonProperty("type")]
            public string Type { get; set; }

            [JsonProperty("max")]
            public string Max { get; set; }

            [JsonProperty("min")]
            public string Min { get; set; }

            [JsonProperty("name")]
            public string Name { get; set; }

            [JsonProperty("long_desc")]
            public string LongDesc { get; set; }
        }

        public class Prefix
        {
            [JsonProperty("name")]
            public string Name { get; set; }

            [JsonProperty("long_desc")]
            public string LongDesc { get; set; }

            [JsonProperty("level")]
            public string Level { get; set; }

            [JsonProperty("enum_allowed")]
            public List<object> EnumAllowed { get; set; }

            [JsonProperty("min")]
            public string Min { get; set; }

            [JsonProperty("max")]
            public string Max { get; set; }

            [JsonProperty("type")]
            public string Type { get; set; }

            [JsonProperty("flags")]
            public int Flags { get; set; }

            [JsonProperty("desc")]
            public string Desc { get; set; }

            [JsonProperty("default_value")]
            public string DefaultValue { get; set; }

            [JsonProperty("see_also")]
            public List<object> SeeAlso { get; set; }

            [JsonProperty("tags")]
            public List<object> Tags { get; set; }
        }

        public class ProgressEvents_
        {
        }

        public class Proxy
        {
            [JsonProperty("name")]
            public string Name { get; set; }

            [JsonProperty("long_desc")]
            public string LongDesc { get; set; }

            [JsonProperty("enum_allowed")]
            public List<object> EnumAllowed { get; set; }

            [JsonProperty("level")]
            public string Level { get; set; }

            [JsonProperty("max")]
            public string Max { get; set; }

            [JsonProperty("min")]
            public string Min { get; set; }

            [JsonProperty("type")]
            public string Type { get; set; }

            [JsonProperty("flags")]
            public int Flags { get; set; }

            [JsonProperty("desc")]
            public string Desc { get; set; }

            [JsonProperty("default_value")]
            public string DefaultValue { get; set; }

            [JsonProperty("tags")]
            public List<object> Tags { get; set; }

            [JsonProperty("see_also")]
            public List<object> SeeAlso { get; set; }
        }

        public class PublicAddrs
        {
            [JsonProperty("addrvec")]
            public List<Addrvec> Addrvec { get; set; }
        }

        public class RbdStatsPools
        {
            [JsonProperty("level")]
            public string Level { get; set; }

            [JsonProperty("enum_allowed")]
            public List<object> EnumAllowed { get; set; }

            [JsonProperty("min")]
            public string Min { get; set; }

            [JsonProperty("max")]
            public string Max { get; set; }

            [JsonProperty("type")]
            public string Type { get; set; }

            [JsonProperty("name")]
            public string Name { get; set; }

            [JsonProperty("long_desc")]
            public string LongDesc { get; set; }

            [JsonProperty("desc")]
            public string Desc { get; set; }

            [JsonProperty("default_value")]
            public string DefaultValue { get; set; }

            [JsonProperty("see_also")]
            public List<object> SeeAlso { get; set; }

            [JsonProperty("tags")]
            public List<object> Tags { get; set; }

            [JsonProperty("flags")]
            public int Flags { get; set; }
        }

        public class RbdStatsPoolsRefreshInterval
        {
            [JsonProperty("long_desc")]
            public string LongDesc { get; set; }

            [JsonProperty("name")]
            public string Name { get; set; }

            [JsonProperty("min")]
            public string Min { get; set; }

            [JsonProperty("max")]
            public string Max { get; set; }

            [JsonProperty("type")]
            public string Type { get; set; }

            [JsonProperty("level")]
            public string Level { get; set; }

            [JsonProperty("enum_allowed")]
            public List<object> EnumAllowed { get; set; }

            [JsonProperty("flags")]
            public int Flags { get; set; }

            [JsonProperty("tags")]
            public List<object> Tags { get; set; }

            [JsonProperty("see_also")]
            public List<object> SeeAlso { get; set; }

            [JsonProperty("default_value")]
            public string DefaultValue { get; set; }

            [JsonProperty("desc")]
            public string Desc { get; set; }
        }

        public class RetainInterval
        {
            [JsonProperty("min")]
            public string Min { get; set; }

            [JsonProperty("max")]
            public string Max { get; set; }

            [JsonProperty("type")]
            public string Type { get; set; }

            [JsonProperty("level")]
            public string Level { get; set; }

            [JsonProperty("enum_allowed")]
            public List<object> EnumAllowed { get; set; }

            [JsonProperty("long_desc")]
            public string LongDesc { get; set; }

            [JsonProperty("name")]
            public string Name { get; set; }

            [JsonProperty("see_also")]
            public List<object> SeeAlso { get; set; }

            [JsonProperty("tags")]
            public List<object> Tags { get; set; }

            [JsonProperty("desc")]
            public string Desc { get; set; }

            [JsonProperty("default_value")]
            public string DefaultValue { get; set; }

            [JsonProperty("flags")]
            public int Flags { get; set; }
        }

        public class RetentionPeriod
        {
            [JsonProperty("long_desc")]
            public string LongDesc { get; set; }

            [JsonProperty("name")]
            public string Name { get; set; }

            [JsonProperty("max")]
            public string Max { get; set; }

            [JsonProperty("type")]
            public string Type { get; set; }

            [JsonProperty("min")]
            public string Min { get; set; }

            [JsonProperty("level")]
            public string Level { get; set; }

            [JsonProperty("enum_allowed")]
            public List<object> EnumAllowed { get; set; }

            [JsonProperty("flags")]
            public int Flags { get; set; }

            [JsonProperty("tags")]
            public List<object> Tags { get; set; }

            [JsonProperty("see_also")]
            public List<object> SeeAlso { get; set; }

            [JsonProperty("desc")]
            public string Desc { get; set; }

            [JsonProperty("default_value")]
            public string DefaultValue { get; set; }
        }

        public class Roption1
        {
            [JsonProperty("long_desc")]
            public string LongDesc { get; set; }

            [JsonProperty("name")]
            public string Name { get; set; }

            [JsonProperty("min")]
            public string Min { get; set; }

            [JsonProperty("max")]
            public string Max { get; set; }

            [JsonProperty("type")]
            public string Type { get; set; }

            [JsonProperty("enum_allowed")]
            public List<object> EnumAllowed { get; set; }

            [JsonProperty("level")]
            public string Level { get; set; }

            [JsonProperty("flags")]
            public int Flags { get; set; }

            [JsonProperty("tags")]
            public List<object> Tags { get; set; }

            [JsonProperty("see_also")]
            public List<object> SeeAlso { get; set; }

            [JsonProperty("desc")]
            public string Desc { get; set; }

            [JsonProperty("default_value")]
            public string DefaultValue { get; set; }
        }

        public class Roption2
        {
            [JsonProperty("tags")]
            public List<object> Tags { get; set; }

            [JsonProperty("see_also")]
            public List<object> SeeAlso { get; set; }

            [JsonProperty("desc")]
            public string Desc { get; set; }

            [JsonProperty("default_value")]
            public string DefaultValue { get; set; }

            [JsonProperty("flags")]
            public int Flags { get; set; }

            [JsonProperty("type")]
            public string Type { get; set; }

            [JsonProperty("max")]
            public string Max { get; set; }

            [JsonProperty("min")]
            public string Min { get; set; }

            [JsonProperty("level")]
            public string Level { get; set; }

            [JsonProperty("enum_allowed")]
            public List<object> EnumAllowed { get; set; }

            [JsonProperty("long_desc")]
            public string LongDesc { get; set; }

            [JsonProperty("name")]
            public string Name { get; set; }
        }

        public class Rwoption1
        {
            [JsonProperty("flags")]
            public int Flags { get; set; }

            [JsonProperty("desc")]
            public string Desc { get; set; }

            [JsonProperty("default_value")]
            public string DefaultValue { get; set; }

            [JsonProperty("tags")]
            public List<object> Tags { get; set; }

            [JsonProperty("see_also")]
            public List<object> SeeAlso { get; set; }

            [JsonProperty("name")]
            public string Name { get; set; }

            [JsonProperty("long_desc")]
            public string LongDesc { get; set; }

            [JsonProperty("level")]
            public string Level { get; set; }

            [JsonProperty("enum_allowed")]
            public List<object> EnumAllowed { get; set; }

            [JsonProperty("min")]
            public string Min { get; set; }

            [JsonProperty("max")]
            public string Max { get; set; }

            [JsonProperty("type")]
            public string Type { get; set; }
        }

        public class Rwoption2
        {
            [JsonProperty("desc")]
            public string Desc { get; set; }

            [JsonProperty("default_value")]
            public string DefaultValue { get; set; }

            [JsonProperty("tags")]
            public List<object> Tags { get; set; }

            [JsonProperty("see_also")]
            public List<object> SeeAlso { get; set; }

            [JsonProperty("flags")]
            public int Flags { get; set; }

            [JsonProperty("level")]
            public string Level { get; set; }

            [JsonProperty("enum_allowed")]
            public List<object> EnumAllowed { get; set; }

            [JsonProperty("max")]
            public string Max { get; set; }

            [JsonProperty("min")]
            public string Min { get; set; }

            [JsonProperty("type")]
            public string Type { get; set; }

            [JsonProperty("name")]
            public string Name { get; set; }

            [JsonProperty("long_desc")]
            public string LongDesc { get; set; }
        }

        public class Rwoption3
        {
            [JsonProperty("type")]
            public string Type { get; set; }

            [JsonProperty("max")]
            public string Max { get; set; }

            [JsonProperty("min")]
            public string Min { get; set; }

            [JsonProperty("enum_allowed")]
            public List<object> EnumAllowed { get; set; }

            [JsonProperty("level")]
            public string Level { get; set; }

            [JsonProperty("long_desc")]
            public string LongDesc { get; set; }

            [JsonProperty("name")]
            public string Name { get; set; }

            [JsonProperty("tags")]
            public List<object> Tags { get; set; }

            [JsonProperty("see_also")]
            public List<object> SeeAlso { get; set; }

            [JsonProperty("desc")]
            public string Desc { get; set; }

            [JsonProperty("default_value")]
            public string DefaultValue { get; set; }

            [JsonProperty("flags")]
            public int Flags { get; set; }
        }

        public class Rwoption4
        {
            [JsonProperty("default_value")]
            public string DefaultValue { get; set; }

            [JsonProperty("desc")]
            public string Desc { get; set; }

            [JsonProperty("tags")]
            public List<object> Tags { get; set; }

            [JsonProperty("see_also")]
            public List<object> SeeAlso { get; set; }

            [JsonProperty("flags")]
            public int Flags { get; set; }

            [JsonProperty("enum_allowed")]
            public List<object> EnumAllowed { get; set; }

            [JsonProperty("level")]
            public string Level { get; set; }

            [JsonProperty("max")]
            public string Max { get; set; }

            [JsonProperty("min")]
            public string Min { get; set; }

            [JsonProperty("type")]
            public string Type { get; set; }

            [JsonProperty("name")]
            public string Name { get; set; }

            [JsonProperty("long_desc")]
            public string LongDesc { get; set; }
        }

        public class Rwoption5
        {
            [JsonProperty("long_desc")]
            public string LongDesc { get; set; }

            [JsonProperty("name")]
            public string Name { get; set; }

            [JsonProperty("type")]
            public string Type { get; set; }

            [JsonProperty("max")]
            public string Max { get; set; }

            [JsonProperty("min")]
            public string Min { get; set; }

            [JsonProperty("level")]
            public string Level { get; set; }

            [JsonProperty("enum_allowed")]
            public List<object> EnumAllowed { get; set; }

            [JsonProperty("flags")]
            public int Flags { get; set; }

            [JsonProperty("see_also")]
            public List<object> SeeAlso { get; set; }

            [JsonProperty("tags")]
            public List<object> Tags { get; set; }

            [JsonProperty("default_value")]
            public string DefaultValue { get; set; }

            [JsonProperty("desc")]
            public string Desc { get; set; }
        }

        public class Rwoption6
        {
            [JsonProperty("tags")]
            public List<object> Tags { get; set; }

            [JsonProperty("see_also")]
            public List<object> SeeAlso { get; set; }

            [JsonProperty("desc")]
            public string Desc { get; set; }

            [JsonProperty("default_value")]
            public string DefaultValue { get; set; }

            [JsonProperty("flags")]
            public int Flags { get; set; }

            [JsonProperty("max")]
            public string Max { get; set; }

            [JsonProperty("type")]
            public string Type { get; set; }

            [JsonProperty("min")]
            public string Min { get; set; }

            [JsonProperty("enum_allowed")]
            public List<object> EnumAllowed { get; set; }

            [JsonProperty("level")]
            public string Level { get; set; }

            [JsonProperty("long_desc")]
            public string LongDesc { get; set; }

            [JsonProperty("name")]
            public string Name { get; set; }
        }

        public class ScrapeFrequency
        {
            [JsonProperty("enum_allowed")]
            public List<object> EnumAllowed { get; set; }

            [JsonProperty("level")]
            public string Level { get; set; }

            [JsonProperty("min")]
            public string Min { get; set; }

            [JsonProperty("max")]
            public string Max { get; set; }

            [JsonProperty("type")]
            public string Type { get; set; }

            [JsonProperty("name")]
            public string Name { get; set; }

            [JsonProperty("long_desc")]
            public string LongDesc { get; set; }

            [JsonProperty("desc")]
            public string Desc { get; set; }

            [JsonProperty("default_value")]
            public string DefaultValue { get; set; }

            [JsonProperty("tags")]
            public List<object> Tags { get; set; }

            [JsonProperty("see_also")]
            public List<object> SeeAlso { get; set; }

            [JsonProperty("flags")]
            public int Flags { get; set; }
        }

        public class ScrapeInterval
        {
            [JsonProperty("default_value")]
            public string DefaultValue { get; set; }

            [JsonProperty("desc")]
            public string Desc { get; set; }

            [JsonProperty("see_also")]
            public List<object> SeeAlso { get; set; }

            [JsonProperty("tags")]
            public List<object> Tags { get; set; }

            [JsonProperty("flags")]
            public int Flags { get; set; }

            [JsonProperty("level")]
            public string Level { get; set; }

            [JsonProperty("enum_allowed")]
            public List<object> EnumAllowed { get; set; }

            [JsonProperty("max")]
            public string Max { get; set; }

            [JsonProperty("min")]
            public string Min { get; set; }

            [JsonProperty("type")]
            public string Type { get; set; }

            [JsonProperty("name")]
            public string Name { get; set; }

            [JsonProperty("long_desc")]
            public string LongDesc { get; set; }
        }

        public class SelfHeal
        {
            [JsonProperty("level")]
            public string Level { get; set; }

            [JsonProperty("enum_allowed")]
            public List<object> EnumAllowed { get; set; }

            [JsonProperty("max")]
            public string Max { get; set; }

            [JsonProperty("type")]
            public string Type { get; set; }

            [JsonProperty("min")]
            public string Min { get; set; }

            [JsonProperty("name")]
            public string Name { get; set; }

            [JsonProperty("long_desc")]
            public string LongDesc { get; set; }

            [JsonProperty("desc")]
            public string Desc { get; set; }

            [JsonProperty("default_value")]
            public string DefaultValue { get; set; }

            [JsonProperty("tags")]
            public List<object> Tags { get; set; }

            [JsonProperty("see_also")]
            public List<object> SeeAlso { get; set; }

            [JsonProperty("flags")]
            public int Flags { get; set; }
        }

        public class ServerAddr
        {
            [JsonProperty("flags")]
            public int Flags { get; set; }

            [JsonProperty("desc")]
            public string Desc { get; set; }

            [JsonProperty("default_value")]
            public string DefaultValue { get; set; }

            [JsonProperty("see_also")]
            public List<object> SeeAlso { get; set; }

            [JsonProperty("tags")]
            public List<object> Tags { get; set; }

            [JsonProperty("name")]
            public string Name { get; set; }

            [JsonProperty("long_desc")]
            public string LongDesc { get; set; }

            [JsonProperty("level")]
            public string Level { get; set; }

            [JsonProperty("enum_allowed")]
            public List<object> EnumAllowed { get; set; }

            [JsonProperty("max")]
            public string Max { get; set; }

            [JsonProperty("min")]
            public string Min { get; set; }

            [JsonProperty("type")]
            public string Type { get; set; }
        }

        public class ServerPort
        {
            [JsonProperty("name")]
            public string Name { get; set; }

            [JsonProperty("long_desc")]
            public string LongDesc { get; set; }

            [JsonProperty("level")]
            public string Level { get; set; }

            [JsonProperty("enum_allowed")]
            public List<object> EnumAllowed { get; set; }

            [JsonProperty("max")]
            public string Max { get; set; }

            [JsonProperty("min")]
            public string Min { get; set; }

            [JsonProperty("type")]
            public string Type { get; set; }

            [JsonProperty("flags")]
            public int Flags { get; set; }

            [JsonProperty("desc")]
            public string Desc { get; set; }

            [JsonProperty("default_value")]
            public string DefaultValue { get; set; }

            [JsonProperty("see_also")]
            public List<object> SeeAlso { get; set; }

            [JsonProperty("tags")]
            public List<object> Tags { get; set; }
        }

        public class Servicemap_
        {
            [JsonProperty("epoch")]
            public int Epoch { get; set; }

            [JsonProperty("services")]
            public Services Services { get; set; }

            [JsonProperty("modified")]
            public DateTime Modified { get; set; }
        }

        public class Services
        {
        }

        public class SleepInterval
        {
            [JsonProperty("flags")]
            public int Flags { get; set; }

            [JsonProperty("default_value")]
            public string DefaultValue { get; set; }

            [JsonProperty("desc")]
            public string Desc { get; set; }

            [JsonProperty("tags")]
            public List<object> Tags { get; set; }

            [JsonProperty("see_also")]
            public List<object> SeeAlso { get; set; }

            [JsonProperty("name")]
            public string Name { get; set; }

            [JsonProperty("long_desc")]
            public string LongDesc { get; set; }

            [JsonProperty("level")]
            public string Level { get; set; }

            [JsonProperty("enum_allowed")]
            public List<object> EnumAllowed { get; set; }

            [JsonProperty("max")]
            public string Max { get; set; }

            [JsonProperty("type")]
            public string Type { get; set; }

            [JsonProperty("min")]
            public string Min { get; set; }
        }

        public class SmtpDestination
        {
            [JsonProperty("long_desc")]
            public string LongDesc { get; set; }

            [JsonProperty("name")]
            public string Name { get; set; }

            [JsonProperty("min")]
            public string Min { get; set; }

            [JsonProperty("max")]
            public string Max { get; set; }

            [JsonProperty("type")]
            public string Type { get; set; }

            [JsonProperty("enum_allowed")]
            public List<object> EnumAllowed { get; set; }

            [JsonProperty("level")]
            public string Level { get; set; }

            [JsonProperty("flags")]
            public int Flags { get; set; }

            [JsonProperty("see_also")]
            public List<object> SeeAlso { get; set; }

            [JsonProperty("tags")]
            public List<object> Tags { get; set; }

            [JsonProperty("default_value")]
            public string DefaultValue { get; set; }

            [JsonProperty("desc")]
            public string Desc { get; set; }
        }

        public class SmtpFromName
        {
            [JsonProperty("flags")]
            public int Flags { get; set; }

            [JsonProperty("default_value")]
            public string DefaultValue { get; set; }

            [JsonProperty("desc")]
            public string Desc { get; set; }

            [JsonProperty("tags")]
            public List<object> Tags { get; set; }

            [JsonProperty("see_also")]
            public List<object> SeeAlso { get; set; }

            [JsonProperty("name")]
            public string Name { get; set; }

            [JsonProperty("long_desc")]
            public string LongDesc { get; set; }

            [JsonProperty("level")]
            public string Level { get; set; }

            [JsonProperty("enum_allowed")]
            public List<object> EnumAllowed { get; set; }

            [JsonProperty("type")]
            public string Type { get; set; }

            [JsonProperty("max")]
            public string Max { get; set; }

            [JsonProperty("min")]
            public string Min { get; set; }
        }

        public class SmtpHost
        {
            [JsonProperty("name")]
            public string Name { get; set; }

            [JsonProperty("long_desc")]
            public string LongDesc { get; set; }

            [JsonProperty("level")]
            public string Level { get; set; }

            [JsonProperty("enum_allowed")]
            public List<object> EnumAllowed { get; set; }

            [JsonProperty("type")]
            public string Type { get; set; }

            [JsonProperty("max")]
            public string Max { get; set; }

            [JsonProperty("min")]
            public string Min { get; set; }

            [JsonProperty("flags")]
            public int Flags { get; set; }

            [JsonProperty("desc")]
            public string Desc { get; set; }

            [JsonProperty("default_value")]
            public string DefaultValue { get; set; }

            [JsonProperty("see_also")]
            public List<object> SeeAlso { get; set; }

            [JsonProperty("tags")]
            public List<object> Tags { get; set; }
        }

        public class SmtpPassword
        {
            [JsonProperty("level")]
            public string Level { get; set; }

            [JsonProperty("enum_allowed")]
            public List<object> EnumAllowed { get; set; }

            [JsonProperty("max")]
            public string Max { get; set; }

            [JsonProperty("min")]
            public string Min { get; set; }

            [JsonProperty("type")]
            public string Type { get; set; }

            [JsonProperty("name")]
            public string Name { get; set; }

            [JsonProperty("long_desc")]
            public string LongDesc { get; set; }

            [JsonProperty("desc")]
            public string Desc { get; set; }

            [JsonProperty("default_value")]
            public string DefaultValue { get; set; }

            [JsonProperty("see_also")]
            public List<object> SeeAlso { get; set; }

            [JsonProperty("tags")]
            public List<object> Tags { get; set; }

            [JsonProperty("flags")]
            public int Flags { get; set; }
        }

        public class SmtpPort
        {
            [JsonProperty("default_value")]
            public string DefaultValue { get; set; }

            [JsonProperty("desc")]
            public string Desc { get; set; }

            [JsonProperty("see_also")]
            public List<object> SeeAlso { get; set; }

            [JsonProperty("tags")]
            public List<object> Tags { get; set; }

            [JsonProperty("flags")]
            public int Flags { get; set; }

            [JsonProperty("enum_allowed")]
            public List<object> EnumAllowed { get; set; }

            [JsonProperty("level")]
            public string Level { get; set; }

            [JsonProperty("type")]
            public string Type { get; set; }

            [JsonProperty("max")]
            public string Max { get; set; }

            [JsonProperty("min")]
            public string Min { get; set; }

            [JsonProperty("name")]
            public string Name { get; set; }

            [JsonProperty("long_desc")]
            public string LongDesc { get; set; }
        }

        public class SmtpSender
        {
            [JsonProperty("see_also")]
            public List<object> SeeAlso { get; set; }

            [JsonProperty("tags")]
            public List<object> Tags { get; set; }

            [JsonProperty("desc")]
            public string Desc { get; set; }

            [JsonProperty("default_value")]
            public string DefaultValue { get; set; }

            [JsonProperty("flags")]
            public int Flags { get; set; }

            [JsonProperty("max")]
            public string Max { get; set; }

            [JsonProperty("min")]
            public string Min { get; set; }

            [JsonProperty("type")]
            public string Type { get; set; }

            [JsonProperty("level")]
            public string Level { get; set; }

            [JsonProperty("enum_allowed")]
            public List<object> EnumAllowed { get; set; }

            [JsonProperty("long_desc")]
            public string LongDesc { get; set; }

            [JsonProperty("name")]
            public string Name { get; set; }
        }

        public class SmtpSsl
        {
            [JsonProperty("type")]
            public string Type { get; set; }

            [JsonProperty("max")]
            public string Max { get; set; }

            [JsonProperty("min")]
            public string Min { get; set; }

            [JsonProperty("level")]
            public string Level { get; set; }

            [JsonProperty("enum_allowed")]
            public List<object> EnumAllowed { get; set; }

            [JsonProperty("long_desc")]
            public string LongDesc { get; set; }

            [JsonProperty("name")]
            public string Name { get; set; }

            [JsonProperty("tags")]
            public List<object> Tags { get; set; }

            [JsonProperty("see_also")]
            public List<object> SeeAlso { get; set; }

            [JsonProperty("desc")]
            public string Desc { get; set; }

            [JsonProperty("default_value")]
            public string DefaultValue { get; set; }

            [JsonProperty("flags")]
            public int Flags { get; set; }
        }

        public class SmtpUser
        {
            [JsonProperty("long_desc")]
            public string LongDesc { get; set; }

            [JsonProperty("name")]
            public string Name { get; set; }

            [JsonProperty("max")]
            public string Max { get; set; }

            [JsonProperty("min")]
            public string Min { get; set; }

            [JsonProperty("type")]
            public string Type { get; set; }

            [JsonProperty("level")]
            public string Level { get; set; }

            [JsonProperty("enum_allowed")]
            public List<object> EnumAllowed { get; set; }

            [JsonProperty("flags")]
            public int Flags { get; set; }

            [JsonProperty("tags")]
            public List<object> Tags { get; set; }

            [JsonProperty("see_also")]
            public List<object> SeeAlso { get; set; }

            [JsonProperty("default_value")]
            public string DefaultValue { get; set; }

            [JsonProperty("desc")]
            public string Desc { get; set; }
        }

        public class SnapshotCloneDelay
        {
            [JsonProperty("min")]
            public string Min { get; set; }

            [JsonProperty("max")]
            public string Max { get; set; }

            [JsonProperty("type")]
            public string Type { get; set; }

            [JsonProperty("enum_allowed")]
            public List<object> EnumAllowed { get; set; }

            [JsonProperty("level")]
            public string Level { get; set; }

            [JsonProperty("long_desc")]
            public string LongDesc { get; set; }

            [JsonProperty("name")]
            public string Name { get; set; }

            [JsonProperty("see_also")]
            public List<object> SeeAlso { get; set; }

            [JsonProperty("tags")]
            public List<object> Tags { get; set; }

            [JsonProperty("desc")]
            public string Desc { get; set; }

            [JsonProperty("default_value")]
            public string DefaultValue { get; set; }

            [JsonProperty("flags")]
            public int Flags { get; set; }
        }

        public class Ssl
        {
            [JsonProperty("flags")]
            public int Flags { get; set; }

            [JsonProperty("see_also")]
            public List<object> SeeAlso { get; set; }

            [JsonProperty("tags")]
            public List<object> Tags { get; set; }

            [JsonProperty("default_value")]
            public string DefaultValue { get; set; }

            [JsonProperty("desc")]
            public string Desc { get; set; }

            [JsonProperty("long_desc")]
            public string LongDesc { get; set; }

            [JsonProperty("name")]
            public string Name { get; set; }

            [JsonProperty("max")]
            public string Max { get; set; }

            [JsonProperty("min")]
            public string Min { get; set; }

            [JsonProperty("type")]
            public string Type { get; set; }

            [JsonProperty("level")]
            public string Level { get; set; }

            [JsonProperty("enum_allowed")]
            public List<object> EnumAllowed { get; set; }
        }

        public class StaleCacheStrategy
        {
            [JsonProperty("default_value")]
            public string DefaultValue { get; set; }

            [JsonProperty("desc")]
            public string Desc { get; set; }

            [JsonProperty("tags")]
            public List<object> Tags { get; set; }

            [JsonProperty("see_also")]
            public List<object> SeeAlso { get; set; }

            [JsonProperty("flags")]
            public int Flags { get; set; }

            [JsonProperty("enum_allowed")]
            public List<object> EnumAllowed { get; set; }

            [JsonProperty("level")]
            public string Level { get; set; }

            [JsonProperty("max")]
            public string Max { get; set; }

            [JsonProperty("type")]
            public string Type { get; set; }

            [JsonProperty("min")]
            public string Min { get; set; }

            [JsonProperty("name")]
            public string Name { get; set; }

            [JsonProperty("long_desc")]
            public string LongDesc { get; set; }
        }

        public class Standby
        {
            [JsonProperty("mgr_features")]
            public object MgrFeatures { get; set; }

            [JsonProperty("gid")]
            public int Gid { get; set; }

            [JsonProperty("name")]
            public string Name { get; set; }

            [JsonProperty("available_modules")]
            public List<AvailableModule> AvailableModules { get; set; }
        }

        public class StandbyBehaviour
        {
            [JsonProperty("name")]
            public string Name { get; set; }

            [JsonProperty("long_desc")]
            public string LongDesc { get; set; }

            [JsonProperty("enum_allowed")]
            public List<string> EnumAllowed { get; set; }

            [JsonProperty("level")]
            public string Level { get; set; }

            [JsonProperty("max")]
            public string Max { get; set; }

            [JsonProperty("type")]
            public string Type { get; set; }

            [JsonProperty("min")]
            public string Min { get; set; }

            [JsonProperty("flags")]
            public int Flags { get; set; }

            [JsonProperty("desc")]
            public string Desc { get; set; }

            [JsonProperty("default_value")]
            public string DefaultValue { get; set; }

            [JsonProperty("tags")]
            public List<object> Tags { get; set; }

            [JsonProperty("see_also")]
            public List<object> SeeAlso { get; set; }
        }

        public class StandbyErrorStatusCode
        {
            [JsonProperty("type")]
            public string Type { get; set; }

            [JsonProperty("max")]
            public string Max { get; set; }

            [JsonProperty("min")]
            public string Min { get; set; }

            [JsonProperty("level")]
            public string Level { get; set; }

            [JsonProperty("enum_allowed")]
            public List<object> EnumAllowed { get; set; }

            [JsonProperty("long_desc")]
            public string LongDesc { get; set; }

            [JsonProperty("name")]
            public string Name { get; set; }

            [JsonProperty("see_also")]
            public List<object> SeeAlso { get; set; }

            [JsonProperty("tags")]
            public List<object> Tags { get; set; }

            [JsonProperty("desc")]
            public string Desc { get; set; }

            [JsonProperty("default_value")]
            public string DefaultValue { get; set; }

            [JsonProperty("flags")]
            public int Flags { get; set; }
        }

        public class Subtree
        {
            [JsonProperty("level")]
            public string Level { get; set; }

            [JsonProperty("enum_allowed")]
            public List<object> EnumAllowed { get; set; }

            [JsonProperty("type")]
            public string Type { get; set; }

            [JsonProperty("max")]
            public string Max { get; set; }

            [JsonProperty("min")]
            public string Min { get; set; }

            [JsonProperty("name")]
            public string Name { get; set; }

            [JsonProperty("long_desc")]
            public string LongDesc { get; set; }

            [JsonProperty("desc")]
            public string Desc { get; set; }

            [JsonProperty("default_value")]
            public string DefaultValue { get; set; }

            [JsonProperty("tags")]
            public List<object> Tags { get; set; }

            [JsonProperty("see_also")]
            public List<object> SeeAlso { get; set; }

            [JsonProperty("flags")]
            public int Flags { get; set; }
        }

        public class Testkey
        {
            [JsonProperty("default_value")]
            public string DefaultValue { get; set; }

            [JsonProperty("desc")]
            public string Desc { get; set; }

            [JsonProperty("tags")]
            public List<object> Tags { get; set; }

            [JsonProperty("see_also")]
            public List<object> SeeAlso { get; set; }

            [JsonProperty("flags")]
            public int Flags { get; set; }

            [JsonProperty("level")]
            public string Level { get; set; }

            [JsonProperty("enum_allowed")]
            public List<object> EnumAllowed { get; set; }

            [JsonProperty("max")]
            public string Max { get; set; }

            [JsonProperty("min")]
            public string Min { get; set; }

            [JsonProperty("type")]
            public string Type { get; set; }

            [JsonProperty("name")]
            public string Name { get; set; }

            [JsonProperty("long_desc")]
            public string LongDesc { get; set; }
        }

        public class Testlkey
        {
            [JsonProperty("desc")]
            public string Desc { get; set; }

            [JsonProperty("default_value")]
            public string DefaultValue { get; set; }

            [JsonProperty("tags")]
            public List<object> Tags { get; set; }

            [JsonProperty("see_also")]
            public List<object> SeeAlso { get; set; }

            [JsonProperty("flags")]
            public int Flags { get; set; }

            [JsonProperty("enum_allowed")]
            public List<object> EnumAllowed { get; set; }

            [JsonProperty("level")]
            public string Level { get; set; }

            [JsonProperty("type")]
            public string Type { get; set; }

            [JsonProperty("max")]
            public string Max { get; set; }

            [JsonProperty("min")]
            public string Min { get; set; }

            [JsonProperty("name")]
            public string Name { get; set; }

            [JsonProperty("long_desc")]
            public string LongDesc { get; set; }
        }

        public class Testnewline
        {
            [JsonProperty("min")]
            public string Min { get; set; }

            [JsonProperty("max")]
            public string Max { get; set; }

            [JsonProperty("type")]
            public string Type { get; set; }

            [JsonProperty("enum_allowed")]
            public List<object> EnumAllowed { get; set; }

            [JsonProperty("level")]
            public string Level { get; set; }

            [JsonProperty("long_desc")]
            public string LongDesc { get; set; }

            [JsonProperty("name")]
            public string Name { get; set; }

            [JsonProperty("tags")]
            public List<object> Tags { get; set; }

            [JsonProperty("see_also")]
            public List<object> SeeAlso { get; set; }

            [JsonProperty("desc")]
            public string Desc { get; set; }

            [JsonProperty("default_value")]
            public string DefaultValue { get; set; }

            [JsonProperty("flags")]
            public int Flags { get; set; }
        }

        public class Threads
        {
            [JsonProperty("tags")]
            public List<object> Tags { get; set; }

            [JsonProperty("see_also")]
            public List<object> SeeAlso { get; set; }

            [JsonProperty("desc")]
            public string Desc { get; set; }

            [JsonProperty("default_value")]
            public string DefaultValue { get; set; }

            [JsonProperty("flags")]
            public int Flags { get; set; }

            [JsonProperty("min")]
            public string Min { get; set; }

            [JsonProperty("max")]
            public string Max { get; set; }

            [JsonProperty("type")]
            public string Type { get; set; }

            [JsonProperty("enum_allowed")]
            public List<object> EnumAllowed { get; set; }

            [JsonProperty("level")]
            public string Level { get; set; }

            [JsonProperty("long_desc")]
            public string LongDesc { get; set; }

            [JsonProperty("name")]
            public string Name { get; set; }
        }

        public class TrashPurgeSchedule
        {
            [JsonProperty("see_also")]
            public List<object> SeeAlso { get; set; }

            [JsonProperty("tags")]
            public List<object> Tags { get; set; }

            [JsonProperty("desc")]
            public string Desc { get; set; }

            [JsonProperty("default_value")]
            public string DefaultValue { get; set; }

            [JsonProperty("flags")]
            public int Flags { get; set; }

            [JsonProperty("max")]
            public string Max { get; set; }

            [JsonProperty("min")]
            public string Min { get; set; }

            [JsonProperty("type")]
            public string Type { get; set; }

            [JsonProperty("level")]
            public string Level { get; set; }

            [JsonProperty("enum_allowed")]
            public List<object> EnumAllowed { get; set; }

            [JsonProperty("long_desc")]
            public string LongDesc { get; set; }

            [JsonProperty("name")]
            public string Name { get; set; }
        }

        public class UpmapMaxDeviation
        {
            [JsonProperty("name")]
            public string Name { get; set; }

            [JsonProperty("long_desc")]
            public string LongDesc { get; set; }

            [JsonProperty("enum_allowed")]
            public List<object> EnumAllowed { get; set; }

            [JsonProperty("level")]
            public string Level { get; set; }

            [JsonProperty("type")]
            public string Type { get; set; }

            [JsonProperty("max")]
            public string Max { get; set; }

            [JsonProperty("min")]
            public string Min { get; set; }

            [JsonProperty("flags")]
            public int Flags { get; set; }

            [JsonProperty("default_value")]
            public string DefaultValue { get; set; }

            [JsonProperty("desc")]
            public string Desc { get; set; }

            [JsonProperty("tags")]
            public List<object> Tags { get; set; }

            [JsonProperty("see_also")]
            public List<object> SeeAlso { get; set; }
        }

        public class UpmapMaxOptimizations
        {
            [JsonProperty("tags")]
            public List<object> Tags { get; set; }

            [JsonProperty("see_also")]
            public List<object> SeeAlso { get; set; }

            [JsonProperty("desc")]
            public string Desc { get; set; }

            [JsonProperty("default_value")]
            public string DefaultValue { get; set; }

            [JsonProperty("flags")]
            public int Flags { get; set; }

            [JsonProperty("type")]
            public string Type { get; set; }

            [JsonProperty("max")]
            public string Max { get; set; }

            [JsonProperty("min")]
            public string Min { get; set; }

            [JsonProperty("level")]
            public string Level { get; set; }

            [JsonProperty("enum_allowed")]
            public List<object> EnumAllowed { get; set; }

            [JsonProperty("long_desc")]
            public string LongDesc { get; set; }

            [JsonProperty("name")]
            public string Name { get; set; }
        }

        public class Url
        {
            [JsonProperty("flags")]
            public int Flags { get; set; }

            [JsonProperty("see_also")]
            public List<object> SeeAlso { get; set; }

            [JsonProperty("tags")]
            public List<object> Tags { get; set; }

            [JsonProperty("desc")]
            public string Desc { get; set; }

            [JsonProperty("default_value")]
            public string DefaultValue { get; set; }

            [JsonProperty("long_desc")]
            public string LongDesc { get; set; }

            [JsonProperty("name")]
            public string Name { get; set; }

            [JsonProperty("max")]
            public string Max { get; set; }

            [JsonProperty("min")]
            public string Min { get; set; }

            [JsonProperty("type")]
            public string Type { get; set; }

            [JsonProperty("level")]
            public string Level { get; set; }

            [JsonProperty("enum_allowed")]
            public List<object> EnumAllowed { get; set; }
        }

        public class Username
        {
            [JsonProperty("flags")]
            public int Flags { get; set; }

            [JsonProperty("default_value")]
            public string DefaultValue { get; set; }

            [JsonProperty("desc")]
            public string Desc { get; set; }

            [JsonProperty("tags")]
            public List<object> Tags { get; set; }

            [JsonProperty("see_also")]
            public List<object> SeeAlso { get; set; }

            [JsonProperty("name")]
            public string Name { get; set; }

            [JsonProperty("long_desc")]
            public string LongDesc { get; set; }

            [JsonProperty("enum_allowed")]
            public List<object> EnumAllowed { get; set; }

            [JsonProperty("level")]
            public string Level { get; set; }

            [JsonProperty("type")]
            public string Type { get; set; }

            [JsonProperty("max")]
            public string Max { get; set; }

            [JsonProperty("min")]
            public string Min { get; set; }
        }

        public class VerifySsl
        {
            [JsonProperty("name")]
            public string Name { get; set; }

            [JsonProperty("long_desc")]
            public string LongDesc { get; set; }

            [JsonProperty("enum_allowed")]
            public List<object> EnumAllowed { get; set; }

            [JsonProperty("level")]
            public string Level { get; set; }

            [JsonProperty("min")]
            public string Min { get; set; }

            [JsonProperty("max")]
            public string Max { get; set; }

            [JsonProperty("type")]
            public string Type { get; set; }

            [JsonProperty("flags")]
            public int Flags { get; set; }

            [JsonProperty("desc")]
            public string Desc { get; set; }

            [JsonProperty("default_value")]
            public string DefaultValue { get; set; }

            [JsonProperty("tags")]
            public List<object> Tags { get; set; }

            [JsonProperty("see_also")]
            public List<object> SeeAlso { get; set; }
        }

        public class WarnRecentInterval
        {
            [JsonProperty("flags")]
            public int Flags { get; set; }

            [JsonProperty("see_also")]
            public List<object> SeeAlso { get; set; }

            [JsonProperty("tags")]
            public List<object> Tags { get; set; }

            [JsonProperty("default_value")]
            public string DefaultValue { get; set; }

            [JsonProperty("desc")]
            public string Desc { get; set; }

            [JsonProperty("long_desc")]
            public string LongDesc { get; set; }

            [JsonProperty("name")]
            public string Name { get; set; }

            [JsonProperty("max")]
            public string Max { get; set; }

            [JsonProperty("type")]
            public string Type { get; set; }

            [JsonProperty("min")]
            public string Min { get; set; }

            [JsonProperty("level")]
            public string Level { get; set; }

            [JsonProperty("enum_allowed")]
            public List<object> EnumAllowed { get; set; }
        }

        public class WarnThreshold
        {
            [JsonProperty("flags")]
            public int Flags { get; set; }

            [JsonProperty("tags")]
            public List<object> Tags { get; set; }

            [JsonProperty("see_also")]
            public List<object> SeeAlso { get; set; }

            [JsonProperty("desc")]
            public string Desc { get; set; }

            [JsonProperty("default_value")]
            public string DefaultValue { get; set; }

            [JsonProperty("long_desc")]
            public string LongDesc { get; set; }

            [JsonProperty("name")]
            public string Name { get; set; }

            [JsonProperty("max")]
            public string Max { get; set; }

            [JsonProperty("min")]
            public string Min { get; set; }

            [JsonProperty("type")]
            public string Type { get; set; }

            [JsonProperty("enum_allowed")]
            public List<object> EnumAllowed { get; set; }

            [JsonProperty("level")]
            public string Level { get; set; }
        }

        public class ZabbixHost
        {
            [JsonProperty("max")]
            public string Max { get; set; }

            [JsonProperty("type")]
            public string Type { get; set; }

            [JsonProperty("min")]
            public string Min { get; set; }

            [JsonProperty("enum_allowed")]
            public List<object> EnumAllowed { get; set; }

            [JsonProperty("level")]
            public string Level { get; set; }

            [JsonProperty("long_desc")]
            public string LongDesc { get; set; }

            [JsonProperty("name")]
            public string Name { get; set; }

            [JsonProperty("see_also")]
            public List<object> SeeAlso { get; set; }

            [JsonProperty("tags")]
            public List<object> Tags { get; set; }

            [JsonProperty("desc")]
            public string Desc { get; set; }

            [JsonProperty("default_value")]
            public string DefaultValue { get; set; }

            [JsonProperty("flags")]
            public int Flags { get; set; }
        }

        public class ZabbixPort
        {
            [JsonProperty("min")]
            public string Min { get; set; }

            [JsonProperty("max")]
            public string Max { get; set; }

            [JsonProperty("type")]
            public string Type { get; set; }

            [JsonProperty("enum_allowed")]
            public List<object> EnumAllowed { get; set; }

            [JsonProperty("level")]
            public string Level { get; set; }

            [JsonProperty("long_desc")]
            public string LongDesc { get; set; }

            [JsonProperty("name")]
            public string Name { get; set; }

            [JsonProperty("see_also")]
            public List<object> SeeAlso { get; set; }

            [JsonProperty("tags")]
            public List<object> Tags { get; set; }

            [JsonProperty("default_value")]
            public string DefaultValue { get; set; }

            [JsonProperty("desc")]
            public string Desc { get; set; }

            [JsonProperty("flags")]
            public int Flags { get; set; }
        }

        public class ZabbixSender
        {
            [JsonProperty("min")]
            public string Min { get; set; }

            [JsonProperty("max")]
            public string Max { get; set; }

            [JsonProperty("type")]
            public string Type { get; set; }

            [JsonProperty("enum_allowed")]
            public List<object> EnumAllowed { get; set; }

            [JsonProperty("level")]
            public string Level { get; set; }

            [JsonProperty("long_desc")]
            public string LongDesc { get; set; }

            [JsonProperty("name")]
            public string Name { get; set; }

            [JsonProperty("tags")]
            public List<object> Tags { get; set; }

            [JsonProperty("see_also")]
            public List<object> SeeAlso { get; set; }

            [JsonProperty("desc")]
            public string Desc { get; set; }

            [JsonProperty("default_value")]
            public string DefaultValue { get; set; }

            [JsonProperty("flags")]
            public int Flags { get; set; }
        }
    }
}