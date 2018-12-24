
@echo off

rem Put the following text into the Post-build event command line:
rem without the "rem":

rem start /D $(SolutionDir) /WAIT buildDeploy.bat  $(TargetDir) $(TargetFileName)
rem  
rem if $(ConfigurationName) == Release (
rem  
rem start /D $(SolutionDir) /WAIT buildRelease.bat $(TargetDir) $(TargetFileName)
rem  
rem )

rem Set variables here

set GAMEDIR=CommNetAntennasInfo
set GAMEDATA="GameData\"
set VERSIONFILE=%GAMEDIR%.version
set LICENSE=%GAMEDIR%-License.txt
set CHANGELOG=ChangeLog.txt

set RELEASEDIR=Releases
set ZIP="c:\Program Files\7-zip\7z.exe"

rem Copy files to GameData locations

copy /Y "%1%2" "%GAMEDATA%\%GAMEDIR%\Plugins"
copy /Y %VERSIONFILE% %GAMEDATA%\%GAMEDIR%

if "%LICENSE%" NEQ "" copy /y  %LICENSE% %GAMEDATA%\%GAMEDIR%
if "%README%" NEQ "" copy /Y %README% %GAMEDATA%\%GAMEDIR%
if "%CHANGELOG%" NEQ "" copy /Y %CHANGELOG% %GAMEDATA%\%GAMEDIR%

rem Get Version info

copy %VERSIONFILE% tmp.version
set VERSIONFILE=tmp.version
rem The following requires the JQ program, available here: https://stedolan.github.io/jq/download/
set JD=c:\Users\User\Games\Development\jq-win64

%JD% ".VERSION.MAJOR" %VERSIONFILE% >tmpfile
set /P major=<tmpfile

%JD% ".VERSION.MINOR"  %VERSIONFILE% >tmpfile
set /P minor=<tmpfile

%JD% ".VERSION.PATCH"  %VERSIONFILE% >tmpfile
set /P patch=<tmpfile

%JD% ".VERSION.BUILD"  %VERSIONFILE% >tmpfile
set /P build=<tmpfile
del tmpfile
del tmp.version
set VERSION=%major%.%minor%.%patch%
if "%build%" NEQ "0"  set VERSION=%VERSION%.%build%

echo Version:  %VERSION%


rem Build the zip FILE
cd %GAMEDATA%\..

set FILE="%RELEASEDIR%\%GAMEDIR%-v%VERSION%.zip"
IF EXIST %FILE% del /F %FILE%
%ZIP% a -tzip %FILE% GameData

pause
