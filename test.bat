rmdir build /S /Q
msbuild StandUpTimer.sln /p:Configuration=Release /p:OutputPath=..\build

packages\NUnit.Runners.2.6.3\tools\nunit-console.exe build\StandUpTimer.UnitTests.dll