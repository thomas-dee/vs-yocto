@echo off

:: DOS batch script for registering any Yocto MinGW SDK.
:: Note: This is currently only tested for ARM64.
::       If you have any issues: https://github.com/thomas-dee/vs-yocto/issues
set YoctoSDKRoot=%1

:: check for admin rights
net session >nul 2>&1
if not %ERRORLEVEL% == 0 (
    echo Please run this script with admin rights.
    goto:ende
) 

if "%YoctoSDKRoot%" == "" call:getYoctoSDKDir

if not exist "%YoctoSDKRoot%\relocate_sdk.py" (
    echo Invalid Yocto SDK root dir "%YoctoSDKRoot%"!
    goto:ende
)

cd /d %YoctoSDKRoot%
set VsWhere=C:\Program Files (x86)\Microsoft Visual Studio\Installer\vswhere.exe

if not exist "%VsWhere%" (
    echo This script requires the Installation of vswhere.exe!
    goto:ende
)

::-----------------------------------------------------------------------------
:: find Visual Studio Installation
set VCInstallDir=
set VCVersion=
for /f "usebackq tokens=*" %%i in (`"%VsWhere%" -latest -products * -requires Microsoft.VisualStudio.Component.VC.Tools.x86.x64 -property installationPath`) do (
  set VCInstallDir=%%i
)

for /f "usebackq tokens=*" %%i in (`"%VsWhere%" -latest -products * -requires Microsoft.VisualStudio.Component.VC.Tools.x86.x64 -property displayName`) do (
  set VCVersion=%%i
)
echo Visual Studio: %VCVersion%

::-----------------------------------------------------------------------------
:: get target architecture
set YoctoSDKArch=
set YoctoSDKPlatformTriplet=
set YoctoSDKPlatformToolset=
for /f "tokens=1,2,* delims=-" %%a in ('dir /A /B version-*') do (
    set YoctoSDKArch=%%b
    set YoctoSDKPlatformTriplet=%%b-%%c
)
echo Yocto SDK Target Architecture: %YoctoSDKArch%

:: get Yocto version
set YoctoSDKVersion=
for /f "tokens=1,2,3" %%a in ('type version-%YoctoSDKPlatformTriplet%') do if "%%b" == "Version:" set YoctoSDKVersion=%%c
echo Yocto SDK Version: %YoctoSDKVersion%
set YoctoSDKPlatformToolset=%YoctoSDKPlatformTriplet%-%YoctoSDKVersion%
:: get GCC version
for /f "tokens=*" %%a in ('dir /B sysroots\%YoctoSDKPlatformTriplet%\usr\include\c++') do set YoctoSDKGCCVersion=%%a
echo Yocto SDK GCC Version: %YoctoSDKGCCVersion%
echo Yocto SDK PlatformToolset: %YoctoSDKPlatformToolset%

:: get correct PlatformToolset
set VCYoctoPlatform=
set VCYoctoSDKArchDefine=
call:%YoctoSDKArch%

if "%VCYoctoPlatform%" == "" (
    echo Unable to detect Linux PlatformToolset for "%YoctoSDKArch%"!
    goto:ende
)

::-----------------------------------------------------------------------------
set VCYoctoPlatformPath=%VCInstallDir%\Common7\IDE\VC\VCTargets\Application Type\Linux\1.0\Yocto
set VCYoctoPlatformToolsetPath=%VCInstallDir%\Common7\IDE\VC\VCTargets\Application Type\Linux\1.0\Platforms\%VCYoctoPlatform%\PlatformToolsets\%YoctoSDKPlatformToolset%

:: Is there already any vs-yocto.Build.CPPTasks.GCC.dll installation
if not exist "%VCYoctoPlatformPath%\vs-yocto.Build.CPPTasks.GCC.dll" call:createYoctoPlatform


set YoctoSDKConfigFile=%VCYoctoPlatformToolsetPath%\YoctoSDKConfig.props

mkdir "%VCYoctoPlatformToolsetPath%"
xcopy /S "%VCYoctoPlatformPath%\PlatformToolsetTemplate" "%VCYoctoPlatformToolsetPath%">NUL 2>NUL

