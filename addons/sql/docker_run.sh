docker run -e 'ACCEPT_EULA=Y' --name sql_server -e 'MSSQL_PID=Express' -e 'SA_PASSWORD=strongPwd123!' -p 1433:1433 -d microsoft/mssql-server-linux:latest