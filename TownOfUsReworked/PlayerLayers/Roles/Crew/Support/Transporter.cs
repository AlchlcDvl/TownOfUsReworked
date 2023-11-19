namespace TownOfUsReworked.PlayerLayers.Roles;

public class Transporter : Crew
{
    public PlayerControl TransportPlayer1 { get; set; }
    public PlayerControl TransportPlayer2 { get; set; }
    public CustomButton TransportButton { get; set; }
    public CustomMenu TransportMenu1 { get; set; }
    public CustomMenu TransportMenu2 { get; set; }
    public SpriteRenderer AnimationPlaying1 { get; set; }
    public SpriteRenderer AnimationPlaying2 { get; set; }
    public GameObject Transport1 { get; set; }
    public GameObject Transport2 { get; set; }
    public bool Transporting { get; set; }
    public DeadBody Player1Body { get; set; }
    public DeadBody Player2Body { get; set; }
    public bool WasInVent1 { get; set; }
    public bool WasInVent2 { get; set; }
    public Vent Vent1 { get; set; }
    public Vent Vent2 { get; set; }

    public override Color Color => ClientGameOptions.CustomCrewColors ? Colors.Transporter : Colors.Crew;
    public override string Name => "Transporter";
    public override LayerEnum Type => LayerEnum.Transporter;
    public override Func<string> StartText => () => "Swap Locations Of Players For Maximum Confusion";
    public override Func<string> Description => () => "- You can swap the locations of 2 players of your choice";

    public Transporter(PlayerControl player) : base(player)
    {
        TransportPlayer1 = null;
        TransportPlayer2 = null;
        Alignment = Alignment.CrewSupport;
        TransportMenu1 = new(Player, Click1, Exception1);
        TransportMenu2 = new(Player, Click2, Exception2);
        TransportButton = new(this, "Transport", AbilityTypes.Targetless, "ActionSecondary", Transport, CustomGameOptions.TransportCd, CustomGameOptions.MaxTransports);
        Player1Body = null;
        Player2Body = null;
        WasInVent1 = false;
        WasInVent2 = false;
        Vent1 = null;
        Vent2 = null;
        Transport1 = new("Transport1") { layer = 5 };
        Transport2 = new("Transport2") { layer = 5 };
        Transport1.AddSubmergedComponent("ElevatorMover");
        Transport2.AddSubmergedComponent("ElevatorMover");
        Transport1.transform.position = new(Player.GetTruePosition().x, Player.GetTruePosition().y, (Player.GetTruePosition().y / 1000f) + 0.01f);
        Transport2.transform.position = new(Player.GetTruePosition().x, Player.GetTruePosition().y, (Player.GetTruePosition().y / 1000f) + 0.01f);
        AnimationPlaying1 = Transport1.AddComponent<SpriteRenderer>();
        AnimationPlaying2 = Transport2.AddComponent<SpriteRenderer>();
        AnimationPlaying1.sprite = AnimationPlaying2.sprite = PortalAnimation[0];
        AnimationPlaying1.material = AnimationPlaying2.material = HatManager.Instance.PlayerMaterial;
        Transport1.SetActive(true);
        Transport2.SetActive(true);
    }

