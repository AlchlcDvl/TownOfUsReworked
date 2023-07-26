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

        public override Color32 Color => ClientGameOptions.CustomSynColors ? Colors.Shapeshifter : Colors.Syndicate;
        public override string Name => "Shapeshifter";
        public override LayerEnum Type => LayerEnum.Shapeshifter;
        public override RoleEnum RoleType => RoleEnum.Shapeshifter;
        public override Func<string> StartText => () => "Change Everyone's Appearances";
        public override Func<string> AbilitiesText => () => $"- You can {(HoldsDrive ? "shuffle everyone's appearances" : "swap the appearances of 2 players")}\n{CommonAbilities}";
        public override InspectorResults InspectorResults => InspectorResults.CreatesConfusion;

        public Shapeshifter(PlayerControl player) : base(player)
        {
            RoleAlignment = RoleAlignment.SyndicateDisrup;
            ShapeshiftPlayer1 = null;
            ShapeshiftPlayer2 = null;
            ShapeshiftMenu1 = new(Player, Click1, Exception1);
            ShapeshiftMenu2 = new(Player, Click2, Exception2);
            ShapeshiftButton = new(this, "Shapeshift", AbilityTypes.Effect, "Secondary", HitShapeshift);
        }

        public void Shapeshift()
        {
            TimeRemaining -= Time.deltaTime;
            Enabled = true;

            if (!SyndicateHasChaosDrive)
            {
                Morph(ShapeshiftPlayer1, ShapeshiftPlayer2);
                Morph(ShapeshiftPlayer2, ShapeshiftPlayer1);
            }
            else
                Utils.Shapeshift();

            if (Meeting)
                TimeRemaining = 0f;
        }

        public void UnShapeshift()
        {
            Enabled = false;
            LastShapeshifted = DateTime.UtcNow;

            if (SyndicateHasChaosDrive)
                DefaultOutfitAll();
            else
            {
                DefaultOutfit(ShapeshiftPlayer1);
                DefaultOutfit(ShapeshiftPlayer2);
            }

            ShapeshiftPlayer1 = null;
            ShapeshiftPlayer2 = null;
        }

        public float ShapeshiftTimer()
        {
            var timespan = DateTime.UtcNow - LastShapeshifted;
            var num = Player.GetModifiedCooldown(CustomGameOptions.ShapeshiftCooldown) * 1000f;
            var time = num - (float)timespan.TotalMilliseconds;
            var flag2 = time < 0f;
            return (flag2 ? 0f : time) / 1000f;
        }

        public void Click1(PlayerControl player)
        {
            var interact = Interact(Player, player);

            if (interact[3])
                ShapeshiftPlayer1 = player;
            else if (interact[0])
                LastShapeshifted = DateTime.UtcNow;
            else if (interact[1])
                LastShapeshifted.AddSeconds(CustomGameOptions.ProtectKCReset);
        }

        public void Click2(PlayerControl player)
        {
            var interact = Interact(Player, player);

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
                TimeRemaining = CustomGameOptions.ShapeshiftDuration;
                Shapeshift();
                CallRpc(CustomRPC.Action, ActionsRPC.Shapeshift, this);
            }
            else if (ShapeshiftPlayer1 == null)
                ShapeshiftMenu1.Open();
            else if (ShapeshiftPlayer2 == null)
                ShapeshiftMenu2.Open();
            else
            {
                CallRpc(CustomRPC.Action, ActionsRPC.Shapeshift, this, ShapeshiftPlayer1, ShapeshiftPlayer2);
                TimeRemaining = CustomGameOptions.ShapeshiftDuration;
                Shapeshift();
            }
        }

        public bool Exception1(PlayerControl player) => player == Player || player == ShapeshiftPlayer2 || (player.Data.IsDead && BodyByPlayer(player) == null) || (player.Is(Faction)
            && !CustomGameOptions.ShapeshiftMates);

        public bool Exception2(PlayerControl player) => player == Player || player == ShapeshiftPlayer1 || (player.Data.IsDead && BodyByPlayer(player) == null) || (player.Is(Faction)
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

                LogSomething("Removed a target");
            }
        }
    }
}