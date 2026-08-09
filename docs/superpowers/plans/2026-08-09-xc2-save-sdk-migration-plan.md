# XC2SaveNETThingy SDK migration and build repair Implementation Plan

> For agentic workers: REQUIRED SUB-SKILL: Use superpowers:subagent-driven-development (recommended) or superpowers:executing-plans to implement this plan task-by-task. Steps use checkbox syntax for tracking.

**Goal:** Convert the solution to modern SDK-style projects, compile it with .NET 10 when the existing dependencies permit it, and otherwise compile the same SDK-style solution on .NET Framework 4.7.2.

**Architecture:** Keep the existing three-project boundary: XC2SaveNETThingy remains the shared WinForms-aware class library, and XC2Bf2SaveNETThingy/XC2IraSaveNETThingy remain executable WinForms front ends. Replace legacy MSBuild item lists and packages.config with SDK default items, PackageReference, normal ProjectReference, explicit copied data, and a modern .slnx solution while retaining the old .sln as a compatibility entry point.

**Tech Stack:** .NET SDK 10.0.302, C#, WinForms, System.Data.SQLite.Core 1.0.109.2, Be.Windows.Forms.HexBox 1.6.1, MSBuild SDK-style projects, PowerShell startup checks.

---

## File map

- Create: XC2SaveNETThingy.slnx
- Modify: XC2SaveNETThingy.sln only if the final SDK project state requires an old-solution compatibility correction.
- Replace: XC2SaveNETThingy/XC2SaveNETThingy.csproj
- Replace: XC2Bf2SaveNETThingy/XC2Bf2SaveNETThingy.csproj
- Replace: XC2IraSaveNETThingy/XC2IraSaveNETThingy.csproj
- Delete: XC2SaveNETThingy/packages.config
- Delete: XC2Bf2SaveNETThingy/packages.config
- Delete: XC2IraSaveNETThingy/packages.config
- Modify only with evidence: XC2Bf2SaveNETThingy/App.config and XC2IraSaveNETThingy/App.config
- Never modify: romdata/

## Task 1: Reproduce and record the legacy baseline

Files: none.

- [ ] Step 1: Check prerequisites.

    Run:

        dotnet --info
        Test-Path 'C:\Program Files (x86)\Reference Assemblies\Microsoft\Framework\.NETFramework\v4.7.2'

    Expected: .NET SDK 10.0.302 is available and the v4.7.2 reference-assembly directory exists.

- [ ] Step 2: Reproduce the current failure.

    Run:

        dotnet build .\XC2SaveNETThingy.sln --configuration Release --no-restore

    Expected before migration: exit code 1 with MSB3644 for .NETFramework,Version=v4.6. Do not change project files to hide this baseline.

- [ ] Step 3: Check the starting tree.

    Run:

        git status --short --branch
        git diff --check

    Expected: only the approved design commit and the existing untracked romdata/ path are present.

## Task 2: Convert the shared library to SDK-style .NET 10

Files: XC2SaveNETThingy/XC2SaveNETThingy.csproj.

