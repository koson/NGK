@ECHO OFF
SetLocal EnableExtensions
set key=HKLM\SOFTWARE\Microsoft\Windows NT\CurrentVersion

FOR /F "delims=" %%a IN ('reg query "%key%" /v "ProductName" ^| find /i "ProductName"') DO (
  set OSName=%%a
)
ECHO %OSName% |>NUL find /i "Windows XP" && call :XP || ECHO %OSName% |>NUL find /i "Windows 7" && call :W7 || call :Other
PAUSE
EXIT /B

:XP
ECHO XP
EXIT /B

:W7
ECHO Win 7
EXIT /B

:Other
ECHO Unknown OS
EXIT /B