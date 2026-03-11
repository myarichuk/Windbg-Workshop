# Hands-on exercises for a .NET Diagnostics workshop
This repository contains hands-on exercises for .NET Diagnostics related workshops (both about memory management and scenarios for high CPU usage, deadlocks and other threading issues).
Each exercise is a small application that simulates a particular issue, which can be then investigated using the modern, cross-platform `.NET Diagnostics CLI tools`.

In order to try the exercises, you should ensure your development workstation is ready for dump investigation. These exercises work on Windows, Linux, and MacOS.

## The exercises
The solution is divided into folders (e.g., "High CPU and Threads", "Memory Issues", "Misc"). Note that each exercise contains **hints.md**, which contains hints on how the case can be investigated, and **solution.md** which contains the commands needed to investigate the issue (with explanations as well :))

## Preparing your dev machine for the workshop
1. Install the **.NET 10.0 SDK** (https://dotnet.microsoft.com/download/dotnet/10.0)
2. Ensure you have an IDE capable of compiling and running .NET 10.0 applications (e.g., Visual Studio, Visual Studio Code, Rider).
3. Install the official cross-platform diagnostics tools using the .NET CLI:
```bash
dotnet tool install --global dotnet-counters
dotnet tool install --global dotnet-dump
dotnet tool install --global dotnet-gcdump
dotnet tool install --global dotnet-trace
```
4. You're ready to go! No symbols configuration or manual tool downloads are required with the modern stack.

## Getting started
Explore the applications. They are designed to exhibit specific pathological behaviors like memory leaks, CPU spikes, or deadlocks.
Run the application and monitor its process ID with the diagnostics tools mentioned above.

There's a `ModernDiagnostics` folder with a basic template showing how you can quickly identify the PID and get ready to run these tools.