- [ ] Step 1: Replace the project with this SDK-style structure.

    Use:

        <Project Sdk="Microsoft.NET.Sdk">
          <PropertyGroup>
            <TargetFramework>net10.0-windows</TargetFramework>
            <UseWindowsForms>true</UseWindowsForms>
            <RootNamespace>XC2SaveNETThingy</RootNamespace>
            <AssemblyName>XC2SaveNETThingy</AssemblyName>
            <GenerateAssemblyInfo>false</GenerateAssemblyInfo>
            <ImplicitUsings>disable</ImplicitUsings>
            <Nullable>disable</Nullable>
          </PropertyGroup>

          <PropertyGroup Condition="'$(Configuration)|$(Platform)' == 'Debug|AnyCPU'">
            <DebugSymbols>true</DebugSymbols>
            <DebugType>portable</DebugType>
            <Optimize>false</Optimize>
            <DefineConstants>DEBUG;TRACE</DefineConstants>
            <PlatformTarget>AnyCPU</PlatformTarget>
            <WarningLevel>4</WarningLevel>
          </PropertyGroup>

          <PropertyGroup Condition="'$(Configuration)|$(Platform)' == 'Release|AnyCPU'">
            <DebugType>portable</DebugType>
            <Optimize>true</Optimize>
            <DefineConstants>TRACE</DefineConstants>
            <PlatformTarget>AnyCPU</PlatformTarget>
            <WarningLevel>4</WarningLevel>
          </PropertyGroup>

          <ItemGroup>
            <PackageReference Include="Be.Windows.Forms.HexBox" Version="1.6.1" />
            <PackageReference Include="System.Data.SQLite.Core" Version="1.0.109.2" />
            <Reference Include="DataGridViewNumericUpDownElements">
              <HintPath>..\external\DataGridViewNumericUpDownElements.dll</HintPath>
              <Private>true</Private>
            </Reference>
            <Reference Include="ZLIB">
              <HintPath>..\external\ZLIB.dll</HintPath>
              <Private>true</Private>
            </Reference>
          </ItemGroup>

          <ItemGroup>
            <None Update="data\**\*.csv" CopyToOutputDirectory="PreserveNewest" />
            <None Update="db\**\*.sql" CopyToOutputDirectory="PreserveNewest" />
            <None Update="XC2Data.db" CopyToOutputDirectory="Always" />
          </ItemGroup>
        </Project>

    SDK default globs must provide all checked-in .cs, .Designer.cs, .resx, and settings files. Remove old Compile, EmbeddedResource, Microsoft.Common.props, Microsoft.CSharp.targets, SQLite targets, ProjectGuid, and EnsureNuGetPackageBuildImports blocks. Keep Properties/AssemblyInfo.cs because GenerateAssemblyInfo=false preserves its metadata.

- [ ] Step 2: Verify no legacy item machinery remains.

    Run:

        Select-String -Path .\XC2SaveNETThingy\XC2SaveNETThingy.csproj -Pattern 'Compile Include|EmbeddedResource Include|Microsoft.CSharp.targets|EnsureNuGetPackageBuildImports|TargetFrameworkVersion|ProjectGuid|packages\\'

    Expected: no matches.

- [ ] Step 3: Restore and build the library.

    Run:

        dotnet restore .\XC2SaveNETThingy\XC2SaveNETThingy.csproj --force-evaluate
        dotnet build .\XC2SaveNETThingy\XC2SaveNETThingy.csproj --configuration Release --no-restore

    Expected: the project reaches C# compilation. NU1701 for the net40 HexBox package is diagnostic evidence, not a reason to suppress all warnings.

## Task 3: Convert both WinForms executables to SDK-style .NET 10

Files: XC2Bf2SaveNETThingy/XC2Bf2SaveNETThingy.csproj and XC2IraSaveNETThingy/XC2IraSaveNETThingy.csproj.

