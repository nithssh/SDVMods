using StardewModdingAPI;
using StardewModdingAPI.Events;

namespace Dem1se.CustomReminders.Multiplayer
{

    class Multiplayer
    {
        /// <summary>The unique ID of this mod.</summary>
        private readonly string ModID;
        private readonly IModHelper Helper;
        private readonly IMonitor Monitor;

        public Multiplayer(IModHelper Helper, IMonitor monitor)
        {
            this.Helper = Helper;
            this.Monitor = monitor;
            this.ModID = this.Helper.ModRegistry.ModID;
        }

        /// <summary>
        /// Used by the peers to recieve and set the value for Utilities.Utilities.SaveFolderName field
        /// </summary>
        /// <param name="sender">Sender of the event</param>
        /// <param name="e">Contains all the data/args of the event</param>
        public void OnModMessageReceived(object sender, ModMessageReceivedEventArgs e)
        {
            if (e.FromModID == this.ModID && e.Type == "SaveFolderName")
            {
                SaveFolderNameModel message = e.ReadAs<SaveFolderNameModel>();
                Utilities.Utilities.SaveFolderName = message.SaveFolderName;
            }
        }

        /// <summary>
        /// Send message method.
        /// The method to call when a peer connects. Host will send the SaveFolderName using this.
        /// </summary>
        /// <param name="sender">The sender of the event</param>
        /// <param name="e">Contains all the arguments of PeerConnectedEvent</param>
        public void OnPeerConnected(object sender, PeerContextReceivedEventArgs e)
        {
            if (Context.IsMainPlayer)
            {
                Helper.Multiplayer.SendMessage<SaveFolderNameModel>(new SaveFolderNameModel() { SaveFolderName = Utilities.Utilities.SaveFolderName }, "SaveFolderName", modIDs: new[] { this.ModID }, playerIDs: new[] { e.Peer.PlayerID });
            }
        }
    }

    /// <summary>Data model for sending and receiving data in Multiplayer messages.</summary>
    class SaveFolderNameModel
    {
        public string SaveFolderName { get; set; }
    }
}
