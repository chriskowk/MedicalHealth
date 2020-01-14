Consul下载地址：https://www.consul.io/downloads.html （Win64位版本）
-------启动CMD窗口---------
启动Consul（开发模式）：
D:\(consul.exe所在路径)> consul agent -dev
注册两个MyServiceA服务：
D:\(bin\Debug\netcoreapp3.1路径)> dotnet .\MyServiceA.dll --urls "http://127.0.0.1:5001"
D:\(bin\Debug\netcoreapp3.1路径)> dotnet .\MyServiceA.dll --urls "http://127.0.0.1:5002"
