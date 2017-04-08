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
if not exist "..\..\configs\xp\drv\%1.cfg" echo Build configuration file "..\..\configs\xp\drv\%1.cfg" not found & goto build_failed

echo ===================================================================
echo Building configuration: %1
echo ===================================================================

rem if exist "..\..\bin\xp\%1.sys" del "..\..\bin\xp\%1.sys"

if not exist %DDKPATH% echo DDK not found! & goto build_failed 
if not exist "drv.cfg" Build configuration NOT found! & goto build_failed 

rem Create intermediate directory if it ain't exist
if exist "..\..\intdir" goto enum_sources
mkdir "..\..\intdir"

:enum_sources

set f_vc_path="%DDKPATH%\bin\x86"

rem Delete previous response files if they exist out there
if exist "..\..\intdir\clnk.rsp" del "..\..\intdir\clnk.rsp"
if exist "..\..\intdir\ccmpl.rsp" del "..\..\intdir\ccmpl.rsp"
rem Cleaning the configuration intermediate dir
if exist "..\..\intdir\%1_sys" del /Q ..\..\intdir\%1_sys\*.* & goto lnk_rsp 
mkdir ..\..\intdir\%1_sys

:lnk_rsp
echo Creating RSP file for the source dir:
echo %2

rem c_file_count is a number of c/cpp input files
set /a c_file_count = 0
rem Iterating recursively through the sources dir (%2)
rem and create the linker response file
for /R "%2" %%A in ("*.c","*.cpp") do (echo "..\..\intdir\%1_sys\"%%~nA.obj >> "..\..\intdir\clnk.rsp" & set /a c_file_count += 1)

if "%3"=="" goto rsp_end
rem Iterating recursively through the user sources dir (%3)
for /R "%3" %%A in ("*.c","*.cpp") do (echo "..\..\intdir\%1_sys\"%%~nA.obj >> "..\..\intdir\clnk.rsp" & set /a c_file_count += 1)

:rsp_end

echo Source files processed: %c_file_count%

if "%c_file_count%"=="0" echo Source files not found in "%2"! & goto build_failed


if "%3"=="" goto cl_rsp
rem Add user source dir to the include path
echo /I "%3" >> "..\..\intdir\ccmpl.rsp"

:cl_rsp
echo /I "%2" >> "..\..\intdir\ccmpl.rsp"
echo /I "%DDKPATH%\inc\wxp" >> "..\..\intdir\ccmpl.rsp"
echo /I "%DDKPATH%\inc\ddk\wxp" >> "..\..\intdir\ccmpl.rsp"
echo /I "%DDKPATH%\inc\crt" >> "..\..\intdir\ccmpl.rsp"

rem Get user options for compiler
findstr /b "COMPILE=" "..\..\configs\xp\drv\%1.cfg" > "..\..\intdir\cluopt.tmp"
if "%errorlevel%" neq "0" echo Build configuration file (..\..\configs\xp\drv\%1.cfg) doesn't consist of user options COMPILE= & goto build_failed
for /f "tokens=2* delims==" %%A in (..\..\intdir\cluopt.tmp) do (echo %%A >> "..\..\intdir\ccmpl.rsp")

rem Get system options for compiler
findstr /b "COMPILE=" drv.cfg > "..\..\intdir\clsopt.tmp"
if "%errorlevel%" neq "0" echo System config file (drv.cfg) doesn't consist of options for COMPILE= & goto build_failed
for /f "tokens=2* delims==" %%A in (..\..\intdir\clsopt.tmp) do (echo %%A >> "..\..\intdir\ccmpl.rsp")

echo  /Fo"..\..\intdir\%1_sys\\" >> "..\..\intdir\ccmpl.rsp"
echo  /Fd"..\..\intdir\%1_sys\%1.pdb" >> "..\..\intdir\ccmpl.rsp"

rem Iterating recursively through the sources dir (%2) and compile c/cpp
set /a c_built_files = 0
for /R "%2" %%A in ("*.c","*.cpp") do (call vccc.bat "%%A" "..\..\intdir\ccmpl.rsp")

if "%3"=="" goto cmpl_end
rem Iterating recursively through the user sources dir (%3) and compile c/cpp
for /R "%3" %%A in ("*.c","*.cpp") do (call vccc.bat "%%A" "..\..\intdir\ccmpl.rsp")
:cmpl_end

if "%c_built_files%" neq "%c_file_count%" echo Compilation failed (files total: %c_file_count%, files built: %c_built_files%)! & goto build_failed

echo Compilation succeeded, files built: %c_built_files%

echo Linking %1

echo /OUT:"..\..\intdir\%1_sys\%1.sys" >> "..\..\intdir\clnk.rsp"
echo /LIBPATH:"%DDKPATH%\lib\wxp\i386" >> "..\..\intdir\clnk.rsp" 

rem Get system options for linker
findstr /b "LINK=" drv.cfg > "..\..\intdir\lnksopt.tmp"
if "%errorlevel%" neq "0" echo System config file (drv.cfg) doesn't consist of options for LINK= & goto build_failed
for /f "tokens=2* delims==" %%A in (..\..\intdir\lnksopt.tmp) do (echo %%A >> "..\..\intdir\clnk.rsp")

rem Get user options for linker
findstr /b "LINK=" "..\..\configs\xp\drv\%1.cfg" > "..\..\intdir\lnkuopt.tmp"
if "%errorlevel%" neq "0" echo System config file (..\..\configs\xp\drv\%1.cfg) doesn't consist of options for LINK= & goto build_failed
for /f "tokens=2* delims==" %%A in (..\..\intdir\lnkuopt.tmp) do (echo %%A >> "..\..\intdir\clnk.rsp")

%DDKPATH%\bin\x86\link @"..\..\intdir\clnk.rsp"

if "%errorlevel%" == "0" echo Linker produced "..\..\intdir\%1_sys\%1.sys" successfully 

findstr /b "POSTBUILD=" "..\..\configs\xp\drv\%1.cfg" > "..\..\intdir\postbuopt.tmp"
for /f "tokens=2* delims==" %%A in (..\..\intdir\postbuopt.tmp) do (call %%A %CD%)

rem ==== COPY files to the  deployment folder
rem echo Copying %1.sys to the deployment folder
rem if not exist "..\..\bin\xp" mkdir "..\..\bin\xp"
rem copy /Y "..\..\intdir\%1\%1.sys" "..\..\bin\xp"

goto all_done

echo Link failed! "..\..\intdir\%1_sys\%1.sys" has not been produced!

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
