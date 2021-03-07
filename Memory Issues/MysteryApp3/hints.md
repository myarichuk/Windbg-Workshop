## Hints
* Take a look at managed heap, especially over time
* If some type has too high of a count, it is suspicious
* Look at references
* Reference leaks can be internal and not visible in the code of the project

# Userful commands
* ``!dumpheap -stat``
* ``!dumpheap -stat -type [type name]``
* ``!dumpheap -type [type name]``
* ``!gcroot [address]``

# Useful links
* https://theartofdev.com/windbg-cheat-sheet/
* https://github.com/dotnet/runtime/issues/25197#issuecomment-371505770
