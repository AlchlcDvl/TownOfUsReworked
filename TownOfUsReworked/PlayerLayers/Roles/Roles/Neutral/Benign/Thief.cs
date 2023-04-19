using System;
using TownOfUsReworked.CustomOptions;
using TownOfUsReworked.Data;
using UnityEngine;
using TownOfUsReworked.Custom;
using Hazel;
using TownOfUsReworked.Classes;
using HarmonyLib;
using TownOfUsReworked.PlayerLayers.Abilities;
using TownOfUsReworked.Extensions;

namespace TownOfUsReworked.PlayerLayers.Roles
{
    public class Thief : NeutralRole
    {
        public PlayerControl ClosestPlayer;
        public DateTime LastStolen;
        public CustomButton StealButton;

        public Thief(PlayerControl player) : base(player)
        {
            Name = "Thief";
            StartText = "Steal From The Killers";
            AbilitiesText = "- You can kill players to steal their roles\n- You cannot steal roles from players who cannot kill.";
            Color = CustomGameOptions.CustomNeutColors ? Colors.Thief : Colors.Neutral;
            RoleType = RoleEnum.Thief;
            RoleAlignment = RoleAlignment.NeutralBen;
            AlignmentName = NB;
            Type = LayerEnum.Thief;
            StealButton = new(this, AssetManager.Placeholder, AbilityTypes.Direct, "ActionSecondary", Steal);
        }

        public float StealTimer()
        {
            var utcNow = DateTime.UtcNow;
            var timespan = utcNow - LastStolen;
            var num = Player.GetModifiedCooldown(CustomGameOptions.ThiefKillCooldown) * 1000f;
            var flag2 = num - (float)timespan.TotalMilliseconds < 0f;
            return flag2 ? 0f : (num - (float)timespan.TotalMilliseconds) / 1000f;
        }

        public void Steal()
        {
            if (Utils.IsTooFar(Player, ClosestPlayer) || StealTimer() != 0f)
                return;

            var interact = Utils.Interact(Player, ClosestPlayer, true);

            if (interact[3])
            {
                if (!(ClosestPlayer.Is(Faction.Intruder) || ClosestPlayer.Is(Faction.Syndicate) || ClosestPlayer.Is(RoleAlignment.NeutralKill) ||
                    ClosestPlayer.Is(RoleAlignment.NeutralNeo) || ClosestPlayer.Is(RoleAlignment.NeutralPros) || ClosestPlayer.Is(RoleAlignment.CrewKill)))
                    Utils.RpcMurderPlayer(Player, Player);
                else
                {
                    var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.Action, SendOption.Reliable);
                    writer.Write((byte)ActionsRPC.Steal);
                    writer.Write(PlayerControl.LocalPlayer.PlayerId);
                    writer.Write(ClosestPlayer.PlayerId);
                    AmongUsClient.Instance.FinishRpcImmediately(writer);
                    //SoundManager.Instance.PlaySound(TownOfUsReworked.StealSound, false, 0.4f);
                    Utils.RpcMurderPlayer(Player, ClosestPlayer);
                    Steal(this, ClosestPlayer);
                }
            }

            if (interact[0])
                LastStolen = DateTime.UtcNow;
            else if (interact[1])
                LastStolen.AddSeconds(CustomGameOptions.ProtectKCReset);
            else if (interact[2])
                LastStolen.AddSeconds(CustomGameOptions.VestKCReset);
        }

