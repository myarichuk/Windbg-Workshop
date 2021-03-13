## Solution
High CPU is often caused by large number of concurrent cpu-bound algorithms, be it user-mode functionality or lower-level things like frequent GC cycles due to a memory leak (Garbage Collection is a cpu intensive task)
As with any issue in post-mortem debugging, we will start with looking at performance counters. (in real scenario this can be dashboards or any other monitoring solutions!) <br/>
1. Using ``dotnet-counters`` tool we will see an output similar to the following. Notice the ever growing **ThreadPool Queue Length** and **ThreadPool Thread Count** - this is a symptom of threadpool starvation
```console
Press p to pause, r to resume, q to quit.
    Status: Running

[System.Runtime]
    % Time in GC since last GC (%)                                 0
    Allocation Rate (B / 1 sec)                              326,936
    CPU Usage (%)                                                 94
    Exception Count (Count / 1 sec)                                0
    GC Fragmentation (%)                                          10.425
    GC Heap Size (MB)                                             65
    Gen 0 GC Count (Count / 1 sec)                                 0
    Gen 0 Size (B)                                                24
    Gen 1 GC Count (Count / 1 sec)                                 0
    Gen 1 Size (B)                                        12,570,056
    Gen 2 GC Count (Count / 1 sec)                                 0
    Gen 2 Size (B)                                        56,623,680
    IL Bytes Jitted (B)                                        6,934
    LOH Size (B)                                             931,208
    Monitor Lock Contention Count (Count / 1 sec)                  0
    Number of Active Timers                                        0
    Number of Assemblies Loaded                                   11
    Number of Methods Jitted                                      47
    POH (Pinned Object Heap) Size (B)                         35,856
    ThreadPool Completed Work Item Count (Count / 1 sec)           0
    ThreadPool Queue Length                                       25
    ThreadPool Thread Count                                        9
    Working Set (MB)                                              94
```

2. Open a dump of the process and run the following
```
:000> !dumpasync -tasks -completed
Statistics:
              MT    Count    TotalSize Class Name
ff7f3e74e88        1           72 System.Threading.Tasks.Task`1[[System.Threading.Tasks.VoidTaskResult, System.Private.CoreLib]]
ff7f3eac170      201        14472 System.Threading.Tasks.Task`1[[System.Collections.Generic.List`1[[System.String, System.Private.CoreLib]], System.Private.CoreLib]]
Total 202 objects
         Address               MT     Size   Status      State Description
