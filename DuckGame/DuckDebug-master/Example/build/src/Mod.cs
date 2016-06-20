using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;

// The title of your mod, as displayed in menus
[assembly: AssemblyTitle("Example")]

// The author of the mod
[assembly: AssemblyCompany("EIM64")]

// The description of the mod
[assembly: AssemblyDescription("An example for testing the DuckDebug application")]

// The mod's version
[assembly: AssemblyVersion("69.69")]

namespace DuckGame.Mymod
{
    public class MyMod : Mod
    {
		// The mod's priority; this property controls the load order of the mod.
		public override Priority priority
		{
			get { return base.priority; }
		}

		// This function is run before all mods are finished loading.
		protected override void OnPreInitialize()
		{
            DuckDebug.DuckDebug.setName("ExampleMod");
            DuckDebug.DuckDebug.Write("PreInit!");
            base.OnPreInitialize();
		}

		// This function is run after all mods are loaded.
		protected override void OnPostInitialize()
		{
            DuckDebug.DuckDebug.Write("PostInit!");
            base.OnPostInitialize();
		}
	}
}
