namespace TownOfUsReworked.PlayerLayers.Roles;

[LayerHeaderOption(LayerEnum.Shifter)]
public sealed class Shifter : Crew
{
    [NumberOption(10f, 60f, 2.5f, Format.Time)]
    private static Number ShiftCd = 25;

    [StringOption<BecomeEnum>]
    private static BecomeEnum ShiftedBecomes = BecomeEnum.Shifter;

    private CustomButton ShiftButton { get; set; }
    private CustomPlayerMenu ShifterMenu { get; set; }

    protected override UColor MainColor => CustomColorManager.Shifter;
    public override LayerEnum Type { get; } = LayerEnum.Shifter;
    public override Func<string> StartText { get; } = () => "Shift Around Roles";
    public override Func<string> Description => () => "- You can steal another player's role\n- Shifting with a non-<#8CFFFFFF>Crew</color> or a framed player will cause you to kill yourself";

    protected override void Init()
    {
        base.Init();
        Alignment = Alignment.Support;
        ShifterMenu = new(Player, ClickShift, Color, Exception);
        ShiftButton ??= new(this, "SHIFT", new SpriteName("Shift"), AbilityTypes.Targetless, KeybindType.ActionSecondary, (OnClickTargetless)ShifterMenu.Open, new Cooldown(ShiftCd));
    }

    private void ClickShift(PlayerControl other)
    {
        if (!other.Is<Crew>() || other.IsFramed())
        {
            Player.Suicide();
            return;
        }

        var cooldown = Interact(Player, other, astral: true);

        if (cooldown != CooldownType.Fail)
        {
            CallRpc(CustomRPC.Action, ActionsRPC.LayerAction, this, other);
            Shift(other);
        }
        else
            ShiftButton.StartCooldown(cooldown);
    }

    private void Shift(PlayerControl other)
    {
        var role = other.GetRole();

        if (other.AmOwner || Local)
            Flash(Color);

        var player = Player;
        role.ClearArrows();
        role.Player = player;
        var historyClone = role.RoleHistory.Clone();
        role.RoleHistory.Clear();
        role.RoleHistory.AddRange(RoleHistory);
        role.RoleHistory.Add(LayerEnum.Shifter);
        ShiftButton.Destroy();

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
            ShiftButton ??= new(this, "SHIFT", new SpriteName("Shift"), AbilityTypes.Targetless, KeybindType.ActionSecondary, (OnClickTargetless)ShifterMenu.Open, new Cooldown(ShiftCd));
        }

        if (player.Data.Role is LayerHandler handler)
            handler.SetUpLayers();

        if (other.Data.Role is LayerHandler handler1)
            handler1.SetUpLayers();
    }

    private bool Exception(PlayerControl player) => player.HasDied() || (Faction is not (Faction.Crew or Faction.Neutral) && player.Is(Faction)) || (SubFaction != SubFaction.None &&
        player.Is(SubFaction)) || player == Player;

    public override void ReadRPC(NetData reader) => Shift(reader.ReadPlayer());
}