#r @"packages/FAKE/tools/FakeLib.dll"
open System
open System.IO
open Fake
open Fake.Git
open Fake.NuGet

RestorePackages()

let buildDir = "./build"
let docDir = "./doc"

Target "Clean" (fun _ ->
    CleanDir buildDir
)

Target "BuildApp" (fun _ ->
    !! "./**/*.csproj"
      |> MSBuildRelease buildDir "Build"
      |> Log "AppBuild-Output: "
)

Target "Test" (fun _ ->
    !! (buildDir + "/*.UnitTests.dll")
      |> NUnit (fun p ->
          {p with
             DisableShadowCopy = true;
             OutputFile = buildDir + "/TestResults.xml" })
)

Target "Spec" (fun _ ->
    CreateDir "./packages/NUnit.Runners.2.6.3/tools/addins"

    CopyFile "./packages/NUnit.Runners.2.6.3/tools/addins" "./packages/Concordion.NET.1.2.0/tools/Concordion.NUnit.dll"

    !! (buildDir + "/*.Specs.dll")
      |> NUnit (fun p ->
          {p with
             DisableShadowCopy = true;
             OutputFile = buildDir + "/SpecResults.xml" })
)

Target "DeployWindowsDesktopApp" (fun _ ->
    CopyFile "./StandUpTimer/StandUpTimer.nuspec" "./StandUpTimer/StandUpTimer.nuspec.template"

    let version =
        ExecProcess(fun info ->
            info.FileName <- "./tools/ReplaceVersionString/bin/debug/ReplaceVersionString"
            info.Arguments <- "build\\StandUpTimer.exe StandUpTimer\\StandUpTimer.nuspec"
        ) (TimeSpan.FromSeconds 10.)

    MoveFile buildDir "./StandUpTimer/StandUpTimer.nuspec"

    NuGetPackDirectly (fun p ->
        {p with
           OutputPath = ".\\build"}) "./build/StandUpTimer.nuspec"

    let result =
        ExecProcess (fun info ->
            info.FileName <- "./packages/squirrel.windows.0.9.3/tools/squirrel.exe"
            info.Arguments <- "-releasify StandUpTimer." + version.ToString() + ".nupkg -r build -p build"
        ) (TimeSpan.FromMinutes 1.)

    if result <> 0 then failwith "squirrel returned with a non-zero exit code"

    let azureKey = File.ReadAllText "./azure.key"

    let result =
        ExecProcess (fun info ->
            info.FileName <- "C:/Program Files (x86)/Microsoft SDKs/Azure/AzCopy/AzCopy.exe"
            info.Arguments <- "/Source:build\\Releases /Dest:http://mufflonosoft.blob.core.windows.net/standuptimer /DestKey:" + azureKey + " /S /XO /Y /NC:1"
        ) (TimeSpan.FromMinutes 5.)

    if result <> 0 then failwith "azCopy returned with a non-zero exit code"
)

Target "UpdateDocumentation" (fun _ ->
    CleanDir docDir

    cloneSingleBranch docDir "https://github.com/PapaMufflon/StandUpTimer.git" "gh-pages" docDir

    CopyFile (docDir + "/StandUpTimer") (buildDir + "/results/StandUpTimer/Specs/Index.html")

    StageAll docDir

    Commit docDir "Updated documentation"

    push docDir
)

Target "Default" (fun _ ->
    trace "Have fun building the Stand-Up Timer!!!"
)

"Clean"
  ==> "BuildApp"
  ==> "Test"
  ==> "Spec"
  ==> "DeployWindowsDesktopApp"
  ==> "UpdateDocumentation"
  ==> "Default"

RunTargetOrDefault "Default"