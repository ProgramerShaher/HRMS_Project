-- نفذ هذا للتحقق من أي قاعدة بيانات أنت متصل
SELECT SYS_CONTEXT('USERENV', 'CON_NAME') AS container_name FROM DUAL;