f0000bee8 00007ff7f3e74e88       72  Success [01004000] System.Threading.Tasks.Task`1[[System.Threading.Tasks.VoidTaskResult, System.Private.CoreLib]] 
f00884dd0 00007ff7f3eac170       72  Success [01032008] System.Threading.Tasks.Task`1[[System.Collections.Generic.List`1[[System.String, System.Private.CoreLib]], System.Private.CoreLib]] 
f00ec6088 00007ff7f3eac170       72  Pending [00032008] System.Threading.Tasks.Task`1[[System.Collections.Generic.List`1[[System.String, System.Private.CoreLib]], System.Private.CoreLib]] (HighCPU.Program+<>c__DisplayClass0_0.<Main>b__2()) 
f01516088 00007ff7f3eac170       72  Pending [00032008] System.Threading.Tasks.Task`1[[System.Collections.Generic.List`1[[System.String, System.Private.CoreLib]], System.Private.CoreLib]] (HighCPU.Program+<>c__DisplayClass0_0.<Main>b__2()) 
f01b5a058 00007ff7f3eac170       72  Pending [00032008] System.Threading.Tasks.Task`1[[System.Collections.Generic.List`1[[System.String, System.Private.CoreLib]], System.Private.CoreLib]] (HighCPU.Program+<>c__DisplayClass0_0.<Main>b__2()) 
f021aa028 00007ff7f3eac170       72  Pending [00032008] System.Threading.Tasks.Task`1[[System.Collections.Generic.List`1[[System.String, System.Private.CoreLib]], System.Private.CoreLib]] (HighCPU.Program+<>c__DisplayClass0_0.<Main>b__2()) 
f02802028 00007ff7f3eac170       72  Pending [00032008] System.Threading.Tasks.Task`1[[System.Collections.Generic.List`1[[System.String, System.Private.CoreLib]], System.Private.CoreLib]] (HighCPU.Program+<>c__DisplayClass0_0.<Main>b__2()) 
f02e4a028 00007ff7f3eac170       72  Pending [00032008] System.Threading.Tasks.Task`1[[System.Collections.Generic.List`1[[System.String, System.Private.CoreLib]], System.Private.CoreLib]] (HighCPU.Program+<>c__DisplayClass0_0.<Main>b__2()) 
f0349c0b8 00007ff7f3eac170       72  Pending [00032008] System.Threading.Tasks.Task`1[[System.Collections.Generic.List`1[[System.String, System.Private.CoreLib]], System.Private.CoreLib]] (HighCPU.Program+<>c__DisplayClass0_0.<Main>b__2()) 

** Omitted LOTS of "Pending" tasks **
```
3. Use ``!runaway`` to see the longest running threads. The first threads in the list will be the longest running ones, and they are a prime suspect to find *what* causes threads to get block and starve the threadpool.
```
0:014> !runaway
 User Mode Time
  Thread       Time
   13:4e1c     0 days 0:01:11.203
   11:61e8     0 days 0:01:11.031
   18:1474     0 days 0:01:08.390
   19:32a0     0 days 0:01:07.171
   16:36bc     0 days 0:01:06.109
   14:3bc      0 days 0:01:05.953
   17:7bc      0 days 0:01:04.796
   15:3168     0 days 0:01:01.734
   22:3910     0 days 0:00:56.218
   23:300      0 days 0:00:45.328
   24:1cd0     0 days 0:00:38.531
   25:3674     0 days 0:00:36.843
   26:268c     0 days 0:00:26.265
``` 
Notice a pattern here: the longest two running threads have VERY similar running time and the next 6 threads have also VERY similar running time. This is another symptom of thread starvation. The threadpool started from two threads and when they got blocked, the CLR added 6 more threads to threadpool and then they got blocked as well. <br/>
4. Switching to thread #13 and #11 and taking a look at their stack traces will confirm the suspicion - those are threadpool threads that are doing intensive computation and block the threads, forcing threadpool to grow, along the way also causing the high CPU symptom that we observed (notice the ``System.Threading.Tasks.Task`` related lines in the stack trace)
```
:011> ~13s
ff7`f3d9104c 483b06          cmp     rax,qword ptr [rsi] ds:0000021f`014eb958=00007ff7f3eb3538
:013> !clrstack
OS Thread Id: 0x4e1c (13)
        Child SP               IP Call Site
D91CDFF0A0 00007ff7f3d9104c System.Linq.Enumerable+ConcatNIterator`1[[System.__Canon, System.Private.CoreLib]].GetEnumerable(Int32) [/_/src/libraries/System.Linq/src/System/Linq/Concat.cs @ 182]
D91CDFF0C0 00007ff870a99511 System.Linq.Enumerable+ConcatIterator`1[[System.__Canon, System.Private.CoreLib]].ToList() [/_/src/libraries/System.Linq/src/System/Linq/Concat.SpeedOpt.cs @ 208]
D91CDFF110 00007ff870a96ab1 System.Linq.Enumerable.ToList[[System.__Canon, System.Private.CoreLib]](System.Collections.Generic.IEnumerable`1<System.__Canon>) [/_/src/libraries/System.Linq/src/System/Linq/ToCollection.cs @ 29]
D91CDFF160 00007ff7f3d8fa43 HighCPU.Program+c__DisplayClass0_0.b__2()
D91CDFF1C0 00007ff831602bff System.Threading.Tasks.Task`1[[System.__Canon, System.Private.CoreLib]].InnerInvoke() [/_/src/libraries/System.Private.CoreLib/src/System/Threading/Tasks/Future.cs @ 497]
D91CDFF200 00007ff83160fc01 System.Threading.Tasks.Task+c.<.cctor>b__277_0(System.Object) [/_/src/libraries/System.Private.CoreLib/src/System/Threading/Tasks/Task.cs @ 2358]
D91CDFF230 00007ff8315f5462 System.Threading.ExecutionContext.RunFromThreadPoolDispatchLoop(System.Threading.Thread, System.Threading.ExecutionContext, System.Threading.ContextCallback, System.Object) [/_/src/libraries/System.Private.CoreLib/src/System/Threading/ExecutionContext.cs @ 274]
D91CDFF280 00007ff831609edb System.Threading.Tasks.Task.ExecuteWithThreadLocal(System.Threading.Tasks.Task ByRef, System.Threading.Thread) [/_/src/libraries/System.Private.CoreLib/src/System/Threading/Tasks/Task.cs @ 2320]
D91CDFF330 00007ff831609cb3 System.Threading.Tasks.Task.ExecuteEntryUnsafe(System.Threading.Thread) [/_/src/libraries/System.Private.CoreLib/src/System/Threading/Tasks/Task.cs @ 2252]
D91CDFF370 00007ff831609c5a System.Threading.Tasks.Task.ExecuteFromThreadPool(System.Threading.Thread) [/_/src/libraries/System.Private.CoreLib/src/System/Threading/Tasks/Task.cs @ 2243]
D91CDFF3A0 00007ff8315fe37a System.Threading.ThreadPoolWorkQueue.Dispatch() [/_/src/libraries/System.Private.CoreLib/src/System/Threading/ThreadPool.cs @ 641]
D91CDFF450 00007ff8315ed83a System.Threading._ThreadPoolWaitCallback.PerformWaitCallback() [/_/src/coreclr/src/System.Private.CoreLib/src/System/Threading/ThreadPool.CoreCLR.cs @ 29]
D91CDFF860 00007ff8538f9c13 [DebuggerU2MCatchHandlerFrame: 000000d91cdff860] 
```

*In this way we can determine the root cause of the high CPU*