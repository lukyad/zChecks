@echo off

REM Run this script from the root of zChecks (the current directory should
REM contain Eco.sln). It will build the whole solution and push new
REM versions of zChecks to nuget.org.

set DEVENV="C:\Program Files (x86)\Microsoft Visual Studio\2017\Professional\Common7\IDE\devenv.exe"

%DEVENV% zChecks.sln /rebuild Release || goto :error

echo Coping zChecks
robocopy zChecks\bin\Release nuget\lib\net40 *.dll 

echo Coping PostBuildUtil
robocopy PostBuildUtil\bin\Release nuget\tools *.dll *.exe 

cd nuget
if exist *.nupkg (del *.nupkg || goto :error)
nuget pack zChecks.nuspec || goto :error
rem nuget push *.nupkg || goto :error
cd .. || goto :error

goto :EOF
:error
echo Failed with error #%errorlevel%.
exit /b %errorlevel%
