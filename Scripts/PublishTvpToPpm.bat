REM A batchscript to deploy the Paratext plugin to the Paratext Plugin Manager.

REM Set function variables
SET SCRIPT_ROOT=%~dp0
SET SOLUTON_ROOT=%SCRIPT_ROOT%..

REM Run the publish executable
pushd %SOLUTON_ROOT%\TvpPublish\
dotnet clean
dotnet run

REM return to starting directory
popd