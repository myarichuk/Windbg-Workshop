## Hints
* Slowness may be a result of threading issues
* In this scenario it is enough to find a way of identifying bottlenecks that block execution
* WinDbg command *syncblk* can show lock contention
* Threadpool's big queue length *may* be a symptom of an issue
* Don't forget to load SOS, of course
* Try to see the big picture -> what process threads are doing?
* Possibly multiple dumps will be required to see the issue

# Userful commands
* ``!syncblk`` will show managed ``Monitor`` based locks
* ``!clrstack`` will show stack trace of current thread
* ``~~[native thread id]s`` sets context to certain thread 
* ``!eestack`` shows stack traces of all threads
* ``!gcroot`` shows reference path from GC root to the specified object
# Useful links
* https://theartofdev.com/windbg-cheat-sheet/