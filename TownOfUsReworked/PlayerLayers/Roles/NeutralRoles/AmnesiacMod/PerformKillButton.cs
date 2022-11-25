using HarmonyLib;
using Hazel;
using TownOfUsReworked.Extensions;
using TownOfUsReworked.PlayerLayers.Abilities;
using TownOfUsReworked.PlayerLayers.Abilities.SnitchMod;
using TownOfUsReworked.PlayerLayers.Roles.Roles;
using TownOfUsReworked.PlayerLayers.Roles.CrewRoles.InvestigatorMod;
using TownOfUsReworked.PlayerLayers.Roles.CrewRoles.OperativeMod;
using TownOfUsReworked.Enums;
using TownOfUsReworked.Lobby.CustomOption;
using TownOfUsReworked.Patches;
using UnityEngine;
using System;
using TownOfUsReworked.PlayerLayers.Abilities.Abilities;
using Il2CppSystem.Collections.Generic;

namespace TownOfUsReworked.PlayerLayers.Roles.NeutralRoles.AmnesiacMod
{
    [HarmonyPatch(typeof(KillButton), nameof(KillButton.DoClick))]
    public class PerformKillButton
    {
        public static Sprite Sprite => TownOfUsReworked.Arrow;
        
        public static bool Prefix(KillButton __instance)
        {
            if (__instance != DestroyableSingleton<HudManager>.Instance.KillButton)
                return true;

            var flag = PlayerControl.LocalPlayer.Is(RoleEnum.Amnesiac);

            if (!flag)
                return true;

            if (!PlayerControl.LocalPlayer.CanMove)
                return false;

            if (PlayerControl.LocalPlayer.Data.IsDead)
                return false;

            var role = Role.GetRole<Amnesiac>(PlayerControl.LocalPlayer);

            var flag2 = __instance.isCoolingDown;

            if (flag2)
                return false;

            if (!__instance.enabled)
                return false;

            var maxDistance = GameOptionsData.KillDistances[PlayerControl.GameOptions.KillDistance];

            if (role == null)
                return false;

            if (role.CurrentTarget == null)
                return false;

            if (Vector2.Distance(role.CurrentTarget.TruePosition, PlayerControl.LocalPlayer.GetTruePosition()) > maxDistance)
                return false;

            var playerId = role.CurrentTarget.ParentId;
            var player = Utils.PlayerById(playerId);

            if ((player.IsInfected() | role.Player.IsInfected()) && !player.Is(RoleEnum.Plaguebearer))
            {
                foreach (var pb in Role.GetRoles(RoleEnum.Plaguebearer))
                    ((Plaguebearer)pb).RpcSpreadInfection(player, role.Player);
            }

            unchecked
            {
                var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte) CustomRPC.Remember,
                    SendOption.Reliable, -1);
                writer.Write(PlayerControl.LocalPlayer.PlayerId);
                writer.Write(playerId);
                AmongUsClient.Instance.FinishRpcImmediately(writer);
            }
            
            try
            {
                SoundManager.Instance.PlaySound(TownOfUsReworked.RememberSound, false, 1f);
            } catch {}

