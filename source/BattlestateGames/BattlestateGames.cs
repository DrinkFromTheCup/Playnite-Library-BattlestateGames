using Playnite.SDK;
using Playnite.SDK.Data;
using Playnite.SDK.Models;
using Playnite.SDK.Plugins;
using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace BattlestateGames
{
    [LoadPlugin]
    public class BattlestateGames : LibraryPluginBase<BattlestateGamesSettingsViewModel>
    {
        // For Step 1, it is enough to keep this mess as is.
        // I'll probably expand on it later.
        bool BattlestateGamesInstalled
        {
            get
            {
                if (string.IsNullOrEmpty(BattlestateGamesChecks.InstallationPath) || !File.Exists(BattlestateGamesChecks.InstallationPath))
                {
                    return false;
                }
                else
                {
                    return true;
                }
            }
        }

        // Start of BattlestateGames plugin definitions
        public BattlestateGames(IPlayniteAPI api) : base(
            "Battlestate Games (Escape from Tarkov)",
            Guid.Parse("d0217e44-0df5-45f7-8515-478bdf21a883"),
            // No need in auto-close. No need in extra settings either.
            new LibraryPluginProperties { CanShutdownClient = false, HasSettings = false },
            new BattlestateGamesChecksClient(),
            BattlestateGamesChecks.Icon,
            (_) => new BattlestateGamesSettingsView(),
            api)
        {
            // No settings - no problem. Looks optional. I believe, API needs this empty entry anyway so keeping it as is.
        }

        public override IEnumerable<GameMetadata> GetGames(LibraryGetGamesArgs args)
        {
            return new List<GameMetadata>()
            {
                // Start of new game entry.
                new GameMetadata()
                {
                    Name = "Escape from Tarkov",
                    // Keeping it generic again (NB: it's 2nd plugin, so - again).
                    GameId = "escapefromtarkov",
                    GameActions = new List<GameAction>
                    {
                        new GameAction()
                        {
                            Name = "Play",
                            Type = GameActionType.File,
                            Path = BattlestateGamesChecks.InstallationPath,
                            IsPlayAction = true
                        }
                    },
                    IsInstalled = BattlestateGamesInstalled,
                    // Launcher name to launcher, game name to game... mixed name to mix.
                    Source = new MetadataNameProperty("Battlestate Games"),
                    Links = new List<Link>()
                    {
                        new Link("Store", @"https://www.escapefromtarkov.com/preorder-page")
                    },
                    Platforms = new HashSet<MetadataProperty> { new MetadataSpecProperty("pc_windows") }
                }
                // End of new game entry. Do note that last entry in the list should not have comma as last symbol.
            };
        }


        // Start of blatant install/uninstall links adding.
        // I'd really like to utilize something much more simple since we have only one entry point anyway,
        // in a form of stand-alone launcher, but we're having what we're having for now.
        public override IEnumerable<InstallController> GetInstallActions(GetInstallActionsArgs args)
        {
            if (args.Game.PluginId != Id)
            {
                yield break;
            }

            yield return new BattlestateGamesInstallController(args.Game);
        }

        public override IEnumerable<UninstallController> GetUninstallActions(GetUninstallActionsArgs args)
        {
            if (args.Game.PluginId != Id)
            {
                yield break;
            }

            yield return new BattlestateGamesUninstallController(args.Game);
        }
        // End of blatant install/uninstall links adding.

        public override UserControl GetSettingsView(bool firstRunSettings)
        {
            return new BattlestateGamesSettingsView();
        }
    }
}