cd ./BlogApp/wwwroot/posts
sh -c 'ls -d -- "$@" > post.manifest' sh *.json
cd ../../..
dotnet publish -c Release -o release