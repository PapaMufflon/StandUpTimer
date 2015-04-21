call build.bat
if %ERRORLEVEL% NEQ 0 goto exit

call test.bat
if %ERRORLEVEL% NEQ 0 goto exit

call deployWindowsDesktopApp.bat
if %ERRORLEVEL% NEQ 0 goto exit

call pushDocumentation.bat
if %ERRORLEVEL% NEQ 0 goto exit

call deployWebApp.bat

:exit
pause