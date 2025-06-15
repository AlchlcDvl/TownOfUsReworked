namespace TownOfUsReworked.PlayerLayers.Roles;

[LayerHeaderOption(Layer.Camouflager)]
public sealed class Camouflager : Concealing
{
    [NumberOption(10f, 60f, 2.5f, Format.Time)]
    private static Number CamouflageCd = 25;

    [NumberOption(5f, 30f, 1f, Format.Time)]
    private static Number CamouflageDur = 10;

    private CustomButton CamouflageButton;
    private bool ClickedAgain;

    protected override UColor MainColor => CustomColorManager.Camouflager;
    public override Layer Type => Layer.Camouflager;
    public override string StartText => "Hinder The <#8CFFFFFF>Crew</color>'s Recognition";
    public override string Description => "- You can disrupt everyone's vision, causing them to be unable to tell players apart\n- When camouflaged, everyone will appear grey " +
        $"with fluctuating names and no cosmetics\n{CommonAbilities}";

    public override void Init()
    {
        base.Init();
        CamouflageButton ??= new(this, new SpriteName("Camouflage"), AbilityTypes.Targetless, KeybindType.Secondary, (OnClickTargetless)HitCamouflage, (ConditionFunc)Condition, "CAMOUFLAGE",
            new Cooldown(CamouflageCd), new Duration(CamouflageDur), (EffectStartVoid)StartCamouflage, (EffectEndVoid)UnCamouflage, (ClickedAgainVoid)ClickAgain);
    }

    private void StartCamouflage()
    {
        Hud.Instance.CamouflagerEnabled = true;
        Camouflage(Condition, CamouflageDur);
    }

    private static void UnCamouflage() => Hud.Instance.CamouflagerEnabled = false;

    private void HitCamouflage() => CamouflageButton.TriggerRpcAndBegin();

    private void ClickAgain() => ClickedAgain = true;

    private bool Condition() => !Hud.Instance.IsCamoed || ClickedAgain;
}