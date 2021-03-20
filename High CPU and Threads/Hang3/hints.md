## Hints
* Don't forget to load SOS extension before doing anything
* Commands from previous hands-on won't be helpful here
* Use **dumpasync** to see information about Task state machines and most importantly, take a look at the state machine GC roots.
* Compare continuation tasks with task owning thread stack trace
* The class **SetOnInvokeMres** is inherited from **ManualResetEventSlim** (see it's definition in [.Net repo](https://github.com/dotnet/runtime/blob/062ae9697dc040d50b128af094f465fb3d8bb0f0/src/libraries/System.Private.CoreLib/src/System/Threading/Tasks/Task.cs#L2948))

# Userful commands
* ``!dumpasync`` - also try to use ``-stacks`` and ``-roots`` parameter
* ``!clrstack`` - see managed stack of a current thread. Add ``-p`` to see also parameters of some methods.
* ``~~[OS Thread Id]s`` - changes current thread context


# Useful links
* https://theartofdev.com/windbg-cheat-sheet/
* https://stackoverflow.com/a/57118678/320103
* https://devblogs.microsoft.com/premier-developer/dissecting-the-async-methods-in-c/
