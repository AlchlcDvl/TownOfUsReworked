using System.Collections.Generic;
using System.Linq;
using Object = UnityEngine.Object;
using TownOfUsReworked.CustomOptions;
using TownOfUsReworked.Classes;
using TownOfUsReworked.Data;
using TownOfUsReworked.Custom;
using Hazel;
using HarmonyLib;
using TownOfUsReworked.PlayerLayers.Abilities;
using UnityEngine;
using TownOfUsReworked.Extensions;
using TownOfUsReworked.Modules;
using TownOfUsReworked.Patches;

namespace TownOfUsReworked.PlayerLayers.Roles
{
    public class Amnesiac : NeutralRole
    {
        public Dictionary<byte, ArrowBehaviour> BodyArrows = new();
        public DeadBody CurrentTarget;
        public CustomButton RememberButton;

        public Amnesiac(PlayerControl player) : base(player)
        {
            Name = "Amnesiac";
            StartText = "You Forgor :skull:";
            AbilitiesText = "- You can copy over a player's role should you find their body" + (CustomGameOptions.RememberArrows ? "\n- When someone dies, you get an arrow " +
                "pointing to their body" : "");
            RoleType = RoleEnum.Amnesiac;
            RoleAlignment = RoleAlignment.NeutralBen;
            AlignmentName = NB;
            Color = CustomGameOptions.CustomNeutColors ? Colors.Amnesiac : Colors.Neutral;
            Objectives = "- Find a dead body, remember their role and then fulfill the win condition for that role";
            BodyArrows = new();
            InspectorResults = InspectorResults.DealsWithDead;
            Type = LayerEnum.Amnesiac;
            RememberButton = new(this, AssetManager.Remember, AbilityTypes.Dead, "ActionSecondary", Remember);
        }

        public void DestroyArrow(byte targetPlayerId)
        {
            var arrow = BodyArrows.FirstOrDefault(x => x.Key == targetPlayerId);

            if (arrow.Value != null)
                Object.Destroy(arrow.Value);
            if (arrow.Value.gameObject != null)
                Object.Destroy(arrow.Value.gameObject);

            BodyArrows.Remove(arrow.Key);
        }

        public override void OnLobby()
        {
            BodyArrows.Values.DestroyAll();
            BodyArrows.Clear();
        }

        public void Remember()
        {
            if (Utils.IsTooFar(Player, CurrentTarget))
                return;

            var playerId = CurrentTarget.ParentId;
            var player = Utils.PlayerById(playerId);
            Utils.Spread(Player, player);
            var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.Action, SendOption.Reliable);
            writer.Write((byte)ActionsRPC.Remember);
            writer.Write(Player.PlayerId);
            writer.Write(playerId);
            AmongUsClient.Instance.FinishRpcImmediately(writer);
            Remember(this, player);
        }

