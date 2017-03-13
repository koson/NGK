@echo off

nuget restore ..\Projects\RubezhService\RubezhService.sln
nuget restore ..\Projects\RubezhAdministrator\RubezhAdministrator.sln
nuget restore ..\Projects\RubezhMonitor\RubezhMonitor.sln
nuget restore ..\Projects\GKOPCServer\GKOPCServer.sln

del %cd%\bin /Q

"%ProgramFiles(x86)%\MSBuild\14.0\Bin\MsBuild.exe" ..\Projects\RubezhService\RubezhService.sln /t:Build /v:minimal /p:Configuration=Release /p:TargetFramework=v4.6.1 /flp:logfile=RubezhService.log;verbosity=minimal
"%ProgramFiles(x86)%\MSBuild\14.0\Bin\MsBuild.exe" ..\Projects\RubezhMonitor\RubezhMonitor.sln /t:Build /v:minimal /p:Configuration=Release /p:TargetFramework=v4.6.1 /flp:logfile=RubezhMonitor.log;verbosity=minimal
"%ProgramFiles(x86)%\MSBuild\14.0\Bin\MsBuild.exe" ..\Projects\RubezhAdministrator\RubezhAdministrator.sln /t:Build /v:minimal /p:Configuration=Release /p:TargetFramework=v4.6.1 /flp:logfile=RubezhAdministrator.log;verbosity=minimal
"%ProgramFiles(x86)%\MSBuild\14.0\Bin\MsBuild.exe" ..\Projects\GKOPCServer\GKOPCServer.sln /t:Build /v:minimal /p:Configuration=Release /p:TargetFramework=v4.6.1 /flp:logfile=GKOPCServer.log;verbosity=minimal
"%ProgramFiles(x86)%\MSBuild\14.0\Bin\MsBuild.exe" ..\Projects\Installers\RubezhInstaller\RubezhInstaller.sln /v:minimal /t:Build /p:Configuration=Release /p:TargetFramework=v4.6.1 /flp:logfile=RubezhInstaller.log;verbosity=minimal

xcopy ..\Projects\Installers\RubezhInstaller\RubezhInstaller\bin\Release %cd%\bin\
pause