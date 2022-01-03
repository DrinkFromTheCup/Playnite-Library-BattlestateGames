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

        // Intended to avoid showing up in launcher launch menu.
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
        // We're using #oddlyspecific approach here. We know the key and the value - but we're not entirely sure of the path -
        // thus, abusing that coopy-paaste from Stackexchange!
        // Even better, since we don't need to launch anything else ever (for now) - it all begins here
        // and here it all ends. The rest of the code just works by example to preserve any possible third-party interactions.
        public static string InstallationPath
        {
            get
            {
            // Blatant coopy-paaste from https://stackoverflow.com/a/64029279 for further use in InstallationPath
            // Do not that not us nor solution author neither compilator expect more than one successful iteration here.
            // But we might expand on it during Stage 2.
            RegistryKey uninstallKey = Registry.LocalMachine.OpenSubKey("SOFTWARE\\WOW6432Node\\Microsoft\\Windows\\CurrentVersion\\Uninstall");
            var programs = uninstallKey.GetSubKeyNames();

            foreach (var program in programs)
            {
                RegistryKey subkey = uninstallKey.OpenSubKey(program);
                if (string.Equals("Battlestate Games", subkey.GetValue("Publisher", string.Empty).ToString(), StringComparison.CurrentCulture))
                {
                    return subkey.GetValue("DisplayIcon").ToString();
                }
            }

            return string.Empty;   
            }
        }

        public static string Icon => Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), @"icon.png");
    }
}