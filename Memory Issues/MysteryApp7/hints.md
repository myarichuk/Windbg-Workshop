## Hints
* Take a look at the heap, especially the Large Object Heap (LOH)
* Track LOH allocations over time
* Are there many large arrays lying around?
* You can check LOH fragmentation

# Useful commands
* ``!dumpheap -stat``
* ``!dumpheap -min 85000``
* ``!gcroot`` on LOH arrays