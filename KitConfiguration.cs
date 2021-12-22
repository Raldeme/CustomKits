using Rocket.API;

namespace Raldeme.Kits
{
    public class KitConfiguration : IRocketPluginConfiguration
    {
        public bool KitsEnabled;
        public bool KitsSaveEntireInventory;
        public bool DeleteInventoryItemsOnSave;
        public bool DeleteDatabaseKitOnOpen;
        public int TotalAllowedKits;
        public bool ShareKitsAcrossServers;

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
            KitsEnabled = true;
            KitsSaveEntireInventory = true;
            DeleteInventoryItemsOnSave = true;
            DeleteDatabaseKitOnOpen = true;
            TotalAllowedKits = 3;
            ShareKitsAcrossServers = false;

            // Debug Mode
            Debug = false;
            
            // Database Settings
            DatabaseHost = "localhost";
            DatabaseUser = "unturned";
            DatabasePass = "password";
            DatabaseName = "unturned";
            DatabasePort = 3306;
            DatabaseTable = "Kits";
        }
    }
}
