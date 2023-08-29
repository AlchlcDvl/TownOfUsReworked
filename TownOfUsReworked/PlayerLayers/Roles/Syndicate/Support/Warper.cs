namespace TownOfUsReworked.PlayerLayers.Roles;

public class Warper : Syndicate
{
    public CustomButton WarpButton { get; set; }
    public DateTime LastWarped { get; set; }
    public PlayerControl WarpPlayer1 { get; set; }
    public PlayerControl WarpPlayer2 { get; set; }
    public Dictionary<byte, DateTime> UnwarpablePlayers { get; set; }
    public CustomMenu WarpMenu1 { get; set; }
    public CustomMenu WarpMenu2 { get; set; }
    public SpriteRenderer AnimationPlaying { get; set; }
    public GameObject WarpObj { get; set; }
    public float TimeRemaining { get; set; }
    public bool Warping => TimeRemaining > 0;
    public DeadBody Player1Body { get; set; }
    public DeadBody Player2Body { get; set; }
    public bool WasInVent { get; set; }
    public Vent Vent { get; set; }

    public override Color32 Color => ClientGameOptions.CustomSynColors ? Colors.Warper : Colors.Syndicate;
    public override string Name => "Warper";
    public override LayerEnum Type => LayerEnum.Warper;
    public override Func<string> StartText => () => "Warp The <color=#8CFFFFFF>Crew</color> Away From Each Other";
    public override Func<string> Description => () => "- You can warp " +
        $"{(HoldsDrive ? "all players, forcing them to be teleported to random locations" : "a player to another player of your choice")}\n{CommonAbilities}";
    public override InspectorResults InspectorResults => InspectorResults.MovesAround;
    public float Timer => ButtonUtils.Timer(Player, LastWarped, CustomGameOptions.WarpCd);

    public Warper(PlayerControl player) : base(player)
    {
        WarpPlayer1 = null;
        WarpPlayer2 = null;
        UnwarpablePlayers = new();
        WarpMenu1 = new(Player, Click1, Exception1);
        WarpMenu2 = new(Player, Click2, Exception2);
        WarpButton = new(this, "Warp", AbilityTypes.Effect, "Secondary", Warp);
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
    }

    public IEnumerator WarpPlayers()
    {
        Player1Body = null;
        Player2Body = null;
        WasInVent = false;
        Vent = null;
        TimeRemaining = CustomGameOptions.WarpDur;

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

        WarpPlayer1.moveable = false;
        WarpPlayer1.NetTransform.Halt();

        if (CustomPlayer.Local == WarpPlayer1)
            Flash(Color, CustomGameOptions.WarpDur);

        if (Player1Body == null && !WasInVent)
            AnimateWarp();

        var startTime = DateTime.UtcNow;

        while (true)
        {
            var now = DateTime.UtcNow;
            var seconds = (now - startTime).TotalSeconds;

            if (seconds < CustomGameOptions.WarpDur)
            {
                TimeRemaining -= Time.deltaTime;
                yield return null;
            }
            else
                break;

            if (Meeting)
            {
                TimeRemaining = 0;
                yield break;
            }
        }

        if (Player1Body == null && Player2Body == null)
        {
            WarpPlayer1.MyPhysics.ResetMoveState();
            WarpPlayer1.NetTransform.SnapTo(new(WarpPlayer2.GetTruePosition().x, WarpPlayer2.GetTruePosition().y + 0.3636f));

            if (IsSubmerged && CustomPlayer.Local == WarpPlayer1)
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

            if (IsSubmerged && CustomPlayer.Local == WarpPlayer2)
            {
                ChangeFloor(WarpPlayer2.GetTruePosition().y > -7);
                CheckOutOfBoundsElevator(CustomPlayer.Local);
            }
        }
        else if (Player1Body == null && Player2Body != null)
        {
            WarpPlayer1.MyPhysics.ResetMoveState();
            WarpPlayer1.NetTransform.SnapTo(new(Player2Body.TruePosition.x, Player2Body.TruePosition.y + 0.3636f));

            if (IsSubmerged && CustomPlayer.Local == WarpPlayer1)
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
            if (Minigame.Instance)
                Minigame.Instance.Close();

            if (Map)
                Map.Close();
        }

        WarpPlayer1.moveable = true;
        WarpPlayer1.Collider.enabled = true;
        WarpPlayer1.NetTransform.enabled = true;
        WarpPlayer2.MyPhysics.ResetMoveState();
        WarpPlayer1 = null;
        WarpPlayer2 = null;
        TimeRemaining = 0; //Insurance
        LastWarped = DateTime.UtcNow;
    }

    public void Click1(PlayerControl player)
    {
        var interact = Interact(Player, player);

        if (interact[3])
            WarpPlayer1 = player;
        else if (interact[0])
            LastWarped = DateTime.UtcNow;
        else if (interact[1])
            LastWarped.AddSeconds(CustomGameOptions.ProtectKCReset);
    }

    public void Click2(PlayerControl player)
    {
        var interact = Interact(Player, player);

        if (interact[3])
            WarpPlayer2 = player;
        else if (interact[0])
            LastWarped = DateTime.UtcNow;
        else if (interact[1])
            LastWarped.AddSeconds(CustomGameOptions.ProtectKCReset);
    }

    public bool Exception1(PlayerControl player) => (player == Player && !CustomGameOptions.WarpSelf) || UnwarpablePlayers.ContainsKey(player.PlayerId) || player == WarpPlayer2 ||
        (BodyById(player.PlayerId) == null && player.Data.IsDead) || player.IsMoving();

    public bool Exception2(PlayerControl player) => (player == Player && !CustomGameOptions.WarpSelf) || UnwarpablePlayers.ContainsKey(player.PlayerId) || player == WarpPlayer1 ||
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
                AnimationPlaying.sprite = null;
        })));
    }

    public void Warp()
    {
        if (Timer != 0f)
            return;

        if (HoldsDrive)
        {
            Utils.Warp();
            LastWarped = DateTime.UtcNow;
        }
        else if (WarpPlayer1 == null)
            WarpMenu1.Open();
        else if (WarpPlayer2 == null)
            WarpMenu2.Open();
        else
        {
            CallRpc(CustomRPC.Action, ActionsRPC.Warp, this, WarpPlayer1, WarpPlayer2);
            Coroutines.Start(WarpPlayers());
        }
    }

    public override void UpdateHud(HudManager __instance)
    {
        base.UpdateHud(__instance);
        var flag1 = WarpPlayer1 == null && !HoldsDrive;
        var flag2 = WarpPlayer2 == null && !HoldsDrive;
        WarpButton.Update(flag1 ? "FIRST TARGET" : (flag2 ? "SECOND TARGET" : "WARP"), Timer, CustomGameOptions.WarpCd, Warping, TimeRemaining,
            CustomGameOptions.WarpDur);

        if (Input.GetKeyDown(KeyCode.Backspace))
        {
            if (!HoldsDrive)
            {
                if (WarpPlayer2 != null)
                    WarpPlayer2 = null;
                else if (WarpPlayer1 != null)
                    WarpPlayer1 = null;
            }

            LogInfo("Removed a target");
        }

        foreach (var entry in UnwarpablePlayers)
        {
            var player = PlayerById(entry.Key);

            if (player?.Data.IsDead == true || player.Data.Disconnected)
                continue;

            if (UnwarpablePlayers.ContainsKey(player.PlayerId) && player.moveable && UnwarpablePlayers.GetValueSafe(player.PlayerId).AddSeconds(0.5) < DateTime.UtcNow)
                UnwarpablePlayers.Remove(player.PlayerId);
        }
    }
}