- [ ] Step 1: Replace the base-game project with this structure.

    Use:

        <Project Sdk="Microsoft.NET.Sdk">
          <PropertyGroup>
            <OutputType>WinExe</OutputType>
            <TargetFramework>net10.0-windows</TargetFramework>
            <UseWindowsForms>true</UseWindowsForms>
            <RootNamespace>XC2Bf2SaveNETThingy</RootNamespace>
            <AssemblyName>XC2Bf2SaveNETThingy</AssemblyName>
            <ApplicationIcon>icon_bf2.ico</ApplicationIcon>
            <GenerateAssemblyInfo>false</GenerateAssemblyInfo>
            <ImplicitUsings>disable</ImplicitUsings>
            <Nullable>disable</Nullable>
          </PropertyGroup>
          <PropertyGroup Condition="'$(Configuration)|$(Platform)' == 'Debug|AnyCPU'">
            <PlatformTarget>x86</PlatformTarget>
            <DebugSymbols>true</DebugSymbols>
            <DebugType>portable</DebugType>
            <Optimize>false</Optimize>
            <DefineConstants>DEBUG;TRACE</DefineConstants>
            <WarningLevel>4</WarningLevel>
          </PropertyGroup>
          <PropertyGroup Condition="'$(Configuration)|$(Platform)' == 'Release|AnyCPU'">
            <PlatformTarget>AnyCPU</PlatformTarget>
            <DebugType>portable</DebugType>
            <Optimize>true</Optimize>
            <DefineConstants>TRACE</DefineConstants>
            <WarningLevel>4</WarningLevel>
          </PropertyGroup>
          <ItemGroup>
            <PackageReference Include="Be.Windows.Forms.HexBox" Version="1.6.1" />
            <PackageReference Include="System.Data.SQLite.Core" Version="1.0.109.2" />
            <Reference Include="DataGridViewNumericUpDownElements">
              <HintPath>..\external\DataGridViewNumericUpDownElements.dll</HintPath>
              <Private>true</Private>
            </Reference>
          </ItemGroup>
          <ItemGroup>
            <ProjectReference Include="..\XC2SaveNETThingy\XC2SaveNETThingy.csproj" />
            <None Update="icon_bf2.ico" CopyToOutputDirectory="PreserveNewest" />
            <Content Include="..\XC2SaveNETThingy\XC2Data.db" Link="XC2Data.db" CopyToOutputDirectory="Always" />
          </ItemGroup>
        </Project>

    The linked database is explicit because the runtime checks for XC2Data.db beside the executable.

- [ ] Step 2: Replace the Torna project with this structure.

    Use the same project shape with these exact differences:

        <OutputType>WinExe</OutputType>
        <TargetFramework>net10.0-windows</TargetFramework>
        <UseWindowsForms>true</UseWindowsForms>
        <RootNamespace>XC2IraSaveNETThingy</RootNamespace>
        <AssemblyName>XC2IraSaveNETThingy</AssemblyName>
        <ApplicationIcon>icon_ira.ico</ApplicationIcon>
        <GenerateAssemblyInfo>false</GenerateAssemblyInfo>
        <ImplicitUsings>disable</ImplicitUsings>
        <Nullable>disable</Nullable>

    Keep Debug and Release PlatformTarget as AnyCPU. Use the same two PackageReference entries, the same ProjectReference, and this final item:

        <None Update="icon_ira.ico" CopyToOutputDirectory="PreserveNewest" />
        <Content Include="..\XC2SaveNETThingy\XC2Data.db" Link="XC2Data.db" CopyToOutputDirectory="Always" />

- [ ] Step 3: Verify both executable projects contain no legacy project constructs.

    Run:

        Select-String -Path .\XC2Bf2SaveNETThingy\XC2Bf2SaveNETThingy.csproj, .\XC2IraSaveNETThingy\XC2IraSaveNETThingy.csproj -Pattern 'Compile Include|EmbeddedResource Include|Microsoft.CSharp.targets|EnsureNuGetPackageBuildImports|TargetFrameworkVersion|ProjectGuid|packages\\|System.Deployment|AutoGenerateBindingRedirects'

    Expected: no matches.

- [ ] Step 4: Restore and build each executable independently.

    Run:

        dotnet restore .\XC2Bf2SaveNETThingy\XC2Bf2SaveNETThingy.csproj --force-evaluate
        dotnet build .\XC2Bf2SaveNETThingy\XC2Bf2SaveNETThingy.csproj --configuration Release --no-restore
        dotnet restore .\XC2IraSaveNETThingy\XC2IraSaveNETThingy.csproj --force-evaluate
        dotnet build .\XC2IraSaveNETThingy\XC2IraSaveNETThingy.csproj --configuration Release --no-restore

    Expected: both projects reach C# compilation. Preserve the exact CS, MSB, and NU diagnostics before making source-level changes.

## Task 4: Remove packages.config and add the modern solution

Files: delete the three packages.config files; create XC2SaveNETThingy.slnx.

