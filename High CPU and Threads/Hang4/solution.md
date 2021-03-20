## Solution
1. Since we see symptoms of a deadlock, check with ``!syncblk``, ``!cs -l`` or ``!dlk`` (SOSEX) if there are some suspicious locks. We will see that there is no suspicious locks.
2.  Since there is no locks despite deadlock symptoms, we should suspect a livelock. Use the following command to take 5 mini-dumps within 5 seconds delay between each
```cmd
.\procdump64.exe -n 5 -s 5 -w Deadlock3.exe
```

3. Open the dumps one after another and execute the following command. Try to see a repeating pattern - livelocks typically try to repeatedly acquire a resource or wait for such resource to be freed.  
Notice that livelock *doesn't have* to use mutexes or any other synchronization primitives.
```windbg
:006> !eestack -short -ee
--------------------------------------------
Thread   0
Current frame: 
Child-SP         RetAddr          Caller, Callee
A1977E540 00007ff7f3fb4141 (MethodDesc 00007ff7f4099978 + 0xa1 ILStubClass.IL_STUB_PInvoke(IntPtr, InputRecord ByRef, Int32, Int32 ByRef))
A1977E5A0 00007ff7f3fb4141 (MethodDesc 00007ff7f4099978 + 0xa1 ILStubClass.IL_STUB_PInvoke(IntPtr, InputRecord ByRef, Int32, Int32 ByRef))
A1977E610 00007ff88ee46010 (MethodDesc 00007ff7f3ec26c0 + 0xb0 System.ConsolePal.ReadKey(Boolean))
A1977E700 00007ff88ee43943 (MethodDesc 00007ff7f3e6ad78 + 0x13 System.Console.ReadKey())
A1977E730 00007ff7f3d97619 (MethodDesc 00007ff7f3e34988 + 0x449 Deadlock3.Program.Main(System.String[]))
--------------------------------------------
Thread   3
Current frame: 
Child-SP         RetAddr          Caller, Callee
A19BFF320 00007ff83151e517 (MethodDesc 00007ff7f3efbc40 + 0x87 System.Gen2GcCallback.Finalize())
--------------------------------------------
Thread   4
Current frame: 
Child-SP         RetAddr          Caller, Callee
--------------------------------------------
Thread   5
Current frame: 
Child-SP         RetAddr          Caller, Callee
A19D7F140 00007ff7f3fb6527 (MethodDesc 00007ff7f3ec8770 + 0x1a7 System.IO.StreamWriter.WriteLine(System.String))
A19D7F250 00007ff7f3fb42f6 (MethodDesc 00007ff7f3e6b8c0 + 0x146 Deadlock3.Program+Philosopher.EatWith(Fork, Philosopher[]))
A19D7F400 00007ff7f3fb37b6 (MethodDesc 00007ff7f3e66288 + 0x136 Deadlock3.Program+<>c__DisplayClass2_1.<Main>b__1())
A19D7F490 00007ff8315ebd6f (MethodDesc 00007ff7f4098078 + 0x2f System.Threading.ThreadHelper.ThreadStart_Context(System.Object))
A19D7F4C0 00007ff8315f5310 (MethodDesc 00007ff7f4098478 + 0x80 System.Threading.ExecutionContext.RunInternal(System.Threading.ExecutionContext, System.Threading.ContextCallback, System.Object))
A19D7F530 00007ff8315ebe7b (MethodDesc 00007ff7f40980c0 + 0x2b System.Threading.ThreadHelper.ThreadStart())
--------------------------------------------
Thread   6
Current frame: 
Child-SP         RetAddr          Caller, Callee
A19EFF480 00007ff7f3fb6527 (MethodDesc 00007ff7f3ec8770 + 0x1a7 System.IO.StreamWriter.WriteLine(System.String))
A19EFF590 00007ff7f3fb42f6 (MethodDesc 00007ff7f3e6b8c0 + 0x146 Deadlock3.Program+Philosopher.EatWith(Fork, Philosopher[]))
A19EFF740 00007ff7f3fb37b6 (MethodDesc 00007ff7f3e66288 + 0x136 Deadlock3.Program+<>c__DisplayClass2_1.<Main>b__1())
A19EFF7D0 00007ff8315ebd6f (MethodDesc 00007ff7f4098078 + 0x2f System.Threading.ThreadHelper.ThreadStart_Context(System.Object))
A19EFF800 00007ff8315f5310 (MethodDesc 00007ff7f4098478 + 0x80 System.Threading.ExecutionContext.RunInternal(System.Threading.ExecutionContext, System.Threading.ContextCallback, System.Object))
A19EFF870 00007ff8315ebe7b (MethodDesc 00007ff7f40980c0 + 0x2b System.Threading.ThreadHelper.ThreadStart())
--------------------------------------------
Thread   7
Current frame: 
Child-SP         RetAddr          Caller, Callee
A1A07F590 00007ff7f3fb6527 (MethodDesc 00007ff7f3ec8770 + 0x1a7 System.IO.StreamWriter.WriteLine(System.String))
A1A07F5A0 00007ff7f3fb4c33 (MethodDesc 00007ff7f40994d8 + 0x33 Deadlock3.Program+Philosopher+<>c.<EatWith>b__7_3(Philosopher))
A1A07F640 00007ff8314f1349 (MethodDesc 00007ff7f3e16f60 + 0xe9 System.String.Concat(System.String, System.String, System.String, System.String))
A1A07F6A0 00007ff7f3fb46d7 (MethodDesc 00007ff7f3e6b8c0 + 0x527 Deadlock3.Program+Philosopher.EatWith(Fork, Philosopher[]))
A1A07F850 00007ff7f3fb37b6 (MethodDesc 00007ff7f3e66288 + 0x136 Deadlock3.Program+<>c__DisplayClass2_1.<Main>b__1())
A1A07F8E0 00007ff8315ebd6f (MethodDesc 00007ff7f4098078 + 0x2f System.Threading.ThreadHelper.ThreadStart_Context(System.Object))
A1A07F910 00007ff8315f5310 (MethodDesc 00007ff7f4098478 + 0x80 System.Threading.ExecutionContext.RunInternal(System.Threading.ExecutionContext, System.Threading.ContextCallback, System.Object))
A1A07F980 00007ff8315ebe7b (MethodDesc 00007ff7f40980c0 + 0x2b System.Threading.ThreadHelper.ThreadStart())
```

4. Use ``!clrstack -p`` to inspect "owner" field of **Fork** object and see how owner changes repeatedly. <br/>
  *Note that viewing parameters require full dumps, you can take multiple full dumps with procdump by using parameter '-ma'*
5. Take more dumps if you still having trouble seeing the above