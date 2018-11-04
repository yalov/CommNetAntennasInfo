
@echo off

set GAMEPATH=c:\Users\User\Games\Kerbal Space Program 1.5.1
rem set GAMEPATH2=c:\Users\User\Games\Kerbal Space Program 1.3.1 rus


set MODNAME=CommNetAntennasExtension

echo %GAMEPATH%

REM copy dll and version to Rep/GameData
xcopy "%1%2" "GameData\%MODNAME%\Plugins\" /Y
xcopy "%MODNAME%.version" "GameData\%MODNAME%\" /Y 

REM copy dll and version in Rep/GameData to GAMEPATH
xcopy "GameData\%MODNAME%" "%GAMEPATH%\GameData\%MODNAME%\" /Y /S /I
rem xcopy "GameData\%MODNAME%" "%GAMEPATH2%\GameData\%MODNAME%\" /Y /S /I