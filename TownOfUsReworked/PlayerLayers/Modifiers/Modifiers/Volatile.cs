// ReSharper disable HeuristicUnreachableCode
#pragma warning disable CS0162 // Unreachable code detected

namespace TownOfUsReworked.PlayerLayers.Modifiers;

[LayerHeaderOption(Layer.Volatile)]
public sealed class Volatile : Modifier
{
    [NumberOption(10f, 30f, 1f, Format.Time)]
    private static Number VolatileInterval = 15;

    [ToggleOption]
    private static bool VolatileKnows = true;

    private static float HallucinationTime;

    private bool Exposed;

    private static bool Lmao;

    protected override UColor MainColor => CustomColorManager.Volatile;
    public override Layer Type => Layer.Volatile;
    public override string Description => "- You experience hallucinations";
    public override bool Hidden => !VolatileKnows && !Exposed && !Dead;

    private static readonly string[] Links = [ "https://www.youtube.com/watch?v=79-AwFZCKpA", "https://www.youtube.com/watch?v=xm3YgoEiEDc" ];

    public override void Init() => Exposed = VolatileKnows;

    public override void UpdateHud(HudManager __instance)
    {
        if (ActiveTask() || HUD().IsIntroDisplayed || Dead)
            return;

        HallucinationTime += Time.deltaTime;

        if (HallucinationTime < VolatileInterval)
            return;

        var randomNumber = URandom.RandomRangeInt(0, 5);
        HallucinationTime -= VolatileInterval;
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