## Hints
* Take a look at managed heap, especially over time
* If some type has too high of a count, it is suspicious
* References are not always the causes of leaks. Take a look at finalizer queue! (look how it changes over time)

# Userful commands
* ``!dumpheap -stat``
* ``!dumpheap -stat -type [type name]``
* ``!dumpheap -type [type name]``
* ``!gcroot [address]``
* ``!finalizequeue`` - list contents of f-reachable and finalization queue
* ``!frq`` (SOSEX) - list contents of f-reachable queue
* ``!finq`` (SOSEX) - list contents of finalization queue

# Useful links
* https://theartofdev.com/windbg-cheat-sheet/
* http://www.stevestechspot.com/downloads/sosex_64.zip
