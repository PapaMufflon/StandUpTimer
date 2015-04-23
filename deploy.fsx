#r @"packages/FAKE/tools/FakeLib.dll"
open System
open System.IO
open Fake
open Fake.Git
open Fake.NuGet

let buildDir = "./build"
let docDir = "./doc"

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

Target "DeployWebApp" (fun _ ->
    pushBranch "." "azure" "master"
)

Target "Default" (fun _ ->
    trace "Have fun deploying the Stand-Up Timer!!!"
)

"DeployWindowsDesktopApp"
  ==> "UpdateDocumentation"
  ==> "Default"

RunTargetOrDefault "Default"