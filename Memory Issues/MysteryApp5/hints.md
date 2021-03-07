## Hints
* Take a look at managed heap, especially over time
* What could be the relationship between the most numerous objects in code?
* Note: GC cannot release objects that have references to them. How is the reference created in our case?

# Userful commands
* ``!dumpheap -stat``
* ``!dumpheap -stat -type [type name]``
* ``!dumpheap -type [type name]``

# Useful links
* https://theartofdev.com/windbg-cheat-sheet/
* http://www.graymatterdeveloper.com/2020/02/12/setting-up-windbg/
* https://www.codeproject.com/Articles/639493/Events-Demystifying-Common-Memory-Leaks
