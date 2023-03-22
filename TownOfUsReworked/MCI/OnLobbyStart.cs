using HarmonyLib;
using TownOfUsReworked.Classes;

namespace TownOfUsReworked.MCI
{
    [HarmonyPatch]
    public sealed class OnLobbyStart
    {
        [HarmonyPatch(typeof(LobbyBehaviour), nameof(LobbyBehaviour.Start))]
        [HarmonyPostfix]
        public static void Postfix()
        {
            if (InstanceControl.clients.Count != 0 && InstanceControl.MCIActive && GameStates.IsLocalGame)
            {
                int count = InstanceControl.clients.Count;
                InstanceControl.clients.Clear();
                InstanceControl.PlayerIdClientId.Clear();

                if (TownOfUsReworked.Persistence)
                {
                    for (int i = 0; i < count; i++)
                        MCIUtils.CreatePlayerInstance();
                }
            }
        }
    }
}