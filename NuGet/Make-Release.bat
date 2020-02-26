
SET VER=2.2.0

SET MSB="C:\Program Files (x86)\Microsoft Visual Studio\2019\Community\MSBuild\Current\Bin\MSBuild.exe"
SET MSB_ARG=/t:Build /p:WarningLevel=0

SET CFG=Release
%MSB% ..\ReoGrid\ReoGrid.csproj %MSB_ARG% /p:Configuration=%CFG%;TargetFrameworkVersion=v3.5,Profile=Client
copy ..\ReoGrid\Bin\%CFG%\unvell.ReoGrid.dll packages\lib\net20\ /y
copy ..\ReoGrid\Bin\%CFG%\unvell.ReoGrid.xml packages\lib\net20\ /y

cd packages

..\NuGet pack -Version %VER%

cd ..

SET CFG=WPFRelease
%MSB% ..\ReoGrid\ReoGrid.csproj %MSB_ARG% /p:Configuration=%CFG%;TargetFrameworkVersion=v3.5,Profile=Client
copy ..\ReoGrid\Bin\%CFG%\unvell.ReoGrid.dll packages-wpf\lib\net20\ /y
copy ..\ReoGrid\Bin\%CFG%\unvell.ReoGrid.xml packages-wpf\lib\net20\ /y

cd packages-wpf

..\NuGet pack -Version %VER%

cd ..
