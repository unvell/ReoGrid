
SET VER=2.1.1.0

NuGet push packages-wpf\unvell.ReoGridWPF.dll.%VER%.nupkg

cd ..

cd packages

..\NuGet push unvell.ReoGrid.dll.%VER%.nupkg

pause