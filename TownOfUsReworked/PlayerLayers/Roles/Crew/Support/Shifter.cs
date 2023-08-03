namespace TownOfUsReworked.PlayerLayers.Roles
{
    public class Shifter : Crew
    {
        public DateTime LastShifted { get; set; }
        public CustomButton ShiftButton { get; set; }
        public float Timer => ButtonUtils.Timer(Player, LastShifted, CustomGameOptions.ShifterCd);

        public override Color32 Color => ClientGameOptions.CustomCrewColors ? Colors.Shifter : Colors.Crew;
        public override string Name => "Shifter";
        public override LayerEnum Type => LayerEnum.Shifter;
        public override RoleEnum RoleType => RoleEnum.Shifter;
        public override Func<string> StartText => () => "Shift Around Roles";
        public override Func<string> Description => () => "- You can steal another player's role\n- Shifting withn on-<color=#8CFFFFFF>Crew</color> will cause you to kill yourself";
        public override InspectorResults InspectorResults => InspectorResults.BringsChaos;

        public Shifter(PlayerControl player) : base(player)
        {
            RoleAlignment = RoleAlignment.CrewSupport;
            ShiftButton = new(this, "Shift", AbilityTypes.Direct, "ActionSecondary", Shift);
        }

        public void Shift()
        {
            if (Timer != 0f || IsTooFar(Player, ShiftButton.TargetPlayer))
                return;

            var interact = Interact(Player, ShiftButton.TargetPlayer);

            if (interact[3])
            {
                CallRpc(CustomRPC.Action, ActionsRPC.Shift, this, ShiftButton.TargetPlayer);
                Shift(this, ShiftButton.TargetPlayer);
            }

            if (interact[0])
                LastShifted = DateTime.UtcNow;
            else if (interact[1])
                LastShifted.AddSeconds(CustomGameOptions.ProtectKCReset);
        }

        public static void Shift(Shifter shifterRole, PlayerControl other)
        {
            var role = GetRole(other);
            var shifter = shifterRole.Player;

            if (!other.Is(Faction.Crew) || other.IsFramed())
            {
                if (AmongUsClient.Instance.AmHost)
                    RpcMurderPlayer(shifter, shifter);

                return;
            }

            if (CustomPlayer.Local == other)
            {
                Flash(shifterRole.Color);
                role.OnLobby();
                ButtonUtils.ResetCustomTimers();
            }

            if (CustomPlayer.Local == shifter)
            {
                Flash(shifterRole.Color);
                shifterRole.OnLobby();
                ButtonUtils.ResetCustomTimers();
            }

            Role newRole = role.RoleType switch
            {
                RoleEnum.Crewmate => new Crewmate(shifter),
                RoleEnum.Detective => new Detective(shifter),
                RoleEnum.Escort => new Escort(shifter),
                RoleEnum.Sheriff => new Sheriff(shifter),
                RoleEnum.Medic => new Medic(shifter),
                RoleEnum.Medium => new Medium(shifter),
                RoleEnum.VampireHunter => new VampireHunter(shifter),
                RoleEnum.Mystic => new Mystic(shifter),
                RoleEnum.Seer => new Seer(shifter),
                RoleEnum.Altruist => new Altruist(shifter) { UsesLeft = ((Altruist)role).UsesLeft },
                RoleEnum.Engineer => new Engineer(shifter) { UsesLeft = ((Engineer)role).UsesLeft },
                RoleEnum.Inspector => new Inspector(shifter) { Inspected = ((Inspector)role).Inspected },
                RoleEnum.Transporter => new Transporter(shifter) { UsesLeft = ((Transporter)role).UsesLeft },
                RoleEnum.Mayor => new Mayor(shifter) { Revealed = ((Mayor)role).Revealed },
                RoleEnum.Operative => new Operative(shifter) { UsesLeft = ((Operative)role).UsesLeft },
                RoleEnum.Veteran => new Veteran(shifter) { UsesLeft = ((Veteran)role).UsesLeft },
                RoleEnum.Vigilante => new Vigilante(shifter) { UsesLeft = ((Vigilante)role).UsesLeft },
                RoleEnum.Chameleon => new Chameleon(shifter) { UsesLeft = ((Chameleon)role).UsesLeft },
                RoleEnum.Dictator => new Dictator(shifter),
                RoleEnum.Tracker => new Tracker(shifter)
                {
                    TrackerArrows = ((Tracker)role).TrackerArrows,
                    UsesLeft = ((Tracker)role).UsesLeft
                },
                RoleEnum.Monarch => new Monarch(shifter)
                {
                    UsesLeft = ((Monarch)role).UsesLeft,
                    ToBeKnighted = ((Monarch)role).ToBeKnighted,
                    Knighted = ((Monarch)role).Knighted
                },
                RoleEnum.Coroner => new Coroner(shifter)
                {
                    ReferenceBodies = ((Coroner)role).ReferenceBodies,
                    Reported = ((Coroner)role).Reported
                },
                RoleEnum.Retributionist => new Retributionist(shifter)
                {
                    TrackerArrows = ((Retributionist)role).TrackerArrows,
                    Inspected = ((Retributionist)role).Inspected,
                    Selected = ((Retributionist)role).Selected,
                    UsesLeft = ((Retributionist)role).UsesLeft,
                    Reported = ((Retributionist)role).Reported,
                    ReferenceBodies = ((Retributionist)role).ReferenceBodies
                },
                _ => new Shifter(shifter),
            };

            newRole.RoleUpdate(shifterRole);
            Role newRole2 = CustomGameOptions.ShiftedBecomes == BecomeEnum.Shifter ? new Shifter(other) : new Crewmate(other);
            newRole2.RoleUpdate(role);
        }

        public bool Exception(PlayerControl player) => Faction is Faction.Intruder or Faction.Syndicate && player.Is(Faction);

        public override void UpdateHud(HudManager __instance)
        {
            base.UpdateHud(__instance);
            ShiftButton.Update("SHIFT", Timer, CustomGameOptions.ShifterCd);
        }
    }
}