const { existsSync } = require('fs');
const { spawnSync } = require('child_process');
const path = require('path');

const projectPath = '.\\NotepadPlusPlus.FlaUI.Tests\\NotepadPlusPlus.FlaUI.Tests.csproj';
const testsDir = path.join('NotepadPlusPlus.FlaUI.Tests', 'Tests');
const smokeDir = path.join('NotepadPlusPlus.FlaUI.Tests', 'SmokeTests');
const artifactsRoot = process.env.NOTEPAD_PLUS_PLUS_ARTIFACTS || path.join('NotepadPlusPlus.FlaUI.Tests', 'bin', 'Debug', 'net48', 'TestArtifacts');
const htmlReportPath = process.env.NOTEPAD_PLUS_PLUS_REPORTS
  ? path.join(process.env.NOTEPAD_PLUS_PLUS_REPORTS, 'index.html')
  : path.join('NotepadPlusPlus.FlaUI.Tests', 'Reporting', 'report', 'index.html');

const command = process.argv[2];
const rawArgs = process.argv.slice(3);

function fail(message) {
  console.error(message);
  process.exit(1);
}

function normalizeFileInput(value) {
  if (!value) {
    fail('Provide a test file or class name. Example: npm run test:file -- DialogBehaviorTests.cs');
  }

  const trimmed = value.trim().replaceAll('/', path.sep);
  const baseName = path.basename(trimmed);
  const fileName = baseName.endsWith('.cs') ? baseName : `${baseName}.cs`;
  const className = fileName.replace(/\.cs$/i, '');

  const candidates = [
    path.join(testsDir, fileName),
    path.join(smokeDir, fileName),
    path.join('NotepadPlusPlus.FlaUI.Tests', trimmed),
  ];

  const existingPath = candidates.find((candidate) => existsSync(candidate));
  if (!existingPath) {
    fail(`Test file not found: ${value}`);
  }

  return { className, existingPath };
}

function runDotnet(filterArgs) {
  const dotnetArgs = ['test', projectPath, '--logger', 'console;verbosity=detailed', ...filterArgs];
  const result = spawnSync('dotnet', dotnetArgs, { stdio: 'inherit', shell: true });

  console.log(`HTML report: ${path.resolve(htmlReportPath)}`);

  if (typeof result.status === 'number') {
    process.exit(result.status);
  }

  process.exit(1);
}

if (command === 'all') {
  runDotnet([]);
}

if (command === 'smoke') {
  runDotnet(['--filter', 'Name~Launches_NotepadPlusPlus_MainWindow']);
}

if (command === 'file') {
  const { className } = normalizeFileInput(rawArgs[0]);
  runDotnet(['--filter', `Name~${className}`]);
}

if (command === 'tests') {
  runDotnet(['--filter', 'FullyQualifiedName~NotepadPlusPlus.FlaUI.Tests.Tests.']);
}

if (command === 'bdd') {
  runDotnet(['--filter', 'FullyQualifiedName~NotepadPlusPlus.FlaUI.Tests.Features.']);
}

if (command === 'filter') {
  if (!rawArgs[0]) {
    fail('Provide a filter. Example: npm run test:one -- Name~DialogBehaviorTests');
  }

  runDotnet(['--filter', rawArgs[0]]);
}

fail(`Unknown command: ${command}`);