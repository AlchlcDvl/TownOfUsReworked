using HarmonyLib;
using TownOfUsReworked.PlayerLayers;
using TownOfUsReworked.MultiClientInstancing;
using TownOfUsReworked.Data;
using TownOfUsReworked.Classes;

namespace TownOfUsReworked.Patches
{
    [HarmonyPatch(typeof(LobbyBehaviour), nameof(LobbyBehaviour.Start))]
    static class LobbyBehaviourPatch
    {
        public static void Postfix()
        {
            //Fix Grenadier and screwed blind in lobby
            HudManager.Instance.FullScreen.gameObject.active = false;

            RoleGen.ResetEverything();
            PlayerLayer.DeleteAll();

            if (!ConstantVariables.IsLocalGame)
                return;

            if (InstanceControl.Clients.Count != 0 && TownOfUsReworked.MCIActive && ConstantVariables.IsLocalGame)
            {
                int count = InstanceControl.Clients.Count;
                InstanceControl.Clients.Clear();
                InstanceControl.PlayerIdClientId.Clear();

                if (TownOfUsReworked.Persistence)
                {
                    for (var i = 0; i < count; i++)
                        MCIUtils.CreatePlayerInstance();
                }
            }
        }
    }
}