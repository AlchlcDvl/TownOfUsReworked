using HarmonyLib;
using Hazel;
using TownOfUsReworked.Enums;
using TownOfUsReworked.Classes;

namespace TownOfUsReworked.PlayerLayers.Objectifiers.FanaticMod
{
    [HarmonyPatch(typeof(HudManager), nameof(HudManager.Update))]
    public static class Betray
    {
        public static void Postfix()
        {
            if (Utils.NoButton(PlayerControl.LocalPlayer, ObjectifierEnum.Fanatic))
                return;

            var fanatic = Objectifier.GetObjectifier<Fanatic>(PlayerControl.LocalPlayer);

            if (!fanatic.Turned)
                return;

            if (fanatic.Betray)
            {
                fanatic.TurnBetrayer();
                var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.Change, SendOption.Reliable);
                writer.Write((byte)TurnRPC.TurnFanaticBetrayer);
                writer.Write(PlayerControl.LocalPlayer.PlayerId);
                AmongUsClient.Instance.FinishRpcImmediately(writer);
            }
        }
    }
}