namespace TownOfUsReworked.PlayerLayers.Roles;

[LayerHeaderOption(LayerEnum.Camouflager)]
public sealed class Camouflager : Intruder
{
    [NumberOption(10f, 60f, 2.5f, Format.Time)]
    public static Number CamouflageCd = 25;

    [NumberOption(5f, 30f, 1f, Format.Time)]
    public static Number CamouflageDur = 10;

    private CustomButton CamouflageButton { get; set; }

    protected override UColor MainColor => CustomColorManager.Camouflager;
    public override LayerEnum Type => LayerEnum.Camouflager;
    public override Func<string> StartText { get; } = () => "Hinder The <#8CFFFFFF>Crew</color>'s Recognition";
    public override Func<string> Description => () => "- You can disrupt everyone's vision, causing them to be unable to tell players apart\n- When camouflaged, everyone will appear grey " +
        $"with fluctuating names and no cosmetics\n{CommonAbilities}";

    protected override void Init()
    {
        base.Init();
        Alignment = Alignment.Concealing;
        CamouflageButton ??= new(this, new SpriteName("Camouflage"), AbilityTypes.Targetless, KeybindType.Secondary, (OnClickTargetless)HitCamouflage, (ConditionFunc)Condition, "CAMOUFLAGE",
            new Cooldown(CamouflageCd), new Duration(CamouflageDur), (EffectVoid)Camouflage, (EffectEndVoid)UnCamouflage);
    }

    public static void Camouflage()
    {
        Hud.Instance.CamouflagerEnabled = true;
        MiscUtils.Camouflage();
    }

    public static void UnCamouflage() => Hud.Instance.CamouflagerEnabled = false;

    private void HitCamouflage()
    {
        CallRpc(CustomRPC.Action, ActionsRPC.ButtonAction, CamouflageButton);
        CamouflageButton.Begin();
    }

    public static bool Condition() => !Hud.Instance.IsCamoed;
}