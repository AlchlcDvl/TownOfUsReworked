namespace TownOfUsReworked.PlayerLayers.Modifiers;

[HeaderOption(MultiMenu.LayerSubOptions)]
public class Volatile : Modifier
{
    [NumberOption(10f, 30f, 1f, Format.Time)]
    private static Number VolatileInterval = 15;

    [ToggleOption]
    private static bool VolatileKnows = true;

    private float Time;
    private bool Exposed { get; set; }

    private static bool Lmao;

    public override UColor Color => ClientOptions.CustomModColors ? CustomColorManager.Volatile : CustomColorManager.Modifier;
    public override LayerEnum Type => LayerEnum.Volatile;
    public override Func<string> Description => () => "- You experience hallucinations";
    public override bool Hidden => !VolatileKnows && !Exposed && !Dead;

    private static readonly string[] Links = [ "https://www.youtube.com/watch?v=79-AwFZCKpA", "https://www.youtube.com/watch?v=xm3YgoEiEDc" ];

    protected override void Init() => Exposed = VolatileKnows;

    public override void UpdateHud(HudManager __instance)
    {
        if (ActiveTask() || IntroCutscene.Instance || ShowRolePatch.Starting || Dead)
            return;

        Time += UnityEngine.Time.deltaTime;

        if (Time >= VolatileInterval)
        {
            var randomNumber = URandom.RandomRangeInt(0, 5);
            Time -= VolatileInterval;
            Exposed = true;

            switch (randomNumber)
            {
                // Flashes
                case 0:
                {
                    var otherNumber = (byte)URandom.RandomRangeInt(0, 256);
                    var otherNumber2 = (byte)URandom.RandomRangeInt(0, 256);
                    var otherNumber3 = (byte)URandom.RandomRangeInt(0, 256);
                    Flash(new Color32(otherNumber, otherNumber2, otherNumber3, 255));
                    break;
                }
                // Fake someone killing you
                case 1:
                {
                    Player.NetTransform.Halt();
                    __instance.KillOverlay.ShowKillAnimation(AllPlayers().Random().Data, Data);
                    break;
                }
                // Get rick rolled lmao
                case 2 when !Lmao && !TownOfUsReworked.IsStream:
                {
                    Lmao = true;
                    Application.OpenURL(Links.Random());
                    break;
                }
                // Hear random things
                case 3:
                {
                    Play(GetAll<AudioClip>().Random());
                    break;
                }
            }
        }
    }
}