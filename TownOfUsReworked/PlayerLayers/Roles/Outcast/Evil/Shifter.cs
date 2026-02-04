namespace TownOfUsReworked.PlayerLayers.Roles;

[LayerHeaderOption(Layer.Shifter)]
public sealed class Shifter : Evil
{
    [NumberOption(10f, 60f, 2.5f, Format.Time)]
    private static Number ShiftCd = 25;

    public static List<PlayerControl> Shifters = [];
    public static List<PlayerControl> Originals = [];

    private CustomButton ShiftButton;
    private CustomPlayerMenu ShifterMenu;

    protected override UColor MainColor => CustomColorManager.Shifter;
    public override Layer Type => Layer.Shifter;
    public override string StartText => "Shift Around Roles";
    public override string Description => "- You can steal another player's role\n- Shifting with a non-<#8CFFFFFF>Crew</color> or a framed player will cause you to kill yourself";
    public override bool HasWon => Shifters.All(x => !Originals.Contains(x) && x.HasDied());
    public override Faction BaseFaction => Faction.Shifter;
    protected override WinLose EndState => WinLose.ShifterWins;

    public override void Init()
    {
        ShifterMenu = new(Player, ClickShift, Color, Exception);
        ShiftButton ??= new(this, "SHIFT", new SpriteName("Shift"), ReworkedAbilityTypes.Targetless, KeybindType.ActionSecondary, (OnClickTargetless)ShifterMenu.Open, new Cooldown(ShiftCd));
        Objectives = () => "- Kill off everyone who has the Shifter role, excluding those who were originally Shifter";
    }

    private void ClickShift(PlayerControl other)
    {
        var cooldown = Interact(Player, other, astral: true);

        if (cooldown != CooldownType.Fail)
        {
            var target = URandom.RandomRangeInt(0, 2) == 0 ? other : AllPlayers().Except(Exception).Random();
            PerformRpcAction(target);
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
        Shifters.Remove(Player);
        Shifters.Add(other);
    }

    private bool Exception(PlayerControl player) => player.HasDied() || Player.IsBuddyWith(player, Handler.CurrentFaction) || player == Player;

    public override void ReadRPC(RpcReader reader) => Shift(reader.ReadPlayer());
}