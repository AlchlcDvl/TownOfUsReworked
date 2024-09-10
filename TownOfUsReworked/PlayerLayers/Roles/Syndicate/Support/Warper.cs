namespace TownOfUsReworked.PlayerLayers.Roles;

[HeaderOption(MultiMenu.LayerSubOptions)]
public class Warper : Syndicate
{
    [NumberOption(MultiMenu.LayerSubOptions, 10f, 60f, 2.5f, Format.Time)]
    public static Number WarpCd { get; set; } = new(25);

    [NumberOption(MultiMenu.LayerSubOptions, 1f, 20f, 1f, Format.Time)]
    public static Number WarpDur { get; set; } = new(5);

    [ToggleOption(MultiMenu.LayerSubOptions)]
    public static bool WarpSelf { get; set; } = true;

    public CustomButton WarpButton { get; set; }
    public PlayerControl WarpPlayer1 { get; set; }
    public PlayerControl WarpPlayer2 { get; set; }
    public CustomMenu WarpMenu1 { get; set; }
    public CustomMenu WarpMenu2 { get; set; }
    public SpriteRenderer AnimationPlaying { get; set; }
    public GameObject WarpObj { get; set; }
    public bool Warping { get; set; }
    public DeadBody Player1Body { get; set; }
    public DeadBody Player2Body { get; set; }
    public bool WasInVent { get; set; }
    public Vent Vent { get; set; }

    public override UColor Color => ClientOptions.CustomSynColors ? CustomColorManager.Warper : CustomColorManager.Syndicate;
    public override string Name => "Warper";
    public override LayerEnum Type => LayerEnum.Warper;
    public override Func<string> StartText => () => "Warp The <color=#8CFFFFFF>Crew</color> Away From Each Other";
    public override Func<string> Description => () => "- You can warp a" + (HoldsDrive ? "ll players, forcing them to be teleported to random locations" :
        " player to another player of your choice") + $"\n{CommonAbilities}";

    public override void Init()
    {
        BaseStart();
        Alignment = Alignment.SyndicateSupport;
        WarpPlayer1 = null;
        WarpPlayer2 = null;
        WarpMenu1 = new(Player, Click1, Exception1);
        WarpMenu2 = new(Player, Click2, Exception2);
        WarpButton = CreateButton(this, new SpriteName("Warp"), AbilityTypes.Targetless, KeybindType.ActionSecondary, (OnClick)Warp, new Cooldown(WarpCd), (LabelFunc)Label);
        Player1Body = null;
        Player2Body = null;
        WasInVent = false;
        Vent = null;
        WarpObj = new("Warp") { layer = 5 };
        WarpObj.AddSubmergedComponent("ElevatorMover");
        WarpObj.transform.position = new(Player.GetTruePosition().x, Player.GetTruePosition().y, (Player.GetTruePosition().y / 1000f) + 0.01f);
        AnimationPlaying = WarpObj.AddComponent<SpriteRenderer>();
        AnimationPlaying.sprite = PortalAnimation[0];
        AnimationPlaying.material = HatManager.Instance.PlayerMaterial;
        WarpObj.SetActive(true);
        Data.Role.IntroSound = GetAudio("WarperIntro");
    }

    public IEnumerator WarpPlayers()
    {
        Player1Body = null;
        Player2Body = null;
        WasInVent = false;
        Vent = null;

        if (WarpPlayer1.Data.IsDead)
        {
            Player1Body = BodyById(WarpPlayer1.PlayerId);

            if (!Player1Body)
                yield break;
        }

        if (WarpPlayer2.Data.IsDead)
        {
            Player2Body = BodyById(WarpPlayer2.PlayerId);

            if (!Player2Body)
                yield break;
        }

        if (WarpPlayer1.inVent)
        {
            while (GetInTransition())
                yield return EndFrame();

            WarpPlayer1.MyPhysics.ExitAllVents();
        }

        if (WarpPlayer2.inVent)
        {
            while (GetInTransition())
                yield return EndFrame();

            Vent = WarpPlayer2.GetClosestVent();
            WasInVent = true;
        }

        Warping = true;

        if (!WarpPlayer1.HasDied())
        {
            WarpPlayer1.moveable = false;
            WarpPlayer1.NetTransform.Halt();
        }

        if (CustomPlayer.Local == WarpPlayer1)
            Flash(Color, WarpDur);

        if (!Player1Body && !WasInVent)
            AnimateWarp();

        var startTime = DateTime.UtcNow;

        while (true)
        {
            var seconds = (DateTime.UtcNow - startTime).TotalSeconds;

            if (seconds < WarpDur)
                yield return EndFrame();
            else
                break;

            if (Meeting())
            {
                AnimationPlaying.sprite = PortalAnimation[0];
                yield break;
            }
        }

        if (!Player1Body && !Player2Body)
        {
            WarpPlayer1.MyPhysics.ResetMoveState();
            WarpPlayer1.CustomSnapTo(new(WarpPlayer2.GetTruePosition().x, WarpPlayer2.GetTruePosition().y + 0.3636f));

            if (IsSubmerged() && CustomPlayer.Local == WarpPlayer1)
            {
                ChangeFloor(WarpPlayer1.GetTruePosition().y > -7);
                CheckOutOfBoundsElevator(CustomPlayer.Local);
            }

            if (WarpPlayer1.CanVent() && Vent && WasInVent)
                WarpPlayer1.MyPhysics.RpcEnterVent(Vent.Id);
        }
        else if (Player1Body && !Player2Body)
        {
            StopDragging(Player1Body.ParentId);
            Player1Body.transform.position = WarpPlayer2.GetTruePosition();

            if (IsSubmerged() && CustomPlayer.Local == WarpPlayer2)
            {
                ChangeFloor(WarpPlayer2.GetTruePosition().y > -7);
                CheckOutOfBoundsElevator(CustomPlayer.Local);
            }
        }
        else if (!Player1Body && Player2Body)
        {
            WarpPlayer1.MyPhysics.ResetMoveState();
            WarpPlayer1.CustomSnapTo(new(Player2Body.TruePosition.x, Player2Body.TruePosition.y + 0.3636f));

            if (IsSubmerged() && CustomPlayer.Local == WarpPlayer1)
            {
                ChangeFloor(WarpPlayer1.GetTruePosition().y > -7);
                CheckOutOfBoundsElevator(CustomPlayer.Local);
            }
        }
        else if (Player1Body && Player2Body)
        {
            StopDragging(Player1Body.ParentId);
            Player1Body.transform.position = Player2Body.TruePosition;
        }

        if (CustomPlayer.Local == WarpPlayer1)
        {
            if (ActiveTask())
                ActiveTask().Close();

            if (MapPatch.MapActive)
                Map().Close();
        }

        WarpPlayer1 = null;
        WarpPlayer2 = null;
        Warping = false;
        yield break;
    }

