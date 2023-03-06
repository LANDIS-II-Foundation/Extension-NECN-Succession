rem set working_dir = %cd%
rem TODO change this so that it will automatically pass the current directory to the script

rem print(working_dir)

Rscript --verbose create_normals.R %cd% "./NECN" "./NECN"

pause