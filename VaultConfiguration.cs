using Rocket.API;

namespace Raldeme.CustomKits
{
    public class KitConfiguration : IRocketPluginConfiguration
    {
        public bool CustomKitsEnabled;
        public bool CustomKitsSaveEntireInventory;
        public bool DeleteInventoryItemsOnSave;
        public bool DeleteDatabaseKitOnOpen;
        public int TotalAllowedCustomKits;
        public bool ShareCustomKitsAcrossServers;

        public bool Debug;

        public string DatabaseHost;
        public string DatabaseUser;
        public string DatabasePass;
        public string DatabaseName;
        public int DatabasePort;
        public string DatabaseTable;

        public void LoadDefaults()
        {
            // Configuration Settings
            CustomKitsEnabled = true;
            CustomKitsSaveEntireInventory = true;
            DeleteInventoryItemsOnSave = true;
            DeleteDatabaseKitOnOpen = true;
            TotalAllowedCustomKits = 3;
            ShareCustomKitsAcrossServers = false;

            // Debug Mode
            Debug = false;
            
            // Database Settings
            DatabaseHost = "localhost";
            DatabaseUser = "unturned";
            DatabasePass = "password";
            DatabaseName = "unturned";
            DatabasePort = 3306;
            DatabaseTable = "CustomKits";
        }
    }
}