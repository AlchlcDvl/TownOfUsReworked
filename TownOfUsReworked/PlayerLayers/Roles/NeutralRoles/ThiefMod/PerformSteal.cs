using HarmonyLib;
using Hazel;
using TownOfUsReworked.Classes;
using TownOfUsReworked.PlayerLayers.Abilities;
using TownOfUsReworked.Enums;
using TownOfUsReworked.CustomOptions;
using UnityEngine;
using System;
using TownOfUsReworked.Extensions;

namespace TownOfUsReworked.PlayerLayers.Roles.NeutralRoles.ThiefMod
{
    [HarmonyPatch(typeof(AbilityButton), nameof(AbilityButton.DoClick))]
    public static class PerformSteal
    {
        public static bool Prefix(AbilityButton __instance)
        {
            if (Utils.NoButton(PlayerControl.LocalPlayer, RoleEnum.Thief))
                return true;

            var role = Role.GetRole<Thief>(PlayerControl.LocalPlayer);

            if (__instance == role.StealButton)
            {
                if (Utils.IsTooFar(role.Player, role.ClosestPlayer))
                    return false;

                if (role.KillTimer() != 0f)
                    return false;

                var interact = Utils.Interact(role.Player, role.ClosestPlayer, true);

                if (interact[3])
                {
                    if (!(role.ClosestPlayer.Is(Faction.Intruder) || role.ClosestPlayer.Is(Faction.Syndicate) || role.ClosestPlayer.Is(RoleAlignment.NeutralKill) ||
                        role.ClosestPlayer.Is(RoleAlignment.NeutralNeo) || role.ClosestPlayer.Is(RoleAlignment.NeutralPros) || role.ClosestPlayer.Is(RoleAlignment.CrewKill)))
                        Utils.RpcMurderPlayer(role.Player, role.Player);
                    else
                    {
                        var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.Action, SendOption.Reliable);
                        writer.Write((byte)ActionsRPC.Steal);
                        writer.Write(PlayerControl.LocalPlayer.PlayerId);
                        writer.Write(role.ClosestPlayer.PlayerId);
                        AmongUsClient.Instance.FinishRpcImmediately(writer);
                        //SoundManager.Instance.PlaySound(TownOfUsReworked.StealSound, false, 0.4f);
                        Utils.RpcMurderPlayer(role.Player, role.ClosestPlayer);
                        Steal(role, role.ClosestPlayer);
                    }
                }

                if (interact[0])
                    role.LastStolen = DateTime.UtcNow;
                else if (interact[1])
                    role.LastStolen.AddSeconds(CustomGameOptions.ProtectKCReset);
                else if (interact[2])
                    role.LastStolen.AddSeconds(CustomGameOptions.VestKCReset);

                return false;
            }

            return true;
        }

