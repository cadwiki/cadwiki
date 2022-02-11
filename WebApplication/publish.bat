START /B CMD /C CALL "build-post-manifest.bat"
call heroku login -i
dotnet publish -c Release -o release
call heroku static:deploy -a cadwiki
timeout /t -1