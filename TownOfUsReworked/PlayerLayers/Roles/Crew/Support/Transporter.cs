namespace TownOfUsReworked.PlayerLayers.Roles;

[LayerHeaderOption(Layer.Transporter)]
public sealed class Transporter : CSupport, IMover
{
    [NumberOption(0, 15, 1, zeroIsInf: true)]
    public static Number MaxTransports = 5;

    [NumberOption(10f, 60f, 2.5f, Format.Time)]
    public static Number TransportCd = 25;

    [NumberOption(1f, 20f, 1f, Format.Time)]
    private static Number TransportDur = 5;

    [ToggleOption]
    public static bool TransSelf = true;

    private CustomButton TransportButton;
    private CustomPlayerMenu TransportMenu;
    public bool Moving { get; set; }

    protected override UColor MainColor => CustomColorManager.Transporter;
    public override Layer Type => Layer.Transporter;
    public override string StartText => "Swap Locations Of Players For Maximum Confusion";
    public override string Description => "- You can swap the locations of 2 players of your choice";

    public override void Init()
    {
        TransportButton ??= new(this, new SpriteName("Transport"), ReworkedAbilityTypes.Targetless, KeybindType.ActionSecondary, (OnClickTargetless)Transport, MaxTransports, new Cooldown(TransportCd),
            (LabelFunc)Label);
        TransportMenu = new(Player, Click, Color, Exception);
    }

    private string Label() => TransportMenu.Selected.Count switch
    {
        < 2 => "SELECT TARGETS",
        _ =>  "TRANSPORT"
    };

    public static IEnumerator TransportPlayers(PlayerControl transport1, PlayerControl transport2, IMover transporter)
    {
        DeadBody player1Body = null;
        DeadBody player2Body = null;
        Vent vent1 = null;
        Vent vent2 = null;

        if (transport1.Data.IsDead)
        {
            player1Body = BodyById(transport1.PlayerId);

            if (!player1Body)
                yield break;
        }

        if (transport2.Data.IsDead)
        {
            player2Body = BodyById(transport2.PlayerId);

            if (!player2Body)
                yield break;
        }

        References.Moving.AddRange(transport1.PlayerId, transport2.PlayerId);

        if (transport1.inVent)
        {
            while (GetInTransition())
                yield return null;

            transport1.MyPhysics.ExitAllVents();
            vent1 = transport1.GetClosestVent();
        }

        if (transport2.inVent)
        {
            while (GetInTransition())
                yield return null;

            transport2.MyPhysics.ExitAllVents();
            vent2 = transport2.GetClosestVent();
        }

        transporter.Moving = true;

        if (transport1.AmOwner || transport2.AmOwner)
            Flash(transporter.Color, TransportDur);

        if (!player1Body)
            AnimatePortal(transport1, TransportDur);

        if (!player2Body)
            AnimatePortal(transport2, TransportDur);

        var startTime = Time.time;

        while (Time.time - startTime < TransportDur)
        {
            yield return null;

            if (!Meeting())
                continue;

            References.Moving.RemoveAll(x => x == transport1.PlayerId || x == transport2.PlayerId);
            transporter.Moving = false;
            yield break;
        }

        if (transport1.Data.IsDead)
        {
            player1Body = BodyById(transport1.PlayerId);

            if (!player1Body)
            {
                References.Moving.RemoveAll(x => x == transport1.PlayerId || x == transport2.PlayerId);
                transporter.Moving = false;
                yield break;
            }
        }

        if (transport2.Data.IsDead)
        {
            player2Body = BodyById(transport2.PlayerId);

            if (!player2Body)
            {
                References.Moving.RemoveAll(x => x == transport1.PlayerId || x == transport2.PlayerId);
                transporter.Moving = false;
                yield break;
            }
        }

        if (!player1Body && !player2Body)
        {
            var tempPos = transport1.GetTruePosition();
            transport1.CustomSnapTo(new(transport2.GetTruePosition().x, transport2.GetTruePosition().y + 0.3636f));
            transport2.CustomSnapTo(new(tempPos.x, tempPos.y + 0.3636f));

            if (transport1.CanVent() && vent2)
                transport1.MyPhysics.RpcEnterVent(vent2.Id);

            if (transport2.CanVent() && vent1)
                transport2.MyPhysics.RpcEnterVent(vent1.Id);
        }
        else if (player1Body && !player2Body)
        {
            StopDragging(player1Body.ParentId);
            var tempPos = player1Body.TruePosition;
            player1Body.transform.position = transport2.GetTruePosition();
            transport2.CustomSnapTo(new(tempPos.x, tempPos.y + 0.3636f));
        }
        else if (!player1Body && player2Body)
        {
            StopDragging(player2Body.ParentId);
            var tempPos = transport1.GetTruePosition();
            transport1.CustomSnapTo(new(player2Body.TruePosition.x, player2Body.TruePosition.y + 0.3636f));
            player2Body.transform.position = tempPos;
        }
        else if (player1Body && player2Body)
        {
            StopDragging(player1Body.ParentId);
            StopDragging(player2Body.ParentId);
            (player1Body.transform.position, player2Body.transform.position) = (player2Body.TruePosition, player1Body.TruePosition);
        }

        if (transport1.AmOwner || transport2.AmOwner)
        {
            if (ActiveTask())
                ActiveTask().Close();

            if (MapBehaviourPatches.MapActive)
                Minimap().Close();
        }

        References.Moving.RemoveAll(x => x == transport1.PlayerId || x == transport2.PlayerId);
        transporter.Moving = false;
    }

    private bool Click(PlayerControl player, out bool shouldClose)
    {
        shouldClose = false;

        if (player.IsMoving())
            return false;

        var cooldown = Interact(Player, player);

        if (cooldown != CooldownType.Fail)
            return true;

        TransportButton.StartCooldown(cooldown);
        shouldClose = true;
        return false;
    }

    private bool Exception(PlayerControl player) => (player == Player && !TransSelf) || UninteractablePlayers.ContainsKey(player.PlayerId) || player.IsMoving() || (!BodyById(player.PlayerId)
        && player.Data.IsDead);

    private void Transport()
    {
        if (TransportMenu.Selected.Count < 2)
        {
            TransportMenu.Open();
            TransportButton.Uses++;
        }
        else
        {
            PerformRpcAction(TransportMenu.Selected[0], TransportMenu.Selected[1]);
            Coroutines.Start(TransportPlayers(PlayerById(TransportMenu.Selected[0]), PlayerById(TransportMenu.Selected[1]), this));
            TransportMenu.Selected.Clear();
            TransportButton.StartCooldown();
        }
    }

    public override void ReadRPC(RpcReader reader) => Coroutines.Start(TransportPlayers(reader.ReadPlayer(), reader.ReadPlayer(), this));

    public override void UpdateHud(HudManager __instance)
    {
        if (!KeyboardJoystick.player.GetButtonDown("Delete"))
            return;

        if (!Moving && TransportMenu.Selected.Count > 0)
            TransportMenu.Selected.TakeLast();

        Message("Removed a target");
    }
}