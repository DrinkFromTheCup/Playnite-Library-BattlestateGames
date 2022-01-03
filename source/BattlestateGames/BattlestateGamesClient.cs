using Microsoft.Win32;
using Playnite.SDK;
using Playnite.Common;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace BattlestateGames
{
    public class BattlestateGamesChecksClient : LibraryClient
    {
        public override string Icon => BattlestateGamesChecks.Icon;

        public override bool IsInstalled => false;

        // Keeping it there despite intentional lack of BYOND in launchers launch menu,
        // just in case of interesting interactions with other plugins.
        public override void Open()
        {
            ProcessStarter.StartProcess(BattlestateGamesChecks.ClientExecPath, string.Empty);
        }
    }

    public class BattlestateGamesChecks
    {

        // Do note that since we got everything necessary in InstallationPath few blocks below,
        // we don't need to append anything anywhere. Unless it bugs. Which it won't.
        // It also enforces filecheck usage instead of foldercheck everywhere later.
        public static string ClientExecPath
        {
            get
            {
                var path = InstallationPath;
                return string.IsNullOrEmpty(path) ? string.Empty : Path.Combine(path, "");
            }
        }

        public static bool IsInstalled
        {
            get
            {
                if (string.IsNullOrEmpty(ClientExecPath) || !File.Exists(ClientExecPath))
                {
                    return false;
                }
                else
                {
                    return true;
                }
            }
        }

        // The plugin's Sequence begins here.
        // Our #1 goal is to get the path to launcher.
        // For this particular game, however, we're blessed with STATIC direct link to the launcher's executable.
        // Even better, since we don't need to launch anything else ever (for now) - it all begins here
        // and here it all ends. The rest of the code just works by example to preserve any possible third-party interactions.
        public static string InstallationPath
        {
            get
            {
                RegistryKey key;
                key = Registry.ClassesRoot.OpenSubKey(@"byond\shell\open\command");
                {
                    // We need a path to executable, not a blind URI copy-paste.
                    // Do note that portable client exists - for which this check will return null by default.
                    // Since this install method breaks connection from web interfaces, it will cause trouble to end user anyway
                    // way before our plugin might suffer from that.
                    // I believe, BYOND adds relevant registry entry anyway after first run so...
                    return key.GetValue("").ToString().Replace(" \"%1\"", "");
                }          
            }
        }

        public static string Icon => Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), @"icon.png");
    }
}