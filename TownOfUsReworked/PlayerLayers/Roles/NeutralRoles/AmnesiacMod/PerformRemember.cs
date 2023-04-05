using HarmonyLib;
using Hazel;
using TownOfUsReworked.Classes;
using TownOfUsReworked.PlayerLayers.Abilities;
using TownOfUsReworked.CustomOptions;
using UnityEngine;
using TownOfUsReworked.Data;
using TownOfUsReworked.Extensions;
using TownOfUsReworked.Modules;

namespace TownOfUsReworked.PlayerLayers.Roles.NeutralRoles.AmnesiacMod
{
    [HarmonyPatch(typeof(AbilityButton), nameof(AbilityButton.DoClick))]
    public static class PerformRemember
    {
        public static bool Prefix(AbilityButton __instance)
        {
            if (Utils.NoButton(PlayerControl.LocalPlayer, RoleEnum.Amnesiac))
                return true;

            var role = Role.GetRole<Amnesiac>(PlayerControl.LocalPlayer);

            if (__instance == role.RememberButton)
            {
                if (Utils.IsTooFar(role.Player, role.CurrentTarget))
                    return false;

                var playerId = role.CurrentTarget.ParentId;
                var player = Utils.PlayerById(playerId);
                Utils.Spread(role.Player, player);
                var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.Action, SendOption.Reliable);
                writer.Write((byte)ActionsRPC.Remember);
                writer.Write(PlayerControl.LocalPlayer.PlayerId);
                writer.Write(playerId);
                AmongUsClient.Instance.FinishRpcImmediately(writer);
                Remember(role, player);
                return false;
            }

            return true;
        }

