## Solution
1. Since we suspect a deadlock, we run first ``!syncblk`` to see if there are any managed locks. Alternatively, we can load SOSEX and run ``!dlk`` as it will cover also ReadWriteLock variants as well.
```
0:000> !syncblk
Index         SyncBlock MonitorHeld Recursion Owning Thread Info          SyncBlock Owner
-----------------------------
Total           65
CCW             4
RCW             5
ComClassFactory 0
Free            0
```
There is no managed locks, so we try to see if there is another reason for a deadlock

2. Run ``!dumpasync -stacks``, which will display all current async state machines. In this hands-on you should see only one state machine. Since we know that something get stuck in the process and there is no **syncblk** segments, it is possible that the hang is due to async deadlock.
```
0:020> !dumpasync -stacks
Statistics:
              MT    Count    TotalSize Class Name
00007ff7cf6a4918        1           96 System.Runtime.CompilerServices.AsyncTaskMethodBuilder`1+AsyncStateMachineBox`1[[System.String, System.Private.CoreLib],[Deadlock2.MainWindow+<GetJsonFromEndpointAsync>d__5, Deadlock2]]
00007ff7cf6a38c8        1           96 System.Runtime.CompilerServices.AsyncTaskMethodBuilder`1+AsyncStateMachineBox`1[[System.Threading.Tasks.VoidTaskResult, System.Private.CoreLib],[Deadlock2.MainWindow+<DoLongTask>d__3, Deadlock2]]
Total 2 objects
In 1 chains.
         Address               MT     Size      State Description
000001f88025d7a8 00007ff7cf6a38c8       96          0 Deadlock2.MainWindow+<DoLongTask>d__3
Async "stack":
.000001f88025d878 (0) Deadlock2.MainWindow+<GetJsonFromEndpointAsync>d__5
..000001f88025d948 System.Threading.Tasks.Task+SetOnInvokeMres
--------------------------------------------------------------------------------
```
We see that the there is one async state machine executing and the method **GetJsonFromEndpointAsync()** is waiting for something. Let's see what exactly it is doing.

