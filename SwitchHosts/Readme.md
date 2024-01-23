sc.exe create "SwitchHosts" binpath="C:\SwitchHosts\SwitchHosts.exe" start="delayed-auto" DisplayName="SwitchHosts"
sc.exe delete "SwitchHosts"
sc.exe start "SwitchHosts"
sc.exe stop "SwitchHosts"
