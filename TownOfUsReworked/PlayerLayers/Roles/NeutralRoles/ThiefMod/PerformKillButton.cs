using HarmonyLib;
using Hazel;
using TownOfUsReworked.Extensions;
using TownOfUsReworked.PlayerLayers.Roles.Roles;
using TownOfUsReworked.Enums;
using TownOfUsReworked.Lobby.CustomOption;
using TownOfUsReworked.Patches;
using System;

using Il2CppSystem.Collections.Generic;

namespace TownOfUsReworked.PlayerLayers.Roles.NeutralRoles.ThiefMod
{
    [HarmonyPatch(typeof(KillButton), nameof(KillButton.DoClick))]
    public class PerformKillButton
    {
        public static bool Prefix(KillButton __instance)
        {
            if (__instance != DestroyableSingleton<HudManager>.Instance.KillButton)
                return true;

            var flag = PlayerControl.LocalPlayer.Is(RoleEnum.Thief);

            if (!flag)
                return true;

            if (!PlayerControl.LocalPlayer.CanMove)
                return false;

            if (PlayerControl.LocalPlayer.Data.IsDead)
                return false;

            var role = Role.GetRole<Thief>(PlayerControl.LocalPlayer);

            var flag2 = __instance.isCoolingDown;

            if (flag2)
                return false;

            if (!__instance.enabled)
                return false;

            var maxDistance = GameOptionsData.KillDistances[CustomGameOptions.InteractionDistance];

            if (role == null)
                return false;

            var playerId = role.ClosestPlayer.PlayerId;
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
            
            //SoundManager.Instance.PlaySound(TownOfUsReworked.StealSound, false, 0.4f);

            Steal(role, player);
            return false;
        }

        public static void Steal(Thief thiefRole, PlayerControl other)
        {
            var role = Role.GetRole(other);
            var roleType = role.RoleType;
            var objectifier = Utils.GetObjectifier(other);
            var ability = Utils.GetAbility(other);
            var modifier = Utils.GetModifier(other);
            var thief = thiefRole.Player;
            List<PlayerTask> tasks1, tasks2;
            List<GameData.TaskInfo> taskinfos1, taskinfos2;

            var becomeImp = true;
            var becomeNeut = true;

            Role newRole;

            switch (roleType)
            {
                case RoleEnum.Vigilante:
                case RoleEnum.Veteran:
                case RoleEnum.VampireHunter:

                    becomeImp = false;
                    becomeNeut = false;

                    break;

                case RoleEnum.Arsonist:
                case RoleEnum.Glitch:
                case RoleEnum.Juggernaut:
                case RoleEnum.Murderer:
                case RoleEnum.Plaguebearer:
                case RoleEnum.Pestilence:
                case RoleEnum.SerialKiller:
                case RoleEnum.Werewolf:
                case RoleEnum.Troll:
                case RoleEnum.Thief:
                case RoleEnum.Dracula:
                case RoleEnum.Dampyr:

                    becomeImp = false;

                    break;
            }

            newRole = Role.GetRole(other);
            newRole.Player = thief;

            Role.RoleDictionary.Remove(thief.PlayerId);

            if (becomeImp == false)
            {
                if (becomeNeut == false)
                    new Crewmate(other);
            }
            else if (becomeImp == true)
            {
                new Impostor(other);
                thief.Data.Role.TeamType = RoleTeamTypes.Impostor;
                RoleManager.Instance.SetRole(thief, RoleTypes.Impostor);
                thief.SetKillTimer(PlayerControl.GameOptions.KillCooldown);

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
                    var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.SetAssassin, SendOption.Reliable, -1);
                    writer.Write(thief.PlayerId);
                    AmongUsClient.Instance.FinishRpcImmediately(writer);
                }

