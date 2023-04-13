using HarmonyLib;
using TownOfUsReworked.Classes;
using Hazel;
using System;
using TownOfUsReworked.CustomOptions;
using TownOfUsReworked.Extensions;
using TownOfUsReworked.Data;

namespace TownOfUsReworked.PlayerLayers.Roles.IntruderRoles.GodfatherMod
{
    [HarmonyPatch(typeof(AbilityButton), nameof(AbilityButton.DoClick))]
    public static class PerformPromote
    {
        public static bool Prefix(AbilityButton __instance)
        {
            if (Utils.NoButton(PlayerControl.LocalPlayer, RoleEnum.Godfather))
                return true;

            var role = Role.GetRole<Godfather>(PlayerControl.LocalPlayer);

            if (__instance == role.DeclareButton && !role.HasDeclared)
            {
                if (Utils.IsTooFar(role.Player, role.ClosestIntruder))
                    return false;

                var interact = Utils.Interact(role.Player, role.ClosestIntruder);

                if (interact[3])
                {
                    var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.Action, SendOption.Reliable);
                    writer.Write((byte)ActionsRPC.Declare);
                    writer.Write(role.Player.PlayerId);
                    writer.Write(role.ClosestIntruder.PlayerId);
                    AmongUsClient.Instance.FinishRpcImmediately(writer);
                    Declare(role, role.ClosestIntruder);
                }
                else if (interact[0])
                    role.LastDeclared = DateTime.UtcNow;
                else if (interact[1])
                    role.LastDeclared.AddSeconds(CustomGameOptions.ProtectKCReset);

                return false;
            }

            return true;
        }

        public static void Declare(Godfather gf, PlayerControl target)
        {
            gf.HasDeclared = true;
            var formerRole = Role.GetRole(target);

            var mafioso = new Mafioso(target)
            {
                FormerRole = formerRole,
                Godfather = gf
            };

            mafioso.RoleUpdate(formerRole);

            if (target == PlayerControl.LocalPlayer)
                Utils.Flash(Colors.Mafioso, "You have been promoted to <color=#6400FFFF>Mafioso</color>!");

            if (PlayerControl.LocalPlayer.Is(RoleEnum.Seer))
                Utils.Flash(Colors.Seer, "Someone changed their identity!");
        }
    }
}