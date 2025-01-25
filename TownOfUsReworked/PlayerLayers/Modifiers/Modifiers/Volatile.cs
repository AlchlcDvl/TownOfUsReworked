namespace TownOfUsReworked.PlayerLayers.Modifiers;

[HeaderOption(MultiMenu.LayerSubOptions)]
public class Volatile : Modifier
{
    [NumberOption(10f, 30f, 1f, Format.Time)]
    public static Number VolatileInterval = 15;

    [ToggleOption]
    public static bool VolatileKnows = true;

    private float _time;
    private bool Exposed { get; set; }

    private static bool LMAO;

    public override UColor Color => ClientOptions.CustomModColors ? CustomColorManager.Volatile : CustomColorManager.Modifier;
    public override LayerEnum Type => LayerEnum.Volatile;
    public override Func<string> Description => () => "- You experience hallucinations";
    public override bool Hidden => !VolatileKnows && !Exposed && !Dead;

    private static readonly string[] Links = [ "https://www.youtube.com/watch?v=79-AwFZCKpA", "https://www.youtube.com/watch?v=xm3YgoEiEDc" ];

    public override void Init() => Exposed = VolatileKnows;

    public override void UpdateHud(HudManager __instance)
    {
        if (ActiveTask() || IntroCutscene.Instance || ShowRolePatch.Starting || Dead)
            return;

        _time += Time.deltaTime;

        if (_time >= VolatileInterval)
        {
            var randomNumber = URandom.RandomRangeInt(0, 5);
            _time -= VolatileInterval;
            Exposed = true;

            // Flashes
            if (randomNumber == 0)
            {
                var otherNumber = (byte)URandom.RandomRangeInt(0, 256);
                var otherNumber2 = (byte)URandom.RandomRangeInt(0, 256);
                var otherNumber3 = (byte)URandom.RandomRangeInt(0, 256);
                Flash(new Color32(otherNumber, otherNumber2, otherNumber3, 255));
            }
            // Fake someone killing you
            else if (randomNumber == 1)
            {
                Player.NetTransform.Halt();
                __instance.KillOverlay.ShowKillAnimation(AllPlayers().Random().Data, Data);
            }
            // Get rick rolled lmao
            else if (randomNumber == 2 && !LMAO && !TownOfUsReworked.IsStream)
            {
                LMAO = true;
                Application.OpenURL(Links.Random());
            }
            // Hear random things
            else if (randomNumber == 3)
                Play(UnityGetAll<AudioClip>().Random());
        }
    }
}