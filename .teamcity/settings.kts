import jetbrains.buildServer.configs.kotlin.v2019_2.*
import jetbrains.buildServer.configs.kotlin.v2019_2.buildSteps.script
import jetbrains.buildServer.configs.kotlin.v2019_2.buildSteps.powerShell
import jetbrains.buildServer.configs.kotlin.v2019_2.buildFeatures.pullRequests
import jetbrains.buildServer.configs.kotlin.v2019_2.triggers.vcs
import jetbrains.buildServer.configs.kotlin.v2019_2.triggers.ScheduleTrigger
import jetbrains.buildServer.configs.kotlin.v2019_2.triggers.schedule
import jetbrains.buildServer.configs.kotlin.v2019_2.vcs.GitVcsRoot

project {
    buildType(ChocolateyGUI)
    buildType(ChocolateyGUISchd)
    buildType(ChocolateyGUIQA)
    buildType(ChocolateyGUISign)
}

object ChocolateyGUI : BuildType({
    id = AbsoluteId("ChocolateyGUI")
    name = "Chocolatey GUI (Built with Unit Tests)"

    artifactRules = """
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
            scriptContent = """
                build.official.bat --verbosity=diagnostic --target=CI --testExecutionType=unit --shouldRunOpenCover=false
            """.trimIndent()
        }
    }

    triggers {
        vcs {
            branchFilter = ""
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

object ChocolateyGUISchd : BuildType({
    id = AbsoluteId("ChocolateyGUISchd")
    name = "Chocolatey GUI (Scheduled Integration Testing)"

    artifactRules = """
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

        script {
            name = "Call Cake"
            scriptContent = """
                build.bat --verbosity=diagnostic --target=CI --testExecutionType=all --shouldRunOpenCover=false --shouldRunAnalyze=false --shouldRunIlMerge=false --shouldObfuscateOutputAssemblies=false --shouldRunChocolatey=false --shouldRunNuGet=false --shouldAuthenticodeSignMsis=false --shouldAuthenticodeSignOutputAssemblies=false --shouldAuthenticodeSignPowerShellScripts=false
            """.trimIndent()
        }
    }

    triggers {
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
})

object ChocolateyGUIQA : BuildType({
    id = AbsoluteId("ChocolateyGUIQA")
    name = "Chocolatey GUI (SonarQube)"

    artifactRules = """
    """.trimIndent()

    params {
        param("env.vcsroot.branch", "%vcsroot.branch%")
        param("env.Git_Branch", "%teamcity.build.vcs.branch.ChocolateyGUI_ChocolateyGuiVcsRoot%")
        param("env.SONARQUBE_ID", "chocolateygui")
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

                    choco install windows-sdk-7.1 netfx-4.0.3-devpack visualstudio2019buildtools netfx-4.8-devpack dotnet-6.0-runtime --confirm --no-progress
                    exit ${'$'}LastExitCode
                """.trimIndent()
            }
        }

        script {
            name = "Call Cake"
            scriptContent = """
                build.bat --verbosity=diagnostic --target=CI --testExecutionType=none --shouldRunAnalyze=false --shouldRunIlMerge=false --shouldObfuscateOutputAssemblies=false --shouldRunChocolatey=false --shouldRunNuGet=false --shouldRunSonarQube=true --shouldRunDependencyCheck=true --shouldAuthenticodeSignMsis=false --shouldAuthenticodeSignOutputAssemblies=false --shouldAuthenticodeSignPowerShellScripts=false
            """.trimIndent()
        }
    }

    triggers {
        schedule {
            schedulingPolicy = weekly {
                dayOfWeek = ScheduleTrigger.DAY.Saturday
                hour = 2
                minute = 45
            }
            branchFilter = """
                +:<default>
            """.trimIndent()
            triggerBuild = always()
            withPendingChangesOnly = false
        }
    }
})

object ChocolateyGUISign : BuildType({
    id = AbsoluteId("ChocolateyGUISign")
    name = "Chocolatey GUI (Script Signing)"

    artifactRules = """
    """.trimIndent()

    params {
        param("env.vcsroot.branch", "%vcsroot.branch%")
        param("env.Git_Branch", "%teamcity.build.vcs.branch.ChocolateyGUI_ChocolateyGuiVcsRoot%")
        param("env.FORCE_OFFICIAL_AUTHENTICODE_SIGNATURE", "true")
        param("teamcity.git.fetchAllHeads", "true")
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

                    choco install windows-sdk-7.1 netfx-4.0.3-devpack dotnet-6.0-runtime --confirm --no-progress
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
            scriptContent = """
                build.official.bat --verbosity=diagnostic --target=Sign-PowerShellScripts --exclusive
            """.trimIndent()
        }
    }

    triggers {
        vcs {
            triggerRules = """
                +:nuspec/**/*.ps1
            """.trimIndent()
            branchFilter = "+:develop"
        }
    }

    requirements {
        doesNotExist("docker.server.version")
        doesNotContain("teamcity.agent.name", "Docker")
    }
})
