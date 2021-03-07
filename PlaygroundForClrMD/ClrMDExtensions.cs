using Microsoft.Diagnostics.Runtime;
using System;
using System.IO;
using System.Linq;

namespace PlaygroundForClrMD
{
	//credit: adapted from https://github.com/Dynatrace/superdump/blob/master/src/SuperDump/ClrMdExtensions.cs
	public static class ClrMdExtensions
	{
		/// <summary>
		/// Creates and returns ClrRuntime based on dump file (takes first loaded CLR if loaded more than one)
		/// Loads dac from public MS symbol server if dac is not locally available, or add path to dac as param
		/// </summary>
		/// <returns>Returns the created runtime object</returns>
		public static ClrRuntime CreateRuntime(this DataTarget target)
		{
			// now check bitness of our program/target, defensive check if Selector was not used or was wrong
			bool isTarget64Bit = target.DataReader.PointerSize == 8;
			if (Environment.Is64BitProcess != isTarget64Bit)
			{   //check if our process is 64bit or not
				throw new InvalidOperationException(string.Format("Architecture do no match:  Process was {0} but target is {1} !", Environment.Is64BitProcess ? "64 bit" : "32 bit", isTarget64Bit ? "64 bit" : "32 bit"));
			}

			if (target.ClrVersions == null || target.ClrVersions.Length <= 0)
				throw new FileNotFoundException("No CLR was loaded in that process!");

			// Get "highest" CLR loaded and use that
			var clrVersion = target.ClrVersions.First(clrInfo => clrInfo.Version == target.ClrVersions.Max(v => v.Version));

			return clrVersion.CreateRuntime();
		}
	}
}