#r @"packages/FAKE/tools/FakeLib.dll"
open System
open System.IO
open Fake
open Fake.Git
open Fake.ReportGeneratorHelper
open Fake.OpenCoverHelper

#load "NUnit3.fsx"
open NUnit3

let buildDir = "./build"
let specsDir = "./build/specs"
let repoDir = "."

let replaceInsideFile file (stringToReplace:string) (stringToReplaceWith:string) =
    let content = File.ReadAllText file
    let modifiedContent = content.Replace(stringToReplace, stringToReplaceWith)

    File.WriteAllText(file, modifiedContent)

Target "Clean" (fun _ ->
    CleanDir buildDir
)

Target "BuildApp" (fun _ ->
    !! "./**/*.csproj"
      |> MSBuildRelease buildDir "Build"
      |> Log "AppBuild-Output: "
)

Target "BuildSpecs" (fun _ ->
    !! "./**/*.Specs.csproj"
      |> MSBuildRelease specsDir "Build"
      |> Log "AppBuild-Output: "
)

Target "BuildInstaller" (fun _ ->
    let version = (GetAssemblyVersion "build\\StandUpTimer.exe").ToString()
    let lastVersion = getLastTag()

    if ("v" + version).StartsWith(lastVersion) then failwith "this version and the last version is the same!"

    CopyFile "./StandUpTimer/StandUpTimer.nuspec" "./StandUpTimer/StandUpTimer.nuspec.template"
    replaceInsideFile "StandUpTimer\\StandUpTimer.nuspec" "$version$" version

    MoveFile buildDir "./StandUpTimer/StandUpTimer.nuspec"

    NuGetPackDirectly (fun p ->
        {p with
           WorkingDir = repoDir
           Version = version
           OutputPath = ".\\build"}) "./build/StandUpTimer.nuspec"

    let result =
        ExecProcess (fun info ->
            info.FileName <- "./packages/squirrel.windows/tools/squirrel.exe"
            info.Arguments <- "-releasify StandUpTimer." + version + ".nupkg"
            info.WorkingDirectory <- buildDir
        ) (TimeSpan.FromMinutes 1.)

    if result <> 0 then failwith "squirrel returned with a non-zero exit code"
)

Target "Test" (fun _ ->
    !! (buildDir + "/*.UnitTests.dll")
      |> NUnit3 (fun p ->
          {p with
             DisableShadowCopy = true;
             OutputFile = buildDir + "/TestResults.xml" })
)

Target "Coverage" (fun _ ->
    for x in !! (buildDir + "/*.UnitTests.dll") do
        OpenCover (fun p ->
            { p with
                ExePath = "./packages/OpenCover/tools/OpenCover.Console.exe"
                TestRunnerExePath = "./packages/NUnit.Console/tools/nunit3-console.exe"
                Register = RegisterType.RegisterUser
                Output = buildDir + "/coverage.xml"
                Filter = "+[StandUpTimer*]* -[StandUpTimer*]StandUpTimer.Annotations*"})
            (x.ToString() + " /config:Release /framework:net-4.5")

        ReportGenerator (fun p ->
            {p with
               ExePath = "./packages/ReportGenerator/tools/ReportGenerator.exe"
               ReportTypes = [ReportGeneratorReportType.Html]
               TargetDir = buildDir + "/coverage/" + System.IO.Path.GetFileNameWithoutExtension(x)
               })
            [ buildDir + "/coverage.xml" ]
)

Target "Spec" (fun _ ->
    CreateDir "./packages/concordion/NUnit/tools/addins"

    CopyFile "./packages/NUnit.Console/tools/addins" "./packages/concordion/Concordion.NET/tools/Concordion.NUnit.dll"

    !! (specsDir + "/*.Specs.dll")
      |> NUnit3 (fun p ->
          {p with
             DisableShadowCopy = true;
             OutputFile = specsDir + "/SpecResults.xml" })
)

Target "Default" (fun _ ->
    trace "Have fun building the Stand-Up Timer!!!"
)

"Clean"
  ==> "BuildApp"
  ==> "Test"
  ==> "Coverage"
  ==> "BuildSpecs"
  ==> "BuildInstaller"
  ==> "Spec"
  ==> "Default"

RunTargetOrDefault "Default"