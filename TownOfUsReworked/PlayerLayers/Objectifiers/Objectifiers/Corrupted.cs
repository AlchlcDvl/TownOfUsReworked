namespace TownOfUsReworked.PlayerLayers.Objectifiers;

public class Corrupted : Objectifier
{
    public DateTime LastCorrupted { get; set; }
    public CustomButton CorruptButton { get; set; }
    public float Timer => ButtonUtils.Timer(Player, LastCorrupted, CustomGameOptions.CorruptCd);

    public override Color32 Color => ClientGameOptions.CustomObjColors ? Colors.Corrupted : Colors.Objectifier;
    public override string Name => "Corrupted";
    public override string Symbol => "δ";
    public override LayerEnum Type => LayerEnum.Corrupted;
    public override Func<string> Description => () => "- Corrupt everyone";

    public Corrupted(PlayerControl player) : base(player)
    {
        CorruptButton = new(this, "Corrupt", AbilityTypes.Direct, "Quarternary", Corrupt);
        Role.GetRole(Player).RoleAlignment = Role.GetRole(Player).RoleAlignment.GetNewAlignment(Faction.Neutral);
    }

    public void Corrupt()
    {
        if (Timer != 0f || IsTooFar(Player, CorruptButton.TargetPlayer))
            return;

        var interact = Interact(Player, CorruptButton.TargetPlayer, true);

        if (interact[3] || interact[0])
            LastCorrupted = DateTime.UtcNow;
        else if (interact[1])
            LastCorrupted.AddSeconds(CustomGameOptions.ProtectKCReset);
        else if (interact[2])
            LastCorrupted.AddSeconds(CustomGameOptions.VestKCReset);
    }

    public override void UpdateHud(HudManager __instance)
    {
        base.UpdateHud(__instance);
        CorruptButton.Update("CORRUPT", Timer, CustomGameOptions.CorruptCd);
    }
}