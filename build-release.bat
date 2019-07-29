@ECHO OFF

SET ERRORLEVEL=0
SET VER=2.1.0.0

REM ### set MSB to locate the msbuild.exe in your environment ###
SET MSB="%ProgramFiles(x86)%\Microsoft Visual Studio\2017\Community\MSBuild\15.0\Bin\msbuild.exe"

REM ### Set /tv to fit build tool version of your environment ###
REM ### Visual Studio 2015: 14 ###
REM ### Visual Studio 2017: 15 ###
SET MSB_ARG=/tv:15.0 /t:Build

REM ### set ZIP to locate the 7z.exe in your environment ###
SET ZIP="%ProgramFiles%\7-Zip\7z.exe"
SET ZIPEXL="-xr!.svn\* -xr!.vs\* -xr!bin -xr!obj -xr!*.user"

IF NOT EXIST %MSB% (
  SET MSB="C:\Windows\Microsoft.NET\Framework64\v4.0.30319\msbuild.exe"
)

IF NOT EXIST %MSB% (
  ECHO Cannot found msbuild.exe, set MSB to locate the msbuild.exe in your environment
  GOTO :EOF
)

IF NOT EXIST %ZIP% (
  ECHO Missing ZIP
  GOTO :EOF
)

SET OUT_REPO=_Output

IF "%1"=="-x" GOTO archive

ECHO ---------------------- Minimum -------------------------
SET CFG=Minimum
%MSB% ReoGrid\ReoGrid.csproj %MSB_ARG% /p:Configuration=%CFG%;TargetFrameworkVersion=v3.5,Profile=Client

SET OUT_DIR=%OUT_REPO%\ReoGrid-%VER%-%CFG%
IF EXIST %OUT_DIR% RMDIR %OUT_DIR% /S /Q
MKDIR %OUT_DIR%
COPY ReoGrid\bin\%CFG%\unvell.ReoGrid.dll %OUT_DIR%\
COPY ReoGrid\bin\%CFG%\unvell.ReoGrid.xml %OUT_DIR%\
COPY "LICENSE" %OUT_DIR%\
COPY "License.bzip2.txt" %OUT_DIR%\
COPY "License.DotNetZipLib.txt" %OUT_DIR%\
COPY "License.zlib.txt" %OUT_DIR%\
COPY "README.md" %OUT_DIR%\

SET CFG=WPFMinimum
%MSB% ReoGrid\ReoGrid.csproj %MSB_ARG% /p:Configuration=%CFG%;TargetFrameworkVersion=v3.5,Profile=Client

SET OUT_DIR=%OUT_REPO%\ReoGrid-%VER%-%CFG%
IF EXIST %OUT_DIR% RMDIR %OUT_DIR% /S /Q
MKDIR %OUT_DIR%
COPY ReoGrid\bin\%CFG%\unvell.ReoGrid.dll %OUT_DIR%\
COPY ReoGrid\bin\%CFG%\unvell.ReoGrid.xml %OUT_DIR%\
COPY "LICENSE" %OUT_DIR%\
COPY "License.bzip2.txt" %OUT_DIR%\
COPY "License.DotNetZipLib.txt" %OUT_DIR%\
COPY "License.zlib.txt" %OUT_DIR%\
COPY "README.md" %OUT_DIR%\

IF NOT %ERRORLEVEL% LEQ 0 (
  PAUSE
  GOTO :EOF
)


ECHO ------------------ Standard Release ---------------------
SET CFG=Release
%MSB% ReoGrid\ReoGrid.csproj %MSB_ARG% /p:Configuration=%CFG%;TargetFrameworkVersion=v3.5,Profile=Client

SET OUT_DIR=%OUT_REPO%\ReoGrid-%VER%-%CFG%
IF EXIST %OUT_DIR% RMDIR %OUT_DIR% /s /q
MKDIR %OUT_DIR%
COPY ReoGrid\bin\%CFG%\unvell.ReoGrid.dll %OUT_DIR%\
COPY ReoGrid\bin\%CFG%\unvell.ReoGrid.xml %OUT_DIR%\
COPY "LICENSE" %OUT_DIR%\
COPY "License.bzip2.txt" %OUT_DIR%\
COPY "License.DotNetZipLib.txt" %OUT_DIR%\
COPY "License.zlib.txt" %OUT_DIR%\
COPY "README.md" %OUT_DIR%\


