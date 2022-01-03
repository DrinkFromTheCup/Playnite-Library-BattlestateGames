using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Playnite;
using Playnite.Common;
using Playnite.SDK;
using Playnite.SDK.Models;
using Playnite.SDK.Plugins;

// As seen in https://github.com/JosefNemec/PlayniteExtensions/blob/master/source/Libraries/GogLibrary/GogGameController.cs and the like.
// Despite my bias against keeping redundant processes in memory, borrowing well-written install/uninstall watcher
// seems to be more end-user-friendly than my original intent to just mark everything at will.
// Simplified (almost gutted), since we're handling 1 (one) already-known title only.

namespace BattlestateGames
{
    public class BattlestateGamesInstallController : InstallController
    {
        private CancellationTokenSource watcherToken;

        public BattlestateGamesInstallController(Game game) : base(game)
        {
            if (BattlestateGamesChecks.IsInstalled)
            {
                Name = "Run BSG Launcher";
            }
            else
            // Failsafe for cases where we added games anyway but launcher is nowhere to be seen.
            {
                Name = "Download BSG Launcher";
            }
        }

        public override void Dispose()
        {
            watcherToken?.Cancel();
        }

        public override void Install(InstallActionArgs args)
        {
            if (BattlestateGamesChecks.IsInstalled)
            // ...but it isn't installed. No need in it until they'll decide to stick their older games into that launcher.
            {
            //    ProcessStarter.StartUrl(@"about:blank");
            }
            else
            // Failsafe for cases where we added games anyway but launcher is nowhere to be seen.
            // Yarr possible - but no need in handling it specifically...
            {
                ProcessStarter.StartUrl(@"https://prod.escapefromtarkov.com/launcher/download");
            }

            StartInstallWatcher();
        }

        public async void StartInstallWatcher()
        {
            watcherToken = new CancellationTokenSource();
            await Task.Run(async () =>
            {
                while (true)
                {
                    if (watcherToken.IsCancellationRequested)
                    {
                        return;
                    }
                    if (File.Exists(BattlestateGamesChecks.InstallationPath))
                    {
                        InvokeOnInstalled(new GameInstalledEventArgs());
                        return;
                    }

                    await Task.Delay(10000);
                }
            });
        }
    }

    public class BattlestateGamesUninstallController : UninstallController
    {
        private CancellationTokenSource watcherToken;

        public BattlestateGamesUninstallController(Game game) : base(game)
        {
            Name = "Uninstall";
        }

        public override void Dispose()
        {
            watcherToken?.Cancel();
        }

        public override void Uninstall(UninstallActionArgs args)
        {
            Dispose();
            if (!File.Exists(BattlestateGamesChecks.ClientExecPath))
            // Launcher was removed but game persisted somehow.
            {
                throw new FileNotFoundException("BYOND has been uninstalled earlier.");
            }
            // Running generic Windows' uninstaller with one simple trick. . .
            ProcessStarter.StartProcess("appwiz.cpl");
            StartUninstallWatcher();
        }

        public async void StartUninstallWatcher()
        {
            watcherToken = new CancellationTokenSource();
            while (true)
            {
                if (watcherToken.IsCancellationRequested)
                {
                    return;
                }

                if (!File.Exists(BattlestateGamesChecks.InstallationPath))
                {
                    InvokeOnUninstalled(new GameUninstalledEventArgs());
                    return;
                }

                await Task.Delay(5000);
            }
        }
    }
}
