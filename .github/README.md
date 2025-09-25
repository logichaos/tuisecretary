# GitHub Workflows

This directory contains GitHub Actions workflows and reusable actions for the TUI Secretary project.

## Workflows

### `test-and-coverage.yml`
Runs on every pull request to the `main` branch. This workflow:

1. **Build & Test**: Builds the .NET solution and runs all tests
2. **Coverage Collection**: Collects code coverage data using XPlat Code Coverage
3. **Coverage Reporting**: Generates HTML coverage reports using ReportGenerator
4. **Threshold Check**: Ensures code coverage is at least 70%

If coverage is below 70%, the workflow will fail and prevent the PR from being merged.

## Reusable Actions

### `run-tests-with-coverage`
A composite action that:
- Installs the ReportGenerator global tool
- Runs all tests with coverage collection
- Generates coverage reports in HTML and Cobertura formats
- Extracts the overall coverage percentage
- Uploads test results and coverage reports as artifacts

**Outputs:**
- `coverage-percentage`: The overall code coverage percentage
- `test-results`: Test execution status (PASSED/FAILED)

### `check-coverage-threshold`
A composite action that:
- Compares coverage percentage against a configurable threshold
- Fails the workflow if coverage is below the threshold
- Provides detailed feedback in the GitHub Actions summary

**Inputs:**
- `coverage-percentage`: The coverage percentage to check (required)
- `threshold`: Minimum coverage threshold percentage (default: 70)

## Usage Examples

### Running Tests Locally with Coverage
```bash
# Install ReportGenerator (one-time setup)
dotnet tool install -g dotnet-reportgenerator-globaltool

# Run tests with coverage
dotnet test --collect:"XPlat Code Coverage" --results-directory ./TestResults

# Generate coverage report
reportgenerator \
  "-reports:./TestResults/**/coverage.cobertura.xml" \
  "-targetdir:./CoverageReport" \
  "-reporttypes:Html;Cobertura"

# Open coverage report
open ./CoverageReport/index.html  # macOS
xdg-open ./CoverageReport/index.html  # Linux
```

### Current Project Coverage
As of the latest run, the project has approximately **33%** code coverage. The workflow will fail until coverage reaches **70%** or higher.

## Extending the Workflow

The actions are designed to be modular and reusable. You can:
- Use the actions in other workflows
- Adjust the coverage threshold by modifying the workflow file
- Add additional steps or validations as needed

Both actions provide detailed output and summaries that appear in the GitHub Actions interface, making it easy to understand test results and coverage information.