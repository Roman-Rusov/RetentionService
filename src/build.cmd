@echo off

echo Building ConsoleApp

pushd ConsoleApp

dotnet build -c Release -r win10-x64

popd

echo Building ConsoleApp complete
