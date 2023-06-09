import jetbrains.buildServer.configs.kotlin.v2019_2.*
import jetbrains.buildServer.configs.kotlin.v2019_2.buildSteps.script
import jetbrains.buildServer.configs.kotlin.v2019_2.buildSteps.powerShell
import jetbrains.buildServer.configs.kotlin.v2019_2.buildFeatures.pullRequests
import jetbrains.buildServer.configs.kotlin.v2019_2.triggers.vcs
import jetbrains.buildServer.configs.kotlin.v2019_2.triggers.schedule
import jetbrains.buildServer.configs.kotlin.v2019_2.vcs.GitVcsRoot

project {
    buildType(ChocolateyGUI)
}

object ChocolateyGUI : BuildType({
    id = AbsoluteId("ChocolateyGUI")
    name = "Build"

    artifactRules = """
        code_drop/MsBuild.log
        code_drop/MSBuild.msi.log
        code_drop/ChocolateyGUI.msi
        code_drop/TestResults/issues-report.html
        code_drop/Packages/**/*.nupkg
    """.trimIndent()

    params {
        param("env.vcsroot.branch", "%vcsroot.branch%")
        param("env.Git_Branch", "%teamcity.build.vcs.branch.ChocolateyGUI_ChocolateyGuiVcsRoot%")
        param("teamcity.git.fetchAllHeads", "true")
        password("env.TRANSIFEX_API_TOKEN", "credentialsJSON:c81283e6-cf59-5c9e-9766-6f465018a295", display = ParameterDisplay.HIDDEN, readOnly = true)
        password("env.GITHUB_PAT", "%system.GitHubPAT%", display = ParameterDisplay.HIDDEN, readOnly = true)
    }

    vcs {
        root(DslContext.settingsRoot)

        branchFilter = """
            +:*
        """.trimIndent()
    }

    steps {
        powerShell {
            name = "Prerequisites"
            scriptMode = script {
                content = """
                    # Install Chocolatey Requirements
                    if ((Get-WindowsFeature -Name NET-Framework-Features).InstallState -ne 'Installed') {
                        Install-WindowsFeature -Name NET-Framework-Features
                    }

                    choco install windows-sdk-7.1 netfx-4.0.3-devpack visualstudio2019buildtools netfx-4.8-devpack --confirm --no-progress
                    exit ${'$'}LastExitCode
                """.trimIndent()
            }
        }

        step {
            name = "Include Signing Keys"
            type = "PrepareSigningEnvironment"
        }

        script {
            name = "Call Cake"
            scriptContent = scriptContent = """
                IF "%teamcity.build.triggeredBy%" == "Schedule Trigger" (SET TestType=all) ELSE (SET TestType=unit)
                call build.official.bat --verbosity=diagnostic --target=CI --testExecutionType=%%TestType%% --shouldRunOpenCover=false
            """.trimIndent()
        }
    }

    triggers {
        vcs {
            branchFilter = ""
        }
        schedule {
            schedulingPolicy = daily {
                hour = 2
                minute = 0
            }
            branchFilter = """
                +:<default>
            """.trimIndent()
            triggerBuild = always()
			withPendingChangesOnly = false
        }
    }

    features {
        pullRequests {
            provider = github {
                authType = token {
                    token = "%system.GitHubPAT%"
                }
            }
        }
    }
})