        public static void Steal(Thief thiefRole, PlayerControl other)
        {
            var role = Role.GetRole(other);
            var thief = thiefRole.Player;
            var target = other.GetTarget();
            var leader = other.GetLeader();

            Role newRole = role.RoleType switch
            {
                RoleEnum.Anarchist => new Anarchist(thief),
                RoleEnum.Arsonist => new Arsonist(thief) { DousedPlayers = ((Arsonist)role).DousedPlayers },
                RoleEnum.Blackmailer => new Blackmailer(thief),
                RoleEnum.Bomber => new Bomber(thief),
                RoleEnum.Camouflager => new Camouflager(thief),
                RoleEnum.Concealer => new Concealer(thief),
                RoleEnum.Consigliere => new Consigliere(thief) { Investigated = ((Consigliere)role).Investigated },
                RoleEnum.Consort => new Consort(thief),
                RoleEnum.Cryomaniac => new Cryomaniac(thief) { DousedPlayers = ((Cryomaniac)role).DousedPlayers },
                RoleEnum.Disguiser => new Disguiser(thief),
                RoleEnum.Dracula => new Dracula(thief) { Converted = ((Dracula)role).Converted },
                RoleEnum.Framer => new Framer(thief),
                RoleEnum.Glitch => new Glitch(thief),
                RoleEnum.Godfather => new Godfather(thief),
                RoleEnum.Gorgon => new Gorgon(thief),
                RoleEnum.Grenadier => new Grenadier(thief),
                RoleEnum.Impostor => new Impostor(thief),
                RoleEnum.Juggernaut => new Juggernaut(thief) { JuggKills = ((Juggernaut)role).JuggKills },
                RoleEnum.Mafioso => new Mafioso(thief) { Godfather = (Godfather)leader },
                RoleEnum.Miner => new Miner(thief),
                RoleEnum.Morphling => new Morphling(thief),
                RoleEnum.Rebel => new Rebel(thief),
                RoleEnum.Sidekick => new Sidekick(thief) { Rebel = (Rebel)leader },
                RoleEnum.Shapeshifter => new Shapeshifter(thief),
                RoleEnum.Murderer => new Murderer(thief),
                RoleEnum.Plaguebearer => new Plaguebearer(thief) { InfectedPlayers = ((Plaguebearer)role).InfectedPlayers },
                RoleEnum.Pestilence => new Pestilence(thief),
                RoleEnum.SerialKiller => new SerialKiller(thief),
                RoleEnum.Werewolf => new Werewolf(thief),
                RoleEnum.Janitor => new Janitor(thief),
                RoleEnum.Poisoner => new Poisoner(thief),
                RoleEnum.Teleporter => new Teleporter(thief),
                RoleEnum.TimeMaster => new TimeMaster(thief),
                RoleEnum.Undertaker => new Undertaker(thief),
                RoleEnum.VampireHunter => new VampireHunter(thief),
                RoleEnum.Veteran => new Veteran(thief),
                RoleEnum.Vigilante => new Vigilante(thief),
                RoleEnum.Warper => new Warper(thief),
                RoleEnum.Wraith => new Wraith(thief),
                RoleEnum.BountyHunter => new BountyHunter(thief) { TargetPlayer = target },
                RoleEnum.Jackal => new Jackal(thief)
                {
                    Recruited = ((Jackal)role).Recruited,
                    EvilRecruit = ((Jackal)role).EvilRecruit,
                    GoodRecruit = ((Jackal)role).GoodRecruit,
                    BackupRecruit = ((Jackal)role).BackupRecruit
                },
                RoleEnum.Necromancer => new Necromancer(thief)
                {
                    Resurrected = ((Necromancer)role).Resurrected,
                    KillCount = ((Necromancer)role).KillCount,
                    ResurrectedCount = ((Necromancer)role).ResurrectedCount
                },
                RoleEnum.Whisperer => new Whisperer(thief)
                {
                    Persuaded = ((Whisperer)role).Persuaded,
                    WhisperCount = ((Whisperer)role).WhisperCount,
                    WhisperConversion = ((Whisperer)role).WhisperConversion
                },
                RoleEnum.Betrayer => new Betrayer(thief),
                RoleEnum.Ambusher => new Ambusher(thief),
                RoleEnum.Beamer => new Beamer(thief),
                RoleEnum.Crusader => new Crusader(thief),
                RoleEnum.Drunkard => new Drunkard(thief),
                _ => new Thief(thief),
            };

            newRole.RoleUpdate(thiefRole);

            if (other.Is(RoleEnum.Dracula))
                ((Dracula)role).Converted.Clear();
            else if (other.Is(RoleEnum.Whisperer))
                ((Whisperer)role).Persuaded.Clear();
            else if (other.Is(RoleEnum.Necromancer))
                ((Necromancer)role).Resurrected.Clear();
            else if (other.Is(RoleEnum.Jackal))
            {
                ((Jackal)role).Recruited.Clear();
                ((Jackal)role).EvilRecruit = null;
                ((Jackal)role).GoodRecruit = null;
                ((Jackal)role).BackupRecruit = null;
            }

            if (thief.Is(Faction.Intruder) || (thief.Is(Faction.Syndicate) && CustomGameOptions.AltImps))
                thief.Data.Role.TeamType = RoleTeamTypes.Impostor;

            if (CustomGameOptions.ThiefSteals)
            {
                var newRole2 = new Thief(other);
                newRole2.RoleUpdate(role);

                if (PlayerControl.LocalPlayer == other && other.Is(Faction.Intruder))
                {
                    HudManager.Instance.SabotageButton.gameObject.SetActive(false);
                    other.Data.Role.TeamType = RoleTeamTypes.Crewmate;
                }
            }

            if (thief.Is(Faction.Intruder) || thief.Is(Faction.Syndicate))
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
                        renderer.sprite = AssetManager.Arrow;
                        arrow.image = renderer;
                        gameObj.layer = 5;
                        snitchRole.SnitchArrows.Add(thief.PlayerId, arrow);
                    }
                    else if (snitchRole.TasksDone && PlayerControl.LocalPlayer == thief)
                    {
                        var gameObj = new GameObject();
                        var arrow = gameObj.AddComponent<ArrowBehaviour>();
                        gameObj.transform.parent = PlayerControl.LocalPlayer.gameObject.transform;
                        var renderer = gameObj.AddComponent<SpriteRenderer>();
                        renderer.sprite = AssetManager.Arrow;
                        arrow.image = renderer;
                        gameObj.layer = 5;
                        snitchRole.ImpArrows.Add(arrow);
                    }
                }
            }

            thiefRole.StealButton.gameObject.SetActive(false);
        }
    }
}