using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Diagnostics;

namespace DuckDebug
{
    class Program
    {
        public static string logPath = @"C:\Users\"+Environment.UserName+@"\Appdata\Roaming\DuckDebug\";
        static private int line;
        private static int Eline;
        private static bool launch;
        private static bool commands;
        private static string launchcommands;
        static void Main(string[] args)
        {
            Console.Title = "DuckDebug";
           
            for(int i = 0;i<args.Length;i++)
            {
                if(args[i] == "-launch")
                {
                    launch = true;
                }
                if(args[i] == "-p")
                {
                    logPath = args[i + 1];
                }
                if(args[i] == "+args")
                {
                    launchcommands = args[i + 1];
                    commands = true;
                }

            }

            Console.ForegroundColor = ConsoleColor.Yellow;
            if (!Directory.Exists(logPath))
            {
                Console.WriteLine("Missing Directories, creating new ones.");
                Directory.CreateDirectory(logPath);
            }
            if (!File.Exists(Path.Combine(logPath, "duckdebug.txt")))
            {
                Console.WriteLine("Found no logfile, creating a new at: {0}",logPath);
                File.WriteAllText(Path.Combine(logPath, "duckdebug.txt"),"ALL THE LOG STUFF GOES IN HERE");
            }
            DirectoryInfo mDir = new DirectoryInfo(@"C:\Users\"+Environment.UserName+ @"\Documents\DuckGame\Mods\");
            DirectoryInfo[] localMods = mDir.GetDirectories();
            String modNames = "";

            for(int i = 0;i<localMods.Length;i++)
            {

                FileSystemWatcher f = new FileSystemWatcher();
                f.Path = localMods[i].FullName + @"\";
                f.Filter = "*.log";
                f.NotifyFilter = NotifyFilters.LastAccess | NotifyFilters.LastWrite
           | NotifyFilters.FileName | NotifyFilters.DirectoryName;
                f.Changed += new FileSystemEventHandler(buildError);
                f.EnableRaisingEvents = true;
                if (i == localMods.Length - 1)
                {
                    modNames = modNames + localMods[i].Name;
                }else if(i == localMods.Length - 2)
                {
                    modNames = modNames + localMods[i].Name + " and ";
                }
                else
                {
                    modNames = modNames + localMods[i].Name + ", ";
                }
                
            }
            Console.Write("Found {0} local mods in Mods directory, \n{1} \n",localMods.Length,modNames); 

            Console.ResetColor();
            line = File.ReadAllLines(Path.Combine(logPath, "duckdebug.txt")).Length;
            Log();
            if (launch)
            {
                ProcessStartInfo _info = new ProcessStartInfo();
                _info.WorkingDirectory = @"C:\Program Files (x86)\Steam\steamapps\common\Duck Game\";
                _info.FileName = "DuckGame.exe";
                if (commands)
                {
                    _info.Arguments = launchcommands.Replace(',', ' ');
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine("starting DuckGame with the arguments: {0}", _info.Arguments);
                    Console.ResetColor();
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine("starting DuckGame..");
                    Console.ResetColor();
                }
                    System.Diagnostics.Process.Start(_info);
            }
            Console.WriteLine("press ESCAPE to exit");
            while (Console.ReadKey(true).Key != ConsoleKey.Escape) ;

        }

        static void buildError(object source, FileSystemEventArgs e)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            String[] lines = File.ReadAllLines(e.FullPath);
            List<string> eLines = new List<string>();
            for(int i = 1;i<lines.Length;i++)
            {
                if (lines[i] != "")
                {
                    if (lines[i].Substring(0, 2) == "c:" && lines[i].Contains("error"))
                    {
                        eLines.Add(lines[i]);
                    }
                }
            }
            string s = "";
            foreach(string t in eLines)
            {
                s = s + t + "\n";
            }
            Console.WriteLine("a build error occured in the mod: {0}, \n{1}",new DirectoryInfo(Path.GetDirectoryName(e.FullPath)).Name,s);
            Console.ResetColor();
        }
        static void Log()
        {
            FileSystemWatcher watcher = new FileSystemWatcher();
            watcher.Path = logPath;
            watcher.NotifyFilter = NotifyFilters.LastAccess | NotifyFilters.LastWrite
           | NotifyFilters.FileName | NotifyFilters.DirectoryName;
            watcher.Filter = "duckdebug.txt";
            watcher.Changed += new FileSystemEventHandler(Change);
            watcher.EnableRaisingEvents = true;

            if (File.Exists(@"C:\Program Files (x86)\Steam\steamapps\common\Duck Game\ducklog.txt"))
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("Found ducklog, listening to file");
                Console.ResetColor();
                Eline = File.ReadAllLines(@"C:\Program Files (x86)\Steam\steamapps\common\Duck Game\ducklog.txt").Length;
                FileSystemWatcher Ewatcher = new FileSystemWatcher();
                Ewatcher.Path = @"C:\Program Files (x86)\Steam\steamapps\common\Duck Game\";
                Ewatcher.NotifyFilter = NotifyFilters.LastAccess | NotifyFilters.LastWrite
               | NotifyFilters.FileName | NotifyFilters.DirectoryName;
                Ewatcher.Filter = "ducklog.txt";
                Ewatcher.Changed += new FileSystemEventHandler(Error);
                Ewatcher.EnableRaisingEvents = true;
            }

        }
        public static void Error(object source, FileSystemEventArgs e)
        {
            string[] lines = File.ReadAllLines(@"C:\Program Files (x86)\Steam\steamapps\common\Duck Game\ducklog.txt");
            string a = "";
            int b = 1;
            for(int i = Eline - 1; i < lines.Length; i++)
            {
                if (b <= 2)
                {
                    a = a + lines[i] + "\n";
                }
                else
                {
                    break;
                }
                b++;
                
            }
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("FATAL ERROR: {0} \n", a);
            Console.ResetColor();
        }
        static void Change(object source,FileSystemEventArgs e)
        {
            try {
                string[] s = File.ReadAllLines(Path.Combine(logPath, "duckdebug.txt"));
                if(s[0] == "clearAllplez")
                {
                    Console.Clear();
                }
                line = s.Length - 1;
                Console.WriteLine(s[line]);
            }
            catch { }

        }
    }
}
