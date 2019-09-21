#load nuget:https://pkgs.dev.azure.com/dotnet/ReactiveUI/_packaging/ReactiveUI/nuget/v3/index.json?package=ReactiveUI.Cake.Recipe&prerelease

Environment.SetVariableNames();

// Whitelisted Packages
var packageWhitelist = new[] 
{ 
    MakeAbsolute(File("./src/ReactiveUI.Validation/ReactiveUI.Validation.csproj")),
};

var packageTestWhitelist = new[]
{
    MakeAbsolute(File("./src/ReactiveUI.Validation.Tests/ReactiveUI.Validation.Tests.csproj")),
};

BuildParameters.SetParameters(context: Context, 
                            buildSystem: BuildSystem,
                            title: "ReactiveUI.Validation",
                            whitelistPackages: packageWhitelist,
                            whitelistTestPackages: packageTestWhitelist,
                            artifactsDirectory: "./artifacts",
                            sourceDirectory: "./src");

ToolSettings.SetToolSettings(context: Context);

Build.Run();
