namespace TownOfUsReworked.PlayerLayers.Roles;

[HeaderOption(MultiMenu.LayerSubOptions)]
public class Detective : Crew
{
    [NumberOption(MultiMenu.LayerSubOptions, 10f, 60f, 2.5f, Format.Time)]
    public static Number ExamineCd { get; set; } = new(25);

    [NumberOption(MultiMenu.LayerSubOptions, 5f, 60f, 2.5f, Format.Time)]
    public static Number RecentKill { get; set; } = new(10);

    [NumberOption(MultiMenu.LayerSubOptions, 0.05f, 2f, 0.05f, Format.Time)]
    public static Number FootprintInterval { get; set; } = new(0.15f);

    [NumberOption(MultiMenu.LayerSubOptions, 1f, 10f, 0.5f, Format.Time)]
    public static Number FootprintDur { get; set; } = new(10);

    [StringOption(MultiMenu.LayerSubOptions)]
    public static FootprintVisibility AnonymousFootPrint { get; set; } = FootprintVisibility.OnlyWhenCamouflaged;

    public CustomButton ExamineButton { get; set; }

    public override UColor Color => ClientOptions.CustomCrewColors ? CustomColorManager.Detective : CustomColorManager.Crew;
    public override string Name => "Detective";
    public override LayerEnum Type => LayerEnum.Detective;
    public override Func<string> StartText => () => "Examine Players For <#AA0000FF>Blood</color>";
    public override Func<string> Description => () => "- You can examine players to see if they have killed recently\n- Your screen will flash red if your target has killed in the last " +
        $"{RecentKill}s\n- You can view everyone's footprints to see where they go or where they came from";

    public override void Init()
    {
        base.Init();
        Alignment = Alignment.CrewInvest;
        ExamineButton ??= new(this, "EXAMINE", new SpriteName("Examine"), AbilityTypes.Alive, KeybindType.ActionSecondary, (OnClickPlayer)Examine, new Cooldown(ExamineCd));
    }

    public void Examine(PlayerControl target)
    {
        var cooldown = Interact(Player, target);

        if (cooldown != CooldownType.Fail)
        {
            Flash(target.IsFramed() || KilledPlayers.Any(x => x.KillerId == target.PlayerId && (DateTime.UtcNow - x.KillTime).TotalSeconds <= RecentKill) ? UColor.red : UColor.green);
            target.EnsureComponent<FootprintHandler>();
        }

        ExamineButton.StartCooldown(cooldown);
    }
}