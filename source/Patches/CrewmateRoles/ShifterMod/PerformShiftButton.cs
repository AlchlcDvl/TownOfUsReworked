using System;
using System.Collections;
using HarmonyLib;
using Hazel;
using Il2CppSystem.Collections.Generic;
using Reactor;
using TownOfUs.CrewmateRoles.InvestigatorMod;
using TownOfUs.CrewmateRoles.MedicMod;
using TownOfUs.CrewmateRoles.SnitchMod;
using TownOfUs.Extensions;
using TownOfUs.Roles;
using TownOfUs.Roles.Modifiers;
using UnityEngine;
using TownOfUs.CrewmateRoles.OperativeMod;

namespace TownOfUs.CrewmateRoles.ShifterMod
{
    public enum BecomeEnum
    {
        Shifter,
        Crewmate
    }

    [HarmonyPatch(typeof(KillButton), nameof(KillButton.DoClick))]
    [HarmonyPriority(Priority.Last)]
    public class PerformShiftButton
    {
        public static bool Prefix(KillButton __instance)
        {
            if (__instance != DestroyableSingleton<HudManager>.Instance.KillButton) return true;
            var flag = PlayerControl.LocalPlayer.Is(RoleEnum.Shifter);
            if (!flag) return true;
            var role = Role.GetRole<Shifter>(PlayerControl.LocalPlayer);
            if (!PlayerControl.LocalPlayer.CanMove) return false;
            if (PlayerControl.LocalPlayer.Data.IsDead) return false;
            var flag2 = role.ShifterShiftTimer() == 0f;
            if (!flag2) return false;
            if (!__instance.enabled) return false;
            var maxDistance = GameOptionsData.KillDistances[PlayerControl.GameOptions.KillDistance];
            if (Vector2.Distance(role.ClosestPlayer.GetTruePosition(), PlayerControl.LocalPlayer.GetTruePosition()) > maxDistance) return false;
            if (role.ClosestPlayer == null) return false;
            var playerId = role.ClosestPlayer.PlayerId;
            var player = PlayerControl.LocalPlayer;

            if ((player.IsInfected() || role.Player.IsInfected()) && !player.Is(RoleEnum.Plaguebearer))
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
                if (CustomGameOptions.ShieldBreaks) role.LastShifted = DateTime.UtcNow;
                StopKill.BreakShield(medic, role.ClosestPlayer.PlayerId, CustomGameOptions.ShieldBreaks);

                return false;
            }

            var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId,
                (byte) CustomRPC.Shift, SendOption.Reliable, -1);
            writer.Write(PlayerControl.LocalPlayer.PlayerId);
            writer.Write(playerId);
            AmongUsClient.Instance.FinishRpcImmediately(writer);

