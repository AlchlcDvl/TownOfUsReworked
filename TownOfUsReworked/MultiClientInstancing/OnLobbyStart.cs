using HarmonyLib;
using TownOfUsReworked.Data;

namespace TownOfUsReworked.MultiClientInstancing
{
    [HarmonyPatch(typeof(LobbyBehaviour), nameof(LobbyBehaviour.Start))]
    public sealed class OnLobbyStart
    {
        public static void Postfix()
        {
            if (InstanceControl.Clients.Count != 0 && TownOfUsReworked.MCIActive && ConstantVariables.IsLocalGame)
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