    public void Click1(PlayerControl player)
    {
        var cooldown = Interact(Player, player);

        if (cooldown != CooldownType.Fail)
            WarpPlayer1 = player;
        else
            WarpButton.StartCooldown(cooldown);
    }

    public void Click2(PlayerControl player)
    {
        var cooldown = Interact(Player, player);

        if (cooldown != CooldownType.Fail)
            WarpPlayer2 = player;
        else
            WarpButton.StartCooldown(cooldown);
    }

    public bool Exception1(PlayerControl player) => (player == Player && !WarpSelf) || UninteractiblePlayers.ContainsKey(player.PlayerId) || player == WarpPlayer2 || player.IsMoving() ||
        (!BodyById(player.PlayerId) && player.Data.IsDead);

    public bool Exception2(PlayerControl player) => (player == Player && !WarpSelf) || UninteractiblePlayers.ContainsKey(player.PlayerId) || player == WarpPlayer1 || player.IsMoving() ||
        (!BodyById(player.PlayerId) && player.Data.IsDead);

    public void AnimateWarp()
    {
        WarpObj.transform.position = new(WarpPlayer1.GetTruePosition().x, WarpPlayer1.GetTruePosition().y + 0.35f, (WarpPlayer1.GetTruePosition().y / 1000f) + 0.01f);
        AnimationPlaying.flipX = WarpPlayer1.MyRend().flipX;
        AnimationPlaying.transform.localScale *= 0.9f * WarpPlayer1.GetModifiedSize();

        Coroutines.Start(PerformTimedAction(WarpDur, p =>
        {
            var index = (int)(p * PortalAnimation.Count);
            index = Mathf.Clamp(index, 0, PortalAnimation.Count - 1);
            AnimationPlaying.sprite = PortalAnimation[index];
            WarpPlayer1.SetPlayerMaterialColors(AnimationPlaying);

            if (p == 1)
                AnimationPlaying.sprite = PortalAnimation[0];
        }));
    }

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

        if (CustomPlayer.Local.walkingToVent)
        {
            CustomPlayer.Local.inVent = false;
            CustomPlayer.Local.moveable = true;
            CustomPlayer.Local.MyPhysics.StopAllCoroutines();
        }
    }

    public void Warp()
    {
        if (HoldsDrive)
        {
            WarpAll();
            WarpButton.StartCooldown();
        }
        else if (!WarpPlayer1)
            WarpMenu1.Open();
        else if (!WarpPlayer2)
            WarpMenu2.Open();
        else
        {
            CallRpc(CustomRPC.Action, ActionsRPC.LayerAction, this, WarpPlayer1, WarpPlayer2);
            Coroutines.Start(WarpPlayers());
        }
    }

    public override void ReadRPC(MessageReader reader)
    {
        WarpPlayer1 = reader.ReadPlayer();
        WarpPlayer2 = reader.ReadPlayer();
        Coroutines.Start(WarpPlayers());
    }

    public string Label()
    {
        if (HoldsDrive)
            return "WARP";
        else if (!WarpPlayer1)
            return "FIRST TARGET";
        else if (!WarpPlayer2)
            return "SECOND TARGET";
        else
            return "WARP";
    }

    public override void UpdateHud(HudManager __instance)
    {
        base.UpdateHud(__instance);

        if (Input.GetKeyDown(KeyCode.Backspace))
        {
            if (!HoldsDrive && !Warping)
            {
                if (WarpPlayer2)
                    WarpPlayer2 = null;
                else if (WarpPlayer1)
                    WarpPlayer1 = null;
            }

            LogMessage("Removed a target");
        }
    }
}