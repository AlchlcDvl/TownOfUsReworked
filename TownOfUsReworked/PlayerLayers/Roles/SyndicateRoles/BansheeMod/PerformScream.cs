using HarmonyLib;
using Hazel;
using TownOfUsReworked.Enums;
using TownOfUsReworked.CustomOptions;
using TownOfUsReworked.Classes;
using TownOfUsReworked.Extensions;

namespace TownOfUsReworked.PlayerLayers.Roles.SyndicateRoles.BansheeMod
{
    [HarmonyPatch(typeof(AbilityButton), nameof(AbilityButton.DoClick))]
    public static class PerformScream
    {
        public static bool Prefix(AbilityButton __instance)
        {
            if (Utils.NoButton(PlayerControl.LocalPlayer, RoleEnum.Banshee))
                return true;

            var role = Role.GetRole<Banshee>(PlayerControl.LocalPlayer);

            if (__instance == role.ScreamButton)
            {
                if (role.ScreamTimer() != 0f)
                    return false;

                var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.Action, SendOption.Reliable);
                writer.Write((byte)ActionsRPC.Scream);
                writer.Write(PlayerControl.LocalPlayer.PlayerId);
                AmongUsClient.Instance.FinishRpcImmediately(writer);
                role.TimeRemaining = CustomGameOptions.ScreamDuration;
                role.Scream();

                foreach (var player in PlayerControl.AllPlayerControls)
                {
                    if (!player.Data.IsDead && !player.Data.Disconnected && !player.Is(Faction.Syndicate))
                    {
                        var targetRole = Role.GetRole(player);
                        targetRole.IsBlocked = !targetRole.RoleBlockImmune;
                        role.Blocked.Add(player.PlayerId);
                    }
                }

                return false;
            }

            return true;
        }
    }
}