ECHO --------------------- WPF ------------------------
SET CFG=WPFRelease
%MSB% ReoGrid\ReoGrid.csproj %MSB_ARG% /p:Configuration=%CFG%;TargetFrameworkVersion=v3.5,Profile=Client

SET OUT_DIR=%OUT_REPO%\ReoGrid-%VER%-%CFG%
IF EXIST %OUT_DIR% RMDIR %OUT_DIR% /S /Q
MKDIR %OUT_DIR%
COPY ReoGrid\bin\%CFG%\unvell.ReoGrid.dll %OUT_DIR%\
COPY ReoGrid\bin\%CFG%\unvell.ReoGrid.xml %OUT_DIR%\
COPY "LICENSE" %OUT_DIR%\
COPY "License.bzip2.txt" %OUT_DIR%\
COPY "License.DotNetZipLib.txt" %OUT_DIR%\
COPY "License.zlib.txt" %OUT_DIR%\
COPY "README.md" %OUT_DIR%\


ECHO -------------------  Extension -----------------------

SET CFG=Extension
%MSB% ReoGrid.sln %MSB_ARG% /p:Configuration=%CFG%;TargetFrameworkVersion=v3.5,Profile=Client

SET OUT_DIR=%OUT_REPO%\ReoGrid-%VER%-%CFG%
IF EXIST %OUT_DIR% RMDIR %OUT_DIR% /s /q
MKDIR %OUT_DIR%

COPY ReoGrid\bin\%CFG%\unvell.ReoGrid.dll %OUT_DIR%\
COPY ReoGrid\bin\%CFG%\unvell.ReoGrid.xml %OUT_DIR%\
COPY Ref\Antlr3.Runtime.dll %OUT_DIR%\
COPY Ref\FastColoredTextBox.dll %OUT_DIR%\
COPY Ref\unvell.ReoScript.dll %OUT_DIR%\
COPY Ref\unvell.ReoScript.EditorLib.dll %OUT_DIR%\
COPY "license*" %OUT_DIR%\
COPY "README.md" %OUT_DIR%\


ECHO ---------------- Demo EN ---------------------

SET OUT_DIR=%OUT_REPO%\ReoGrid-%VER%-Demo-Binary
IF EXIST %OUT_DIR% RMDIR %OUT_DIR% /S /Q
MKDIR %OUT_DIR%

COPY Editor\Bin\Extension\unvell.ReoGridEditor.exe %OUT_DIR%\ /Y
COPY ReoGrid\Bin\Extension\unvell.ReoGrid.dll %OUT_DIR%\ /Y
COPY Demo\Bin\Release\ReoGridDemo.exe %OUT_DIR%\ /Y

MKDIR %OUT_DIR%\_Templates
XCOPY Demo\_Templates %OUT_DIR%\_Templates /e
COPY Ref\Antlr3.Runtime.dll %OUT_DIR%\
COPY Ref\FastColoredTextBox.dll %OUT_DIR%\
COPY Ref\unvell.ReoScript.dll %OUT_DIR%\
COPY Ref\unvell.ReoScript.EditorLib.dll %OUT_DIR%\
COPY "license*" %OUT_DIR%\
COPY "README.md" %OUT_DIR%\
XCOPY Editor\Bin\Extension\ja-JP %OUT_DIR%\ja-JP\ /e
XCOPY Editor\Bin\Extension\ru-RU %OUT_DIR%\ru-RU\ /e
XCOPY Editor\Bin\Extension\zh-CN %OUT_DIR%\zh-CN\ /e



ECHO ---------------- Demo JP ---------------------

SET OUT_DIR=%OUT_REPO%\ReoGrid-%VER%-Demo_Ja-Binary
IF EXIST %OUT_DIR% RMDIR %OUT_DIR% /S /Q
MKDIR %OUT_DIR%

COPY Editor\Bin\Extension\unvell.ReoGridEditor.exe DemoJP\Ref /Y
COPY ReoGrid\Bin\Extension\unvell.ReoGrid.dll DemoJP\Ref /Y

