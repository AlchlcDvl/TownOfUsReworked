namespace TownOfUsReworked.PlayerLayers.Modifiers
{
    public class Volatile : Modifier
    {
        private float _time;
        private int RandomNumber;
        private int OtherNumber;

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

            if (Minigame.Instance || IntroCutscene.Instance)
                return;

            _time += Time.deltaTime;

            if (_time >= CustomGameOptions.VolatileInterval)
            {
                RandomNumber = URandom.RandomRangeInt(0, 3);
                _time -= CustomGameOptions.VolatileInterval;
                Hidden = false;
                Player.RegenTask();

                if (RandomNumber == 0)
                {
                    //Flashes
                    OtherNumber = URandom.RandomRangeInt(0, 256);
                    var otherNumber2 = URandom.RandomRangeInt(0, 256);
                    var otherNumber3 = URandom.RandomRangeInt(0, 256);
                    var flashColor = new Color32((byte)OtherNumber, (byte)otherNumber2, (byte)otherNumber3, 255);
                    Utils.Flash(flashColor);
                }
                else if (RandomNumber == 1)
                {
                    //Fake someone killing you
                    var fakePlayer = CustomPlayer.AllPlayers.ToArray().ToList().Random();
                    Player.NetTransform.Halt();
                    __instance.KillOverlay.ShowKillAnimation(fakePlayer.Data, Player.Data);
                }
                /*else if (RandomNumber == 2)
                {
                    //Hearing things
                    OtherNumber = URandom.RandomRangeInt(0, AssetManager.Sounds.Count);
                    var sound = AssetManager.Sounds[OtherNumber];
                    AssetManager.Play(sound);
                }*/
            }
        }
    }
}