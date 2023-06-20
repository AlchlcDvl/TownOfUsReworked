namespace TownOfUsReworked.PlayerLayers.Roles
{
    public class Ghoul : Intruder
    {
        public CustomButton MarkButton;
        public bool Caught;
        public bool Faded;
        public DateTime LastMarked;
        public PlayerControl MarkedPlayer;

        public Ghoul(PlayerControl player) : base(player)
        {
            Name = "Ghoul";
            RoleType = RoleEnum.Ghoul;
            StartText = () => "BOO!";
            AbilitiesText = () => "- You can mark a player for death every round\n- Marked players will be announced to all players and will die at the end of the next meeting if you are "
                + "not clicked";
            RoleAlignment = RoleAlignment.IntruderUtil;
            InspectorResults = InspectorResults.Ghostly;
            Color = CustomGameOptions.CustomIntColors ? Colors.Ghoul : Colors.Intruder;
            MarkedPlayer = null;

            if (TownOfUsReworked.IsTest)
                Utils.LogSomething($"{Player.name} is {Name}");

            Type = LayerEnum.Ghoul;
            MarkButton = new(this, "GhoulMark", AbilityTypes.Direct, "ActionSecondary", Mark, Exception1, false, true);
        }

        public float MarkTimer()
        {
            var timespan = DateTime.UtcNow - LastMarked;
            var num = CustomGameOptions.GhoulMarkCd * 1000f;
            var flag2 = num - (float)timespan.TotalMilliseconds < 0f;
            return flag2 ? 0f : (num - (float)timespan.TotalMilliseconds) / 1000f;
        }

        public void Fade()
        {
            Faded = true;
            Player.Visible = true;
            var color = new Color(1f, 1f, 1f, 0f);

            var maxDistance = ShipStatus.Instance.MaxLightRadius * TownOfUsReworked.VanillaOptions.CrewLightMod;
            var distance = (CustomPlayer.Local.GetTruePosition() - Player.GetTruePosition()).magnitude;

            var distPercent = distance / maxDistance;
            distPercent = Mathf.Max(0, distPercent - 1);

            var velocity = Player.gameObject.GetComponent<Rigidbody2D>().velocity.magnitude;
            color.a = 0.07f + (velocity / Player.MyPhysics.TrueSpeed * 0.13f);
            color.a = Mathf.Lerp(color.a, 0, distPercent);

            if (Player.GetCustomOutfitType() != CustomPlayerOutfitType.PlayerNameOnly)
                Player.SetOutfit(CustomPlayerOutfitType.PlayerNameOnly, Utils.BlankOutfit(Player));

            Player.MyRend().color = color;
            Player.NameText().color = new(0f, 0f, 0f, 0f);
            Player.cosmetics.colorBlindText.color = new(0f, 0f, 0f, 0f);
        }

        public void Mark()
        {
            if (MarkTimer() != 0f || Utils.IsTooFar(Player, MarkButton.TargetPlayer))
                return;

            var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.Action, SendOption.Reliable);
            writer.Write((byte)ActionsRPC.Mark);
            writer.Write(PlayerId);
            writer.Write(MarkButton.TargetPlayer.PlayerId);
            AmongUsClient.Instance.FinishRpcImmediately(writer);
            MarkedPlayer = MarkButton.TargetPlayer;
        }

        public bool Exception1(PlayerControl player) => player == MarkedPlayer || player.Is(Faction) || player.Is(SubFaction);

        public override void UpdateHud(HudManager __instance)
        {
            base.UpdateHud(__instance);
            KillButton.Disable();
            MarkButton.Update("MARK", MarkTimer(), CustomGameOptions.GhoulMarkCd, true, MarkedPlayer == null);
        }
    }
}