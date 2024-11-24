namespace TownOfUsReworked.PlayerLayers.Roles;

[HeaderOption(MultiMenu.LayerSubOptions)]
public class Camouflager : Intruder
{
    [NumberOption(MultiMenu.LayerSubOptions, 10f, 60f, 2.5f, Format.Time)]
    public static Number CamouflageCd { get; set; } = new(25);

    [NumberOption(MultiMenu.LayerSubOptions, 5f, 30f, 1f, Format.Time)]
    public static Number CamouflageDur { get; set; } = new(10);

    public CustomButton CamouflageButton { get; set; }

    public override UColor Color => ClientOptions.CustomIntColors ? CustomColorManager.Camouflager : CustomColorManager.Intruder;
    public override string Name => "Camouflager";
    public override LayerEnum Type => LayerEnum.Camouflager;
    public override Func<string> StartText => () => "Hinder The <color=#8CFFFFFF>Crew</color>'s Recognition";
    public override Func<string> Description => () => "- You can disrupt everyone's vision, causing them to be unable to tell players apart\n- When camouflaged, everyone will appear grey " +
        $"with fluctuating names and no cosmetics\n{CommonAbilities}";

    public override void Init()
    {
        base.Init();
        Alignment = Alignment.IntruderConceal;
        CamouflageButton ??= new(this, new SpriteName("Camouflage"), AbilityTypes.Targetless, KeybindType.Secondary, (OnClickTargetless)HitCamouflage, (ConditionFunc)Condition, "CAMOUFLAGE",
            new Cooldown(CamouflageCd), new Duration(CamouflageDur), (EffectVoid)Camouflage, (EffectEndVoid)UnCamouflage);
    }

    public void Camouflage()
    {
        HudHandler.Instance.CamouflagerEnabled = true;
        Utils.Camouflage();
    }

    public void UnCamouflage()
    {
        HudHandler.Instance.CamouflagerEnabled = false;
        DefaultOutfitAll();
    }

    public void HitCamouflage()
    {
        CallRpc(CustomRPC.Action, ActionsRPC.ButtonAction, CamouflageButton);
        CamouflageButton.Begin();
    }

    public bool Condition() => !HudHandler.Instance.IsCamoed;
}