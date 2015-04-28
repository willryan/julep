#r @"./packages/FAKE/tools/FakeLib.dll"

open Fake

Target "build" (fun _ ->
  MSBuildDebug "" "Build" [ "Julep.fsproj" ]
  |> Log "TestBuild-Output: "
)


Target "test" (fun _ -> 
  let testDlls = !! ("test/bin/Debug/*Tests.dll")
  testDlls 
  |> NUnit (fun p -> 
    {p with
       DisableShadowCopy = true; 
       OutputFile = "test/TestResults.xml"})
)

Target "spec" (fun _ -> 
  // can't find file for some reason, or a mono issue
  let canopyExe = "spec/bin/Debug/Julep.Specs.exe"
  let result = 
    ExecProcess (fun info ->
      info.FileName <- canopyExe
      info.WorkingDirectory <- "spec"
    ) (System.TimeSpan.FromMinutes 5.)

  //ProcessHelper.killProcessById webSiteProcess.Id

  if result <> 0 then failwith "Failed result from canopy tests"
)

RunTargetOrDefault "build"
