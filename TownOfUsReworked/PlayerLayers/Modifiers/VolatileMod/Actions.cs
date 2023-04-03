using HarmonyLib;
using UnityEngine;
using TownOfUsReworked.Classes;
using TownOfUsReworked.CustomOptions;
using TownOfUsReworked.Data;

namespace TownOfUsReworked.PlayerLayers.Modifiers.VolatileMod
{
    [HarmonyPatch(typeof(HudManager), nameof(HudManager.Update))]
    public static class Actions
    {
        private static float _time;
        private static int randomNumber;
        private static int otherNumber;

        public static void Postfix(HudManager __instance)
        {
            if (Utils.NoButton(PlayerControl.LocalPlayer, ModifierEnum.Volatile) || PlayerControl.LocalPlayer.Data.IsDead)
                return;

            _time += Time.deltaTime;

            if (_time >= CustomGameOptions.VolatileInterval)
            {
                randomNumber = Random.RandomRangeInt(0, 3);
                _time -= CustomGameOptions.VolatileInterval;

                if (randomNumber == 0)
                {
                    //Flashes
                    otherNumber = Random.RandomRangeInt(0, 256);
                    var otherNumber2 = Random.RandomRangeInt(0, 256);
                    var otherNumber3 = Random.RandomRangeInt(0, 256);
                    var flashColor = new Color32((byte)otherNumber, (byte)otherNumber2, (byte)otherNumber3, 255);
                    Utils.Flash(flashColor);
                }
                else if (randomNumber == 1)
                {
                    //Fake someone killing you
                    otherNumber = Random.RandomRangeInt(0, PlayerControl.AllPlayerControls.Count);
                    var fakePlayer = PlayerControl.AllPlayerControls[otherNumber];
                    __instance.KillOverlay.ShowKillAnimation(fakePlayer.Data, PlayerControl.LocalPlayer.Data);
                }
                else if (randomNumber == 2)
                {
                    //Hearing things
                    otherNumber = Random.RandomRangeInt(0, AssetManager.Sounds.Count);
                    var sound = AssetManager.Sounds[otherNumber];
                    AssetManager.Play(sound);
                }
            }
        }
    }
}