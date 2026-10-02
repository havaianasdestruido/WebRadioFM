import React from 'react';
import clsx from 'clsx';
import Link from '@docusaurus/Link';
import Layout from '@theme/Layout';
import Heading from '@theme/Heading';
import styles from './index.module.css';

const features = [
  {
    number: '01',
    title: 'Play the local library',
    text: 'Understand library discovery, transport controls, shuffle behavior, progress updates, and the XNA MediaPlayer bridge.',
    link: '/docs/user-guide/playback',
    label: 'Playback guide',
  },
  {
    number: '02',
    title: 'Scrobble with confidence',
    text: 'Follow Last.fm authentication from API credentials to a saved session, then tune exactly when a play is submitted.',
    link: '/docs/user-guide/scrobbling',
    label: 'Scrobbling guide',
  },
  {
    number: '03',
    title: 'Maintain a legacy app',
    text: 'Navigate every page, service, helper, model, manifest, build script, and platform constraint in the repository.',
    link: '/docs/architecture/overview',
    label: 'Explore architecture',
  },
];

const modules = [
  ['UI', 'XAML pages + code-behind'],
  ['Audio', 'XNA MediaLibrary / MediaPlayer'],
  ['Last.fm', 'Signed HTTP API client'],
  ['State', 'IsolatedStorageSettings'],
];

function PlayerPreview() {
  return (
    <div className={styles.phoneWrap} aria-label="Stylized WebRadioFM player preview">
      <div className={styles.signal} aria-hidden="true">
        <span />
        <span />
        <span />
        <span />
      </div>
      <div className={styles.phone}>
        <div className={styles.phoneTop}>
          <span>WEBRADIO.FM</span>
          <span>10:08</span>
        </div>
        <p className={styles.eyebrow}>NOW PLAYING</p>
        <div className={styles.albumArt} aria-hidden="true">
          <span className={styles.albumRing} />
          <span className={styles.albumDot} />
        </div>
        <div className={styles.trackRow}>
          <div>
            <strong>Midnight Signal</strong>
            <span>Open Frequencies</span>
          </div>
          <span className={styles.heart}>♥</span>
        </div>
        <div className={styles.progress} aria-hidden="true">
          <span />
        </div>
        <div className={styles.timeRow}>
          <span>1:42</span>
          <span>3:31</span>
        </div>
        <div className={styles.controls} aria-hidden="true">
          <span>↝</span>
          <span>◀</span>
          <span className={styles.play}>Ⅱ</span>
          <span>▶</span>
          <span>↻</span>
        </div>
        <div className={styles.scrobbleBadge}>
          <span /> Scrobbling on
        </div>
      </div>
    </div>
  );
}

function FeatureGrid() {
  return (
    <section className={styles.featureSection}>
      <div className="container">
        <div className={styles.sectionHeading}>
          <p className={styles.kicker}>ONE SOURCE OF TRUTH</p>
          <Heading as="h2">From first tap to final callback.</Heading>
          <p>
            Task-oriented guides for listeners and code-level references for the people
            keeping WebRadioFM alive.
          </p>
        </div>
        <div className={styles.featureGrid}>
          {features.map((feature) => (
            <article className={styles.featureCard} key={feature.number}>
              <span className={styles.featureNumber}>{feature.number}</span>
              <Heading as="h3">{feature.title}</Heading>
              <p>{feature.text}</p>
              <Link to={feature.link}>{feature.label} <span aria-hidden="true">→</span></Link>
            </article>
          ))}
        </div>
      </div>
    </section>
  );
}

function Architecture() {
  return (
    <section className={styles.architectureSection}>
      <div className={clsx('container', styles.architectureGrid)}>
        <div>
          <p className={styles.kicker}>THE WHOLE SIGNAL PATH</p>
          <Heading as="h2">A small app, fully mapped.</Heading>
          <p className={styles.architectureLead}>
            See how a selected song becomes an XNA playback request, a transformed
            metadata record, and a signed Last.fm scrobble—without losing track of the
            UI thread or persisted state.
          </p>
          <Link className="button button--primary button--lg" to="/docs/architecture/overview">
            Open the architecture map
          </Link>
        </div>
        <div className={styles.moduleStack}>
          {modules.map(([name, detail], index) => (
            <div className={styles.module} key={name}>
              <span>{String(index + 1).padStart(2, '0')}</span>
              <strong>{name}</strong>
              <code>{detail}</code>
            </div>
          ))}
        </div>
      </div>
    </section>
  );
}

function QuickStart() {
  return (
    <section className={styles.quickStartSection}>
      <div className={clsx('container', styles.quickStartGrid)}>
        <div>
          <p className={styles.kicker}>DOCUMENTATION DEVELOPMENT</p>
          <Heading as="h2">Run the handbook locally.</Heading>
          <p>
            The site uses Docusaurus 3.10.2. Application builds remain a separate,
            Windows-only workflow that requires the legacy Windows Phone toolchain.
          </p>
          <Link to="/docs/development/documentation">Documentation contributor guide →</Link>
        </div>
        <div className={styles.terminal} aria-label="Commands to start the documentation site">
          <div className={styles.terminalTop}>
            <span />
            <span />
            <span />
            <code>webradiofm / docs</code>
          </div>
          <pre><code><span>$</span> npm install{`\n`}<span>$</span> npm start{`\n`}{`\n`}<i>✓</i> Local: http://localhost:3000/</code></pre>
        </div>
      </div>
    </section>
  );
}

export default function Home() {
  return (
    <Layout
      title="Project handbook"
      description="Complete user and developer documentation for the WebRadioFM Windows Phone 8.1 Last.fm scrobbler">
      <main>
        <header className={styles.heroBanner}>
          <div className={clsx('container', styles.heroGrid)}>
            <div className={styles.heroCopy}>
              <div className={styles.statusPill}>
                <span aria-hidden="true" /> Windows Phone 8.1 · Open source
              </div>
              <Heading as="h1">
                Your music.<br />
                <em>Your listening history.</em>
              </Heading>
              <p className={styles.heroLead}>
                The complete handbook for WebRadioFM—a local music player and Last.fm
                scrobbler built for Windows Phone 8.1 Silverlight.
              </p>
              <div className={styles.heroButtons}>
                <Link className="button button--primary button--lg" to="/docs/getting-started/overview">
                  Start reading <span aria-hidden="true">→</span>
                </Link>
                <Link className={clsx('button button--secondary button--lg', styles.secondaryButton)} to="/docs/development/building">
                  Build from source
                </Link>
              </div>
              <div className={styles.heroMeta} aria-label="Project technology summary">
                <span><b>C#</b> application</span>
                <span><b>3</b> XAML pages</span>
                <span><b>10</b> Last.fm methods</span>
              </div>
            </div>
            <PlayerPreview />
          </div>
        </header>
        <FeatureGrid />
        <Architecture />
        <QuickStart />
      </main>
    </Layout>
  );
}
