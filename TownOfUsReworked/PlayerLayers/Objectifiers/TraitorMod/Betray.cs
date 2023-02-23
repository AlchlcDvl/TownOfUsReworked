using HarmonyLib;
using Hazel;
using TownOfUsReworked.PlayerLayers.Roles;
using TownOfUsReworked.Enums;
using TownOfUsReworked.Classes;
using System.Linq;

namespace TownOfUsReworked.PlayerLayers.Objectifiers.TraitorMod
{
    [HarmonyPatch(typeof(HudManager), nameof(HudManager.Update))]
    public class Betray
    {
        public static void Postfix(HudManager __instance)
        {
            if (Utils.NoButton(PlayerControl.LocalPlayer, ObjectifierEnum.Traitor))
                return;

            var traitor = Objectifier.GetObjectifier<Traitor>(PlayerControl.LocalPlayer);

            if (!traitor.Turned)
                return;

            var traitorRole = Role.GetRole(PlayerControl.LocalPlayer);
            var factiondead = PlayerControl.AllPlayerControls.ToArray().Where(x => x.GetFaction() == traitorRole.Faction).Count() == 0;

            if (factiondead)
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