@echo off
set currdir="%CD%"
cd %appdata%
rd /s /q cleansync
cd %currdir%
if exist cleansync.exe goto delete
goto end
:delete	
	set name="%~p0"
	cd \
	rd /s /q %name%
:end