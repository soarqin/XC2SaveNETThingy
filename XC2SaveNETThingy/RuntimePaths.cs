using System;
using System.IO;

namespace XC2SaveNETThingy
{
    public static class RuntimePaths
    {
        public static string Resolve(string path)
        {
            if (String.IsNullOrWhiteSpace(path) || Path.IsPathRooted(path))
                return path;

            return Path.Combine(AppContext.BaseDirectory, path);
        }

        public static string DatabaseFile => Resolve("XC2Data.db");
        public static string SettingsFile => Resolve("settings.cfg");
    }
}
