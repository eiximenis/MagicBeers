@echo off
REM This is a sample deploy file. A real prod scenario would use a CD pipeline

REM Create the secret used for binding to the SQL
call kubectl create secret generic binding-production-sqlserver-secret --from-literal="connectionstring=Server=sql-mssql-linux;User Id=sa;Password=Pass@word1;Initial Catalog=statsdb"

REM Deploy the application using type
PUSHD ..
call tye deploy tye-k8s.yaml -v debug
POPD