- [ ] Step 1: Confirm package declarations are project-local.

    Run:

        rg -n -uu 'PackageReference|packages.config|System.Data.SQLite.Core.targets|EnsureNuGetPackageBuildImports' --glob '*.csproj' --glob 'packages.config'

    Expected: each SDK project contains Be.Windows.Forms.HexBox and System.Data.SQLite.Core PackageReference entries, and no legacy targets import remains.

- [ ] Step 2: Delete exactly these files with apply_patch:

    XC2SaveNETThingy/packages.config
    XC2Bf2SaveNETThingy/packages.config
    XC2IraSaveNETThingy/packages.config

    Do not delete the ignored packages/ restore cache.

- [ ] Step 3: Generate and populate the .slnx.

    Run:

        dotnet new sln --name XC2SaveNETThingy --format slnx --force
        dotnet sln .\XC2SaveNETThingy.slnx add .\XC2Bf2SaveNETThingy\XC2Bf2SaveNETThingy.csproj .\XC2IraSaveNETThingy\XC2IraSaveNETThingy.csproj .\XC2SaveNETThingy\XC2SaveNETThingy.csproj
        dotnet sln .\XC2SaveNETThingy.slnx list

    Expected: the .slnx list contains exactly the three projects.

- [ ] Step 4: Verify the old solution still resolves the same projects.

    Run:

        dotnet sln .\XC2SaveNETThingy.sln list

    Expected: the old .sln reports the same three project paths.

## Task 5: Restore, build, and diagnose the complete .NET 10 solution

Files: only the project/config/source file identified by a concrete diagnostic.

- [ ] Step 1: Restore and inspect package assets.

    Run:

        dotnet restore .\XC2SaveNETThingy.slnx --force-evaluate
        Select-String -Path .\XC2SaveNETThingy\obj\project.assets.json, .\XC2Bf2SaveNETThingy\obj\project.assets.json, .\XC2IraSaveNETThingy\obj\project.assets.json -Pattern 'net10.0-windows|System.Data.SQLite|Be.Windows.Forms.HexBox|win-x64|win-x86'

    Expected: each assets file contains the net10.0-windows target; SQLite resolves its netstandard2.0 managed asset and Windows native runtime assets.

- [ ] Step 2: Run the full modern build.

    Run:

        dotnet build .\XC2SaveNETThingy.slnx --configuration Release --no-restore --verbosity minimal

    Expected success: exit code 0, all three projects built, and zero errors. NU1701 for the net40 HexBox package may remain as a package-compatibility warning.

- [ ] Step 3: Handle migration errors in root-cause order, one change at a time.

    Apply these exact rules:

    1. NETSDK1022 duplicate items: remove duplicated explicit items and keep SDK default items enabled.
    2. Duplicate assembly attributes: keep GenerateAssemblyInfo=false; do not delete AssemblyInfo.cs.
    3. Missing WinForms types or NETSDK1136: retain net10.0-windows and UseWindowsForms=true.
    4. Missing HexBox types: verify the PackageReference in both executable projects and the selected lib/net40 asset before considering a direct reference.
    5. Missing SQLite types: verify the PackageReference in all three projects and the selected netstandard2.0 asset; never restore the old net46 HintPath or targets import.
    6. Native SQLite startup errors: verify x86/x64 SQLite.Interop.dll files in Release output before changing code or package versions.
    7. A compiler error in handwritten source: reproduce it with a project-only build, make one minimal source edit, rebuild that project, and preserve the output proving the original error disappeared without unrelated behavior changes.

    Do not suppress warnings globally, upgrade dependencies, or refactor save-file code.

## Task 6: Use the net472 fallback only when .NET 10 is concretely blocked

Files: the three SDK project files and both App.config files, only if the .NET 10 build or startup check fails.

- [ ] Step 1: Record the blocker.

    Include the exact error code, assembly/package, project, and whether the failure happened during restore, compile, or startup. Do not enter this task for a warning-only build.