            Remember(role, player);
            return false;
        }

        public static void Remember(Amnesiac amneRole, PlayerControl other)
        {
            var role = Role.GetRole(other);
            var roleType = role.RoleType;
            var objectifier = Utils.GetObjectifier(other);
            var ability = Utils.GetAbility(other);
            var modifier = Utils.GetModifier(other);
            var amnesiac = amneRole.Player;
            List<PlayerTask> tasks1, tasks2;
            List<GameData.TaskInfo> taskinfos1, taskinfos2;

            var rememberImp = true;
            var rememberNeut = true;
            var rememberSyn = true;
            var rememberCrew = true;

            Role newRole;

            if (PlayerControl.LocalPlayer == amnesiac)
            {
                var amnesiacRole = Role.GetRole<Amnesiac>(amnesiac);
                amnesiacRole.BodyArrows.Values.DestroyAll();
                amnesiacRole.BodyArrows.Clear();
                amnesiacRole.CurrentTarget.bodyRenderer.material.SetFloat("_Outline", 0f);
            }

            switch (roleType)
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
                case RoleEnum.Inspector:
                case RoleEnum.Escort:
                case RoleEnum.VampireHunter:

                    rememberImp = false;
                    rememberNeut = false;
                    rememberSyn = false;

                    break;

                case RoleEnum.Warper:
                case RoleEnum.Anarchist:
                case RoleEnum.Gorgon:
                case RoleEnum.Concealer:
                case RoleEnum.Rebel:
                case RoleEnum.Sidekick:
                case RoleEnum.Puppeteer:

                    rememberImp = false;
                    rememberNeut = false;
                    rememberCrew = false;

                    break;

                case RoleEnum.Jester:
                case RoleEnum.Executioner:
                case RoleEnum.Arsonist:
                case RoleEnum.Amnesiac:
                case RoleEnum.Glitch:
                case RoleEnum.Juggernaut:
                case RoleEnum.Murderer:
                case RoleEnum.Survivor:
                case RoleEnum.GuardianAngel:
                case RoleEnum.Plaguebearer:
                case RoleEnum.Pestilence:
                case RoleEnum.SerialKiller:
                case RoleEnum.Cannibal:
                case RoleEnum.Taskmaster:
                case RoleEnum.Werewolf:
                case RoleEnum.Troll:
                case RoleEnum.Thief:
                case RoleEnum.Dracula:
                case RoleEnum.Vampire:
                case RoleEnum.Dampyr:
                case RoleEnum.Cryomaniac:

                    rememberImp = false;
                    rememberSyn = false;
                    rememberCrew = false;

                    break;
            }

            if (roleType == RoleEnum.Investigator)
                Footprint.DestroyAll(Role.GetRole<Investigator>(other));

            newRole = Role.GetRole(other);
            newRole.Player = amnesiac;

            if (roleType == RoleEnum.Taskmaster)
                CompleteTask.Postfix(amnesiac);

            Role.RoleDictionary.Remove(amnesiac.PlayerId);

            if (rememberCrew)
            {
                new Crewmate(other);
                    
                if (CustomGameOptions.AmneTurnAssassin)
                {
                    var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.SetAssassin,
                        SendOption.Reliable, -1);
                    writer.Write(amnesiac.PlayerId);
                    AmongUsClient.Instance.FinishRpcImmediately(writer);
                }
            }
            else if (rememberSyn)
            {
                new Anarchist(other);

                foreach (var player in PlayerControl.AllPlayerControls)
                {
                    if (player.Is(Faction.Syndicate) && PlayerControl.LocalPlayer.Is(Faction.Syndicate))
                    {
                        var role2 = Role.GetRole(player);

                        if (CustomGameOptions.FactionSeeRoles)
                            player.nameText().color = role2.Color;
                        else
                            player.nameText().color = Colors.Syndicate;
                    }
                }

                if (CustomGameOptions.AmneTurnAssassin)
                {
                    var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.SetAssassin,
                        SendOption.Reliable, -1);
                    writer.Write(amnesiac.PlayerId);
                    AmongUsClient.Instance.FinishRpcImmediately(writer);
                }
            }
            else if (rememberNeut)
            {
                if (Lists.NeutralKillers.Contains(role))
                {
                    if (CustomGameOptions.AmneTurnAssassin)
                    {
                        var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.SetAssassin,
                            SendOption.Reliable, -1);
                        writer.Write(amnesiac.PlayerId);
                        AmongUsClient.Instance.FinishRpcImmediately(writer);
                    }
                }
                else
                    new Amnesiac(other);
            }
            else if (rememberImp)
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

                        if (CustomGameOptions.FactionSeeRoles)
                            player.nameText().color = role2.Color;
                        else
                            player.nameText().color = Colors.Intruder;
                    }
                }

                if (CustomGameOptions.AmneTurnAssassin)
                {
                    var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.SetAssassin,
                        SendOption.Reliable, -1);
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

            if (ability == AbilityEnum.Snitch)
            {
                var snitchRole = Ability.GetAbility<Snitch>(amnesiac);
                snitchRole.ImpArrows.DestroyAll();
                snitchRole.SnitchArrows.Values.DestroyAll();
                snitchRole.SnitchArrows.Clear();
                CompleteTask.Postfix(amnesiac);

                if (other.AmOwner)
                {
                    foreach (var player in PlayerControl.AllPlayerControls)
                        player.nameText().color = Color.white;
                }

                DestroyableSingleton<HudManager>.Instance.KillButton.gameObject.SetActive(false);
            }

            if (roleType == RoleEnum.Vigilante)
            {
                var vigilanteRole = Role.GetRole<Vigilante>(amnesiac);
                vigilanteRole.LastKilled = DateTime.UtcNow;
            }
            else if (roleType == RoleEnum.Engineer)
            {
                var engiRole = Role.GetRole<Engineer>(amnesiac);
                engiRole.UsedThisRound = false;
            }
            else if (roleType == RoleEnum.Medic)
            {
                var medicRole = Role.GetRole<Medic>(amnesiac);
                medicRole.UsedAbility = false;
            }
            else if (roleType == RoleEnum.Mayor)
            {
                var mayorRole = Role.GetRole<Mayor>(amnesiac);
                mayorRole.VoteBank = CustomGameOptions.MayorVoteBank;
                DestroyableSingleton<HudManager>.Instance.KillButton.gameObject.SetActive(false);
            }
            else if (roleType == RoleEnum.Veteran)
            {
                var vetRole = Role.GetRole<Veteran>(amnesiac);
                vetRole.UsesLeft = CustomGameOptions.MaxAlerts;
                vetRole.LastAlerted = DateTime.UtcNow;
            }
            else if (roleType == RoleEnum.Tracker)
            {
                var trackerRole = Role.GetRole<Tracker>(amnesiac);
                trackerRole.TrackerArrows.Values.DestroyAll();
                trackerRole.TrackerArrows.Clear();
                trackerRole.UsesLeft = CustomGameOptions.MaxTracks;
                trackerRole.LastTracked = DateTime.UtcNow;
            }
            else if (roleType == RoleEnum.Detective)
            {
                var detectiveRole = Role.GetRole<Detective>(amnesiac);
                detectiveRole.LastExamined = DateTime.UtcNow;
            }
            else if (roleType == RoleEnum.Coroner)
            {
                var coronerRole = Role.GetRole<Coroner>(amnesiac);
                coronerRole.BodyArrows.Values.DestroyAll();
                coronerRole.BodyArrows.Clear();
                DestroyableSingleton<HudManager>.Instance.KillButton.gameObject.SetActive(false);
            }
            else if (roleType == RoleEnum.TimeLord)
            {
                var tlRole = Role.GetRole<TimeLord>(amnesiac);
                tlRole.FinishRewind = DateTime.UtcNow;
                tlRole.StartRewind = DateTime.UtcNow;
                tlRole.StartRewind = tlRole.StartRewind.AddSeconds(-10.0f);
                tlRole.UsesLeft = CustomGameOptions.RewindMaxUses;
            }
            else if (roleType == RoleEnum.Transporter)
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
            else if (roleType == RoleEnum.Medium)
            {
                var medRole = Role.GetRole<Medium>(amnesiac);
                medRole.MediatedPlayers.Values.DestroyAll();
                medRole.MediatedPlayers.Clear();
                medRole.LastMediated = DateTime.UtcNow;
            }
            else if (roleType == RoleEnum.Werewolf)
            {
                var wwRole = Role.GetRole<Werewolf>(amnesiac);
                wwRole.LastMauled = DateTime.UtcNow;
            }
            else if (roleType == RoleEnum.Sheriff)
            {
                var sheriffRole = Role.GetRole<Sheriff>(amnesiac);
                sheriffRole.Interrogated.RemoveRange(0, sheriffRole.Interrogated.Count);
                sheriffRole.LastInterrogated = DateTime.UtcNow;
            }
            else if (roleType == RoleEnum.Arsonist)
            {
                var arsoRole = Role.GetRole<Arsonist>(amnesiac);
                arsoRole.DousedPlayers.RemoveRange(0, arsoRole.DousedPlayers.Count);
                arsoRole.LastDoused = DateTime.UtcNow;
            }
            else if (roleType == RoleEnum.Survivor)
            {
                var survRole = Role.GetRole<Survivor>(amnesiac);
                survRole.LastVested = DateTime.UtcNow;
                survRole.UsesLeft = CustomGameOptions.MaxVests;
            }
            else if (roleType == RoleEnum.GuardianAngel)
            {
                var gaRole = Role.GetRole<GuardianAngel>(amnesiac);
                gaRole.LastProtected = DateTime.UtcNow;
                gaRole.UsesLeft = CustomGameOptions.MaxProtects;
            }
            else if (roleType == RoleEnum.Glitch)
            {
                var glitchRole = Role.GetRole<Glitch>(amnesiac);
                glitchRole.LastKill = DateTime.UtcNow;
                glitchRole.LastHack = DateTime.UtcNow;
                glitchRole.LastMimic = DateTime.UtcNow;
            }
            else if (roleType == RoleEnum.Juggernaut)
            {
                var juggRole = Role.GetRole<Juggernaut>(amnesiac);
                juggRole.JuggKills = 0;
                juggRole.LastKill = DateTime.UtcNow;
            }
            else if (roleType == RoleEnum.Murderer)
            {
                var murdRole = Role.GetRole<Murderer>(amnesiac);
                murdRole.LastKill = DateTime.UtcNow;
            }
            else if (roleType == RoleEnum.Grenadier)
            {
                var grenadeRole = Role.GetRole<Grenadier>(amnesiac);
                grenadeRole.LastFlashed = DateTime.UtcNow;
            }
            else if (roleType == RoleEnum.Morphling)
            {
                var morphlingRole = Role.GetRole<Morphling>(amnesiac);
                morphlingRole.LastMorphed = DateTime.UtcNow;
            }
            else if (roleType == RoleEnum.Wraith)
            {
                var wraithRole = Role.GetRole<Wraith>(amnesiac);
                wraithRole.LastInvis = DateTime.UtcNow;
            }
            else if (roleType == RoleEnum.Blackmailer)
            {
                var blackmailerRole = Role.GetRole<Blackmailer>(amnesiac);
                blackmailerRole.LastBlackmailed = DateTime.UtcNow;
                blackmailerRole.Blackmailed = null;
            }
            else if (roleType == RoleEnum.Camouflager)
            {
                var camoRole = Role.GetRole<Camouflager>(amnesiac);
                camoRole.LastCamouflaged = DateTime.UtcNow;
            }
            else if (roleType == RoleEnum.Disguiser)
            {
                var disguiserRole = Role.GetRole<Disguiser>(amnesiac);
                disguiserRole.LastDisguised = DateTime.UtcNow;
            }
            else if (roleType == RoleEnum.Miner)
            {
                var minerRole = Role.GetRole<Miner>(amnesiac);
                minerRole.LastMined = DateTime.UtcNow;
            }
            else if (roleType == RoleEnum.Undertaker)
            {
                var dienerRole = Role.GetRole<Undertaker>(amnesiac);
                dienerRole.LastDragged = DateTime.UtcNow;
            }
            else if (roleType == RoleEnum.SerialKiller)
            {
                var skRole = Role.GetRole<SerialKiller>(amnesiac);
                skRole.LastLusted = DateTime.UtcNow;
                skRole.LastKilled = DateTime.UtcNow;
            }
            else if (roleType == RoleEnum.Plaguebearer)
            {
                var plagueRole = Role.GetRole<Plaguebearer>(amnesiac);
                plagueRole.InfectedPlayers.Add(amnesiac.PlayerId);
                plagueRole.LastInfected = DateTime.UtcNow;
            }
            else if (roleType == RoleEnum.Pestilence)
            {
                var pestRole = Role.GetRole<Pestilence>(amnesiac);
                pestRole.LastKill = DateTime.UtcNow;
            }
            else if (roleType == RoleEnum.Operative)
            {
                var opRole = Role.GetRole<Operative>(amnesiac);
                opRole.lastBugged = DateTime.UtcNow;
                opRole.UsesLeft = CustomGameOptions.MaxBugs;
                opRole.buggedPlayers.Clear();
                opRole.bugs.ClearBugs();
            }
            else if (roleType == RoleEnum.Cannibal)
            {
                var cannibalRole = Role.GetRole<Cannibal>(amnesiac);
                cannibalRole.LastEaten = DateTime.UtcNow;
            }
            else if (roleType == RoleEnum.Shifter)
            {
                var shifterRole = Role.GetRole<Shifter>(amnesiac);
                shifterRole.LastShifted = DateTime.UtcNow;
            }
            else if (roleType == RoleEnum.Consigliere)
            {
                var consigRole = Role.GetRole<Consigliere>(amnesiac);
                consigRole.LastInvestigated = DateTime.UtcNow;
            }
            else if (!(amnesiac.Is(RoleEnum.Altruist) | amnesiac.Is(RoleEnum.Amnesiac) | amnesiac.Is(Faction.Intruders)))
            {
                DestroyableSingleton<HudManager>.Instance.KillButton.gameObject.SetActive(false);
            }

            if (amnesiac.Is(Faction.Intruders) && (!amnesiac.Is(ObjectifierEnum.Traitor) | CustomGameOptions.SnitchSeesTraitor))
            {
                foreach (var snitch in Ability.GetAbilities(AbilityEnum.Snitch))
                {
                    var snitchRole = (Snitch)snitch;

                    if (snitchRole.TasksDone && PlayerControl.LocalPlayer.Is(AbilityEnum.Snitch))
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
        }
    }
}
