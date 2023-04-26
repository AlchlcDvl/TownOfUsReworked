using System.Collections.Generic;
using TownOfUsReworked.CustomOptions;
using TownOfUsReworked.Classes;
using TownOfUsReworked.Data;

namespace TownOfUsReworked.PlayerLayers.Abilities
{
    public class Radar : Ability
    {
        public List<ArrowBehaviour> RadarArrow = new();
        public PlayerControl ClosestPlayer;

        public Radar(PlayerControl player) : base(player)
        {
            Name = "Radar";
            TaskText = "- You are aware of those close to you";
            Color = CustomGameOptions.CustomAbilityColors ? Colors.Radar : Colors.Ability;
            AbilityType = AbilityEnum.Radar;
            RadarArrow = new();
            Type = LayerEnum.Radar;
        }

        public override void OnLobby() => RadarArrow.DestroyAll();

        public override void UpdateHud(HudManager __instance)
        {
            base.UpdateHud(__instance);

            if (Player.Data.IsDead)
            {
                RadarArrow.DestroyAll();
                RadarArrow.Clear();
            }

            foreach (var arrow in RadarArrow)
            {
                ClosestPlayer = PlayerControl.LocalPlayer.GetClosestPlayer(null, float.MaxValue);
                arrow.target = ClosestPlayer.transform.position;
            }
        }
    }
}