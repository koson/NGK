@echo off

rem user build config: %1
rem system source dir: %2
rem user source dir: %3

rem Build configuration name not specified
if "%1"=="" echo Please specify build configuration & goto build_failed
rem Sources dir not specified 
if "%2"=="" echo Please specify the sources folder & goto build_failed
rem Sources dir not exist
if not exist "%2" echo Source folder "%2" NOT exist! & goto build_failed
if "%3" == "" goto check_bld_cfg
if not exist "%3" echo User source folder "%3" NOT exist! & goto build_failed


:check_bld_cfg
rem Build configuration configuration file not exist
if not exist "..\..\configs\xp\lib\%1.cfg" echo Build configuration file "..\..\configs\xp\lib\%1.cfg" not found & goto build_failed

echo ===================================================================
echo Building configuration: %1
echo ===================================================================

rem if exist "..\..\bin\xp\%1.dll" del "..\..\bin\xp\%1.dll"
rem if exist "..\..\lib\xp\%1.lib" del "..\..\lib\xp\%1.lib"

set f_vc_path=""
set /a f_vc_ver=0
rem Check for the MSVC existance, result in f_vc_path
call vc_chk.bat

if %f_vc_path% == "" echo MS Visual C++ tools not found! & goto build_failed 
echo VC Tools version: %f_vc_ver%

set f_vc_cfg="vcx_lib.cfg"
set /a f_vc_manifest = 0
if "%f_vc_ver%"=="90" set /a f_vc_manifest=1 & goto tool_selected
if "%f_vc_ver%"=="80" set /a f_vc_manifest=1 & goto tool_selected
if "%f_vc_ver%"=="71" goto tool_selected
if "%f_vc_ver%"=="70" goto tool_selected
if "%f_vc_ver%"=="60" goto tool_selected

echo Build tools configuration NOT found!
goto build_failed

:tool_selected

echo Compiling tool config in: %f_vc_cfg%
if not exist %f_vc_cfg% echo No compiling tool configuration file found! & goto build_failed

rem Create intermediate directory if it ain't exist
if exist "..\..\intdir" goto enum_sources
mkdir "..\..\intdir"

:enum_sources
rem Delete previous response files if they exist out there
if exist "..\..\intdir\clnk.rsp" del "..\..\intdir\clnk.rsp"
if exist "..\..\intdir\ccmpl.rsp" del "..\..\intdir\ccmpl.rsp"
if exist "..\..\intdir\cmt.rsp" del "..\..\intdir\cmt.rsp"
rem Cleaning the configuration intermediate dir
if exist "..\..\intdir\%1_lib" del /Q ..\..\intdir\%1_lib\*.* & goto lnk_rsp 
mkdir ..\..\intdir\%1_lib


:lnk_rsp
echo Creating RSP file for the source dir:
echo %2