- [ ] Step 2: Change all three SDK targets.

    Replace:

        <TargetFramework>net10.0-windows</TargetFramework>

    with:

        <TargetFramework>net472</TargetFramework>

    Keep UseWindowsForms, PackageReference, local references, linked XC2Data.db, GenerateAssemblyInfo=false, and SDK default items unchanged.

- [ ] Step 3: Update the two framework startup declarations.

    In both App.config files, use:

        <startup>
          <supportedRuntime version="v4.0" sku=".NETFramework,Version=v4.7.2" />
        </startup>

    Preserve the base-game Entity Framework section unless it causes a demonstrated startup error.

- [ ] Step 4: Restore and build the fallback.

    Run:

        dotnet restore .\XC2SaveNETThingy.slnx --force-evaluate
        dotnet build .\XC2SaveNETThingy.slnx --configuration Release --no-restore --verbosity minimal

    Expected: all three SDK-style projects build using the installed v4.7.2 reference assemblies.

## Task 7: Run startup checks on both Release executables

Files: none unless startup identifies an approved configuration/dependency correction.

- [ ] Step 1: Verify runtime files.

    Run:

        Get-ChildItem -Recurse -File .\XC2Bf2SaveNETThingy\bin\Release, .\XC2IraSaveNETThingy\bin\Release | Where-Object { $_.Name -in @('XC2Data.db', 'SQLite.Interop.dll', 'DataGridViewNumericUpDownElements.dll', 'ZLIB.dll') } | Select-Object FullName,Length

    Expected: both executable trees contain XC2Data.db and the required managed/native dependencies.

- [ ] Step 2: Start each executable for five seconds and terminate it safely.

    For .NET 10 use targetFramework = 'net10.0-windows'; for the fallback use targetFramework = 'net472':

        $targetFramework = 'net10.0-windows'
        $apps = @(
          @{ Name = 'XC2Bf2SaveNETThingy'; Directory = '.\XC2Bf2SaveNETThingy' },
          @{ Name = 'XC2IraSaveNETThingy'; Directory = '.\XC2IraSaveNETThingy' }
        )

        foreach ($app in $apps) {
          $exe = Join-Path (Resolve-Path $app.Directory) ("bin\Release\$targetFramework\" + $app.Name + '.exe')
          if (-not (Test-Path -LiteralPath $exe)) {
            throw "Expected executable was not found: $exe"
          }
          $process = Start-Process -FilePath $exe -WorkingDirectory (Split-Path $exe) -PassThru
          try {
            Start-Sleep -Seconds 5
            if ($process.HasExited) {
              throw "$($app.Name) exited during startup with code $($process.ExitCode)."
            }
            Write-Output "$($app.Name) remained running for the startup window."
          }
          finally {
            if (-not $process.HasExited) {
              Stop-Process -Id $process.Id -Force
            }
          }
        }

    Expected: both programs remain alive for the startup window and are then terminated by the finally blocks.

## Task 8: Final verification and handoff

Files: none.

- [ ] Step 1: Build both solution formats.

    Run:

        dotnet build .\XC2SaveNETThingy.slnx --configuration Release --no-restore --verbosity minimal
        dotnet build .\XC2SaveNETThingy.sln --configuration Release --no-restore --verbosity minimal

    Expected: both commands exit 0 and report zero errors for all three projects. Report the actual final target framework.

- [ ] Step 2: Check the final diff.

    Run:

        git diff --check
        git status --short
        git diff --stat
        git diff --name-only

    Expected: only the approved solution/project/package/config files and evidence-based source fixes are changed. romdata/ remains untracked and unstaged.

- [ ] Step 3: Commit the implementation.

    Run:

        git add -- XC2SaveNETThingy.slnx XC2SaveNETThingy.sln XC2SaveNETThingy XC2Bf2SaveNETThingy XC2IraSaveNETThingy
        git diff --cached --check
        git commit -m "build: migrate solution to SDK-style projects"

    Never stage romdata/. If the fallback was used, include the concrete .NET 10 blocker in the commit body.
