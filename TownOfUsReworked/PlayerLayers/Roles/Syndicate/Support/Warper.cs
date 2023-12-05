namespace TownOfUsReworked.PlayerLayers.Roles;

public class Warper : Syndicate
{
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

    public override Color Color => ClientGameOptions.CustomSynColors ? Colors.Warper : Colors.Syndicate;
    public override string Name => "Warper";
    public override LayerEnum Type => LayerEnum.Warper;
    public override Func<string> StartText => () => "Warp The <color=#8CFFFFFF>Crew</color> Away From Each Other";
    public override Func<string> Description => () => "- You can warp a" + (HoldsDrive ? "ll players, forcing them to be teleported to random locations" :
        " player to another player of your choice") + $"\n{CommonAbilities}";

    public Warper(PlayerControl player) : base(player)
    {
        WarpPlayer1 = null;
        WarpPlayer2 = null;
        WarpMenu1 = new(Player, Click1, Exception1);
        WarpMenu2 = new(Player, Click2, Exception2);
        WarpButton = new(this, "Warp", AbilityTypes.Targetless, "ActionSecondary", Warp, CustomGameOptions.WarpCd);
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
        player.Data.Role.IntroSound = GetAudio("WarperIntro");
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

            if (Player1Body == null)
                yield break;
        }

        if (WarpPlayer2.Data.IsDead)
        {
            Player2Body = BodyById(WarpPlayer2.PlayerId);

            if (Player2Body == null)
                yield break;
        }

        if (WarpPlayer1.inVent)
        {
            while (GetInTransition())
                yield return null;

            WarpPlayer1.MyPhysics.ExitAllVents();
        }

        if (WarpPlayer2.inVent)
        {
            while (GetInTransition())
                yield return null;

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
            Flash(Color, CustomGameOptions.WarpDur);

        if (Player1Body == null && !WasInVent)
            AnimateWarp();

        var startTime = DateTime.UtcNow;

        while (true)
        {
            var seconds = (DateTime.UtcNow - startTime).TotalSeconds;

            if (seconds < CustomGameOptions.WarpDur)
                yield return null;
            else
                break;

            if (Meeting)
            {
                AnimationPlaying.sprite = PortalAnimation[0];
                yield break;
            }
        }

        if (Player1Body == null && Player2Body == null)
        {
            WarpPlayer1.MyPhysics.ResetMoveState();
            WarpPlayer1.NetTransform.SnapTo(new(WarpPlayer2.GetTruePosition().x, WarpPlayer2.GetTruePosition().y + 0.3636f));

            if (IsSubmerged() && CustomPlayer.Local == WarpPlayer1)
            {
                ChangeFloor(WarpPlayer1.GetTruePosition().y > -7);
                CheckOutOfBoundsElevator(CustomPlayer.Local);
            }

            if (WarpPlayer1.CanVent() && Vent != null && WasInVent)
                WarpPlayer1.MyPhysics.RpcEnterVent(Vent.Id);
        }
        else if (Player1Body != null && Player2Body == null)
        {
            StopDragging(Player1Body.ParentId);
            Player1Body.transform.position = WarpPlayer2.GetTruePosition();

            if (IsSubmerged() && CustomPlayer.Local == WarpPlayer2)
            {
                ChangeFloor(WarpPlayer2.GetTruePosition().y > -7);
                CheckOutOfBoundsElevator(CustomPlayer.Local);
            }
        }
        else if (Player1Body == null && Player2Body != null)
        {
            WarpPlayer1.MyPhysics.ResetMoveState();
            WarpPlayer1.NetTransform.SnapTo(new(Player2Body.TruePosition.x, Player2Body.TruePosition.y + 0.3636f));

            if (IsSubmerged() && CustomPlayer.Local == WarpPlayer1)
            {
                ChangeFloor(WarpPlayer1.GetTruePosition().y > -7);
                CheckOutOfBoundsElevator(CustomPlayer.Local);
            }
        }
        else if (Player1Body != null && Player2Body != null)
        {
            StopDragging(Player1Body.ParentId);
            Player1Body.transform.position = Player2Body.TruePosition;
        }

        if (CustomPlayer.Local == WarpPlayer1)
        {
            if (ActiveTask)
                ActiveTask.Close();

            if (MapPatch.MapActive)
                Map.Close();
        }

        WarpPlayer1 = null;
        WarpPlayer2 = null;
        Warping = false;
        yield break;
    }

