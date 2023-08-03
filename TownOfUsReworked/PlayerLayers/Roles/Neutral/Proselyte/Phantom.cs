namespace TownOfUsReworked.PlayerLayers.Roles
{
    public class Phantom : Neutral
    {
        public bool Caught { get; set; }
        public bool CompletedTasks { get; set; }
        public bool PhantomWin { get; set; }
        public bool Faded { get; set; }

        public override Color32 Color => ClientGameOptions.CustomNeutColors ? Colors.Phantom : Colors.Neutral;
        public override string Name => "Phantom";
        public override LayerEnum Type => LayerEnum.Phantom;
        public override RoleEnum RoleType => RoleEnum.Phantom;
        public override Func<string> StartText => () => "Peek-A-Boo!";
        public override Func<string> Description => () => "- You end the game upon finishing your objective";
        public override InspectorResults InspectorResults => InspectorResults.Ghostly;

        public Phantom(PlayerControl player) : base(player)
        {
            Objectives = () => "- Finish your tasks without getting clicked";
            RoleAlignment = RoleAlignment.NeutralPros;
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
    }
}