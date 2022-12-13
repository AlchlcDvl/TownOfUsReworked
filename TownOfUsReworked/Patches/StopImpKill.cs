using HarmonyLib;
using TownOfUsReworked.PlayerLayers.Roles.CrewRoles.MedicMod;
using TownOfUsReworked.PlayerLayers.Roles.Roles;
using Hazel;
using TownOfUsReworked.Extensions;
using TownOfUsReworked.PlayerLayers.Roles;
using TownOfUsReworked.Enums;
using TownOfUsReworked.Lobby.CustomOption;

namespace TownOfUsReworked.Patches
{
    [HarmonyPatch(typeof(KillButton), nameof(KillButton.DoClick))]
    public class StopImpKill
    {
        [HarmonyPriority(Priority.First)]
        public static bool Prefix(KillButton __instance)
        {
            if (__instance != DestroyableSingleton<HudManager>.Instance.KillButton)
                return true;

            if (!PlayerControl.LocalPlayer.Is(Faction.Intruders))
                return true;

            var target = __instance.currentTarget;

            if (target == null)
                return true;

            if (!__instance.isActiveAndEnabled | __instance.isCoolingDown)
                return true;
            
            if (target.Is(RoleEnum.Pestilence))
            {
                if (PlayerControl.LocalPlayer.IsShielded())
                {
                    var medic = PlayerControl.LocalPlayer.GetMedic().Player.PlayerId;

                    unchecked
                    {
                        var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.AttemptSound,
                            SendOption.Reliable, -1);
                        writer.Write(medic);
                        writer.Write(PlayerControl.LocalPlayer.PlayerId);
                        AmongUsClient.Instance.FinishRpcImmediately(writer);
                    }

                    if (CustomGameOptions.ShieldBreaks)
                        PlayerControl.LocalPlayer.SetKillTimer(GameOptionsManager.Instance.currentNormalGameOptions.KillCooldown);
                    else
                        PlayerControl.LocalPlayer.SetKillTimer(0.01f);

                    StopKill.BreakShield(medic, PlayerControl.LocalPlayer.PlayerId, CustomGameOptions.ShieldBreaks);
                }

                if (PlayerControl.LocalPlayer.IsProtected())
                {
                    PlayerControl.LocalPlayer.SetKillTimer(CustomGameOptions.ProtectKCReset + 0.01f);
                    return false;
                }

                Utils.RpcMurderPlayer(target, PlayerControl.LocalPlayer);
                return false;
            }

            if (target.IsInfected() | PlayerControl.LocalPlayer.IsInfected())
            {
                foreach (var pb in Role.GetRoles(RoleEnum.Plaguebearer))
                    ((Plaguebearer)pb).RpcSpreadInfection(target, PlayerControl.LocalPlayer);
            }

            if (target.IsOnAlert())
            {
                if (target.IsShielded())
                {
                    var medic = target.GetMedic().Player.PlayerId;
                    var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId,
                        (byte)CustomRPC.AttemptSound, SendOption.Reliable, -1);
                    writer.Write(medic);
                    writer.Write(target.PlayerId);
                    AmongUsClient.Instance.FinishRpcImmediately(writer);

                    if (CustomGameOptions.ShieldBreaks)
                        PlayerControl.LocalPlayer.SetKillTimer(GameOptionsManager.Instance.currentNormalGameOptions.KillCooldown);
                    else
                        PlayerControl.LocalPlayer.SetKillTimer(0.01f);

                    StopKill.BreakShield(medic, target.PlayerId, CustomGameOptions.ShieldBreaks);

                    if (!PlayerControl.LocalPlayer.IsProtected())
                        Utils.RpcMurderPlayer(target, PlayerControl.LocalPlayer);
                }
                else if (PlayerControl.LocalPlayer.IsShielded())
                {
                    var medic = PlayerControl.LocalPlayer.GetMedic().Player.PlayerId;
                    var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId,
                        (byte)CustomRPC.AttemptSound, SendOption.Reliable, -1);
                    writer.Write(medic);
                    writer.Write(PlayerControl.LocalPlayer.PlayerId);
                    AmongUsClient.Instance.FinishRpcImmediately(writer);

                    if (CustomGameOptions.ShieldBreaks)
                        PlayerControl.LocalPlayer.SetKillTimer(GameOptionsManager.Instance.currentNormalGameOptions.KillCooldown);
                    else
                        PlayerControl.LocalPlayer.SetKillTimer(0.01f);

                    StopKill.BreakShield(medic, PlayerControl.LocalPlayer.PlayerId, CustomGameOptions.ShieldBreaks);
                }
                else
                {
                    if (!PlayerControl.LocalPlayer.IsProtected())
                        Utils.RpcMurderPlayer(target, PlayerControl.LocalPlayer);
                    else
                        PlayerControl.LocalPlayer.SetKillTimer(CustomGameOptions.ProtectKCReset + 0.01f);
                }
                return false;
            }
            else if (target.IsShielded())
            {
                var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId,
                    (byte)CustomRPC.AttemptSound, SendOption.Reliable, -1);
                writer.Write(target.GetMedic().Player.PlayerId);
                writer.Write(target.PlayerId);
                AmongUsClient.Instance.FinishRpcImmediately(writer);

                System.Console.WriteLine(CustomGameOptions.ShieldBreaks + "- shield break");

                if (CustomGameOptions.ShieldBreaks)
                    PlayerControl.LocalPlayer.SetKillTimer(GameOptionsManager.Instance.currentNormalGameOptions.KillCooldown);
                else
                    PlayerControl.LocalPlayer.SetKillTimer(0.01f);

                StopKill.BreakShield(target.GetMedic().Player.PlayerId, target.PlayerId, CustomGameOptions.ShieldBreaks);
                return false;
            }
            else if (target.IsVesting())
            {
                PlayerControl.LocalPlayer.SetKillTimer(CustomGameOptions.VestKCReset + 0.01f);
                return false;
            }
            else if (target.IsProtected())
            {
                PlayerControl.LocalPlayer.SetKillTimer(CustomGameOptions.ProtectKCReset + 0.01f);
                return false;
            }
            
            PlayerControl.LocalPlayer.SetKillTimer(GameOptionsManager.Instance.currentNormalGameOptions.KillCooldown);
            Utils.RpcMurderPlayer(PlayerControl.LocalPlayer, target);
            return false;
        }
    }
}