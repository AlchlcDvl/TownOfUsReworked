namespace TownOfUsReworked.PlayerLayers.Roles
{
    public class Actor : Neutral
    {
        public bool Guessed;
        public InspectorResults PretendRoles => TargetRole.InspectorResults;
        public Role TargetRole;
        public bool Failed => !GetLayers<Assassin>(LayerEnum.Assassin).Any(x => !x.IsDead && !x.Disconnected);
        public CustomButton RoleButton;
        public int Rounds;
        public bool TargetFailed => TargetRole == null && Rounds > 0;

        public override Color32 Color => ClientGameOptions.CustomNeutColors ? Colors.Actor : Colors.Neutral;
        public override string Name => "Actor";
        public override LayerEnum Type => LayerEnum.Actor;
        public override RoleEnum RoleType => RoleEnum.Actor;
        public override Func<string> StartText => () => "Play Pretend With The Others";
        public override Func<string> AbilitiesText => () => TargetRole == null ? "- You can select a player whose role you can pretend to be" : ("- Upon being guessed, you kill your " +
            "guesser");
        public override InspectorResults InspectorResults => TargetRole == null ? InspectorResults.Manipulative : TargetRole.InspectorResults;

        public Actor(PlayerControl player) : base(player)
        {
            Objectives = () => Guessed ? "- You have successfully fooled the crew" : ($"- Get guessed as one of your target roles\n- Your target roles belong to the {PretendRoles} role " +
                "list");
            RoleAlignment = RoleAlignment.NeutralEvil;
            RoleButton = new(this, "ActorRole", AbilityTypes.Direct, "ActionSecondary", PickRole);
        }

        public void PickRole()
        {
            if (IsTooFar(Player, RoleButton.TargetPlayer))
                return;

            TargetRole = GetRole(RoleButton.TargetPlayer);
            CallRpc(CustomRPC.Target, TargetRPC.SetActPretendList, this, TargetRole);
        }

        public void TurnRole()
        {
            Role newRole;

            if (TargetRole == null)
                newRole = new Crewmate(Player);
            else
            {
                var target = TargetRole.Player.GetTarget();
                var leader = TargetRole.Player.GetLeader();

                newRole = TargetRole.RoleType switch
                {
                    RoleEnum.Anarchist => new Anarchist(Player),
                    RoleEnum.Arsonist => new Arsonist(Player),
                    RoleEnum.Blackmailer => new Blackmailer(Player),
                    RoleEnum.Bomber => new Bomber(Player),
                    RoleEnum.Camouflager => new Camouflager(Player),
                    RoleEnum.Cannibal => new Cannibal(Player),
                    RoleEnum.Enforcer => new Enforcer(Player),
                    RoleEnum.Concealer => new Concealer(Player),
                    RoleEnum.Consigliere => new Consigliere(Player),
                    RoleEnum.Consort => new Consort(Player),
                    RoleEnum.Crewmate => new Crewmate(Player),
                    RoleEnum.Cryomaniac => new Cryomaniac(Player),
                    RoleEnum.Detective => new Detective(Player),
                    RoleEnum.Disguiser => new Disguiser(Player),
                    RoleEnum.Dracula => new Dracula(Player),
                    RoleEnum.Escort => new Escort(Player),
                    RoleEnum.Executioner => new Executioner(Player) { TargetPlayer = target },
                    RoleEnum.Framer => new Framer(Player),
                    RoleEnum.Glitch => new Glitch(Player),
                    RoleEnum.Godfather => new Godfather(Player),
                    RoleEnum.PromotedGodfather => new PromotedGodfather(Player),
                    RoleEnum.Grenadier => new Grenadier(Player),
                    RoleEnum.GuardianAngel => new GuardianAngel(Player) { TargetPlayer = target },
                    RoleEnum.Impostor => new Impostor(Player),
                    RoleEnum.Jackal => new Jackal(Player),
                    RoleEnum.Jester => new Jester(Player),
                    RoleEnum.Juggernaut => new Juggernaut(Player),
                    RoleEnum.Sheriff => new Sheriff(Player),
                    RoleEnum.Mafioso => new Mafioso(Player) { Godfather = (Godfather)leader },
                    RoleEnum.Miner => new Miner(Player),
                    RoleEnum.Morphling => new Morphling(Player),
                    RoleEnum.Medic => new Medic(Player),
                    RoleEnum.Medium => new Medium(Player),
                    RoleEnum.Shifter => new Shifter(Player),
                    RoleEnum.Rebel => new Rebel(Player),
                    RoleEnum.PromotedRebel => new PromotedRebel(Player),
                    RoleEnum.Sidekick => new Sidekick(Player) { Rebel = (Rebel)leader },
                    RoleEnum.Shapeshifter => new Shapeshifter(Player),
                    RoleEnum.Murderer => new Murderer(Player),
                    RoleEnum.Survivor => new Survivor(Player),
                    RoleEnum.Plaguebearer => new Plaguebearer(Player),
                    RoleEnum.Pestilence => new Pestilence(Player),
                    RoleEnum.SerialKiller => new SerialKiller(Player),
                    RoleEnum.Werewolf => new Werewolf(Player),
                    RoleEnum.Janitor => new Janitor(Player),
                    RoleEnum.Poisoner => new Poisoner(Player),
                    RoleEnum.Teleporter => new Teleporter(Player),
                    RoleEnum.Troll => new Troll(Player),
                    RoleEnum.Thief => new Thief(Player),
                    RoleEnum.VampireHunter => new VampireHunter(Player),
                    RoleEnum.Warper => new Warper(Player),
                    RoleEnum.Wraith => new Wraith(Player),
                    RoleEnum.Mystic => new Mystic(Player),
                    RoleEnum.Dictator => new Dictator(Player),
                    RoleEnum.Seer => new Seer(Player),
                    RoleEnum.BountyHunter => new BountyHunter(Player) { TargetPlayer = target },
                    RoleEnum.Guesser => new Guesser(Player) { TargetPlayer = target },
                    RoleEnum.Necromancer => new Necromancer(Player),
                    RoleEnum.Whisperer => new Whisperer(Player),
                    RoleEnum.Betrayer => new Betrayer(Player),
                    RoleEnum.Ambusher => new Ambusher(Player),
                    RoleEnum.Crusader => new Crusader(Player),
                    RoleEnum.Altruist => new Altruist(Player),
                    RoleEnum.Engineer => new Engineer(Player),
                    RoleEnum.Inspector => new Inspector(Player),
                    RoleEnum.Tracker => new Tracker(Player),
                    RoleEnum.Stalker => new Stalker(Player),
                    RoleEnum.Transporter => new Transporter(Player),
                    RoleEnum.Mayor => new Mayor(Player),
                    RoleEnum.Operative => new Operative(Player),
                    RoleEnum.Veteran => new Veteran(Player),
                    RoleEnum.Vigilante => new Vigilante(Player),
                    RoleEnum.Chameleon => new Chameleon(Player),
                    RoleEnum.Coroner => new Coroner(Player),
                    RoleEnum.Monarch => new Monarch(Player),
                    RoleEnum.Retributionist => new Retributionist(Player),
                    _ => new Amnesiac(Player),
                };
            }

            newRole.RoleUpdate(this);

            if (Local)
                Flash(newRole.Color);

            if (CustomPlayer.Local.Is(RoleEnum.Seer))
                Flash(Colors.Seer);
        }

        public override void UpdateHud(HudManager __instance)
        {
            base.UpdateHud(__instance);
            RoleButton.Update("PRETEND", true, TargetRole == null);

            if ((TargetFailed || (TargetRole != null && Failed)) && !IsDead)
            {
                CallRpc(CustomRPC.Change, TurnRPC.TurnRole, this);
                TurnRole();
            }
        }
    }
}