// @ts-check
import { defineConfig } from 'astro/config';
import starlight from '@astrojs/starlight';
import corsinvestTheme from '@corsinvest/cv4pve-docs-theme';

export default defineConfig({
  site: 'https://corsinvest.github.io',
  base: '/cv4pve-api-dotnet',
  integrations: [
    starlight({
      title: 'cv4pve-api-dotnet',
      description: 'Proxmox VE API client for .NET: the whole API as typed C# calls, with helpers for VMs, containers, tasks and command line tools.',
      // Brand, logo, GitHub and "Edit page" links, the Corsinvest sidebar group and
      // external links in a new tab come from the shared cv4pve theme.
      plugins: [
        corsinvestTheme({
          repo: 'cv4pve-api-dotnet',
          // Product icon: favicon and header, dark variant for the dark theme.
          icon: { light: '/icon.svg', dark: '/icon-dark.svg' },
          // Install-and-run panel in the home hero. A library, not a release binary: one custom target.
          install: {
            targets: [
              {
                id: 'dotnet',
                label: '.NET',
                lines: [
                  '# add the client to your project',
                  'dotnet add package Corsinvest.ProxmoxVE.Api',
                  '',
                  '# helpers for VMs, containers and tasks',
                  'dotnet add package Corsinvest.ProxmoxVE.Api.Extension',
                ],
              },
            ],
          },
        }),
      ],
      lastUpdated: true,
      sidebar: [
        {
          label: 'Start here',
          items: ['getting-started', 'connection', 'permissions', 'troubleshooting'],
        },
      ],
    }),
  ],
});
