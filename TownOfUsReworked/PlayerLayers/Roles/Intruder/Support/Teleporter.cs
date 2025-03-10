namespace TownOfUsReworked.PlayerLayers.Roles;

[LayerHeaderOption(LayerEnum.Teleporter)]
public sealed class Teleporter : Intruder, IMover
{
    [NumberOption(10f, 60f, 2.5f, Format.Time)]
    public static Number TeleportCd = 25;

    [NumberOption(10f, 60f, 2.5f, Format.Time)]
    public static Number TeleMarkCd = 25;

    [NumberOption(1f, 20f, 1f, Format.Time)]
    private static Number TeleportDur = 5;

    [ToggleOption]
    public static bool TeleCooldownsLinked = false;

    [ToggleOption]
    public static bool TeleVent = false;

    private CustomButton TeleportButton { get; set; }
    private CustomButton MarkButton { get; set; }
    public Vector2 TeleportPoint { get; set; }
    public bool Moving { get; set; }

    public override UColor Color => ClientOptions.CustomIntColors ? CustomColorManager.Teleporter : FactionColor;
    public override LayerEnum Type => LayerEnum.Teleporter;
    public override Func<string> StartText => () => "X Marks The Spot";
    public override Func<string> Description => () => $"- You can mark a spot to teleport to later\n{CommonAbilities}";

    protected override void Init()
    {
        base.Init();
        Alignment = Alignment.Support;
        TeleportPoint = Vector2.zero;
        MarkButton ??= new(this, new SpriteName("Mark"), AbilityTypes.Targetless, KeybindType.Secondary, (OnClickTargetless)Mark, new Cooldown(TeleMarkCd), "MARK POSITION",
            (ConditionFunc)Condition1);
        TeleportButton ??= new(this, new SpriteName("Teleport"), AbilityTypes.Targetless, KeybindType.Secondary, (OnClickTargetless)Teleport, new Cooldown(TeleportCd), "TELEPORT",
            (UsableFunc)Usable, (ConditionFunc)Condition2);
    }

    public override void Reset(bool meeting, bool start) => TeleportPoint = Vector2.zero;

    private void Mark()
    {
        TeleportPoint = Player.transform.position;
        MarkButton.StartCooldown();

        if (TeleCooldownsLinked)
            TeleportButton.StartCooldown();
    }

    private void Teleport()
    {
        CallRpc(CustomRPC.Action, ActionsRPC.LayerAction, this, TeleportPoint);
        Coroutines.Start(TeleportPlayer(TeleportPoint, this));
        TeleportButton.StartCooldown();

        if (TeleCooldownsLinked)
            MarkButton.StartCooldown();
    }

    public override void ReadRPC(MessageReader reader) => Coroutines.Start(TeleportPlayer(reader.ReadVector2(), this));

    private bool Condition1() => !Physics2D.OverlapBoxAll(Player.transform.position, GetSize(), 0).Any(c => (c.name.Contains("Vent") || !c.isTrigger) && c.gameObject.layer is not (8 or 5)) &&
        Player.moveable && !GetPlayerElevator(Player).IsInElevator && TeleportPoint != (Vector2)Player.transform.position;

    private bool Usable() => TeleportPoint != Vector2.zero;

    private bool Condition2() => Vector2.Distance(Player.transform.position, TeleportPoint) <= 1f && !Moving;

    public static IEnumerator TeleportPlayer(Vector2 point, IMover teleporter)
    {
        var player = teleporter.Player;
        DeadBody playerBody;

        if (player.Data.IsDead)
        {
            playerBody = BodyById(player.PlayerId);

            if (!playerBody)
                yield break;
        }

        References.Moving.Add(player.PlayerId);

        if (player.inVent)
        {
            while (GetInTransition())
                yield return EndFrame();

            player.MyPhysics.ExitAllVents();
        }

        teleporter.Moving = true;

        if (player.AmOwner)
            Flash(teleporter.Color, TeleportDur);

        AnimatePortal(player, TeleportDur);
        var startTime = Time.time;

        while (Time.time - startTime <= TeleportDur)
        {
            yield return EndFrame();

            if (!Meeting())
                continue;

            References.Moving.RemoveAll(x => x == player.PlayerId);
            teleporter.Moving = false;
            yield break;
        }

        if (player.Data.IsDead)
        {
            playerBody = BodyById(player.PlayerId);

            if (!playerBody)
            {
                References.Moving.RemoveAll(x => x == player.PlayerId);
                teleporter.Moving = false;
                yield break;
            }
        }

        player.CustomSnapTo(new(point.x, point.y + 0.3636f));

        References.Moving.RemoveAll(x => x == player.PlayerId);
        teleporter.Moving = false;
    }
}