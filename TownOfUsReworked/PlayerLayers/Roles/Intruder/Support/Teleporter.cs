namespace TownOfUsReworked.PlayerLayers.Roles;

[HeaderOption(MultiMenu.LayerSubOptions)]
public class Teleporter : Intruder
{
    [NumberOption(MultiMenu.LayerSubOptions, 10f, 60f, 2.5f, Format.Time)]
    public static Number TeleportCd { get; set; } = new(25);

    [NumberOption(MultiMenu.LayerSubOptions, 10f, 60f, 2.5f, Format.Time)]
    public static Number TeleMarkCd { get; set; } = new(25);

    [ToggleOption(MultiMenu.LayerSubOptions)]
    public static bool TeleCooldownsLinked { get; set; } = false;

    [ToggleOption(MultiMenu.LayerSubOptions)]
    public static bool TeleVent { get; set; } = false;

    public CustomButton TeleportButton { get; set; }
    public CustomButton MarkButton { get; set; }
    public Vector3 TeleportPoint { get; set; }

    public override UColor Color => ClientOptions.CustomIntColors ? CustomColorManager.Teleporter : CustomColorManager.Intruder;
    public override string Name => "Teleporter";
    public override LayerEnum Type => LayerEnum.Teleporter;
    public override Func<string> StartText => () => "X Marks The Spot";
    public override Func<string> Description => () => $"- You can mark a spot to teleport to later\n{CommonAbilities}";

    public override void Init()
    {
        BaseStart();
        Alignment = Alignment.IntruderSupport;
        TeleportPoint = Vector3.zero;
        MarkButton = CreateButton(this, new SpriteName("Mark"), AbilityTypes.Targetless, KeybindType.Secondary, (OnClick)Mark, new Cooldown(TeleMarkCd), "MARK POSITION",
            (ConditionFunc)Condition1);
        TeleportButton = CreateButton(this, new SpriteName("Teleport"), AbilityTypes.Targetless, KeybindType.Secondary, (OnClick)Teleport, new Cooldown(TeleportCd), "TELEPORT",
            (UsableFunc)Usable, (ConditionFunc)Condition2);
    }

    public void Mark()
    {
        TeleportPoint = Player.transform.position;
        MarkButton.StartCooldown();

        if (TeleCooldownsLinked)
            TeleportButton.StartCooldown();
    }

    public void Teleport()
    {
        Player.RpcCustomSnapTo(TeleportPoint);
        Flash(Color);
        TeleportButton.StartCooldown();

        if (TeleCooldownsLinked)
            MarkButton.StartCooldown();
    }

    public bool Condition1()
    {
        var hits = Physics2D.OverlapBoxAll(Player.transform.position, GetSize(), 0);
        hits = hits.Where(c => (c.name.Contains("Vent") || !c.isTrigger) && c.gameObject.layer is not (8 or 5)).ToArray();
        return hits.Count == 0 && Player.moveable && !GetPlayerElevator(Player).IsInElevator && TeleportPoint != Player.transform.position;
    }

    public bool Usable() => TeleportPoint != Vector3.zero;

    public bool Condition2() => Player.transform.position != TeleportPoint;
}