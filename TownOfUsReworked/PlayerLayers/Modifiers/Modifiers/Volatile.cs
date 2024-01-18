namespace TownOfUsReworked.PlayerLayers.Modifiers;

public class Volatile : Modifier
{
    private float _time;
    private int OtherNumber { get; set; }
    private bool Exposed { get; set; }

    private static bool LMAO;

    public override UColor Color => ClientGameOptions.CustomModColors ? CustomColorManager.Volatile : CustomColorManager.Modifier;
    public override string Name => "Volatile";
    public override LayerEnum Type => LayerEnum.Volatile;
    public override Func<string> Description => () => "- You experience hallucinations";
    public override bool Hidden => !CustomGameOptions.VolatileKnows && !Exposed && !IsDead;

    private static readonly string[] Links = { "https://www.youtube.com/watch?v=79-AwFZCKpA", "https://www.youtube.com/watch?v=xm3YgoEiEDc" };

    public Volatile(PlayerControl player) : base(player) => Exposed = !CustomGameOptions.VolatileKnows;

    public override void UpdateHud(HudManager __instance)
    {
        base.UpdateHud(__instance);

        if (ActiveTask || IntroCutscene.Instance || IsDead)
            return;

        _time += Time.deltaTime;

        if (_time >= CustomGameOptions.VolatileInterval)
        {
            var randomNumber = URandom.RandomRangeInt(0, 5);
            _time -= CustomGameOptions.VolatileInterval;
            Exposed = !Exposed && randomNumber != 3;

            //Flashes
            if (randomNumber == 0)
            {
                OtherNumber = URandom.RandomRangeInt(0, 256);
                var otherNumber2 = URandom.RandomRangeInt(0, 256);
                var otherNumber3 = URandom.RandomRangeInt(0, 256);
                Flash(new((byte)OtherNumber, (byte)otherNumber2, (byte)otherNumber3, 255));
            }
            //Fake someone killing you
            else if (randomNumber == 1)
            {
                Player.NetTransform.Halt();
                __instance.KillOverlay.ShowKillAnimation(CustomPlayer.AllPlayers.Random().Data, Data);
            }
            //Get rick rolled lmao
            else if (randomNumber == 2 && !LMAO && !TownOfUsReworked.IsStream)
            {
                LMAO = true;
                Application.OpenURL(Links.Random());
            }
            //Hear random things
            else if (randomNumber == 3)
                Play(SoundEffects.Keys.Random());
        }
    }
}