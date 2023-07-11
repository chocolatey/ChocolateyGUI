[CmdletBinding(DefaultParameterSetName = "tag")]
param(
    # The location where Chocolatey CLI sources are located,
    # or will be located if the directory does not already exist.
    # If not specified and the environment variable `CHOCO_SOURCE_LOCATION`
    # is not definied, the location will default to `$env:TEMP\
    [Alias("ChocoLocation")]
    [string] $ChocoSourceLocation = $env:CHOCO_SOURCE_LOCATION,

    # Checkout a specific tag of your own choosing.
    # This value can also be a specific commit or a branch, but
    # will be notified as being a tag.
    # NOTE: Only tags already pulled down will be considered.
    [Parameter(ParameterSetName = "tag")]
    [string] $CheckoutTag = $null,

    # Checkout the latest tag available in the Chocolatey Source Location.
    # NOTE: Only tags already pulled down will be considered.
    [Parameter(ParameterSetName = "latest")]
    [switch] $CheckoutLatestTag,

    # Try check out a tage with the same name as what is used as a reference
    # in the packages.config file. If the reference specified is not a stable
    # version, the latest tag will be checked out instead.
    # NOTE: Only tags already pulled down will be considered.
    [Parameter(ParameterSetName = 'ref-tag')]
    [switch] $CheckoutRefTag,

    # Remove and clone the specified Chocolatey Source Location again.
    # This is a very destructive operation, and should only be used if
    # you are not interested in any local information.
    [switch] $ForceChocoClone
)

function CheckoutTag {
    param(
        [Parameter(Mandatory)]
        [string] $SourceLocation,

        [string] $TagName
    )
  
    Push-Location "$SourceLocation"
    if (!$TagName) {
        $TagName = . git tag --sort v:refname | Where-Object { $_ -match "^[\d\.]+$" } | Select-Object -last 1
    }

    if ($TagName) {
        git checkout $TagName -q 2>$null
        if ($LASTEXITCODE -eq 0) {
            Write-Host "Checked out Chocolatey CLI tag '$TagName'"
        }
        else {
            $currentBranch = . git branch --show-current
            if ($currentBranch) {
                Write-Warning "Unable to check out tag $TagName. Leaving source in branch $currentBranch"
            }
            else {
                Write-Warning "Unable to check out tag $TagName. Leaving source in commit $(git rev-parse HEAD)"
            }
        }
    }

    Pop-Location
}

Write-Host "We are at $PSScriptRoot"

[xml]$packagesConfigFile = Get-Content -Path "$PSScriptRoot/Source/ChocolateyGui/packages.config"

$chocolateyLibPackageVersion = $($packagesConfigFile.packages.package | Where-Object { $_.id -eq "chocolatey.lib" }).version

if ($CheckoutRefTag) {

    if ($chocolateyLibPackageVersion -match '^[\d\.]+$') {
        $CheckoutTag = $chocolateyLibPackageVersion
    }
    else {
        $CheckoutLatestTag = $true
  }
}

if (!$ChocoSourceLocation) {
    # To allow a default path being used for cloning the repository
    $ChocoSourceLocation = "$env:TEMP\chocoSource"
}

Write-Host "Looking for choco in '$ChocoSourceLocation'"

if ($ForceChocoClone -and (Test-Path $ChocoSourceLocation)) {
    Write-Host "Removing existing Chocolatey CLI Source in '$ChocoSourceLocation'"
    # We use error action stop here, as there may be times the `.git` directory is locked.
    # Having information about this is helpful to rectify the issue.
    Remove-Item $ChocoSourceLocation -Recurse -Force -EA Stop
}

if (!(Test-Path $ChocoSourceLocation)) {
    Write-Host "Cloning Chocolatey CLI Repository to '$ChocoSourceLocation'"
    git clone "https://github.com/chocolatey/choco.git" "$ChocoSourceLocation"

    if ($CheckoutLatestTag) {
        CheckoutTag $ChocoSourceLocation
    }
    elseif ($CheckoutTag) {
        CheckoutTag $ChocoSourceLocation -TagName $CheckoutTag
    }
}
elseif ($CheckoutLatestTag) {
    CheckoutTag $ChocoSourceLocation
}
elseif ($CheckoutTag) {
    CheckoutTag $ChocoSourceLocation -TagName $CheckoutTag
}

if (-not (Test-Path -Path $ChocoSourceLocation)) {
    # We leave this here on purpose in case the cloning of the repository has failed.
    throw "Location '$ChocoSourceLocation' not found; please rerun with the -ChocoSourceLocation parameter or set the CHOCO_SOURCE_LOCATION environment variable."
}

Write-Host "Restore packages on project first..."
& ./build.debug.bat --target='Restore'

Write-Host "Building choco at $ChocoSourceLocation with Debug..."

Push-Location $ChocoSourceLocation
if (Test-Path "recipe.cake") {
    & ./build.debug.bat --target='Run-ILMerge' --shouldRunTests=false --shouldRunAnalyze=false
}
else {
    & ./build.debug.bat
}
Pop-Location

Write-Host "Copying chocolatey artifacts to current Chocolatey Package Version folder..."

$chocolateyLibPackageFolder = "$PSScriptRoot/Source/packages/chocolatey.lib.$chocolateyLibPackageVersion/lib/net48"

if (-not (Test-Path -Path $chocolateyLibPackageFolder)) {
    New-Item -ItemType Directory -Path $chocolateyLibPackageFolder > $null
}

$codeDropLibs = "$ChocoSourceLocation/code_drop/temp/_PublishedLibs/chocolatey_merged"

if (!(Test-Path $codeDropLibs)) {
  $codeDropLibs = "$ChocoSourceLocation/code_drop/chocolatey/lib"
}

Write-Host "Copying chocolatey lib items from '$codeDropLibs/*' to '$chocolateyLibPackageFolder'."
Copy-Item -Path "$codeDropLibs/*" -Destination "$chocolateyLibPackageFolder/" -Force