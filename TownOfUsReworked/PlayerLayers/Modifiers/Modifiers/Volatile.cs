namespace TownOfUsReworked.PlayerLayers.Modifiers;

public class Volatile : Modifier
{
    private float _time;
    private int OtherNumber { get; set; }
    private static bool RickRolled;
    public bool Exposed { get; set; }

    public override Color32 Color => ClientGameOptions.CustomModColors ? Colors.Volatile : Colors.Modifier;
    public override string Name => "Volatile";
    public override LayerEnum Type => LayerEnum.Volatile;
    public override Func<string> Description => () => "- You experience hallucinations";
    public override bool Hidden => !CustomGameOptions.VolatileKnows && !Exposed && !IsDead;

    public Volatile(PlayerControl player) : base(player) => Exposed = !CustomGameOptions.VolatileKnows;

    public override void UpdateHud(HudManager __instance)
    {
        base.UpdateHud(__instance);

        if (Minigame.Instance || IntroCutscene.Instance || IsDead)
            return;

        _time += Time.deltaTime;

        if (_time >= CustomGameOptions.VolatileInterval)
        {
            var randomNumber = URandom.RandomRangeInt(0, 4);
            _time -= CustomGameOptions.VolatileInterval;
            Exposed = !Exposed && randomNumber != 3;

            if (randomNumber == 0)
            {
                //Flashes
                OtherNumber = URandom.RandomRangeInt(0, 256);
                var otherNumber2 = URandom.RandomRangeInt(0, 256);
                var otherNumber3 = URandom.RandomRangeInt(0, 256);
                Flash(new((byte)OtherNumber, (byte)otherNumber2, (byte)otherNumber3, 255));
            }
            else if (randomNumber == 1)
            {
                //Fake someone killing you
                var fakePlayer = CustomPlayer.AllPlayers.Random();
                Player.NetTransform.Halt();
                __instance.KillOverlay.ShowKillAnimation(fakePlayer.Data, Player.Data);
            }
            else if (randomNumber == 2 && !RickRolled)
            {
                //Get rick rolled lmao
                RickRolled = true;
                Application.OpenURL("https://www.youtube.com/watch?v=xm3YgoEiEDc");
            }
        }
    }
}