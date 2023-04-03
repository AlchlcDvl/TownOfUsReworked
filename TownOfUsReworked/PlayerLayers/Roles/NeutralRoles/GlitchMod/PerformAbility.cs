using HarmonyLib;
using TownOfUsReworked.Classes;
using TownOfUsReworked.CustomOptions;
using System;
using Hazel;
using TownOfUsReworked.Data;

namespace TownOfUsReworked.PlayerLayers.Roles.NeutralRoles.GlitchMod
{
    [HarmonyPatch(typeof(AbilityButton), nameof(AbilityButton.DoClick))]
    public static class PerformAbility
    {
        public static bool Prefix(AbilityButton __instance)
        {
            if (Utils.NoButton(PlayerControl.LocalPlayer, RoleEnum.Glitch))
                return true;

            var role = Role.GetRole<Glitch>(PlayerControl.LocalPlayer);

            if (__instance == role.HackButton)
            {
                if (role.HackTimer() != 0f)
                    return false;

                if (Utils.IsTooFar(role.Player, role.ClosestPlayer))
                    return false;

                var interact = Utils.Interact(role.Player, role.ClosestPlayer);

                if (interact[3])
                {
                    var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.Action, SendOption.Reliable);
                    writer.Write((byte)ActionsRPC.GlitchRoleblock);
                    writer.Write(PlayerControl.LocalPlayer.PlayerId);
                    writer.Write(role.ClosestPlayer.PlayerId);
                    AmongUsClient.Instance.FinishRpcImmediately(writer);
                    role.HackTarget = role.ClosestPlayer;
                    role.TimeRemaining = CustomGameOptions.HackDuration;
                    role.Hack();

                    foreach (var layer in PlayerLayer.GetLayers(role.HackTarget))
                        layer.IsBlocked = !Role.GetRole(role.HackTarget).RoleBlockImmune;
                }
                else if (interact[0])
                    role.LastHack = DateTime.UtcNow;
                else if (interact[1])
                    role.LastHack.AddSeconds(CustomGameOptions.ProtectKCReset);

                return false;
            }
            else if (__instance == role.KillButton)
            {
                if (Utils.IsTooFar(role.Player, role.ClosestPlayer))
                    return false;

                if (role.KillTimer() != 0f)
                    return false;

                var interact = Utils.Interact(role.Player, role.ClosestPlayer, true);

                if (interact[3] || interact[0])
                    role.LastKilled = DateTime.UtcNow;
                else if (interact[1])
                    role.LastKilled.AddSeconds(CustomGameOptions.ProtectKCReset);
                else if (interact[2])
                    role.LastKilled.AddSeconds(CustomGameOptions.VestKCReset);

                return false;
            }
            else if (__instance == role.MimicButton)
            {
                if (role.MimicTimer() != 0f)
                    return false;

                if (role.MimicTarget == null)
                    return false;

                var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.Action, SendOption.Reliable);
                writer.Write((byte)ActionsRPC.Mimic);
                writer.Write(PlayerControl.LocalPlayer.PlayerId);
                role.TimeRemaining2 = CustomGameOptions.MimicDuration;
                role.Mimic();
                return false;
            }
            else if (__instance == role.SampleButton)
            {
                if (role.MimicTimer() != 0f)
                    return false;

                role.OpenMimicMenu();
                return false;
            }

            return true;
        }
    }
}