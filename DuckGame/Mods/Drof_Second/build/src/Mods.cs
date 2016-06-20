using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;

// The title of your mod, as displayed in menus
[assembly: AssemblyTitle("Drof_Second")]

// The author of the mod
[assembly: AssemblyCompany("Drofdarb")]

// The description of the mod
[assembly: AssemblyDescription("Update to first Mod Pack")]

// The mod's version
[assembly: AssemblyVersion("0.8")]

namespace DuckGame.MyMod
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
			base.OnPreInitialize();
		}

		// This function is run after all mods are loaded.
		protected override void OnPostInitialize()
		{
			base.OnPostInitialize();
		}
	}
}

// Special Thanks
//UFF Items Mod
// - AlexMdle, Flan, Pure Question, and ArcOfDream
// 
//Grenade Pack
// - Thomas
//
//MISC Weapons
// - Killer-Fucker
//
//The "Yes" Group: Weapon Mod
// - Old Yensi