                if (thief.Is(RoleEnum.Poisoner))
                {
                    if (PlayerControl.LocalPlayer == thief)
                    {
                        var poisonerRole = Role.GetRole<Poisoner>(thief);
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
            tasks2 = thief.myTasks;
            taskinfos2 = thief.Data.Tasks;

            thief.myTasks = tasks1;
            thief.Data.Tasks = taskinfos1;
            other.myTasks = tasks2;
            other.Data.Tasks = taskinfos2;

            if (roleType == RoleEnum.Vigilante)
            {
                var vigilanteRole = Role.GetRole<Vigilante>(thief);
                vigilanteRole.LastKilled = DateTime.UtcNow;
            }
            else if (roleType == RoleEnum.Veteran)
            {
                var vetRole = Role.GetRole<Veteran>(thief);
                vetRole.UsesLeft = CustomGameOptions.MaxAlerts;
                vetRole.LastAlerted = DateTime.UtcNow;
            }
            else if (roleType == RoleEnum.Arsonist)
            {
                var arsoRole = Role.GetRole<Arsonist>(thief);
                arsoRole.DousedPlayers.RemoveRange(0, arsoRole.DousedPlayers.Count);
                arsoRole.LastDoused = DateTime.UtcNow;
            }
            else if (roleType == RoleEnum.Glitch)
            {
                var glitchRole = Role.GetRole<Glitch>(thief);
                glitchRole.LastKill = DateTime.UtcNow;
                glitchRole.LastHack = DateTime.UtcNow;
                glitchRole.LastMimic = DateTime.UtcNow;
            }
            else if (roleType == RoleEnum.Juggernaut)
            {
                var juggRole = Role.GetRole<Juggernaut>(thief);
                juggRole.JuggKills = 0;
                juggRole.LastKill = DateTime.UtcNow;
            }
            else if (roleType == RoleEnum.Murderer)
            {
                var murdRole = Role.GetRole<Murderer>(thief);
                murdRole.LastKill = DateTime.UtcNow;
            }
            else if (roleType == RoleEnum.Werewolf)
            {
                var wwRole = Role.GetRole<Werewolf>(thief);
                wwRole.LastMauled = DateTime.UtcNow;
            }
            else if (roleType == RoleEnum.Grenadier)
            {
                var grenadeRole = Role.GetRole<Grenadier>(thief);
                grenadeRole.LastFlashed = DateTime.UtcNow;
            }
            else if (roleType == RoleEnum.Morphling)
            {
                var morphlingRole = Role.GetRole<Morphling>(thief);
                morphlingRole.LastMorphed = DateTime.UtcNow;
            }
            else if (roleType == RoleEnum.Wraith)
            {
                var wraithRole = Role.GetRole<Wraith>(thief);
                wraithRole.LastInvis = DateTime.UtcNow;
            }
            else if (roleType == RoleEnum.Blackmailer)
            {
                var blackmailerRole = Role.GetRole<Blackmailer>(thief);
                blackmailerRole.LastBlackmailed = DateTime.UtcNow;
                blackmailerRole.Blackmailed = null;
            }
            else if (roleType == RoleEnum.Camouflager)
            {
                var camoRole = Role.GetRole<Camouflager>(thief);
                camoRole.LastCamouflaged = DateTime.UtcNow;
            }
            else if (roleType == RoleEnum.Disguiser)
            {
                var disguiserRole = Role.GetRole<Disguiser>(thief);
                disguiserRole.LastDisguised = DateTime.UtcNow;
            }
            else if (roleType == RoleEnum.Miner)
            {
                var minerRole = Role.GetRole<Miner>(thief);
                minerRole.LastMined = DateTime.UtcNow;
            }
            else if (roleType == RoleEnum.Undertaker)
            {
                var dienerRole = Role.GetRole<Undertaker>(thief);
                dienerRole.LastDragged = DateTime.UtcNow;
            }
            else if (roleType == RoleEnum.SerialKiller)
            {
                var skRole = Role.GetRole<SerialKiller>(thief);
                skRole.LastLusted = DateTime.UtcNow;
                skRole.LastKilled = DateTime.UtcNow;
            }
            else if (roleType == RoleEnum.Plaguebearer)
            {
                var plagueRole = Role.GetRole<Plaguebearer>(thief);
                plagueRole.InfectedPlayers.Add(thief.PlayerId);
                plagueRole.LastInfected = DateTime.UtcNow;
            }
            else if (roleType == RoleEnum.Pestilence)
            {
                var pestRole = Role.GetRole<Pestilence>(thief);
                pestRole.LastKill = DateTime.UtcNow;
            }
            else if (roleType == RoleEnum.Consigliere)
            {
                var consigRole = Role.GetRole<Consigliere>(thief);
                consigRole.LastInvestigated = DateTime.UtcNow;
            }
        }
    }
}
