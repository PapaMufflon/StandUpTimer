cd build
..\packages\NUnit.Runners.2.6.3\tools\nunit-console.exe StandUpTimer.UnitTests.dll
if %ERRORLEVEL% NEQ 0 goto exit

md ..\packages\NUnit.Runners.2.6.3\tools\addins
copy ..\packages\Concordion.NET.1.2.0\tools\Concordion.NUnit.dll ..\packages\NUnit.Runners.2.6.3\tools\addins

..\packages\NUnit.Runners.2.6.3\tools\nunit-console.exe StandUpTimer.Specs.dll
if %ERRORLEVEL% NEQ 0 goto exit

:exit
cd ..