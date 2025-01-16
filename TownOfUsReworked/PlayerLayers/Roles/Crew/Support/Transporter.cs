namespace TownOfUsReworked.PlayerLayers.Roles;

[HeaderOption(MultiMenu.LayerSubOptions)]
public class Transporter : Crew, ITransporter
{
    [NumberOption(MultiMenu.LayerSubOptions, 0, 15, 1, zeroIsInf: true)]
    public static Number MaxTransports { get; set; } = new(5);

    [NumberOption(MultiMenu.LayerSubOptions, 10f, 60f, 2.5f, Format.Time)]
    public static Number TransportCd { get; set; } = new(25);

    [NumberOption(MultiMenu.LayerSubOptions, 1f, 20f, 1f, Format.Time)]
    public static Number TransportDur { get; set; } = new(5);

    [ToggleOption(MultiMenu.LayerSubOptions)]
    public static bool TransSelf { get; set; } = true;

    public CustomButton TransportButton { get; set; }
    public CustomPlayerMenu TransportMenu { get; set; }
    public bool Transporting { get; set; }

    public override UColor Color => ClientOptions.CustomCrewColors ? CustomColorManager.Transporter : FactionColor;
    public override LayerEnum Type => LayerEnum.Transporter;
    public override Func<string> StartText => () => "Swap Locations Of Players For Maximum Confusion";
    public override Func<string> Description => () => "- You can swap the locations of 2 players of your choice";

    public override void Init()
    {
        base.Init();
        Alignment = Alignment.CrewSupport;
        TransportButton ??= new(this, new SpriteName("Transport"), AbilityTypes.Targetless, KeybindType.ActionSecondary, (OnClickTargetless)Transport, MaxTransports, new Cooldown(TransportCd),
            (LabelFunc)Label);
        TransportMenu = new(Player, Click, Exception);
    }

    public string Label() => TransportMenu.Selected.Count switch
    {
        0 => "FIRST TARGET",
        1 => "SECOND TARGET",
        _ =>  "TRANSPORT"
    };

    public static IEnumerator TransportPlayers(PlayerControl transport1, PlayerControl transport2, ITransporter transporter)
    {
        var player1Body = (DeadBody)null;
        var player2Body = (DeadBody)null;
        var wasInVent1 = false;
        var wasInVent2 = false;
        var vent1 = (Vent)null;
        var vent2 = (Vent)null;

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

        Moving.Add(transport1.PlayerId, transport2.PlayerId);

        if (transport1.inVent)
        {
            while (GetInTransition())
                yield return EndFrame();

            transport1.MyPhysics.ExitAllVents();
            vent1 = transport1.GetClosestVent();
            wasInVent1 = true;
        }

        if (transport2.inVent)
        {
            while (GetInTransition())
                yield return EndFrame();

            transport2.MyPhysics.ExitAllVents();
            vent2 = transport2.GetClosestVent();
            wasInVent2 = true;
        }

        transporter.Transporting = true;

        if (!transport1.HasDied())
        {
            transport1.moveable = false;
            transport1.NetTransform.Halt();
            transport1.MyPhysics.ResetMoveState();
            transport1.MyPhysics.ResetAnimState();
            transport1.MyPhysics.StopAllCoroutines();
        }

        if (!transport2.HasDied())
        {
            transport2.moveable = false;
            transport2.NetTransform.Halt();
            transport2.MyPhysics.ResetMoveState();
            transport2.MyPhysics.ResetAnimState();
            transport2.MyPhysics.StopAllCoroutines();
        }

        if (transport1.AmOwner || transport2.AmOwner)
            Flash(transporter.Color, TransportDur);

        if (!player1Body && !wasInVent1)
            AnimatePortal(transport1, TransportDur);

        if (!player2Body && !wasInVent2)
            AnimatePortal(transport2, TransportDur);

        var startTime = Time.time;

        while (true)
        {
            if (Time.time - startTime < TransportDur)
                yield return EndFrame();
            else
                break;

            if (Meeting())
            {
                Moving.RemoveAll(x => x == transport1.PlayerId || x == transport2.PlayerId);
                transporter.Transporting = false;
                yield break;
            }
        }

        if (transport1.Data.IsDead)
        {
            player1Body = BodyById(transport1.PlayerId);

            if (!player1Body)
            {
                Moving.RemoveAll(x => x == transport1.PlayerId || x == transport2.PlayerId);
                transporter.Transporting = false;
                yield break;
            }
        }

        if (transport2.Data.IsDead)
        {
            player2Body = BodyById(transport2.PlayerId);

            if (!player2Body)
            {
                Moving.RemoveAll(x => x == transport1.PlayerId || x == transport2.PlayerId);
                transporter.Transporting = false;
                yield break;
            }
        }

        if (!player1Body && !player2Body)
        {
            var tempPos = transport1.GetTruePosition();
            transport1.CustomSnapTo(new(transport2.GetTruePosition().x, transport2.GetTruePosition().y + 0.3636f));
            transport2.CustomSnapTo(new(tempPos.x, tempPos.y + 0.3636f));

            if (transport1.CanVent() && vent2 && wasInVent2)
                transport1.MyPhysics.RpcEnterVent(vent2.Id);

            if (transport2.CanVent() && vent1 && wasInVent1)
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
                Map().Close();
        }

        Moving.RemoveAll(x => x == transport1.PlayerId || x == transport2.PlayerId);
        transporter.Transporting = false;
    }

    public bool Click(PlayerControl player, out bool shouldClose)
    {
        var cooldown = Interact(Player, player);
        shouldClose = false;

        if (cooldown != CooldownType.Fail)
            return true;
        else
            TransportButton.StartCooldown(cooldown);

        shouldClose = true;
        return false;
    }

    public bool Exception(PlayerControl player) => (player == Player && !TransSelf) || UninteractiblePlayers.ContainsKey(player.PlayerId) || player.IsMoving() || (!BodyById(player.PlayerId)
        && player.Data.IsDead);

    public void Transport()
    {
        if (TransportMenu.Selected.Count < 2)
        {
            TransportMenu.Open();
            TransportButton.Uses++;
        }
        else
        {
            CallRpc(CustomRPC.Action, ActionsRPC.LayerAction, this, TransportMenu.Selected[0], TransportMenu.Selected[1]);
            Coroutines.Start(TransportPlayers(PlayerById(TransportMenu.Selected[0]), PlayerById(TransportMenu.Selected[1]), this));
            TransportMenu.Selected.Clear();
            TransportButton.StartCooldown();
        }
    }

    public override void ReadRPC(MessageReader reader) => Coroutines.Start(TransportPlayers(reader.ReadPlayer(), reader.ReadPlayer(), this));

    public override void UpdateHud(HudManager __instance)
    {
        base.UpdateHud(__instance);

        if (KeyboardJoystick.player.GetButtonDown("Delete"))
        {
            if (!Transporting && TransportMenu.Selected.Count > 0)
                TransportMenu.Selected.TakeLast();

            Message("Removed a target");
        }
    }
}