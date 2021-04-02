## Solution
1. Since something got stuck or process hangs, we suspect that something is blocking a thread. Use the following command to find suspects (longest running threads are suspects)
```
0:000> !runaway
 User Mode Time
  Thread       Time
    0:3050     0 days 0:00:20.187
    9:2834     0 days 0:00:00.000
    8:483c     0 days 0:00:00.000
    7:396c     0 days 0:00:00.000
    6:26e4     0 days 0:00:00.000
    5:5360     0 days 0:00:00.000
    4:3e40     0 days 0:00:00.000
    3:4f74     0 days 0:00:00.000
    2:3f78     0 days 0:00:00.000
    1:5128     0 days 0:00:00.000
```

2. We see that thread with Id **0** runs much longer than the rest. We should check what it is doing
```
:009> ~0s
mscorlib_ni!System.Collections.Generic.Dictionary`2[System.Int32,System.__Canon].FindEntry(Int32)$##6003936+0x5d:
fff`d14d7f2d 4863d5          movsxd  rdx,ebp
:000> !clrstack
OS Thread Id: 0x3050 (0)
        Child SP               IP Call Site
f4fe5a0 00007fffd14d7f2d System.Collections.Generic.Dictionary`2[[System.Int32, mscorlib],[System.__Canon, mscorlib]].FindEntry(Int32)
f4fe5f0 00007fffd14d7e85 System.Collections.Generic.Dictionary`2[[System.Int32, mscorlib],[System.__Canon, mscorlib]].TryGetValue(Int32, System.__Canon ByRef)
f4fe630 00007fff75d20ebc Hang.Program+c__DisplayClass0_0.b__0(Int32) [C:\Users\User\source\repos\WindbgWorkshop\High CPU and Threads\Hang\Program.cs @ 24]
f4fe6f0 00007fffd1f82058 System.Threading.Tasks.Parallel+c__DisplayClass17_0`1[[System.__Canon, mscorlib]].b__1()
f4fe7d0 00007fffd1eafe20 System.Threading.Tasks.Task.InnerInvokeWithArg(System.Threading.Tasks.Task)
f4fe800 00007fffd1f84216 System.Threading.Tasks.Task+c__DisplayClass176_0.b__0(System.Object)
f4fe870 00007fffd14fdf12 System.Threading.ExecutionContext.RunInternal(System.Threading.ExecutionContext, System.Threading.ContextCallback, System.Object, Boolean)
f4fe940 00007fffd14fdd95 System.Threading.ExecutionContext.Run(System.Threading.ExecutionContext, System.Threading.ContextCallback, System.Object, Boolean)
f4fe970 00007fffd156b1e1 System.Threading.Tasks.Task.ExecuteWithThreadLocal(System.Threading.Tasks.Task ByRef)
f4fea20 00007fffd156a8c1 System.Threading.Tasks.Task.ExecuteEntry(Boolean)
f4fea60 00007fffd156ee72 System.Threading.Tasks.ThreadPoolTaskScheduler.TryExecuteTaskInline(System.Threading.Tasks.Task, Boolean)
f4feab0 00007fffd156ed46 System.Threading.Tasks.TaskScheduler.TryRunInline(System.Threading.Tasks.Task, Boolean)
f4feb50 00007fffd1eaefaa System.Threading.Tasks.Task.InternalRunSynchronously(System.Threading.Tasks.TaskScheduler, Boolean)
f4febe0 00007fffd1ebc3a9 System.Threading.Tasks.Parallel.ForWorker[[System.__Canon, mscorlib]](Int32, Int32, System.Threading.Tasks.ParallelOptions, System.Action`1, System.Action`2<Int32,System.Threading.Tasks.ParallelLoopState>, System.Func`4<Int32,System.Threading.Tasks.ParallelLoopState,System.__Canon,System.__Canon>, System.Func`1<System.__Canon>, System.Action`1<System.__Canon>)
f4fecc0 00007fffd1ebb8d1 System.Threading.Tasks.Parallel.For(Int32, Int32, System.Action`1)
f4fed40 00007fff75d209a7 Hang.Program.Main(System.String[]) [C:\Users\User\source\repos\WindbgWorkshop\High CPU and Threads\Hang\Program.cs @ 19]
```

We see that the thread is stuck while doing an internal operation of **Dictionary** that should be relatively short. It is likely we found an issue.

*Note: this can cause very interesting side effects. For example, see this: https://stackoverflow.com/q/14838032/320103*