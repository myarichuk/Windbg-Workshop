## Hints
* Notice the CPU usage - one of the CPU cores should be used to MAXIMUM. In a more real scenario, 
* When dealing High CPU (perhaps even 100%), usually some threads will take the most user-mode time slices
* The threads with the most user-mode time are suspects as root-causes of being CPU bottlenecks
* *Note that high CPU can be caused by lots of GC cycles as well*

# Userful commands
* ``!runaway`` will allow ordering running threads by user-mode time they spent running 
* ``~{thread id]}`` - dispay information about a thread

# Useful links
* https://theartofdev.com/windbg-cheat-sheet/
* https://improve.dk/debugging-in-production-part-1-analyzing-100-cpu-usage-using-windbg/
