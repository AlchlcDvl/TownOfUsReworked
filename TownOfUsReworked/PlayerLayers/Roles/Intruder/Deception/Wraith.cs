namespace TownOfUsReworked.PlayerLayers.Roles;

public class Wraith : Intruder
{
    public CustomButton InvisButton { get; set; }

    public override Color Color => ClientGameOptions.CustomIntColors ? Colors.Wraith : Colors.Intruder;
    public override string Name => "Wraith";
    public override LayerEnum Type => LayerEnum.Wraith;
    public override Func<string> StartText => () => "Sneaky Sneaky";
    public override Func<string> Description => () => $"- You can turn invisible\n{CommonAbilities}";

    public Wraith(PlayerControl player) : base(player)
    {
        Alignment = Alignment.IntruderDecep;
        InvisButton = new(this, "Invis", AbilityTypes.Targetless, "Secondary", HitInvis, CustomGameOptions.InvisCd, CustomGameOptions.InvisDur, (CustomButton.EffectVoid)Invis, UnInvis);
    }

    public void Invis() => Utils.Invis(Player, CustomPlayer.Local.Is(Faction.Intruder));

    public void UnInvis() => DefaultOutfit(Player);

    public void HitInvis()
    {
        CallRpc(CustomRPC.Action, ActionsRPC.LayerAction1, InvisButton);
        InvisButton.Begin();
    }

    public override void UpdateHud(HudManager __instance)
    {
        base.UpdateHud(__instance);
        InvisButton.Update2("INVISIBILITY");
    }

    public override void TryEndEffect() => InvisButton.Update3(IsDead);
}