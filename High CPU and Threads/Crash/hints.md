## Hints
* Use the command to capture a dump: ``procdump64.exe -e 1 -g -b -ma -w Crash.exe`` (notice that we are setting up to take a first-chance exception here)
* Try using the command ``procdump64.exe -e -ma -w Crash.exe`` to take a dump at second-chance exception and see the difference
* Don't forget to load SOS extension before doing anything
* Notice: dump take during second-chance exception would be useless in this case

# Userful commands
* ``~*k`` - can be used to get overview at what threads are doing
* ``!analyze -v``


# Useful links
* https://theartofdev.com/windbg-cheat-sheet/
* http://www.graymatterdeveloper.com/2020/02/12/setting-up-windbg/
