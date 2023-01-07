/*using System.Collections.Generic;
using HarmonyLib;
using UnityEngine;
using TownOfUsReworked.Enums;
using TownOfUsReworked.Extensions;

namespace TownOfUsReworked.Patches
{
    [HarmonyPatch(typeof(KillOverlay), nameof(KillOverlay.ShowKillAnimation))]
    public class KillBackground
    {
        private static List<PlayerControl> AllPlayers;

        private static MurderEnum GetKiller(GameData.PlayerInfo killer, GameData.PlayerInfo victim)
        {
            if (victim.Object.Is(RoleEnum.Shifter) || (killer.Object.Is(RoleEnum.Shifter)))
                return MurderEnum.Shifter;

            if (killer.Object.Is(RoleEnum.Vigilante))
                return MurderEnum.Vigilante;

            if (killer.Object.Is(RoleEnum.Glitch))
                return MurderEnum.Glitch;

            if (killer.Object.Is(RoleEnum.Veteran))
                return MurderEnum.Veteran;

            if (killer.Object.Is(RoleEnum.Juggernaut))
                return MurderEnum.Juggernaut;

            if (killer.Object.Is(RoleEnum.Arsonist))
                return MurderEnum.Arsonist;

            if (killer.Object.Is(RoleEnum.Pestilence))
                return MurderEnum.Pestilence;

            if (killer.Object.Is(RoleEnum.Werewolf))
                return MurderEnum.Werewolf;

            if (killer.Object.Is(ObjectifierEnum.Lovers))
                return MurderEnum.Lover;

            return MurderEnum.Normal;
        }

        public static void Prefix(KillOverlay __instance, GameData.PlayerInfo pc)
        {
            var prefab = __instance;
            var renderer = __instance.background;
            var closestPlayer = Utils.GetClosestPlayer(PlayerControl.LocalPlayer, AllPlayers);
            var player = pc.Object;
            var option = GetKiller(PlayerControl.LocalPlayer.Data, closestPlayer.Data);
            var wait = new WaitForSeconds(0.83333336f);

            switch (option)
            {
                case MurderEnum.Normal:
                    renderer.color = Colors.Intruder;
                    renderer.sprite = TownOfUsReworked.NormalKill;
                    break;
                    
                case MurderEnum.Vigilante:
                    renderer.color = Colors.Vigilante;
                    renderer.sprite = TownOfUsReworked.SheriffKill;
                    break;

                case MurderEnum.Shifter:
                    renderer.color = Colors.Shifter;
                    renderer.sprite = TownOfUsReworked.ShiftKill;
                    break;

                case MurderEnum.Lover:
                    renderer.color = Colors.Lovers;
                    renderer.sprite = TownOfUsReworked.LoverKill;
                    break;
                case MurderEnum.Glitch:
                    renderer.color = Colors.Glitch;
                    renderer.sprite = TownOfUsReworked.GlitchKill;
                    break;

                case MurderEnum.Juggernaut:
                    renderer.color = Colors.Juggernaut;
                    renderer.sprite = TownOfUsReworked.JuggKill;
                    break;

                case MurderEnum.Werewolf:
                    renderer.color = Colors.Werewolf;
                    renderer.sprite = TownOfUsReworked.WWKill;
                    break;

                case MurderEnum.Pestilence:
                    renderer.color = Colors.Pestilence;
                    renderer.sprite = TownOfUsReworked.PestKill;
                    break;

                case MurderEnum.Veteran:
                    renderer.color = Colors.Veteran;
                    renderer.sprite = TownOfUsReworked.VetKill;
                    break;

                case MurderEnum.Arsonist:
                    renderer.color = Colors.Arsonist;
                    renderer.sprite = TownOfUsReworked.ArsoKill;
                    break;
            }
        }
    }
}*/