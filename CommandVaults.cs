using Rocket.API;
using Rocket.Unturned.Chat;
using Rocket.Unturned.Player;
using System.Collections.Generic;
using UnityEngine;

namespace Raldeme.CustomKits
{
    public class CommandCustomKits : IRocketCommand
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
            get { return "CustomKits"; }
        }

        public string Help
        {
            get { return "View your current Kit items."; }
        }

        public List<string> Aliases
        {
            get { return new List<string>() { }; }
        }

        public string Syntax
        {
            get { return "/CustomKits <help>"; }
        }

        public List<string> Permissions
        {
            get
            {
                return new List<string>() { "Raldeme.CustomKits" };
            }
        }

        public void Execute(IRocketPlayer caller, params string[] param)
        {
            UnturnedPlayer player = (UnturnedPlayer)caller;

            if (Kit.Instance.Configuration.Instance.CustomKitsEnabled)
            {
                if (param.Length > 0)
                {
                    switch (param[0])
                    {
                        case "help":
                            UnturnedChat.Say(caller, Help, Color.white);
                            UnturnedChat.Say(caller, Syntax, Color.white);
                            break;
                        default:
                            UnturnedChat.Say(caller, Kit.Instance.Translations.Instance.Translate("CustomKits_invalid_action"), Color.red);
                            break;
                    }
                }
                else
                {
                    // list CustomKits
                    Kit.Instance.Database.ListCustomKits(player);
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