        public static void Steal(Thief thiefRole, PlayerControl other)
        {
            var role = GetRole(other);
            var thief = thiefRole.Player;
            var target = other.GetTarget();
            var leader = other.GetLeader();
            thief.DisableButtons();
            other.DisableButtons();

            if (PlayerControl.LocalPlayer == other)
            {
                Utils.Flash(thiefRole.Color);
                role.OnLobby();
            }

            if (PlayerControl.LocalPlayer == thief)
            {
                Utils.Flash(thiefRole.Color);
                thiefRole.OnLobby();
            }

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
                RoleEnum.Framer => new Framer(thief) { Framed = ((Framer)role).Framed },
                RoleEnum.Glitch => new Glitch(thief),
                RoleEnum.Enforcer => new Enforcer(thief),
                RoleEnum.Godfather => new Godfather(thief),
                RoleEnum.Grenadier => new Grenadier(thief),
                RoleEnum.Impostor => new Impostor(thief),
                RoleEnum.Juggernaut => new Juggernaut(thief) { JuggKills = ((Juggernaut)role).JuggKills },
                RoleEnum.Mafioso => new Mafioso(thief) { Godfather = (Godfather)leader },
                RoleEnum.PromotedGodfather => new PromotedGodfather(thief) { Investigated = ((PromotedGodfather)role).Investigated },
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
                RoleEnum.VampireHunter => new VampireHunter(thief),
                RoleEnum.Veteran => new Veteran(thief) { UsesLeft = ((Veteran)role).UsesLeft },
                RoleEnum.Vigilante => new Vigilante(thief) { UsesLeft = ((Vigilante)role).UsesLeft },
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
                RoleEnum.Betrayer => new Betrayer(thief) { Faction = role.Faction },
                RoleEnum.Ambusher => new Ambusher(thief),
                RoleEnum.Crusader => new Crusader(thief),
                RoleEnum.Drunkard => new Drunkard(thief),
                RoleEnum.PromotedRebel => new PromotedRebel(thief)
                {
                    VoteBank = ((PromotedRebel)role).VoteBank,
                    Framed = ((Framer)role).Framed
                },
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

            thief.Data.SetImpostor(thief.Is(Faction.Intruder) || (thief.Is(Faction.Syndicate) && CustomGameOptions.AltImps));

            if (CustomGameOptions.ThiefSteals)
            {
                if (PlayerControl.LocalPlayer == other && other.Is(Faction.Intruder))
                {
                    HudManager.Instance.SabotageButton.gameObject.SetActive(false);
                    other.Data.Role.TeamType = RoleTeamTypes.Crewmate;
                }

                var newRole2 = new Thief(other);
                newRole2.RoleUpdate(role);
            }

            if (thief.Is(Faction.Intruder) || thief.Is(Faction.Syndicate) || (thief.Is(Faction.Neutral) && CustomGameOptions.SnitchSeesNeutrals))
            {
                foreach (var snitch in Ability.GetAbilities<Snitch>(AbilityEnum.Snitch))
                {
                    if (snitch.TasksDone && PlayerControl.LocalPlayer.Is(AbilityEnum.Snitch))
                    {
                        var gameObj = new GameObject();
                        var arrow = gameObj.AddComponent<ArrowBehaviour>();
                        gameObj.transform.parent = PlayerControl.LocalPlayer.gameObject.transform;
                        var renderer = gameObj.AddComponent<SpriteRenderer>();
                        renderer.sprite = AssetManager.Arrow;
                        arrow.image = renderer;
                        gameObj.layer = 5;
                        snitch.SnitchArrows.Add(thief.PlayerId, arrow);
                    }
                    else if (snitch.TasksDone && PlayerControl.LocalPlayer == thief)
                    {
                        var gameObj = new GameObject();
                        var arrow = gameObj.AddComponent<ArrowBehaviour>();
                        gameObj.transform.parent = PlayerControl.LocalPlayer.gameObject.transform;
                        var renderer = gameObj.AddComponent<SpriteRenderer>();
                        renderer.sprite = AssetManager.Arrow;
                        arrow.image = renderer;
                        gameObj.layer = 5;
                        snitch.ImpArrows.Add(arrow);
                    }
                }
            }

            foreach (var revealer in GetRoles<Revealer>(RoleEnum.Revealer))
            {
                if (revealer.Revealed && (thief.Is(Faction.Intruder) || thief.Is(Faction.Syndicate) || (thief.Is(Faction.Neutral) && CustomGameOptions.RevealerRevealsNeutrals)))
                {
                    var gameObj = new GameObject();
                    var arrow = gameObj.AddComponent<ArrowBehaviour>();
                    gameObj.transform.parent = thief.gameObject.transform;
                    var renderer = gameObj.AddComponent<SpriteRenderer>();
                    renderer.sprite = AssetManager.Arrow;
                    arrow.image = renderer;
                    gameObj.layer = 5;
                    revealer.ImpArrows.Add(arrow);
                }
            }

            thief.EnableButtons();
            other.EnableButtons();
        }

        public override void UpdateHud(HudManager __instance)
        {
            base.UpdateHud(__instance);
            StealButton.Update("STEAL", StealTimer(), CustomGameOptions.ThiefKillCooldown);
        }
    }
}