## Hints
* Take a look at managed heap, especially over time
* If some type has too high of a count, it is suspicious
* Look at reference roots...

# Userful commands
* ``!dumpheap -stat``
* ``!dumpheap -stat -type [type name]``
* ``!dumpheap -type [type name]``
* ``!gcroot [address]``

# Useful links
* https://theartofdev.com/windbg-cheat-sheet/
* http://www.graymatterdeveloper.com/2020/02/12/setting-up-windbg/
