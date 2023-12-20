Import-Module ../helpers/gui-helpers.psm1

Describe "chocolateyguicli" -Tag ChocolateyGuiCli {
    BeforeDiscovery {
        # Perhaps a better way is to pull these from the LiteDB similar to how we do features from the xml in CLI, but this will do for an initial setup.
        $Features = Get-GuiFeature
    }

    Context "Basic CLI functionality" {
        BeforeAll {
            $Output = Invoke-GuiCli
        }

        It 'Should exit Success (0)' {
            $Output.ExitCode | Should -Be 0 -Because $Output.String
        }

        It 'Should output appropriate message' {
            $Output.Lines | Should -Contain "Please run chocolateyguicli with 'chocolateyguicli -?' or 'chocolateyguicli <command> -?' for specific help on each command"
        }
    }

    Context "Lists available features" {
        BeforeAll {
            $Output = Invoke-GuiCli feature list
        }

        It "Contains reference of the option (<_.Name>)" -ForEach $Features {
            $Output.String | Should -Match $Name
        }
    }

    Context "Toggles the feature (<_.Name>) successfully" -ForEach $Features {
        BeforeAll {
            if ($Enabled) {
                $DisableOutput = Invoke-GuiCli feature disable --name $_.Name
                $EnableOutput = Invoke-GuiCli feature enable --name $_.Name
            } else {
                $EnableOutput = Invoke-GuiCli feature enable --name $_.Name
                $DisableOutput = Invoke-GuiCli feature disable --name $_.Name
            }
        }

        It "Should exit success (0)" {
            $EnableOutput.ExitCode | Should -Be 0 -Because $EnableOutput.String
            $DisableOutput.ExitCode | Should -Be 0 -Because $DisableOutput.String
        }
    }
}
