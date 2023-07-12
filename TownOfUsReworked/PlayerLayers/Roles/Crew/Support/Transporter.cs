using static TownOfUsReworked.Languages.Language;
namespace TownOfUsReworked.PlayerLayers.Roles
{
    public class Transporter : Crew
    {
        public DateTime LastTransported;
        public PlayerControl TransportPlayer1;
        public PlayerControl TransportPlayer2;
        public int UsesLeft;
        public bool ButtonUsable => UsesLeft > 0;
        public Dictionary<byte, DateTime> UntransportablePlayers = new();
        public CustomButton TransportButton;
        public CustomMenu TransportMenu1;
        public CustomMenu TransportMenu2;
        public SpriteRenderer AnimationPlaying1;
        public SpriteRenderer AnimationPlaying2;
        public GameObject Transport1;
        public GameObject Transport2;
        public float TimeRemaining;
        public bool Transporting => TimeRemaining > 0;
        public DeadBody Player1Body;
        public DeadBody Player2Body;
        public bool WasInVent1;
        public bool WasInVent2;
        public Vent Vent1;
        public Vent Vent2;

        public Transporter(PlayerControl player) : base(player)
        {
            Name = GetString("Transporter");
            StartText = () => GetString("TransporterStartText");
            AbilitiesText = () => GetString("TransporterAbilitiesText");
            Color = CustomGameOptions.CustomCrewColors ? Colors.Transporter : Colors.Crew;
            RoleType = RoleEnum.Transporter;
            TransportPlayer1 = null;
            TransportPlayer2 = null;
            UsesLeft = CustomGameOptions.TransportMaxUses;
            RoleAlignment = RoleAlignment.CrewSupport;
            InspectorResults = InspectorResults.MovesAround;
            UntransportablePlayers = new();
            TransportMenu1 = new(Player, Click1, Exception1);
            TransportMenu2 = new(Player, Click2, Exception2);
            Type = LayerEnum.Transporter;
            TransportButton = new(this, "Transport", AbilityTypes.Effect, "ActionSecondary", Transport, true);
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
            Transport1.transform.position = new(Player.GetTruePosition().x, Player.GetTruePosition().y, (Player.GetTruePosition().y / 1000f) + 0.01f);
            AnimationPlaying1 = Transport1.AddComponent<SpriteRenderer>();
            AnimationPlaying2 = Transport2.AddComponent<SpriteRenderer>();
            AnimationPlaying1.sprite = AssetManager.PortalAnimation[0];
            AnimationPlaying2.sprite = AssetManager.PortalAnimation[0];
            AnimationPlaying1.material = HatManager.Instance.PlayerMaterial;
            AnimationPlaying2.material = HatManager.Instance.PlayerMaterial;
            Transport1.SetActive(true);
            Transport2.SetActive(true);

            if (TownOfUsReworked.IsTest)
                Utils.LogSomething($"{Player.name} is {Name}");
        }

        public float TransportTimer()
        {
            var timespan = DateTime.UtcNow - LastTransported;
            var num = Player.GetModifiedCooldown(CustomGameOptions.TransportCooldown) * 1000f;
            var flag2 = num - (float)timespan.TotalMilliseconds < 0f;
            return flag2 ? 0f : (num - (float)timespan.TotalMilliseconds) / 1000f;
        }

