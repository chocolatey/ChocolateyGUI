Get-ChildItem -Path $PSScriptRoot\gui -Filter *.ps1 -Recurse | ForEach-Object { . $_.FullName }
