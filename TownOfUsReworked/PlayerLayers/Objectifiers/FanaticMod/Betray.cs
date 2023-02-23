using HarmonyLib;
using Hazel;
using TownOfUsReworked.PlayerLayers.Roles;
using TownOfUsReworked.Enums;
using TownOfUsReworked.Classes;
using System.Linq;

namespace TownOfUsReworked.PlayerLayers.Objectifiers.FanaticMod
{
    [HarmonyPatch(typeof(HudManager), nameof(HudManager.Update))]
    public class Betray
    {
        public static void Postfix(HudManager __instance)
        {
            if (Utils.NoButton(PlayerControl.LocalPlayer, ObjectifierEnum.Fanatic))
                return;

            var fanatic = Objectifier.GetObjectifier<Fanatic>(PlayerControl.LocalPlayer);

            if (!fanatic.Turned)
                return;

            var fanaticRole = Role.GetRole(PlayerControl.LocalPlayer);
            var factiondead = PlayerControl.AllPlayerControls.ToArray().Where(x => x.GetFaction() == fanaticRole.Faction).Count() == 0;

            if (factiondead)
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