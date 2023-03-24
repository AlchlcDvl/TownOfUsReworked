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
            if (InstanceControl.Clients.Count != 0 && TownOfUsReworked.MCIActive && GameStates.IsLocalGame)
            {
                int count = InstanceControl.Clients.Count;
                InstanceControl.Clients.Clear();
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