## Hints
* Don't forget to load SOS extension before doing anything
* When process hangs, deadlocks and threading issue is not necessarily the issue...
* Look at the running threads and watch for "weird" things that stand out - like thread running time that is much bigger than the rest

# Userful commands
* ``!runaway`` shows user-mode time of currently running threads
* ``!clrstack`` shows stack trace of current thread
* Use ``~{thread Id}s`` or ``~~[{OS Thread Id}]s`` to switch thread context

# Useful links
* https://theartofdev.com/windbg-cheat-sheet/
* http://www.graymatterdeveloper.com/2020/02/12/setting-up-windbg/
