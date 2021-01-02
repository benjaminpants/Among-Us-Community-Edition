@echo off
set /p user="Enter Steam Username: "
dotnet %~dp0DepotDownloader.dll %* -app 945360 -depot 945361 -manifest 2146956919302566155 -user %user%
pause