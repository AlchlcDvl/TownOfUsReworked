using System;
using HarmonyLib;
using Hazel;
using TownOfUsReworked.PlayerLayers.Roles.CrewRoles.InvestigatorMod;
using TownOfUsReworked.PlayerLayers.Roles.CrewRoles.MedicMod;
using TownOfUsReworked.PlayerLayers.Roles.NeutralRoles.VampireMod;
using TownOfUsReworked.PlayerLayers.Roles.NeutralRoles.AmnesiacMod;
using TownOfUsReworked.PlayerLayers.Roles.NeutralRoles.CannibalMod;
using TownOfUsReworked.PlayerLayers.Roles.NeutralRoles.TaskmasterMod;
using TownOfUsReworked.PlayerLayers.Abilities;
using TownOfUsReworked.PlayerLayers.Abilities.SnitchMod;
using TownOfUsReworked.PlayerLayers.Roles.CrewRoles.OperativeMod;
using TownOfUsReworked.PlayerLayers.Roles.Roles;
using TownOfUsReworked.Enums;
using TownOfUsReworked.Lobby.CustomOption;
using TownOfUsReworked.Extensions;
using TownOfUsReworked.Patches;
using UnityEngine;

namespace TownOfUsReworked.PlayerLayers.Roles.NeutralRoles.DraculaMod
{
    [HarmonyPatch(typeof(KillButton), nameof(KillButton.DoClick))]
    public class PerformConvertButton
    {
        public static bool Prefix(KillButton __instance)
        {
            if (__instance != DestroyableSingleton<HudManager>.Instance.KillButton) return true;
            var flag = PlayerControl.LocalPlayer.Is(RoleEnum.Dracula);
            if (!flag) return true;
            var role = Role.GetRole<Dracula>(PlayerControl.LocalPlayer);
            if (!PlayerControl.LocalPlayer.CanMove) return false;
            if (PlayerControl.LocalPlayer.Data.IsDead) return false;
            var flag2 = role.ConvertTimer() == 0f;
            if (!flag2) return false;
            if (!__instance.enabled) return false;
            var maxDistance = GameOptionsData.KillDistances[PlayerControl.GameOptions.KillDistance];
            if (Vector2.Distance(role.ClosestPlayer.GetTruePosition(), PlayerControl.LocalPlayer.GetTruePosition()) > maxDistance) return false;
            if (role.ClosestPlayer == null) return false;
            var playerId = role.ClosestPlayer.PlayerId;
            var player = PlayerControl.LocalPlayer;

            if ((player.IsInfected() | role.Player.IsInfected()) && !player.Is(RoleEnum.Plaguebearer))
            {
                foreach (var pb in Role.GetRoles(RoleEnum.Plaguebearer)) ((Plaguebearer)pb).RpcSpreadInfection(player, role.Player);
            }

            if (role.ClosestPlayer.IsShielded())
            {
                var medic = role.ClosestPlayer.GetMedic().Player.PlayerId;

                var writer1 = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId,
                    (byte) CustomRPC.AttemptSound, SendOption.Reliable, -1);
                writer1.Write(medic);
                writer1.Write(role.ClosestPlayer.PlayerId);
                AmongUsClient.Instance.FinishRpcImmediately(writer1);
                if (CustomGameOptions.ShieldBreaks) role.LastBitten = DateTime.UtcNow;
                StopKill.BreakShield(medic, role.ClosestPlayer.PlayerId, CustomGameOptions.ShieldBreaks);

                return false;
            }

            unchecked
            {
                var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId,
                    (byte) CustomRPC.Convert, SendOption.Reliable, -1);
                writer.Write(PlayerControl.LocalPlayer.PlayerId);
                writer.Write(playerId);
                AmongUsClient.Instance.FinishRpcImmediately(writer);
            }

