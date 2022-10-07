using HarmonyLib;
using Hazel;
using TownOfUs.CrewmateRoles.InvestigatorMod;
using TownOfUs.CrewmateRoles.SnitchMod;
using TownOfUs.CrewmateRoles.OperativeMod;
using TownOfUs.Roles;
using UnityEngine;
using System;
using Il2CppSystem.Collections.Generic;
using TownOfUs.Extensions;

namespace TownOfUs.NeutralRoles.AmnesiacMod
{
    [HarmonyPatch(typeof(KillButton), nameof(KillButton.DoClick))]
    public class PerformKillButton
    {
        public static Sprite Sprite => TownOfUs.Arrow;
        
        public static bool Prefix(KillButton __instance)
        {
            if (__instance != DestroyableSingleton<HudManager>.Instance.KillButton) return true;
            var flag = PlayerControl.LocalPlayer.Is(RoleEnum.Amnesiac);
            if (!flag) return true;
            if (!PlayerControl.LocalPlayer.CanMove) return false;
            if (PlayerControl.LocalPlayer.Data.IsDead) return false;
            var role = Role.GetRole<Amnesiac>(PlayerControl.LocalPlayer);

            var flag2 = __instance.isCoolingDown;
            if (flag2) return false;
            if (!__instance.enabled) return false;
            var maxDistance = GameOptionsData.KillDistances[PlayerControl.GameOptions.KillDistance];
            if (role == null)
                return false;
            if (role.CurrentTarget == null)
                return false;
            if (Vector2.Distance(role.CurrentTarget.TruePosition,
                PlayerControl.LocalPlayer.GetTruePosition()) > maxDistance) return false;
            var playerId = role.CurrentTarget.ParentId;
            var player = Utils.PlayerById(playerId);
            if ((player.IsInfected() || role.Player.IsInfected()) && !player.Is(RoleEnum.Plaguebearer))
            {
                foreach (var pb in Role.GetRoles(RoleEnum.Plaguebearer)) ((Plaguebearer)pb).RpcSpreadInfection(player, role.Player);
            }

            var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId,
                (byte) CustomRPC.Remember, SendOption.Reliable, -1);
            writer.Write(PlayerControl.LocalPlayer.PlayerId);
            writer.Write(playerId);
            AmongUsClient.Instance.FinishRpcImmediately(writer);

            try {
                AudioClip RememberSFX = TownOfUs.loadAudioClipFromResources("TownOfUs.Resources.Remember.raw");
                SoundManager.Instance.PlaySound(RememberSFX, false, 0.4f);
            } catch {
            }

            Remember(role, player);
            return false;
        }

        public static void Remember(Amnesiac amneRole, PlayerControl other)
        {
            var role = Utils.GetRole(other);
            var amnesiac = amneRole.Player;
            List<PlayerTask> tasks1, tasks2;
            List<GameData.TaskInfo> taskinfos1, taskinfos2;

            var rememberImp = true;
            var rememberNeut = true;

            Role newRole;

            if (PlayerControl.LocalPlayer == amnesiac)
            {
                var amnesiacRole = Role.GetRole<Amnesiac>(amnesiac);
                amnesiacRole.BodyArrows.Values.DestroyAll();
                amnesiacRole.BodyArrows.Clear();
                amnesiacRole.CurrentTarget.bodyRenderer.material.SetFloat("_Outline", 0f);
            }

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

                    rememberImp = false;
                    rememberNeut = false;

                    break;

                case RoleEnum.Jester:
                case RoleEnum.Executioner:
                case RoleEnum.Arsonist:
                case RoleEnum.Amnesiac:
                case RoleEnum.Glitch:
                case RoleEnum.Juggernaut:
                case RoleEnum.Survivor:
                case RoleEnum.GuardianAngel:
                case RoleEnum.Plaguebearer:
                case RoleEnum.Pestilence:
                case RoleEnum.Werewolf:
                case RoleEnum.Cannibal:
                case RoleEnum.Taskmaster:

                    rememberImp = false;

                    break;
            }

            if (role == RoleEnum.Investigator) Footprint.DestroyAll(Role.GetRole<Investigator>(other));

            newRole = Role.GetRole(other);
            newRole.Player = amnesiac;

            if (role == RoleEnum.Snitch || role == RoleEnum.Taskmaster) CompleteTask.Postfix(amnesiac);

            Role.RoleDictionary.Remove(amnesiac.PlayerId);
            if (!(role == RoleEnum.Haunter || role == RoleEnum.Phantom))
            {
                Role.RoleDictionary.Add(amnesiac.PlayerId, newRole);
                Role.RoleDictionary.Add(amnesiac.PlayerId, newRole);
                newRole.AddToRoleHistory(newRole.RoleType);
            }
            else
            {
                new Crewmate(amnesiac);
            }

