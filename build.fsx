#r @"packages/FAKE/tools/FakeLib.dll"
open System
open Fake
open Fake.ReportGeneratorHelper
open Fake.OpenCoverHelper

RestorePackages()

let buildDir = "./build"

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

Target "Coverage" (fun _ ->
    for x in !! (buildDir + "/*.UnitTests.dll") do
        OpenCover (fun p ->
            { p with
                ExePath = "./packages/OpenCover.4.5.3723/OpenCover.Console.exe"
                TestRunnerExePath = "./packages/NUnit.Runners.2.6.3/tools/nunit-console.exe"
                Register = RegisterType.RegisterUser
                Output = buildDir + "/coverage.xml"
                Filter = "+[StandUpTimer*]* -[StandUpTimer*]StandUpTimer.Annotations*"})
            (x.ToString() + " /config:Release /noshadow /framework:net-4.5")

        ReportGenerator (fun p ->
            {p with
               ExePath = "./packages/ReportGenerator.2.1.4.0/ReportGenerator.exe"
               ReportTypes = [ReportGeneratorReportType.Html]
               TargetDir = buildDir + "/coverage/" + System.IO.Path.GetFileNameWithoutExtension(x)
               })
            [ buildDir + "/coverage.xml" ]
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

Target "Default" (fun _ ->
    trace "Have fun building the Stand-Up Timer!!!"
)

"Clean"
  ==> "BuildApp"
  ==> "Test"
  ==> "Coverage"
  ==> "Spec"
  ==> "Default"

RunTargetOrDefault "Default"