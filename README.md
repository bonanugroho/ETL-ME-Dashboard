# ETL-ME-Dashboard
Server & Network Operations Dashboard, Custom dashboard is used to facilitate monitoring network and server operations. 
Data Loaded from two different Manage engine tools, namely Netflow analyzer and Application Manager. 
The netflow analyzer datasource is loaded from PostgreSQL and fed to Datamart in Ms SQL Server. 
For datasource Application manager loaded from REST API that has been provided and fed to Ms SQL Server. 
ETL uses custom apps developed using C# .Net. Apps are registered in Windows Task Scheduller. 
The output is displayed as a custom web dasboard.
