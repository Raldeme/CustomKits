using System;
using Rocket.API;
using Rocket.Unturned.Chat;
using Rocket.Unturned.Player;
using System.Collections.Generic;
using UnityEngine;

namespace Raldeme.Kits
{
    public class CommandKit : IRocketCommand
    {
        public AllowedCaller AllowedCaller
        {
            get { return AllowedCaller.Player; }
        }
        public bool AllowFromConsole
        {
            get { return false; }
        }

        public string Name
        {
            get { return "Kit"; }
        }

        public string Help
        {
            get { return "Save and load items from your inventory to your own personal Kit."; }
        }

        public List<string> Aliases
        {
            get { return new List<string>() { }; }
        }

        public string Syntax
        {
            get { return "/Kit <save|load|delete|help> <itemId>"; }
        }

        public List<string> Permissions
        {
            get
            {
                return new List<string>() { "raldeme.kit" };
            }
        }

        public void Execute(IRocketPlayer caller, params string[] param)
        {
            UnturnedPlayer player = (UnturnedPlayer)caller;

            if (Kit.Instance.Configuration.Instance.KitsEnabled)
            {
                if (param.Length > 0)
                {
                    if (param.Length == 1 && Kit.Instance.Configuration.Instance.KitsSaveEntireInventory)
                    {
                        switch (param[0])
                        {
                            case "save":
                                // save player Kit to database
                                Kit.Instance.Database.SavePlayerInventory(player);
                                break;
                            case "open":
                            case "load":
                                // open player Kit from database
                                Kit.Instance.Database.OpenPlayerInventory(player);
                                break;
                            case "delete":
                                // open player Kit from database
                                Kit.Instance.Database.DeletePlayerKit(player);
                                break;
                            case "help":
                                UnturnedChat.Say(caller, Help, Color.white);
                                UnturnedChat.Say(caller, Syntax, Color.white);
                                break;
                            default:
                                // invalid action
                                UnturnedChat.Say(caller, Kit.Instance.Translations.Instance.Translate("Kit_action_invalid"), Color.red);
                                break;
                        }
                    }
                    else if (param.Length == 2 && !Kit.Instance.Configuration.Instance.KitsSaveEntireInventory)
                    {
                        ushort itemId;
                        if (ushort.TryParse(param[1], out itemId))
                        {
                            switch (param[0])
                            {
                                case "save":
                                    // save player Kit to database
                                    Kit.Instance.Database.SavePlayerInventory(player, itemId);
                                    break;
                                case "open":
                                case "load":
                                    // open player Kit from database
                                    Kit.Instance.Database.OpenPlayerInventory(player, itemId);
                                    break;
                                case "help":
                                    UnturnedChat.Say(caller, Help, Color.white);
                                    UnturnedChat.Say(caller, Syntax, Color.white);
                                    break;
                                default:
                                    // invalid action
                                    UnturnedChat.Say(caller, Kit.Instance.Translations.Instance.Translate("Kit_action_invalid"), Color.red);
                                    break;
                            }
                        }
                    }
                    else
                    {
                        UnturnedChat.Say(caller, "Incorrect Syntax! Use:" + Syntax, Color.white);
                    }
                }
                else
                {
                    // user typed /Kit only
                    UnturnedChat.Say(caller, Help, Color.white);
                }
            }
            else
            {
                // Kit disabled in configuration
                UnturnedChat.Say(caller, Kit.Instance.Translations.Instance.Translate("Kit_disabled"), Color.yellow);
            }
        }
    }
}
