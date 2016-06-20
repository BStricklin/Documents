using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.IO;

namespace ducklog
{
    class DuckDebug
    {
        public static string logPath = @"C:\Users\" + Environment.UserName + @"\Appdata\Roaming\DuckDebug\";
        private static string ModName = "UnnamedMod";
        public static void Write(string s)
        {
            if (!Directory.Exists(logPath))
            {
                Directory.CreateDirectory(logPath);
            }
            try
            {
                File.AppendAllText(Path.Combine(logPath, "duckdebug.txt"), Environment.NewLine + DateTime.Now.ToString() + " " + ModName + ": " + s);
            }
            catch
            {
            }
            }

        public static void clearLog()
        {
            File.WriteAllText(Path.Combine(logPath, "duckdebug.txt"),"cleared");
        }

        public static void changePath(string newPath)
        {
            logPath = newPath;
        }

        public static string getPath()
        {
            return logPath;
        }
        public static void setName(string name)
        {
            ModName = name;
        }

        public static string getName()
        {
            return ModName;
        }

        

    }
}
