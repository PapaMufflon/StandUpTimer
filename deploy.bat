rmdir build /S /Q
msbuild StandUpTimer.sln /p:Configuration=Release /p:OutputPath=..\build

for /f "delims=" %%a in ('tools\ReplaceVersionString\bin\debug\ReplaceVersionString build\StandUpTimer.exe StandUpTimer\StandUpTimer.nuspec $version$') do @set version=%%a

del *.nupkg
packages\nuget.exe pack StandUpTimer\StandUpTimer.nuspec
move *.nupkg build

cd build
..\packages\squirrel.windows.0.5.5\tools\squirrel -releasify StandUpTimer.%version%.nupkg
copy Releases\*.* Y:\Austausch\tw\StandUpTimer
cd..

pause