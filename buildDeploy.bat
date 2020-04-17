
@echo off
echo BUILDDEPLOY.BAT

rem v2
rem instruction in the BUILDRELEASE.BAT

rem Set variables here
set GAMEPATH=c:\Users\User\Games\Kerbal Space Program 1.9.1
set MODNAME=CommNetAntennasInfo
set EXTRAS=Extras-Consumptor

REM copying dll and .version to Repository/GameData
REM /Y	Suppresses prompting to confirm that you want to overwrite an existing destination file.
REM /Q	Suppresses the display of files
REM /I	Suppresses prompting to specify whether Destination is a file or a directory
xcopy "%1%2"              "GameData\%MODNAME%\Plugins\" /Y /I /Q
xcopy "%MODNAME%.version" "GameData\%MODNAME%\"         /Y /I /Q

REM copying Repository/GameData to GAMEPATH
REM mkdir "%GAMEPATH%\GameData\%MODNAME%"
REM /S	recursive copuing un-empty directories

xcopy "GameData\%MODNAME%" "%GAMEPATH%\GameData\%MODNAME%\" /Y /I /Q /S
xcopy "GameData\%EXTRAS%\%MODNAME%" "%GAMEPATH%\GameData\%MODNAME%\" /Y /I /Q /S
