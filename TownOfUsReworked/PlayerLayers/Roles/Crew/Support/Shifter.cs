namespace TownOfUsReworked.PlayerLayers.Roles;

[HeaderOption(MultiMenu.LayerSubOptions)]
public class Shifter : Crew
{
    [NumberOption(MultiMenu.LayerSubOptions, 10f, 60f, 2.5f, Format.Time)]
    public static Number ShiftCd { get; set; } = new(25);

    [StringOption(MultiMenu.LayerSubOptions)]
    public static BecomeEnum ShiftedBecomes { get; set; } = BecomeEnum.Shifter;

    public CustomButton ShiftButton { get; set; }
    public CustomPlayerMenu ShifterMenu { get; set; }

    public override UColor Color => ClientOptions.CustomCrewColors ? CustomColorManager.Shifter : CustomColorManager.Crew;
    public override string Name => "Shifter";
    public override LayerEnum Type => LayerEnum.Shifter;
    public override Func<string> StartText => () => "Shift Around Roles";
    public override Func<string> Description => () => "- You can steal another player's role\n- Shifting withn on-<color=#8CFFFFFF>Crew</color> will cause you to kill yourself";

    public override void Init()
    {
        base.Init();
        Alignment = Alignment.CrewSupport;
        ShifterMenu = new(Player, Shift, Exception);
        ShiftButton ??= new(this, "SHIFT", new SpriteName("Shift"), AbilityTypes.Targetless, KeybindType.ActionSecondary, (OnClick)ShifterMenu.Open, new Cooldown(ShiftCd));
    }

    public override void Deinit()
    {
        base.Deinit();
        CustomMenu.AllMenus.Remove(ShifterMenu);
    }

    public void Shift()
    {
        var target = ShiftButton.GetTarget<PlayerControl>();
        var cooldown = Interact(Player, target, astral: true);

        if (cooldown != CooldownType.Fail)
        {
            CallRpc(CustomRPC.Action, ActionsRPC.LayerAction, this, target);
            Shift(target);
        }
        else
            ShiftButton.StartCooldown(cooldown);
    }

    public void Shift(PlayerControl other)
    {
        var role = other.GetRole();

        if (!other.IsBase(Faction.Crew) || other.IsFramed())
        {
            if (AmongUsClient.Instance.AmHost)
                RpcMurderPlayer(Player);

            return;
        }

        var player = Player;

        Role newRole = role switch
        {
            Altruist => new Altruist(),
            Bastion bastion => new Bastion() { BombedIDs = bastion.BombedIDs },
            Chameleon => new Chameleon(),
            Coroner => new Coroner(),
            Crewmate => new Crewmate(),
            Detective => new Detective(),
            Dictator => new Dictator(),
            Engineer => new Engineer(),
            Escort => new Escort(),
            Mayor => new Mayor(),
            Medic medic => new Medic() { ShieldedPlayer = medic.ShieldedPlayer },
            Medium => new Medium(),
            Monarch mon => new Monarch()
            {
                ToBeKnighted = mon.ToBeKnighted,
                Knighted = mon.Knighted
            },
            Mystic => new Mystic(),
            Operative => new Operative(),
            Retributionist ret => new Retributionist()
            {
                Selected = ret.Selected,
                ShieldedPlayer = ret.ShieldedPlayer,
                BombedIDs = ret.BombedIDs,
                Trapped = ret.Trapped
            },
            Seer => new Seer(),
            Sheriff => new Sheriff(),
            Tracker => new Tracker(),
            Transporter => new Transporter(),
            Trapper trap => new Trapper() { Trapped = trap.Trapped },
            VampireHunter => new VampireHunter(),
            Veteran => new Veteran(),
            Vigilante => new Vigilante(),
            Shifter or _ => new Shifter(),
        };

        newRole.RoleUpdate(this, player, true);
        Role newRole2 = ShiftedBecomes == BecomeEnum.Shifter ? new Shifter() : new Crewmate();
        newRole2.RoleUpdate(role, other, true);

        if (other.AmOwner)
            Flash(Color);
    }

    public bool Exception(PlayerControl player) => player.HasDied() || (Faction is Faction.Intruder or Faction.Syndicate && player.Is(Faction)) || (SubFaction != SubFaction.None &&
        player.Is(SubFaction));

    public override void ReadRPC(MessageReader reader) => Shift(reader.ReadPlayer());
}