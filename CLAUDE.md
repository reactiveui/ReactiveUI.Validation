# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Build & Test Commands

This project uses **Microsoft Testing Platform (MTP)** with the **TUnit** testing framework. Test commands differ significantly from traditional VSTest.

See: https://learn.microsoft.com/en-us/dotnet/core/tools/dotnet-test?tabs=dotnet-test-with-mtp

### Prerequisites

```powershell
# Check .NET installation
dotnet --info

# Restore NuGet packages
dotnet restore ReactiveUI.Validation.slnx
```

**Note**: This repository uses **SLNX** (XML-based solution format) instead of the legacy SLN format.

### Build Commands

**CRITICAL:** The working folder must be `./src` folder. These commands won't function properly without the correct working folder.

```powershell
# Build the solution
dotnet build ReactiveUI.Validation.slnx -c Release

# Build with warnings as errors (includes StyleCop violations)
dotnet build ReactiveUI.Validation.slnx -c Release -warnaserror

# Clean the solution
dotnet clean ReactiveUI.Validation.slnx
```

### Test Commands (Microsoft Testing Platform)

**CRITICAL:** This repository uses MTP configured in `global.json`. All TUnit-specific arguments must be passed after `--`:

The working folder must be `./src` folder. These commands won't function properly without the correct working folder.

**IMPORTANT:**
- Do NOT use `--no-build` flag when running tests. Always build before testing to ensure all code changes (including test changes) are compiled. Using `--no-build` can cause tests to run against stale binaries and produce misleading results.
- Use `--output Detailed` to see Console.WriteLine output from tests. This must be placed BEFORE any `--` separator:
  ```powershell
  dotnet test --output Detailed -- --treenode-filter "..."
  ```

```powershell
# Run all tests in the solution
dotnet test --solution ReactiveUI.Validation.slnx -c Release

# Run all tests in a specific project
dotnet test --project tests/ReactiveUI.Validation.Tests/ReactiveUI.Validation.Tests.csproj -c Release

# Run a single test method using treenode-filter
# Syntax: /{AssemblyName}/{Namespace}/{ClassName}/{TestMethodName}
dotnet test --project tests/ReactiveUI.Validation.Tests/ReactiveUI.Validation.Tests.csproj -- --treenode-filter "/*/*/*/MyTestMethod"

# Run all tests in a specific class
dotnet test --project tests/ReactiveUI.Validation.Tests/ReactiveUI.Validation.Tests.csproj -- --treenode-filter "/*/*/MyClassName/*"

# Run tests in a specific namespace
dotnet test --project tests/ReactiveUI.Validation.Tests/ReactiveUI.Validation.Tests.csproj -- --treenode-filter "/*/MyNamespace/*/*"

# Filter by test property (e.g., Category)
dotnet test --solution ReactiveUI.Validation.slnx -- --treenode-filter "/*/*/*/*[Category=Integration]"

# Run tests with code coverage (Microsoft Code Coverage)
dotnet test --solution ReactiveUI.Validation.slnx -- --coverage --coverage-output-format cobertura

# Run tests with detailed output
dotnet test --solution ReactiveUI.Validation.slnx -- --output Detailed

# List all available tests without running them
dotnet test --project tests/ReactiveUI.Validation.Tests/ReactiveUI.Validation.Tests.csproj -- --list-tests

# Fail fast (stop on first failure)
dotnet test --solution ReactiveUI.Validation.slnx -- --fail-fast

# Control parallel test execution
dotnet test --solution ReactiveUI.Validation.slnx -- --maximum-parallel-tests 4

# Generate TRX report
dotnet test --solution ReactiveUI.Validation.slnx -- --report-trx

# Disable logo for cleaner output
dotnet test --project tests/ReactiveUI.Validation.Tests/ReactiveUI.Validation.Tests.csproj -- --disable-logo

# Combine options: coverage + TRX report + detailed output
dotnet test --solution ReactiveUI.Validation.slnx -- --coverage --coverage-output-format cobertura --report-trx --output Detailed
```

**Alternative: Using `dotnet run` for single project**
```powershell
# Run tests using dotnet run (easier for passing flags)
dotnet run --project tests/ReactiveUI.Validation.Tests/ReactiveUI.Validation.Tests.csproj -c Release -- --treenode-filter "/*/*/*/MyTest"

# Disable logo for cleaner output
dotnet run --project tests/ReactiveUI.Validation.Tests/ReactiveUI.Validation.Tests.csproj -- --disable-logo --treenode-filter "/*/*/*/Test1"
```

### TUnit Treenode-Filter Syntax

The `--treenode-filter` follows the pattern: `/{AssemblyName}/{Namespace}/{ClassName}/{TestMethodName}`

**Examples:**
- Single test: `--treenode-filter "/*/*/*/MyTestMethod"`
- All tests in class: `--treenode-filter "/*/*/MyClassName/*"`
- All tests in namespace: `--treenode-filter "/*/MyNamespace/*/*"`
- Filter by property: `--treenode-filter "/*/*/*/*[Category=Integration]"`
- Multiple wildcards: `--treenode-filter "/*/*/MyTests*/*"`

**Note:** Use single asterisks (`*`) to match segments. Double asterisks (`/**`) are not supported in treenode-filter.

### Key TUnit Command-Line Flags

