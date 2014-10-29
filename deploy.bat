rmdir build /S /Q
msbuild StandUpTimer.sln /p:Configuration=Release /p:OutputPath=..\build

copy StandUpTimer\StandUpTimer.nuspec.template StandUpTimer\StandUpTimer.nuspec
for /f "delims=" %%a in ('tools\ReplaceVersionString\bin\debug\ReplaceVersionString build\StandUpTimer.exe StandUpTimer\StandUpTimer.nuspec $version$') do @set version=%%a
move StandUpTimer\StandUpTimer.nuspec build

del *.nupkg
packages\nuget.exe pack build\StandUpTimer.nuspec
move *.nupkg build

cd build
..\packages\squirrel.windows.0.5.5\tools\squirrel -releasify StandUpTimer.%version%.nupkg
copy Releases\*.* Y:\Austausch\tw\StandUpTimer
cd..

pause