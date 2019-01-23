Vagrant.configure("2") do |config|

  config.vm.define "chocolateygui" do |chocolateygui|
    chocolateygui.vm.box = "StefanScherer/windows_10"
    chocolateygui.vm.box_version = "2018.09.12"
    chocolateygui.windows.halt_timeout = 20
    chocolateygui.winrm.username = "vagrant"
    chocolateygui.winrm.password = "vagrant"
    chocolateygui.vm.guest = :windows
    chocolateygui.vm.communicator = "winrm"

    chocolateygui.vm.hostname = "chocolateygui"
    chocolateygui.windows.set_work_network = true

    chocolateygui.vm.network :forwarded_port, guest: 5985, host: 6985, id: "winrm", auto_correct: true
    chocolateygui.vm.network :forwarded_port, guest: 3389, host: 4389, id: "rdp", auto_correct: true
    chocolateygui.vm.network :forwarded_port, guest: 22, host: 3222, id: "ssh", auto_correct: true

    chocolateygui.vm.synced_folder "packages", "/packages"
    chocolateygui.vm.synced_folder "licenses", "/licenses"

    chocolateygui.vm.provider :virtualbox do |v, override|
      override.vm.network :private_network, ip: "10.10.13.14"
      v.customize ["modifyvm", :id, "--natdnshostresolver1", "on"]
      v.gui = true
      v.customize ["modifyvm", :id, "--vram", 32]
      v.customize ["modifyvm", :id, "--memory", "1024"]
      v.customize ["modifyvm", :id, "--audio", "none"]
      v.customize ["modifyvm", :id, "--clipboard", "bidirectional"]
      v.customize ["modifyvm", :id, "--draganddrop", "hosttoguest"]
      v.customize ["modifyvm", :id, "--usb", "off"]
      # linked clones for speed and size
      v.linked_clone = true if Vagrant::VERSION >= '1.8.0'
    end

    # privileged:false - https://github.com/hashicorp/vagrant/issues/9138
    # Auto Login issue corrected, as per discussion here: https://twitter.com/stefscherer/status/1011120268222304256
    chocolateygui.vm.provision "shell", privileged: false, inline: <<-SHELL
      Set-ItemProperty "HKLM:\\SOFTWARE\\Microsoft\\Windows NT\\CurrentVersion\\Winlogon" -Name AutoAdminLogon -Value 1
      Set-ItemProperty "HKLM:\\SOFTWARE\\Microsoft\\Windows NT\\CurrentVersion\\Winlogon" -Name DefaultUserName -Value "vagrant"
      Set-ItemProperty "HKLM:\\SOFTWARE\\Microsoft\\Windows NT\\CurrentVersion\\Winlogon" -Name DefaultPassword -Value "vagrant"
      Remove-ItemProperty "HKLM:\\SOFTWARE\\Microsoft\\Windows NT\\CurrentVersion\\Winlogon" -Name AutoAdminLogonCount -Confirm -ErrorAction SilentlyContinue
    SHELL
    chocolateygui.vm.provision "reload"
    chocolateygui.vm.provision :shell, :path => "shell/PrepareWindows.ps1", privileged: false
    chocolateygui.vm.provision :shell, :path => "shell/SetWindowsPreferences.ps1", privileged: false
    chocolateygui.vm.provision :shell, :path => "shell/InstallChocolatey.ps1", privileged: false
    chocolateygui.vm.provision :shell, :path => "shell/NotifyGuiAppsOfEnvironmentChanges.ps1", privileged: false
  end
end
