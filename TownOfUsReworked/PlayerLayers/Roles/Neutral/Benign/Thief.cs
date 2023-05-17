namespace TownOfUsReworked.PlayerLayers.Roles
{
    public class Thief : NeutralRole
    {
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
            Type = LayerEnum.Thief;
            StealButton = new(this, "Steal", AbilityTypes.Direct, "ActionSecondary", Steal);
            InspectorResults = InspectorResults.BringsChaos;
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
            if (Utils.IsTooFar(Player, StealButton.TargetPlayer) || StealTimer() != 0f)
                return;

            var interact = Utils.Interact(Player, StealButton.TargetPlayer, true);

            if (interact[3])
            {
                if (!(StealButton.TargetPlayer.Is(Faction.Intruder) || StealButton.TargetPlayer.Is(Faction.Syndicate) || StealButton.TargetPlayer.Is(RoleAlignment.NeutralKill) ||
                    StealButton.TargetPlayer.Is(RoleAlignment.NeutralNeo) || StealButton.TargetPlayer.Is(RoleAlignment.NeutralPros) || StealButton.TargetPlayer.Is(RoleAlignment.CrewKill)))
                    Utils.RpcMurderPlayer(Player, Player);
                else
                {
                    var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.Action, SendOption.Reliable);
                    writer.Write((byte)ActionsRPC.Steal);
                    writer.Write(PlayerControl.LocalPlayer.PlayerId);
                    writer.Write(StealButton.TargetPlayer.PlayerId);
                    AmongUsClient.Instance.FinishRpcImmediately(writer);
                    Utils.RpcMurderPlayer(Player, StealButton.TargetPlayer);
                    Steal(this, StealButton.TargetPlayer);
                }
            }

            if (interact[0])
                LastStolen = DateTime.UtcNow;
            else if (interact[1])
                LastStolen.AddSeconds(CustomGameOptions.ProtectKCReset);
            else if (interact[2])
                LastStolen.AddSeconds(CustomGameOptions.VestKCReset);
        }

        public bool Exception(PlayerControl player) => player.Is(Faction) || (player.Is(SubFaction) && SubFaction != SubFaction.None) || player == Player.GetOtherLover() || player ==
            Player.GetOtherRival() || (player.Is(ObjectifierEnum.Mafia) && Player.Is(ObjectifierEnum.Mafia));

        public static void Steal(Thief thiefRole, PlayerControl other)
        {
            var role = GetRole(other);
            var thief = thiefRole.Player;
            var target = other.GetTarget();
            var leader = other.GetLeader();
            thief.DisableButtons();
            other.DisableButtons();

            if (PlayerControl.LocalPlayer == other || PlayerControl.LocalPlayer == thief)
            {
                Utils.Flash(thiefRole.Color);
                role.OnLobby();
                thiefRole.OnLobby();
            }

            Role newRole = role.RoleType switch
            {
                RoleEnum.Anarchist => new Anarchist(thief),
                RoleEnum.Arsonist => new Arsonist(thief) { Doused = ((Arsonist)role).Doused },
                RoleEnum.Blackmailer => new Blackmailer(thief),
                RoleEnum.Bomber => new Bomber(thief),
                RoleEnum.Camouflager => new Camouflager(thief),
                RoleEnum.Concealer => new Concealer(thief),
                RoleEnum.Consigliere => new Consigliere(thief) { Investigated = ((Consigliere)role).Investigated },
                RoleEnum.Consort => new Consort(thief),
                RoleEnum.Cryomaniac => new Cryomaniac(thief) { Doused = ((Cryomaniac)role).Doused },
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
                RoleEnum.Plaguebearer => new Plaguebearer(thief) { Infected = ((Plaguebearer)role).Infected },
                RoleEnum.Pestilence => new Pestilence(thief),
                RoleEnum.SerialKiller => new SerialKiller(thief),
                RoleEnum.Werewolf => new Werewolf(thief),
                RoleEnum.Janitor => new Janitor(thief),
                RoleEnum.Poisoner => new Poisoner(thief),
                RoleEnum.Teleporter => new Teleporter(thief),
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
                RoleEnum.PromotedRebel => new PromotedRebel(thief) { Framed = ((PromotedRebel)role).Framed },
                RoleEnum.Stalker => new Stalker(thief) { StalkerArrows = ((Stalker)role).StalkerArrows },
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
                    if (snitch.TasksLeft <= CustomGameOptions.SnitchTasksRemaining && PlayerControl.LocalPlayer == thief)
                    {
                        var gameObj = new GameObject("SnitchArrow") { layer = 5 };
                        var arrow = gameObj.AddComponent<ArrowBehaviour>();
                        gameObj.transform.parent = PlayerControl.LocalPlayer.gameObject.transform;
                        var renderer = gameObj.AddComponent<SpriteRenderer>();
                        renderer.sprite = AssetManager.GetSprite("Arrow");
                        arrow.image = renderer;
                        LocalRole.AllArrows.Add(snitch.PlayerId, arrow);
                    }
                    else if (snitch.TasksDone && PlayerControl.LocalPlayer == snitch.Player)
                    {
                        var gameObj = new GameObject("SnitchEvilArrow") { layer = 5 };
                        var arrow = gameObj.AddComponent<ArrowBehaviour>();
                        gameObj.transform.parent = PlayerControl.LocalPlayer.gameObject.transform;
                        var renderer = gameObj.AddComponent<SpriteRenderer>();
                        renderer.sprite = AssetManager.GetSprite("Arrow");
                        arrow.image = renderer;
                        LocalRole.AllArrows.Add(thief.PlayerId, arrow);
                    }
                }

                foreach (var revealer in GetRoles<Revealer>(RoleEnum.Revealer))
                {
                    if (revealer.Revealed && PlayerControl.LocalPlayer == thief)
                    {
                        var gameObj = new GameObject("RevealerArrow") { layer = 5 };
                        var arrow = gameObj.AddComponent<ArrowBehaviour>();
                        gameObj.transform.parent = PlayerControl.LocalPlayer.gameObject.transform;
                        var renderer = gameObj.AddComponent<SpriteRenderer>();
                        renderer.sprite = AssetManager.GetSprite("Arrow");
                        arrow.image = renderer;
                        LocalRole.AllArrows.Add(revealer.PlayerId, arrow);
                    }
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