#module nuget:?package=Cake.DotNetTool.Module&version=0.3.1
#addin "Cake.Npm&version=0.17.0"
#addin "nuget:?package=Cake.Sonar"
#tool "nuget:?package=MSBuild.SonarQube.Runner.Tool"
#tool "nuget:?package=xunit.runner.console&version=2.2.0"
#addin nuget:?package=Cake.Coverlet&version=2.3.4

var target = Argument<string>("target", "Default");
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

Task("__StartAnalysis")
    .Does(() => {
        SonarBegin(new SonarBeginSettings{
         Name = "trekking-for-charity",
         Key = "TrekkingForCharity",
         Url = "https://sonarcloud.io",
         OpenCoverReportsPath= "../**/results.opencover.xml"  
      });
    });
Task("__RestoreServer")
    .Does(() => {
        DotNetCoreRestore("../TrekkingForCharity.sln");
    });

Task("__RestoreClient")
    .Does(() => {
        var npmInstallSettings = new NpmInstallSettings{
            WorkingDirectory = Directory("../source/TrekkingForCharity.Web")
        };
        
		NpmInstall(npmInstallSettings);
    });

Task("__BuildServer")
    .Does(() => {
        var dotNetCoreBuildSettings = new DotNetCoreBuildSettings {
            Configuration = "Release"
        };

        DotNetCoreBuild("../TrekkingForCharity.sln", dotNetCoreBuildSettings);     
    });

Task("__BuildClient")
    .Does(() => {
        var npmRunScriptSettings = new NpmRunScriptSettings{
           ScriptName = "release:build",
           WorkingDirectory = Directory("../source/TrekkingForCharity.Web")
        };
		NpmRunScript(npmRunScriptSettings);
    });

Task("__Test")
    .Does(() => {
        var path = MakeAbsolute(coverPath + File("results.xunit.xml"));
        var testSettings = new DotNetCoreTestSettings {
            Configuration = "Release" ,
            NoBuild = true,
            Logger = $"xunit;LogFilePath={path}",
            ArgumentCustomization = args => args.Append("--no-restore"),
        };

        var coverletSettings = new CoverletSettings {
            CollectCoverage = true,
            CoverletOutputFormat = (CoverletOutputFormat)12,
            CoverletOutputDirectory = coverPath,
            CoverletOutputName = "results"
        };

        DotNetCoreTest(File("../tests/TrekkingForCharity.Tests/TrekkingForCharity.Tests.csproj"), testSettings, coverletSettings);
    });

Task("__Publish")
    .Does(() => {
        DotNetCorePublish(
            "../source/TrekkingForCharity.Web/TrekkingForCharity.Web.csproj",
            new DotNetCorePublishSettings()
            {
                Configuration = "Release",
                OutputDirectory = publishPath,
                ArgumentCustomization = args => args.Append("--no-restore"),
            });
    });
Task("__EndAnalysis")
    .Does(() => {
     
        
        SonarEnd(new SonarEndSettings()));
  
    });

Task("Build")
    .IsDependentOn("__Clean")
    .IsDependentOn("__RestoreServer")
    .IsDependentOn("__RestoreClient")
    .IsDependentOn("__BuildServer")
    .IsDependentOn("__BuildClient")
    .IsDependentOn("__Test")
    .IsDependentOn("__Publish")
    ;
Task("Clean")
    .IsDependentOn("__Clean")
    ;

Task("Publish")
    .IsDependentOn("__StartAnalysis")
    .IsDependentOn("__RestoreServer")
    .IsDependentOn("__BuildServer")
    .IsDependentOn("__Test")
    .IsDependentOn("__Publish")
    .IsDependentOn("__EndAnalysis")
    ;


Task("Default")
    .IsDependentOn("Build");

RunTarget(target);