@echo off
cls
rem user build config: %1
rem system source dir: %2
rem user source dir: %3

echo Building libs for Windows XP

if not exist "..\configs\xp\lib" echo Windows XP build configurations folder not found! & goto all_done
if not exist "..\src" echo Sources folder "..\src" not found! & goto all_done  

pushd xp
for /R "..\..\configs\xp\lib" %%A in ("*.cfg") do (call vc_build_lib %%~nA "..\..\src")
popd


:all_done 
