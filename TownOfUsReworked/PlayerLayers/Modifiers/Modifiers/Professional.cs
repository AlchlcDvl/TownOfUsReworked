namespace TownOfUsReworked.PlayerLayers.Modifiers
{
    public class Professional : Modifier
    {
        public bool LifeUsed;

        public Professional(PlayerControl player) : base(player)
        {
            Name = "Professional";
            TaskText = "- You have an extra life when assassinating.";
            Color = CustomGameOptions.CustomModifierColors ? Colors.Professional : Colors.Modifier;
            ModifierType = ModifierEnum.Professional;
            Hidden = !CustomGameOptions.ProfessionalKnows && !LifeUsed;
            LifeUsed = false;
            Type = LayerEnum.Professional;
        }
    }
}