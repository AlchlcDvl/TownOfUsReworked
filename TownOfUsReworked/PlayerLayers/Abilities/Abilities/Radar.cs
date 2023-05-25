namespace TownOfUsReworked.PlayerLayers.Abilities
{
    public class Radar : Ability
    {
        public CustomArrow RadarArrow;

        public Radar(PlayerControl player) : base(player)
        {
            Name = "Radar";
            TaskText = "- You are aware of those close to you";
            Color = CustomGameOptions.CustomAbilityColors ? Colors.Radar : Colors.Ability;
            AbilityType = AbilityEnum.Radar;
            RadarArrow = new(Player, Color);
            Type = LayerEnum.Radar;
        }

        public override void OnLobby()
        {
            base.OnLobby();
            RadarArrow?.Destroy();
        }

        public override void UpdateHud(HudManager __instance)
        {
            base.UpdateHud(__instance);

            if (IsDead)
                OnLobby();
            else
                RadarArrow.Update(Player.GetClosestPlayer(null, float.MaxValue, true).transform.position);
        }
    }
}