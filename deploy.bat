call build.bat
if %ERRORLEVEL% NEQ 0 goto exit

call test.bat
if %ERRORLEVEL% NEQ 0 goto exit

copy StandUpTimer\StandUpTimer.nuspec.template StandUpTimer\StandUpTimer.nuspec
for /f "delims=" %%a in ('tools\ReplaceVersionString\bin\debug\ReplaceVersionString build\StandUpTimer.exe StandUpTimer\StandUpTimer.nuspec $version$') do @set version=%%a
move StandUpTimer\StandUpTimer.nuspec build

del *.nupkg
packages\nuget.exe pack build\StandUpTimer.nuspec
move *.nupkg build

cd build
..\packages\squirrel.windows.0.6.0.1\tools\squirrel -releasify StandUpTimer.%version%.nupkg
cd..

set /p AzureKey=<azure.key
"C:\Program Files (x86)\Microsoft SDKs\Azure\AzCopy\AzCopy.exe" /Source:build\Releases /Dest:http://mufflonosoft.blob.core.windows.net/standuptimer /DestKey:%AzureKey% /S /XO /Y /NC:1

:exit
pause