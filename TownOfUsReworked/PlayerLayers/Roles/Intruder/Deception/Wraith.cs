namespace TownOfUsReworked.PlayerLayers.Roles;

public class Wraith : Intruder
{
    public CustomButton InvisButton { get; set; }
    public bool Enabled { get; set; }
    public DateTime LastInvis { get; set; }
    public float TimeRemaining { get; set; }
    public bool IsInvis => TimeRemaining > 0f;

    public override Color Color => ClientGameOptions.CustomIntColors ? Colors.Wraith : Colors.Intruder;
    public override string Name => "Wraith";
    public override LayerEnum Type => LayerEnum.Wraith;
    public override Func<string> StartText => () => "Sneaky Sneaky";
    public override Func<string> Description => () => $"- You can turn invisible\n{CommonAbilities}";
    public override InspectorResults InspectorResults => InspectorResults.Unseen;
    public float Timer => ButtonUtils.Timer(Player, LastInvis, CustomGameOptions.InvisCd);

    public Wraith(PlayerControl player) : base(player)
    {
        Alignment = Alignment.IntruderDecep;
        InvisButton = new(this, "Invis", AbilityTypes.Effect, "Secondary", HitInvis);
    }

    public void Invis()
    {
        Enabled = true;
        TimeRemaining -= Time.deltaTime;
        Utils.Invis(Player, CustomPlayer.Local.Is(Faction.Intruder));

        if (IsDead || Meeting)
            TimeRemaining = 0f;
    }

    public void Uninvis()
    {
        Enabled = false;
        LastInvis = DateTime.UtcNow;
        DefaultOutfit(Player);
    }

    public void HitInvis()
    {
        if (Timer != 0f || IsInvis)
            return;

        CallRpc(CustomRPC.Action, ActionsRPC.Invis, this);
        TimeRemaining = CustomGameOptions.InvisDur;
    }

    public override void UpdateHud(HudManager __instance)
    {
        base.UpdateHud(__instance);
        InvisButton.Update("INVIS", Timer, CustomGameOptions.InvisCd, IsInvis, TimeRemaining, CustomGameOptions.InvisDur);
    }
}