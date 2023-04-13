using System;
using HarmonyLib;
using Hazel;
using TownOfUsReworked.CustomOptions;
using TownOfUsReworked.Classes;
using TownOfUsReworked.Data;
using TownOfUsReworked.Extensions;
using System.Linq;

namespace TownOfUsReworked.PlayerLayers.Roles.SyndicateRoles.PoisonerMod
{
    [HarmonyPatch(typeof(AbilityButton), nameof(AbilityButton.DoClick))]
    public static class PerformPoison
    {
        public static bool Prefix(AbilityButton __instance)
        {
            if (Utils.NoButton(PlayerControl.LocalPlayer, RoleEnum.Poisoner))
                return true;

            var role = Role.GetRole<Poisoner>(PlayerControl.LocalPlayer);

            if (__instance == role.PoisonButton)
            {
                if (role.PoisonTimer() != 0f)
                    return false;

                if (!role.HoldsDrive)
                {
                    if (Utils.IsTooFar(role.Player, role.ClosestPlayer))
                        return false;

                    var interact = Utils.Interact(role.Player, role.ClosestPlayer);

                    if (interact[3])
                    {
                        var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.Action, SendOption.Reliable);
                        writer.Write((byte)ActionsRPC.Poison);
                        writer.Write(PlayerControl.LocalPlayer.PlayerId);
                        writer.Write(role.PoisonedPlayer.PlayerId);
                        AmongUsClient.Instance.FinishRpcImmediately(writer);
                        role.PoisonedPlayer = role.ClosestPlayer;
                        role.TimeRemaining = CustomGameOptions.PoisonDuration;
                        role.Poison();
                    }
                    else if (interact[0])
                        role.LastPoisoned = DateTime.UtcNow;
                    else if (interact[1])
                        role.LastPoisoned.AddSeconds(CustomGameOptions.ProtectKCReset);
                    else if (interact[2])
                        role.LastPoisoned.AddSeconds(CustomGameOptions.VestKCReset);
                }
                else if (role.PoisonedPlayer == null)
                    role.PoisonMenu.Open(PlayerControl.AllPlayerControls.ToArray().Where(x => !x.Is(Faction.Syndicate) && x != role.PoisonedPlayer).ToList());
                else
                {
                    var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.Action, SendOption.Reliable);
                    writer.Write((byte)ActionsRPC.Poison);
                    writer.Write(PlayerControl.LocalPlayer.PlayerId);
                    writer.Write(role.PoisonedPlayer.PlayerId);
                    AmongUsClient.Instance.FinishRpcImmediately(writer);
                    role.TimeRemaining = CustomGameOptions.PoisonDuration;
                    role.Poison();
                }
            }

            return true;
        }
    }
}