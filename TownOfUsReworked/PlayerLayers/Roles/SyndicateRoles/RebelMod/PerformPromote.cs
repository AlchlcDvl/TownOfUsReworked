using HarmonyLib;
using TownOfUsReworked.Classes;
using Hazel;
using TownOfUsReworked.CustomOptions;
using System;
using TownOfUsReworked.Data;
using TownOfUsReworked.Extensions;

namespace TownOfUsReworked.PlayerLayers.Roles.SyndicateRoles.RebelMod
{
    [HarmonyPatch(typeof(AbilityButton), nameof(AbilityButton.DoClick))]
    public static class PerformPromote
    {
        public static bool Prefix(AbilityButton __instance)
        {
            if (Utils.NoButton(PlayerControl.LocalPlayer, RoleEnum.Rebel))
                return true;

            var role = Role.GetRole<Rebel>(PlayerControl.LocalPlayer);

            if (__instance == role.DeclareButton && !role.HasDeclared)
            {
                if (Utils.IsTooFar(role.Player, role.ClosestPlayer))
                    return false;

                var interact = Utils.Interact(role.Player, role.ClosestPlayer);

                if (interact[3])
                {
                    var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.Action, SendOption.Reliable);
                    writer.Write((byte)ActionsRPC.Sidekick);
                    writer.Write(role.Player.PlayerId);
                    writer.Write(role.ClosestPlayer.PlayerId);
                    AmongUsClient.Instance.FinishRpcImmediately(writer);
                    Sidekick(role, role.ClosestPlayer);
                }
                else if (interact[0])
                    role.LastDeclared = DateTime.UtcNow;
                else if (interact[1])
                    role.LastDeclared.AddSeconds(CustomGameOptions.ProtectKCReset);

                return false;
            }

            return true;
        }

        public static void Sidekick(Rebel reb, PlayerControl target)
        {
            reb.HasDeclared = true;
            var formerRole = Role.GetRole(target);

            var sidekick = new Sidekick(target)
            {
                FormerRole = formerRole,
                Rebel = reb
            };

            reb.Sidekick = sidekick;
            sidekick.RoleUpdate(formerRole);

            if (target == PlayerControl.LocalPlayer)
                Utils.Flash(Colors.Rebel, "You have been promoted to <color=#979C9FFF>Sidekick</color>!");

            if (PlayerControl.LocalPlayer.Is(RoleEnum.Seer))
                Utils.Flash(Colors.Seer, "Someone has changed their identity!");
        }
    }
}