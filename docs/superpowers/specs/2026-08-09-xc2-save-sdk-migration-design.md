# XC2SaveNETThingy SDK-style migration design

## Goal

Convert the three-project XC2SaveNETThingy solution from legacy .NET Framework project files to modern SDK-style projects, first targeting .NET 10 Windows Desktop and falling back to .NET Framework 4.7.2 only when a concrete compatibility blocker prevents a successful build or startup check.

## Current context

- The solution contains one class library and two WinForms executables.
- All three projects use legacy MSBuild project files and target .NET Framework 4.6.
- NuGet dependencies are managed with `packages.config`; SQLite also imports a package-specific legacy `.targets` file.
- The projects use WinForms, embedded resources, copied data/database files, two local DLL references, and SQLite native runtime files.
- The current machine has .NET SDK 10.0.302 but does not have the .NET Framework 4.6 reference assemblies, so the baseline build stops with `MSB3644` before compiling source code.
- The working tree contains an existing untracked `romdata/` directory. It is outside this change and must remain untouched.

## Chosen approach

### Project and solution format

Rewrite all three `.csproj` files as SDK-style projects. Use the SDK default file globs for C# files and resources, while explicitly describing non-code content that must be copied or embedded. Use `PackageReference` for the existing `Be.Windows.Forms.HexBox` and `System.Data.SQLite.Core` versions, and use normal `ProjectReference` entries for the two executable-to-library relationships.

Create a modern `.slnx` solution containing the same three projects and retain the existing `.sln` as a compatibility entry point. Both solution files must reference the same SDK-style projects and remain buildable without separate project configuration.

The first target framework is `net10.0-windows`, with WinForms enabled and implicit usings/nullable disabled to minimize unrelated source changes. Existing `Properties/AssemblyInfo.cs` files remain authoritative for assembly metadata, so SDK-generated assembly attributes are disabled where necessary.

### Dependency and runtime compatibility

Keep the existing dependency versions during migration. This isolates project-format changes from dependency upgrades. Preserve the local `DataGridViewNumericUpDownElements.dll` and `ZLIB.dll` references, application icons, App.config files, resource files, SQL/CSV data, and `XC2Data.db` copy behavior.

Preserve the existing executable platform intent, including the x86 configuration used by the base-game editor. Confirm that the SQLite managed assembly and native runtime assets resolve after the SDK migration by building and starting both WinForms applications.

If .NET 10 compilation or startup is blocked by a specific legacy assembly or API compatibility issue, record that evidence and convert the same SDK-style projects to `net472`. Do not hide the problem with warning suppression or unrelated source rewrites.

### Diagnosis and validation

Proceed in small, observable steps:

1. Generate the SDK-style project/solution structure and run a Release build.
2. Classify each compiler or MSBuild error by root cause before making a fix.
3. Rebuild the full solution after each focused change.
4. After a successful build, launch `XC2Bf2SaveNETThingy` and `XC2IraSaveNETThingy` long enough to verify that startup initialization does not fail immediately.
5. If .NET 10 cannot satisfy both build and startup requirements, apply the documented `net472` fallback and repeat the same validation.

The acceptance command is `dotnet build XC2SaveNETThingy.slnx --configuration Release`. The final result must state the actual target framework, build output, and startup-check outcome.

## Alternatives considered

1. **SDK-style + .NET 10 first, SDK-style + net472 fallback — selected.** This follows the requested priority, removes dependence on the missing .NET Framework 4.6 developer pack, and keeps the fallback conservative.
2. **Direct SDK-style + net472.** Lower behavioral risk for the old WinForms and SQLite stack, but it does not test the requested .NET 10 migration and still depends on the 4.7.2 reference assemblies being installed.
3. **Multi-target net472 and net10.0-windows.** Provides two outputs but doubles compatibility and native-runtime configuration complexity without being required for this build-fix task.

## Scope boundaries

The work covers project/solution migration, dependency wiring, build errors caused by the migration, and immediate startup blockers. It does not redesign UI behavior, update save-file logic, upgrade dependency versions without evidence, modify `romdata/`, or introduce unrelated refactoring.
