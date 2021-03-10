## Solution 1 (with SOS only)
1. Run ``!syncblk`` command and see what locks are there. Since we know that something is stuck and there is two locks, we should suspect a deadlock.
```
0:008> !syncblk
Index         SyncBlock MonitorHeld Recursion Owning Thread Info          SyncBlock Owner
   11 000002306E5B3AB8            3         1 000002307051F7F0 2ae4   9   000002300000edb0 System.Object
   12 000002306E5B3B08            3         1 0000023070517D50 43b0   8   000002300000ed78 System.Object
```
2. Write down the two lock object addresses and owning thread numbers, in this case this is
```
thread 9 locks on object 000002300000edb0
thread 8 locks on object 000002300000ed78
```
3. Now, in order to prove a deadlock we need to find which threads are waiting for those locks
4. Run ~8s to switch thread context
5. Run ``kv`` to show unamanaged call stack with parameters
```
:008> kv
 # Child-SP          RetAddr               : Args to Child                                                           : Call Site
0000006e`008fe6a8 00007ff8`a8770d40     : 00007ff7`ca08b228 0000006e`008fe7b0 00007ff7`c9fbe430 00000018`00000000 : ntdll!NtWaitForMultipleObjects+0x14
0000006e`008fe6b0 00007ff8`29a87f80     : 00000000`00000000 00000000`00000000 00000000`00000001 00000000`00000000 : KERNELBASE!WaitForMultipleObjectsEx+0xf0
(Inline Function) --------`--------     : --------`-------- --------`-------- --------`-------- --------`-------- : coreclr!Thread::DoAppropriateAptStateWait+0x41 (Inline Function @ 00007ff8`29a87f80) [F:\workspace\_work\1\s\src\coreclr\src\vm\threads.cpp @ 3305] 
0000006e`008fe9a0 00007ff8`29a87cdd     : 00000000`00000001 00000000`00000001 00000230`6e5b3ad0 00007ff7`00000000 : coreclr!Thread::DoAppropriateWaitWorker+0x1f8 [F:\workspace\_work\1\s\src\coreclr\src\vm\threads.cpp @ 3437] 
0000006e`008fea90 00007ff8`29a856e1     : 0000006e`008febd9 00007ff8`00000001 00000230`70517d50 00000000`00000130 : coreclr!Thread::DoAppropriateWait+0x89 [F:\workspace\_work\1\s\src\coreclr\src\vm\threads.cpp @ 3154] 
(Inline Function) --------`--------     : --------`-------- --------`-------- --------`-------- --------`-------- : coreclr!CLREventBase::WaitEx+0x53 (Inline Function @ 00007ff8`29a856e1) [F:\workspace\_work\1\s\src\coreclr\src\vm\synch.cpp @ 463] 
0000006e`008feb10 00007ff8`29a85370     : 00000230`70517d50 00007ff8`00000000 0000006e`008fec88 00000230`70517d50 : coreclr!CLREventBase::Wait+0x59 [F:\workspace\_work\1\s\src\coreclr\src\vm\synch.cpp @ 417] 
0000006e`008feb60 00007ff8`29a8520a     : 00000230`0000ed00 00007ff8`29a865d7 00000230`70517d50 00000000`00000000 : coreclr!AwareLock::EnterEpilogHelper+0x14c [F:\workspace\_work\1\s\src\coreclr\src\vm\syncblk.cpp @ 2630] 
0000006e`008fec40 00007ff8`29a851b6     : 00000230`70517d50 00000230`0000ee70 00000000`00000000 00000230`0000ee00 : coreclr!AwareLock::EnterEpilog+0x42 [F:\workspace\_work\1\s\src\coreclr\src\vm\syncblk.cpp @ 2501] 
0000006e`008fec90 00007ff8`29a84e07     : 00007ff8`29a84970 00007ff8`29e86070 00000230`6e5b3ab8 00007ff8`29a84970 : coreclr!AwareLock::Enter+0xa2 [F:\workspace\_work\1\s\src\coreclr\src\vm\syncblk.cpp @ 2403] 
a (Inline Function) --------`--------     : --------`-------- --------`-------- --------`-------- --------`-------- : coreclr!SyncBlock::EnterMonitor+0x8 (Inline Function @ 00007ff8`29a84e07) [F:\workspace\_work\1\s\src\coreclr\src\vm\syncblk.h @ 1116] 
b (Inline Function) --------`--------     : --------`-------- --------`-------- --------`-------- --------`-------- : coreclr!ObjHeader::EnterObjMonitor+0xd (Inline Function @ 00007ff8`29a84e07) [F:\workspace\_work\1\s\src\coreclr\src\vm\syncblk.cpp @ 1572] 
c (Inline Function) --------`--------     : --------`-------- --------`-------- --------`-------- --------`-------- : coreclr!Object::EnterObjMonitor+0x16 (Inline Function @ 00007ff8`29a84e07) [F:\workspace\_work\1\s\src\coreclr\src\vm\object.h @ 277] 
d 0000006e`008fecc0 00007ff8`29a84a47     : **00000230`0000edb0** 0000006e`008fef40 00007ff8`29a84970 00000000`00000008 : coreclr!JIT_MonEnter_Helper+0x107 [F:\workspace\_work\1\s\src\coreclr\src\vm\jithelpers.cpp @ 3735] 
e 0000006e`008fee50 00007ff8`295eaf9f     : 00000230`0000f390 00000230`0000ee70 0000006e`008ff3f0 00000230`000112d8 : coreclr!JIT_MonReliableEnter_Portable+0xd7 [F:\workspace\_work\1\s\src\coreclr\src\vm\jithelpers.cpp @ 3783] 
f 0000006e`008fee90 00007ff7`c9fbddc6     : 00000230`0000ed90 00000000`ffffffff 00000000`00000000 00000230`6e5b3a68 : System_Private_CoreLib!System.Threading.Monitor.Enter(System.Object, Boolean ByRef)$##600256C+0xf
0000006e`008feec0 00007ff7`c9fbdc7b     : 00000230`0000edc8 00000230`0000ee18 00007ff7`ca08b3b8 00000230`0000ee70 : 0x00007ff7`c9fbddc6
0000006e`008fef50 00007ff8`2960a09e     : 00000230`0000b728 00000230`0000ee18 00007ff7`ca08b3b8 00000230`0000ee70 : 0x00007ff7`c9fbdc7b
0000006e`008fef80 00007ff8`2960fc01     : 00000230`0000ee70 00000230`0000ee70 00000230`0000c120 00000230`0000ee70 : System_Private_CoreLib!System.Threading.Tasks.Task.InnerInvoke()$##6002A77+0x1e
0000006e`008fefb0 00007ff8`295f5462     : 00000230`0000c160 00000230`0000ee70 00000230`0000c120 00000230`0000ee70 : System_Private_CoreLib!System.Threading.Tasks.Task+<>c.<.cctor>b__277_0(System.Object)$##6002B11+0x11
0000006e`008fefe0 00007ff8`29609edb     : 00000230`00011310 00000230`0000eef8 00000230`0000c120 00000230`0000ee70 : System_Private_CoreLib!System.Threading.ExecutionContext.RunFromThreadPoolDispatchLoop(System.Threading.Thread, System.Threading.ExecutionContext, System.Threading.ContextCallback, System.Object)$##600278A+0x42
0000006e`008ff030 00007ff8`29609cb3     : 00000230`0000ee70 00000230`000112b0 00000230`00011310 00000000`00000000 : System_Private_CoreLib!System.Threading.Tasks.Task.ExecuteWithThreadLocal(System.Threading.Tasks.Task ByRef, System.Threading.Thread)$##6002A76+0x1cb
0000006e`008ff0e0 00007ff8`29609c5a     : 00000230`0000ee70 00000230`00011310 00007ff7`ca085ab8 0000006e`008ff140 : System_Private_CoreLib!System.Threading.Tasks.Task.ExecuteEntryUnsafe(System.Threading.Thread)$##6002A74+0x53
0000006e`008ff120 00007ff8`295fe37a     : 00000230`0000ee70 00000230`00011310 00007ff7`ca085ab8 0000006e`008ff140 : System_Private_CoreLib!System.Threading.Tasks.Task.ExecuteFromThreadPool(System.Threading.Thread)$##6002A73+0xa
0000006e`008ff150 00007ff8`295ed83a     : 00007ff8`29a45fcb 00007ff8`29debf38 00000000`00000004 00000000`000000bc : System_Private_CoreLib!System.Threading.ThreadPoolWorkQueue.Dispatch()$##60028B0+0x1ba
0000006e`008ff200 00007ff8`29b19c13     : 00007ff8`29a45fcb 00007ff8`29debf38 00000000`00000004 00000000`000000bc : System_Private_CoreLib!System.Threading._ThreadPoolWaitCallback.PerformWaitCallback()$##600264A+0xa
a 0000006e`008ff230 00007ff8`29a461b0     : 00007ff7`ca0c4380 0000006e`008ff301 00000000`00000007 00000000`00000000 : coreclr!CallDescrWorkerInternal+0x83 [F:\workspace\_work\1\s\src\coreclr\src\vm\amd64\CallDescrWorkerAMD64.asm @ 100] 
b 0000006e`008ff270 00007ff8`29a45e6d     : 0000006e`008ff722 0000006e`008ff722 0000006e`008ff459 00007ff8`aa00c0e8 : coreclr!MethodDescCallSite::CallTargetWorker+0x268 [F:\workspace\_work\1\s\src\coreclr\src\vm\callhelpers.cpp @ 552] 
c (Inline Function) --------`--------     : --------`-------- --------`-------- --------`-------- --------`-------- : coreclr!MethodDescCallSite::Call_RetBool+0x15 (Inline Function @ 00007ff8`29a45e6d) [F:\workspace\_work\1\s\src\coreclr\src\vm\callhelpers.h @ 458] 
d 0000006e`008ff3b0 00007ff8`29a606a3     : ffffffff`ffffffff 00000230`704c3770 00000230`70517d50 00000000`00000000 : coreclr!QueueUserWorkItemManagedCallback+0xed [F:\workspace\_work\1\s\src\coreclr\src\vm\comthreadpool.cpp @ 464] 
e (Inline Function) --------`--------     : --------`-------- --------`-------- --------`-------- --------`-------- : coreclr!ManagedThreadBase_DispatchInner+0xd (Inline Function @ 00007ff8`29a606a3) [F:\workspace\_work\1\s\src\coreclr\src\vm\threads.cpp @ 7325] 
f 0000006e`008ff4c0 00007ff8`29a60587     : 00000000`00000000 00006773`e83675af 00000000`00000007 00000000`08000000 : coreclr!ManagedThreadBase_DispatchMiddle+0x8f [F:\workspace\_work\1\s\src\coreclr\src\vm\threads.cpp @ 7369] 
0000006e`008ff5d0 00007ff8`29a6044e     : 00000230`00000001 ffffffff`ffffffff 00000230`70517d50 00000230`704c3770 : coreclr!ManagedThreadBase_DispatchOuter+0xb3 [F:\workspace\_work\1\s\src\coreclr\src\vm\threads.cpp @ 7529] 
(Inline Function) --------`--------     : --------`-------- --------`-------- --------`-------- --------`-------- : coreclr!ManagedThreadBase_FullTransition+0x23 (Inline Function @ 00007ff8`29a6044e) [F:\workspace\_work\1\s\src\coreclr\src\vm\threads.cpp @ 7576] 
(Inline Function) --------`--------     : --------`-------- --------`-------- --------`-------- --------`-------- : coreclr!ManagedThreadBase::ThreadPool+0x23 (Inline Function @ 00007ff8`29a6044e) [F:\workspace\_work\1\s\src\coreclr\src\vm\threads.cpp @ 7618] 
0000006e`008ff680 00007ff8`29abf942     : 00000000`00000000 00000230`704c6270 00000230`70517d50 00000000`00000000 : coreclr!ManagedPerAppDomainTPCount::DispatchWorkItem+0x8e [F:\workspace\_work\1\s\src\coreclr\src\vm\threadpoolrequest.cpp @ 642] 
(Inline Function) --------`--------     : --------`-------- --------`-------- --------`-------- --------`-------- : coreclr!ThreadpoolMgr::ExecuteWorkRequest+0x1d2 (Inline Function @ 00007ff8`29abf942) [F:\workspace\_work\1\s\src\coreclr\src\vm\win32threadpool.cpp @ 1552] 
0000006e`008ff6f0 00007ff8`a8f27034     : 00000000`00000000 00000000`00000000 00000000`00000000 00000000`00000000 : coreclr!ThreadpoolMgr::WorkerThreadStart+0x342 [F:\workspace\_work\1\s\src\coreclr\src\vm\win32threadpool.cpp @ 1977] 
0000006e`008ff840 00007ff8`aaf02651     : 00000000`00000000 00000000`00000000 00000000`00000000 00000000`00000000 : KERNEL32!BaseThreadInitThunk+0x14
0000006e`008ff870 00000000`00000000     : 00000000`00000000 00000000`00000000 00000000`00000000 00000000`00000000 : ntdll!RtlUserThreadStart+0x21
```
6. Look at the first parameter of **coreclr!JIT_MonEnter_Helper**, this would be the address of object the thread is **trying** to acquire the lock to. As we can see from the ``kv`` output above, thread 8 is trying to acquire lock to a mutex object owned by thread 9 (highlighted in the command output by '**')
7. Do the same process for thread 9 and verify that it is waiting to acquire lock on thread 8's mutex object 


# Solution 2 (with SOS and SOSEX)
## Solution 1 (with SOS only)
0. Ensure SOSEX and SOS are loaded
1. Run ``!syncblk`` command and see what locks are there. Since we know that something is stuck and there is two locks, we should suspect a deadlock.
```
0:008> !syncblk
Index         SyncBlock MonitorHeld Recursion Owning Thread Info          SyncBlock Owner
   11 000002306E5B3AB8            3         1 000002307051F7F0 2ae4   9   000002300000edb0 System.Object
   12 000002306E5B3B08            3         1 0000023070517D50 43b0   8   000002300000ed78 System.Object
```
2. Write down the two lock object addresses and owning thread numbers, in this case this is
```
thread 9 locks on object 000002300000edb0
thread 8 locks on object 000002300000ed78
```
3. Use ``~8s`` to switch context to thread 8 then run ``!mk``
```
:008> !mk
Thread 8:
           SP               IP
:U 0000006e008fe6a8 00007ff8aaf4d764 ntdll!NtWaitForMultipleObjects+0x14
:U 0000006e008fe6b0 00007ff8a8770d40 KERNELBASE!WaitForMultipleObjectsEx+0xf0
:U 0000006e008fe9a0 00007ff829a87f80 coreclr!Thread::DoAppropriateWaitWorker+0x1f8 [F:\workspace\_work\1\s\src\coreclr\src\vm\threads.cpp @ 3437]
:U 0000006e008fea90 00007ff829a87cdd coreclr!Thread::DoAppropriateWait+0x89 [F:\workspace\_work\1\s\src\coreclr\src\vm\threads.cpp @ 3154]
:U 0000006e008feb10 00007ff829a856e1 coreclr!CLREventBase::Wait+0x59 [F:\workspace\_work\1\s\src\coreclr\src\vm\synch.cpp @ 417]
:U 0000006e008feb60 00007ff829a85370 coreclr!AwareLock::EnterEpilogHelper+0x14c [F:\workspace\_work\1\s\src\coreclr\src\vm\syncblk.cpp @ 2630]
:U 0000006e008fec40 00007ff829a8520a coreclr!AwareLock::EnterEpilog+0x42 [F:\workspace\_work\1\s\src\coreclr\src\vm\syncblk.cpp @ 2501]
:U 0000006e008fec90 00007ff829a851b6 coreclr!AwareLock::Enter+0xa2 [F:\workspace\_work\1\s\src\coreclr\src\vm\syncblk.cpp @ 2403]
:U 0000006e008fecc0 00007ff829a84e07 coreclr!JIT_MonEnter_Helper+0x107 [F:\workspace\_work\1\s\src\coreclr\src\vm\jithelpers.cpp @ 3735]
:U 0000006e008fee50 00007ff829a84a47 coreclr!JIT_MonReliableEnter_Portable+0xd7 [F:\workspace\_work\1\s\src\coreclr\src\vm\jithelpers.cpp @ 3783]
a:M 0000006e008fee90 00007ff8295eaf9f System.Threading.Monitor.Enter(System.Object, Boolean ByRef)(+0xf Native)
b:M 0000006e008feec0 00007ff7c9fbddc6 Deadlock.Program+ResourceConsumer.DoWork()(+0x51 IL,+0x126 Native)
c:M 0000006e008fef50 00007ff7c9fbdc7b Deadlock.Program+<>c__DisplayClass3_0.<Main>b__0()(+0xb IL,+0x2b Native)
d:M 0000006e008fef80 00007ff82960a09e System.Threading.Tasks.Task.InnerInvoke()(+0x1e Native)
e:M 0000006e008fefb0 00007ff82960fc01 System.Threading.Tasks.Task+<>c.<.cctor>b__277_0(System.Object)(+0x0 IL,+0x11 Native)
f:M 0000006e008fefe0 00007ff8295f5462 System.Threading.ExecutionContext.RunFromThreadPoolDispatchLoop(System.Threading.Thread, System.Threading.ExecutionContext, System.Threading.ContextCallback, System.Object)(+0x15 IL,+0x42 Native)
:M 0000006e008ff030 00007ff829609edb System.Threading.Tasks.Task.ExecuteWithThreadLocal(System.Threading.Tasks.Task ByRef, System.Threading.Thread)(+0xb9 IL,+0x1cb Native)
:M 0000006e008ff0e0 00007ff829609cb3 System.Threading.Tasks.Task.ExecuteEntryUnsafe(System.Threading.Thread)(+0x53 Native)
:M 0000006e008ff120 00007ff829609c5a System.Threading.Tasks.Task.ExecuteFromThreadPool(System.Threading.Thread)(+0x0 IL,+0xa Native)
:M 0000006e008ff150 00007ff8295fe37a System.Threading.ThreadPoolWorkQueue.Dispatch()(+0xd9 IL,+0x1ba Native)
:M 0000006e008ff200 00007ff8295ed83a System.Threading._ThreadPoolWaitCallback.PerformWaitCallback()(+0x0 IL,+0xa Native)
:U 0000006e008ff230 00007ff829b19c13 coreclr!CallDescrWorkerInternal+0x83 [F:\workspace\_work\1\s\src\coreclr\src\vm\amd64\CallDescrWorkerAMD64.asm @ 100]
:U 0000006e008ff270 00007ff829a461b0 coreclr!MethodDescCallSite::CallTargetWorker+0x268 [F:\workspace\_work\1\s\src\coreclr\src\vm\callhelpers.cpp @ 552]
:U 0000006e008ff3b0 00007ff829a45e6d coreclr!QueueUserWorkItemManagedCallback+0xed [F:\workspace\_work\1\s\src\coreclr\src\vm\comthreadpool.cpp @ 464]
:U 0000006e008ff4c0 00007ff829a606a3 coreclr!ManagedThreadBase_DispatchMiddle+0x8f [F:\workspace\_work\1\s\src\coreclr\src\vm\threads.cpp @ 7369]
:U 0000006e008ff5d0 00007ff829a60587 coreclr!ManagedThreadBase_DispatchOuter+0xb3 [F:\workspace\_work\1\s\src\coreclr\src\vm\threads.cpp @ 7529]
a:U 0000006e008ff680 00007ff829a6044e coreclr!ManagedPerAppDomainTPCount::DispatchWorkItem+0x8e [F:\workspace\_work\1\s\src\coreclr\src\vm\threadpoolrequest.cpp @ 642]
b:U 0000006e008ff6f0 00007ff829abf942 coreclr!ThreadpoolMgr::WorkerThreadStart+0x342 [F:\workspace\_work\1\s\src\coreclr\src\vm\win32threadpool.cpp @ 1977]
c:U 0000006e008ff840 00007ff8a8f27034 KERNEL32!BaseThreadInitThunk+0x14
d:U 0000006e008ff870 00007ff8aaf02651 ntdll!RtlUserThreadStart+0x21
```

4. Run ``!mframe b`` to switch the context to frame that runs **DoWork()** - the last method to run before **Sytem.Threading.Monitor.Enter()**. We need this in order to try and find out what parameter is passed to **Monitor.Enter** (notice frame numbering at the left column -> a:M, b:M, ...)
5. Run ``!mdv`` to see the local variables and arguments of the frame we select in step #4
```
0:008> !mdv
Frame 0xb: (Deadlock.Program+ResourceConsumer.DoWork()):
[A0]:this:Failed to load App Domain data for the shared AppDomain. Error = 0x80070057
000002300000edc8 (ResourceConsumer)
[L0]:Failed to load App Domain data for the shared AppDomain. Error = 0x80070057
000002300000ed78 (System.Object)
[L1]:true (System.Boolean)
[L2]:Failed to load App Domain data for the shared AppDomain. Error = 0x80070057
000002300000edb0 (System.Object)
[L3]:false (System.Boolean)
```
*Note that L = Local Variable and A = Argument*  
We have both the owned mutex object by thread 8 and 9 in here - and this means that one is the owned lock and the other is the lock we want to acquire.  
6. Do the same process with thread 9 and see that it has also reference for thread 8 and 9 in DoWork() method

# Solution 3 (SOSEX)
0. Ensure SOS and SOSEX are loaded
1. Run ``!dlk`` command (SOSEX) - it will detect any managed deadlocks
```
0:008> !dlk
Examining SyncBlocks...
Failed to load App Domain data for the shared AppDomain. Error = 0x80070057
Scanning for ReaderWriterLock(Slim) instances...
Scanning for holders of ReaderWriterLock locks...
Failed to load App Domain data for the shared AppDomain. Error = 0x80070057
Scanning for holders of ReaderWriterLockSlim locks...
Examining CriticalSections...
Scanning for threads waiting on SyncBlocks...
Scanning for threads waiting on ReaderWriterLock locks...
Failed to load App Domain data for the shared AppDomain. Error = 0x80070057
Scanning for threads waiting on ReaderWriterLocksSlim locks...
Scanning for threads waiting on CriticalSections...
*** WARNING: Unable to verify checksum for C:\Users\User\source\repos\WindbgWorkshop\High CPU and Threads\Deadlock\bin\Debug\net5.0\Deadlock.exe
*DEADLOCK DETECTED*
CLR thread 0x5 holds the lock on SyncBlock 000002306e5b3ab8 OBJ:000002300000edb0[System.Object]
...and is waiting for the lock on SyncBlock 000002306e5b3b08 OBJ:000002300000ed78[System.Object]
CLR thread 0x4 holds the lock on SyncBlock 000002306e5b3b08 OBJ:000002300000ed78[System.Object]
...and is waiting for the lock on SyncBlock 000002306e5b3ab8 OBJ:000002300000edb0[System.Object]
CLR Thread 0x5 is waiting at System.Threading.Monitor.Enter(System.Object, Boolean ByRef)(+0xf Native)
CLR Thread 0x4 is waiting at System.Threading.Monitor.Enter(System.Object, Boolean ByRef)(+0xf Native)


1 deadlock detected.
```