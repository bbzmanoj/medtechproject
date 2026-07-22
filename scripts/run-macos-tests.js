const fs = require('fs');
const path = require('path');
const { spawnSync } = require('child_process');

const repoRoot = path.resolve(__dirname, '..');
const macRoot = path.join(repoRoot, 'NotepadPlusPlus.FlaUI.Tests', 'Platforms', 'Mac');
const contractPath = path.join(macRoot, 'adapter-contract.json');
const appleScriptPath = path.join(repoRoot, 'scripts', 'macos', 'run-textedit-scenarios.applescript');
const artifactsRoot = process.env.MEDTECH_MAC_ARTIFACTS || path.join(macRoot, 'artifacts');
const reportRoot = process.env.MEDTECH_MAC_REPORTS || path.join(macRoot, 'report');
const reportPath = path.join(reportRoot, 'index.html');
const resultsPath = path.join(artifactsRoot, 'results.json');

function fail(message) {
  console.error(message);
  process.exit(1);
}

function ensureDirectory(directoryPath) {
  fs.mkdirSync(directoryPath, { recursive: true });
}

function readContract() {
  if (!fs.existsSync(contractPath)) {
    fail(`Mac adapter contract not found: ${contractPath}`);
  }

  return JSON.parse(fs.readFileSync(contractPath, 'utf8'));
}

function parseScenarioOutput(stdout, contract) {
  const plannedStatuses = new Map(contract.scenarios.map((scenario) => [scenario.id, scenario.status]));
  const lines = stdout.split(/\r?\n/).map((line) => line.trim()).filter(Boolean);

  return lines.map((line) => {
    const [id, status, durationMs, details] = line.split('|||');
    return {
      id,
      status,
      durationMs: Number(durationMs || 0),
      details: details || '',
      implementationStatus: plannedStatuses.get(id) || 'unknown'
    };
  });
}

function generateHtmlReport(results, contract) {
  const passedCount = results.filter((result) => result.status === 'passed').length;
  const failedCount = results.filter((result) => result.status === 'failed').length;
  const skippedCount = results.filter((result) => result.status === 'skipped').length;
  const totalCount = results.length;
  const totalDurationMs = results.reduce((sum, result) => sum + result.durationMs, 0);
  const generatedAt = new Date().toISOString();
  const rows = results.map((result) => {
    return `
      <tr>
        <td>${escapeHtml(result.id)}</td>
        <td><span class="status-pill ${escapeHtml(result.status)}">${escapeHtml(result.status)}</span></td>
        <td>${escapeHtml(result.implementationStatus)}</td>
        <td>${result.durationMs}</td>
        <td>${escapeHtml(result.details)}</td>
      </tr>`;
  }).join('');

  return `<!doctype html>
<html lang="en">
<head>
  <meta charset="utf-8">
  <title>TextEdit macOS Scenario Report</title>
  <style>
    body { font-family: -apple-system, BlinkMacSystemFont, "Segoe UI", sans-serif; margin: 0; background: #f5f7fb; color: #1f2937; }
    header { padding: 32px; background: linear-gradient(135deg, #0f3d56, #1b6b7b); color: #fff; }
    main { padding: 24px 32px; }
    .summary, .charts { display: flex; gap: 16px; margin: 0 0 24px; flex-wrap: wrap; }
    .card, .chart-card { background: #fff; border-radius: 16px; padding: 16px 20px; box-shadow: 0 10px 30px rgba(15, 61, 86, 0.08); min-width: 160px; }
    .chart-card { flex: 1 1 320px; }
    .label { font-size: 12px; text-transform: uppercase; letter-spacing: 0.04em; color: #6b7280; }
    .value { font-size: 28px; font-weight: 700; margin-top: 6px; }
    .subtle { color: #475467; margin-top: 8px; }
    .chart-row { display: grid; grid-template-columns: 90px 1fr 48px; gap: 12px; align-items: center; margin: 12px 0; }
    .chart-track { background: #e5e7eb; border-radius: 999px; overflow: hidden; height: 14px; }
    .chart-fill { height: 100%; border-radius: 999px; }
    .chart-fill.passed { background: #22a06b; }
    .chart-fill.failed { background: #d92d20; }
    .chart-fill.skipped { background: #1570ef; }
    table { width: 100%; border-collapse: collapse; background: #fff; border-radius: 16px; overflow: hidden; box-shadow: 0 10px 30px rgba(15, 61, 86, 0.08); }
    th, td { padding: 14px 16px; border-bottom: 1px solid #e5e7eb; text-align: left; vertical-align: top; }
    th { font-size: 12px; text-transform: uppercase; letter-spacing: 0.04em; color: #6b7280; background: #f9fafb; }
    tr:last-child td { border-bottom: 0; }
    .status-pill { display: inline-block; padding: 4px 10px; border-radius: 999px; font-size: 12px; font-weight: 700; text-transform: uppercase; letter-spacing: 0.04em; }
    .status-pill.passed { background: #e8f7f0; color: #13795b; }
    .status-pill.failed { background: #fdeceb; color: #b42318; }
    .status-pill.skipped { background: #e8f1fd; color: #175cd3; }
    .note { margin-top: 16px; color: #475467; }
  </style>
</head>
<body>
  <header>
    <h1>TextEdit macOS Scenario Report</h1>
    <p>Automation: ${escapeHtml(contract.automation)} | Generated: ${escapeHtml(generatedAt)}</p>
  </header>
  <main>
    <section class="summary">
      <div class="card"><div class="label">Passed</div><div class="value">${passedCount}</div></div>
      <div class="card"><div class="label">Failed</div><div class="value">${failedCount}</div></div>
      <div class="card"><div class="label">Skipped</div><div class="value">${skippedCount}</div></div>
      <div class="card"><div class="label">OS</div><div class="value">macOS</div></div>
    </section>
    <section class="charts">
      <div class="chart-card">
        <div class="label">Status Distribution</div>
        ${buildChartRow('Passed', passedCount, totalCount, 'passed')}
        ${buildChartRow('Failed', failedCount, totalCount, 'failed')}
        ${buildChartRow('Skipped', skippedCount, totalCount, 'skipped')}
      </div>
      <div class="chart-card">
        <div class="label">Run Summary</div>
        <div class="value">${totalCount}</div>
        <div class="subtle">Total scenarios in this report</div>
        <div class="subtle">Adapter: ${escapeHtml(contract.application)}</div>
        <div class="subtle">Total duration: ${totalDurationMs} ms</div>
      </div>
    </section>
    <table>
      <thead>
        <tr>
          <th>Scenario</th>
          <th>Status</th>
          <th>Adapter Status</th>
          <th>Duration (ms)</th>
          <th>Details</th>
        </tr>
      </thead>
      <tbody>${rows}</tbody>
    </table>
    <p class="note">Scenario B remains planned until a concrete TextEdit replace-dialog automation mapping is added.</p>
  </main>
</body>
</html>`;
}