echo ^<Project xmlns="http://schemas.microsoft.com/developer/msbuild/2003"^> > "%YoctoSDKConfigFile%
echo  ^<PropertyGroup^> >> "%YoctoSDKConfigFile%
echo    ^<YoctoSDKRoot^>%YoctoSDKRoot%^</YoctoSDKRoot^> >> "%YoctoSDKConfigFile%
echo    ^<YoctoSDKPlatformTriplet^>%YoctoSDKPlatformTriplet%^</YoctoSDKPlatformTriplet^> >> "%YoctoSDKConfigFile%
echo    ^<YoctoSDKGCCVersion^>%YoctoSDKGCCVersion%^</YoctoSDKGCCVersion^> >> "%YoctoSDKConfigFile%
echo    ^<YoctoSDKMinGW^>x86_64-pokysdk-mingw32^</YoctoSDKMinGW^> >> "%YoctoSDKConfigFile%
echo    ^<YoctoSDKArchDefine^>%VCYoctoSDKArchDefine%^</YoctoSDKArchDefine^> >> "%YoctoSDKConfigFile%
echo  ^</PropertyGroup^> >> "%YoctoSDKConfigFile%
echo ^</Project^> >> "%YoctoSDKConfigFile%
cd ..

echo Done.
::-----------------------------------------------------------------------------
:ende
pause
goto:eof

::
:getYoctoSDKDir
set /p "YoctoSDKRoot=Yocto SDK root path: "
if "%YoctoSDKRoot%" == "" goto:getYoctoSDKDir
goto:eof

::-----------------------------------------------------------------------------
:: Architectue settings
:aarch64
set VCYoctoPlatform=ARM64
set VCYoctoSDKArchDefine=__aarch64__;__arm64;__LP64__
goto:eof

:aarch
set VCYoctoPlatform=ARM
set VCYoctoSDKArchDefine=__aarch__;__arm__
goto:eof

:i386
set VCYoctoPlatform=x86
set VCYoctoSDKArchDefine=__i386__;__ILP32__
goto:eof

:i486
set VCYoctoPlatform=x86
set VCYoctoSDKArchDefine=__i486__;__ILP32__
goto:eof

:i586
set VCYoctoPlatform=x86
set VCYoctoSDKArchDefine=__i586__;__ILP32__
goto:eof

:i686
set VCYoctoPlatform=x86
set VCYoctoSDKArchDefine=__i686__;__ILP32__
goto:eof

:x86_64
set VCYoctoPlatform=x64
set VCYoctoSDKArchDefine=__x86_64__
goto:eof


::-----------------------------------------------------------------------------
:createYoctoPlatform
set CurrentDir=%CD%

:: Building Yocto CPPTasks.dll
call "%VCInstallDir%\VC\Auxiliary\Build\vcvarsall.bat" x86 >NUL
cd %~dp0

if exist "vs-yocto.Build.CPPTasks.GCC.sln" goto:skipGitClone

mkdir "%TEMP%\.YoctoSdkPlatformBuilder" >NUL
cd /d "%TEMP%\.YoctoSdkPlatformBuilder" >NUL

:: Getting source code
echo Checking out vs-yocto.Build.CPPTasks.GCC sources..
git clone -b master https://github.com/thomas-dee/vs-yocto.git >NUL 2>NUL
cd vs-yocto

:skipGitClone

:: Building
echo Building vs-yocto.Build.CPPTasks.GCC.dll ...
msbuild vs-yocto.Build.CPPTasks.GCC.sln /p:Configuration="Release" /p:Platform="Any CPU" >NUL 2>NUL

:: Copying stuff
mkdir "%VCYoctoPlatformPath%">NUL 2>NUL
xcopy /S Yocto "%VCYoctoPlatformPath%">NUL 2>NUL
copy vs-yocto.Build.CPPTasks.GCC\bin\Release\vs-yocto.Build.CPPTasks.GCC.dll "%VCYoctoPlatformPath%\vs-yocto.Build.CPPTasks.GCC.dll">NUL 2>NUL

cd /d %CurrentDir%>NUL 2>NUL
if exist "%TEMP%\.YoctoSdkPlatformBuilder" rmdir /S /Q "%TEMP%\.YoctoSdkPlatformBuilder"
goto:eof