        public static void Remember(Amnesiac amneRole, PlayerControl other)
        {
            var role = GetRole(other);
            var amnesiac = amneRole.Player;
            var target = other.GetTarget();
            var leader = other.GetLeader();
            var actor = other.GetActorList();
            amnesiac.DisableButtons();
            other.DisableButtons();

            if (PlayerControl.LocalPlayer == amnesiac)
            {
                foreach (var component in amneRole.CurrentTarget?.bodyRenderers)
                    component.material.SetFloat("_Outline", 0f);

                Utils.Flash(amneRole.Color);
                amneRole.OnLobby();
            }

            if (PlayerControl.LocalPlayer == other)
            {
                Utils.Flash(amneRole.Color);
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
                RoleEnum.Cannibal => new Cannibal(amnesiac) { EatNeed = ((Cannibal)role).EatNeed },
                RoleEnum.Enforcer => new Enforcer(amnesiac),
                RoleEnum.Concealer => new Concealer(amnesiac),
                RoleEnum.Consigliere => new Consigliere(amnesiac) { Investigated = ((Consigliere)role).Investigated },
                RoleEnum.Consort => new Consort(amnesiac),
                RoleEnum.Coroner => new Coroner(amnesiac) { Reported = ((Coroner)role).Reported },
                RoleEnum.Crewmate => new Crewmate(amnesiac),
                RoleEnum.Cryomaniac => new Cryomaniac(amnesiac) { DousedPlayers = ((Cryomaniac)role).DousedPlayers },
                RoleEnum.Detective => new Detective(amnesiac),
                RoleEnum.Disguiser => new Disguiser(amnesiac),
                RoleEnum.Dracula => new Dracula(amnesiac) { Converted = ((Dracula)role).Converted },
                RoleEnum.Engineer => new Engineer(amnesiac) { UsesLeft = ((Engineer)role).UsesLeft },
                RoleEnum.Escort => new Escort(amnesiac),
                RoleEnum.Executioner => new Executioner(amnesiac) { TargetPlayer = target },
                RoleEnum.Framer => new Framer(amnesiac) { Framed = ((Framer)role).Framed },
                RoleEnum.Glitch => new Glitch(amnesiac),
                RoleEnum.Godfather => new Godfather(amnesiac),
                RoleEnum.PromotedGodfather => new PromotedGodfather(amnesiac) { Investigated = ((PromotedGodfather)role).Investigated },
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
                RoleEnum.Sheriff => new Sheriff(amnesiac),
                RoleEnum.Mayor => new Mayor(amnesiac) { VoteBank = ((Mayor)role).VoteBank },
                RoleEnum.Politician => new Politician(amnesiac) { VoteBank = ((Politician)role).VoteBank },
                RoleEnum.Mafioso => new Mafioso(amnesiac) { Godfather = (Godfather)leader },
                RoleEnum.Miner => new Miner(amnesiac),
                RoleEnum.Morphling => new Morphling(amnesiac),
                RoleEnum.Swapper => new Swapper(amnesiac),
                RoleEnum.Medic => new Medic(amnesiac),
                RoleEnum.Tracker => new Tracker(amnesiac)
                {
                    TrackerArrows = ((Tracker)role).TrackerArrows,
                    UsesLeft = ((Tracker)role).UsesLeft
                },
                RoleEnum.Transporter => new Transporter(amnesiac) { UsesLeft = ((Transporter)role).UsesLeft },
                RoleEnum.Medium => new Medium(amnesiac),
                RoleEnum.Operative => new Operative(amnesiac),
                RoleEnum.Shifter => new Shifter(amnesiac),
                RoleEnum.Rebel => new Rebel(amnesiac),
                RoleEnum.PromotedRebel => new PromotedRebel(amnesiac)
                {
                    VoteBank = ((PromotedRebel)role).VoteBank,
                    Framed = ((Framer)role).Framed
                },
                RoleEnum.Sidekick => new Sidekick(amnesiac) { Rebel = (Rebel)leader },
                RoleEnum.Shapeshifter => new Shapeshifter(amnesiac),
                RoleEnum.Murderer => new Murderer(amnesiac),
                RoleEnum.Survivor => new Survivor(amnesiac) { UsesLeft = ((Survivor)role).UsesLeft },
                RoleEnum.Plaguebearer => new Plaguebearer(amnesiac) { InfectedPlayers = ((Plaguebearer)role).InfectedPlayers },
                RoleEnum.Pestilence => new Pestilence(amnesiac),
                RoleEnum.SerialKiller => new SerialKiller(amnesiac),
                RoleEnum.Werewolf => new Werewolf(amnesiac),
                RoleEnum.Janitor => new Janitor(amnesiac),
                RoleEnum.Poisoner => new Poisoner(amnesiac),
                RoleEnum.TimeLord => new TimeLord(amnesiac) { UsesLeft = ((Engineer)role).UsesLeft },
                RoleEnum.Teleporter => new Teleporter(amnesiac) { TeleportPoint = ((Teleporter)role).TeleportPoint },
                RoleEnum.TimeMaster => new TimeMaster(amnesiac),
                RoleEnum.Troll => new Troll(amnesiac),
                RoleEnum.Thief => new Thief(amnesiac),
                RoleEnum.VampireHunter => new VampireHunter(amnesiac),
                RoleEnum.Veteran => new Veteran(amnesiac) { UsesLeft = ((Engineer)role).UsesLeft },
                RoleEnum.Vigilante => new Vigilante(amnesiac) { UsesLeft = ((Engineer)role).UsesLeft },
                RoleEnum.Warper => new Warper(amnesiac),
                RoleEnum.Wraith => new Wraith(amnesiac),
                RoleEnum.Chameleon => new Chameleon(amnesiac) { UsesLeft = ((Engineer)role).UsesLeft },
                RoleEnum.Mystic => new Mystic(amnesiac),
                RoleEnum.Retributionist => new Retributionist(amnesiac)
                {
                    TrackerArrows = ((Retributionist)role).TrackerArrows,
                    Inspected = ((Retributionist)role).Inspected,
                    UsesLeft = ((Retributionist)role).UsesLeft
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

            amnesiac.Data.SetImpostor(amnesiac.Is(Faction.Intruder) || (amnesiac.Is(Faction.Syndicate) && CustomGameOptions.AltImps));

            if (amnesiac.Is(Faction.Intruder) || amnesiac.Is(Faction.Syndicate) || (amnesiac.Is(Faction.Neutral) && CustomGameOptions.SnitchSeesNeutrals))
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
                        snitch.SnitchArrows.Add(amnesiac.PlayerId, arrow);
                    }
                    else if (snitch.TasksDone && PlayerControl.LocalPlayer == amnesiac)
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
                if (revealer.Revealed && (amnesiac.Is(Faction.Intruder) || amnesiac.Is(Faction.Syndicate) || (amnesiac.Is(Faction.Neutral) && CustomGameOptions.RevealerRevealsNeutrals)))
                {
                    var gameObj = new GameObject();
                    var arrow = gameObj.AddComponent<ArrowBehaviour>();
                    gameObj.transform.parent = amnesiac.gameObject.transform;
                    var renderer = gameObj.AddComponent<SpriteRenderer>();
                    renderer.sprite = AssetManager.Arrow;
                    arrow.image = renderer;
                    gameObj.layer = 5;
                    revealer.ImpArrows.Add(arrow);
                }
            }

            amnesiac.EnableButtons();
            other.EnableButtons();
        }

