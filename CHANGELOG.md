# Changelog

All notable changes to this project will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.0.0/).
The major and minor version follow Proxmox VE (9.2.x targets Proxmox VE 9.2); the patch number can include breaking changes, listed under "Changed (breaking)".

## [9.2.4] - 2026-09-30

### Added
- Extension: new namespace `Corsinvest.ProxmoxVE.Api.Extension.Shell`, with API calls, aliases and the API schema as data and the formatting left to the caller ([#104](https://github.com/Corsinvest/cv4pve-api-dotnet/pull/104))
  - `ApiRequest.ExecuteAsync` returns `ApiResponse` (status, error, rejected parameters, data, raw answer, task id); `--wait` support with `ApiWaitOptions` and exit status
  - `ApiCommandLine`: `--key value` parameters, alias expansion with placeholders, `--guest` lookup, `--yes` confirmation; a value placed in the path cannot change it
  - `ApiSchema`: methods, parameters, allowed values and children of a path (indexed values sorted as numbers); `ToTable` turns an answer into a table as `pvesh` shows it, with `ApiTableOptions` (`AllColumns`, `HumanReadable`)
  - `ApiSchemaText`: usage and `ls` text
  - `CancellationToken` on `ExecuteAsync`, `ExpandAliasAsync` and `GetChildrenAsync`
- Shared: SPICE helpers on `VmConfigQemu`: `IsSpiceDisplay`, `SpiceMonitors`, `HasSpiceAudio`, `HasSpiceUsb`, `HasSpiceFolderSharing`; they read `vga`, `audio0`, `usbN` and `spice_enhancements` as Proxmox VE does ([#99](https://github.com/Corsinvest/cv4pve-api-dotnet/pull/99))
- Api: Ceph health mute (`HealthMute`, `HealthMuteIndex`) ([#102](https://github.com/Corsinvest/cv4pve-api-dotnet/pull/102))

### Changed (breaking)
- Shared: `TableGenerator` is a table object: `TableGenerator.From(items).Column(key or expression).Title/Format/Align/When`, or `new TableGenerator(columns).AddRow(...)`. Numbers aligned right, Html and Markdown escaped, cells on more lines. The old static members are removed ([#104](https://github.com/Corsinvest/cv4pve-api-dotnet/pull/104))
- Api: path parameters no longer repeated as method arguments: drop `route_map_id` from `GetRouteMapEntry`, `DeleteRouteMapEntry`, `UpdateRouteMapEntry`, `ListRouteMapEntriesForRouteMap` and `pci_id_or_mapping` from `PciIndex`, `Mdevscan`. The value comes from the indexer ([#102](https://github.com/Corsinvest/cv4pve-api-dotnet/pull/102))
- Api: `asn`, `level`, `seq` are `long?` instead of `int?` (up to 4294967295): recompile ([#102](https://github.com/Corsinvest/cv4pve-api-dotnet/pull/102))
- Metadata: the schema cache keeps every field of the schema (renderer, verbose description, formats, nested items, method name, links, path parameters) and has a format version; a cache of another version is not loaded ([#104](https://github.com/Corsinvest/cv4pve-api-dotnet/pull/104))

### Changed
- Extension: `ApiExplorerHelper` is obsolete and runs on the Shell classes ([#104](https://github.com/Corsinvest/cv4pve-api-dotnet/pull/104))
- Extension: schema values rendered again as `pvesh` does (sizes, percentages, durations, dates) ([#104](https://github.com/Corsinvest/cv4pve-api-dotnet/pull/104))
- Api: `/cluster/ha/rules` `CreateRule` and `UpdateRule` have their parameters again (`rule`, `type`, `resources`, `affinity`, `nodes`, `strict`, `comment`, `disable`) ([#102](https://github.com/Corsinvest/cv4pve-api-dotnet/pull/102))
- VM/CT selection logic moved to `VmHelper.GetVmsFromJollyAsync`, without API calls and covered by tests; `GetVmsAsync(client, jolly)` keeps its signature ([#100](https://github.com/Corsinvest/cv4pve-api-dotnet/pull/100))
- `Directory.Packages.props` renamed to `Directory.Build.props`; Source Link, portable symbols (`.snupkg`), deterministic CI builds, code style enforced in build ([#98](https://github.com/Corsinvest/cv4pve-api-dotnet/pull/98))
- Package tags fixed (`Api`, `Client`), Metadata product name fixed, `LICENSE` renamed to `LICENSE.md` and no longer packed (SPDX expression) ([#98](https://github.com/Corsinvest/cv4pve-api-dotnet/pull/98))
- Package icon (Lucide `braces`), same style as the other cv4pve tools ([#98](https://github.com/Corsinvest/cv4pve-api-dotnet/pull/98))
- CI: workflow permissions and job timeouts declared ([#97](https://github.com/Corsinvest/cv4pve-api-dotnet/pull/97))

### Fixed
- Login with a second factor never worked on Proxmox VE 7+ (TOTP, WebAuthn, recovery keys): the answer to the challenge is now sent in a second call with `tfa-challenge`; a code without a type is sent as `totp:<code>`. `tfa-challenge` is masked in the debug log ([#103](https://github.com/Corsinvest/cv4pve-api-dotnet/pull/103))
- `WaitForTaskToFinishAsync` returns true when the task is finished, also when the last check came after the timeout ([#103](https://github.com/Corsinvest/cv4pve-api-dotnet/pull/103))
- `PveClientBase`: an answer that is not JSON gives a failed result instead of an exception ([#104](https://github.com/Corsinvest/cv4pve-api-dotnet/pull/104))
- Shell: a 2xx answer without `data` is not a success ([#104](https://github.com/Corsinvest/cv4pve-api-dotnet/pull/104))
- VM/CT selection: an excluded range (`-150:200`) no longer adds guests up to 200 ([#100](https://github.com/Corsinvest/cv4pve-api-dotnet/pull/100))
- VM/CT selection: `@pool-` returns the guests of the cluster list, so exclusions and duplicates work with pools (`@all,-@pool-x`, `@pool-x,-100`) ([#100](https://github.com/Corsinvest/cv4pve-api-dotnet/pull/100))
- VM/CT selection: `text%` matches names starting with the text and `%text` names ending with it; they were inverted ([#100](https://github.com/Corsinvest/cv4pve-api-dotnet/pull/100))
- Metadata: parameters described with `allOf`/`oneOf` are read (POST/PUT `/cluster/ha/rules` lost them in every generator); a parameter required in only some variants becomes optional ([#101](https://github.com/Corsinvest/cv4pve-api-dotnet/pull/101))
- The VNC WebSocket bridge accepted any certificate while sending the session cookie: it now follows `PveClient.ValidateCertificate` ([#99](https://github.com/Corsinvest/cv4pve-api-dotnet/pull/99))
- SPICE: removed the proxy rewrite for `http://` proxies, which Proxmox VE rejects and which logged the whole `.vv` file, SPICE password included ([#99](https://github.com/Corsinvest/cv4pve-api-dotnet/pull/99))

## [9.2.3] and earlier

See [GitHub releases](https://github.com/Corsinvest/cv4pve-api-dotnet/releases).
