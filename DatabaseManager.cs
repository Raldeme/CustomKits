using Logger = Rocket.Core.Logging.Logger;
using Rocket.Unturned.Player;
using Rocket.Unturned.Chat;
using SDG.Unturned;
using MySql.Data.MySqlClient;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace Raldeme.Kits
{
    public class DatabaseManager
    {
        public DatabaseManager()
        {
            new I18N.West.CP1250();
            CheckSchema();
        }

        public void CheckSchema()
        {
            try
            {
                MySqlConnection MySQLConnection = CreateConnection();
                MySqlCommand MySQLCommand = MySQLConnection.CreateCommand();

                MySQLCommand.CommandText = "SHOW TABLES LIKE '" + Kit.Instance.Configuration.Instance.DatabaseTable + "'";
                MySQLConnection.Open();

                object result = MySQLCommand.ExecuteScalar();

                if (result == null)
                {
                    MySQLCommand.CommandText = "CREATE TABLE " + Kit.Instance.Configuration.Instance.DatabaseTable +
                    "(id INT(8) NOT NULL AUTO_INCREMENT," +
                    "steam_id VARCHAR(50) NOT NULL," +
                    "inventory TEXT NULL," +
                    "server_id VARCHAR(255) NOT NULL," +
                    "timestamp TIMESTAMP DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP," +
                    "PRIMARY KEY(id));";

                    MySQLCommand.ExecuteNonQuery();
                }
                MySQLConnection.Close();
            }
            catch (Exception ex)
            {
                Logger.LogException(ex);
            }
        }

        private MySqlConnection CreateConnection()
        {
            MySqlConnection MySQLConnection = null;

            try
            {
                if (Kit.Instance.Configuration.Instance.DatabasePort == 0)
                {
                    Kit.Instance.Configuration.Instance.DatabasePort = 3306;
                }

                MySQLConnection = new MySqlConnection(string.Format("SERVER={0};DATABASE={1};UID={2};PASSWORD={3};PORT={4};", new object[] {
                    Kit.Instance.Configuration.Instance.DatabaseHost,
                    Kit.Instance.Configuration.Instance.DatabaseName,
                    Kit.Instance.Configuration.Instance.DatabaseUser,
                    Kit.Instance.Configuration.Instance.DatabasePass,
                    Kit.Instance.Configuration.Instance.DatabasePort
                }));
            }
            catch (Exception ex)
            {
                Logger.LogException(ex);
            }
            return MySQLConnection;
        }

        /**
         * LIST PLAYER Kits
         * 
         * This function lists all player Kits by returning a chat message
         * @param UnturnedPlayer player Player data
         */
        public void ListKits(UnturnedPlayer player)
        {
            try
            {
                MySqlConnection MySQLConnection = CreateConnection();
                MySqlCommand MySQLCommand = MySQLConnection.CreateCommand();
                MySQLConnection.Open();

                string server_id_sel = "";

                // check if player Kit already exists
                if (!Kit.Instance.Configuration.Instance.ShareKitsAcrossServers)
                {
                    server_id_sel = " AND server_id = '" + Provider.serverID + "'";
                }

                MySQLCommand.CommandText = "SELECT COUNT(*) FROM " + Kit.Instance.Configuration.Instance.DatabaseTable + " WHERE steam_id = '" + player.CSteamID.ToString() + "'" + server_id_sel;
                int KitCount = Convert.ToInt32(MySQLCommand.ExecuteScalar());

                MySQLCommand.CommandText = "SELECT * FROM " + Kit.Instance.Configuration.Instance.DatabaseTable + " WHERE steam_id = '" + player.CSteamID.ToString() + "'" + server_id_sel;
                MySqlDataReader Kits = MySQLCommand.ExecuteReader();

                UnturnedChat.Say(player, "Kits Used: " + KitCount + " / " + Kit.Instance.Configuration.Instance.TotalAllowedKits, Color.white);

                while (Kits.Read())
                {
                    if (KitCount > 0)
                    {
                        UnturnedChat.Say(player, "Kit: " + Kits["inventory"], Color.white);                        
                    }
                    else
                    {
                        UnturnedChat.Say(player, Kit.Instance.Translations.Instance.Translate("Kit_saved_noitems"), Color.white);
                    }
                }
                Kits.Close();
                MySQLConnection.Close();
            }
            catch (Exception ex)
            {
                Logger.LogException(ex);
            }
        }

        /**
         * SAVE PLAYER INVENTORY ITEM(S)
         * 
         * This function saves a player's inventory or individual item (depending on configuration settings)
         * to the server database that is configured
         * @param UnturnedPlayer player Player data
         * @param ushort itemId Item ID to save to the database; if equals 0 save entire inventory
         */
        public void SavePlayerInventory(UnturnedPlayer player, ushort itemId = 0)
        {
            try
            {
                MySqlConnection MySQLConnection = CreateConnection();
                MySqlCommand MySQLCommand = MySQLConnection.CreateCommand();
                MySQLConnection.Open();

                if (Kit.Instance.Configuration.Instance.KitsSaveEntireInventory)
                {
                    string server_id_sel = "";

                    // check if player Kit already exists
                    if (!Kit.Instance.Configuration.Instance.ShareKitsAcrossServers)
                    {
                        server_id_sel = " AND server_id = '" + Provider.serverID + "'";
                    }
 
                    MySQLCommand.CommandText = "SELECT * FROM " + Kit.Instance.Configuration.Instance.DatabaseTable + " WHERE steam_id = '" + player.CSteamID.ToString() + "'" + server_id_sel;
                    object KitExists = MySQLCommand.ExecuteScalar();

                    // check if Kit already exists
                    if (KitExists == null)
                    {
                        List<string> InventoryItemsFound = new List<string>();
                        string InventoryDatabaseString = "";

                        // save and remove items
                        try
                        {
                            // get clothing
                            InventoryItemsFound.Add(player.Player.clothing.shirt.ToString());
                            InventoryItemsFound.Add(player.Player.clothing.pants.ToString());
                            InventoryItemsFound.Add(player.Player.clothing.mask.ToString());
                            InventoryItemsFound.Add(player.Player.clothing.hat.ToString());
                            InventoryItemsFound.Add(player.Player.clothing.glasses.ToString());
                            InventoryItemsFound.Add(player.Player.clothing.vest.ToString());
                            InventoryItemsFound.Add(player.Player.clothing.backpack.ToString());

                            // get items
                            foreach (var i in player.Inventory.items)
                            {
                                if (i == null) continue;
                                for (byte w = 0; w < i.width; w++)
                                {
                                    for (byte h = 0; h < i.height; h++)
                                    {
                                        try
                                        {
                                            byte index = i.getIndex(w, h);
                                            if (index == 255) continue;
                                            // add item found to list
                                            ItemJar invItem = player.Inventory.getItem(i.page, index);
                                            InventoryItemsFound.Add(invItem.item.id.ToString());
                                        }
                                        catch { }
                                    }
                                }
                            }

                            // create mysql string from array items separated by commas
                            InventoryDatabaseString = string.Join(",", InventoryItemsFound.ToArray());

                            if (InventoryItemsFound.Capacity > 0)
                            {
                                // add Kit to database
                                MySQLCommand.CommandText = "INSERT INTO " + Kit.Instance.Configuration.Instance.DatabaseTable + " (steam_id,inventory,server_id) VALUES ('" + player.CSteamID.ToString() + "','" + InventoryDatabaseString + "','" + (!Kit.Instance.Configuration.Instance.ShareKitsAcrossServers ? Provider.serverID : "") + "')";
                                MySQLCommand.ExecuteNonQuery();

                                // delete all player inventory items, in enabled in configuration
                                if (Kit.Instance.Configuration.Instance.DeleteInventoryItemsOnSave)
                                {
                                    ClearInventory(player);
                                }
                            }
                            else
                            {
                                // no items to save
                                UnturnedChat.Say(player, Kit.Instance.Translations.Instance.Translate("Kit_saved_noitems"), Color.red);
                                return;
                            }
                            
                        }
                        catch (Exception ex)
                        {
                            Logger.LogException(ex);
                        }

                        // debug
                        if (Kit.Instance.Configuration.Instance.Debug) { Logger.Log(player.CharacterName + " saved Kit items: " + InventoryDatabaseString, ConsoleColor.Yellow); }

                        // Kit saved successfully 
                        UnturnedChat.Say(player, Kit.Instance.Translations.Instance.Translate("Kit_saved_inventory"), Color.green);
                    }
                    else
                    {
                        // Kit already exists
                        UnturnedChat.Say(player, Kit.Instance.Translations.Instance.Translate("Kit_full"), Color.red);
                    }
                }
                else
                /**
                 * SAVE INDIVIDUAL ITEM */
                {
                    // check if item to Kit exists in player inventory
                    if (player.Inventory.has(itemId) != null)
                    {
                        string server_id_sel = "";

                        // check if player Kit already exists
                        if (!Kit.Instance.Configuration.Instance.ShareKitsAcrossServers)
                        {
                            server_id_sel = " AND server_id = '" + Provider.serverID + "'";
                        }

                        // check available Kits
                        MySQLCommand.CommandText = "SELECT COUNT(*) FROM " + Kit.Instance.Configuration.Instance.DatabaseTable + " WHERE steam_id = '" + player.CSteamID.ToString() + "'" + server_id_sel;
                        int KitCount = Convert.ToInt32(MySQLCommand.ExecuteScalar());

                        // check if player has used all available Kits
                        if (KitCount >= Kit.Instance.Configuration.Instance.TotalAllowedKits)
                        {
                            // all Kits are full
                            UnturnedChat.Say(player, Kit.Instance.Translations.Instance.Translate("Kit_full"), Color.red);
                        }
                        else
                        {
                            // Kit available; save the item 
                            MySQLCommand.CommandText = "INSERT INTO " + Kit.Instance.Configuration.Instance.DatabaseTable + " (steam_id,inventory,server_id) VALUES ('" + player.CSteamID.ToString() + "','" + itemId.ToString() + "','" + (!Kit.Instance.Configuration.Instance.ShareKitsAcrossServers ? Provider.serverID : "") + "')";
                            MySQLCommand.ExecuteNonQuery();

                            // remove item from player inventory, if enabled in configuration
                            if (Kit.Instance.Configuration.Instance.DeleteInventoryItemsOnSave)
                            {
                                RemoveInventoryItem(player, itemId);
                            }

                            // DEBUG
                            if (Kit.Instance.Configuration.Instance.Debug) { Logger.Log(player.CharacterName + " saved Kit item: " + itemId, ConsoleColor.Yellow); }

                            UnturnedChat.Say(player, Kit.Instance.Translations.Instance.Translate("Kit_saved"), Color.green);
                        }
                    }
                    else
                    {
                        // player does not have that item
                        UnturnedChat.Say(player, Kit.Instance.Translations.Instance.Translate("Kit_invalid_item"), Color.red);
                    }
                }
                MySQLConnection.Close();
            }
            catch (Exception ex)
            {
                Logger.LogException(ex);
            }
        }

        /**
         * OPEN PLAYER INVENTORY ITEM(S)
         * 
         * This function loads a player's inventory or individual item (depending on configuration settings)
         * from the server database that is configured
         * @param UnturnedPlayer player Player data
         * @param ushort itemId Item ID to load from database; if equals 0 load entire inventory
         */
        public void OpenPlayerInventory(UnturnedPlayer player, int Kit = 0)
        {
            try
            {
                MySqlConnection MySQLConnection = CreateConnection();
                MySqlCommand MySQLCommand = MySQLConnection.CreateCommand();
                MySQLConnection.Open();

                string server_id_sel = "";

                // check if player Kit already exists
                if (!Kit.Instance.Configuration.Instance.ShareKitsAcrossServers)
                {
                    server_id_sel = " AND server_id = '" + Provider.serverID + "'";
                }

                MySQLCommand.CommandText = "SELECT steam_id FROM " + Kit.Instance.Configuration.Instance.DatabaseTable + " WHERE steam_id = '" + player.CSteamID.ToString() + "'" + server_id_sel;
                object result = MySQLCommand.ExecuteScalar();                

                if (result != null)
                {
                    // query player Kit inventory items
                    MySQLCommand.CommandText = "SELECT inventory FROM " + Kit.Instance.Configuration.Instance.DatabaseTable + " WHERE steam_id = '" + player.CSteamID.ToString() + "'" + server_id_sel;
                    MySqlDataReader inventory = MySQLCommand.ExecuteReader();

                    if (inventory.Read())
                    {
                        string[] ItemIDs = inventory["inventory"].ToString().Split(',');

                        // add each item to player inventory
                        foreach (string id in ItemIDs)
                        {
                            Item item = new Item(ushort.Parse(id), true);
                            player.Inventory.forceAddItem(item,true);
                        }
                    }
                    inventory.Close();

                    // delete Kit from database, if enabled in configuration
                    if (Kit.Instance.Configuration.Instance.DeleteDatabaseKitOnOpen)
                    {
                        MySQLCommand.CommandText = "DELETE FROM " + Kit.Instance.Configuration.Instance.DatabaseTable + " WHERE steam_id = '" + player.CSteamID.ToString() + "'" + (!Kit.Instance.Configuration.Instance.ShareKitsAcrossServers ? " AND server_id = '" + Provider.serverID + "'" : "");
                        MySQLCommand.ExecuteNonQuery();
                    }

                    // DEBUG
                    if (Kit.Instance.Configuration.Instance.Debug) { Logger.Log(player.CharacterName + " opened Kit!", ConsoleColor.Yellow); }

                    // Kit opened successfully 
                    UnturnedChat.Say(player, Kit.Instance.Translations.Instance.Translate("Kit_opened"), Color.green);
                }
                else
                {
                    // no Kit exists
                    UnturnedChat.Say(player, Kit.Instance.Translations.Instance.Translate("Kit_empty"), Color.red);
                }
                MySQLConnection.Close();
            }
            catch (Exception ex)
            {
                Logger.LogException(ex);
            }
        }

        /**
         * DELETE PLAYER Kit
         * 
         * This function deletes a player Kit from the server database
         * @param UnturnedPlayer player Player data
         * @param ushort itemId Item ID optionally passed to target a specific Kit
         */
        public void DeletePlayerKit(UnturnedPlayer player, ushort itemId = 0)
        {
            try
            {
                MySqlConnection MySQLConnection = CreateConnection();
                MySqlCommand MySQLCommand = MySQLConnection.CreateCommand();
                MySQLConnection.Open();

                string itemId_delete = "";

                // check if player Kit already exists
                if (itemId > 0)
                {
                    itemId_delete = " AND inventory = '" + itemId.ToString() + "'";
                }

                MySQLCommand.CommandText = "SELECT steam_id FROM " + Kit.Instance.Configuration.Instance.DatabaseTable + " WHERE steam_id = '" + player.CSteamID.ToString() + "' AND server_id = '" + Provider.serverID + "'" + itemId_delete;
                object result = MySQLCommand.ExecuteScalar();

                if (result != null)
                {
                    // delete Kit from database
                    MySQLCommand.CommandText = "DELETE FROM " + Kit.Instance.Configuration.Instance.DatabaseTable + " WHERE steam_id = '" + player.CSteamID.ToString() + "' AND server_id = '" + Provider.serverID + "'" + itemId_delete;
                    MySQLCommand.ExecuteNonQuery();

                    // DEBUG
                    if (Kit.Instance.Configuration.Instance.Debug) { Logger.Log(player.CharacterName + " deleted Kit!", ConsoleColor.Yellow); }

                    // Kit deleted successfully 
                    UnturnedChat.Say(player, Kit.Instance.Translations.Instance.Translate("Kit_deleted"), Color.green);
                }
                else
                {
                    // no Kit exists
                    UnturnedChat.Say(player, Kit.Instance.Translations.Instance.Translate("Kit_delete_empty"), Color.red);
                }
                MySQLConnection.Close();
            }
            catch (Exception ex)
            {
                Logger.LogException(ex);
            }
        }

        /**
         *  REMOVE ITEM FROM PLAYER INVENTORY
         *  
         *  This function removes an item from a player inventory, for use when saving 
         *  items to the database.
         *  @param UnturnedPlayer player Player data
         *  @param ushort itemId Item ID
         */
        public void RemoveInventoryItem(UnturnedPlayer player, ushort itemId)
        {
            try
            {
                for (byte page = 0; page < 8; page++)
                {
                    var count = player.Inventory.getItemCount(page);

                    for (byte index = 0; index < count; index++)
                    {
                        if (player.Inventory.getItem(page, index).item.id == itemId)
                        {
                            player.Inventory.removeItem(page, index);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.LogException(ex);
            }
        }

        /**
         *  REMOVE ALL ITEMS FROM INVENTORY
         *  
         *  This function removes all items from a player inventory, for use when saving 
         *  items to the database.
         *  @param UnturnedPlayer player Player data
         */
        public void ClearInventory(UnturnedPlayer player)
        {            
            try
            {
                player.Player.equipment.dequip();

                foreach (var i in player.Inventory.items)
                {
                    if (i == null) continue;
                    for (byte w = 0; w < i.width; w++)
                    {
                        for (byte h = 0; h < i.height; h++)
                        {
                            try
                            {
                                byte index = i.getIndex(w, h);
                                if (index == 255) continue;
                                i.removeItem(index);
                            }
                            catch { }
                        }
                    }
                }
                                
                // glasses
                player.Player.clothing.askWearGlasses(0, 0, new byte[0], true);
                for (byte p2 = 0; p2 < player.Player.inventory.getItemCount(2); p2++)
                {
                    player.Player.inventory.removeItem(2, 0);
                }
                // hat
                player.Player.clothing.askWearHat(0, 0, new byte[0], true);
                for (byte p2 = 0; p2 < player.Player.inventory.getItemCount(2); p2++)
                {
                    player.Player.inventory.removeItem(2, 0);
                }
                // mask
                player.Player.clothing.askWearMask(0, 0, new byte[0], true);
                for (byte p2 = 0; p2 < player.Player.inventory.getItemCount(2); p2++)
                {
                    player.Player.inventory.removeItem(2, 0);
                }
                // pants
                player.Player.clothing.askWearPants(0, 0, new byte[0], true);
                for (byte p2 = 0; p2 < player.Player.inventory.getItemCount(2); p2++)
                {
                    player.Player.inventory.removeItem(2, 0);
                }
                // shirt
                player.Player.clothing.askWearShirt(0, 0, new byte[0], true);
                for (byte p2 = 0; p2 < player.Player.inventory.getItemCount(2); p2++)
                {
                    player.Player.inventory.removeItem(2, 0);
                }
                // vest
                player.Player.clothing.askWearVest(0, 0, new byte[0], true);
                for (byte p2 = 0; p2 < player.Player.inventory.getItemCount(2); p2++)
                {
                    player.Player.inventory.removeItem(2, 0);
                }
                // backpack
                player.Player.clothing.askWearBackpack(0, 0, new byte[0], true);
                for (byte p2 = 0; p2 < player.Player.inventory.getItemCount(2); p2++)
                {
                    player.Player.inventory.removeItem(2, 0);
                }
            }
            catch (Exception ex)
            {
                Logger.LogException(ex);
            }
        }
    }
}