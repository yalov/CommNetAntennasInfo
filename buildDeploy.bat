
@echo off

set GAMEPATH=c:\Users\User\Games\Kerbal Space Program 1.6.0

set MODNAME=CommNetAntennasInfo

echo %GAMEPATH%

rem copy dll and version to Rep/GameData
xcopy "%1%2" "GameData\%MODNAME%\Plugins\" /Y
xcopy "%MODNAME%.version" "GameData\%MODNAME%\" /Y 

rem replace mod folder from Rep/GameData to GAMEPATH
rmdir "%GAMEPATH%\GameData\%MODNAME%" /s /q
xcopy "GameData\%MODNAME%" "%GAMEPATH%\GameData\%MODNAME%\" /y /s /i


rem pause

rem start /d "%GAMEPATH%" KSP_x64.exe