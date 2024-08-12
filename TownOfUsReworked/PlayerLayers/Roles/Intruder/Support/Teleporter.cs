namespace TownOfUsReworked.PlayerLayers.Roles;

[HeaderOption(MultiMenu2.LayerSubOptions)]
public class Teleporter : Intruder
{
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
        MarkButton = CreateButton(this, new SpriteName("Mark"), AbilityTypes.Targetless, KeybindType.Secondary, (OnClick)Mark, new Cooldown(CustomGameOptions.TeleMarkCd), "MARK POSITION",
            (ConditionFunc)Condition1);
        TeleportButton = CreateButton(this, new SpriteName("Teleport"), AbilityTypes.Targetless, KeybindType.Secondary, (OnClick)Teleport, new Cooldown(CustomGameOptions.TeleportCd),
            "TELEPORT", (UsableFunc)Usable, (ConditionFunc)Condition2);
    }

    public void Mark()
    {
        TeleportPoint = Player.transform.position;
        MarkButton.StartCooldown();

        if (CustomGameOptions.TeleCooldownsLinked)
            TeleportButton.StartCooldown();
    }

    public void Teleport()
    {
        Player.RpcCustomSnapTo(TeleportPoint);
        Flash(Color);
        TeleportButton.StartCooldown();

        if (CustomGameOptions.TeleCooldownsLinked)
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