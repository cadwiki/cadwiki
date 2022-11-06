$scriptPath = split-path -parent $MyInvocation.MyCommand.Definition

$templateFolder = $scriptPath
$postFolder = "$templateFolder\BlogApp\wwwroot\posts"
$mdFile = "post-template.md"
$jsonFile = "post-template.json"

Set-Location -Path $postFolder -PassThru
$countFiles = Get-ChildItem .\* -Recurse | where {$_.extension -in ".json"} 
write-host ("found {0} post json files in directory." -f $countFiles.Count)

Copy-Item "$templateFolder\$mdFile" -Destination "$postFolder"
Copy-Item "$templateFolder\$jsonFile" -Destination "$postFolder"
write-host "copied new template files."

$content = Get-Content $postFolder\$jsonFile
$json = $content | ConvertFrom-Json
$date = (Get-Date)
$json.DateCreated = $date.Year.ToString() + "/" + $date.Month.ToString() + "/" + $date.Day.ToString()
$postId = $countFiles.Count + 1
$json.Id = $postId.ToString()

$json | ConvertTo-Json | Set-Content -Path $postFolder\$jsonFile