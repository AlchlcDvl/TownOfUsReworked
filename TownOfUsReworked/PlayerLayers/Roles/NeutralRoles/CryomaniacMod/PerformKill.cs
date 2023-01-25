using System;
using HarmonyLib;
using Hazel;
using TownOfUsReworked.Enums;
using TownOfUsReworked.Lobby.CustomOption;
using TownOfUsReworked.Extensions;
using TownOfUsReworked.PlayerLayers.Roles.CrewRoles.MedicMod;
using TownOfUsReworked.PlayerLayers.Roles.Roles;
using AmongUs.GameOptions;

namespace TownOfUsReworked.PlayerLayers.Roles.NeutralRoles.CryomaniacMod
{
    [HarmonyPatch(typeof(KillButton), nameof(KillButton.DoClick))]
    public class PerformKill
    {
        public static bool Prefix(KillButton __instance)
        {
            if (!PlayerControl.LocalPlayer.Is(RoleEnum.Cryomaniac))
                return true;

            if (PlayerControl.LocalPlayer.Data.IsDead || !PlayerControl.LocalPlayer.CanMove)
                return false;

            var role = Role.GetRole<Cryomaniac>(PlayerControl.LocalPlayer);

            if (role.FreezeUsed)
                return false;

            if (__instance == role.FreezeButton)
            {
                if (!__instance.isActiveAndEnabled)
                    return false;
                
                role.FreezeUsed = true;

                var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte) CustomRPC.Action, SendOption.Reliable, -1);
                writer.Write((byte)ActionsRPC.AllFreeze);
                writer.Write(PlayerControl.LocalPlayer.PlayerId);
                AmongUsClient.Instance.FinishRpcImmediately(writer);
                
                return false;
            }

            if (__instance != DestroyableSingleton<HudManager>.Instance.KillButton)
                return true;

            if (!__instance.isActiveAndEnabled || role.ClosestPlayer == null || role.DouseTimer() != 0 || role.DousedPlayers.Contains(role.ClosestPlayer.PlayerId))
                return false;

            var distBetweenPlayers = Utils.GetDistBetweenPlayers(PlayerControl.LocalPlayer, role.ClosestPlayer);
            var flag3 = distBetweenPlayers < GameOptionsData.KillDistances[CustomGameOptions.InteractionDistance];

            if (!flag3)
                return false;

            if (Utils.CheckInteractionSesitive(role.ClosestPlayer))
            {
                if (role.Player.IsShielded())
                {
                    var writer3 = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.AttemptSound, SendOption.Reliable, -1);
                    writer3.Write(PlayerControl.LocalPlayer.GetMedic().Player.PlayerId);
                    writer3.Write(PlayerControl.LocalPlayer.PlayerId);
                    AmongUsClient.Instance.FinishRpcImmediately(writer3);

                    System.Console.WriteLine(CustomGameOptions.ShieldBreaks + "- shield break");

                    if (CustomGameOptions.ShieldBreaks)
                        role.LastDoused = DateTime.UtcNow;

                    StopKill.BreakShield(PlayerControl.LocalPlayer.GetMedic().Player.PlayerId, PlayerControl.LocalPlayer.PlayerId, CustomGameOptions.ShieldBreaks);
                    return false;
                }
                else if (!role.Player.IsProtected())
                    Utils.RpcMurderPlayer(role.ClosestPlayer, PlayerControl.LocalPlayer);

                role.LastDoused = DateTime.UtcNow;
                return false;
            }
            else if (role.Player.IsOtherRival(role.ClosestPlayer))
            {
                role.LastDoused = DateTime.UtcNow;
                return false;
            }

            var writer2 = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.Action, SendOption.Reliable, -1);
            writer2.Write((byte)ActionsRPC.FreezeDouse);
            writer2.Write(PlayerControl.LocalPlayer.PlayerId);
            writer2.Write(role.ClosestPlayer.PlayerId);
            AmongUsClient.Instance.FinishRpcImmediately(writer2);
            role.DousedPlayers.Add(role.ClosestPlayer.PlayerId);
            role.LastDoused = DateTime.UtcNow;
            __instance.SetTarget(null);
            return false;
        }
    }
}