- `--treenode-filter` - Filter tests by path pattern or properties (syntax: `/{Assembly}/{Namespace}/{Class}/{Method}`)
- `--list-tests` - Display available tests without running
- `--fail-fast` - Stop after first failure
- `--maximum-parallel-tests` - Limit concurrent execution (default: processor count)
- `--coverage` - Enable Microsoft Code Coverage
- `--coverage-output-format` - Set coverage format (cobertura, xml, coverage)
- `--report-trx` - Generate TRX format reports
- `--output` - Control verbosity (Normal or Detailed)
- `--no-progress` - Suppress progress reporting
- `--disable-logo` - Remove TUnit logo display
- `--diagnostic` - Enable diagnostic logging (Trace level)
- `--timeout` - Set global test timeout

See https://tunit.dev/docs/reference/command-line-flags for complete TUnit flag reference.

### Code Coverage Reports

To generate and view a code coverage report:

```powershell
# 1. Clean bin/obj to ensure fresh instrumentation
find . -type d \( -name bin -o -name obj \) -exec rm -rf {} + 2>/dev/null

# 2. Run tests with coverage
dotnet test --project tests/ReactiveUI.Validation.Tests/ReactiveUI.Validation.Tests.csproj -c Release -- --coverage --coverage-output-format cobertura --disable-logo --no-progress

# 3. Generate a text summary (requires dotnet-reportgenerator-globaltool)
reportgenerator -reports:"tests/ReactiveUI.Validation.Tests/bin/Release/net10.0/TestResults/*.cobertura.xml" -targetdir:/tmp/coverage-report -reporttypes:TextSummary
cat /tmp/coverage-report/Summary.txt

# 4. Generate an HTML report for detailed per-file analysis
reportgenerator -reports:"tests/ReactiveUI.Validation.Tests/bin/Release/net10.0/TestResults/*.cobertura.xml" -targetdir:/tmp/coverage-report -reporttypes:Html
```

**Note:** `Microsoft.Testing.Extensions.CodeCoverage` is pinned at 18.4.1 due to a regression in newer versions that produces incorrect coverage data. Do not upgrade without verifying coverage output.

### Key Configuration Files

- `global.json` - Specifies `"Microsoft.Testing.Platform"` as the test runner
- `testconfig.json` - Configures test execution (`"parallel": true`) and code coverage (Cobertura format)
- `Directory.Build.props` - Enables `TestingPlatformDotnetTestSupport` for test projects

## Architecture Overview

ReactiveUI.Validation is a validation library for ReactiveUI applications, providing a fluent API for defining and binding validation rules to view models and views.

### Project Structure

- `ReactiveUI.Validation/` - Core validation library
- `ReactiveUI.Validation.AndroidX/` - Android-specific extensions
- `tests/ReactiveUI.Validation.Tests/` - Unit tests

### Core Concepts

- **`ValidationContext`** - Container that aggregates multiple validation components
- **`BasePropertyValidation<TViewModel, TValue>`** - Validates a single view model property
- **`ObservableValidation<TViewModel, TValue>`** - Observable-based validation component
- **`ValidationHelper`** - Bindable wrapper around a `ValidationContext` or single rule
- **`ValidationText`** - Immutable collection of error message strings
- **`IValidationState`** - Represents valid/invalid state with associated text

### Extension Methods

- `vm.ValidationRule(...)` - Attach a validation rule to a view model
- `view.BindValidation(...)` - Bind validation errors to a view property
- `vm.IsValid()` - Observable stream of validity changes
- `vm.ClearValidationRules(...)` - Remove attached rules

### Key Patterns

**Defining rules on a view model:**
```csharp
this.ValidationRule(
    vm => vm.Name,
    name => !string.IsNullOrWhiteSpace(name),
    "Name should not be empty.");
```

**Binding to a view:**
```csharp
this.BindValidation(ViewModel, vm => vm.Name, v => v.NameErrorLabel);
```

**Observable-based rules:**
```csharp
this.ValidationRule(
    vm => vm.Name,
    this.WhenAnyValue(x => x.Name, x => x.Name2, (a, b) => a == b),
    "Names must match.");
```

## Code Style & Quality Requirements

**CRITICAL:** All code must comply with ReactiveUI contribution guidelines: https://www.reactiveui.net/contribute/index.html

### Style Enforcement

- EditorConfig rules (`.editorconfig`) - comprehensive C# formatting and naming conventions
- StyleCop Analyzers - builds fail on violations
- Roslynator Analyzers - additional code quality rules
- **All public APIs require XML documentation comments**

### C# Style Rules

- **Braces:** Allman style (each brace on new line)
- **Indentation:** 4 spaces, no tabs
- **Fields:** `_camelCase` for private/internal, `readonly` where possible
- **Visibility:** Always explicit, visibility first modifier
- **Namespaces:** File-scoped preferred, imports outside namespace
- **Modern C#:** Use nullable reference types, pattern matching, file-scoped namespaces

## Testing Guidelines

- Unit tests use **TUnit** framework with **Microsoft Testing Platform**
- Test projects detected via `IsTestProject` MSBuild property
- Coverage configured in `testconfig.json` (Cobertura format, skip auto-properties)
- Parallel test execution enabled (`"parallel": true` in testconfig.json)
- Always write unit tests for new features or bug fixes
- Follow existing test patterns in `tests/ReactiveUI.Validation.Tests/`
- API approval tests use **Verify.TUnit** — run tests to regenerate `.verified.txt` files when the public API changes
