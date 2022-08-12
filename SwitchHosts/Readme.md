sc.exe create "SwitchHosts" binpath="C:\SwitchHosts\SwitchHosts.exe"
sc.exe delete "SwitchHosts"
sc.exe start "SwitchHosts"
sc.exe stop "SwitchHosts"