            Convert(role, role.ClosestPlayer);
            return false;
        }

        public static void Convert(Dracula dracRole, PlayerControl other)
        {
            var role = Utils.GetRole(other);
            var tm2 = Role.GetRole<Taskmaster>(other);
            var drac = dracRole.Player;
            dracRole.LastBitten = DateTime.UtcNow;
            var snitch = false;
            var convert = false;

            switch (role)
            {
                case RoleEnum.Sheriff:
                case RoleEnum.Engineer:
                case RoleEnum.Mayor:
                case RoleEnum.Swapper:
                case RoleEnum.Investigator:
                case RoleEnum.TimeLord:
                case RoleEnum.Medic:
                case RoleEnum.Agent:
                case RoleEnum.Altruist:
                case RoleEnum.Vigilante:
                case RoleEnum.Veteran:
                case RoleEnum.Crewmate:
                case RoleEnum.Tracker:
                case RoleEnum.Transporter:
                case RoleEnum.Medium:
                case RoleEnum.Coroner:
                case RoleEnum.Operative:
                case RoleEnum.Detective:
                case RoleEnum.Shifter:
                case RoleEnum.Amnesiac:
                case RoleEnum.Survivor:
                case RoleEnum.Jester:
                case RoleEnum.Cannibal:
                case RoleEnum.Cryomaniac:
                case RoleEnum.Taskmaster:

                    convert = true;

                    break;
            }

            if (convert == true && CustomGameOptions.DraculaConvertNeuts)
            {
                if (role == RoleEnum.Amnesiac)
                {
                    var amne = Role.GetRole<Amnesiac>(other);
                    amne.BodyArrows.Values.DestroyAll();
                    amne.BodyArrows.Clear();
                    amne.CurrentTarget.bodyRenderer.material.SetFloat("_Outline", 0f);
                }
                else if (role == RoleEnum.Cannibal)
                {
                    var can = Role.GetRole<Cannibal>(other);
                    can.BodyArrows.Values.DestroyAll();
                    can.BodyArrows.Clear();
                    can.CurrentTarget.bodyRenderer.material.SetFloat("_Outline", 0f);
                }
                else if (role == RoleEnum.Taskmaster && tm2.TasksLeft <= CustomGameOptions.TMTasksRemaining)
                {
                    Utils.RpcMurderPlayer(drac, other);
                }

                Role.RoleDictionary.Remove(other.PlayerId);
                new Vampire(other);
            }
            else if (convert == true)
            {
                if (role == RoleEnum.Investigator)
                {
                    var invRole = Role.GetRole<Investigator>(drac);
                    Footprint.DestroyAll(invRole);
                }
                else if (role == RoleEnum.Vigilante)
                {
                    var vigilanteRole = Role.GetRole<Vigilante>(drac);
                    vigilanteRole.LastKilled = DateTime.UtcNow;
                }
                else if (role == RoleEnum.Engineer)
                {
                    var engiRole = Role.GetRole<Engineer>(drac);
                    engiRole.UsedThisRound = false;
                }
                else if (role == RoleEnum.Medic)
                {
                    var medicRole = Role.GetRole<Medic>(drac);
                    medicRole.UsedAbility = false;
                }
                else if (role == RoleEnum.Mayor)
                {
                    var mayorRole = Role.GetRole<Mayor>(drac);
                    mayorRole.VoteBank = CustomGameOptions.MayorVoteBank;
                    DestroyableSingleton<HudManager>.Instance.KillButton.gameObject.SetActive(false);
                }
                else if (role == RoleEnum.Veteran)
                {
                    var vetRole = Role.GetRole<Veteran>(drac);
                    vetRole.UsesLeft = CustomGameOptions.MaxAlerts;
                    vetRole.LastAlerted = DateTime.UtcNow;
                }
                else if (role == RoleEnum.Tracker)
                {
                    var trackerRole = Role.GetRole<Tracker>(drac);
                    trackerRole.TrackerArrows.Values.DestroyAll();
                    trackerRole.TrackerArrows.Clear();
                    trackerRole.UsesLeft = CustomGameOptions.MaxTracks;
                    trackerRole.LastTracked = DateTime.UtcNow;
                }
                else if (role == RoleEnum.Detective)
                {
                    var detectiveRole = Role.GetRole<Detective>(drac);
                    detectiveRole.LastExamined = DateTime.UtcNow;
                }
                else if (role == RoleEnum.Coroner)
                {
                    var coronerRole = Role.GetRole<Coroner>(drac);
                    coronerRole.BodyArrows.Values.DestroyAll();
                    coronerRole.BodyArrows.Clear();
                    DestroyableSingleton<HudManager>.Instance.KillButton.gameObject.SetActive(false);
                }
                else if (role == RoleEnum.TimeLord)
                {
                    var tlRole = Role.GetRole<TimeLord>(drac);
                    tlRole.FinishRewind = DateTime.UtcNow;
                    tlRole.StartRewind = DateTime.UtcNow;
                    tlRole.StartRewind = tlRole.StartRewind.AddSeconds(-10.0f);
                    tlRole.UsesLeft = CustomGameOptions.RewindMaxUses;
                }
                else if (role == RoleEnum.Transporter)
                {
                    var tpRole = Role.GetRole<Transporter>(drac);
                    tpRole.PressedButton = false;
                    tpRole.MenuClick = false;
                    tpRole.LastMouse = false;
                    tpRole.TransportList = null;
                    tpRole.TransportPlayer1 = null;
                    tpRole.TransportPlayer2 = null;
                    tpRole.LastTransported = DateTime.UtcNow;
                    tpRole.UsesLeft = CustomGameOptions.TransportMaxUses;
                }
                else if (role == RoleEnum.Medium)
                {
                    var medRole = Role.GetRole<Medium>(drac);
                    medRole.MediatedPlayers.Values.DestroyAll();
                    medRole.MediatedPlayers.Clear();
                    medRole.LastMediated = DateTime.UtcNow;
                }
                else if (role == RoleEnum.Sheriff)
                {
                    var sheriffRole = Role.GetRole<Sheriff>(drac);
                    sheriffRole.Interrogated.RemoveRange(0, sheriffRole.Interrogated.Count);
                    sheriffRole.LastInterrogated = DateTime.UtcNow;
                }
                else if (role == RoleEnum.Operative)
                {
                    var opRole = Role.GetRole<Operative>(drac);
                    opRole.lastBugged = DateTime.UtcNow;
                    opRole.UsesLeft = CustomGameOptions.MaxBugs;
                    opRole.buggedPlayers.Clear();
                    opRole.bugs.ClearBugs();
                }

                new Vampire(other);

                //Role.RoleDictionary.Add(drac.PlayerId, newRole);
                dracRole.AddToRoleHistory(dracRole.RoleType);
            }
            else if (role == RoleEnum.VampireHunter)
            {
                Utils.RpcMurderPlayer(other, drac);
            }
            else
            {
                Utils.RpcMurderPlayer(drac, other);
            }
            
            if (snitch)
            {
                var snitchRole = Ability.GetAbility<Snitch>(drac);
                snitchRole.ImpArrows.DestroyAll();
                snitchRole.SnitchArrows.Clear();
                snitchRole.ImpArrows.Clear();
                CompleteTask.Postfix(drac);
                if (other.AmOwner)
                    foreach (var player in PlayerControl.AllPlayerControls)
                        player.name.Color("white");
            }

            if (drac.AmOwner | other.AmOwner)
            {
                foreach (var vigilanteRole in Role.GetRoles(RoleEnum.Vigilante))
                {
                    var vigilante = (Vigilante)vigilanteRole;
                    vigilante.LastKilled = DateTime.UtcNow;
                    vigilante.LastKilled = vigilante.LastKilled.AddSeconds(2.5f - CustomGameOptions.VigiKillCd);
                }

                DestroyableSingleton<HudManager>.Instance.KillButton.gameObject.SetActive(false);
            }
        }
    }
}
