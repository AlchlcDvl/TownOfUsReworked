using HarmonyLib;
using UnityEngine;
using Reactor.Utilities;
using Random = UnityEngine.Random;
using TownOfUsReworked.Extensions;
using TownOfUsReworked.Lobby.CustomOption;
using TownOfUsReworked.Enums;
using TownOfUsReworked.PlayerLayers.Roles;
using System.Collections.Generic;

namespace TownOfUsReworked.PlayerLayers.Modifiers.VolatileMod
{
    public class Actions
    {
        public static float _time = 0f;
        public static int randomNumber = 0;
        public static int otherNumber = 0;
        
        [HarmonyPatch(typeof(HudManager), nameof(HudManager.Update))]
        public class HudManagerUpdate
        {
            public static void Postfix(HudManager __instance)
            {
                if (PlayerControl.LocalPlayer.Is(ModifierEnum.Volatile) && !PlayerControl.LocalPlayer.Data.IsDead && PlayerControl.LocalPlayer.CanMove && !MeetingHud.Instance)
                {
                    _time += Time.deltaTime;
                    
                    if (_time >= CustomGameOptions.VolatileInterval)
                    {
                        randomNumber = Random.RandomRangeInt(0, 4);
                        _time -= CustomGameOptions.VolatileInterval;

                        if (randomNumber == 0)
                        {
                            //Uses primary ability
                            if (__instance.KillButton != null)
                                __instance.KillButton.DoClick();
                        }
                        else if (randomNumber == 1)
                        {
                            //Flashes
                            var allRoles = new List<Role>();

                            foreach (var role in Role.AllRoles)
                                allRoles.Add(role);

                            otherNumber = Random.RandomRangeInt(0, allRoles.Count);
                            var role2 = allRoles[otherNumber];

                            Coroutines.Start(Utils.FlashCoroutine(role2.Color));
                        }
                        else if (randomNumber == 2)
                        {
                            //Fake someone killing you
                            otherNumber = Random.RandomRangeInt(0, PlayerControl.AllPlayerControls.Count);
                            var fakePlayer = PlayerControl.AllPlayerControls[otherNumber];

                            DestroyableSingleton<HudManager>.Instance.KillOverlay.ShowKillAnimation(fakePlayer.Data, PlayerControl.LocalPlayer.Data);
                        }
                        else if (randomNumber == 3)
                        {
                            //Fake role sound effects
                            otherNumber = Random.RandomRangeInt(0, Lists.Sounds.Count);
                            var sound = Lists.Sounds[otherNumber];

                            try
                            {
                                SoundManager.Instance.PlaySound(sound, false, 1f);
                            } catch {}
                        }
                    }
                }
            }
        }
    }
}