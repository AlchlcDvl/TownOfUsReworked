using HarmonyLib;
using UnityEngine;
using Reactor.Utilities;
using Random = UnityEngine.Random;
using TownOfUsReworked.Extensions;
using TownOfUsReworked.Lobby.CustomOption;
using TownOfUsReworked.PlayerLayers.Roles;
using TownOfUsReworked.Enums;
using TownOfUsReworked.Extentions;

namespace TownOfUsReworked.PlayerLayers.Modifiers.VolatileMod
{
    [HarmonyPatch(typeof(HudManager), nameof(HudManager.Update))]
    public class Actions
    {
        public static float _time = 0f;
        public static int randomNumber = 0;
        public static int otherNumber = 0;

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
                            //Uses a random ability
                            var role = Role.GetRole(PlayerControl.LocalPlayer);

                            if (role != null)
                            {
                                if (role.ExtraButtons.Count != 0)
                                {
                                    otherNumber = Random.RandomRangeInt(0, role.ExtraButtons.Count);
                                    var button = role.ExtraButtons[otherNumber].Value;
                                    
                                    button.DoClick();
                                }
                            }
                        }
                        else if (randomNumber == 1)
                        {
                            //Flashes
                            otherNumber = Random.RandomRangeInt(0, Lists.AllRoles.Count);
                            var role2 = Lists.AllRoles[otherNumber];

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
                            otherNumber = Random.RandomRangeInt(0, SoundEffects.Sounds.Count);
                            AudioClip sound = SoundEffects.Sounds[otherNumber];

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