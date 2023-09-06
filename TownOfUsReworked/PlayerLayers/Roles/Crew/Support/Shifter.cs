namespace TownOfUsReworked.PlayerLayers.Roles;

public class Shifter : Crew
{
    public DateTime LastShifted { get; set; }
    public CustomButton ShiftButton { get; set; }
    public float Timer => ButtonUtils.Timer(Player, LastShifted, CustomGameOptions.ShiftCd);

    public override Color Color => ClientGameOptions.CustomCrewColors ? Colors.Shifter : Colors.Crew;
    public override string Name => "Shifter";
    public override LayerEnum Type => LayerEnum.Shifter;
    public override Func<string> StartText => () => "Shift Around Roles";
    public override Func<string> Description => () => "- You can steal another player's role\n- Shifting withn on-<color=#8CFFFFFF>Crew</color> will cause you to kill yourself";
    public override InspectorResults InspectorResults => InspectorResults.BringsChaos;

    public Shifter(PlayerControl player) : base(player)
    {
        Alignment = Alignment.CrewSupport;
        ShiftButton = new(this, "Shift", AbilityTypes.Direct, "ActionSecondary", Shift);
    }

    public void Shift()
    {
        if (Timer != 0f || IsTooFar(Player, ShiftButton.TargetPlayer))
            return;

        var interact = Interact(Player, ShiftButton.TargetPlayer);

        if (interact.AbilityUsed)
        {
            CallRpc(CustomRPC.Action, ActionsRPC.Shift, this, ShiftButton.TargetPlayer);
            Shift(this, ShiftButton.TargetPlayer);
        }

        if (interact.Reset)
            LastShifted = DateTime.UtcNow;
        else if (interact.Protected)
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

        if (CustomPlayer.Local == other || CustomPlayer.Local == shifter)
        {
            Flash(Colors.Shifter);
            role.OnLobby();
            shifterRole.OnLobby();
            ButtonUtils.ResetCustomTimers();
        }

        Role newRole = role.Type switch
        {
            LayerEnum.Crewmate => new Crewmate(shifter),
            LayerEnum.Detective => new Detective(shifter),
            LayerEnum.Escort => new Escort(shifter),
            LayerEnum.Sheriff => new Sheriff(shifter),
            LayerEnum.Medic => new Medic(shifter),
            LayerEnum.Medium => new Medium(shifter),
            LayerEnum.VampireHunter => new VampireHunter(shifter),
            LayerEnum.Mystic => new Mystic(shifter),
            LayerEnum.Seer => new Seer(shifter),
            LayerEnum.Altruist => new Altruist(shifter) { UsesLeft = ((Altruist)role).UsesLeft },
            LayerEnum.Engineer => new Engineer(shifter) { UsesLeft = ((Engineer)role).UsesLeft },
            LayerEnum.Inspector => new Inspector(shifter) { Inspected = ((Inspector)role).Inspected },
            LayerEnum.Transporter => new Transporter(shifter) { UsesLeft = ((Transporter)role).UsesLeft },
            LayerEnum.Mayor => new Mayor(shifter) { Revealed = ((Mayor)role).Revealed },
            LayerEnum.Operative => new Operative(shifter) { UsesLeft = ((Operative)role).UsesLeft },
            LayerEnum.Veteran => new Veteran(shifter) { UsesLeft = ((Veteran)role).UsesLeft },
            LayerEnum.Vigilante => new Vigilante(shifter) { UsesLeft = ((Vigilante)role).UsesLeft },
            LayerEnum.Chameleon => new Chameleon(shifter) { UsesLeft = ((Chameleon)role).UsesLeft },
            LayerEnum.Dictator => new Dictator(shifter),
            LayerEnum.Tracker => new Tracker(shifter)
            {
                TrackerArrows = ((Tracker)role).TrackerArrows,
                UsesLeft = ((Tracker)role).UsesLeft
            },
            LayerEnum.Monarch => new Monarch(shifter)
            {
                UsesLeft = ((Monarch)role).UsesLeft,
                ToBeKnighted = ((Monarch)role).ToBeKnighted,
                Knighted = ((Monarch)role).Knighted
            },
            LayerEnum.Coroner => new Coroner(shifter)
            {
                ReferenceBodies = ((Coroner)role).ReferenceBodies,
                Reported = ((Coroner)role).Reported
            },
            LayerEnum.Retributionist => new Retributionist(shifter)
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
        ShiftButton.Update("SHIFT", Timer, CustomGameOptions.ShiftCd);
    }
}