3. We take the address of state machine from previous command and call ``!DumpAsync -addr 000001f88025d7a8 -roots`` to see where the suspect state machine is "referenced" in code.
```
0:000> !DumpAsync -addr 000001f88025d7a8 -stacks -roots
         Address               MT     Size      State Description
000001f88025d7a8 00007ff7cf6a38c8       96          0 Deadlock2.MainWindow+<DoLongTask>d__3
Async "stack":
.000001f88025d878 (0) Deadlock2.MainWindow+<GetJsonFromEndpointAsync>d__5
..000001f88025d948 System.Threading.Tasks.Task+SetOnInvokeMres
GC roots:
    Thread 49d8:
        00000088F157D2B0 00007FF82271A939 System.Threading.Tasks.Task.SpinThenBlockingWait(Int32, System.Threading.CancellationToken) [/_/src/libraries/System.Private.CoreLib/src/System/Threading/Tasks/Task.cs @ 2849]
            rbp+10: 00000088f157d320
                ->  000001F88025D878 System.Runtime.CompilerServices.AsyncTaskMethodBuilder`1+AsyncStateMachineBox`1[[System.String, System.Private.CoreLib],[Deadlock2.MainWindow+<GetJsonFromEndpointAsync>d__5, Deadlock2]]
                ->  000001F88025CF10 Deadlock2.MainWindow+<GetJsonFromEndpointAsync>d__5
                ->  000001F88025D7A8 System.Runtime.CompilerServices.AsyncTaskMethodBuilder`1+AsyncStateMachineBox`1[[System.Threading.Tasks.VoidTaskResult, System.Private.CoreLib],[Deadlock2.MainWindow+<DoLongTask>d__3, Deadlock2]]
    
        00000088F157D410 00007FF7CF581F7B Deadlock2.MainWindow.Button_Click(System.Object, System.Windows.RoutedEventArgs) [C:\Users\User\source\repos\WindbgWorkshop\Deadlock2\MainWindow.xaml.cs @ 18]
            rbp-8: 00000088f157d448
                ->  000001F8800DE5E0 System.Windows.Controls.Button
                ->  000001F8800398F0 System.Windows.Threading.Dispatcher
                ->  000001F8801CEC60 System.EventHandler
                ->  000001F880195B50 System.Object[]
                ->  000001F8800DA740 System.EventHandler
                ->  000001F8800D93D0 System.Windows.Media.MediaContext
                ->  000001F88025BA00 System.Windows.Threading.DispatcherOperation
                ->  000001F88025BB28 System.Windows.Threading.PriorityItem`1[[System.Windows.Threading.DispatcherOperation, WindowsBase]]
                ->  000001F88025E260 System.Windows.Threading.PriorityItem`1[[System.Windows.Threading.DispatcherOperation, WindowsBase]]
                ->  000001F88025E138 System.Windows.Threading.DispatcherOperation
                ->  000001F88025D838 System.Action
                ->  000001F88025D7A8 System.Runtime.CompilerServices.AsyncTaskMethodBuilder`1+AsyncStateMachineBox`1[[System.Threading.Tasks.VoidTaskResult, System.Private.CoreLib],[Deadlock2.MainWindow+<DoLongTask>d__3, Deadlock2]]
    
--------------------------------------------------------------------------------
0:000> !DumpAsync -addr 000001f88025d7a8 -roots
         Address               MT     Size      State Description
000001f88025d7a8 00007ff7cf6a38c8       96          0 Deadlock2.MainWindow+<DoLongTask>d__3
GC roots:
    Thread 49d8:
        00000088F157D2B0 00007FF82271A939 System.Threading.Tasks.Task.SpinThenBlockingWait(Int32, System.Threading.CancellationToken) [/_/src/libraries/System.Private.CoreLib/src/System/Threading/Tasks/Task.cs @ 2849]
            rbp+10: 00000088f157d320
                ->  000001F88025D878 System.Runtime.CompilerServices.AsyncTaskMethodBuilder`1+AsyncStateMachineBox`1[[System.String, System.Private.CoreLib],[Deadlock2.MainWindow+<GetJsonFromEndpointAsync>d__5, Deadlock2]]
                ->  000001F88025CF10 Deadlock2.MainWindow+<GetJsonFromEndpointAsync>d__5
                ->  000001F88025D7A8 System.Runtime.CompilerServices.AsyncTaskMethodBuilder`1+AsyncStateMachineBox`1[[System.Threading.Tasks.VoidTaskResult, System.Private.CoreLib],[Deadlock2.MainWindow+<DoLongTask>d__3, Deadlock2]]
    
        00000088F157D410 00007FF7CF581F7B Deadlock2.MainWindow.Button_Click(System.Object, System.Windows.RoutedEventArgs) [C:\Users\User\source\repos\WindbgWorkshop\Deadlock2\MainWindow.xaml.cs @ 18]
            rbp-8: 00000088f157d448
                ->  000001F8800DE5E0 System.Windows.Controls.Button
                ->  000001F8800398F0 System.Windows.Threading.Dispatcher
                ->  000001F8801CEC60 System.EventHandler
                ->  000001F880195B50 System.Object[]
                ->  000001F8800DA740 System.EventHandler
                ->  000001F8800D93D0 System.Windows.Media.MediaContext
                ->  000001F88025BA00 System.Windows.Threading.DispatcherOperation
                ->  000001F88025BB28 System.Windows.Threading.PriorityItem`1[[System.Windows.Threading.DispatcherOperation, WindowsBase]]
                ->  000001F88025E260 System.Windows.Threading.PriorityItem`1[[System.Windows.Threading.DispatcherOperation, WindowsBase]]
                ->  000001F88025E138 System.Windows.Threading.DispatcherOperation
                ->  000001F88025D838 System.Action
                ->  000001F88025D7A8 System.Runtime.CompilerServices.AsyncTaskMethodBuilder`1+AsyncStateMachineBox`1[[System.Threading.Tasks.VoidTaskResult, System.Private.CoreLib],[Deadlock2.MainWindow+<DoLongTask>d__3, Deadlock2]]
    
--------------------------------------------------------------------------------

```
In the command results we see GC roots of our state machine and we see something interesting: the same state machine that is 'blocking-waiting' in **GetJsonFromEndpointAsync()** is also waiting in button click handler.

4. So the state machine is blocking in the **GetJsonFromEndpointAsync()** method, but what is the thread doing?  
So first we run the following to switch the context to the thread that started the async state machine
```
:000> ~~[49d8]s
win32u!NtUserPeekMessage+0x14:
ff8`a8af1064 c3              ret
```
And then we run:
```

:000> !clrstack
OS Thread Id: 0x49d8 (0)
        Child SP               IP Call Site