function buildChartRow(label, count, totalCount, cssClass) {
  const percentage = totalCount === 0 ? 0 : Math.round((count * 100) / totalCount);
  return `
    <div class="chart-row">
      <span>${escapeHtml(label)}</span>
      <div class="chart-track"><div class="chart-fill ${cssClass}" style="width: ${percentage}%"></div></div>
      <span>${percentage}%</span>
    </div>`;
}

function escapeHtml(value) {
  return String(value)
    .replaceAll('&', '&amp;')
    .replaceAll('<', '&lt;')
    .replaceAll('>', '&gt;')
    .replaceAll('"', '&quot;')
    .replaceAll("'", '&#39;');
}

function runMacScenarios(contract) {
  if (process.platform !== 'darwin') {
    fail('The macOS adapter runner can only execute on macOS because it depends on osascript and TextEdit.');
  }

  if (!fs.existsSync(appleScriptPath)) {
    fail(`AppleScript runner not found: ${appleScriptPath}`);
  }

  ensureDirectory(artifactsRoot);
  ensureDirectory(reportRoot);

  const result = spawnSync('osascript', [appleScriptPath, artifactsRoot], {
    encoding: 'utf8'
  });

  if (result.error) {
    fail(result.error.message);
  }

  if (typeof result.status === 'number' && result.status !== 0) {
    fail((result.stderr || result.stdout || 'The macOS scenario runner failed.').trim());
  }

  const parsedResults = parseScenarioOutput(result.stdout || '', contract);
  fs.writeFileSync(resultsPath, JSON.stringify({ generatedAt: new Date().toISOString(), results: parsedResults }, null, 2));
  fs.writeFileSync(reportPath, generateHtmlReport(parsedResults, contract));

  console.log(`macOS HTML report: ${reportPath}`);
  console.log(`macOS JSON results: ${resultsPath}`);

  const failed = parsedResults.filter((scenario) => scenario.status === 'failed');
  if (failed.length > 0) {
    process.exit(1);
  }
}

runMacScenarios(readContract());