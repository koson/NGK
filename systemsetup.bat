@ECHO OFF
SET Root=E:\
SET PathToLogs="%Root%System Logs"\
REM Если PageFileMinSize=0 PageFileMaxSize=0 то размер файла подкачки выбирает операционная система
SET PageFileMinSize=0
SET PageFileMaxSize=0
SET TempDirectory=%Root%Temp\
SET LocalSettingsDirectory="%TempDirectory%Local Settings"\

@ECHO ON
ECHO This script prepares operational system for use (Данный сценарий погдотовит систему для защиты основного тома от записи)
REM Диалого Продолжить/выйти 

@ECHO OFF
REM Перенос файлов системных журналов на незащищённый том
@ECHO ON
ECHO Removing system logs to unprotected disk (Перенос файлов системных журналов на незащищённый том)
REG ADD HKEY_LOCAL_MACHINE\SYSTEM\CurrentControlSet\Services\Eventlog\Application /v File /t REG_EXPAND_SZ /d %PathToLogs% /f
REG ADD HKEY_LOCAL_MACHINE\SYSTEM\CurrentControlSet\Services\Eventlog\System /v File /t REG_EXPAND_SZ /d %PathToLogs% /f
REG ADD HKEY_LOCAL_MACHINE\SYSTEM\CurrentControlSet\Services\Eventlog\Security /v File /t REG_EXPAND_SZ /d %PathToLogs% /f
XCOPY %SystemRoot%\system32\config\AppEvent.Evt %PathToLogs% /Y
XCOPY %SystemRoot%\System32\config\SecEvent.Evt %PathToLogs% /Y
XCOPY %SystemRoot%\system32\config\SysEvent.Evt %PathToLogs% /Y

@ECHO OFF
REM Перенос файла подкачки на незащищённый том
@ECHO ON
ECHO Current location of page files (Перенос файла подкачки на незащищённый том):
pagefileconfig /query
ECHO Set new location Новое расположение файла
REG ADD "HKEY_LOCAL_MACHINE\SYSTEM\CurrentControlSet\Control\Session Manager\Memory Management" /v PagingFiles /t REG_MULTI_SZ /d "%Root%pagefile.sys %PageFileMinSize% %PageFileMaxSize%" /f
REM pagefileconfig /delete C:
REM pagefileconfig /create E:
pagefileconfig /query

@ECHO OFF
REM Перенос директорий временных файлов на незащищённый том
@ECHO ON
ECHO Removing directories of temporary files (Перенос директорий временных файлов на незащищённый том):
REG ADD "HKEY_CURRENT_USER\Software\Microsoft\Windows\CurrentVersion\Explorer\User Shell Folders" /v Cache /t REG_EXPAND_SZ /d %TempDirectory% /f
REG ADD "HKEY_CURRENT_USER\Software\Microsoft\Windows\CurrentVersion\Explorer\Shell Folders" /v Cache /t REG_EXPAND_SZ /d %TempDirectory% /f
REG ADD HKEY_CURRENT_USER\Environment /v TEMP /t REG_EXPAND_SZ /d %LocalSettingsDirectory% /f
REG ADD HKEY_CURRENT_USER\Environment /v TMP /t REG_EXPAND_SZ /d %LocalSettingsDirectory% /f

@ECHO OFF
REM Отключение записи времени последнего доступа
@ECHO ON
ECHO Disable last access time recording (Отключение записи времени последнего доступа);
REG ADD HKEY_LOCAL_MACHINE\SYSTEM\CurrentControlSet\Control\FileSystem /v NtfsDisableLastAccessUpdate /t REG_DWORD /d 1 /f

@ECHO OFF
REM Отключение предварительной загрузки
@ECHO ON
ECHO Disabling Preboot (Отключение предварительной загрузки):
REG ADD "HKEY_LOCAL_MACHINE\SYSTEM\CurrentControlSet\Control\Session Manager\Memory Management\PrefetchParameters" /v EnablePrefetcher /t REG_DWORD /d 0 /f

PAUSE
ECHO You should restart computer for apply changes
ECHO Do you want restart?
SHUTDOWN /r /d P:12:555 /t 0
