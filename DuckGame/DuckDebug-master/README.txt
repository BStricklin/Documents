This is a debugging application for Duckgame mods.
It listens for updates in the Ducklog files and reports them.
Theres an Example of how the DuckDebug.cs file works in the Example folder.

arguments:
-p <path> sets the path to your log file. (default: C:\Users\UserName\Appdata\Roaming\DuckDebug\)
-launch if used duckDebug will launch DuckGame and the arguments selectid with +args.
+args <arguments> selects the arguments that will be used to start DuckGame. Use ',' between if you have multiple argument, dont use spaces.


DuckDebug.cs Class:
public static void Write(string text): Wries a message to the console with the time and the selected ModName.
public static void clearLog(): clears the log file.
public static void changePath(string newPath): sets the new Path.
public static String getPath(): gets the current Path. (default: C:\Users\UserName\Appdata\Roaming\DuckDebug\)
public static string getName(): get the selected ModName.
public static void setName(string newName): sets the name of the mod. (default: UnnamedMod)

i recommend using setName() in preInit or just change the modName string in the DuckDebug.cs file.  

Loading the ExampleMod:
put the Example folder in Documents\DuckGame\Mods\

to start duckgame in the level editor put this in a batch (.bat) file:
<------------------------------------------------->
@echo off
duckdebug -launch +args -startineditor
<------------------------------------------------->
or just add "-launch +args -startineditor" in the start arguments.