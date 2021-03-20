## Hints
* Don't forget to load SOS extension before doing anything
* There are multiple ways to root cause a deadlock
* Using SOSEX a shortest and the most convenient way to root cause in THIS case
* SyncBlk is a lock that uses specific objects as mutexes. Object's address is very useful.
* Take a look at parameters of unmanaged stacks, you can find threads waiting for a lock in this way
* Looking at parameters of **coreclr!JIT_MonEnter_Helper** may be helpful

# Userful commands
* ``kv`` shows unmanaged stack trace **with** parameters
* ``~*kv`` - can be used to get overview at what threads are doing including their *parameters*
* ``!syncblk`` can be used to see managed locks
* ``!mk`` can display both managed and unmanaged stack trace (SOSEX)
* ``!mframe [frame ID] can focus on local context of the frame (SOSEX)
* ``!mdv`` can display stack of the frame, including type and address of objects (SOSEX)


# Useful links
* https://theartofdev.com/windbg-cheat-sheet/
* http://www.graymatterdeveloper.com/2020/02/12/setting-up-windbg/
* https://github.com/lowleveldesign/debug-recipes/blob/master/debugging-using-windbg/sosex.help.txt
