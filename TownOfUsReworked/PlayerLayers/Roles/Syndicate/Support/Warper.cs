namespace TownOfUsReworked.PlayerLayers.Roles;

[HeaderOption(MultiMenu.LayerSubOptions)]
public class Warper : Syndicate, IWarper
{
    [NumberOption(MultiMenu.LayerSubOptions, 10f, 60f, 2.5f, Format.Time)]
    public static Number WarpCd { get; set; } = new(25);

    [NumberOption(MultiMenu.LayerSubOptions, 1f, 20f, 1f, Format.Time)]
    public static Number WarpDur { get; set; } = new(5);

    [ToggleOption(MultiMenu.LayerSubOptions)]
    public static bool WarpSelf { get; set; } = true;

    public CustomButton WarpButton { get; set; }
    public CustomPlayerMenu WarpMenu { get; set; }
    public bool Warping { get; set; }

    public override UColor Color => ClientOptions.CustomSynColors ? CustomColorManager.Warper : FactionColor;
    public override LayerEnum Type => LayerEnum.Warper;
    public override Func<string> StartText => () => "Warp The <#8CFFFFFF>Crew</color> Away From Each Other";
    public override Func<string> Description => () => "- You can warp a" + (HoldsDrive ? "ll players, forcing them to be teleported to random locations" :
        " player to another player of your choice") + $"\n{CommonAbilities}";

    public override void Init()
    {
        base.Init();
        Alignment = Alignment.SyndicateSupport;
        WarpMenu = new(Player, Click, Exception1);
        WarpButton ??= new(this, new SpriteName("Warp"), AbilityTypes.Targetless, KeybindType.ActionSecondary, (OnClickTargetless)Warp, new Cooldown(WarpCd), (LabelFunc)Label);
    }

    public static IEnumerator WarpPlayers(PlayerControl player1, PlayerControl player2, IWarper warper)
    {
        var player1Body = (DeadBody)null;
        var player2Body = (DeadBody)null;
        var wasInVent = false;
        var vent = (Vent)null;

        if (player1.Data.IsDead)
        {
            player1Body = BodyById(player1.PlayerId);

            if (!player1Body)
                yield break;
        }

        if (player2.Data.IsDead)
        {
            player2Body = BodyById(player2.PlayerId);

            if (!player2Body)
                yield break;
        }

        Moving.Add(player1.PlayerId);

        if (player1.inVent)
        {
            while (GetInTransition())
                yield return EndFrame();

            player1.MyPhysics.ExitAllVents();
        }

        if (player2.inVent)
        {
            while (GetInTransition())
                yield return EndFrame();

            vent = player2.GetClosestVent();
            wasInVent = true;
        }

        warper.Warping = true;

        if (!player1.HasDied())
        {
            player1.moveable = false;
            player1.NetTransform.Halt();
            player1.MyPhysics.ResetMoveState();
            player1.MyPhysics.ResetAnimState();
            player1.MyPhysics.StopAllCoroutines();
        }

        if (player1.AmOwner)
            Flash(warper.Color, WarpDur);

        if (!player1Body && !wasInVent)
            AnimatePortal(player1, WarpDur);

        var startTime = Time.time;

        while (true)
        {
            if (Time.time - startTime < WarpDur)
                yield return EndFrame();
            else
                break;

            if (Meeting())
            {
                Moving.RemoveAll(x => x == player1.PlayerId);
                warper.Warping = false;
                yield break;
            }
        }

        if (player1.Data.IsDead)
        {
            player1Body = BodyById(player1.PlayerId);

            if (!player1Body)
            {
                Moving.RemoveAll(x => x == player1.PlayerId);
                warper.Warping = false;
                yield break;
            }
        }

        if (player2.Data.IsDead)
        {
            player2Body = BodyById(player2.PlayerId);

            if (!player2Body)
            {
                Moving.RemoveAll(x => x == player1.PlayerId);
                warper.Warping = false;
                yield break;
            }
        }

        if (!player1Body && !player2Body)
        {
            player1.CustomSnapTo(new(player2.GetTruePosition().x, player2.GetTruePosition().y + 0.3636f));

            if (player1.CanVent() && vent && wasInVent)
                player1.MyPhysics.RpcEnterVent(vent.Id);
        }
        else if (player1Body && !player2Body)
        {
            StopDragging(player1Body.ParentId);
            player1Body.transform.position = player2.GetTruePosition();
        }
        else if (!player1Body && player2Body)
        {
            player1.CustomSnapTo(new(player2Body.TruePosition.x, player2Body.TruePosition.y + 0.3636f));

            if (player1.CanVent() && vent && wasInVent)
                player1.MyPhysics.RpcEnterVent(vent.Id);
        }
        else if (player1Body && player2Body)
        {
            StopDragging(player1Body.ParentId);
            player1Body.transform.position = player2Body.TruePosition;
        }

        Moving.RemoveAll(x => x == player1.PlayerId);
        warper.Warping = false;
    }

    public bool Click(PlayerControl player, out bool shouldClose)
    {
        var cooldown = Interact(Player, player);
        shouldClose = false;

        if (cooldown != CooldownType.Fail)
            return true;
        else
            WarpButton.StartCooldown(cooldown);

        shouldClose = true;
        return false;
    }

    public bool Exception1(PlayerControl player) => (player == Player && !WarpSelf) || UninteractiblePlayers.ContainsKey(player.PlayerId) || player.IsMoving() || (!BodyById(player.PlayerId) &&
        player.Data.IsDead);

    public static void WarpAll()
    {
        var coords = GenerateWarpCoordinates();

        foreach (var (id, pos) in coords)
        {
            var player = PlayerById(id);
            var body = BodyById(id);

            if (body)
            {
                body.transform.position = pos;
                CallRpc(CustomRPC.Misc, MiscRPC.MoveBody, body, pos);
            }
            else
                player.RpcCustomSnapTo(pos);
        }
    }

    public void Warp()
    {
        if (HoldsDrive)
        {
            WarpAll();
            WarpButton.StartCooldown();
        }
        else if (WarpMenu.Selected.Count < 2)
            WarpMenu.Open();
        else
        {
            CallRpc(CustomRPC.Action, ActionsRPC.LayerAction, this, WarpMenu.Selected[0], WarpMenu.Selected[1]);
            Coroutines.Start(WarpPlayers(PlayerById(WarpMenu.Selected[0]), PlayerById(WarpMenu.Selected[1]), this));
            WarpMenu.Selected.Clear();
            WarpButton.StartCooldown();
        }
    }

    public override void ReadRPC(MessageReader reader) => Coroutines.Start(WarpPlayers(reader.ReadPlayer(), reader.ReadPlayer(), this));

    public string Label()
    {
        if (HoldsDrive)
            return "WARP";
        else
        {
            return WarpMenu.Selected.Count switch
            {
                0 => "FIRST TARGET",
                1 => "SECOND TARGET",
                _ =>  "WARP"
            };
        }
    }

    public override void UpdateHud(HudManager __instance)
    {
        base.UpdateHud(__instance);

        if (KeyboardJoystick.player.GetButtonDown("Delete"))
        {
            if (!HoldsDrive && !Warping && WarpMenu.Selected.Count > 0)
                WarpMenu.Selected.TakeLast();

            Message("Removed a target");
        }
    }
}