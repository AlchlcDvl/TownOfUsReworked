/*using System.Reflection;
using System.Collections.Generic;
using HarmonyLib;
using UnityEngine;

namespace TownOfUs
{
    [HarmonyPatch(typeof(KillOverlay), nameof(KillOverlay.ShowKillAnimation))]
    [HarmonyPriority(Priority.Last)]
    public class KillBackground
    {
        public static MethodBase TargetMethod()
        {
            return typeof(KillOverlay).GetNestedType("NLJFBGBIAFG").GetMethod("MoveNext");
        }
        
        private static List<PlayerControl> AllPlayers;

        private static MurderEnum GetOption(GameData.PlayerInfo killer, GameData.PlayerInfo victim)
        {
            if (victim.Object.Is(RoleEnum.Shifter) || (killer.Object.Is(RoleEnum.Shifter))) return MurderEnum.Shifter;
            if (killer.Object.Is(RoleEnum.Sheriff)) return MurderEnum.Sheriff;
            if (killer.Object.Is(RoleEnum.Glitch)) return MurderEnum.Glitch;
            if (killer.Object.Is(RoleEnum.Veteran)) return MurderEnum.Veteran;
            if (killer.Object.Is(RoleEnum.Juggernaut)) return MurderEnum.Juggernaut;
            if (killer.Object.Is(RoleEnum.Arsonist)) return MurderEnum.Arsonist;
            if (killer.Object.Is(RoleEnum.Pestilence)) return MurderEnum.Pestilence;
            if (killer.Object.Is(RoleEnum.Werewolf)) return MurderEnum.Werewolf;
            if (killer.Object.Is(ModifierEnum.Lover)) return MurderEnum.Lover;
            return MurderEnum.Normal;
        }

        public static void Prefix(KillOverlay __instance, GameData.PlayerInfo pc)
        {
            var prefab = __instance;
            var renderer = __instance.flameParent.transform.GetChild(0).gameObject.GetComponent<SpriteRenderer>();
            var closestPlayer = Utils.GetClosestPlayer(PlayerControl.LocalPlayer, AllPlayers);
            PlayerControl @object = pc.Object;

            var option = GetOption(PlayerControl.LocalPlayer.Data, closestPlayer.Data);
            var wait = new WaitForSeconds(0.83333336f);
            var hud = DestroyableSingleton<HudManager>.Instance;
            var overlay = hud.KillOverlay;
            var transform = overlay.flameParent.transform;
            var flame = transform.GetChild(0).gameObject;

            switch (option)
            {
                case MurderEnum.Normal:
                    renderer.color = Patches.Colors.Impostor;
                    renderer.sprite = TownOfUs.NormalKill;
                    break;
                    
                case MurderEnum.Sheriff:
                    renderer.color = Patches.Colors.Sheriff;
                    renderer.sprite = TownOfUs.SheriffKill;
                    break;

                case MurderEnum.Shifter:
                    renderer.color = Patches.Colors.Shifter;
                    renderer.sprite = TownOfUs.ShiftKill;
                    break;

                case MurderEnum.Lover:
                    renderer.color = Patches.Colors.Lovers;
                    renderer.sprite = TownOfUs.LoverKill;
                    break;
                case MurderEnum.Glitch:
                    renderer.color = Patches.Colors.Glitch;
                    renderer.sprite = TownOfUs.GlitchKill;
                    break;

                case MurderEnum.Juggernaut:
                    renderer.color = Patches.Colors.Juggernaut;
                    renderer.sprite = TownOfUs.JuggKill;
                    break;

                case MurderEnum.Werewolf:
                    renderer.color = Patches.Colors.Werewolf;
                    renderer.sprite = TownOfUs.WWKill;
                    break;

                case MurderEnum.Pestilence:
                    renderer.color = Patches.Colors.Pestilence;
                    renderer.sprite = TownOfUs.PestKill;
                    break;

                case MurderEnum.Veteran:
                    renderer.color = Patches.Colors.Veteran;
                    renderer.sprite = TownOfUs.VetKill;
                    break;

                case MurderEnum.Arsonist:
                    renderer.color = Patches.Colors.Arsonist;
                    renderer.sprite = TownOfUs.ArsoKill;
                    break;
            }
        }
    }
}*/