$baget = "http://localhost:9001/v3/index.json"
$files = Get-ChildItem -Path .\packages -Recurse -File | Where-Object { $_.Name -like "*.nupkg" }

foreach($f in $files) { dotnet nuget push $f.FullName -s $baget --skip-duplicate }
