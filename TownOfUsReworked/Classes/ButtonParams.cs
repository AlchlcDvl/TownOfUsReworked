namespace TownOfUsReworked.Classes;

public static class ButtonParams
{
    public delegate bool PlayerBodyExclusion(PlayerControl player);
    public delegate bool VentExclusion(Vent vent);
    public delegate bool ConsoleExclusion(Console console);
    public delegate bool EndFunc();
    public delegate bool UsableFunc();
    public delegate bool ConditionFunc();
    public delegate void EffectVoid();
    public delegate void EffectStartVoid();
    public delegate void EffectEndVoid();
    public delegate void DelayVoid();
    public delegate void DelayStartVoid();
    public delegate void DelayEndVoid();
    public delegate void OnClick();
    public delegate float DifferenceFunc();
    public delegate float MultiplierFunc();
    public delegate string LabelFunc();
}