COPY Editor\Bin\Extension\unvell.ReoGridEditor.exe %OUT_DIR%\ /Y
COPY ReoGrid\Bin\Extension\unvell.ReoGrid.dll %OUT_DIR%\ /Y
COPY DemoJP\Bin\Release\ReoGridDemoJP.exe %OUT_DIR%\ /Y

MKDIR %OUT_DIR%\_Templates
XCOPY DemoJP\_Templates %OUT_DIR%\_Templates /e
COPY Ref\Antlr3.Runtime.dll %OUT_DIR%\
COPY Ref\FastColoredTextBox.dll %OUT_DIR%\
COPY Ref\unvell.ReoScript.dll %OUT_DIR%\
COPY Ref\unvell.ReoScript.EditorLib.dll %OUT_DIR%\
COPY "license*" %OUT_DIR%\
COPY "README.md" %OUT_DIR%\
XCOPY Editor\Bin\Extension\ja-JP %OUT_DIR%\ja-JP\ /e
XCOPY Editor\Bin\Extension\ru-RU %OUT_DIR%\ru-RU\ /e
XCOPY Editor\Bin\Extension\zh-CN %OUT_DIR%\zh-CN\ /e



ECHO -------------------  WPFExtension -----------------------

SET CFG=Extension
%MSB% ReoGrid\ReoGrid.csproj %MSB_ARG% /p:Configuration=%CFG%;TargetFrameworkVersion=v3.5,Profile=Client

SET OUT_DIR=%OUT_REPO%\ReoGrid-%VER%-WPF%CFG%
IF EXIST %OUT_DIR% RMDIR %OUT_DIR% /S /Q
MKDIR %OUT_DIR%
COPY ReoGrid\bin\%CFG%\unvell.ReoGrid.dll %OUT_DIR%\
COPY ReoGrid\bin\%CFG%\unvell.ReoGrid.xml %OUT_DIR%\
COPY Ref\Antlr3.Runtime.dll %OUT_DIR%\
COPY Ref\unvell.ReoScript.dll %OUT_DIR%\
COPY "license*" %OUT_DIR%\
COPY "README.md" %OUT_DIR%\


ECHO -------------------  WPFRelease -----------------------

SET CFG=Release
%MSB% ReoGrid\ReoGrid.csproj %MSB_ARG% /p:Configuration=%CFG%;TargetFrameworkVersion=v3.5,Profile=Client

SET OUT_DIR=%OUT_REPO%\ReoGrid-%VER%-WPF%CFG%
IF EXIST %OUT_DIR% RMDIR %OUT_DIR% /S /Q
MKDIR %OUT_DIR%
COPY ReoGrid\bin\%CFG%\unvell.ReoGrid.dll %OUT_DIR%\
COPY ReoGrid\bin\%CFG%\unvell.ReoGrid.xml %OUT_DIR%\
COPY "license*" %OUT_DIR%\
COPY "README.md" %OUT_DIR%\


ECHO -------------------  WPFMinimum -----------------------

SET CFG=Minimum
%MSB% ReoGrid\ReoGrid.csproj %MSB_ARG% /p:Configuration=%CFG%;TargetFrameworkVersion=v3.5,Profile=Client

SET OUT_DIR=%OUT_REPO%\ReoGrid-%VER%-WPF%CFG%
IF EXIST %OUT_DIR% RMDIR %OUT_DIR% /S /Q
MKDIR %OUT_DIR%
COPY ReoGrid\bin\%CFG%\unvell.ReoGrid.dll %OUT_DIR%\
COPY ReoGrid\bin\%CFG%\unvell.ReoGrid.xml %OUT_DIR%\
COPY "license*" %OUT_DIR%\
COPY "README.md" %OUT_DIR%\


ECHO ----------------- Demo WPF ---------------------

SET OUT_DIR=%OUT_REPO%\ReoGrid-%VER%-WPFDemo-Binary
IF EXIST %OUT_DIR% RMDIR %OUT_DIR% /S /Q
MKDIR %OUT_DIR%

COPY DemoWPF\bin\Release\WPFDemo.exe %OUT_DIR%\
COPY ReoGrid\Bin\WPFRelease\unvell.ReoGrid.dll %OUT_DIR%\
COPY "license*" %OUT_DIR%\
COPY "README.md" %OUT_DIR%\

IF NOT %ERRORLEVEL% LEQ 0 (
  PAUSE
  GOTO :EOF
)


