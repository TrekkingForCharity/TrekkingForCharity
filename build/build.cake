#addin "Cake.Npm"
#tool "nuget:?package=GitVersion.CommandLine"
#tool "nuget:?package=OctopusTools&version=4.15.5"
#tool "nuget:?package=xunit.runner.console&version=2.2.0"
#addin nuget:?package=Cake.Coverlet

var target = Argument<string>("target", "Default");
GitVersion gitversion;
var buildPath = Directory("./build-artifacts");
var publishPath = buildPath + Directory("publish");
var releasePath = buildPath + Directory("release");
var coverPath = buildPath + Directory("cover");


Task("__Clean")
    .Does(() => {
        if (BuildSystem.IsLocalBuild) {
            CleanDirectories(new DirectoryPath[] {
                buildPath
            });

            CleanDirectories("../**/bin");
            CleanDirectories("../**/obj");
        }  

        CreateDirectory(releasePath);
        CreateDirectory(publishPath);
        CreateDirectory(coverPath);
    });

Task("__Versioning")
    .Does(() => {
        gitversion = GitVersion();        
        TeamCity.SetBuildNumber(gitversion.FullSemVer);
    });

Task("__RestorePackages")
    .Does(() => {
        var npmInstallSettings = new NpmInstallSettings();
        npmInstallSettings.FromPath("../Source/TrekkingForCharity.Web");
		npmInstallSettings.WithLogLevel(NpmLogLevel.Silent);
        NpmInstall(npmInstallSettings);
    });

Task("__Build")
    .Does(() => {
        var npmRunScriptSettings = new NpmRunScriptSettings{
           ScriptName = "release:build"
        };
		npmRunScriptSettings.WithLogLevel(NpmLogLevel.Silent);
        npmRunScriptSettings.FromPath("../Source/TrekkingForCharity.Web");
        NpmRunScript(npmRunScriptSettings);  

        var settings = new DotNetCoreBuildSettings {
            Configuration = "Release"
        };

        DotNetCoreBuild("../TrekkingForCharity.sln", settings);
    });
Task("__Test")
    .Does(() => {
        var path = MakeAbsolute(coverPath + File("results.xunit.xml"));
        var testSettings = new DotNetCoreTestSettings {
            Configuration = "Release" ,
            NoBuild = true,
            Logger = $"xunit;LogFilePath={path}"
        };

        var coverletSettings = new CoverletSettings {
            CollectCoverage = true,
            CoverletOutputFormat = (CoverletOutputFormat)8,
            CoverletOutputDirectory = coverPath,
            CoverletOutputName = "results"
        };

        DotNetCoreTest(File("../Tests/TrekkingForCharity.Tests/TrekkingForCharity.Tests.csproj"), testSettings, coverletSettings);
    });
Task("__Publish")
    .Does(() => {
        var pubSettings = new DotNetCorePublishSettings {
            Configuration = "Release",
            OutputDirectory = publishPath
        };
        
        DotNetCorePublish("../source/TrekkingForCharity.Web/TrekkingForCharity.Web.csproj", pubSettings);

            });
Task("__Package")
    .Does(() => {
        Zip(publishPath, releasePath + File("TrekkingForCharity.Web.zip"));        
    });

Task("Build")
    .IsDependentOn("__Clean")
    .IsDependentOn("__Versioning")
    .IsDependentOn("__RestorePackages")
    .IsDependentOn("__Build")
    .IsDependentOn("__Test")
    .IsDependentOn("__Publish")
    .IsDependentOn("__Package")
    ;

Task("Default")
    .IsDependentOn("Build");

RunTarget(target);