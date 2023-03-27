using HarmonyLib;
using UnityEngine;
using TownOfUsReworked.Enums;
using TownOfUsReworked.Classes;
using TownOfUsReworked.Extensions;

namespace TownOfUsReworked.PlayerLayers.Abilities.RadarMod
{
    [HarmonyPatch(typeof(IntroCutscene._CoBegin_d__29), nameof(IntroCutscene._CoBegin_d__29.MoveNext))]
    public static class Start
    {
        public static void Postfix()
        {
            if (PlayerControl.LocalPlayer.Is(AbilityEnum.Radar))
            {
                var radar = Ability.GetAbility<Radar>(PlayerControl.LocalPlayer);
                var gameObj = new GameObject();
                var arrow = gameObj.AddComponent<ArrowBehaviour>();
                gameObj.transform.parent = PlayerControl.LocalPlayer.gameObject.transform;
                var renderer = gameObj.AddComponent<SpriteRenderer>();
                renderer.sprite = AssetManager.Arrow;
                renderer.color = radar.Color;
                arrow.image = renderer;
                gameObj.layer = 5;
                arrow.target = PlayerControl.LocalPlayer.transform.position;
                radar.RadarArrow.Add(arrow);
            }
        }
    }
}