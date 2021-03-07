As a first step, since we are talking about a .Net app, we load SOS

```windbg
.loadby sos coreclr
```

Then, since we are talking about a memory leak, we execute dumpheap -stat. We should see something like the following.
Notice that there is nothing looks out of the ordinary, byte arrays are a bit suspicious, but still it could be some serialization action.
```windbg
!dumpheap -stat
00007ffb55093248     4018      1092896 System.Collections.Concurrent.ConcurrentDictionary`2+Node[[System.String, System.Private.CoreLib],[Microsoft.Extensions.FileProviders.Physical.PhysicalFilesWatcher+ChangeTokenInfo, Microsoft.Extensions.FileProviders.Physical]][]
00007ffb54ec0c68    48178      1156272 System.Object
00007ffb550baa10    95959      2303016 System.WeakReference`1[[System.IO.FileSystemWatcher, System.IO.FileSystem.Watcher]]
00007ffb550bb530    95959      3070688 System.Threading.ThreadPoolBoundHandle
00007ffb550ba5c0    95959      3070688 System.Threading.PreAllocatedOverlapped
00007ffb550bafd8    95961      4606128 Microsoft.Win32.SafeHandles.SafeFileHandle
00007ffb550b9b58    95959      6141376 System.IO.FileSystemWatcher+AsyncReadState
00007ffb550bbc30    95959      6909048 System.Threading.OverlappedData
00007ffb550bba30    95959      6909048 System.Threading.ThreadPoolBoundHandleOverlapped
0000015d85c015d0   191280    361304416      Free
00007ffb54fcb718    95968    788403295 System.Byte[]
```
Since byte arrays are suspicious, we can check them, but we won't see anything useful here. Note that it will take a while to enumerate the byte arrays.
```windbg
!gcroot -all 0000015dc64b72c8
HandleTable:
    0000015DA7AA4168 (async pinned handle)
    -> 0000015DC64B93C0 System.Threading.OverlappedData
    -> 0000015DC64B72C8 System.Byte[]

Found 1 roots.
```

Next thing would be to check unamanaged heaps, since we don't see anything interesting in the managed heap. If there is no managed leak, perhaps there is an unmanaged leak?
```
!heap -s


************************************************************************************************************************
                                              NT HEAP STATS BELOW
************************************************************************************************************************
NtGlobalFlag enables following debugging aids for new heaps:
    stack back traces
LFH Key                   : 0x64ecab356b9e5983
Termination on corruption : ENABLED
          Heap     Flags   Reserv  Commit  Virt   Free  List   UCR  Virt  Lock  Fast 
                            (k)     (k)    (k)     (k) length      blocks cont. heap 
-------------------------------------------------------------------------------------
0000015d85bb0000 08000002    8376   6816   8176    254    97     5    1      3   LFH
0000015d84290000 08008000      64      4     64      2     1     1    0      0      
0000015d87700000 08001002     260     40     60      2     1     1    0      0   LFH
0000015d878b0000 08001002      60      8     60      2     1     1    0      0      
0000015d879e0000 08041002      60      8     60      5     1     1    0      0      
-------------------------------------------------------------------------------------
```

If we take multiple dumps, we will see that the size of one of the heaps is increasing. 
The next step would be to group by size and see what kind of allocations are there. 

```windbg
!heap -stat -h 0000015d85bb0000
 heap @ 0000015d85bb0000
group-by: TOTSIZE max-display: 20
    size     #blocks     total     ( %) (percent of total busy bytes)
    28 14b40 - 33c200  (38.60)
    24b238 1 - 24b238  (27.37)
    100000 1 - 100000  (11.93)
    90000 1 - 90000  (6.71)
    12f77 1 - 12f77  (0.88)
    10000 1 - 10000  (0.75)
    30 411 - c330  (0.57)
    c000 1 - c000  (0.56)
    c0 f6 - b880  (0.54)
    90 131 - ab90  (0.50)
    38 2de - a090  (0.47)
    70 16a - 9e60  (0.46)
    20 494 - 9280  (0.43)
    5f8 18 - 8f40  (0.42)
    50 1ca - 8f20  (0.42)
    40 217 - 85c0  (0.39)
    80 10a - 8500  (0.39)
    790 11 - 8090  (0.37)
    8058 1 - 8058  (0.37)
    7807 1 - 7807  (0.35)
```
Now, as we have verified that this is an unmanaged memory leak and saw that allocated blocks of certain size hold up the majority of the heap, it is time to see who allocated them.

