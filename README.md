## How to add new posts

Write new .md and .json file for a post here:
./WebApplication/BlogApp/wwwroot/posts/

Call script to build manifest file
./WebApplication/build-post-manifest.bat

## error
- Could not find class: Microsoft.AspNetCore.Components.WebAssembly.Hosting:EntrypointInvoker in assembly Microsoft.AspNetCore.Components.WebAssembly
- https://github.com/dotnet/aspnetcore/issues/38436

## disable output caching
-https://docs.microsoft.com/en-us/iis/manage/managing-performance-settings/configure-iis-7-output-caching

##Publish Steps
heroku plugins:install heroku-cli-static
heroku login
dotnet publish -c Release -o release
heroku static:deploy -a cadwiki