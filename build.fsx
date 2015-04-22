#r @"packages/FAKE/tools/FakeLib.dll"
open Fake

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
             OutputFile = buildDir + "TestResults.xml" })
)

Target "Spec" (fun _ ->
    CreateDir "./packages/NUnit.Runners.2.6.3/tools/addins"

    Copy "./packages/NUnit.Runners.2.6.3/tools/addins" !! "./packages/Concordion.NET.1.2.0/tools/Concordion.NUnit.dll"

    !! (buildDir + "/*.Specs.dll")
      |> NUnit (fun p ->
          {p with
             DisableShadowCopy = true;
             OutputFile = buildDir + "SpecResults.xml" })
)

Target "Default" (fun _ ->
    trace "Hello World from FAKE"
)

"Clean"
  ==> "BuildApp"
  ==> "Test"
  ==> "Spec"
  ==> "Default"

RunTargetOrDefault "Default"