function Get-GuiFeature {
    <#
        .Synopsis
            Helper function to call chocolateyguicli and return the feature information as a PSCustomObject.
    #>
    [CmdletBinding()]
    param(
        [string[]]$Feature = '*'
    )

    $featureList = (Invoke-GuiCli feature list -r).Lines |
        ConvertFrom-Csv -Delimiter '|' -Header Name, State, Description |
        Select-Object Name, @{Name = 'enabled'; Expression = { $_.State -eq 'Enabled' } }, Description
    foreach ($ftr in $Feature) {
        $featureList | Where-Object Name -Like $ftr
    }
}
