using TownOfUsReworked.CustomOptions;
using TownOfUsReworked.Data;
using UnityEngine;
using TownOfUsReworked.Classes;
using TownOfUsReworked.Extensions;
using System.Linq;

namespace TownOfUsReworked.PlayerLayers.Modifiers
{
    public class Volatile : Modifier
    {
        public bool Exposed;
        private static float _time;
        private static int randomNumber;
        private static int otherNumber;

        public Volatile(PlayerControl player) : base(player)
        {
            Name = "Volatile";
            TaskText = "- You experience a lot of hallucinations and lash out.";
            Color = CustomGameOptions.CustomModifierColors ? Colors.Volatile : Colors.Modifier;
            ModifierType = ModifierEnum.Volatile;
            Hidden = !CustomGameOptions.VolatileKnows && !Exposed;
            Type = LayerEnum.Volatile;
            Exposed = false;
        }

        public override void UpdateHud(HudManager __instance)
        {
            base.UpdateHud(__instance);
            _time += Time.deltaTime;

            if (_time >= CustomGameOptions.VolatileInterval)
            {
                randomNumber = Random.RandomRangeInt(0, 3);
                _time -= CustomGameOptions.VolatileInterval;
                Exposed = true;
                Player.RegenTask();

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
                    var fakePlayer = PlayerControl.AllPlayerControls.ToArray().ToList().Random();
                    Player.NetTransform.Halt();
                    __instance.KillOverlay.ShowKillAnimation(fakePlayer.Data, Player.Data);
                }
                /*else if (randomNumber == 2)
                {
                    //Hearing things
                    otherNumber = Random.RandomRangeInt(0, AssetManager.Sounds.Count);
                    var sound = AssetManager.Sounds[otherNumber];
                    AssetManager.Play(sound);
                }*/
            }
        }
    }
}