        public IEnumerator TransportPlayers()
        {
            Player1Body = null;
            Player2Body = null;
            WasInVent1 = false;
            WasInVent2 = false;
            Vent1 = null;
            Vent2 = null;
            TimeRemaining = CustomGameOptions.TransportDuration;

            if (TransportPlayer1.Data.IsDead)
            {
                Player1Body = Utils.BodyById(TransportPlayer1.PlayerId);

                if (Player1Body == null)
                    yield break;
            }

            if (TransportPlayer2.Data.IsDead)
            {
                Player2Body = Utils.BodyById(TransportPlayer2.PlayerId);

                if (Player2Body == null)
                    yield break;
            }

            if (TransportPlayer1.inVent)
            {
                while (ModCompatibility.GetInTransition())
                    yield return null;

                TransportPlayer1.MyPhysics.ExitAllVents();
                Vent1 = TransportPlayer1.GetClosestVent();
                WasInVent1 = true;
            }

            if (TransportPlayer2.inVent)
            {
                while (ModCompatibility.GetInTransition())
                    yield return null;

                TransportPlayer2.MyPhysics.ExitAllVents();
                Vent2 = TransportPlayer2.GetClosestVent();
                WasInVent2 = true;
            }

            TransportPlayer1.moveable = false;
            TransportPlayer2.moveable = false;
            TransportPlayer1.NetTransform.Halt();
            TransportPlayer2.NetTransform.Halt();

            if (CustomPlayer.Local == TransportPlayer1 || CustomPlayer.Local == TransportPlayer2)
                Utils.Flash(Color, CustomGameOptions.TransportDuration);

            if (Player1Body == null && !WasInVent1)
                AnimateTransport1();

            if (Player2Body == null && !WasInVent2)
                AnimateTransport2();

            var startTime = DateTime.UtcNow;

            while (true)
            {
                var now = DateTime.UtcNow;
                var seconds = (now - startTime).TotalSeconds;

                if (seconds < CustomGameOptions.TransportDuration)
                {
                    TimeRemaining -= Time.deltaTime;
                    yield return null;
                }
                else
                    break;

                if (Utils.Meeting)
                    yield break;
            }

            if (Player1Body == null && Player2Body == null)
            {
                TransportPlayer1.MyPhysics.ResetMoveState();
                TransportPlayer2.MyPhysics.ResetMoveState();
                var TempPosition = TransportPlayer1.GetTruePosition();
                TransportPlayer1.NetTransform.SnapTo(new(TransportPlayer2.GetTruePosition().x, TransportPlayer2.GetTruePosition().y + 0.3636f));
                TransportPlayer2.NetTransform.SnapTo(new(TempPosition.x, TempPosition.y + 0.3636f));

                if (ModCompatibility.IsSubmerged)
                {
                    if (CustomPlayer.Local == TransportPlayer1)
                    {
                        ModCompatibility.ChangeFloor(TransportPlayer1.GetTruePosition().y > -7);
                        ModCompatibility.CheckOutOfBoundsElevator(CustomPlayer.Local);
                    }

                    if (CustomPlayer.Local == TransportPlayer2)
                    {
                        ModCompatibility.ChangeFloor(TransportPlayer2.GetTruePosition().y > -7);
                        ModCompatibility.CheckOutOfBoundsElevator(CustomPlayer.Local);
                    }
                }

                if (TransportPlayer1.CanVent() && Vent2 != null && WasInVent2)
                    TransportPlayer1.MyPhysics.RpcEnterVent(Vent2.Id);

                if (TransportPlayer2.CanVent() && Vent1 != null && WasInVent1)
                    TransportPlayer2.MyPhysics.RpcEnterVent(Vent1.Id);
            }
            else if (Player1Body != null && Player2Body == null)
            {
                Utils.StopDragging(Player1Body.ParentId);
                TransportPlayer2.MyPhysics.ResetMoveState();
                var TempPosition = Player1Body.TruePosition;
                Player1Body.transform.position = TransportPlayer2.GetTruePosition();
                TransportPlayer2.NetTransform.SnapTo(new(TempPosition.x, TempPosition.y + 0.3636f));

                if (ModCompatibility.IsSubmerged && CustomPlayer.Local == TransportPlayer2)
                {
                    ModCompatibility.ChangeFloor(TransportPlayer2.GetTruePosition().y > -7);
                    ModCompatibility.CheckOutOfBoundsElevator(CustomPlayer.Local);
                }
            }
            else if (Player1Body == null && Player2Body != null)
            {
                Utils.StopDragging(Player2Body.ParentId);
                TransportPlayer1.MyPhysics.ResetMoveState();
                var TempPosition = TransportPlayer1.GetTruePosition();
                TransportPlayer1.NetTransform.SnapTo(new(Player2Body.TruePosition.x, Player2Body.TruePosition.y + 0.3636f));
                Player2Body.transform.position = TempPosition;

                if (ModCompatibility.IsSubmerged && CustomPlayer.Local == TransportPlayer1)
                {
                    ModCompatibility.ChangeFloor(TransportPlayer1.GetTruePosition().y > -7);
                    ModCompatibility.CheckOutOfBoundsElevator(CustomPlayer.Local);
                }
            }
            else if (Player1Body != null && Player2Body != null)
            {
                Utils.StopDragging(Player1Body.ParentId);
                Utils.StopDragging(Player2Body.ParentId);
                (Player1Body.transform.position, Player2Body.transform.position) = (Player2Body.TruePosition, Player1Body.TruePosition);
            }

            if (CustomPlayer.Local == TransportPlayer1 || CustomPlayer.Local == TransportPlayer2)
            {
                if (Minigame.Instance)
                    Minigame.Instance.Close();

                if (MapBehaviour.Instance)
                    MapBehaviour.Instance.Close();
            }

            TransportPlayer1.moveable = true;
            TransportPlayer2.moveable = true;
            TransportPlayer1.Collider.enabled = true;
            TransportPlayer2.Collider.enabled = true;
            TransportPlayer1.NetTransform.enabled = true;
            TransportPlayer2.NetTransform.enabled = true;
            TransportPlayer1 = null;
            TransportPlayer2 = null;
            TimeRemaining = 0; //Insurance
            LastTransported = DateTime.UtcNow;
        }

