@echo off

rem Find all folders named "bin" recursively
for /d /r %%a in (bin) do (

    rem Delete each folder using rd command
    rd /s /q "%%a"
)