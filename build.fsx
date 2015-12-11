#r @"packages/FAKE/tools/FakeLib.dll"
open System
open Fake
open Fake.ReportGeneratorHelper
open Fake.OpenCoverHelper

#load "NUnit3.fsx"
open NUnit3

let buildDir = "./build"
let specsDir = "./build/specs"

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
  ==> "Spec"
  ==> "Default"

RunTargetOrDefault "Default"