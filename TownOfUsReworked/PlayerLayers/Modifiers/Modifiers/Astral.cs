namespace TownOfUsReworked.PlayerLayers.Modifiers
{
    public class Astral : Modifier
    {
        public Vector3 LastPosition = Vector3.zero;

        public Astral(PlayerControl player) : base(player)
        {
            Name = "Astral";
            TaskText = () => "- You will not teleport to the meeting button";
            Color = CustomGameOptions.CustomModifierColors ? Colors.Astral : Colors.Modifier;
            ModifierType = ModifierEnum.Astral;
            Type = LayerEnum.Astral;
            LastPosition = Vector3.zero;

            if (TownOfUsReworked.IsTest)
                Utils.LogSomething($"{Player.name} is {Name}");
        }

        public void SetPosition()
        {
            if (LastPosition == Vector3.zero)
                return;

            Player.NetTransform.RpcSnapTo(LastPosition);

            if (ModCompatibility.IsSubmerged)
                ModCompatibility.ChangeFloor(LastPosition.y > -7);
        }
    }
}