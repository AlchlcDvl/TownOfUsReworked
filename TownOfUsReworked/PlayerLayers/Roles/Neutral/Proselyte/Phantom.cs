namespace TownOfUsReworked.PlayerLayers.Roles
{
    public class Phantom : Neutral
    {
        public bool Caught;
        public bool CompletedTasks;
        public bool PhantomWin;
        public bool Faded;

        public Phantom(PlayerControl player) : base(player)
        {
            Name = "Phantom";
            StartText = () => "Peek-A-Boo!";
            Color = CustomGameOptions.CustomNeutColors ? Colors.Phantom : Colors.Neutral;
            Objectives = () => "- Finish your tasks without getting clicked";
            RoleType = RoleEnum.Phantom;
            RoleAlignment = RoleAlignment.NeutralPros;
            Type = LayerEnum.Phantom;
            InspectorResults = InspectorResults.Ghostly;

            if (TownOfUsReworked.IsTest)
                Utils.LogSomething($"{Player.name} is {Name}");
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
    }
}