namespace TownOfUsReworked.PlayerLayers.Modifiers
{
    public class Volatile : Modifier
    {
        private float _time;
        private int randomNumber;
        private int otherNumber;

        public Volatile(PlayerControl player) : base(player)
        {
            Name = "Volatile";
            TaskText = () => "- You experience a lot of hallucinations and lash out";
            Color = CustomGameOptions.CustomModifierColors ? Colors.Volatile : Colors.Modifier;
            ModifierType = ModifierEnum.Volatile;
            Hidden = !CustomGameOptions.VolatileKnows;
            Type = LayerEnum.Volatile;

            if (TownOfUsReworked.IsTest)
                Utils.LogSomething($"{Player.name} is {Name}");
        }

        public override void UpdateHud(HudManager __instance)
        {
            base.UpdateHud(__instance);

            if (Minigame.Instance)
                return;

            _time += Time.deltaTime;

            if (_time >= CustomGameOptions.VolatileInterval)
            {
                randomNumber = URandom.RandomRangeInt(0, 3);
                _time -= CustomGameOptions.VolatileInterval;
                Hidden = false;
                Player.RegenTask();

                if (randomNumber == 0)
                {
                    //Flashes
                    otherNumber = URandom.RandomRangeInt(0, 256);
                    var otherNumber2 = URandom.RandomRangeInt(0, 256);
                    var otherNumber3 = URandom.RandomRangeInt(0, 256);
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
                    otherNumber = URandom.RandomRangeInt(0, AssetManager.Sounds.Count);
                    var sound = AssetManager.Sounds[otherNumber];
                    AssetManager.Play(sound);
                }*/
            }
        }
    }
}