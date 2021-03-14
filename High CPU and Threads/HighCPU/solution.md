## Solution 1 (WinDBG)
1. Since we see a high CPU usage and don't see symptoms of threadpool starvation or high GC usage, we should suspect either an issue with thread scheduling or some sort of CPU-bound bottleneck.
2. We take a number of dumps (minidumps will work as well)
```cmd
.\procdump64.exe -n 3 -s 5 -w HighCPU.exe
```
3. In each of the dumps use command ``!runaway``
```
0:012> !runaway
 User Mode Time
  Thread       Time
    8:21dc     0 days 0:00:07.000
    9:26d0     0 days 0:00:06.921
   10:2544     0 days 0:00:00.046
    0:1e54     0 days 0:00:00.046
    6:63c8     0 days 0:00:00.015
   12:19a0     0 days 0:00:00.000
   11:3f54     0 days 0:00:00.000
    7:574c     0 days 0:00:00.000
    5:5368     0 days 0:00:00.000
    4:834      0 days 0:00:00.000
    3:4214     0 days 0:00:00.000
    2:46f4     0 days 0:00:00.000
    1:39d0     0 days 0:00:00.000
```
In each progressive dump we would see that some threads utilize much more user-mode time than others. This means that for some reason some threads get more processor time than others.
4. One the possible reasons for this is thread priority. Use ``~[thread id]`` command to verify that threads with more user-mode time have higher priority
```
0:008> ~8
.  8  Id: 23e0.21dc Suspend: 2 Teb: 00000089`5e7ca000 Unfrozen "High priority thread #1"
      Start: coreclr!ThreadNative::KickOffThread (00007ff8`0f4644f0)
      Priority: 2  Priority class: 32  Affinity: 1
0:008> ~9
   9  Id: 23e0.26d0 Suspend: 1 Teb: 00000089`5e7cc000 Unfrozen "High priority thread #2"
      Start: coreclr!ThreadNative::KickOffThread (00007ff8`0f4644f0)
      Priority: 2  Priority class: 32  Affinity: 1
0:008> ~10
  10  Id: 23e0.2544 Suspend: 1 Teb: 00000089`5e7ce000 Unfrozen "Low priority thread"
      Start: coreclr!ThreadNative::KickOffThread (00007ff8`0f4644f0)
      Priority: -2  Priority class: 32  Affinity: 1

```
We can see that the thread that gets "starved" has much lower priority, thus it receives much less processor time slices from OS scheduler. Also notice that those threads have the same **Affinity** and this means they compete for the same CPU core and since Windows scheduler manages thread schedule per core, some threads will get starved...
