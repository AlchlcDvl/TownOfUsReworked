namespace TownOfUsReworked.PlayerLayers.Roles;

public abstract class NKilling : Neutral
{
    public override void Init()
    {
        base.Init();
        Alignment = Alignment.Killing;
    }

    // public override void UpdatePlayerName(LayerHandler playerHandler, bool deadSeeEverything, ref string name, ref UColor color, ref bool revealed, ref bool removeFromConsig)
    // {
    //     if (!revealed && playerHandler.CustomRole is NKilling && !deadSeeEverything && NeutralKillingSettings.KnowEachOther && ((playerHandler.CustomRole.Type == Type && NeutralSettings.NoSolo
    //         == NoSolo.SameNKs) || NeutralSettings.NoSolo == NoSolo.AllNKs))
    //     {
    //         color = playerHandler.CustomRole.Color;
    //         name += $"\n{playerHandler.CustomRole}";
    //         revealed = true;
    //     }
    // }
}