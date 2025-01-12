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

    public override UColor Color => ClientOptions.CustomCrewColors ? CustomColorManager.Shifter : FactionColor;
    public override LayerEnum Type => LayerEnum.Shifter;
    public override Func<string> StartText => () => "Shift Around Roles";
    public override Func<string> Description => () => "- You can steal another player's role\n- Shifting withn on-<#8CFFFFFF>Crew</color> will cause you to kill yourself";

    public override void Init()
    {
        base.Init();
        Alignment = Alignment.CrewSupport;
        ShifterMenu = new(Player, ClickShift, Exception);
        ShiftButton ??= new(this, "SHIFT", new SpriteName("Shift"), AbilityTypes.Targetless, KeybindType.ActionSecondary, (OnClickTargetless)ShifterMenu.Open, new Cooldown(ShiftCd));
    }

    public void ClickShift(PlayerControl other)
    {
        if (!other.Is<Crew>() || other.IsFramed())
        {
            MurderPlayer(Player);
            return;
        }

        var cooldown = Interact(Player, other, astral: true);

        if (cooldown != CooldownType.Fail)
        {
            CallRpc(CustomRPC.Action, ActionsRPC.LayerAction, this, other);
            Shift(other);
            Flash(Color);
        }
        else
            ShiftButton.StartCooldown(cooldown);
    }

    public void Shift(PlayerControl other)
    {
        var role = other.GetRole();

        if (other.AmOwner)
            Flash(Color);

        var player = Player;
        role.ClearArrows();
        role.Player = player;
        var historyClone = role.RoleHistory.Clone();
        role.RoleHistory.Clear();
        role.RoleHistory.AddRange(RoleHistory);
        role.RoleHistory.Add(LayerEnum.Shifter);

        if (ShiftedBecomes == BecomeEnum.Crewmate)
        {
            var crew = new Crewmate();
            crew.Start(other);
            crew.RoleHistory.AddRange(historyClone);
            crew.RoleHistory.Add(role.Type);
            End();
            CustomButton.AllButtons.Where(x => x.Owner == this || !x.Owner).ForEach(x => x.Destroy());
            ShifterMenu.Destroy();
        }
        else
        {
            Player = other;
            ShifterMenu.Owner = other;
            RoleHistory.Add(role.Type);
            RoleHistory.AddRange(historyClone);
        }

        if (player.Data.Role is LayerHandler handler)
            handler.SetUpLayers();

        if (other.Data.Role is LayerHandler handler1)
            handler1.SetUpLayers();
    }

    public bool Exception(PlayerControl player) => player.HasDied() || (Faction is Faction.Intruder or Faction.Syndicate && player.Is(Faction)) || (SubFaction != SubFaction.None &&
        player.Is(SubFaction)) || player == Player;

    public override void ReadRPC(MessageReader reader) => Shift(reader.ReadPlayer());
}