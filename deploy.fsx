#r @"packages/FAKE/tools/FakeLib.dll"
open System
open System.IO
open Fake
open Fake.Git
open Fake.NuGet

let buildDir = "./build"
let docDir = "./doc"
let repoDir = "."

Target "DeployWebApp" (fun _ ->
    pushBranch repoDir "azure" "master"
)

Target "ReleasifyWindowsDesktopApp" (fun _ ->
    CopyFile "./StandUpTimer/StandUpTimer.nuspec" "./StandUpTimer/StandUpTimer.nuspec.template"

    let result =
        ExecProcess(fun info ->
            info.FileName <- "./tools/ReplaceVersionString/bin/debug/ReplaceVersionString"
            info.Arguments <- "build\\StandUpTimer.exe StandUpTimer\\StandUpTimer.nuspec $version$"
        ) (TimeSpan.FromSeconds 10.)

    let version = File.ReadAllText "./version"

    MoveFile buildDir "./StandUpTimer/StandUpTimer.nuspec"

    NuGetPackDirectly (fun p ->
        {p with
           WorkingDir = repoDir
           Version = version.ToString()
           OutputPath = ".\\build"}) "./build/StandUpTimer.nuspec"

    let result =
        ExecProcess (fun info ->
            info.FileName <- "./packages/squirrel.windows.0.9.3/tools/squirrel.exe"
            info.Arguments <- "-releasify StandUpTimer." + version.ToString() + ".nupkg"
            info.WorkingDirectory <- buildDir
        ) (TimeSpan.FromMinutes 1.)

    if result <> 0 then failwith "squirrel returned with a non-zero exit code"
)

Target "DeployWindowsDesktopAppToAzure" (fun _ ->
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

    cloneSingleBranch repoDir "https://github.com/PapaMufflon/StandUpTimer.git" "gh-pages" docDir

    CopyFile (docDir + "concordion-logo.png") (buildDir + "/results/image/concordion-logo.png")
    let indexHtml = docDir + "/Index.html"
    CopyFile indexHtml (buildDir + "/results/StandUpTimer/Specs/Index.html")

    let index = File.ReadAllText indexHtml
    let modifiedIndex = index.Replace("..\..\image\concordion-logo.png", "concordion-logo.png")
    File.WriteAllText(indexHtml, modifiedIndex)

    StageAll docDir

    Commit docDir "Updated documentation"

    push docDir
)

Target "PushToOrigin" (fun _ ->
    pushBranch repoDir "origin" "master"
)

Target "Tag" (fun _ ->
    let version = GetAssemblyVersion "build\\StandUpTimer.exe"
    let versionTag = "v" + version.ToString()

    tag repoDir ("-a " + versionTag)

    pushTag repoDir "origin" versionTag
)

Target "Default" (fun _ ->
    trace "Have fun deploying the Stand-Up Timer!!!"
)

"DeployWebApp"
  ==> "ReleasifyWindowsDesktopApp"
  ==> "DeployWindowsDesktopAppToAzure"
  ==> "UpdateDocumentation"
  ==> "PushToOrigin"
  ==> "Tag"
  ==> "Default"

RunTargetOrDefault "Default"