## error
- Could not find class: Microsoft.AspNetCore.Components.WebAssembly.Hosting:EntrypointInvoker in assembly Microsoft.AspNetCore.Components.WebAssembly
- https://github.com/dotnet/aspnetcore/issues/38436

## disable output caching
-https://docs.microsoft.com/en-us/iis/manage/managing-performance-settings/configure-iis-7-output-caching

##Publish Steps
heroku login
dotnet publish -c Release -o release
heroku static:deploy -a cadwiki