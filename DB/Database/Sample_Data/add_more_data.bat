@echo off
echo ========================================
echo Adding Additional Data to HR_CORE
echo ========================================
echo.

sqlplus HR_CORE/Pwd_Core_123@localhost:1521/xepdb1 @02_ADDITIONAL_HR_CORE_DATA.sql

echo.
echo ========================================
echo Done!
echo ========================================
pause
