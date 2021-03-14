## Hints
* Computation-bound algorithms can cause high-CPU
* Computation-bound algorithms may also cause threadpool starvation
* Sometimes ``Threads`` run parallel tasks and sometimes threadpool ``Task`` - make sure to inspect all of them
* Check threadpool tasks for unusual activity, if "scheduled" tasks count ONLY increases OR threadpool thread count increases, this means likely threadpool starvation
* Take multiple dumps to ensure that "scheduled" task count only increases.
*Note: MEX plugin that is mentioned in StackOverflow answer link below can be useful only if you diagnose full .Net framework dumps, it won't work for .Net Core or .Net 5*

# Userful commands
* ``!dumpasync -tasks -completed`` will show all existing tasks and their status
* ``!runaway`` will allow ordering running threads by how long they run
* ``!clstack`` and ``~[thread #]s`` will allow looking at stack traces of a certain thread
* ``!eestack -short -ee`` will show stack traces of all the 'interesting' threads
* ``!threads`` - list process threads - both native and managed


# Useful links
* https://theartofdev.com/windbg-cheat-sheet/
* https://stackoverflow.com/a/44315348/320103
