## Hints
* Take a look at managed heap
* Take a look at unmanaged heap (use ``!heap -s`` to see the big picture)
* Take a look at unmanaged heap over time to see the changes
* For ``!heap`` command, **-p** and **-a** flags might be useful
* You can use [gflags tool](https://docs.microsoft.com/en-us/windows-hardware/drivers/debugger/gflags?redirectedfrom=MSDN) to enable saving allocation stack traces by using a command line command.
Enabling tracing:  
```
"C:\Program Files (x86)\Windows Kits\10\Debuggers\x64\"gflags /i [path to mystery app6]\MysteryApp6.exe +ust
```
Disabling tracing:  
```
"C:\Program Files (x86)\Windows Kits\10\Debuggers\x64\"gflags /i [path to mystery app6]\MysteryApp6.exe -ust
```
* 

# Userful commands
* ``!dumpheap``
* ``!heap``

# Useful links
* https://docs.microsoft.com/en-us/windows-hardware/drivers/debugger/-heap
* https://stackoverflow.com/a/35195631