        public static void Remember(Amnesiac amneRole, PlayerControl other)
        {
            var role = Role.GetRole(other);
            var amnesiac = amneRole.Player;
            var target = other.GetTarget();
            var leader = other.GetLeader();
            var actor = other.GetActorList();

            if (PlayerControl.LocalPlayer == amnesiac)
            {
                foreach (var component in amneRole.CurrentTarget?.bodyRenderers)
                    component.material.SetFloat("_Outline", 0f);

                amneRole.RememberButton.gameObject.SetActive(false);
                Utils.Flash(amneRole.Color, "You have attempted to remember who you were!");
                amneRole.OnLobby();
                CustomButtons.ResetCustomTimers(false);
            }

            if (PlayerControl.LocalPlayer == other)
            {
                Utils.Flash(amneRole.Color, "Someone has attempted to remember what you were like!");
                role.OnLobby();
            }

            Role newRole = role.RoleType switch
            {
                RoleEnum.Altruist => new Altruist(amnesiac),
                RoleEnum.Anarchist => new Anarchist(amnesiac),
                RoleEnum.Arsonist => new Arsonist(amnesiac) { DousedPlayers = ((Arsonist)role).DousedPlayers },
                RoleEnum.Blackmailer => new Blackmailer(amnesiac),
                RoleEnum.Bomber => new Bomber(amnesiac),
                RoleEnum.Camouflager => new Camouflager(amnesiac),
                RoleEnum.Cannibal => new Cannibal(amnesiac),
                RoleEnum.Enforcer => new Enforcer(amnesiac),
                RoleEnum.Concealer => new Concealer(amnesiac),
                RoleEnum.Consigliere => new Consigliere(amnesiac) { Investigated = ((Consigliere)role).Investigated },
                RoleEnum.Consort => new Consort(amnesiac),
                RoleEnum.Coroner => new Coroner(amnesiac),
                RoleEnum.Crewmate => new Crewmate(amnesiac),
                RoleEnum.Cryomaniac => new Cryomaniac(amnesiac) { DousedPlayers = ((Cryomaniac)role).DousedPlayers },
                RoleEnum.Detective => new Detective(amnesiac),
                RoleEnum.Disguiser => new Disguiser(amnesiac),
                RoleEnum.Dracula => new Dracula(amnesiac) { Converted = ((Dracula)role).Converted },
                RoleEnum.Engineer => new Engineer(amnesiac),
                RoleEnum.Escort => new Escort(amnesiac),
                RoleEnum.Executioner => new Executioner(amnesiac) { TargetPlayer = target },
                RoleEnum.Framer => new Framer(amnesiac),
                RoleEnum.Glitch => new Glitch(amnesiac),
                RoleEnum.Godfather => new Godfather(amnesiac),
                RoleEnum.Gorgon => new Gorgon(amnesiac),
                RoleEnum.Grenadier => new Grenadier(amnesiac),
                RoleEnum.GuardianAngel => new GuardianAngel(amnesiac) { TargetPlayer = target },
                RoleEnum.Impostor => new Impostor(amnesiac),
                RoleEnum.Inspector => new Inspector(amnesiac) { Inspected = ((Inspector)role).Inspected },
                RoleEnum.Jackal => new Jackal(amnesiac)
                {
                    Recruited = ((Jackal)role).Recruited,
                    EvilRecruit = ((Jackal)role).EvilRecruit,
                    GoodRecruit = ((Jackal)role).GoodRecruit,
                    BackupRecruit = ((Jackal)role).BackupRecruit
                },
                RoleEnum.Jester => new Jester(amnesiac),
                RoleEnum.Juggernaut => new Juggernaut(amnesiac) { JuggKills = ((Juggernaut)role).JuggKills },
                RoleEnum.Sheriff => new Sheriff(amnesiac) { Interrogated = ((Sheriff)role).Interrogated },
                RoleEnum.Mayor => new Mayor(amnesiac),
                RoleEnum.Mafioso => new Mafioso(amnesiac) { Godfather = (Godfather)leader },
                RoleEnum.Miner => new Miner(amnesiac),
                RoleEnum.Morphling => new Morphling(amnesiac),
                RoleEnum.Swapper => new Swapper(amnesiac),
                RoleEnum.Medic => new Medic(amnesiac),
                RoleEnum.Tracker => new Tracker(amnesiac) { TrackerArrows = ((Tracker)role).TrackerArrows },
                RoleEnum.Transporter => new Transporter(amnesiac),
                RoleEnum.Medium => new Medium(amnesiac),
                RoleEnum.Operative => new Operative(amnesiac),
                RoleEnum.Shifter => new Shifter(amnesiac),
                RoleEnum.Rebel => new Rebel(amnesiac),
                RoleEnum.Sidekick => new Sidekick(amnesiac) { Rebel = (Rebel)leader },
                RoleEnum.Shapeshifter => new Shapeshifter(amnesiac),
                RoleEnum.Murderer => new Murderer(amnesiac),
                RoleEnum.Survivor => new Survivor(amnesiac),
                RoleEnum.Plaguebearer => new Plaguebearer(amnesiac) { InfectedPlayers = ((Plaguebearer)role).InfectedPlayers },
                RoleEnum.Pestilence => new Pestilence(amnesiac),
                RoleEnum.SerialKiller => new SerialKiller(amnesiac),
                RoleEnum.Werewolf => new Werewolf(amnesiac),
                RoleEnum.Janitor => new Janitor(amnesiac),
                RoleEnum.Poisoner => new Poisoner(amnesiac),
                RoleEnum.TimeLord => new TimeLord(amnesiac),
                RoleEnum.Teleporter => new Teleporter(amnesiac),
                RoleEnum.TimeMaster => new TimeMaster(amnesiac),
                RoleEnum.Troll => new Troll(amnesiac),
                RoleEnum.Thief => new Thief(amnesiac),
                RoleEnum.VampireHunter => new VampireHunter(amnesiac),
                RoleEnum.Veteran => new Veteran(amnesiac),
                RoleEnum.Vigilante => new Vigilante(amnesiac),
                RoleEnum.Warper => new Warper(amnesiac),
                RoleEnum.Wraith => new Wraith(amnesiac),
                RoleEnum.Chameleon => new Chameleon(amnesiac),
                RoleEnum.Mystic => new Mystic(amnesiac),
                RoleEnum.Retributionist => new Retributionist(amnesiac)
                {
                    TrackerArrows = ((Retributionist)role).TrackerArrows,
                    Inspected = ((Retributionist)role).Inspected,
                    Interrogated = ((Retributionist)role).Interrogated
                },
                RoleEnum.Seer => new Seer(amnesiac),
                RoleEnum.Actor => new Actor(amnesiac) { PretendRoles = actor },
                RoleEnum.BountyHunter => new BountyHunter(amnesiac) { TargetPlayer = target },
                RoleEnum.Guesser => new Guesser(amnesiac) { TargetPlayer = target },
                RoleEnum.Necromancer => new Necromancer(amnesiac)
                {
                    Resurrected = ((Necromancer)role).Resurrected,
                    KillCount = ((Necromancer)role).KillCount,
                    ResurrectedCount = ((Necromancer)role).ResurrectedCount
                },
                RoleEnum.Whisperer => new Whisperer(amnesiac)
                {
                    Persuaded = ((Whisperer)role).Persuaded,
                    WhisperCount = ((Whisperer)role).WhisperCount,
                    WhisperConversion = ((Whisperer)role).WhisperConversion
                },
                RoleEnum.Betrayer => new Betrayer(amnesiac) { Faction = role.Faction },
                RoleEnum.Ambusher => new Ambusher(amnesiac),
                RoleEnum.Beamer => new Beamer(amnesiac),
                RoleEnum.Crusader => new Crusader(amnesiac),
                RoleEnum.Drunkard => new Drunkard(amnesiac),
                _ => new Amnesiac(amnesiac),
            };

            newRole.RoleUpdate(amneRole);

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

            if (amnesiac.Is(Faction.Intruder) || (amnesiac.Is(Faction.Syndicate) && CustomGameOptions.AltImps))
                amnesiac.Data.Role.TeamType = RoleTeamTypes.Impostor;

            if (amnesiac.Is(Faction.Intruder) || amnesiac.Is(Faction.Syndicate))
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
                        snitchRole.SnitchArrows.Add(amnesiac.PlayerId, arrow);
                    }
                    else if (snitchRole.TasksDone && PlayerControl.LocalPlayer == amnesiac)
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
        }
    }
}