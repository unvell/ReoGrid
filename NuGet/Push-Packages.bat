
SET VER=2.1.0.0

cd packages-wpf

..\NuGet push unvell.ReoGridWPF.dll.%VER%.nupkg

cd ..

cd packages

..\NuGet push unvell.ReoGrid.dll.%VER%.nupkg

pause