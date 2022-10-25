# Hands-on excecises for a WinDBG workshop
This repository contains hands-on excercises for WinDBG related workshops (both about memory management and scenarios for high CPU usage, deadlocks and other threading issues)  
Eeach excercise is a small application that simulates a particular issue, which can be then investigated using a memory dump and WinDBG.  
In order to try the excercises, you should do ensure your development workstation is ready for dump investigation. The assumption is that the dev machine runs Windows 10.  
*Note: it is possible to investigate memory dumps on Linux/MacOS as well - the principle is the same yet the tools will be different*

## The excercises
For DotNext workshop "Crashes, hangs, and high CPU", we will use excercises found in the "High CPU and Threads" folder of the solution. Note that each excercise contains **hints.md**, which contains hints on how the case can be investigated and **solution.md** which contains the commands needed to investigate the issue (with explanations as well :)) 

## Preparing your dev machine for the workshop123
1. Make sure you have .Net 4.8 developer pack and .Net 5.0 SDK are installed (https://dotnet.microsoft.com/download/visual-studio-sdks)
2. Ensure you have Visual Studio 2019 or any IDE capable of compiling and running .Net 5.0
3. Install [**WinDbg Preview**](https://www.microsoft.com/en-us/p/windbg-preview/9pgjgd53tn86?activetab=pivot:overviewtab) from Windows Store.
4. WinDbg requires a bit of configuring. Take a look at the following or use [this blog post](http://www.graymatterdeveloper.com/2020/02/12/setting-up-windbg/) as a guide  
  Set up symbol resolution string. The format of the symbol string is **cache*[local cache folder 1]*[local cache folder 2];srv*[local cache folder]*[symbol server path]**  
  Note 1: This will enable WinDbg to resolve symbols so proper stack traces can be displayed.  
  Note 2: *!sym noisy on* command will enable debug output to see what kind of symbols WinDbg looks for and why the resolution is missing.
  ![](https://github.com/myarichuk/Memory-Leak-Investigation-Workshop/blob/master/Images/SymbolsInWinDBG.PNG) 
5. Download **Process Explorer** from [here](https://docs.microsoft.com/en-us/sysinternals/downloads/process-explorer). It will be useful for taking dumps of processes.
6. Download and install Windows 10 SDK (you can [download it from here](https://developer.microsoft.com/en-us/windows/downloads/windows-10-sdk/)). Make sure to tick **Debugging Tools for Windows** when you install
7. Download **ProcDump** from [here](https://docs.microsoft.com/en-us/sysinternals/downloads/procdump)
