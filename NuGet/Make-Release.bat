
SET VER=2.0.1.0

SET MSB="C:\Program Files (x86)\MSBuild\14.0\Bin\MSBuild.exe"
SET MSB_ARG=/tv:14.0 /t:Build

SET CFG=Release
%MSB% ..\ReoGrid\ReoGrid.csproj %MSB_ARG% /p:Configuration=%CFG%;TargetFrameworkVersion=v3.5,Profile=Client
copy ..\ReoGrid\Bin\%CFG%\unvell.ReoGrid.dll packages\lib\20\ /y
copy ..\ReoGrid\Bin\%CFG%\unvell.ReoGrid.xml packages\lib\20\ /y

cd packages

..\NuGet pack -Version %VER%

cd ..

SET CFG=WPFRelease
%MSB% ..\ReoGrid\ReoGrid.csproj %MSB_ARG% /p:Configuration=%CFG%;TargetFrameworkVersion=v3.5,Profile=Client
copy ..\ReoGrid\Bin\%CFG%\unvell.ReoGrid.dll packages-wpf\lib\20\ /y
copy ..\ReoGrid\Bin\%CFG%\unvell.ReoGrid.xml packages-wpf\lib\20\ /y

cd packages-wpf

..\NuGet pack -Version %VER%

cd ..
