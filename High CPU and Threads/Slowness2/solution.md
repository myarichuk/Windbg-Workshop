## Solution
1. Since we see huge task queue length, we suspect that something blocks the tasks from running properly
```
Press p to pause, r to resume, q to quit.
    Status: Running

[System.Runtime]
    % Time in GC since last GC (%)                                  99
    Allocation Rate (B / 1 sec)                             44,968,352
    CPU Usage (%)                                                    1
    Exception Count (Count / 1 sec)                                  0
    GC Fragmentation (%)                                             0.311
    GC Heap Size (MB)                                            8,154
    Gen 0 GC Count (Count / 1 sec)                                   8
    Gen 0 Size (B)                                                  24
    Gen 1 GC Count (Count / 1 sec)                                   4
    Gen 1 Size (B)                                           2,178,544
    Gen 2 GC Count (Count / 1 sec)                                   0
    Gen 2 Size (B)                                       7,252,840,552
    IL Bytes Jitted (B)                                         54,557
    LOH Size (B)                                           924,097,288
    Monitor Lock Contention Count (Count / 1 sec)                  309
    Number of Active Timers                                          1
    Number of Assemblies Loaded                                     20
    Number of Methods Jitted                                       613
    POH (Pinned Object Heap) Size (B)                           39,976
    ThreadPool Completed Work Item Count (Count / 1 sec)        13,712
    ThreadPool Queue Length                                 56,359,344
    ThreadPool Thread Count                                         21
    Working Set (MB)                                             8,214
```
2. Use ``!threads`` to see the list of process threads. Notice that most threads belong to threadpool - which makes sense, considering the threadpool count..
```
0:045> !threads
ThreadCount:      64
UnstartedThread:  0
BackgroundThread: 60
PendingThread:    0
DeadThread:       2
Hosted Runtime:   no
                                                                                                            Lock  
 DBG   ID     OSID ThreadOBJ           State GC Mode     GC Alloc Context                  Domain           Count Apt Exception
   0    1     31dc 0000017CA98AEA20    2a020 Preemptive  0000000000000000:0000000000000000 0000017ca98a68b0 -00001 MTA 
   3    2     3570 0000017CA98B49B0    2b220 Preemptive  0000000000000000:0000000000000000 0000017ca98a68b0 -00001 MTA (Finalizer) 
   4    3     266c 0000017CA98B7D70  102a220 Preemptive  0000000000000000:0000000000000000 0000017ca98a68b0 -00001 MTA (Threadpool Worker) 
   5    4     1f58 0000017CCBAFF510  1029220 Preemptive  0000000000000000:0000000000000000 0000017ca98a68b0 -00001 MTA (Threadpool Worker) 
   1    5      840 0000017CCBB4E120    20220 Preemptive  0000000000000000:0000000000000000 0000017ca98a68b0 -00001 Ukn 
   6    6     3eb8 0000017CCD5CC400  202b220 Preemptive  0000000000000000:0000000000000000 0000017ca98a68b0 -00001 MTA 
   7    7     1678 0000017CCD5DDFF0    21220 Preemptive  0000000000000000:0000000000000000 0000017ca98a68b0 -00001 Ukn 
   8    8     40f4 0000017CCD5E4440  1029220 Preemptive  0000000000000000:0000000000000000 0000017ca98a68b0 -00001 MTA (Threadpool Worker) 
   9    9     1d28 0000017CCD69A700    2b020 Cooperative 0000017E9E1D8FB0:0000017E9E1DAFD0 0000017ca98a68b0 -00001 MTA 
  11   10      c04 0000017CCD69AD10  1029220 Preemptive  0000000000000000:0000000000000000 0000017ca98a68b0 -00001 MTA (Threadpool Worker) 
  12   11     3eb4 0000017CCD69B320  1029220 Preemptive  0000000000000000:0000000000000000 0000017ca98a68b0 -00001 MTA (Threadpool Worker) 
  14   13     4294 0000017CCD697070  1029220 Preemptive  0000000000000000:0000000000000000 0000017ca98a68b0 -00001 MTA (Threadpool Worker) 
  15   15     40c0 0000017CCD6988B0  1029220 Preemptive  0000000000000000:0000000000000000 0000017ca98a68b0 -00001 MTA (Threadpool Worker) 
  16   16     4794 0000017CCD698EC0  1029220 Preemptive  0000000000000000:0000000000000000 0000017ca98a68b0 -00001 MTA (Threadpool Worker) 
  18   20     2e4c 0000017CCD6982A0  1029220 Preemptive  0000000000000000:0000000000000000 0000017ca98a68b0 -00001 MTA (Threadpool Worker) 
  21   24     2dc8 0000017CCD69C550  1029220 Preemptive  0000000000000000:0000000000000000 0000017ca98a68b0 -00001 MTA (Threadpool Worker) 
  23   26     1c3c 0000017CCD69D170  1029220 Preemptive  0000000000000000:0000000000000000 0000017ca98a68b0 -00001 MTA (Threadpool Worker) 
  25   27     4548 0000017CCD695E40  1029220 Preemptive  0000000000000000:0000000000000000 0000017ca98a68b0 -00001 MTA (Threadpool Worker) 
  27   30     2768 0000017CCD72CB10  1029220 Preemptive  0000000000000000:0000000000000000 0000017ca98a68b0 -00001 MTA (Threadpool Worker) 
  29   32     2e80 0000017CCD72E960  1029220 Preemptive  0000000000000000:0000000000000000 0000017ca98a68b0 -00001 MTA (Threadpool Worker) 
  30   33     2d28 0000017CCD72D730  3029220 Preemptive  0000000000000000:0000000000000000 0000017ca98a68b0 -00001 MTA (Threadpool Worker) 
  32   35     1230 0000017CCD72C500  3029220 Preemptive  0000000000000000:0000000000000000 0000017ca98a68b0 -00001 MTA (Threadpool Worker) 
  33   36     22b8 0000017CCD7313D0  3029220 Preemptive  0000000000000000:0000000000000000 0000017ca98a68b0 -00001 MTA (Threadpool Worker) 
  36   39     2e5c 0000017CCD72DD40  3029220 Preemptive  0000000000000000:0000000000000000 0000017ca98a68b0 -00001 MTA (Threadpool Worker) 
  39   14     10a8 0000017CCD731FF0  3029220 Preemptive  0000000000000000:0000000000000000 0000017ca98a68b0 -00001 MTA (Threadpool Worker) 
  40   40     49c0 0000017CCD72B2D0  3029220 Preemptive  0000000000000000:0000000000000000 0000017ca98a68b0 -00001 MTA (Threadpool Worker) 
  41   41     1ffc 0000017CCD7301A0  3029220 Preemptive  0000000000000000:0000000000000000 0000017ca98a68b0 -00001 MTA (Threadpool Worker) 
  43   18     3624 0000017CCD72ACC0  3029220 Preemptive  0000000000000000:0000000000000000 0000017ca98a68b0 -00001 MTA (Threadpool Worker) 
  44   17     1bc8 0000017CCD72FB90  3029220 Preemptive  0000000000000000:0000000000000000 0000017ca98a68b0 -00001 MTA (Threadpool Worker) 
  45   43     412c 0000017CCD72B8E0  1029220 Preemptive  0000000000000000:0000000000000000 0000017ca98a68b0 -00001 MTA (Threadpool Worker) 
  46   44     3bd4 0000017CCD72BEF0  3029220 Preemptive  0000000000000000:0000000000000000 0000017ca98a68b0 -00001 MTA (Threadpool Worker) 
  47   45     491c 0000017CCD699AE0  3029220 Preemptive  0000000000000000:0000000000000000 0000017ca98a68b0 -00001 MTA (Threadpool Worker) 
  48   46     14c4 0000017CCD69A0F0  3029220 Preemptive  0000000000000000:0000000000000000 0000017ca98a68b0 -00001 MTA (Threadpool Worker) 
XXXX   47        0 0000017CCD697680  1039820 Preemptive  0000000000000000:0000000000000000 0000017ca98a68b0 -00001 Ukn (Threadpool Worker) 
  50   48     1cb4 0000017CCD69BF40  3029220 Preemptive  0000000000000000:0000000000000000 0000017ca98a68b0 -00001 MTA (Threadpool Worker) 
  51   49     48b8 0000017CCD696A60  3029220 Preemptive  0000000000000000:0000000000000000 0000017ca98a68b0 -00001 MTA (Threadpool Worker) 
  13   50     15a8 0000017CCD70DA10  3029220 Preemptive  0000000000000000:0000000000000000 0000017ca98a68b0 -00001 MTA (Threadpool Worker) 
  17   51     3ba8 0000017CCD709D70  3029220 Preemptive  0000000000000000:0000000000000000 0000017ca98a68b0 -00001 MTA (Threadpool Worker) 
  19   52     4404 0000017CCD70E020  3029220 Preemptive  0000000000000000:0000000000000000 0000017ca98a68b0 -00001 MTA (Threadpool Worker) 
  20   53     43b8 0000017CCD70A380  3029220 Preemptive  0000000000000000:0000000000000000 0000017ca98a68b0 -00001 MTA (Threadpool Worker) 
  22   54     47ec 0000017CCD70C7E0  3029220 Preemptive  0000000000000000:0000000000000000 0000017ca98a68b0 -00001 MTA (Threadpool Worker) 
  24   55     493c 0000017CCD70B5B0  3029220 Preemptive  0000000000000000:0000000000000000 0000017ca98a68b0 -00001 MTA (Threadpool Worker) 
  31   56      8d0 0000017CCD6B2180  3029220 Preemptive  0000000000000000:0000000000000000 0000017ca98a68b0 -00001 MTA (Threadpool Worker) 
  26   57     4568 0000017CCD70D400  1029220 Preemptive  0000000000000000:0000000000000000 0000017ca98a68b0 -00001 MTA (Threadpool Worker) 
  28   58     28f4 0000017CCD711160  3029220 Preemptive  0000000000000000:0000000000000000 0000017ca98a68b0 -00001 MTA (Threadpool Worker) 
  42   59     18d8 0000017CCD710B50  3029220 Preemptive  0000000000000000:0000000000000000 0000017ca98a68b0 -00001 MTA (Threadpool Worker) 
  52   60     3cc8 0000017CCD712B60  3029220 Preemptive  0000000000000000:0000000000000000 0000017ca98a68b0 -00001 MTA (Threadpool Worker) 
XXXX   61        0 0000017CCD711D80  1039820 Preemptive  0000000000000000:0000000000000000 0000017ca98a68b0 -00001 Ukn (Threadpool Worker) 
  54   62     2cfc 0000017CCD714B70  1029220 Preemptive  0000000000000000:0000000000000000 0000017ca98a68b0 -00001 MTA (Threadpool Worker) 
  55   63     2ba4 0000017CCD712390  3029220 Preemptive  0000000000000000:0000000000000000 0000017ca98a68b0 -00001 MTA (Threadpool Worker) 
  56   64     40a0 0000017CCD713780  3029220 Preemptive  0000000000000000:0000000000000000 0000017ca98a68b0 -00001 MTA (Threadpool Worker) 
  57   65      a5c 0000017CCD711770  3029220 Preemptive  0000000000000000:0000000000000000 0000017ca98a68b0 -00001 MTA (Threadpool Worker) 
  58   66     37d0 0000017CCD713170  3029220 Preemptive  0000000000000000:0000000000000000 0000017ca98a68b0 -00001 MTA (Threadpool Worker) 
  59   67     14f0 0000017CA98C6CA0  3029220 Preemptive  0000000000000000:0000000000000000 0000017ca98a68b0 -00001 MTA (Threadpool Worker) 
  60   68     248c 0000017CCD764EF0  3029220 Preemptive  0000000000000000:0000000000000000 0000017ca98a68b0 -00001 MTA (Threadpool Worker) 
  61   69     4304 0000017CCD5C5A40  3029220 Preemptive  0000000000000000:0000000000000000 0000017ca98a68b0 -00001 MTA (Threadpool Worker) 
  62   70     2674 0000017CCD713D90  3029220 Preemptive  0000000000000000:0000000000000000 0000017ca98a68b0 -00001 MTA (Threadpool Worker) 
  63   71     13a8 0000017CCD70BBC0  1029220 Preemptive  0000000000000000:0000000000000000 0000017ca98a68b0 -00001 MTA (Threadpool Worker) 
  64   72     1dd8 0000017CCD715790  3029220 Preemptive  0000000000000000:0000000000000000 0000017ca98a68b0 -00001 MTA (Threadpool Worker) 
  65   73     2dcc 0000017CCD70CDF0  3029220 Preemptive  0000000000000000:0000000000000000 0000017ca98a68b0 -00001 MTA (Threadpool Worker) 
  67   74     3e84 0000017CCBAD4890  3029220 Preemptive  0000000000000000:0000000000000000 0000017ca98a68b0 -00001 MTA (Threadpool Worker) 
  66   75     3a24 0000017CCD70E630  3029220 Preemptive  0000000000000000:0000000000000000 0000017ca98a68b0 -00001 MTA (Threadpool Worker) 
  68   76      58c 0000017CCD7143A0  3029220 Preemptive  0000000000000000:0000000000000000 0000017ca98a68b0 -00001 MTA (Threadpool Worker) 
  69   77     26d4 0000017CCD6B2790  3029220 Preemptive  0000000000000000:0000000000000000 0000017ca98a68b0 -00001 MTA (Threadpool Worker) 
``` 

