namespace TownOfUsReworked.PlayerLayers.Roles;

[HeaderOption(MultiMenu2.LayerSubOptions)]
public class Camouflager : Intruder
{
    public CustomButton CamouflageButton { get; set; }

    public override UColor Color => ClientOptions.CustomIntColors ? CustomColorManager.Camouflager : CustomColorManager.Intruder;
    public override string Name => "Camouflager";
    public override LayerEnum Type => LayerEnum.Camouflager;
    public override Func<string> StartText => () => "Hinder The <color=#8CFFFFFF>Crew</color>'s Recognition";
    public override Func<string> Description => () => "- You can disrupt everyone's vision, causing them to be unable to tell players apart\n- When camouflaged, everyone will appear grey " +
        $"with fluctuating names and no cosmetics\n{CommonAbilities}";

    public override void Init()
    {
        BaseStart();
        Alignment = Alignment.IntruderConceal;
        CamouflageButton = CreateButton(this, new SpriteName("Camouflage"), AbilityTypes.Targetless, KeybindType.Secondary, (OnClick)HitCamouflage, (ConditionFunc)Condition, "CAMOUFLAGE",
            new Cooldown(CustomGameOptions.CamouflagerCd), new Duration(CustomGameOptions.CamouflageDur), (EffectVoid)Camouflage, (EffectEndVoid)UnCamouflage);
        Data.Role.IntroSound = GetAudio("CamouflagerIntro");
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