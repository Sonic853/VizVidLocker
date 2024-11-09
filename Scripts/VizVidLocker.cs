
using JLChnToZ.VRC.VVMW;
using TMPro;
using UdonSharp;
using UnityEngine;
using UnityEngine.UI;
using VRC.SDKBase;
using VRC.Udon;

namespace Sonic853.VizVid.Locker {
    public class VizVidLocker : SyncBehaviour {
        [SerializeField] Toggle toggle;
        [SerializeField] TMP_Text text;
        [UdonSynced] public string ownerName = string.Empty;
        [SerializeField] FrontendHandler frontendHandler;
        VRCPlayerApi LocalPlayer;
        protected override void Start() {
            base.Start();
            LocalPlayer = Networking.LocalPlayer;
        }
        public void ButtonClick() {
            if (!isSynced) { return; }
            if (
                !string.IsNullOrEmpty(ownerName)
                && LocalPlayer.displayName != ownerName
            ) {
                if (HaveOwner()) { return; }
            }
            if (!Networking.IsOwner(gameObject)) {
                Networking.SetOwner(LocalPlayer, gameObject);
            }
            if (!string.IsNullOrEmpty(ownerName)) {
                ownerName = string.Empty;
            } else {
                ownerName = LocalPlayer.displayName;
            }
            NeedSync();
        }
        public override void OnDeserialization() {
            base.OnDeserialization();
            if (string.IsNullOrEmpty(ownerName)) {
                toggle.SetIsOnWithoutNotify(false);
                text.text = "Unlocked.";
                frontendHandler._OnUnlock();
                return;
            }
            if (LocalPlayer.displayName == ownerName) {
                toggle.SetIsOnWithoutNotify(true);
                text.text = $"You are Owner: {ownerName}";
                frontendHandler._OnUnlock();
            } else if (!HaveOwner()) {
                toggle.SetIsOnWithoutNotify(false);
                text.text = "Owner Not Found, Unlocking...";
                ownerName = string.Empty;
                frontendHandler._OnUnlock();
            } else {
                toggle.SetIsOnWithoutNotify(true);
                text.text = $"Owner is: {ownerName}";
                frontendHandler._Lock();
            }

        }
        public bool HaveOwner() {
            var players = GetAllPlayers();
            foreach (var player in players) {
                if (player.displayName == ownerName) {
                    return true;
                }
            }
            return false;
        }
        public static VRCPlayerApi[] GetAllPlayers() {
            VRCPlayerApi[] players = new VRCPlayerApi[VRCPlayerApi.GetPlayerCount()];
            players = VRCPlayerApi.GetPlayers(players);
            return players;
        }
    }
}
