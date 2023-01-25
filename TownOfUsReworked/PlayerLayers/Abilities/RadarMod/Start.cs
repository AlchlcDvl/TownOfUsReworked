using HarmonyLib;
using UnityEngine;
using TownOfUsReworked.Enums;
using TownOfUsReworked.Extensions;
using TownOfUsReworked.PlayerLayers.Abilities.Abilities;

namespace TownOfUsReworked.PlayerLayers.Abilities.RadarMod
{
    [HarmonyPatch(typeof(IntroCutscene._CoBegin_d__29), nameof(IntroCutscene._CoBegin_d__29.MoveNext))]
    public static class Start
    {
        public static Sprite Sprite => TownOfUsReworked.Arrow;

        public static void Postfix(IntroCutscene._CoBegin_d__29 __instance)
        {
            foreach (var ability in Ability.GetAbilities(AbilityEnum.Radar))
            {
                if (PlayerControl.LocalPlayer.Is(AbilityEnum.Radar))
                {
                    var radar = (Radar)ability;
                    var gameObj = new GameObject();
                    var arrow = gameObj.AddComponent<ArrowBehaviour>();
                    gameObj.transform.parent = PlayerControl.LocalPlayer.gameObject.transform;
                    var renderer = gameObj.AddComponent<SpriteRenderer>();
                    renderer.sprite = Sprite;
                    renderer.color = radar.Color;
                    arrow.image = renderer;
                    gameObj.layer = 5;
                    arrow.target = PlayerControl.LocalPlayer.transform.position;
                    radar.RadarArrow.Add(arrow);
                }
            }
        }
    }
}