Unofficial .Net Binding for [EJDB](http://ejdb.org) 
===================================================


**Note: This is early days, so .Net EJDB binding is not completely tested.**


You can install it using nuget:
--------------------------------

 * install-package Nejdb.Unofficial


**What's differences with official binding**

  0. tcejdbdll.dll is embedded in resource, and loaded at runtime for both x32/x64. 
  1. API is closer to .Net style, not C style.
  2. Better unmanaged resource handling.
  3. Dropped support for Mono (I hope it's not hard to restore it).
  4. Nuget package available.


One snippet intro
---------------------------------

```c#
using System;
using System.IO;
using Nejdb;
using Nejdb.Bson;
using Nejdb.Internals;

namespace sample 
{
	class MainClass 
	{
			using (var library = Library.Create())
			using (var database = library.CreateDatabase())
			{
				database.Open("MyDB.db");
				using (var collection = database.CreateCollection("Parrots", CollectionOptions.None))
				{
					var parrot = BsonDocument.ValueOf(new
					{
						name = "Grenny",
						type = "African Grey",
						male = true,
						age = 1,
						birthdate = DateTime.Now,
						likes = new[] { "green color", "night", "toys" },
						extra = BsonNull.VALUE
					});
					collection.Save(parrot, false);
				}
			}
		}
	}
}
```
