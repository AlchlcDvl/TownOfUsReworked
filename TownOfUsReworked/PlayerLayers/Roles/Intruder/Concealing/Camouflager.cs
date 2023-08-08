namespace TownOfUsReworked.PlayerLayers.Roles;

public class Camouflager : Intruder
{
    public CustomButton CamouflageButton { get; set; }
    public bool Enabled { get; set; }
    public DateTime LastCamouflaged { get; set; }
    public float TimeRemaining { get; set; }
    public bool Camouflaged => TimeRemaining > 0f;

    public override Color32 Color => ClientGameOptions.CustomIntColors ? Colors.Camouflager : Colors.Intruder;
    public override string Name => "Camouflager";
    public override LayerEnum Type => LayerEnum.Camouflager;
    public override Func<string> StartText => () => "Hinder The <color=#8CFFFFFF>Crew</color>'s Recognition";
    public override Func<string> Description => () => "- You can disrupt everyone's vision, causing them to be unable to tell players apart\n- When camouflaged, everyone will appear " +
        $"grey with fluctuating names and no cosmetics\n{CommonAbilities}";
    public override InspectorResults InspectorResults => InspectorResults.BringsChaos;
    public float Timer => ButtonUtils.Timer(Player, LastCamouflaged, CustomGameOptions.CamouflagerCd);

    public Camouflager(PlayerControl player) : base(player)
    {
        RoleAlignment = RoleAlignment.IntruderConceal;
        CamouflageButton = new(this, "Camouflage", AbilityTypes.Effect, "Secondary", HitCamouflage);
    }

    public void Camouflage()
    {
        Enabled = true;
        TimeRemaining -= Time.deltaTime;
        Utils.Camouflage();

        if (Meeting)
            TimeRemaining = 0f;
    }

    public void UnCamouflage()
    {
        Enabled = false;
        LastCamouflaged = DateTime.UtcNow;
        DefaultOutfitAll();
    }

    public void HitCamouflage()
    {
        if (Timer != 0f || DoUndo.IsCamoed)
            return;

        CallRpc(CustomRPC.Action, ActionsRPC.Camouflage, this);
        TimeRemaining = CustomGameOptions.CamouflagerDuration;
        Camouflage();
    }

    public override void UpdateHud(HudManager __instance)
    {
        base.UpdateHud(__instance);
        CamouflageButton.Update("CAMOUFLAGE", Timer, CustomGameOptions.CamouflagerCd, Camouflaged, TimeRemaining, CustomGameOptions.CamouflagerDuration, !DoUndo.IsCamoed);
    }
}