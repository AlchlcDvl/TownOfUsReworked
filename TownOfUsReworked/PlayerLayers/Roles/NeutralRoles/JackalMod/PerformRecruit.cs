using System;
using HarmonyLib;
using Hazel;
using TownOfUsReworked.CustomOptions;
using TownOfUsReworked.Classes;
using TownOfUsReworked.Extensions;
using TownOfUsReworked.Data;

namespace TownOfUsReworked.PlayerLayers.Roles.NeutralRoles.JackalMod
{
    [HarmonyPatch(typeof(AbilityButton), nameof(AbilityButton.DoClick))]
    public static class PerformRecruit
    {
        public static bool Prefix(AbilityButton __instance)
        {
            if (Utils.NoButton(PlayerControl.LocalPlayer, RoleEnum.Jackal))
                return true;

            var role = Role.GetRole<Jackal>(PlayerControl.LocalPlayer);

            if (Utils.IsTooFar(role.Player, role.ClosestPlayer))
                return false;

            if (__instance == role.RecruitButton)
            {
                if (role.RecruitTimer() != 0f)
                    return false;

                var interact = Utils.Interact(role.Player, role.ClosestPlayer, !role.ClosestPlayer.Is(SubFaction.None), role.ClosestPlayer.Is(SubFaction.None));

                if (interact[3])
                {
                    RoleGen.Convert(role.ClosestPlayer.PlayerId, role.Player.PlayerId, SubFaction.Cabal, false);
                    var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.Action, SendOption.Reliable);
                    writer.Write((byte)ActionsRPC.Convert);
                    writer.Write(PlayerControl.LocalPlayer.PlayerId);
                    writer.Write(role.ClosestPlayer.PlayerId);
                    writer.Write((byte)SubFaction.Cabal);
                    writer.Write(false);
                    AmongUsClient.Instance.FinishRpcImmediately(writer);
                    return false;
                }
                else if (interact[0])
                    role.LastRecruited = DateTime.UtcNow;
                else if (interact[1])
                    role.LastRecruited.AddSeconds(CustomGameOptions.ProtectKCReset);

                return false;
            }

            return true;
        }
    }
}