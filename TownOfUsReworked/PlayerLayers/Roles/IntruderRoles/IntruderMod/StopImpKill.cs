using HarmonyLib;
using TownOfUsReworked.PlayerLayers.Roles.CrewRoles.MedicMod;
using TownOfUsReworked.PlayerLayers.Roles.Roles;
using Hazel;
using TownOfUsReworked.Extensions;
using TownOfUsReworked.Patches;
using TownOfUsReworked.Enums;
using TownOfUsReworked.Lobby.CustomOption;

namespace TownOfUsReworked.PlayerLayers.Roles.IntruderRoles.IntruderMod
{
    [HarmonyPatch(typeof(KillButton), nameof(KillButton.DoClick))]
    public class StopImpKill
    {
        [HarmonyPriority(Priority.First)]
        public static bool Prefix(KillButton __instance)
        {
            if (!PlayerControl.LocalPlayer.Is(Faction.Intruder))
                return false;

            var target = __instance.currentTarget;

            if (target == null)
                return false;

            if (!__instance.isActiveAndEnabled || __instance.isCoolingDown)
                return false;
            
            //Last ditch effort to stop Consigliere and Disguiser from killing
            var role = Role.GetRole(PlayerControl.LocalPlayer);

            if (role.RoleType == RoleEnum.Consigliere)
            {
                var consig = (Consigliere)role;

                if (__instance == consig.InvestigateButton)
                    return false;
            }
            else if (role.RoleType == RoleEnum.Blackmailer)
            {
                var bmer = (Blackmailer)role;

                if (__instance == bmer.BlackmailButton)
                    return false;
            }
            else if (role.RoleType == RoleEnum.Morphling)
            {
                var morphling = (Morphling)role;

                if (__instance == morphling.MorphButton)
                    return false;
            }
            else if (role.RoleType == RoleEnum.Disguiser)
            {
                var disg = (Disguiser)role;

                if (__instance == disg.DisguiseButton)
                    return false;
            }
            //Progress report: does not fucking work

            if (target.IsInfected() || PlayerControl.LocalPlayer.IsInfected())
            {
                foreach (var pb in Role.GetRoles(RoleEnum.Plaguebearer))
                    ((Plaguebearer)pb).RpcSpreadInfection(target, PlayerControl.LocalPlayer);
            }

            if (target.IsOnAlert() || target.Is(RoleEnum.Pestilence))
            {
                if (target.IsShielded())
                {
                    var medic = target.GetMedic().Player.PlayerId;
                    var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.AttemptSound, SendOption.Reliable, -1);
                    writer.Write(medic);
                    writer.Write(target.PlayerId);
                    AmongUsClient.Instance.FinishRpcImmediately(writer);

                    if (CustomGameOptions.ShieldBreaks)
                        PlayerControl.LocalPlayer.SetKillTimer(CustomGameOptions.IntKillCooldown);
                    else
                        PlayerControl.LocalPlayer.SetKillTimer(0.01f);

                    StopKill.BreakShield(medic, target.PlayerId, CustomGameOptions.ShieldBreaks);

                    if (!PlayerControl.LocalPlayer.IsProtected())
                        Utils.RpcMurderPlayer(target, PlayerControl.LocalPlayer);
                }
                else if (PlayerControl.LocalPlayer.IsShielded())
                {
                    var medic = PlayerControl.LocalPlayer.GetMedic().Player.PlayerId;
                    var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.AttemptSound, SendOption.Reliable, -1);
                    writer.Write(medic);
                    writer.Write(PlayerControl.LocalPlayer.PlayerId);
                    AmongUsClient.Instance.FinishRpcImmediately(writer);

                    if (CustomGameOptions.ShieldBreaks)
                        PlayerControl.LocalPlayer.SetKillTimer(CustomGameOptions.IntKillCooldown);
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
                var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.AttemptSound, SendOption.Reliable, -1);
                writer.Write(target.GetMedic().Player.PlayerId);
                writer.Write(target.PlayerId);
                AmongUsClient.Instance.FinishRpcImmediately(writer);

                System.Console.WriteLine(CustomGameOptions.ShieldBreaks + "- shield break");

                if (CustomGameOptions.ShieldBreaks)
                    PlayerControl.LocalPlayer.SetKillTimer(CustomGameOptions.IntKillCooldown);
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
            
            PlayerControl.LocalPlayer.SetKillTimer(CustomGameOptions.IntKillCooldown);
            Utils.RpcMurderPlayer(PlayerControl.LocalPlayer, target);
            return false;
        }
    }
}