            Shift(role, role.ClosestPlayer);
            return false;
        }

        [HarmonyPatch(typeof(HudManager), nameof(HudManager.Update))]
        public static IEnumerator ShowShift()
        {
            var wait = new WaitForSeconds(0.83333336f);
            var hud = DestroyableSingleton<HudManager>.Instance;
            var overlay = hud.KillOverlay;
            var transform = overlay.flameParent.transform;
            var flame = transform.GetChild(0).gameObject;
            var renderer = flame.GetComponent<SpriteRenderer>();

            renderer.sprite = TownOfUs.ShiftKill;
            var background = overlay.background;
            overlay.flameParent.SetActive(true);
            yield return new WaitForLerp(0.16666667f, delegate(float t) { overlay.flameParent.transform.localScale = new Vector3(1f, t, 1f);});
            yield return new WaitForSeconds(1f);
            yield return new WaitForLerp(0.16666667f, delegate(float t) { overlay.flameParent.transform.localScale = new Vector3(1f, 1f - t, 1f);});
            overlay.flameParent.SetActive(false);
            overlay.showAll = null;
            renderer.sprite = TownOfUs.ShiftKill;
        }


        public static void Shift(Shifter shifterRole, PlayerControl other)
        {
            var role = Utils.GetRole(other);
            shifterRole.LastShifted = DateTime.UtcNow;
            var shifter = shifterRole.Player;
            List<PlayerTask> tasks1, tasks2;
            List<GameData.TaskInfo> taskinfos1, taskinfos2;

            var swapTasks = true;
            var resetShifter = false;
            var snitch = false;
            var shift = false;

            Role newRole;

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
                case RoleEnum.Snitch:
                case RoleEnum.Altruist:
                case RoleEnum.Vigilante:
                case RoleEnum.Veteran:
                case RoleEnum.Crewmate:
                case RoleEnum.Tracker:
                case RoleEnum.Transporter:
                case RoleEnum.Medium:
                case RoleEnum.Mystic:
                case RoleEnum.Operative:
                case RoleEnum.Detective:
                case RoleEnum.Shifter:

                    shift = true;

                    break;
            }

            if (shift == true)
            {
                newRole = Role.GetRole(other);
                newRole.Player = shifter;

                var modifier = Modifier.GetModifier(other);
                var modifier2 = Modifier.GetModifier(shifter);
                var player = new PlayerControl();

                if (Utils.Is(other, ModifierEnum.Lover) || Utils.Is(shifter, ModifierEnum.Lover))
                {
                    Modifier.ModifierDictionary.Remove(shifter.PlayerId);
                    Modifier.ModifierDictionary.Add(shifter.PlayerId, modifier2);
                    Modifier.ModifierDictionary.Remove(other.PlayerId);
                    Modifier.ModifierDictionary.Add(other.PlayerId, modifier);
                }
                else if (modifier2 != null && !Utils.Is(other, ModifierEnum.Lover))
                {
                    modifier2.Player = other;
                    Modifier.ModifierDictionary.Remove(shifter.PlayerId);
                    Modifier.ModifierDictionary.Add(other.PlayerId, modifier2);
                }
                else if (modifier != null && !Utils.Is(shifter, ModifierEnum.Lover))
                {
                    modifier.Player = shifter;
                    Modifier.ModifierDictionary.Remove(other.PlayerId);
                    Modifier.ModifierDictionary.Add(shifter.PlayerId, modifier);
                }                    
                else if ((modifier != null && modifier2 != null) && !(Utils.Is(shifter, ModifierEnum.Lover) && !Utils.Is(other, ModifierEnum.Lover)))
                {
                    modifier.Player = shifter;
                    modifier2.Player = other;
                    Modifier.ModifierDictionary.Remove(other.PlayerId);
                    Modifier.ModifierDictionary.Remove(shifter.PlayerId);
                    Modifier.ModifierDictionary.Add(shifter.PlayerId, modifier);
                    Modifier.ModifierDictionary.Add(other.PlayerId, modifier2);
                }

                Role.RoleDictionary.Remove(shifter.PlayerId);
                Role.RoleDictionary.Remove(other.PlayerId);

                if (role == RoleEnum.Snitch)
                {
                    var snitchRole = Role.GetRole<Snitch>(shifter);
                    snitchRole.ImpArrows.DestroyAll();
                    snitchRole.SnitchArrows.Values.DestroyAll();
                    snitchRole.SnitchArrows.Clear();
                    CompleteTask.Postfix(shifter);
                    if (other.AmOwner)
                        foreach (var player1 in PlayerControl.AllPlayerControls)
                            player1.nameText().color = Color.white;
                    DestroyableSingleton<HudManager>.Instance.KillButton.gameObject.SetActive(false);
                } 
                else if (role == RoleEnum.Investigator)
                {
                    var invRole = Role.GetRole<Investigator>(shifter);
                    Footprint.DestroyAll(invRole);
                }
                else if (role == RoleEnum.Vigilante)
                {
                    var vigilanteRole = Role.GetRole<Vigilante>(shifter);
                    vigilanteRole.LastKilled = DateTime.UtcNow;
                }
                else if (role == RoleEnum.Engineer)
                {
                    var engiRole = Role.GetRole<Engineer>(shifter);
                    engiRole.UsedThisRound = false;
                }
                else if (role == RoleEnum.Medic)
                {
                    var medicRole = Role.GetRole<Medic>(shifter);
                    medicRole.UsedAbility = false;
                }
                else if (role == RoleEnum.Mayor)
                {
                    var mayorRole = Role.GetRole<Mayor>(shifter);
                    mayorRole.VoteBank = CustomGameOptions.MayorVoteBank;
                    DestroyableSingleton<HudManager>.Instance.KillButton.gameObject.SetActive(false);
                }
                else if (role == RoleEnum.Veteran)
                {
                    var vetRole = Role.GetRole<Veteran>(shifter);
                    vetRole.UsesLeft = CustomGameOptions.MaxAlerts;
                    vetRole.LastAlerted = DateTime.UtcNow;
                }
                else if (role == RoleEnum.Tracker)
                {
                    var trackerRole = Role.GetRole<Tracker>(shifter);
                    trackerRole.TrackerArrows.Values.DestroyAll();
                    trackerRole.TrackerArrows.Clear();
                    trackerRole.UsesLeft = CustomGameOptions.MaxTracks;
                    trackerRole.LastTracked = DateTime.UtcNow;
                }
                else if (role == RoleEnum.Detective)
                {
                    var detectiveRole = Role.GetRole<Detective>(shifter);
                    detectiveRole.LastExamined = DateTime.UtcNow;
                }
                else if (role == RoleEnum.Mystic)
                {
                    var mysticRole = Role.GetRole<Mystic>(shifter);
                    mysticRole.BodyArrows.Values.DestroyAll();
                    mysticRole.BodyArrows.Clear();
                    DestroyableSingleton<HudManager>.Instance.KillButton.gameObject.SetActive(false);
                }
                else if (role == RoleEnum.TimeLord)
                {
                    var tlRole = Role.GetRole<TimeLord>(shifter);
                    tlRole.FinishRewind = DateTime.UtcNow;
                    tlRole.StartRewind = DateTime.UtcNow;
                    tlRole.StartRewind = tlRole.StartRewind.AddSeconds(-10.0f);
                    tlRole.UsesLeft = CustomGameOptions.RewindMaxUses;
                }
                else if (role == RoleEnum.Transporter)
                {
                    var tpRole = Role.GetRole<Transporter>(shifter);
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
                    var medRole = Role.GetRole<Medium>(shifter);
                    medRole.MediatedPlayers.Values.DestroyAll();
                    medRole.MediatedPlayers.Clear();
                    medRole.LastMediated = DateTime.UtcNow;
                }
                else if (role == RoleEnum.Sheriff)
                {
                    var sheriffRole = Role.GetRole<Sheriff>(shifter);
                    sheriffRole.Interrogated.RemoveRange(0, sheriffRole.Interrogated.Count);
                    sheriffRole.LastInterrogated = DateTime.UtcNow;
                }
                else if (role == RoleEnum.Operative)
                {
                    var opRole = Role.GetRole<Operative>(shifter);
                    opRole.lastBugged = DateTime.UtcNow;
                    opRole.UsesLeft = CustomGameOptions.MaxBugs;
                    opRole.buggedPlayers.Clear();
                    opRole.bugs.ClearBugs();
                }
                else if (role == RoleEnum.Shifter)
                {
                    shifterRole.LastShifted = DateTime.UtcNow;
                }

                if (CustomGameOptions.ShiftedBecomes == BecomeEnum.Shifter)
                {
                    resetShifter = true;
                    shifterRole.Player = other;
                    Role.RoleDictionary.Add(other.PlayerId, shifterRole);
                }
                else
                {
                    new Crewmate(other);
                }

                Role.RoleDictionary.Add(shifter.PlayerId, newRole);
                shifterRole.AddToRoleHistory(shifterRole.RoleType);
            }
            else
            {
                shifter.MurderPlayer(shifter);
            }
            
            if (swapTasks)
            {
                tasks1 = other.myTasks;
                taskinfos1 = other.Data.Tasks;
                tasks2 = shifter.myTasks;
                taskinfos2 = shifter.Data.Tasks;

                shifter.myTasks = tasks1;
                shifter.Data.Tasks = taskinfos1;
                other.myTasks = tasks2;
                other.Data.Tasks = taskinfos2;

                if (other.AmOwner) Coroutines.Start(ShowShift());

                if (snitch)
                {
                    var snitchRole = Role.GetRole<Snitch>(shifter);
                    snitchRole.ImpArrows.DestroyAll();
                    snitchRole.SnitchArrows.Clear();
                    snitchRole.ImpArrows.Clear();
                    CompleteTask.Postfix(shifter);
                    if (other.AmOwner)
                        foreach (var player in PlayerControl.AllPlayerControls)
                            player.name.Color("white");
                }

                if (resetShifter) shifterRole.RegenTask();
            }

            if (shifter.AmOwner || other.AmOwner)
            {

                foreach (var vigilanteRole in Role.GetRoles(RoleEnum.Vigilante))
                {
                    var vigilante = (Vigilante)vigilanteRole;
                    vigilante.LastKilled = DateTime.UtcNow;
                    vigilante.LastKilled = vigilante.LastKilled.AddSeconds(2.5f - CustomGameOptions.VigiKillCd);
                }

                DestroyableSingleton<HudManager>.Instance.KillButton.gameObject.SetActive(false);
                //DestroyableSingleton<HudManager>.Instance.KillButton.isActive = false;

                Lights.SetLights();
            }
        }
    }
}
