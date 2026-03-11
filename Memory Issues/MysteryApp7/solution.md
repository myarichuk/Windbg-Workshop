## Solution

1. ``dotnet-counters monitor -p [pid] --counters System.Runtime``
Notice the ``LOH Size (MB)`` metric keeps climbing steadily.

2. Run ``dotnet-dump collect -p [pid]``
3. Analyze the dump with ``dotnet-dump analyze [dump_path]``
4. Use ``dumpheap -stat`` to see overall heap stats. You'll see `System.Byte[]` taking up huge amounts of memory.
5. Filter by LOH size: ``dumpheap -min 85000``
You'll see many byte arrays on LOH.
6. Pick an address of one of these byte arrays and run ``gcroot [address]``
You'll find that it's rooted by a `System.Collections.Generic.List<System.Byte[]>` held by the main thread/task.