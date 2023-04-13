using HarmonyLib;
using Hazel;
using TownOfUsReworked.CustomOptions;
using TownOfUsReworked.Classes;
using TownOfUsReworked.Data;
using TownOfUsReworked.Extensions;
using System.Linq;

namespace TownOfUsReworked.PlayerLayers.Roles.SyndicateRoles.ConcealerMod
{
    [HarmonyPatch(typeof(AbilityButton), nameof(AbilityButton.DoClick))]
    public static class PerformConceal
    {
        public static bool Prefix(AbilityButton __instance)
        {
            if (Utils.NoButton(PlayerControl.LocalPlayer, RoleEnum.Concealer))
                return true;

            var role = Role.GetRole<Concealer>(PlayerControl.LocalPlayer);

            if (__instance == role.ConcealButton)
            {
                if (role.ConcealTimer() != 0f)
                    return false;

                if (role.HoldsDrive)
                {
                    var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.Action, SendOption.Reliable);
                    writer.Write((byte)ActionsRPC.Conceal);
                    writer.Write(PlayerControl.LocalPlayer.PlayerId);
                    AmongUsClient.Instance.FinishRpcImmediately(writer);
                    role.TimeRemaining = CustomGameOptions.ConcealDuration;
                    role.Conceal();
                    Utils.Conceal();
                }
                else if (role.ConcealedPlayer == null)
                    role.ConcealMenu.Open(PlayerControl.AllPlayerControls.ToArray().Where(x => x != role.Player && x != role.ConcealedPlayer).ToList());
                else
                {
                    var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.Action, SendOption.Reliable);
                    writer.Write((byte)ActionsRPC.Conceal);
                    writer.Write(PlayerControl.LocalPlayer.PlayerId);
                    writer.Write(role.ConcealedPlayer.PlayerId);
                    AmongUsClient.Instance.FinishRpcImmediately(writer);
                    role.TimeRemaining = CustomGameOptions.ConcealDuration;
                    role.Conceal();
                    Utils.Invis(role.ConcealedPlayer, PlayerControl.LocalPlayer.Is(Faction.Syndicate));
                }

                return false;
            }

            return true;
        }
    }
}