    public void Click1(PlayerControl player)
    {
        var interact = Interact(Player, player);

        if (interact.AbilityUsed)
            WarpPlayer1 = player;
        else if (interact.Reset)
            WarpButton.StartCooldown();
        else if (interact.Protected)
            WarpButton.StartCooldown(CooldownType.GuardianAngel);
    }

    public void Click2(PlayerControl player)
    {
        var interact = Interact(Player, player);

        if (interact.AbilityUsed)
            WarpPlayer2 = player;
        else if (interact.Reset)
            WarpButton.StartCooldown();
        else if (interact.Protected)
            WarpButton.StartCooldown(CooldownType.GuardianAngel);
    }

    public bool Exception1(PlayerControl player) => (player == Player && !CustomGameOptions.WarpSelf) || UninteractiblePlayers.ContainsKey(player.PlayerId) || player == WarpPlayer2 ||
        (BodyById(player.PlayerId) == null && player.Data.IsDead) || player.IsMoving();

    public bool Exception2(PlayerControl player) => (player == Player && !CustomGameOptions.WarpSelf) || UninteractiblePlayers.ContainsKey(player.PlayerId) || player == WarpPlayer1 ||
        (BodyById(player.PlayerId) == null && player.Data.IsDead) || player.IsMoving();

    public void AnimateWarp()
    {
        WarpObj.transform.position = new(WarpPlayer1.GetTruePosition().x, WarpPlayer1.GetTruePosition().y + 0.35f, (WarpPlayer1.GetTruePosition().y / 1000f) + 0.01f);
        AnimationPlaying.flipX = WarpPlayer1.MyRend().flipX;
        AnimationPlaying.transform.localScale *= 0.9f * WarpPlayer1.GetModifiedSize();

        HUD.StartCoroutine(Effects.Lerp(CustomGameOptions.WarpDur, new Action<float>(p =>
        {
            var index = (int)(p * PortalAnimation.Length);
            index = Mathf.Clamp(index, 0, PortalAnimation.Length - 1);
            AnimationPlaying.sprite = PortalAnimation[index];
            WarpPlayer1.SetPlayerMaterialColors(AnimationPlaying);

            if (p == 1)
                AnimationPlaying.sprite = PortalAnimation[0];
        })));
    }

    public static void WarpAll()
    {
        var coords = GenerateWarpCoordinates();

        foreach (var (id, pos) in coords)
        {
            var player = PlayerById(id);
            var body = BodyById(id);

            if (body != null)
            {
                body.transform.position = pos;
                CallRpc(CustomRPC.Misc, MiscRPC.MoveBody, body, pos);
            }
            else
                player.NetTransform.RpcSnapTo(pos);
        }
    }

    public void Warp()
    {
        if (HoldsDrive)
        {
            WarpAll();
            WarpButton.StartCooldown();
        }
        else if (WarpPlayer1 == null)
            WarpMenu1.Open();
        else if (WarpPlayer2 == null)
            WarpMenu2.Open();
        else
        {
            CallRpc(CustomRPC.Action, ActionsRPC.LayerAction2, this, WarpPlayer1, WarpPlayer2);
            Coroutines.Start(WarpPlayers());
        }
    }

    public override void ReadRPC(MessageReader reader)
    {
        WarpPlayer1 = reader.ReadPlayer();
        WarpPlayer2 = reader.ReadPlayer();
        Coroutines.Start(WarpPlayers());
    }

    public override void UpdateHud(HudManager __instance)
    {
        base.UpdateHud(__instance);
        WarpButton.Update2(WarpPlayer1 == null && !HoldsDrive ? "FIRST TARGET" : (WarpPlayer2 == null && !HoldsDrive ? "SECOND TARGET" : "WARP"));

        if (Input.GetKeyDown(KeyCode.Backspace))
        {
            if (!HoldsDrive && !Warping)
            {
                if (WarpPlayer2 != null)
                    WarpPlayer2 = null;
                else if (WarpPlayer1 != null)
                    WarpPlayer1 = null;
            }

            LogInfo("Removed a target");
        }
    }
}