        public void Click1(PlayerControl player)
        {
            var interact = Utils.Interact(Player, player);

            if (interact[3])
                TransportPlayer1 = player;
            else if (interact[0])
                LastTransported = DateTime.UtcNow;
            else if (interact[1])
                LastTransported.AddSeconds(CustomGameOptions.ProtectKCReset);
        }

        public void Click2(PlayerControl player)
        {
            var interact = Utils.Interact(Player, player);

            if (interact[3])
                TransportPlayer2 = player;
            else if (interact[0])
                LastTransported = DateTime.UtcNow;
            else if (interact[1])
                LastTransported.AddSeconds(CustomGameOptions.ProtectKCReset);
        }

        public void AnimateTransport1()
        {
            Transport1.transform.position = new(TransportPlayer1.GetTruePosition().x, TransportPlayer1.GetTruePosition().y + 0.35f, (TransportPlayer1.GetTruePosition().y / 1000f) + 0.01f);
            AnimationPlaying1.flipX = TransportPlayer1.MyRend().flipX;
            AnimationPlaying1.transform.localScale *= 0.9f * TransportPlayer1.GetModifiedSize();

            Utils.HUD.StartCoroutine(Effects.Lerp(CustomGameOptions.TransportDuration, new Action<float>(p =>
            {
                var index = (int)(p * AssetManager.PortalAnimation.Length);
                index = Mathf.Clamp(index, 0, AssetManager.PortalAnimation.Length - 1);
                AnimationPlaying1.sprite = AssetManager.PortalAnimation[index];
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

            Utils.HUD.StartCoroutine(Effects.Lerp(CustomGameOptions.TransportDuration, new Action<float>(p =>
            {
                var index = (int)(p * AssetManager.PortalAnimation.Length);
                index = Mathf.Clamp(index, 0, AssetManager.PortalAnimation.Length - 1);
                AnimationPlaying2.sprite = AssetManager.PortalAnimation[index];
                TransportPlayer2.SetPlayerMaterialColors(AnimationPlaying2);

                if (p == 1)
                    AnimationPlaying2.sprite = null;
            })));
        }

        public bool Exception1(PlayerControl player) => (player == Player && !CustomGameOptions.TransSelf) || UntransportablePlayers.ContainsKey(player.PlayerId) ||
            (Utils.BodyById(player.PlayerId) == null && player.Data.IsDead) || player == TransportPlayer2 || player.IsMoving();

        public bool Exception2(PlayerControl player) => (player == Player && !CustomGameOptions.TransSelf) || UntransportablePlayers.ContainsKey(player.PlayerId) ||
            (Utils.BodyById(player.PlayerId) == null && player.Data.IsDead) || player == TransportPlayer1 || player.IsMoving();

        public void Transport()
        {
            if (TransportTimer() != 0f)
                return;

            if (TransportPlayer1 == null)
                TransportMenu1.Open();
            else if (TransportPlayer2 == null)
                TransportMenu2.Open();
            else
            {
                var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.Action, SendOption.Reliable);
                writer.Write((byte)ActionsRPC.Transport);
                writer.Write(PlayerId);
                writer.Write(TransportPlayer1.PlayerId);
                writer.Write(TransportPlayer2.PlayerId);
                AmongUsClient.Instance.FinishRpcImmediately(writer);
                Coroutines.Start(TransportPlayers());
                UsesLeft--;
            }
        }

        public override void UpdateHud(HudManager __instance)
        {
            base.UpdateHud(__instance);
            var flag1 = TransportPlayer1 == null;
            var flag2 = TransportPlayer2 == null;
            TransportButton.Update(flag1 ? "FIRST TARGET" : (flag2 ? "SECOND TARGET" : "TRANSPORT"), TransportTimer(), CustomGameOptions.TransportCooldown, UsesLeft, Transporting,
                TimeRemaining, CustomGameOptions.TransportDuration, true, ButtonUsable);

            if (Input.GetKeyDown(KeyCode.Backspace))
            {
                if (TransportPlayer2 != null)
                    TransportPlayer2 = null;
                else if (TransportPlayer1 != null)
                    TransportPlayer1 = null;

                Utils.LogSomething("Removed a target");
            }

            foreach (var entry in UntransportablePlayers)
            {
                var player = Utils.PlayerById(entry.Key);

                if (player == null)
                    continue;

                if (player.Data.IsDead || player.Data.Disconnected)
                    continue;

                if (UntransportablePlayers.ContainsKey(player.PlayerId) && player.moveable && UntransportablePlayers.GetValueSafe(player.PlayerId).AddSeconds(0.5) < DateTime.UtcNow)
                    UntransportablePlayers.Remove(player.PlayerId);
            }
        }
    }
}