using System;
using HarmonyLib;
using Hazel;
using Il2CppSystem.Collections.Generic;
using TownOfUsReworked.Enums;
using TownOfUsReworked.Patches;
using TownOfUsReworked.Extensions;
using TownOfUsReworked.Lobby.CustomOption;
using TownOfUsReworked.PlayerLayers.Modifiers;
using TownOfUsReworked.PlayerLayers.Roles.CrewRoles.MedicMod;
using TownOfUsReworked.PlayerLayers.Abilities;
using TownOfUsReworked.PlayerLayers.Abilities.SnitchMod;
using TownOfUsReworked.PlayerLayers.Roles.Roles;
using TownOfUsReworked.PlayerLayers.Abilities.Abilities;
using UnityEngine;

namespace TownOfUsReworked.PlayerLayers.Roles.CrewRoles.ShifterMod
{
    [HarmonyPatch(typeof(KillButton), nameof(KillButton.DoClick))]
    public class PerformShiftButton
    {
        public static bool Prefix(KillButton __instance)
        {
            if (__instance != DestroyableSingleton<HudManager>.Instance.KillButton)
                return true;

            var flag = PlayerControl.LocalPlayer.Is(RoleEnum.Shifter);

            if (!flag)
                return true;

            var role = Role.GetRole<Shifter>(PlayerControl.LocalPlayer);

            if (!PlayerControl.LocalPlayer.CanMove)
                return false;

            if (PlayerControl.LocalPlayer.Data.IsDead)
                return false;

            var flag2 = role.ShifterShiftTimer() == 0f;

            if (!flag2)
                return false;

            if (!__instance.enabled)
                return false;

            var maxDistance = GameOptionsData.KillDistances[CustomGameOptions.InteractionDistance];

            if (Utils.GetDistBetweenPlayers(role.Player, role.ClosestPlayer) > maxDistance)
                return false;

            if (role.ClosestPlayer == null)
                return false;

            var playerId = role.ClosestPlayer.PlayerId;
            var player = PlayerControl.LocalPlayer;

            if ((player.IsInfected() || role.Player.IsInfected()) && !player.Is(RoleEnum.Plaguebearer))
            {
                foreach (var pb in Role.GetRoles(RoleEnum.Plaguebearer))
                    ((Plaguebearer)pb).RpcSpreadInfection(player, role.Player);
            }

            if (role.ClosestPlayer.IsShielded())
            {
                var medic = role.ClosestPlayer.GetMedic().Player.PlayerId;
                var writer1 = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId,
                    (byte)CustomRPC.AttemptSound, SendOption.Reliable, -1);
                writer1.Write(medic);
                writer1.Write(role.ClosestPlayer.PlayerId);
                AmongUsClient.Instance.FinishRpcImmediately(writer1);

                if (CustomGameOptions.ShieldBreaks)
                    role.LastShifted = DateTime.UtcNow;

                StopKill.BreakShield(medic, role.ClosestPlayer.PlayerId, CustomGameOptions.ShieldBreaks);

                return false;
            }

            if (!role.ClosestPlayer.Is(Faction.Crew))
            {
                Utils.RpcMurderPlayer(role.Player, role.Player);
                return false;
            }

            var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.Shift, SendOption.Reliable, -1);
            writer.Write(PlayerControl.LocalPlayer.PlayerId);
            writer.Write(playerId);
            AmongUsClient.Instance.FinishRpcImmediately(writer);
            Shift(role, role.ClosestPlayer);
            return false;
        }

        public static void Shift(Shifter shifterRole, PlayerControl other)
        {
            var role = Role.GetRole(other);
            var roleType = role.RoleType;
            var ability = Utils.GetAbility(other);
            shifterRole.LastShifted = DateTime.UtcNow;
            var shifter = shifterRole.Player;
            List<PlayerTask> tasks1, tasks2;
            List<GameData.TaskInfo> taskinfos1, taskinfos2;
            Role newRole;

            switch (roleType)
            {
                case RoleEnum.Agent:
                    newRole = new Agent(shifter);
                    break;

                case RoleEnum.Altruist:
                    newRole = new Altruist(shifter);
                    break;
                    
                case RoleEnum.Coroner:
                    newRole = new Coroner(shifter);
                    break;
                    
                case RoleEnum.Crewmate:
                    newRole = new Crewmate(shifter);
                    break;
                    
                case RoleEnum.Detective:
                    newRole = new Detective(shifter);
                    break;
                    
                case RoleEnum.Engineer:
                    newRole = new Engineer(shifter);
                    break;
                    
                case RoleEnum.Escort:
                    newRole = new Escort(shifter);
                    break;
                    
                case RoleEnum.Inspector:
                    newRole = new Inspector(shifter);
                    break;
                    
                case RoleEnum.Investigator:
                    newRole = new Investigator(shifter);
                    break;
                    
                case RoleEnum.Sheriff:
                    newRole = new Sheriff(shifter);
                    break;
                    
                case RoleEnum.Mayor:
                    newRole = new Mayor(shifter);
                    break;
                    
                case RoleEnum.Swapper:
                    newRole = new Swapper(shifter);
                    break;
                    
                case RoleEnum.Medic:
                    newRole = new Medic(shifter);
                    break;
                    
                case RoleEnum.Tracker:
                    newRole = new Tracker(shifter);
                    break;
                    
                case RoleEnum.Transporter:
                    newRole = new Transporter(shifter);
                    break;
                    
                case RoleEnum.Medium:
                    newRole = new Medium(shifter);
                    break;
                    
                case RoleEnum.Operative:
                    newRole = new Operative(shifter);
                    break;
                    
                case RoleEnum.Shifter:
                    newRole = new Shifter(shifter);
                    break;
                    
                case RoleEnum.TimeLord:
                    newRole = new TimeLord(shifter);
                    break;
                    
                case RoleEnum.VampireHunter:
                    newRole = new VampireHunter(shifter);
                    break;
                    
                case RoleEnum.Veteran:
                    newRole = new Veteran(shifter);
                    break;
                    
                case RoleEnum.Vigilante:
                    newRole = new Vigilante(shifter);
                    break;
                
                default:
                    newRole = new Shifter(shifter);
                    break;
            }
            
            newRole.RoleHistory.Add(shifterRole);
            newRole.RoleHistory.AddRange(shifterRole.RoleHistory);
            
            if (newRole.Player == PlayerControl.LocalPlayer)
                newRole.RegenTask();

            if (other.IsRecruit())
                newRole.IsRecruit = true;
            
            Role newRole2;
            
            if (CustomGameOptions.ShiftedBecomes == BecomeEnum.Shifter)
                newRole2 = new Shifter(other);
            else
                newRole2 = new Crewmate(other);

            newRole2.RoleHistory.Add(role);
            newRole2.RoleHistory.AddRange(role.RoleHistory);

            if (other.IsRecruit())
                newRole2.IsRecruit = true;

            if (other == PlayerControl.LocalPlayer)
                newRole2.RegenTask();
            
            tasks1 = other.myTasks;
            taskinfos1 = other.Data.Tasks;
            tasks2 = shifter.myTasks;
            taskinfos2 = shifter.Data.Tasks;

            shifter.myTasks = tasks1;
            shifter.Data.Tasks = taskinfos1;
            other.myTasks = tasks2;
            other.Data.Tasks = taskinfos2;
        }
    }
}
