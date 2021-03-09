using System;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace Crash
{
    public static class Program
    {
        private static uint SEM_NOGPFAULTERRORBOX = 0x0002;
        [DllImport("Kernel32.dll")]
        private static extern void RaiseException(uint exceptionCode, uint exceptionFlags, uint numberOfArguments, IntPtr arguments);

        [DllImport("kernel32.dll")]
        private static extern int SetErrorMode(uint uMode);

        private enum ExceptionCode : uint
        {
            DivideByZeroException = 0xc0000094,
            StackOverflowException = 0xc00000fd,
            AccessViolationException = 0xc0000005
        }

        public static void SomeOperationThatMayFail(Action operation)
        {
            try
            {
                operation();
            }
            catch(Exception e)
            {
                Console.WriteLine(e);
            }
        }

        public static void Main(string[] args)
        {
            Console.WriteLine("Process ID:" + Process.GetCurrentProcess().Id);
            Console.WriteLine("Choose exception to throw");
            Console.WriteLine("1. " + ExceptionCode.AccessViolationException);
            Console.WriteLine("2. " + ExceptionCode.DivideByZeroException);
            Console.WriteLine("3. " + ExceptionCode.StackOverflowException);
            Console.WriteLine("4. " + nameof(NullReferenceException));
            Console.Write("Choice:");//.ecxr
            int choice = 0;
            SetErrorMode(SEM_NOGPFAULTERRORBOX);
            do
            {
                if(!Int32.TryParse(Console.ReadLine(), out choice))
                {
                    choice = 0;
                    continue;
                }
                switch (choice)
                {
                    case 1:
                        SomeOperationThatMayFail(() => RaiseException((uint)ExceptionCode.AccessViolationException, 0, 0, IntPtr.Zero));
                        break;
                    case 2:
                        SomeOperationThatMayFail(() => RaiseException((uint)ExceptionCode.DivideByZeroException, 0, 0, IntPtr.Zero));
                        break;
                    case 3:
                        SomeOperationThatMayFail(() => RaiseException((uint)ExceptionCode.StackOverflowException, 0, 0, IntPtr.Zero));
                        break;
                    case 4:
                        SomeOperationThatMayFail(() => throw new NullReferenceException("Ooops!"));
                        break;
                }
            } while (choice == 0);

        }
    }
}
