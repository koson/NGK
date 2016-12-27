@echo off
rem cls

if exist vcpath.txt del vcpath.txt
if exist regout.txt del regout.txt

echo Looking for MSVC++...

:msvs2008_check
echo Looking for MS Visual C++ 2008...
reg query "HKLM\SOFTWARE\Microsoft\VisualStudio\9.0\Setup\VC" /v ProductDir > regout.txt
if "%errorlevel%"=="1" goto msvs2005_check

echo MS Visual C++ 2008 installed
set /a f_vc_ver=90
goto cl_checking

:msvs2005_check
echo Looking for MS Visual C++ 2005...
query "HKLM\SOFTWARE\Microsoft\VisualStudio\8.0\Setup\VC" /v ProductDir > regout.txt
if "%errorlevel%"=="1" goto msvs2003_check

echo MS Visual C++ 2005 installed
set /a f_vc_ver=80
goto cl_checking

:msvs2003_check
echo Looking for MS Visual C++ 2003...
reg query "HKLM\SOFTWARE\Microsoft\VisualStudio\7.1\Setup\VC" /v ProductDir > regout.txt
if "%errorlevel%"=="1" goto msvs2002_check

echo MS Visual C++ 2003 installed
set /a f_vc_ver=71
goto cl_checking


:msvs2002_check
echo Looking for MS Visual C++ 2002...
reg query "HKLM\SOFTWARE\Microsoft\VisualStudio\7.0\Setup\VC" /v ProductDir > regout.txt
if "%errorlevel%"=="1" goto msvs6_check


echo MS Visual C++ 2002 installed
set /a f_vc_ver=70
goto cl_checking

:msvs6_check
echo Looking for MSVC++ 6.0...
reg query "HKLM\SOFTWARE\Microsoft\VisualStudio\6.0\Setup\Microsoft Visual C++" /v ProductDir > regout.txt
if "%errorlevel%"=="1" goto failure_check
echo Microsoft MS Visual C++ 6.0 installed
set /a f_vc_ver=60
for /f "skip=4 tokens=3*" %%A in (regout.txt) do (echo "%%A %%B\bin" > vcpath.txt)
for /f "skip=4 tokens=3*" %%A in (regout.txt) do (set f_vc_path="%%A %%B\bin")

goto cl_check_exist

:failure_check

echo Failure: MS Visual C++ not installed on this computer!
goto all_done

if not exist regout.txt goto all_done

:cl_checking
if exist vcpath.txt del vcpath.txt

for /f "skip=4 tokens=3*" %%A in (regout.txt) do (echo "%%A %%Bbin" > vcpath.txt)
for /f "skip=4 tokens=3*" %%A in (regout.txt) do (set f_vc_path="%%A %%Bbin")

rem echo VC Path: %f_vc_path%
rem echo Checking VC tools...

:check_varargs
if not exist %f_vc_path%\vcvars32.bat echo %f_vc_path%\vcvars32.bat NOT exist! & del vcpath.txt & goto all_done
:cl_check_exist
if not exist %f_vc_path%\cl.exe echo %f_vc_path%\cl.exe compiler NOT exist! & del vcpath.txt & goto all_done
if not exist %f_vc_path%\link.exe echo %f_vc_path%\link.exe linker NOT exist! & del vcpath.txt & goto all_done

echo MSVC++ tools located in: %f_vc_path%


:all_done
if exist regout.txt del regout.txt