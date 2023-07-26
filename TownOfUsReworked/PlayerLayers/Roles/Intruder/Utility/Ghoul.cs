namespace TownOfUsReworked.PlayerLayers.Roles
{
    public class Ghoul : Intruder
    {
        public CustomButton MarkButton;
        public bool Caught;
        public bool Faded;
        public DateTime LastMarked;
        public PlayerControl MarkedPlayer;

        public override Color32 Color => ClientGameOptions.CustomIntColors ? Colors.Ghoul : Colors.Intruder;
        public override string Name => "Ghoul";
        public override LayerEnum Type => LayerEnum.Ghoul;
        public override RoleEnum RoleType => RoleEnum.Ghoul;
        public override Func<string> StartText => () => "BOO!";
        public override Func<string> AbilitiesText => () => "- You can mark a player for death every round\n- Marked players will be announced to all players and will die at the end of the"
            + " next meeting if you are not clicked";
        public override InspectorResults InspectorResults => InspectorResults.Ghostly;

        public Ghoul(PlayerControl player) : base(player)
        {
            RoleAlignment = RoleAlignment.IntruderUtil;
            MarkedPlayer = null;
            MarkButton = new(this, "GhoulMark", AbilityTypes.Direct, "ActionSecondary", Mark, Exception1, false, true);
        }

        public float MarkTimer()
        {
            var timespan = DateTime.UtcNow - LastMarked;
            var num = CustomGameOptions.GhoulMarkCd * 1000f;
            var time = num - (float)timespan.TotalMilliseconds;
            var flag2 = time < 0f;
            return (flag2 ? 0f : time) / 1000f;
        }

        public void Fade()
        {
            Faded = true;
            Player.Visible = true;
            var color = new Color(1f, 1f, 1f, 0f);

            var maxDistance = ShipStatus.Instance.MaxLightRadius * TownOfUsReworked.NormalOptions.CrewLightMod;
            var distance = (CustomPlayer.Local.GetTruePosition() - Player.GetTruePosition()).magnitude;

            var distPercent = distance / maxDistance;
            distPercent = Mathf.Max(0, distPercent - 1);

            var velocity = Player.gameObject.GetComponent<Rigidbody2D>().velocity.magnitude;
            color.a = 0.07f + (velocity / Player.MyPhysics.TrueSpeed * 0.13f);
            color.a = Mathf.Lerp(color.a, 0, distPercent);

            if (Player.GetCustomOutfitType() != CustomPlayerOutfitType.PlayerNameOnly)
                Player.SetOutfit(CustomPlayerOutfitType.PlayerNameOnly, BlankOutfit(Player));

            Player.MyRend().color = color;
            Player.NameText().color = new(0f, 0f, 0f, 0f);
            Player.cosmetics.colorBlindText.color = new(0f, 0f, 0f, 0f);
        }

        public void Mark()
        {
            if (MarkTimer() != 0f || IsTooFar(Player, MarkButton.TargetPlayer))
                return;

            MarkedPlayer = MarkButton.TargetPlayer;
            CallRpc(CustomRPC.Action, ActionsRPC.Mark, this, MarkedPlayer);
        }

        public bool Exception1(PlayerControl player) => player == MarkedPlayer || player.Is(Faction) || (player.Is(SubFaction) && SubFaction != SubFaction.None);

        public override void UpdateHud(HudManager __instance)
        {
            base.UpdateHud(__instance);
            KillButton.Disable();
            MarkButton.Update("MARK", MarkTimer(), CustomGameOptions.GhoulMarkCd, true, MarkedPlayer == null);
        }
    }
}