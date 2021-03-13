## Hints
* Multiple mini-dumps taken with 5 second interval from each other may be helpful
* Don't forget to load SOS extension before doing anything
* Take a look at multiple threads in *multiple* dumps and try to see a pattern
* Deadlock-like symptoms do not necessarily means there is a deadlock
* Sometimes adding logging can help -> "printf debugging" can help diagnose what *exactly* is the issue

# Userful commands
* ``!eestack -short -ee`` - display managed stacks for all locked and managed threads
* ``~~[OS Thread Id]s`` - changes current thread context
* If needed, ``!clrstack -p`` can be used to inspect managed stack trace with method parameters


# Useful links
* https://theartofdev.com/windbg-cheat-sheet/
