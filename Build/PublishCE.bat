@ECHO OFF
SET TARGET_DIR=%~1
SET SOLUTION_DIR=%CD%

:: Clean Publish Directory
RMDIR /S "%SOLUTION_DIR%\Publish"
MKDIR "%SOLUTION_DIR%\Publish"
MKDIR "%SOLUTION_DIR%\Publish\Hats"
MKDIR "%SOLUTION_DIR%\Publish\Skins"
MKDIR "%SOLUTION_DIR%\Publish\Lua"
MKDIR "%SOLUTION_DIR%\Among Us_Data\Managed"

:: Copy All Needed Folder
ROBOCOPY "%SOLUTION_DIR%\Among Us_Data" "%SOLUTION_DIR%\Publish\Among Us_Data" /E /NFL /NDL /NJH /NJS /nc /ns /np
ROBOCOPY "%SOLUTION_DIR%\Hats" "%SOLUTION_DIR%\Publish\Hats" /E /NFL /NDL /NJH /NJS /nc /ns /np
ROBOCOPY "%SOLUTION_DIR%\Skins" "%SOLUTION_DIR%\Publish\Skins" /E /NFL /NDL /NJH /NJS /nc /ns /np
ROBOCOPY "%SOLUTION_DIR%\Lua" "%SOLUTION_DIR%\Publish\Lua" /E /NFL /NDL /NJH /NJS /nc /ns /np
ROBOCOPY "%SOLUTION_DIR%\DepotDownloader" "%SOLUTION_DIR%\Publish" /E /NFL /NDL /NJH /NJS /nc /ns /np

:: Copy All Needed Files
COPY "%TARGET_DIR%\Assembly-CSharp.dll" 									"%SOLUTION_DIR%\Publish\Among Us_Data\Managed\Assembly-CSharp.dll"
COPY "%SOLUTION_DIR%\Build\Assemblies\System.Runtime.Serialization.dll" 	"%SOLUTION_DIR%\Publish\Among Us_Data\Managed\System.Runtime.Serialization.dll"
COPY "%SOLUTION_DIR%\Build\Assemblies\System.Numerics.dll" 					"%SOLUTION_DIR%\Publish\Among Us_Data\Managed\System.Numerics.dll"
COPY "%SOLUTION_DIR%\Build\Assemblies\System.Data.dll" 						"%SOLUTION_DIR%\Publish\Among Us_Data\Managed\System.Data.dll"
COPY "%TARGET_DIR%\Newtonsoft.Json.dll" 									"%SOLUTION_DIR%\Publish\Among Us_Data\Managed\Newtonsoft.Json.dll"
COPY "%TARGET_DIR%\MoonSharp.Interpreter.xml" 								"%SOLUTION_DIR%\Publish\Among Us_Data\Managed\MoonSharp.Interpreter.xml"
COPY "%TARGET_DIR%\MoonSharp.Interpreter.dll" 								"%SOLUTION_DIR%\Publish\Among Us_Data\Managed\MoonSharp.Interpreter.dll"

set /p DUMMY=Hit ENTER to open result, or close this window to exit...

:: Open Resulting Folder
START %WINDIR%\explorer.exe "%SOLUTION_DIR%\Publish"

