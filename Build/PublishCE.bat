@ECHO OFF
SET TARGET_DIR=%*
SET SOLUTION_DIR=%CD%

set /p WARNING1=WARNING: MAKE SURE THAT YOU HAVE BUILT THE CE INSTALLER BEFORE PROCEEDING!

:: Clean Publish Directory
RMDIR /S /Q "%SOLUTION_DIR%\Publish"
MKDIR "%SOLUTION_DIR%\Publish"
MKDIR "%SOLUTION_DIR%\Publish\Mod"
MKDIR "%SOLUTION_DIR%\Publish\Mod\Mods"
MKDIR "%SOLUTION_DIR%\Publish\Mod\Among Us_Data\Managed"

:: Copy All Needed Folder
ROBOCOPY "%SOLUTION_DIR%\Among Us_Data" "%SOLUTION_DIR%\Publish\Mod\Among Us_Data" /E
ROBOCOPY "%SOLUTION_DIR%\Mods" "%SOLUTION_DIR%\Publish\Mod\Mods" /E
ROBOCOPY "%SOLUTION_DIR%\AmongUs_CE_Installer\bin\Release" "%SOLUTION_DIR%\Publish" 


@ECHO ON
:: Copy All Needed Files
COPY "%TARGET_DIR%\Assembly-CSharp.dll" 									"%SOLUTION_DIR%\Publish\Mod\Among Us_Data\Managed\Assembly-CSharp.dll"
COPY "%SOLUTION_DIR%\Build\Assemblies\System.Runtime.Serialization.dll" 	"%SOLUTION_DIR%\Publish\Mod\Among Us_Data\Managed\System.Runtime.Serialization.dll"
COPY "%SOLUTION_DIR%\Build\Assemblies\System.Numerics.dll" 					"%SOLUTION_DIR%\Publish\Mod\Among Us_Data\Managed\System.Numerics.dll"
COPY "%SOLUTION_DIR%\Build\Assemblies\System.Data.dll" 						"%SOLUTION_DIR%\Publish\Mod\Among Us_Data\Managed\System.Data.dll"
COPY "%TARGET_DIR%\Newtonsoft.Json.dll" 									"%SOLUTION_DIR%\Publish\Mod\Among Us_Data\Managed\Newtonsoft.Json.dll"
COPY "%TARGET_DIR%\MoonSharp.Interpreter.xml" 								"%SOLUTION_DIR%\Publish\Mod\Among Us_Data\Managed\MoonSharp.Interpreter.xml"
COPY "%TARGET_DIR%\MoonSharp.Interpreter.dll" 								"%SOLUTION_DIR%\Publish\Mod\Among Us_Data\Managed\MoonSharp.Interpreter.dll"
@ECHO OFF

set /p DUMMY=Hit ENTER to open result, or close this window to exit...

:: Open Resulting Folder
START %WINDIR%\explorer.exe "%SOLUTION_DIR%\Publish"

