#r @"packages/FAKE/tools/FakeLib.dll"
open Fake
open Fake.ReportGeneratorHelper
open Fake.OpenCoverHelper
open System
open System.Text

let NUnit3Defaults = 
    let toolname = "nunit3-console.exe"
    { IncludeCategory = ""
      ExcludeCategory = ""
      ToolPath = findToolFolderInSubPath toolname (currentDirectory @@ "tools" @@ "Nunit")
      ToolName = toolname
      DontTestInNewThread = false
      StopOnError = false
      OutputFile = currentDirectory @@ "TestResult.xml"
      Out = ""
      ErrorOutputFile = ""
      WorkingDir = ""
      Framework = ""
      ProcessModel = DefaultProcessModel
      ShowLabels = true
      XsltTransformFile = ""
      TimeOut = TimeSpan.FromMinutes 5.0
      DisableShadowCopy = false
      Domain = DefaultDomainModel
      ErrorLevel = Error 
      Fixture = ""}

/// Builds the command line arguments from the given parameter record and the given assemblies.
/// [omit]
let buildNUnitdArgs parameters assemblies = 
    new StringBuilder()
    |> append "--result=nunit2"
    |> appendFileNamesIfNotNull assemblies
    |> appendIfNotNullOrEmpty parameters.OutputFile "--out="
    |> toText

let NUnit3 (setParams : NUnitParams -> NUnitParams) (assemblies : string seq) =
    let details = assemblies |> separated ", "
    traceStartTask "NUnit" details
    let parameters = NUnit3Defaults |> setParams
    let assemblies = assemblies |> Seq.toArray
    if Array.isEmpty assemblies then failwith "NUnit: cannot run tests (the assembly list is empty)."
    let tool = parameters.ToolPath @@ parameters.ToolName
    let args = buildNUnitdArgs parameters assemblies
    trace (tool + " " + args)
    let result = 
        ExecProcess (fun info -> 
            info.FileName <- tool
            info.WorkingDirectory <- getWorkingDir parameters
            info.Arguments <- args) parameters.TimeOut
    sendTeamCityNUnitImport parameters.OutputFile
    let errorDescription error = 
        match error with
        | OK -> "OK"
        | TestsFailed -> sprintf "NUnit test failed (%d)." error
        | FatalError x -> sprintf "NUnit test failed. Process finished with exit code %s (%d)." x error
    match parameters.ErrorLevel with
    | DontFailBuild -> 
        match result with
        | OK | TestsFailed -> traceEndTask "NUnit" details
        | _ -> raise (FailedTestsException(errorDescription result))
    | Error | FailOnFirstError -> 
        match result with
        | OK -> traceEndTask "NUnit" details
        | _ -> raise (FailedTestsException(errorDescription result))