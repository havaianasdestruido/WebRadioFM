// @ts-check

import {themes as prismThemes} from 'prism-react-renderer';

const repositoryUrl = 'https://github.com/havaianasdestruido/WebRadioFM';

/** @type {import('@docusaurus/types').Config} */
const config = {
  title: 'WebRadioFM',
  tagline: 'Local music. Last.fm history. One tiny Windows Phone app.',
  favicon: 'img/favicon.svg',

  url: 'https://havaianasdestruido.github.io',
  baseUrl: process.env.DOCUSAURUS_BASE_URL || '/WebRadioFM/',
  organizationName: 'havaianasdestruido',
  projectName: 'WebRadioFM',
  deploymentBranch: 'gh-pages',
  trailingSlash: false,
  onBrokenLinks: 'throw',

  i18n: {
    defaultLocale: 'en',
    locales: ['en'],
  },

  markdown: {
    mermaid: true,
  },
  themes: ['@docusaurus/theme-mermaid'],

  presets: [
    [
      'classic',
      /** @type {import('@docusaurus/preset-classic').Options} */
      ({
        docs: {
          routeBasePath: 'docs',
          sidebarPath: './sidebars.js',
          editUrl: `${repositoryUrl}/edit/main/`,
          showLastUpdateAuthor: true,
          showLastUpdateTime: true,
        },
        blog: false,
        theme: {
          customCss: './src/css/custom.css',
        },
      }),
    ],
  ],

  themeConfig:
    /** @type {import('@docusaurus/preset-classic').ThemeConfig} */
    ({
      image: 'img/social-card.svg',
      metadata: [
        {name: 'theme-color', content: '#f04f78'},
        {
          name: 'description',
          content:
            'Complete user, contributor, architecture, and API documentation for the WebRadioFM Windows Phone 8.1 Last.fm scrobbler.',
        },
      ],
      announcementBar: {
        id: 'legacy-platform',
        content:
          'WebRadioFM targets the legacy Windows Phone 8.1 Silverlight platform. A compatible Windows SDK is required to build it.',
        backgroundColor: '#152338',
        textColor: '#ffffff',
        isCloseable: true,
      },
      colorMode: {
        defaultMode: 'light',
        respectPrefersColorScheme: true,
      },
      navbar: {
        title: 'WebRadioFM',
        logo: {
          alt: 'WebRadioFM waveform logo',
          src: 'img/logo.svg',
        },
        hideOnScroll: true,
        items: [
          {
            type: 'docSidebar',
            sidebarId: 'userGuideSidebar',
            position: 'left',
            label: 'User guide',
          },
          {
            type: 'docSidebar',
            sidebarId: 'developerSidebar',
            position: 'left',
            label: 'Developer guide',
          },
          {
            to: '/docs/api-reference/overview',
            label: 'API reference',
            position: 'left',
          },
          {
            href: repositoryUrl,
            label: 'GitHub',
            position: 'right',
          },
        ],
      },
      footer: {
        style: 'dark',
        links: [
          {
            title: 'Use WebRadioFM',
            items: [
              {label: 'Get started', to: '/docs/getting-started/overview'},
              {label: 'Install', to: '/docs/getting-started/install'},
              {label: 'Last.fm setup', to: '/docs/user-guide/lastfm-setup'},
            ],
          },
          {
            title: 'Build & understand',
            items: [
              {label: 'Build from source', to: '/docs/development/building'},
              {label: 'Architecture', to: '/docs/architecture/overview'},
              {label: 'API reference', to: '/docs/api-reference/overview'},
            ],
          },
          {
            title: 'Project',
            items: [
              {label: 'Source code', href: repositoryUrl},
              {label: 'Issues', href: `${repositoryUrl}/issues`},
              {label: 'License', href: `${repositoryUrl}/blob/main/LICENSE`},
            ],
          },
        ],
        copyright: `Copyright © ${new Date().getFullYear()} WebRadioFM contributors. Built with Docusaurus.`,
      },
      prism: {
        theme: prismThemes.github,
        darkTheme: prismThemes.dracula,
        additionalLanguages: ['csharp', 'powershell', 'json', 'xml-doc'],
      },
      tableOfContents: {
        minHeadingLevel: 2,
        maxHeadingLevel: 4,
      },
    }),
};

export default config;