    public IEnumerator TransportPlayers()
    {
        Player1Body = null;
        Player2Body = null;
        WasInVent1 = false;
        WasInVent2 = false;
        Vent1 = null;
        Vent2 = null;

        if (TransportPlayer1.Data.IsDead)
        {
            Player1Body = BodyById(TransportPlayer1.PlayerId);

            if (Player1Body == null)
                yield break;
        }

        if (TransportPlayer2.Data.IsDead)
        {
            Player2Body = BodyById(TransportPlayer2.PlayerId);

            if (Player2Body == null)
                yield break;
        }

        if (TransportPlayer1.inVent)
        {
            while (GetInTransition())
                yield return null;

            TransportPlayer1.MyPhysics.ExitAllVents();
            Vent1 = TransportPlayer1.GetClosestVent();
            WasInVent1 = true;
        }

        if (TransportPlayer2.inVent)
        {
            while (GetInTransition())
                yield return null;

            TransportPlayer2.MyPhysics.ExitAllVents();
            Vent2 = TransportPlayer2.GetClosestVent();
            WasInVent2 = true;
        }

        Transporting = true;
        TransportPlayer1.moveable = false;
        TransportPlayer2.moveable = false;
        TransportPlayer1.NetTransform.Halt();
        TransportPlayer2.NetTransform.Halt();

        if (CustomPlayer.Local == TransportPlayer1 || CustomPlayer.Local == TransportPlayer2)
            Flash(Color, CustomGameOptions.TransportDur);

        if (Player1Body == null && !WasInVent1)
            AnimateTransport1();

        if (Player2Body == null && !WasInVent2)
            AnimateTransport2();

        var startTime = DateTime.UtcNow;

        while (true)
        {
            var seconds = (DateTime.UtcNow - startTime).TotalSeconds;

            if (seconds < CustomGameOptions.TransportDur)
                yield return null;
            else
                break;

            if (Meeting)
            {
                AnimationPlaying1.sprite = AnimationPlaying2.sprite = PortalAnimation[0];
                yield break;
            }
        }

        if (Player1Body == null && Player2Body == null)
        {
            TransportPlayer1.MyPhysics.ResetMoveState();
            TransportPlayer2.MyPhysics.ResetMoveState();
            var TempPosition = TransportPlayer1.GetTruePosition();
            TransportPlayer1.NetTransform.SnapTo(new(TransportPlayer2.GetTruePosition().x, TransportPlayer2.GetTruePosition().y + 0.3636f));
            TransportPlayer2.NetTransform.SnapTo(new(TempPosition.x, TempPosition.y + 0.3636f));

            if (IsSubmerged())
            {
                if (CustomPlayer.Local == TransportPlayer1)
                {
                    ChangeFloor(TransportPlayer1.GetTruePosition().y > -7);
                    CheckOutOfBoundsElevator(CustomPlayer.Local);
                }

                if (CustomPlayer.Local == TransportPlayer2)
                {
                    ChangeFloor(TransportPlayer2.GetTruePosition().y > -7);
                    CheckOutOfBoundsElevator(CustomPlayer.Local);
                }
            }

            if (TransportPlayer1.CanVent() && Vent2 != null && WasInVent2)
                TransportPlayer1.MyPhysics.RpcEnterVent(Vent2.Id);

            if (TransportPlayer2.CanVent() && Vent1 != null && WasInVent1)
                TransportPlayer2.MyPhysics.RpcEnterVent(Vent1.Id);
        }
        else if (Player1Body != null && Player2Body == null)
        {
            StopDragging(Player1Body.ParentId);
            TransportPlayer2.MyPhysics.ResetMoveState();
            var TempPosition = Player1Body.TruePosition;
            Player1Body.transform.position = TransportPlayer2.GetTruePosition();
            TransportPlayer2.NetTransform.SnapTo(new(TempPosition.x, TempPosition.y + 0.3636f));

            if (IsSubmerged() && CustomPlayer.Local == TransportPlayer2)
            {
                ChangeFloor(TransportPlayer2.GetTruePosition().y > -7);
                CheckOutOfBoundsElevator(CustomPlayer.Local);
            }
        }
        else if (Player1Body == null && Player2Body != null)
        {
            StopDragging(Player2Body.ParentId);
            TransportPlayer1.MyPhysics.ResetMoveState();
            var TempPosition = TransportPlayer1.GetTruePosition();
            TransportPlayer1.NetTransform.SnapTo(new(Player2Body.TruePosition.x, Player2Body.TruePosition.y + 0.3636f));
            Player2Body.transform.position = TempPosition;

            if (IsSubmerged() && CustomPlayer.Local == TransportPlayer1)
            {
                ChangeFloor(TransportPlayer1.GetTruePosition().y > -7);
                CheckOutOfBoundsElevator(CustomPlayer.Local);
            }
        }
        else if (Player1Body != null && Player2Body != null)
        {
            StopDragging(Player1Body.ParentId);
            StopDragging(Player2Body.ParentId);
            (Player1Body.transform.position, Player2Body.transform.position) = (Player2Body.TruePosition, Player1Body.TruePosition);
        }

        if (CustomPlayer.Local == TransportPlayer1 || CustomPlayer.Local == TransportPlayer2)
        {
            if (ActiveTask)
                ActiveTask.Close();

            if (Map)
                Map.Close();
        }

        TransportPlayer1.moveable = true;
        TransportPlayer2.moveable = true;
        TransportPlayer1.Collider.enabled = true;
        TransportPlayer2.Collider.enabled = true;
        TransportPlayer1.NetTransform.enabled = true;
        TransportPlayer2.NetTransform.enabled = true;
        TransportPlayer1 = null;
        TransportPlayer2 = null;
        Transporting = false;
        yield break;
    }

