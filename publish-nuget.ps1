# SPDX-FileCopyrightText: Copyright Corsinvest Srl
# SPDX-License-Identifier: AGPL-3.0-only

.\setKey.ps1

#Read project version
$xml = [xml](Get-Content .\common.props)
$version = $xml.Project.PropertyGroup.Version
Write-Host "Project version: $version"

dotnet publish -c Release

dotnet nuget push ..\.nupkgs\Corsinvest.ProxmoxVE.Api.$version.nupkg --api-key $ENV:nugetapikey --source https://api.nuget.org/v3/index.json
dotnet nuget push ..\.nupkgs\Corsinvest.ProxmoxVE.Api.Extension.$version.nupkg --api-key $ENV:nugetapikey --source https://api.nuget.org/v3/index.json
dotnet nuget push ..\.nupkgs\Corsinvest.ProxmoxVE.Api.Metadata.$version.nupkg --api-key $ENV:nugetapikey --source https://api.nuget.org/v3/index.json
dotnet nuget push ..\.nupkgs\Corsinvest.ProxmoxVE.Api.Shared.$version.nupkg --api-key $ENV:nugetapikey --source https://api.nuget.org/v3/index.json
dotnet nuget push ..\.nupkgs\Corsinvest.ProxmoxVE.Api.Shell.$version.nupkg --api-key $ENV:nugetapikey --source https://api.nuget.org/v3/index.json
