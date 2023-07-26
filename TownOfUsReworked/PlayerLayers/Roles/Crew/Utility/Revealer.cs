namespace TownOfUsReworked.PlayerLayers.Roles
{
    public class Revealer : Crew
    {
        public bool Caught;
        public bool Revealed;
        public bool CompletedTasks;
        public bool Faded;
        public Role FormerRole;

        public override Color32 Color => ClientGameOptions.CustomCrewColors ? Colors.Revealer : Colors.Crew;
        public override string Name => "Revealer";
        public override LayerEnum Type => LayerEnum.Revealer;
        public override RoleEnum RoleType => RoleEnum.Revealer;
        public override Func<string> StartText => () => "OOOOOOO";
        public override Func<string> AbilitiesText => () => "- You can reveal evils players to the <color=#8CFFFFFF>Crew</color> once you finish your tasks without getting clicked.";
        public override InspectorResults InspectorResults => InspectorResults.Ghostly;

        public Revealer(PlayerControl player) : base(player) => RoleAlignment = RoleAlignment.CrewUtil;

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