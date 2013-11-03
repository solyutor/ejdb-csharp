using System;

namespace Ejdb
{
	[AttributeUsage(AttributeTargets.Delegate, AllowMultiple = false, Inherited = false)]
	public class UnmanagedProcedure : Attribute
	{
		public readonly string Name;

		public UnmanagedProcedure(string name)
		{
			if (string.IsNullOrWhiteSpace(name))
			{
				throw new ArgumentException("Name should be neither null nor empty string", "name");
			}
			Name = name;
		}
	}
}