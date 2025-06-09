namespace TownOfUsReworked.PlayerLayers.Roles;

[LayerHeaderOption(LayerEnum.Shifter)]
public sealed class Shifter : Evil
{
    [NumberOption(10f, 60f, 2.5f, Format.Time)]
    private static Number ShiftCd = 25;

    public static List<PlayerControl> Shifters = [];
    public static List<PlayerControl> Originals = [];

    private CustomButton ShiftButton { get; set; }
    private CustomPlayerMenu ShifterMenu { get; set; }

    protected override UColor MainColor => CustomColorManager.Shifter;
    public override LayerEnum Type => LayerEnum.Shifter;
    public override Func<string> StartText { get; } = () => "Shift Around Roles";
    public override Func<string> Description => () => "- You can steal another player's role\n- Shifting with a non-<#8CFFFFFF>Crew</color> or a framed player will cause you to kill yourself";
    public override bool HasWon => Shifters.All(x => !Originals.Contains(x) && x.HasDied());
    public override WinLose EndState => WinLose.ShifterWins;

    protected override void Init()
    {
        base.Init();
        Alignment = Alignment.Support;
        ShifterMenu = new(Player, ClickShift, Color, Exception);
        ShiftButton ??= new(this, "SHIFT", new SpriteName("Shift"), AbilityTypes.Targetless, KeybindType.ActionSecondary, (OnClickTargetless)ShifterMenu.Open, new Cooldown(ShiftCd));
        Originals.Add(Player);
    }

    private void ClickShift(PlayerControl other)
    {
        var cooldown = Interact(Player, other, astral: true);

        if (cooldown != CooldownType.Fail)
        {
            var target = URandom.RandomRangeInt(0, 2) == 0 ? other : AllPlayers().Except(Exception).Random();
            CallRpc(CustomRPC.Action, ActionsRPC.LayerAction, this, target);
            Shift(target);
        }
        else
            ShiftButton.StartCooldown(cooldown);
    }

    private void Shift(PlayerControl other)
    {
        var role = other.GetRole();
        var player = Player;

        // TODO: Finish this
    }

    private bool Exception(PlayerControl player) => player.HasDied() || Player.IsBuddyWith(player, Faction) || player == Player;

    public override void ReadRPC(RpcReader reader) => Shift(reader.ReadPlayer());
}