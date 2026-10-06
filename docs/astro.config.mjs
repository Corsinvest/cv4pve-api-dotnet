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
      // Brand, product icon, GitHub link, the Corsinvest sidebar group and
      // external links in a new tab come from the shared cv4pve theme.
      plugins: [
        corsinvestTheme({
          repo: 'cv4pve-api-dotnet',
          // Product icon: favicon and header, dark variant for the dark theme.
          icon: { light: '/icon.svg', dark: '/icon-dark.svg' },
          // The readers of a library are developers: the motto says "By developers, for developers."
          audience: 'developers',
          // Steps panel in the home hero: the same steps, in the same order and words, as Getting started.
          // A library, not a release binary: the commands to add it are on the home page and in Getting started.
          steps: {
            items: [
              'Add the package',
              'Create the client',
              'Make the first call',
              'Read some data',
            ],
          },
        }),
      ],
      sidebar: [
        {
          label: 'Start here',
          items: ['getting-started', 'connection', 'permissions', 'troubleshooting'],
        },
        {
          label: 'Concepts',
          items: ['concepts/api-structure', 'concepts/results', 'concepts/indexed-parameters', 'concepts/tasks', 'concepts/errors'],
        },
        {
          label: 'Packages',
          items: ['packages/api', 'packages/extension', 'packages/shared', 'packages/console', 'packages/metadata'],
        },
        {
          label: 'Examples',
          items: ['examples/common-tasks', 'examples/create-vm', 'examples/bulk-operations'],
        },
      ],
    }),
  ],
});
