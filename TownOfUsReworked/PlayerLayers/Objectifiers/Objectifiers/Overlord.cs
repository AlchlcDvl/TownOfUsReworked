namespace TownOfUsReworked.PlayerLayers.Objectifiers;

public class Overlord : Objectifier
{
    public override UColor Color => ClientGameOptions.CustomObjColors ? CustomColorManager.Overlord : CustomColorManager.Objectifier;
    public override string Name => "Overlord";
    public override string Symbol => "β";
    public override LayerEnum Type => LayerEnum.Overlord;
    public override Func<string> Description => () => $"- Stay alive for {CustomGameOptions.OverlordMeetingWinCount} rounds";
    public override bool Hidden => !CustomGameOptions.OverlordKnows && !Dead;
}