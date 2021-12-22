using Logger = Rocket.Core.Logging.Logger;
using Rocket.Core.Plugins;
using System;
using Rocket.API.Collections;

namespace Raldeme.Kits
{
    public class Kit : RocketPlugin<KitConfiguration>
    {
        public static Kit Instance;
        public DatabaseManager Database;

        protected override void Load()
        {
            Instance = this;
            Database = new DatabaseManager();
            Logger.Log("Kits have been successfully loaded!", ConsoleColor.Yellow);
        }

        protected override void Unload()
        {
            Logger.Log("Kits have been successfully unloaded!", ConsoleColor.Yellow);
        }

        public void FixedUpdate()
        {
        }

        public override TranslationList DefaultTranslations
        {
            get
            {
                return new TranslationList() {
                    {"Kit_disabled", "Whoops! Sorry, but Kits are currently disabled. ='("},
                    {"Kit_invalid_item", "You do not have that item! You can only Kit items in your inventory."},
                    {"Kit_action_invalid", "Invalid action! Type \"/Kit help\" for more information."},
                    {"Kit_params_invalid", "Invalid parameters! Type \"/Kit help\" for more information."},
                    {"Kit_opened", "You open a Kit and receive the contents inside!"},
                    {"Kit_opened_error", "There was an error opening your Kit!"},
                    {"Kit_saved", "You have saved an item to your Kit!"},
                    {"Kit_saved_inventory", "You saved all items to your Kit!"},
                    {"Kit_saved_error", "There was an error saving your Kit!"},
                    {"Kit_saved_noitems", "You don't have any items to save!"},
                    {"Kit_full", "All of your Kits are full! You must first clear your Kit by typing: /Kit delete"},
                    {"Kit_empty", "No Kit exists for you to open! You must save some items first."},
                    {"Kit_delete_empty", "You have no Kits to delete! Try saving one first: /Kit save"},
                    {"Kit_deleted", "You have deleted a Kit!"}
                };
            }
        }

    }

}