Before we continue, we will enable allocation stack tracing for the application and restart it. (the tool **gflags** is installed together with Debugging Tools of Windows 10 SDK)
"C:\Program Files (x86)\Windows Kits\10\Debuggers\x64\"gflags /i C:\Users\User\source\repos\Memory-Leak-Investigations-Workshop\Basics\MysteryApp6\bin\Release\net5.0\MysteryApp6.exe +ust

The following will *filter* heap output by size 0x28 and output pointers for the allocated blocks.
```windbg
0:011> !heap -flt s 28
    _HEAP @ 15d85bb0000
              HEAP_ENTRY Size Prev Flags            UserPtr UserSize - state
        0000015d85bbb830 0005 0000  [00]   0000015d85bbb860    00028 - (busy)
          unknown!printable
        0000015d85bc79e0 0005 0005  [00]   0000015d85bc7a10    00028 - (busy)
          unknown!printable
        0000015d85bc7b70 0005 0005  [00]   0000015d85bc7ba0    00028 - (busy)
        0000015d85c91730 0005 0005  [00]   0000015d85c91760    00028 - (busy)
          unknown!printable       
```

Next, we will take a look at allocation stack traces. We will see that most of those pointers have stack trace similar to the following. Notice that ``FileSystemWatcher`` allocates native memory
```windbg
0:011> !heap -p -a 0000015da8026b50
    address 0000015da8026b50 found in
    _HEAP @ 15d85bb0000
              HEAP_ENTRY Size Prev Flags            UserPtr UserSize - state
        0000015da8026b50 0005 0000  [00]   0000015da8026b80    00028 - (busy)
        7ffc3be3d887 ntdll!RtlpAllocateHeapInternal+0x0000000000096b87
        7ffbb4a2ff56 coreclr!AllocateNativeOverlapped+0x00000000000000f6
        7ffbb42facc6 System_Private_CoreLib+0x000000000050acc6
        7ffbb42fa160 System_Private_CoreLib+0x000000000050a160
        7ffbede7be1f System_IO_FileSystem_Watcher!System.IO.FileSystemWatcher.StartRaisingEvents()$##6000056+0x00000000000001ef
        7ffbede7ba80 System_IO_FileSystem_Watcher!System.IO.FileSystemWatcher.StartRaisingEventsIfNotDisposed()$##600004E+0x0000000000000020
        7ffbede7a5ce System_IO_FileSystem_Watcher!System.IO.FileSystemWatcher.set_EnableRaisingEvents(Boolean)$##600002A+0x000000000000005e
        7ffb54f18807 +0x00007ffb54f18807
```

If we check FileSystemWatcher we see it has only one instance! This is definitely an unmanaged memory leak. 
```windbg
!dumpheap -stat -type FileSystemWatcher
Statistics:
              MT    Count    TotalSize Class Name
00007ffb550bb650        1           24 System.IO.FileSystemWatcher+<>c
00007ffb5507cd10     2009        48216 System.IO.FileSystemWatcher+NormalizedFilterCollection+ImmutableStringList
00007ffb5507c098     2009        48216 System.IO.FileSystemWatcher+NormalizedFilterCollection
00007ffb5507b2a0     2009       241080 System.IO.FileSystemWatcher
00007ffb550baa10    95959      2303016 System.WeakReference`1[[System.IO.FileSystemWatcher, System.IO.FileSystem.Watcher]]
00007ffb550b9b58    95959      6141376 System.IO.FileSystemWatcher+AsyncReadState
Total 197946 objects
```

Now we know what to look for in the code. If we take a look at C# code, we will see the following:
```csharp
while (!mre.IsSet)
{
    try
    {
        var fp = new PhysicalFileProvider(Path.GetTempPath());
        fp.Watch("*.*"); // <= this looks like something that would use FileSystemWatcher!
    }
    catch (Exception) { }
}
```

If we take a look at an implementation of ``PhysicalFileProvider`` [here](https://github.com/dotnet/runtime/blob/master/src/libraries/Microsoft.Extensions.FileProviders.Physical/src/PhysicalFileProvider.cs#L30), we will see it uses ``PhysicalFileWatcher`` which in turn uses ``FileSystemWatcher`` as can be seen [here](https://github.com/dotnet/runtime/blob/191ea066b157103d6332b2cf3e986b055fda3bcd/src/libraries/Microsoft.Extensions.FileProviders.Physical/src/PhysicalFilesWatcher.cs#L36)

Since ``PhysicalFileProvider`` is disposable, adding ``using`` will fix the leak.
```csharp
while (!mre.IsSet)
{
    try
    {
        using var fp = new PhysicalFileProvider(Path.GetTempPath());
        fp.Watch("*.*"); // <= this looks like something that would use FileSystemWatcher!
    }
    catch (Exception) { }
}
```