3. Use ``!eestack`` command to see what the process threads are doing. Notice that majority of the threads are blocked with something like this:
```
KERNELBASE!WaitForMultipleObjectsEx + 0xf0, calling ntdll!NtWaitForMultipleObjects
```

4. One of the reasons the threads may be blocked is some sort of mutex. Since we are dealing with a .Net process, let's check for basic .Net synchronization primitive:
```
0:028> !syncblk
Index         SyncBlock MonitorHeld Recursion Owning Thread Info          SyncBlock Owner
   12 00000225F3320488           33         1 00000225F4E69230 4634  11   00000225d2ced908 System.Object
-----------------------------
Total           13
CCW             0
RCW             0
ComClassFactory 0
Free            2
```

The **MonitorHeld** value tells us how many threads are waiting to acquire the lock - this means we found our bottleneck. 

5. Notice the id of the owning thread. The following commands will show what the thread is doing.
```
:028> ~~[4634]s
ntdll!NtWaitForSingleObject+0x14:
fff`f6f0cc94 c3              ret
:011> !clrstack
OS Thread Id: 0x4634 (11)
        Child SP               IP Call Site
FD925BF008 00007ffff6f0cc94 [HelperMethodFrame: 000000fd925bf008] 
FD925BF120 00007fff19978e3b System.Collections.Generic.Dictionary`2[[System.__Canon, System.Private.CoreLib],[System.__Canon, System.Private.CoreLib]].Resize(Int32, Boolean)
FD925BF1B0 00007fff1978ce36 System.Collections.Generic.Dictionary`2[[System.__Canon, System.Private.CoreLib],[System.__Canon, System.Private.CoreLib]].TryInsert(System.__Canon, System.__Canon, System.Collections.Generic.InsertionBehavior)
FD925BF260 00007fff199803f8 Slowness2.MegaUserCache.TryAdd(System.String, Slowness2.User) [C:\Users\User\source\repos\WindbgWorkshop\High CPU and Threads\Slowness2\Program.cs @ 39]
FD925BF2F0 00007fff199763e7 Slowness2.UserRepository.Get(System.String) [C:\Users\User\source\repos\WindbgWorkshop\High CPU and Threads\Slowness2\Program.cs @ 91]
FD925BF3A0 00007fff19975e43 Slowness2.Program+c__DisplayClass0_1.g__GetRandomUser|1() [C:\Users\User\source\repos\WindbgWorkshop\High CPU and Threads\Slowness2\Program.cs @ 115]
FD925BF410 00007fff19975d3c Slowness2.Program+c__DisplayClass0_1.b__2() [C:\Users\User\source\repos\WindbgWorkshop\High CPU and Threads\Slowness2\Program.cs @ 120]
FD925BF450 00007fff19983951 System.Threading.ExecutionContext.RunFromThreadPoolDispatchLoop(System.Threading.Thread, System.Threading.ExecutionContext, System.Threading.ContextCallback, System.Object) [/_/src/libraries/System.Private.CoreLib/src/System/Threading/ExecutionContext.cs @ 300]
FD925BF4A0 00007fff199836f7 System.Threading.Tasks.Task.ExecuteWithThreadLocal(System.Threading.Tasks.Task ByRef, System.Threading.Thread) [/_/src/libraries/System.Private.CoreLib/src/System/Threading/Tasks/Task.cs @ 2352]
FD925BF550 00007fff19983ba4 System.Threading.ThreadPoolWorkQueue.Dispatch() [/_/src/libraries/System.Private.CoreLib/src/System/Threading/ThreadPool.cs @ 677]
```

6. We now know what leads to the bottleneck. Now, let's see which threads are waiting to acquire the lock.  
In ordert to do that, we take the address of **syncblk** owner and run **gcroot**
```
0:011> !gcroot 00000225d2ced908
Thread 28a4:
    000000FD9137E570 00007FFFE9176010 System.ConsolePal.ReadKey(Boolean) [/_/src/libraries/System.Console/src/System/ConsolePal.Windows.cs @ 319]
        rbx:  (interior)
            ->  00000225EACE1018 System.Object[]
            ->  00000225D2CFA808 System.Threading.ThreadPoolWorkQueue
            ->  00000225D2CFA8A8 System.Collections.Concurrent.ConcurrentQueue`1[[System.Object, System.Private.CoreLib]]
            ->  00000227E3E8E0B8 System.Collections.Concurrent.ConcurrentQueueSegment`1[[System.Object, System.Private.CoreLib]]
            ->  00000227C09F1170 System.Collections.Concurrent.ConcurrentQueueSegment`1+Slot[[System.Object, System.Private.CoreLib]][]
            ->  00000227E3E8E078 System.Threading.Tasks.Task
            ->  00000227E3E8E038 System.Action
            ->  00000225D30F1DB8 Slowness2.Program+<>c__DisplayClass0_1
            ->  00000225D2CEA420 Slowness2.Program+<>c__DisplayClass0_0
            ->  00000225D2CED868 Slowness2.UserRepository
            ->  00000225D2CED8E8 Slowness2.MegaUserCache
            ->  00000225D2CED908 System.Object

Thread 393c:
    000000FD91DFEE78 [HelperMethodFrame_1OBJ: 000000fd91dfee78] 
        000000fd91dfee28
            ->  00000225D2CED908 System.Object

    000000FD91DFEFD0 00007FFF199818BA Slowness2.MegaUserCache+<>c__DisplayClass3_0.<TryAdd>b__0(System.Threading.Tasks.Task) [C:\Users\User\source\repos\WindbgWorkshop\High CPU and Threads\Slowness2\Program.cs @ 45]
        rbp-8: 000000fd91dff028
            ->  00000225D2CED908 System.Object

Thread 2350:
    000000FD922BF108 [HelperMethodFrame_1OBJ: 000000fd922bf108] 
        000000fd922bf0b8
            ->  00000225D2CED908 System.Object

    000000FD922BF260 00007FFF199764F5 Slowness2.MegaUserCache.TryGet(System.String, Slowness2.User ByRef) [C:\Users\User\source\repos\WindbgWorkshop\High CPU and Threads\Slowness2\Program.cs @ 27]
        rbp-8: 000000fd922bf298
            ->  00000225D2CED908 System.Object
*** more threads here... ***
```