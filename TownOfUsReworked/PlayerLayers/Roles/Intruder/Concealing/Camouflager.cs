namespace TownOfUsReworked.PlayerLayers.Roles;

public class Camouflager : Intruder
{
    public CustomButton CamouflageButton { get; set; }

    public override UColor Color => ClientGameOptions.CustomIntColors ? CustomColorManager.Camouflager : CustomColorManager.Intruder;
    public override string Name => "Camouflager";
    public override LayerEnum Type => LayerEnum.Camouflager;
    public override Func<string> StartText => () => "Hinder The <color=#8CFFFFFF>Crew</color>'s Recognition";
    public override Func<string> Description => () => "- You can disrupt everyone's vision, causing them to be unable to tell players apart\n- When camouflaged, everyone will appear grey " +
        $"with fluctuating names and no cosmetics\n{CommonAbilities}";

    public Camouflager() : base() {}

    public override PlayerLayer Start(PlayerControl player)
    {
        SetPlayer(player);
        BaseStart();
        Alignment = Alignment.IntruderConceal;
        CamouflageButton = new(this, "Camouflage", AbilityTypes.Targetless, "Secondary", HitCamouflage, CustomGameOptions.CamouflagerCd, CustomGameOptions.CamouflageDur,
            (CustomButton.EffectVoid)Camouflage, UnCamouflage);
        Data.Role.IntroSound = GetAudio("CamouflagerIntro");
        return this;
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
        CallRpc(CustomRPC.Action, ActionsRPC.LayerAction1, CamouflageButton);
        CamouflageButton.Begin();
    }

    public override void UpdateHud(HudManager __instance)
    {
        base.UpdateHud(__instance);
        CamouflageButton.Update2("CAMOUFLAGE", condition: !HudHandler.Instance.IsCamoed);
    }
}