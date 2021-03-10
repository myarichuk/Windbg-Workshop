## Solution
* Take dump with first-chance exception by using ``procdump64.exe -e 1 -g -b -ma -w Crash.exe``  
  * Notice that second-chance exception will not yield stack traces
* Ensure symbols are loaded then run ``!analyze -v`` which will yield the throwing thread and a stack-trace
