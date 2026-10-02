// @ts-check

/** @type {import('@docusaurus/plugin-content-docs').SidebarsConfig} */
const sidebars = {
  userGuideSidebar: [
    {
      type: 'doc',
      id: 'getting-started/overview',
      label: 'Welcome',
    },
    {
      type: 'category',
      label: 'Getting started',
      collapsed: false,
      items: [
        'getting-started/install',
        'getting-started/first-run',
        'getting-started/requirements',
      ],
    },
    {
      type: 'category',
      label: 'User guide',
      collapsed: false,
      items: [
        'user-guide/playback',
        'user-guide/lastfm-setup',
        'user-guide/scrobbling',
        'user-guide/metadata-rules',
        'user-guide/statistics',
        'user-guide/appearance',
        'user-guide/privacy-security',
        'user-guide/troubleshooting',
      ],
    },
  ],

  developerSidebar: [
    {
      type: 'doc',
      id: 'development/contributing',
      label: 'Contributor overview',
    },
    {
      type: 'category',
      label: 'Development',
      collapsed: false,
      items: [
        'development/building',
        'development/project-structure',
        'development/debugging-testing',
        'development/releasing',
        'development/documentation',
      ],
    },
    {
      type: 'category',
      label: 'Architecture',
      collapsed: false,
      items: [
        'architecture/overview',
        'architecture/application-lifecycle',
        'architecture/audio-playback',
        'architecture/lastfm-integration',
        'architecture/scrobble-pipeline',
        'architecture/settings-storage',
        'architecture/ui-theming-localization',
      ],
    },
    {
      type: 'category',
      label: 'API reference',
      collapsed: false,
      items: [
        'api-reference/overview',
        'api-reference/application-pages',
        'api-reference/services',
        'api-reference/helpers',
        'api-reference/models',
        'api-reference/manifests-resources',
      ],
    },
  ],
};

export default sidebars;
