function Set-FeatureState {
    [CmdletBinding()]
    param(
        [Parameter(Mandatory=$true)]
        [string] $FeatureName,

        [Parameter(Mandatory=$true)]
        [bool] $EnableFeature,

        [Parameter(Mandatory=$true)]
        [bool] $Global
    )

    if ($EnableFeature) {
        if($Global) {
            Write-Output "Enabling $FeatureName globally..."
            Start-ChocolateyProcessAsAdmin -Statements "feature enable --name=$FeatureName --global" -ExeToRun "chocolateyguicli"
        } else {
            Write-Output "Enabling $FeatureName..."
            Start-ChocolateyProcessAsAdmin -Statements "feature enable --name=$FeatureName" -ExeToRun "chocolateyguicli"
        }
    } else {
        if($Global) {
            Write-Output "Disabling $FeatureName globally..."
            Start-ChocolateyProcessAsAdmin -Statements "feature disable --name=$FeatureName --global" -ExeToRun "chocolateyguicli"
        } else {
            Write-Output "Disabling $FeatureName..."
            Start-ChocolateyProcessAsAdmin -Statements "feature disable --name=$FeatureName" -ExeToRun "chocolateyguicli"
        }
    }
}

function Set-Config {
    [CmdletBinding()]
    param(
        [Parameter(Mandatory=$true)]
        [string] $ConfigName,

        [Parameter(Mandatory=$true)]
        [string] $ConfigValue,

        [Parameter(Mandatory=$true)]
        [bool] $Global
    )

    if($Global) {
        Write-Output "Setting $ConfigName globally..."
        Start-ChocolateyProcessAsAdmin -Statements "config set --name=$ConfigName --value=$ConfigValue" -ExeToRun "chocolateyguicli"
    } else {
        Write-Output "Setting $ConfigName..."
    Start-ChocolateyProcessAsAdmin -Statements "config set --name=$ConfigName --value=$ConfigValue --global" -ExeToRun "chocolateyguicli"
    }
}

function Set-UserSettings {
    [CmdletBinding()]
    param(
        [hashtable]$pp = ( Get-PackageParameters )

    )
    # Features
    $applyGlobally = $pp.ContainsKey("Global")

    if($pp.ContainsKey("ShowConsoleOutput")) {
        Set-FeatureState "ShowConsoleOutput" ($pp.ShowConsoleOutput -eq $true) $applyGlobally
    }

    if($pp.ContainsKey("DefaultToTileViewForLocalSource")) {
        Set-FeatureState "DefaultToTileViewForLocalSource" ($pp.DefaultToTileViewForLocalSource -eq $true) $applyGlobally
    }

    if($pp.ContainsKey("DefaultToTileViewForRemoteSource")) {
        Set-FeatureState "DefaultToTileViewForRemoteSource" ($pp.DefaultToTileViewForRemoteSource -eq $true) $applyGlobally
    }

    if($pp.ContainsKey("UseDelayedSearch")) {
        Set-FeatureState "UseDelayedSearch" ($pp.UseDelayedSearch -eq $true) $applyGlobally
    }

    if($pp.ContainsKey("PreventPreload")) {
        Set-FeatureState "PreventPreload" ($pp.PreventPreload -eq $true) $applyGlobally
    }

    if($pp.ContainsKey("PreventAutomatedOutdatedPackagesCheck")) {
        Set-FeatureState "PreventAutomatedOutdatedPackagesCheck" ($pp.PreventAutomatedOutdatedPackagesCheck -eq $true) $applyGlobally
    }

    if($pp.ContainsKey("ExcludeInstalledPackages")) {
        Set-FeatureState "ExcludeInstalledPackages" ($pp.ExcludeInstalledPackages -eq $true) $applyGlobally
    }

    if($pp.ContainsKey("ShowAggregatedSourceView")) {
        Set-FeatureState "ShowAggregatedSourceView" ($pp.ShowAggregatedSourceView -eq $true) $applyGlobally
    }

    if($pp.ContainsKey("ShowAdditionalPackageInformation")) {
        Set-FeatureState "ShowAdditionalPackageInformation" ($pp.ShowAdditionalPackageInformation -eq $true) $applyGlobally
    }

    if($pp.ContainsKey("AllowNonAdminAccessToSettings")) {
        Set-FeatureState "AllowNonAdminAccessToSettings" ($pp.AllowNonAdminAccessToSettings -eq $true) $applyGlobally
    }

    if($pp.ContainsKey("UseKeyboardBindings")) {
        Set-FeatureState "UseKeyboardBindings" ($pp.UseKeyboardBindings -eq $true) $applyGlobally
    }

    if($pp.ContainsKey("HidePackageDownloadCount")) {
        Set-FeatureState "HidePackageDownloadCount" ($pp.HidePackageDownloadCount -eq $true) $applyGlobally
    }

    if($pp.ContainsKey("PreventAllPackageIconDownloads")) {
        Set-FeatureState "PreventAllPackageIconDownloads" ($pp.PreventAllPackageIconDownloads -eq $true) $applyGlobally
    }

    if($pp.ContainsKey("HideAllRemoteChocolateySources")) {
        Set-FeatureState "HideAllRemoteChocolateySources" ($pp.HideAllRemoteChocolateySources -eq $true) $applyGlobally
    }

    if($pp.ContainsKey("DefaultToDarkMode")) {
        Set-FeatureState "DefaultToDarkMode" ($pp.DefaultToDarkMode -eq $true) $applyGlobally
    }

    # config
    if($pp.ContainsKey("OutdatedPackagesCacheDurationInMinutes")) {
        Set-Config "OutdatedPackagesCacheDurationInMinutes" ($pp.OutdatedPackagesCacheDurationInMinutes) $applyGlobally
    }

    if($pp.ContainsKey("DefaultSourceName")) {
        Set-Config "DefaultSourceName" ($pp.DefaultSourceName) $applyGlobally
    }
}