            if (rememberImp == false && !(role == RoleEnum.Haunter || role == RoleEnum.Phantom))
            {
                if (rememberNeut == false)
                {
                    new Crewmate(other);
                    if (CustomGameOptions.AmneTurnCrewAssassin)
                    {
                        var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.SetAssassin, SendOption.Reliable, -1);
                        writer.Write(amnesiac.PlayerId);
                        AmongUsClient.Instance.FinishRpcImmediately(writer);
                    }
                }
                else
                {
                    if (role == RoleEnum.Arsonist || role == RoleEnum.Glitch || role == RoleEnum.Plaguebearer || role == RoleEnum.Pestilence || role == RoleEnum.Werewolf || role == RoleEnum.Juggernaut)
                    {
                        if (!(role == RoleEnum.Arsonist || role == RoleEnum.Glitch || role == RoleEnum.Plaguebearer || role == RoleEnum.Pestilence || role == RoleEnum.Werewolf || role == RoleEnum.Juggernaut))
                        {
                            new Amnesiac(other);
                        }
                        else
                        {
                            if (CustomGameOptions.AmneTurnNeutAssassin)
                            {
                                var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.SetAssassin, SendOption.Reliable, -1);
                                writer.Write(amnesiac.PlayerId);
                                AmongUsClient.Instance.FinishRpcImmediately(writer);
                            }
                        }
                    }
                }
            }
            else if (rememberImp == true)
            {
                new Impostor(other);
                amnesiac.Data.Role.TeamType = RoleTeamTypes.Impostor;
                RoleManager.Instance.SetRole(amnesiac, RoleTypes.Impostor);
                amnesiac.SetKillTimer(PlayerControl.GameOptions.KillCooldown);
                foreach (var player in PlayerControl.AllPlayerControls)
                {
                    if (player.Data.IsImpostor() && PlayerControl.LocalPlayer.Data.IsImpostor())
                    {
                        var role2 = Role.GetRole(player);
                        if (CustomGameOptions.ImpostorSeeRoles)
                            player.nameText().color = role2.Color;
                        else
                            player.nameText().color = Patches.Colors.Impostor;
                    }
                }
                if (CustomGameOptions.AmneTurnImpAssassin)
                {
                    var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.SetAssassin, SendOption.Reliable, -1);
                    writer.Write(amnesiac.PlayerId);
                    AmongUsClient.Instance.FinishRpcImmediately(writer);
                }
                if (amnesiac.Is(RoleEnum.Poisoner))
                {
                    if (PlayerControl.LocalPlayer == amnesiac)
                    {
                        var poisonerRole = Role.GetRole<Poisoner>(amnesiac);
                        poisonerRole.LastPoisoned = DateTime.UtcNow;
                        DestroyableSingleton<HudManager>.Instance.KillButton.graphic.enabled = false;
                    }
                    else if (PlayerControl.LocalPlayer == other)
                    {
                        DestroyableSingleton<HudManager>.Instance.KillButton.enabled = true;
                        DestroyableSingleton<HudManager>.Instance.KillButton.graphic.enabled = true;
                    }
                }
            }

            tasks1 = other.myTasks;
            taskinfos1 = other.Data.Tasks;
            tasks2 = amnesiac.myTasks;
            taskinfos2 = amnesiac.Data.Tasks;

            amnesiac.myTasks = tasks1;
            amnesiac.Data.Tasks = taskinfos1;
            other.myTasks = tasks2;
            other.Data.Tasks = taskinfos2;

            if (role == RoleEnum.Snitch)
            {
                var snitchRole = Role.GetRole<Snitch>(amnesiac);
                snitchRole.ImpArrows.DestroyAll();
                snitchRole.SnitchArrows.Values.DestroyAll();
                snitchRole.SnitchArrows.Clear();
                CompleteTask.Postfix(amnesiac);
                if (other.AmOwner)
                    foreach (var player in PlayerControl.AllPlayerControls)
                        player.nameText().color = Color.white;
                DestroyableSingleton<HudManager>.Instance.KillButton.gameObject.SetActive(false);
            }
            else if (role == RoleEnum.Vigilante)
            {
                var vigilanteRole = Role.GetRole<Vigilante>(amnesiac);
                vigilanteRole.LastKilled = DateTime.UtcNow;
            }
            else if (role == RoleEnum.Engineer)
            {
                var engiRole = Role.GetRole<Engineer>(amnesiac);
                engiRole.UsedThisRound = false;
            }
            else if (role == RoleEnum.Medic)
            {
                var medicRole = Role.GetRole<Medic>(amnesiac);
                medicRole.UsedAbility = false;
            }
            else if (role == RoleEnum.Mayor)
            {
                var mayorRole = Role.GetRole<Mayor>(amnesiac);
                mayorRole.VoteBank = CustomGameOptions.MayorVoteBank;
                DestroyableSingleton<HudManager>.Instance.KillButton.gameObject.SetActive(false);
            }
            else if (role == RoleEnum.Veteran)
            {
                var vetRole = Role.GetRole<Veteran>(amnesiac);
                vetRole.UsesLeft = CustomGameOptions.MaxAlerts;
                vetRole.LastAlerted = DateTime.UtcNow;
            }
            else if (role == RoleEnum.Tracker)
            {
                var trackerRole = Role.GetRole<Tracker>(amnesiac);
                trackerRole.TrackerArrows.Values.DestroyAll();
                trackerRole.TrackerArrows.Clear();
                trackerRole.UsesLeft = CustomGameOptions.MaxTracks;
                trackerRole.LastTracked = DateTime.UtcNow;
            }
            else if (role == RoleEnum.Detective)
            {
                var detectiveRole = Role.GetRole<Detective>(amnesiac);
                detectiveRole.LastExamined = DateTime.UtcNow;
            }
            else if (role == RoleEnum.Mystic)
            {
                var mysticRole = Role.GetRole<Mystic>(amnesiac);
                mysticRole.BodyArrows.Values.DestroyAll();
                mysticRole.BodyArrows.Clear();
                DestroyableSingleton<HudManager>.Instance.KillButton.gameObject.SetActive(false);
            }
            else if (role == RoleEnum.TimeLord)
            {
                var tlRole = Role.GetRole<TimeLord>(amnesiac);
                tlRole.FinishRewind = DateTime.UtcNow;
                tlRole.StartRewind = DateTime.UtcNow;
                tlRole.StartRewind = tlRole.StartRewind.AddSeconds(-10.0f);
                tlRole.UsesLeft = CustomGameOptions.RewindMaxUses;
            }
            else if (role == RoleEnum.Transporter)
            {
                var tpRole = Role.GetRole<Transporter>(amnesiac);
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
                var medRole = Role.GetRole<Medium>(amnesiac);
                medRole.MediatedPlayers.Values.DestroyAll();
                medRole.MediatedPlayers.Clear();
                medRole.LastMediated = DateTime.UtcNow;
            }
            else if (role == RoleEnum.Sheriff)
            {
                var sheriffRole = Role.GetRole<Sheriff>(amnesiac);
                sheriffRole.Interrogated.RemoveRange(0, sheriffRole.Interrogated.Count);
                sheriffRole.LastInterrogated = DateTime.UtcNow;
            }
            else if (role == RoleEnum.Arsonist)
            {
                var arsoRole = Role.GetRole<Arsonist>(amnesiac);
                arsoRole.DousedPlayers.RemoveRange(0, arsoRole.DousedPlayers.Count);
                arsoRole.LastDoused = DateTime.UtcNow;
            }
            else if (role == RoleEnum.Survivor)
            {
                var survRole = Role.GetRole<Survivor>(amnesiac);
                survRole.LastVested = DateTime.UtcNow;
                survRole.UsesLeft = CustomGameOptions.MaxVests;
            }
            else if (role == RoleEnum.GuardianAngel)
            {
                var gaRole = Role.GetRole<GuardianAngel>(amnesiac);
                gaRole.LastProtected = DateTime.UtcNow;
                gaRole.UsesLeft = CustomGameOptions.MaxProtects;
            }
            else if (role == RoleEnum.Glitch)
            {
                var glitchRole = Role.GetRole<Glitch>(amnesiac);
                glitchRole.LastKill = DateTime.UtcNow;
                glitchRole.LastHack = DateTime.UtcNow;
                glitchRole.LastMimic = DateTime.UtcNow;
            }
            else if (role == RoleEnum.Juggernaut)
            {
                var juggRole = Role.GetRole<Juggernaut>(amnesiac);
                juggRole.JuggKills = 0;
                juggRole.LastKill = DateTime.UtcNow;
            }
            else if (role == RoleEnum.Grenadier)
            {
                var grenadeRole = Role.GetRole<Grenadier>(amnesiac);
                grenadeRole.LastFlashed = DateTime.UtcNow;
            }
            else if (role == RoleEnum.Morphling)
            {
                var morphlingRole = Role.GetRole<Morphling>(amnesiac);
                morphlingRole.LastMorphed = DateTime.UtcNow;
            }
            else if (role == RoleEnum.Wraith)
            {
                var wraithRole = Role.GetRole<Wraith>(amnesiac);
                wraithRole.LastInvis = DateTime.UtcNow;
            }
            else if (role == RoleEnum.Blackmailer)
            {
                var blackmailerRole = Role.GetRole<Blackmailer>(amnesiac);
                blackmailerRole.LastBlackmailed = DateTime.UtcNow;
                blackmailerRole.Blackmailed = null;
            }
            else if (role == RoleEnum.Disguiser)
            {
                var disguiserRole = Role.GetRole<Disguiser>(amnesiac);
                disguiserRole.LastDisguised = DateTime.UtcNow;
            }
            else if (role == RoleEnum.Miner)
            {
                var minerRole = Role.GetRole<Miner>(amnesiac);
                minerRole.LastMined = DateTime.UtcNow;
            }
            else if (role == RoleEnum.Undertaker)
            {
                var dienerRole = Role.GetRole<Undertaker>(amnesiac);
                dienerRole.LastDragged = DateTime.UtcNow;
            }
            else if (role == RoleEnum.Werewolf)
            {
                var wwRole = Role.GetRole<Werewolf>(amnesiac);
                wwRole.LastRampaged = DateTime.UtcNow;
                wwRole.LastKilled = DateTime.UtcNow;
            }
            else if (role == RoleEnum.Plaguebearer)
            {
                var plagueRole = Role.GetRole<Plaguebearer>(amnesiac);
                plagueRole.InfectedPlayers.RemoveRange(0, plagueRole.InfectedPlayers.Count);
                plagueRole.InfectedPlayers.Add(amnesiac.PlayerId);
                plagueRole.LastInfected = DateTime.UtcNow;
            }
            else if (role == RoleEnum.Pestilence)
            {
                var pestRole = Role.GetRole<Pestilence>(amnesiac);
                pestRole.LastKill = DateTime.UtcNow;
            }
            else if (role == RoleEnum.Operative)
            {
                var opRole = Role.GetRole<Operative>(amnesiac);
                opRole.lastBugged = DateTime.UtcNow;
                opRole.UsesLeft = CustomGameOptions.MaxBugs;
                opRole.buggedPlayers.Clear();
                opRole.bugs.ClearBugs();
            }
            else if (role == RoleEnum.Cannibal)
            {
                var cannibalRole = Role.GetRole<Cannibal>(amnesiac);
                cannibalRole.LastEaten = DateTime.UtcNow;
            }
            else if (role == RoleEnum.Shifter)
            {
                var shifterRole = Role.GetRole<Shifter>(amnesiac);
                shifterRole.LastShifted = DateTime.UtcNow;
            }
            else if (role == RoleEnum.Consigliere)
            {
                var consigRole = Role.GetRole<Consigliere>(amnesiac);
                consigRole.LastInvestigated = DateTime.UtcNow;
            }
            else if (!(amnesiac.Is(RoleEnum.Altruist) || amnesiac.Is(RoleEnum.Amnesiac) || amnesiac.Is(Faction.Intruders)))
            {
                DestroyableSingleton<HudManager>.Instance.KillButton.gameObject.SetActive(false);
            }

            if (amnesiac.Is(Faction.Intruders) && (!amnesiac.Is(RoleEnum.Traitor) || CustomGameOptions.SnitchSeesTraitor))
            {
                foreach (var snitch in Role.GetRoles(RoleEnum.Snitch))
                {
                    var snitchRole = (Snitch)snitch;
                    if (snitchRole.TasksDone && PlayerControl.LocalPlayer.Is(RoleEnum.Snitch))
                    {
                        var gameObj = new GameObject();
                        var arrow = gameObj.AddComponent<ArrowBehaviour>();
                        gameObj.transform.parent = PlayerControl.LocalPlayer.gameObject.transform;
                        var renderer = gameObj.AddComponent<SpriteRenderer>();
                        renderer.sprite = Sprite;
                        arrow.image = renderer;
                        gameObj.layer = 5;
                        snitchRole.SnitchArrows.Add(amnesiac.PlayerId, arrow);
                    }
                    else if (snitchRole.Revealed && PlayerControl.LocalPlayer == amnesiac)
                    {
                        var gameObj = new GameObject();
                        var arrow = gameObj.AddComponent<ArrowBehaviour>();
                        gameObj.transform.parent = PlayerControl.LocalPlayer.gameObject.transform;
                        var renderer = gameObj.AddComponent<SpriteRenderer>();
                        renderer.sprite = Sprite;
                        arrow.image = renderer;
                        gameObj.layer = 5;
                        snitchRole.ImpArrows.Add(arrow);
                    }
                }
            }

            if (other.Is(RoleEnum.Crewmate))
            {
                var role2 = Role.GetRole<Crewmate>(other);
                role2.RegenTask();
            }
            else if (other.Is(RoleEnum.Survivor))
            {
                var role2 = Role.GetRole<Survivor>(other);
                role2.RegenTask();
            }
            else
            {
                var role2 = Role.GetRole<Impostor>(other);
                role2.RegenTask();
            }
            
            Lights.SetLights();
        }
    }
}