    public void Click1(PlayerControl player)
    {
        var interact = Interact(Player, player);

        if (interact.AbilityUsed)
            TransportPlayer1 = player;
        else if (interact.Reset)
            TransportButton.StartCooldown();
        else if (interact.Protected)
            TransportButton.StartCooldown(CooldownType.GuardianAngel);
    }

    public void Click2(PlayerControl player)
    {
        var interact = Interact(Player, player);

        if (interact.AbilityUsed)
            TransportPlayer2 = player;
        else if (interact.Reset)
            TransportButton.StartCooldown();
        else if (interact.Protected)
            TransportButton.StartCooldown(CooldownType.GuardianAngel);
    }

    public void AnimateTransport1()
    {
        Transport1.transform.position = new(TransportPlayer1.GetTruePosition().x, TransportPlayer1.GetTruePosition().y + 0.35f, (TransportPlayer1.GetTruePosition().y / 1000f) + 0.01f);
        AnimationPlaying1.flipX = TransportPlayer1.MyRend().flipX;
        AnimationPlaying1.transform.localScale *= 0.9f * TransportPlayer1.GetModifiedSize();

        HUD.StartCoroutine(Effects.Lerp(CustomGameOptions.TransportDur, new Action<float>(p =>
        {
            var index = (int)(p * PortalAnimation.Length);
            index = Mathf.Clamp(index, 0, PortalAnimation.Length - 1);
            AnimationPlaying1.sprite = PortalAnimation[index];
            TransportPlayer1.SetPlayerMaterialColors(AnimationPlaying1);

            if (p == 1)
                AnimationPlaying1.sprite = null;
        })));
    }

    public void AnimateTransport2()
    {
        Transport2.transform.position = new(TransportPlayer2.GetTruePosition().x, TransportPlayer2.GetTruePosition().y + 0.35f, (TransportPlayer2.GetTruePosition().y / 1000f) + 0.01f);
        AnimationPlaying2.flipX = TransportPlayer2.MyRend().flipX;
        AnimationPlaying2.transform.localScale *= 0.9f * TransportPlayer2.GetModifiedSize();

        HUD.StartCoroutine(Effects.Lerp(CustomGameOptions.TransportDur, new Action<float>(p =>
        {
            var index = (int)(p * PortalAnimation.Length);
            index = Mathf.Clamp(index, 0, PortalAnimation.Length - 1);
            AnimationPlaying2.sprite = PortalAnimation[index];
            TransportPlayer2.SetPlayerMaterialColors(AnimationPlaying2);

            if (p == 1)
                AnimationPlaying2.sprite = null;
        })));
    }

    public bool Exception1(PlayerControl player) => (player == Player && !CustomGameOptions.TransSelf) || UninteractiblePlayers.ContainsKey(player.PlayerId) || (BodyById(player.PlayerId) ==
        null && player.Data.IsDead) || player == TransportPlayer2 || player.IsMoving();

    public bool Exception2(PlayerControl player) => (player == Player && !CustomGameOptions.TransSelf) || UninteractiblePlayers.ContainsKey(player.PlayerId) || (BodyById(player.PlayerId) ==
        null && player.Data.IsDead) || player == TransportPlayer1 || player.IsMoving();

    public void Transport()
    {
        if (TransportPlayer1 == null)
            TransportMenu1.Open();
        else if (TransportPlayer2 == null)
            TransportMenu2.Open();
        else
        {
            CallRpc(CustomRPC.Action, ActionsRPC.LayerAction2, this, TransportPlayer1, TransportPlayer2);
            Coroutines.Start(TransportPlayers());
            TransportButton.StartCooldown();
        }
    }

    public override void ReadRPC(MessageReader reader)
    {
        TransportPlayer1 = reader.ReadPlayer();
        TransportPlayer2 = reader.ReadPlayer();
        Coroutines.Start(TransportPlayers());
    }

    public override void UpdateHud(HudManager __instance)
    {
        base.UpdateHud(__instance);
        TransportButton.Update2(TransportPlayer1 == null ? "FIRST TARGET" : (TransportPlayer2 == null ? "SECOND TARGET" : "TRANSPORT"));

        if (Input.GetKeyDown(KeyCode.Backspace))
        {
            if (!Transporting)
            {
                if (TransportPlayer2 != null)
                    TransportPlayer2 = null;
                else if (TransportPlayer1 != null)
                    TransportPlayer1 = null;
            }

            LogInfo("Removed a target");
        }
    }
}