IF EXIST Demo\bin rmdir /s/q Demo\bin
IF EXIST Demo\obj rmdir /s/q Demo\obj
IF EXIST DemoJP\bin rmdir /s/q DemoJP\bin
IF EXIST DemoJP\obj rmdir /s/q DemoJP\obj
IF EXIST DemoWPF\bin rmdir /s/q DemoWPF\bin
IF EXIST DemoWPF\obj rmdir /s/q DemoWPF\obj
IF EXIST TestCase\bin rmdir /s/q TestCase\bin
IF EXIST TestCase\obj rmdir /s/q TestCase\obj
IF EXIST Editor\bin rmdir /s/q Editor\bin
IF EXIST Editor\obj rmdir /s/q Editor\obj


cd %OUT_REPO%

ECHO ----------------- Demo Source En ---------------------
@SET OUT_FILE=ReoGrid-%VER%-Demo-Source.zip
IF EXIST %OUT_FILE% del %OUT_FILE%
%ZIP% a -tzip %OUT_FILE% ..\Demo ..\Readme.md ..\license* ..\Demo2008.sln -xr!?svn\* -xr!?.vs\* -xr!?*.user

ECHO ----------------- Demo Source Ja ---------------------
@SET OUT_FILE=ReoGrid-%VER%-Demo_Ja-Source.zip
IF EXIST %OUT_FILE% del %OUT_FILE%
%ZIP% a -tzip %OUT_FILE% ..\DemoJP ..\Readme.md ..\license* ..\DemoJP.sln -xr!?svn\* -xr!?.vs\* -xr!?*.user

@SET OUT_FILE=ReoGrid-%VER%-Demo-Binary.zip
IF EXIST %OUT_FILE% del %OUT_FILE%
%ZIP% a -tzip %OUT_FILE% ReoGrid-%VER%-Demo-Binary\

@SET OUT_FILE=ReoGrid-%VER%-DemoWPF-Binary.zip
IF EXIST %OUT_FILE% del %OUT_FILE%
%ZIP% a -tzip %OUT_FILE% ReoGrid-%VER%-WPFDemo-Binary\

ECHO ----------------- Demo WPF Source ---------------------
@SET OUT_FILE=ReoGrid-%VER%-WPFDemo-Source.zip
IF EXIST %OUT_FILE% del %OUT_FILE%
%ZIP% a -tzip %OUT_FILE% ..\DemoWPF ..\DemoWPF.sln ..\Readme.md ..\license* %ZIPEXL%

ECHO ----------------- All Packages EN ---------------------
SET OUT_PKG_NAME=ReoGrid-%VER%-All-Packages.zip
@IF EXIST %OUT_PKG_NAME% DEL %OUT_PKG_NAME%
%ZIP% a -tzip %OUT_PKG_NAME% ^
 ReoGrid-%VER%-Demo-Binary ^
 ReoGrid-%VER%-WPFDemo-Binary ^
 ReoGrid-%VER%-Minimum ^
 ReoGrid-%VER%-WPFMinimum ^
 ReoGrid-%VER%-Release ^
 ReoGrid-%VER%-WPFRelease ^
 ReoGrid-%VER%-Extension ^
 ReoGrid-%VER%-WPFExtension ^
 ReoGrid-%VER%-Demo-Source.zip ^
 ReoGrid-%VER%-WPFDemo-Source.zip

ECHO ----------------- All Packages Ja ---------------------
SET OUT_PKG_NAME=ReoGrid-%VER%-All-Packages-Ja.zip
@IF EXIST %OUT_PKG_NAME% DEL %OUT_PKG_NAME%
%ZIP% a -tzip %OUT_PKG_NAME% ^
 ReoGrid-%VER%-Demo_Ja-Binary ^
 ReoGrid-%VER%-WPFDemo-Binary ^
 ReoGrid-%VER%-Minimum ^
 ReoGrid-%VER%-WPFMinimum ^
 ReoGrid-%VER%-Release ^
 ReoGrid-%VER%-WPFRelease ^
 ReoGrid-%VER%-Extension ^
 ReoGrid-%VER%-WPFExtension ^
 ReoGrid-%VER%-Demo_Ja-Source.zip ^
 ReoGrid-%VER%-WPFDemo-Source.zip

cd..

ECHO --------------------- DONE --------------------------
