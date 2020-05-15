dotnet publish -r linux-arm -c release /p:PublishSingleFile=true

xcopy "install.sh" bin\Release\netcoreapp3.1\linux-arm\publish\ /y
xcopy "konfig.json" bin\Release\netcoreapp3.1\linux-arm\publish\ /y