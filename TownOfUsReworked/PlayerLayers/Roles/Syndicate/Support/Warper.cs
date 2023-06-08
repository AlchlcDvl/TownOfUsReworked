namespace TownOfUsReworked.PlayerLayers.Roles
{
    public class Warper : SyndicateRole
    {
        public CustomButton WarpButton;
        public DateTime LastWarped;
        public PlayerControl WarpPlayer1;
        public PlayerControl WarpPlayer2;
        public Dictionary<byte, DateTime> UnwarpablePlayers = new();
        public CustomMenu WarpMenu1;
        public CustomMenu WarpMenu2;
        public SpriteRenderer AnimationPlaying;
        public GameObject WarpObj;
        public float TimeRemaining;
        public bool Warping => TimeRemaining > 0;
        public DeadBody Player1Body;
        public DeadBody Player2Body;
        public bool WasInVent;
        public Vent Vent;

        public Warper(PlayerControl player) : base(player)
        {
            Name = "Warper";
            StartText = () => "Warp The <color=#8CFFFFFF>Crew</color> Away From Each Other";
            AbilitiesText = () => "- You can warp all players, forcing them to be teleported to random locations\n- With the Chaos Drive, more locations are opened to you\n" +
                $"{CommonAbilities}";
            Color = CustomGameOptions.CustomSynColors ? Colors.Warper : Colors.Syndicate;
            RoleType = RoleEnum.Warper;
            WarpPlayer1 = null;
            WarpPlayer2 = null;
            UnwarpablePlayers = new();
            WarpMenu1 = new(Player, Click1, Exception1);
            WarpMenu2 = new(Player, Click2, Exception2);

            Type = LayerEnum.Warper;
            WarpButton = new(this, "Warp", AbilityTypes.Effect, "Secondary", Warp);
            InspectorResults = InspectorResults.MovesAround;
            Player1Body = null;
            Player2Body = null;
            WasInVent = false;
            Vent = null;
            WarpObj = new("Warp") { layer = 5 };
            WarpObj.AddSubmergedComponent("ElevatorMover");
            WarpObj.transform.position = new(Player.GetTruePosition().x, Player.GetTruePosition().y, (Player.GetTruePosition().y / 1000f) + 0.01f);
            AnimationPlaying = WarpObj.AddComponent<SpriteRenderer>();
            AnimationPlaying.sprite = AssetManager.PortalAnimation[0];
            AnimationPlaying.material = HatManager.Instance.PlayerMaterial;
            WarpObj.SetActive(true);

            if (TownOfUsReworked.IsTest)
                Utils.LogSomething($"{Player.name} is {Name}");
        }

        public float WarpTimer()
        {
            var timespan = DateTime.UtcNow - LastWarped;
            var num = Player.GetModifiedCooldown(CustomGameOptions.WarpCooldown) * 1000f;
            var flag2 = num - (float)timespan.TotalMilliseconds < 0f;
            return flag2 ? 0f : (num - (float)timespan.TotalMilliseconds) / 1000f;
        }

        public IEnumerator WarpPlayers()
        {
            Player1Body = null;
            Player2Body = null;
            WasInVent = false;
            Vent = null;
            TimeRemaining = CustomGameOptions.WarpDuration;

            if (WarpPlayer1.Data.IsDead)
            {
                Player1Body = Utils.BodyById(WarpPlayer1.PlayerId);

                if (Player1Body == null)
                    yield break;
            }

            if (WarpPlayer2.Data.IsDead)
            {
                Player2Body = Utils.BodyById(WarpPlayer2.PlayerId);

                if (Player2Body == null)
                    yield break;
            }

            if (WarpPlayer1.inVent)
            {
                while (ModCompatibility.GetInTransition())
                    yield return null;

                WarpPlayer1.MyPhysics.ExitAllVents();
            }

            if (WarpPlayer2.inVent)
            {
                while (ModCompatibility.GetInTransition())
                    yield return null;

                Vent = WarpPlayer2.GetClosestVent();
                WasInVent = true;
            }

            WarpPlayer1.moveable = false;
            WarpPlayer1.NetTransform.Halt();

            if (PlayerControl.LocalPlayer == WarpPlayer1)
                Utils.Flash(Color, CustomGameOptions.WarpDuration);

            if (Player1Body == null && !WasInVent)
                AnimateWarp();

            var startTime = DateTime.UtcNow;

            while (true)
            {
                var now = DateTime.UtcNow;
                var seconds = (now - startTime).TotalSeconds;

                if (seconds < CustomGameOptions.WarpDuration)
                {
                    TimeRemaining -= Time.deltaTime;
                    yield return null;
                }
                else
                    break;

                if (MeetingHud.Instance)
                    yield break;
            }

            if (Player1Body == null && Player2Body == null)
            {
                WarpPlayer1.MyPhysics.ResetMoveState();
                WarpPlayer1.NetTransform.SnapTo(new(WarpPlayer2.GetTruePosition().x, WarpPlayer2.GetTruePosition().y + 0.3636f));

                if (ModCompatibility.IsSubmerged && PlayerControl.LocalPlayer == WarpPlayer1)
                {
                    ModCompatibility.ChangeFloor(WarpPlayer1.GetTruePosition().y > -7);
                    ModCompatibility.CheckOutOfBoundsElevator(PlayerControl.LocalPlayer);
                }

                if (WarpPlayer1.CanVent() && Vent != null && WasInVent)
                    WarpPlayer1.MyPhysics.RpcEnterVent(Vent.Id);
            }
            else if (Player1Body != null && Player2Body == null)
            {
                Utils.StopDragging(Player1Body.ParentId);
                Player1Body.transform.position = WarpPlayer2.GetTruePosition();

                if (ModCompatibility.IsSubmerged && PlayerControl.LocalPlayer == WarpPlayer2)
                {
                    ModCompatibility.ChangeFloor(WarpPlayer2.GetTruePosition().y > -7);
                    ModCompatibility.CheckOutOfBoundsElevator(PlayerControl.LocalPlayer);
                }
            }
            else if (Player1Body == null && Player2Body != null)
            {
                WarpPlayer1.MyPhysics.ResetMoveState();
                WarpPlayer1.NetTransform.SnapTo(new(Player2Body.TruePosition.x, Player2Body.TruePosition.y + 0.3636f));

                if (ModCompatibility.IsSubmerged && PlayerControl.LocalPlayer == WarpPlayer1)
                {
                    ModCompatibility.ChangeFloor(WarpPlayer1.GetTruePosition().y > -7);
                    ModCompatibility.CheckOutOfBoundsElevator(PlayerControl.LocalPlayer);
                }
            }
            else if (Player1Body != null && Player2Body != null)
            {
                Utils.StopDragging(Player1Body.ParentId);
                Player1Body.transform.position = Player2Body.TruePosition;
            }

            if (PlayerControl.LocalPlayer == WarpPlayer1)
            {
                if (Minigame.Instance)
                    Minigame.Instance.Close();

                if (MapBehaviour.Instance)
                    MapBehaviour.Instance.Close();
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
            var interact = Utils.Interact(Player, player);

            if (interact[3])
                WarpPlayer1 = player;
            else if (interact[0])
                LastWarped = DateTime.UtcNow;
            else if (interact[1])
                LastWarped.AddSeconds(CustomGameOptions.ProtectKCReset);
        }

        public void Click2(PlayerControl player)
        {
            var interact = Utils.Interact(Player, player);

            if (interact[3])
                WarpPlayer2 = player;
            else if (interact[0])
                LastWarped = DateTime.UtcNow;
            else if (interact[1])
                LastWarped.AddSeconds(CustomGameOptions.ProtectKCReset);
        }

        public bool Exception1(PlayerControl player) => (player == Player && !CustomGameOptions.WarpSelf) || UnwarpablePlayers.ContainsKey(player.PlayerId) || player == WarpPlayer2 ||
            (Utils.BodyById(player.PlayerId) == null && player.Data.IsDead) || player.IsMoving();

        public bool Exception2(PlayerControl player) => (player == Player && !CustomGameOptions.WarpSelf) || UnwarpablePlayers.ContainsKey(player.PlayerId) || player == WarpPlayer1 ||
            (Utils.BodyById(player.PlayerId) == null && player.Data.IsDead) || player.IsMoving();

        public void AnimateWarp()
        {
            WarpObj.transform.position = new(WarpPlayer1.GetTruePosition().x, WarpPlayer1.GetTruePosition().y + 0.35f, (WarpPlayer1.GetTruePosition().y / 1000f) + 0.01f);
            AnimationPlaying.flipX = WarpPlayer1.MyRend().flipX;
            AnimationPlaying.transform.localScale *= 0.9f * WarpPlayer1.GetModifiedSize();

            HudManager.Instance.StartCoroutine(Effects.Lerp(CustomGameOptions.WarpDuration, new Action<float>(p =>
            {
                var index = (int)(p * AssetManager.PortalAnimation.Length);
                index = Mathf.Clamp(index, 0, AssetManager.PortalAnimation.Length - 1);
                AnimationPlaying.sprite = AssetManager.PortalAnimation[index];
                WarpPlayer1.SetPlayerMaterialColors(AnimationPlaying);

                if (p == 1)
                    AnimationPlaying.sprite = null;
            })));
        }

        public void Warp()
        {
            if (WarpTimer() != 0f)
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
                var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.Action, SendOption.Reliable);
                writer.Write((byte)ActionsRPC.Warp);
                writer.Write(PlayerId);
                writer.Write(WarpPlayer1.PlayerId);
                writer.Write(WarpPlayer2.PlayerId);
                AmongUsClient.Instance.FinishRpcImmediately(writer);
                Coroutines.Start(WarpPlayers());
            }
        }

        public override void UpdateHud(HudManager __instance)
        {
            base.UpdateHud(__instance);
            var flag1 = WarpPlayer1 == null && !HoldsDrive;
            var flag2 = WarpPlayer2 == null && !HoldsDrive;
            WarpButton.Update(flag1 ? "FIRST TARGET" : (flag2 ? "SECOND TARGET" : "WARP"), WarpTimer(), CustomGameOptions.WarpCooldown, Warping, TimeRemaining,
                CustomGameOptions.WarpDuration);

            if (Input.GetKeyDown(KeyCode.Backspace))
            {
                if (!HoldsDrive)
                {
                    if (WarpPlayer2 != null)
                        WarpPlayer2 = null;
                    else if (WarpPlayer1 != null)
                        WarpPlayer1 = null;
                }

                Utils.LogSomething("Removed a target");
            }

            foreach (var entry in UnwarpablePlayers)
            {
                var player = Utils.PlayerById(entry.Key);

                if (player?.Data.IsDead == true || player.Data.Disconnected)
                    continue;

                if (UnwarpablePlayers.ContainsKey(player.PlayerId) && player.moveable && UnwarpablePlayers.GetValueSafe(player.PlayerId).AddSeconds(0.5) < DateTime.UtcNow)
                    UnwarpablePlayers.Remove(player.PlayerId);
            }
        }
    }
}