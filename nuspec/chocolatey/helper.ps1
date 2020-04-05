function Set-FeatureState {
    [CmdletBinding()]
    param(
        [Parameter(Mandatory=$true)]
        [string] $FeatureName,

        [Parameter(Mandatory=$true)]
        [bool] $EnableFeature
    )
  
    if ($EnableFeature) {
        Write-Output "Enabling $FeatureName..."
        Start-ChocolateyProcessAsAdmin -Statements "feature enable --name=$FeatureName" -ExeToRun "chocolateyguicli"
    } else {
        Write-Output "Disabling $FeatureName..."
        Start-ChocolateyProcessAsAdmin -Statements "feature disable --name=$FeatureName" -ExeToRun "chocolateyguicli"
    }
}

function Set-UserSettings {
    [CmdletBinding()]
    param(
        [hashtable]$pp = ( Get-PackageParameters )
    
    )
    # Features
    if($pp.ContainsKey("ShowConsoleOutput")) {
        Set-FeatureState "ShowConsoleOutput" ($pp.ShowConsoleOutput -eq $true)
    }
    
    if($pp.ContainsKey("DefaultToTileViewForLocalSource")) {
        Set-FeatureState "DefaultToTileViewForLocalSource" ($pp.DefaultToTileViewForLocalSource -eq $true)
    }
    
    if($pp.ContainsKey("DefaultToTileViewForRemoteSource")) {
        Set-FeatureState "DefaultToTileViewForRemoteSource" ($pp.DefaultToTileViewForRemoteSource -eq $true)
    }
    
    if($pp.ContainsKey("UseDelayedSearch")) {
        Set-FeatureState "UseDelayedSearch" ($pp.UseDelayedSearch -eq $true)
    }
    
    if($pp.ContainsKey("PreventPreload")) {
        Set-FeatureState "PreventPreload" ($pp.PreventPreload -eq $true)
    }
    
    if($pp.ContainsKey("ExcludeInstalledPackages")) {
        Set-FeatureState "ExcludeInstalledPackages" ($pp.ExcludeInstalledPackages -eq $true)
    }
    
    if($pp.ContainsKey("ShowAggregatedSourceView")) {
        Set-FeatureState "ShowAggregatedSourceView" ($pp.ShowAggregatedSourceView -eq $true)
    }

    if($pp.ContainsKey("ShowAdditionalPackageInformation")) {
        Set-FeatureState "ShowAdditionalPackageInformation" ($pp.ShowAdditionalPackageInformation -eq $true)
    }

    if($pp.ContainsKey("AllowNonAdminAccessToSettings")) {
        Set-FeatureState "AllowNonAdminAccessToSettings" ($pp.AllowNonAdminAccessToSettings -eq $true)
    }

    if($pp.ContainsKey("UseKeyboardBindings")) {
        Set-FeatureState "UseKeyboardBindings" ($pp.UseKeyboardBindings -eq $true)
    }

    if($pp.ContainsKey("HidePackageDownloadCount")) {
        Set-FeatureState "HidePackageDownloadCount" ($pp.HidePackageDownloadCount -eq $true)
    }

    # config    
    if($pp.ContainsKey("OutdatedPackagesCacheDurationInMinutes")) {
        Start-ChocolateyProcessAsAdmin -Statements "config set --name=OutdatedPackagesCacheDurationInMinutes --value=$($pp.OutdatedPackagesCacheDurationInMinutes)" -ExeToRun "chocolateyguicli"
    }
}