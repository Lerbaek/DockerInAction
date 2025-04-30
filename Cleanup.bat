@echo off

rem Find all folders named "obj" recursively
for /d /r %%a in (obj) do (

    rem Delete each folder using rd command
    rd /s /q "%%a"
)