        public override void UpdateHud(HudManager __instance)
        {
            base.UpdateHud(__instance);
            RememberButton.Update("REMEBER", 0, 1);

            if (CustomGameOptions.RememberArrows && !PlayerControl.LocalPlayer.Data.IsDead)
            {
                var validBodies = Object.FindObjectsOfType<DeadBody>().Where(x => Murder.KilledPlayers.Any(y => y.PlayerId == x.ParentId &&
                    y.KillTime.AddSeconds(CustomGameOptions.RememberArrowDelay) < System.DateTime.UtcNow));

                foreach (var bodyArrow in BodyArrows.Keys)
                {
                    if (!validBodies.Any(x => x.ParentId == bodyArrow))
                        DestroyArrow(bodyArrow);
                }

                foreach (var body in validBodies)
                {
                    if (!BodyArrows.ContainsKey(body.ParentId))
                    {
                        var gameObj = new GameObject();
                        var arrow = gameObj.AddComponent<ArrowBehaviour>();
                        gameObj.transform.parent = PlayerControl.LocalPlayer.gameObject.transform;
                        var renderer = gameObj.AddComponent<SpriteRenderer>();
                        renderer.sprite = AssetManager.Arrow;
                        arrow.image = renderer;
                        gameObj.layer = 5;
                        BodyArrows.Add(body.ParentId, arrow);
                    }

                    BodyArrows.GetValueSafe(body.ParentId).target = body.TruePosition;
                }
            }
            else if (BodyArrows.Count != 0)
                OnLobby();
        }
    }
}