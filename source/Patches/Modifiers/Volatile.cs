using HarmonyLib;
using UnityEngine;
using Reactor;
using Random = UnityEngine.Random;
namespace TownOfUs.Modifiers
{
    public class Volatile
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
                    if (_time >= 45f)
                    {
                        randomNumber = Random.RandomRangeInt(0, 6);
                        _time -= 45f;
                        try {
                            if (randomNumber == 0)
                            {
                                //Press kill button
                                __instance.KillButton.DoClick();
                            }
                            else if (randomNumber == 1)
                            {
                                //Fake sabotage flash
                                Coroutines.Start(Utils.FlashCoroutine(Palette.ImpostorRed, 1f));
                            }
                            else if (randomNumber == 2)
                            {
                                //Fake death sound
                                SoundManager.Instance.PlaySound(PlayerControl.LocalPlayer.KillSfx, false, 0.25f);
                            }
                            else if (randomNumber == 3)
                            {
                                //Fake you killed you
                                DestroyableSingleton<HudManager>.Instance.KillOverlay.ShowKillAnimation(PlayerControl.LocalPlayer.Data, PlayerControl.LocalPlayer.Data);
                            }
                            else if (randomNumber == 4)
                            {
                                //Fake role sound effects
                                otherNumber = Random.RandomRangeInt(0, 6);

                                try {
                                    AudioClip VolatileSFX = TownOfUs.loadAudioClipFromResources("TownOfUs.Resources.Sample.raw");
                                    SoundManager.Instance.PlaySound(VolatileSFX, false, 0.4f);
                                }
                                catch {}
                            }
                        }
                        catch {}
                    }
                }
            }
        }
    }
}