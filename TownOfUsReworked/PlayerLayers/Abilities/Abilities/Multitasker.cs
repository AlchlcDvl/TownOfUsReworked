using TownOfUsReworked.Data;
using TownOfUsReworked.CustomOptions;
using UnityEngine;

namespace TownOfUsReworked.PlayerLayers.Abilities
{
    public class Multitasker : Ability
    {
        public Multitasker(PlayerControl player) : base(player)
        {
            Name = "Multitasker";
            TaskText = "- Your task windows are transparent";
            Color = CustomGameOptions.CustomAbilityColors ? Colors.Multitasker : Colors.Ability;
            AbilityType = AbilityEnum.Multitasker;
            Type = LayerEnum.Multitasker;
        }

        public override void UpdateHud(HudManager __instance)
        {
            base.UpdateHud(__instance);

            if (!Minigame.Instance)
                return;

            var rends = Minigame.Instance.GetComponentsInChildren<SpriteRenderer>();
            var trans = CustomGameOptions.Transparancy / 100f;

            for (int i = 0; i < rends.Length; i++)
            {
                var oldColor1 = rends[i].color[0];
                var oldColor2 = rends[i].color[1];
                var oldColor3 = rends[i].color[2];
                rends[i].color = new Color(oldColor1, oldColor2, oldColor3, trans);
            }
        }
    }
}