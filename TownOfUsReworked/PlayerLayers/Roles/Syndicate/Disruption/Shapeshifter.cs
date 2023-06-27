namespace TownOfUsReworked.PlayerLayers.Roles
{
    public class Shapeshifter : Syndicate
    {
        public CustomButton ShapeshiftButton;
        public bool Enabled;
        public DateTime LastShapeshifted;
        public float TimeRemaining;
        public bool Shapeshifted => TimeRemaining > 0f;
        public PlayerControl ShapeshiftPlayer1;
        public PlayerControl ShapeshiftPlayer2;
        public CustomMenu ShapeshiftMenu1;
        public CustomMenu ShapeshiftMenu2;

        public Shapeshifter(PlayerControl player) : base(player)
        {
            Name = "Shapeshifter";
            StartText = () => "Change Everyone's Appearances";
            AbilitiesText = () => $"- You can shuffle everyone's appearances\n{CommonAbilities}";
            Color = CustomGameOptions.CustomSynColors ? Colors.Shapeshifter : Colors.Syndicate;
            RoleType = RoleEnum.Shapeshifter;
            RoleAlignment = RoleAlignment.SyndicateDisrup;
            ShapeshiftPlayer1 = null;
            ShapeshiftPlayer2 = null;
            ShapeshiftMenu1 = new(Player, Click1, Exception1);
            ShapeshiftMenu2 = new(Player, Click2, Exception2);
            Type = LayerEnum.Shapeshifter;
            ShapeshiftButton = new(this, "Shapeshift", AbilityTypes.Effect, "Secondary", HitShapeshift);
            InspectorResults = InspectorResults.CreatesConfusion;

            if (TownOfUsReworked.IsTest)
                Utils.LogSomething($"{Player.name} is {Name}");
        }

        public void Shapeshift()
        {
            TimeRemaining -= Time.deltaTime;
            Enabled = true;

            if (!SyndicateHasChaosDrive)
            {
                Utils.Morph(ShapeshiftPlayer1, ShapeshiftPlayer2);
                Utils.Morph(ShapeshiftPlayer2, ShapeshiftPlayer1);
            }
            else
                Utils.Shapeshift();

            if (Utils.Meeting)
                TimeRemaining = 0f;
        }

        public void UnShapeshift()
        {
            Enabled = false;
            LastShapeshifted = DateTime.UtcNow;

            if (SyndicateHasChaosDrive)
                Utils.DefaultOutfitAll();
            else
            {
                Utils.DefaultOutfit(ShapeshiftPlayer1);
                Utils.DefaultOutfit(ShapeshiftPlayer2);
            }

            ShapeshiftPlayer1 = null;
            ShapeshiftPlayer2 = null;
        }

        public float ShapeshiftTimer()
        {
            var timespan = DateTime.UtcNow - LastShapeshifted;
            var num = Player.GetModifiedCooldown(CustomGameOptions.ShapeshiftCooldown) * 1000f;
            var flag2 = num - (float)timespan.TotalMilliseconds < 0f;
            return flag2 ? 0f : (num - (float)timespan.TotalMilliseconds) / 1000f;
        }

        public void Click1(PlayerControl player)
        {
            var interact = Utils.Interact(Player, player);

            if (interact[3])
                ShapeshiftPlayer1 = player;
            else if (interact[0])
                LastShapeshifted = DateTime.UtcNow;
            else if (interact[1])
                LastShapeshifted.AddSeconds(CustomGameOptions.ProtectKCReset);
        }

        public void Click2(PlayerControl player)
        {
            var interact = Utils.Interact(Player, player);

            if (interact[3])
                ShapeshiftPlayer2 = player;
            else if (interact[0])
                LastShapeshifted = DateTime.UtcNow;
            else if (interact[1])
                LastShapeshifted.AddSeconds(CustomGameOptions.ProtectKCReset);
        }

        public void HitShapeshift()
        {
            if (ShapeshiftTimer() != 0f)
                return;

            if (HoldsDrive)
            {
                var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.Action, SendOption.Reliable);
                writer.Write((byte)ActionsRPC.Shapeshift);
                writer.Write(PlayerId);
                AmongUsClient.Instance.FinishRpcImmediately(writer);
                TimeRemaining = CustomGameOptions.ShapeshiftDuration;
                Shapeshift();
                Utils.Shapeshift();
            }
            else if (ShapeshiftPlayer1 == null)
                ShapeshiftMenu1.Open();
            else if (ShapeshiftPlayer2 == null)
                ShapeshiftMenu2.Open();
            else
            {
                var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.Action, SendOption.Reliable);
                writer.Write((byte)ActionsRPC.Shapeshift);
                writer.Write(PlayerId);
                writer.Write(ShapeshiftPlayer1.PlayerId);
                writer.Write(ShapeshiftPlayer2.PlayerId);
                AmongUsClient.Instance.FinishRpcImmediately(writer);
                TimeRemaining = CustomGameOptions.ShapeshiftDuration;
                Shapeshift();
            }
        }

        public bool Exception1(PlayerControl player) => player == Player || player == ShapeshiftPlayer2 || (player.Data.IsDead && Utils.BodyByPlayer(player) == null) || (player.Is(Faction)
            && !CustomGameOptions.ShapeshiftMates);

        public bool Exception2(PlayerControl player) => player == Player || player == ShapeshiftPlayer1 || (player.Data.IsDead && Utils.BodyByPlayer(player) == null) || (player.Is(Faction)
            && !CustomGameOptions.ShapeshiftMates);

        public override void UpdateHud(HudManager __instance)
        {
            base.UpdateHud(__instance);
            var flag1 = ShapeshiftPlayer1 == null && !HoldsDrive;
            var flag2 = ShapeshiftPlayer2 == null && !HoldsDrive;
            ShapeshiftButton.Update(flag1 ? "FIRST TARGET" : (flag2 ? "SECOND TARGET": "SHAPESHIFT"), ShapeshiftTimer(), CustomGameOptions.ShapeshiftCooldown, Shapeshifted, TimeRemaining,
                CustomGameOptions.ShapeshiftDuration);

            if (Input.GetKeyDown(KeyCode.Backspace))
            {
                if (!HoldsDrive && !Shapeshifted)
                {
                    if (ShapeshiftPlayer2 != null)
                        ShapeshiftPlayer2 = null;
                    else if (ShapeshiftPlayer1 != null)
                        ShapeshiftPlayer1 = null;
                }

                Utils.LogSomething("Removed a target");
            }
        }
    }
}