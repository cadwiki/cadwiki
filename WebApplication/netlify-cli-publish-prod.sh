dotnet publish -c Release -o release
cd ./release/wwwroot
netlify deploy --prod .
read -p "Press any key to resume ..."