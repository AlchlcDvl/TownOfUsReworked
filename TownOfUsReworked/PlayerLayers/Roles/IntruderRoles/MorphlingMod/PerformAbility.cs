using System;
using HarmonyLib;
using Hazel;
using TownOfUsReworked.Enums;
using TownOfUsReworked.CustomOptions;
using TownOfUsReworked.Classes;

namespace TownOfUsReworked.PlayerLayers.Roles.IntruderRoles.MorphlingMod
{
    [HarmonyPatch(typeof(AbilityButton), nameof(AbilityButton.DoClick))]
    public class PerformAbility
    {
        public static bool Prefix(AbilityButton __instance)
        {
            if (Utils.NoButton(PlayerControl.LocalPlayer, RoleEnum.Morphling))
                return true;

            var role = Role.GetRole<Morphling>(PlayerControl.LocalPlayer);

            if (__instance == role.MorphButton)
            {
                if (role.MorphTimer() != 0f)
                    return false;

                var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.Action, SendOption.Reliable);
                writer.Write((byte)ActionsRPC.Morph);
                writer.Write(PlayerControl.LocalPlayer.PlayerId);
                writer.Write(role.SampledPlayer.PlayerId);
                AmongUsClient.Instance.FinishRpcImmediately(writer);
                role.TimeRemaining = CustomGameOptions.MorphlingDuration;
                role.MorphedPlayer = role.SampledPlayer;
                role.Morph();
                return false;
            }
            else if (__instance == role.SampleButton)
            {
                if (role.SampleTimer() != 0f)
                    return false;

                if (Utils.IsTooFar(role.Player, role.ClosestTarget))
                    return false;

                if (role.SampledPlayer == role.ClosestTarget)
                    return false;

                var interact = Utils.Interact(role.Player, role.ClosestPlayer);

                if (interact[3] == true)
                    role.SampledPlayer = role.ClosestTarget;

                if (interact[0] == true)
                {
                    role.LastSampled = DateTime.UtcNow;

                    if (CustomGameOptions.MorphCooldownsLinked)
                        role.LastMorphed = DateTime.UtcNow;
                }
                else if (interact[1] == true)
                {
                    role.LastSampled.AddSeconds(CustomGameOptions.ProtectKCReset);

                    if (CustomGameOptions.MorphCooldownsLinked)
                        role.LastMorphed.AddSeconds(CustomGameOptions.ProtectKCReset);
                }
            }

            return true;
        }
    }
}
