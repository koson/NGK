@ECHO OFF

REM Данный скрип подготавливает опереционную систему Windows XP Embedded для использования системного тома как защищённого
REM Основные ресурсы переносятся на незащищённый том (переменная Root)

SET Root=E:\
SET PathToLogs="%Root%System Logs"\
REM Если PageFileMinSize=0 PageFileMaxSize=0 то выбор размера файла подкачки осуществляет операционная система
SET PageFileMinSize=0
SET PageFileMaxSize=0
SET TempDirectory=%Root%Temp\
SET LocalSettingsDirectory="%TempDirectory%Local Settings"\

@ECHO OFF
ECHO This script prepares Windows XP Embedded for using with protected disk EWF
CHOICE /C YN /M "Continue? Y - Yes, N - No"
IF errorlevel 2 GOTO labelExit
IF errorlevel 1 GOTO labelYes

:labelExit
	REM Завершить выполнение сценария
	EXIT 0; 
:labelYes

@ECHO OFF
REM Перенос файлов системных журналов на незащищённый том
@ECHO ON
ECHO Removing system logs to unprotected disk
REG ADD HKEY_LOCAL_MACHINE\SYSTEM\CurrentControlSet\Services\Eventlog\Application /v File /t REG_EXPAND_SZ /d %PathToLogs% /f
REG ADD HKEY_LOCAL_MACHINE\SYSTEM\CurrentControlSet\Services\Eventlog\System /v File /t REG_EXPAND_SZ /d %PathToLogs% /f
REG ADD HKEY_LOCAL_MACHINE\SYSTEM\CurrentControlSet\Services\Eventlog\Security /v File /t REG_EXPAND_SZ /d %PathToLogs% /f
XCOPY %SystemRoot%\system32\config\AppEvent.Evt %PathToLogs% /Y
XCOPY %SystemRoot%\System32\config\SecEvent.Evt %PathToLogs% /Y
XCOPY %SystemRoot%\system32\config\SysEvent.Evt %PathToLogs% /Y

@ECHO OFF
REM Перенос файла подкачки на незащищённый том
@ECHO ON
ECHO Current location of page files:
pagefileconfig /query
ECHO Set new location
REG ADD "HKEY_LOCAL_MACHINE\SYSTEM\CurrentControlSet\Control\Session Manager\Memory Management" /v PagingFiles /t REG_MULTI_SZ /d "%Root%pagefile.sys %PageFileMinSize% %PageFileMaxSize%" /f
REM pagefileconfig /delete C:
REM pagefileconfig /create E:
pagefileconfig /query

@ECHO OFF
REM Перенос директорий временных файлов на незащищённый том
@ECHO ON
ECHO Removing directories of temporary files:
REG ADD "HKEY_CURRENT_USER\Software\Microsoft\Windows\CurrentVersion\Explorer\User Shell Folders" /v Cache /t REG_EXPAND_SZ /d %TempDirectory% /f
REG ADD "HKEY_CURRENT_USER\Software\Microsoft\Windows\CurrentVersion\Explorer\Shell Folders" /v Cache /t REG_EXPAND_SZ /d %TempDirectory% /f
REG ADD HKEY_CURRENT_USER\Environment /v TEMP /t REG_EXPAND_SZ /d %LocalSettingsDirectory% /f
REG ADD HKEY_CURRENT_USER\Environment /v TMP /t REG_EXPAND_SZ /d %LocalSettingsDirectory% /f

@ECHO OFF
REM Отключение записи времени последнего доступа
@ECHO ON
ECHO Disable last access time recording:
REG ADD HKEY_LOCAL_MACHINE\SYSTEM\CurrentControlSet\Control\FileSystem /v NtfsDisableLastAccessUpdate /t REG_DWORD /d 1 /f

@ECHO OFF
REM Отключение предварительной загрузки
@ECHO ON
ECHO Disabling Preboot:
REG ADD "HKEY_LOCAL_MACHINE\SYSTEM\CurrentControlSet\Control\Session Manager\Memory Management\PrefetchParameters" /v EnablePrefetcher /t REG_DWORD /d 0 /f

ECHO You should restart computer for apply changes
ECHO Do you want restart?
CHOICE /C YN /T 10 /D Y /M "Restart computer? Y - Yes, restart, N - No, exit without restart; Default settings: Yes, Timeout 10с"
IF errorlevel 2 GOTO labelExit
IF errorlevel 1 GOTO labelRestart

labelRestart:
SHUTDOWN /r /d P:12:555 /t 0
