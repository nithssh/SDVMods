using StardewModdingAPI;
using StardewModdingAPI.Events;

namespace Dem1se.CustomReminders.Multiplayer
{

    class Multiplayer
    {
        private IModHelper Helper;
        
        /// <summary>The unique ID of this mod.</summary>
        private string ModID;

        public Multiplayer(IModHelper Helper)
        {
            this.Helper = Helper;
            this.ModID = this.Helper.ModRegistry.ModID;
        }

        public void SendMessage(string SaveFolderName)
        {
            SaveFolderNameModel message = new SaveFolderNameModel();
            message.SaveFolderName = SaveFolderName;
            this.Helper.Multiplayer.SendMessage<SaveFolderNameModel>(message, "SaveFolderNameModel", modIDs: new[] { this.ModID });
        }

        public void OnModMessageReceived(object sender, ModMessageReceivedEventArgs e)
        {
            if (e.FromModID == this.ModID && e.Type == "SaveFolderNameModel")
            {
                SaveFolderNameModel message = e.ReadAs<SaveFolderNameModel>();
                // handle message fields here
            }
        }
    }

    class SaveFolderNameModel
    {
        public string SaveFolderName { get; set; }
    }
}
