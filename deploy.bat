rmdir build /S /Q
msbuild StandUpTimer.sln /p:Configuration=Release /p:OutputPath=..\build

test

if errorlevel NEQ 0 exit

copy StandUpTimer\StandUpTimer.nuspec.template StandUpTimer\StandUpTimer.nuspec
for /f "delims=" %%a in ('tools\ReplaceVersionString\bin\debug\ReplaceVersionString build\StandUpTimer.exe StandUpTimer\StandUpTimer.nuspec $version$') do @set version=%%a
move StandUpTimer\StandUpTimer.nuspec build

del *.nupkg
packages\nuget.exe pack build\StandUpTimer.nuspec
move *.nupkg build

cd build
..\packages\squirrel.windows.0.5.5\tools\squirrel -releasify StandUpTimer.%version%.nupkg
cd..

set /p AzureKey=<azure.key
"C:\Program Files (x86)\Microsoft SDKs\Azure\AzCopy\AzCopy.exe" /Source:build\Releases /Dest:http://mufflonosoft.blob.core.windows.net/standuptimer /DestKey:%AzureKey% /S /XO /Y /NC:1

pause