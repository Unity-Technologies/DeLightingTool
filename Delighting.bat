@echo OFF

SET projectPath=%~dp0
SET output=

SET inputFolder=

SET base=
SET normals=
SET bentNormals=
SET ao=
SET position=
SET mask=
SET separateDarkAreas=
SET forceLocalDelighting=
SET removeHighlights=
SET removeDarkNoise=
SET switchYZ=

:parseArg
IF "%1"=="" goto execute
IF "%1"=="-output" ( SET output=%2 )
IF "%1"=="-inputFolder" ( SET inputFolder=%2 )
IF "%1"=="-base" ( SET base=%2 )
IF "%1"=="-normals" ( SET normals=%2 )
IF "%1"=="-bentNormals" ( SET bentNormals=%2 )
IF "%1"=="-ao" ( SET ao=%2 )
IF "%1"=="-position" ( SET position=%2 )
IF "%1"=="-mask" ( SET mask=%2 )
IF "%1"=="-separateDarkAreas" ( SET separateDarkAreas=%2 )
IF "%1"=="-forceLocalDelighting" ( SET forceLocalDelighting=%2 )
IF "%1"=="-removeHighlights" ( SET removeHighlights=%2 )
IF "%1"=="-removeDarkNoise" ( SET removeDarkNoise=%2 )
IF "%1"=="-switchYZ" ( SET switchYZ=%2 )
SHIFT
goto parseArg

:execute
IF "%output%"=="" GOTO usage
IF "%inputFolder%"=="" (
    IF "%base%"=="" GOTO usage
    IF "%normals%"=="" GOTO usage
    IF "%bentNormals%"=="" GOTO usage
    IF "%ao%"=="" GOTO usage
    IF "%position%"=="" GOTO usage
)

SET args=
IF NOT "%output%"=="" ( SET args=%args% -output %output% )
IF NOT "%inputFolder%"=="" ( SET args=%args% -inputFolder %inputFolder% )
IF NOT "%base%"=="" ( SET args=%args% -base %base% )
IF NOT "%normals%"=="" ( SET args=%args% -normals %normals% )
IF NOT "%bentNormals%"=="" ( SET args=%args% -bentNormals %bentNormals% )
IF NOT "%ao%"=="" ( SET args=%args% -ao %ao% )
IF NOT "%position%"=="" ( SET args=%args% -position %position% )
IF NOT "%mask%"=="" ( SET args=%args% -mask %mask% )
IF NOT "%forceLocalDelighting%"=="" ( SET args=%args% -forceLocalDelighting %forceLocalDelighting% )
IF NOT "%separateDarkAreas%"=="" ( SET args=%args% -separateDarkAreas %separateDarkAreas% )
IF NOT "%removeHighlights%"=="" ( SET args=%args% -removeHighlights %removeHighlights% )
IF NOT "%removeDarkNoise%"=="" ( SET args=%args% -removeDarkNoise %removeDarkNoise% )
IF NOT "%switchYZ%"=="" ( SET args=%args% -switchYZ %switchYZ% )

Unity.exe -quit -logfile delighting.log.txt -projectPath %projectPath% -executeMethod DelightingCLI.Run %args%
ECHO Check logs at %projectPath%/delighting.log.txt
goto eof

:usage
ECHO Usage:
ECHO.
ECHO Delighting.bat [OPTIONS] -output OUTPUT_FILE -inputFolder INPUT_FOLDER
ECHO   Load a folder containing the textures to use to de-light
ECHO   The textures will be selected depending on their suffix
ECHO.
ECHO   output:              Output file (PNG)
ECHO   inputFolder:         Asset folder in Unity project to load
ECHO.
ECHO.
ECHO Delighting.bat [OPTIONS] -output OUTPUT_FILE -base TEXTURE_PATH -normals TEXTURE_PATH -bentNormals TEXTURE_PATH -ao TEXTURE_PATH [-position TEXTURE_PATH] [-mask TEXTURE_PATH]
ECHO   Load a folder containing the textures to use to de-light
ECHO.
ECHO   output:              Output file (PNG)
ECHO   base:                Base texture to de-light
ECHO   normals:             World normals texture
ECHO   bentNormals:         Bent normals texture
ECHO   ao:                  Ambiant occlusion texture
ECHO   position:            Position texture
ECHO   mask:                Mask texture
ECHO.
ECHO.
ECHO OPTIONS:
ECHO   removeHighlights:    Remove highlights threshold (default = 0.5, [0-1])
ECHO   removeDarkNoise:     Remove dark noise threshold (default = 0, [0-1])
ECHO   separateDarkAreas:   Separate dark areas threshold (default = 0, [0-1])
ECHO   forceLocalDelighting:Force to use local delighting (default = 0, [0-1])
ECHO   switchYZ:            Switch Y and Z axis

:eof