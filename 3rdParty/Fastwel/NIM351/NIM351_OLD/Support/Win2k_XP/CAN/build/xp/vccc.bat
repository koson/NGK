@echo off
rem %1 -- input file
rem %2 -- rsp

%f_vc_path%\cl.exe /nologo /D "_WIN32_WINNT=0x500" %1 @%2

if "%errorlevel%"=="0" set /a c_built_files+=1 


:end