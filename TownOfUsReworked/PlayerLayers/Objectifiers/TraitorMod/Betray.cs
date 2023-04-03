using HarmonyLib;
using Hazel;
using TownOfUsReworked.Classes;
using TownOfUsReworked.Data;

namespace TownOfUsReworked.PlayerLayers.Objectifiers.TraitorMod
{
    [HarmonyPatch(typeof(HudManager), nameof(HudManager.Update))]
    public static class Betray
    {
        public static void Postfix()
        {
            if (Utils.NoButton(PlayerControl.LocalPlayer, ObjectifierEnum.Traitor))
                return;

            var traitor = Objectifier.GetObjectifier<Traitor>(PlayerControl.LocalPlayer);

            if (!traitor.Turned)
                return;

            if (traitor.Betray)
            {
                traitor.TurnBetrayer();
                var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.Change, SendOption.Reliable);
                writer.Write((byte)TurnRPC.TurnTraitorBetrayer);
                writer.Write(PlayerControl.LocalPlayer.PlayerId);
                AmongUsClient.Instance.FinishRpcImmediately(writer);
            }
        }
    }
}