F157C938 00007ff8a8af1064 [HelperMethodFrame: 00000088f157c938] System.Threading.WaitHandle.WaitMultipleIgnoringSyncContext(IntPtr*, Int32, Boolean, Int32)
F157CA60 00007ff8226fbc32 System.Threading.SynchronizationContext.WaitHelper(IntPtr[], Boolean, Int32) [/_/src/libraries/System.Private.CoreLib/src/System/Threading/SynchronizationContext.cs @ 52]
F157CAA0 00007ff82081974e System.Windows.Threading.DispatcherSynchronizationContext.Wait(IntPtr[], Boolean, Int32) [/_/src/Microsoft.DotNet.Wpf/src/WindowsBase/System/Windows/Threading/DispatcherSynchronizationContext.cs @ 108]
F157CAE0 00007ff8226fbaa1 System.Threading.SynchronizationContext.InvokeWaitMethodHelper(System.Threading.SynchronizationContext, IntPtr[], Boolean, Int32) [/_/src/coreclr/src/System.Private.CoreLib/src/System/Threading/SynchronizationContext.CoreCLR.cs @ 10]
F157D0E8 00007ff82e8a9c13 [HelperMethodFrame_1OBJ: 00000088f157d0e8] System.Threading.Monitor.ObjWait(Boolean, Int32, System.Object)
F157D210 00007ff822706cd3 System.Threading.ManualResetEventSlim.Wait(Int32, System.Threading.CancellationToken)
F157D2B0 00007ff82271a939 System.Threading.Tasks.Task.SpinThenBlockingWait(Int32, System.Threading.CancellationToken) [/_/src/libraries/System.Private.CoreLib/src/System/Threading/Tasks/Task.cs @ 2849]
F157D320 00007ff82271a7e7 System.Threading.Tasks.Task.InternalWaitCore(Int32, System.Threading.CancellationToken) [/_/src/libraries/System.Private.CoreLib/src/System/Threading/Tasks/Task.cs @ 2788]
F157D3A0 00007ff822712b57 System.Threading.Tasks.Task`1[[System.__Canon, System.Private.CoreLib]].GetResultCore(Boolean) [/_/src/libraries/System.Private.CoreLib/src/System/Threading/Tasks/Future.cs @ 464]
F157D3E0 00007ff822712b17 System.Threading.Tasks.Task`1[[System.__Canon, System.Private.CoreLib]].get_Result() [/_/src/libraries/System.Private.CoreLib/src/System/Threading/Tasks/Future.cs @ 439]
F157D410 00007ff7cf581f7b Deadlock2.MainWindow.Button_Click(System.Object, System.Windows.RoutedEventArgs) [C:\Users\User\source\repos\WindbgWorkshop\Deadlock2\MainWindow.xaml.cs @ 18]
F157D460 00007ff7cf5610a3 System.Windows.EventRoute.InvokeHandlersImpl(System.Object, System.Windows.RoutedEventArgs, Boolean) [/_/src/Microsoft.DotNet.Wpf/src/PresentationCore/System/Windows/EventRoute.cs @ 155]
F157D520 00007ff7cf560214 System.Windows.UIElement.RaiseEventImpl(System.Windows.DependencyObject, System.Windows.RoutedEventArgs)
F157D580 000000005db91425 System.Windows.UIElement.RaiseEvent(System.Windows.RoutedEventArgs) [/_/src/Microsoft.DotNet.Wpf/src/PresentationCore/System/Windows/Generated/UIElement.cs @ 419]
F157D5C0 00007ff821451007 System.Windows.Controls.Primitives.ButtonBase.OnClick() [/_/src/Microsoft.DotNet.Wpf/src/PresentationFramework/System/Windows/Controls/Primitives/ButtonBase.cs @ 72]

** The rest of the stack is omitted **
```
We can see that the thread is waiting as well - see the line with **ManualResetEventSlim.Wait()**. <br/>   Let's see if we find the connection on waiting async state machine and the waiting thread.  
In order to look into the **ManualResetEventSlim** we will run ``!clrstack -p`` and find the relevant line in the output.
```
00000088F157D210 00007ff822706cd3 System.Threading.ManualResetEventSlim.Wait(Int32, System.Threading.CancellationToken)
    PARAMETERS:
        this (0x00000088F157D2B0) = 0x000001f88025d948
        millisecondsTimeout (<CLR reg>) = 0x00000000ffffffff
        cancellationToken = <no data>
```
5. We have an address of **ManualResetEventSlim** (address = 0x000001f88025d948) which would be the same as was seen in the output of ``!dumpasync -stacks``
```
Async "stack":
.000001f88025d878 (0) Deadlock2.MainWindow+<GetJsonFromEndpointAsync>d__5
..000001f88025d948 System.Threading.Tasks.Task+SetOnInvokeMres
``` 
This means that the thread and async state machine wait on the same ManualResetEventSlim, therefore it is an async deadlock between **GetJsonFromEndpointAsync()** and button click handler.