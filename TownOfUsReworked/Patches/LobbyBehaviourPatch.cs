using HarmonyLib;
using TownOfUsReworked.PlayerLayers;
using TownOfUsReworked.MultiClientInstancing;
using TownOfUsReworked.Data;
using TownOfUsReworked.Classes;

namespace TownOfUsReworked.Patches
{
    [HarmonyPatch(typeof(LobbyBehaviour), nameof(LobbyBehaviour.Start))]
    public static class LobbyBehaviourPatch
    {
        public static void Postfix()
        {
            //Fix Grenadier and screwed blind in lobby
            HudManager.Instance.FullScreen.gameObject.active = false;
            RoleGen.ResetEverything();
            PlayerLayer.DeleteAll();

            if (!ConstantVariables.IsLocalGame)
                return;

            if (MCIUtils.Clients.Count != 0 && TownOfUsReworked.MCIActive && ConstantVariables.IsLocalGame)
            {
                var count = MCIUtils.Clients.Count;
                TownOfUsReworked.Debugger.TestWindow.Enabled = true;
                MCIUtils.Clients.Clear();
                MCIUtils.PlayerIdClientId.Clear();

                if (TownOfUsReworked.Persistence)
                {
                    for (var i = 0; i < count; i++)
                        MCIUtils.CreatePlayerInstance();
                }
            }
        }
    }
}