rem c_file_count is a number of c/cpp input files
set /a c_file_count = 0
rem Iterating recursively through the sources dir (%2)
rem and create the linker response file
for /R "%2" %%A in ("*.c","*.cpp") do (echo "..\..\intdir\%1_lib\"%%~nA.obj >> "..\..\intdir\clnk.rsp" & set /a c_file_count += 1)

if "%3"=="" goto rsp_end
rem Iterating recursively through the user sources dir (%3)
for /R "%3" %%A in ("*.c","*.cpp") do (echo "..\..\intdir\%1_lib\"%%~nA.obj >> "..\..\intdir\clnk.rsp" & set /a c_file_count += 1)

:rsp_end

echo Source files processed: %c_file_count%

if "%c_file_count%"=="0" echo Source files not found in "%2"! & goto build_failed


if "%3"=="" goto cl_rsp
rem Add user source dir to the include path
echo /I "%3" >> "..\..\intdir\ccmpl.rsp"

:cl_rsp
echo /I "%2" >> "..\..\intdir\ccmpl.rsp"

rem Get user options for compiler
findstr /b "COMPILE=" "..\..\configs\xp\lib\%1.cfg" > "..\..\intdir\cluopt.tmp"
if "%errorlevel%" neq "0" echo Build configuration file (..\..\configs\xp\lib\%1.cfg) doesn't consist of user options COMPILE= & goto build_failed
for /f "tokens=2* delims==" %%A in (..\..\intdir\cluopt.tmp) do (echo %%A >> "..\..\intdir\ccmpl.rsp")

rem Get system options for compiler
findstr /b "COMPILE=" %f_vc_cfg% > "..\..\intdir\clsopt.tmp"
if "%errorlevel%" neq "0" echo System config file (%f_vc_cfg%) doesn't consist of options for COMPILE= & goto build_failed
for /f "tokens=2* delims==" %%A in (..\..\intdir\clsopt.tmp) do (echo %%A >> "..\..\intdir\ccmpl.rsp")

echo  /Fo"..\..\intdir\%1_lib\\" >> "..\..\intdir\ccmpl.rsp"
echo  /Fd"..\..\intdir\%1_lib\%1.pdb" >> "..\..\intdir\ccmpl.rsp"

echo Setting MSVC environment (%f_vc_path%\vcvars32.bat)...
call %f_vc_path%\vcvars32.bat

rem Iterating recursively through the sources dir (%2) and compile c/cpp
set /a c_built_files = 0
for /R "%2" %%A in ("*.c","*.cpp") do (call vccc.bat "%%A" "..\..\intdir\ccmpl.rsp")

if "%3"=="" goto cmpl_end
rem Iterating recursively through the user sources dir (%3) and compile c/cpp
for /R "%3" %%A in ("*.c","*.cpp") do (call vccc.bat "%%A" "..\..\intdir\ccmpl.rsp")
:cmpl_end

if "%c_built_files%" neq "%c_file_count%" echo MS Visual C++ build failed (files total: %c_file_count%, files built: %c_built_files%)! & goto build_failed

echo Compilation succeeded, files built: %c_built_files%

echo Linking %1

echo /OUT:"..\..\intdir\%1_lib\%1.dll" >> "..\..\intdir\clnk.rsp"
if "%f_vc_manifest%"=="0" goto skip_manifest_opts
echo /MANIFEST /MANIFESTFILE:"..\..\intdir\%1_lib\%1.manifest" /MANIFESTUAC:"level='asInvoker' uiAccess='false'" >> "..\..\intdir\clnk.rsp"
:skip_manifest_opts

rem Get system options for linker
findstr /b "LINK=" %f_vc_cfg% > "..\..\intdir\lnksopt.tmp"
if "%errorlevel%" neq "0" echo System config file (%f_vc_cfg%) doesn't consist of options for LINK= & goto build_failed
for /f "tokens=2* delims==" %%A in (..\..\intdir\lnksopt.tmp) do (echo %%A >> "..\..\intdir\clnk.rsp")

rem Get user options for linker
findstr /b "LINK=" "..\..\configs\xp\lib\%1.cfg" > "..\..\intdir\lnkuopt.tmp"
if "%errorlevel%" neq "0" echo System config file (..\..\configs\xp\lib\%1.cfg) doesn't consist of options for LINK= & goto build_failed
for /f "tokens=2* delims==" %%A in (..\..\intdir\lnkuopt.tmp) do (echo %%A >> "..\..\intdir\clnk.rsp")

%f_vc_path%\link @"..\..\intdir\clnk.rsp"

if "%errorlevel%" neq "0" goto build_failed 

if "%f_vc_manifest%"=="0" goto skip_embedding_manifest
echo Embedding manifest...
echo /nologo /outputresource:"..\..\intdir\%1_lib\%1.dll;#2" /manifest "..\..\intdir\%1_lib\%1.manifest" >> "..\..\intdir\cmt.rsp"
mt.exe @"..\..\intdir\cmt.rsp" 
if "%errorlevel%" neq "0" echo Manifest Tool error(%errorlevel%) & goto build_failed

:skip_embedding_manifest

echo Linker produced "..\..\intdir\%1_lib\%1.dll" and "..\..\intdir\%1_lib\%1.lib" successfully 

findstr /b "POSTBUILD=" "..\..\configs\xp\lib\%1.cfg" > "..\..\intdir\postbuopt.tmp"
for /f "tokens=2* delims==" %%A in (..\..\intdir\postbuopt.tmp) do (call %%A %CD%)

rem ==== COPY files to the  deployment folder

rem echo Copying %1.dll to the deployment folder
rem if not exist "..\..\bin\xp" mkdir "..\..\bin\xp"
rem copy /Y "..\..\intdir\%1_lib\%1.dll" "..\..\bin\xp"

rem echo Copying %1.lib to the lib folder
rem if not exist "..\..\lib\xp" mkdir "..\..\lib\xp"
rem copy /Y "..\..\intdir\%1_lib\%1.lib" "..\..\lib\xp"

goto all_done

echo Link failed! "..\..\intdir\%1_lib\%1.dll" has not been produced!

:build_failed
echo Configuration %1 BUILD FAILED!!!

:all_done

rem Delete response and tmp files if they exist out there
if exist "..\..\intdir\ccmpl.rsp" del "..\..\intdir\ccmpl.rsp"
if exist "..\..\intdir\clnk.rsp" del "..\..\intdir\clnk.rsp"
if exist "..\..\intdir\cmt.rsp" del "..\..\intdir\cmt.rsp"
if exist "..\..\intdir\clsopt.tmp" del "..\..\intdir\clsopt.tmp"
if exist "..\..\intdir\cluopt.tmp" del "..\..\intdir\cluopt.tmp"
if exist "..\..\intdir\lnksopt.tmp" del "..\..\intdir\lnksopt.tmp"
if exist "..\..\intdir\lnkuopt.tmp" del "..\..\intdir\lnkuopt.tmp"
if exist "..\..\intdir\postbuopt.tmp" del "..\..\intdir\postbuopt.tmp"

