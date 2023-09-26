namespace TownOfUsReworked.PlayerLayers.Roles;

public class Shifter : Crew
{
    public CustomButton ShiftButton { get; set; }

    public override Color Color => ClientGameOptions.CustomCrewColors ? Colors.Shifter : Colors.Crew;
    public override string Name => "Shifter";
    public override LayerEnum Type => LayerEnum.Shifter;
    public override Func<string> StartText => () => "Shift Around Roles";
    public override Func<string> Description => () => "- You can steal another player's role\n- Shifting withn on-<color=#8CFFFFFF>Crew</color> will cause you to kill yourself";
    public override InspectorResults InspectorResults => InspectorResults.BringsChaos;

    public Shifter(PlayerControl player) : base(player)
    {
        Alignment = Alignment.CrewSupport;
        ShiftButton = new(this, "Shift", AbilityTypes.Target, "ActionSecondary", Shift, CustomGameOptions.ShiftCd);
    }

    public void Shift()
    {
        var interact = Interact(Player, ShiftButton.TargetPlayer);

        if (interact.AbilityUsed)
        {
            CallRpc(CustomRPC.Action, ActionsRPC.LayerAction2, this, ShiftButton.TargetPlayer);
            Shift(ShiftButton.TargetPlayer);
        }
        else if (interact.Reset)
            ShiftButton.StartCooldown(CooldownType.Reset);
        else if (interact.Protected)
            ShiftButton.StartCooldown(CooldownType.GuardianAngel);
    }

    public void Shift(PlayerControl other)
    {
        var role = GetRole(other);

        if (!other.Is(Faction.Crew) || other.IsFramed())
        {
            if (AmongUsClient.Instance.AmHost)
                RpcMurderPlayer(Player, Player);

            return;
        }

        if (CustomPlayer.Local == other || CustomPlayer.Local == Player)
        {
            Flash(Colors.Shifter);
            role.OnLobby();
            OnLobby();
            ButtonUtils.ResetCustomTimers();
        }

        Role newRole = role.Type switch
        {
            LayerEnum.Crewmate => new Crewmate(Player),
            LayerEnum.Detective => new Detective(Player),
            LayerEnum.Escort => new Escort(Player),
            LayerEnum.Sheriff => new Sheriff(Player),
            LayerEnum.Medium => new Medium(Player),
            LayerEnum.VampireHunter => new VampireHunter(Player),
            LayerEnum.Mystic => new Mystic(Player),
            LayerEnum.Seer => new Seer(Player),
            LayerEnum.Altruist => new Altruist(Player),
            LayerEnum.Engineer => new Engineer(Player),
            LayerEnum.Inspector => new Inspector(Player),
            LayerEnum.Transporter => new Transporter(Player),
            LayerEnum.Mayor => new Mayor(Player),
            LayerEnum.Operative => new Operative(Player),
            LayerEnum.Veteran => new Veteran(Player),
            LayerEnum.Vigilante => new Vigilante(Player),
            LayerEnum.Chameleon => new Chameleon(Player),
            LayerEnum.Dictator => new Dictator(Player),
            LayerEnum.Tracker => new Tracker(Player),
            LayerEnum.Coroner => new Coroner(Player),
            LayerEnum.Medic => new Medic(Player) { ShieldedPlayer = ((Medic)role).ShieldedPlayer },
            LayerEnum.Monarch => new Monarch(Player)
            {
                ToBeKnighted = ((Monarch)role).ToBeKnighted,
                Knighted = ((Monarch)role).Knighted
            },
            LayerEnum.Retributionist => new Retributionist(Player)
            {
                Selected = ((Retributionist)role).Selected,
                ShieldedPlayer = ((Retributionist)role).ShieldedPlayer
            },
            LayerEnum.Shifter or _ => new Shifter(Player),
        };

        newRole.RoleUpdate(this);
        Role newRole2 = CustomGameOptions.ShiftedBecomes == BecomeEnum.Shifter ? new Shifter(other) : new Crewmate(other);
        newRole2.RoleUpdate(role);
    }

    public bool Exception(PlayerControl player) => (Faction is Faction.Intruder or Faction.Syndicate && player.Is(Faction)) || (SubFaction != SubFaction.None && player.Is(SubFaction));

    public override void ReadRPC(MessageReader reader) => Shift(reader.ReadPlayer());

    public override void UpdateHud(HudManager __instance)
    {
        base.UpdateHud(__instance);
        ShiftButton.Update2("SHIFT");
    }
}