cd ./BlogApp/wwwroot/posts
sh -c 'ls -d -- "$@" > post.manifest' sh *.json
cd ../../..
## Bash add pause prompt for 5 seconds ##
## read -t 5 -p "waiting 5 seconds..."
cd ./BlogApp/
netlify deploy --build
read -p "Press any key to resume ..."