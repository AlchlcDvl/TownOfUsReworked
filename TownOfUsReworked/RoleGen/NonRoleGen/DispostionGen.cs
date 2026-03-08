using static TownOfUsReworked.Managers.RoleGenManager;

namespace TownOfUsReworked.RoleGen;

public sealed class DispositionGen : BaseNonRoleLayerGen
{
    private static readonly Layer[] LoverRival = [Layer.Lovers, Layer.Rivals];
    private static readonly Layer[] CrewDisp = [Layer.Corrupted, Layer.Fanatic, Layer.Traitor];
    private static readonly Layer[] OutcastDisp = [Layer.Taskmaster, Layer.Overlord, Layer.Linked];

    protected override PlayerLayerEnum LayerType => PlayerLayerEnum.Disposition;
    protected override Layer MinLayer => Layer.Allied;
    protected override Layer MaxLayer => Layer.Traitor;
    protected override List<RoleOptionData> TargetList => AllDispositions;
    protected override int MinSetting => DispositionsSettings.MinDispositions.Value;
    protected override int MaxSetting => DispositionsSettings.MaxDispositions.Value;

    protected override bool HasLayer(PlayerControl player) => player.GetDisposition();

    protected override PlayerControl? GetAssignee(Layer id, List<PlayerControl> playerList) => id switch
    {
        Layer.Mafia when playerList.Count > 1 => playerList.FirstOrDefault(),
        Layer.Defector => playerList.FirstOrDefault(x => x.GetFaction().IsFactionedEvil()),
        Layer.Allied => playerList.FirstOrDefault(x => x.Is(Alignment.Killing) && x.GetFaction().IsOutcast()),
        _ when LoverRival.Contains(id) && playerList.Count > 1 => playerList.FirstOrDefault(x => x.GetRole() is not (Troll or Actor or Jester)),
        _ when CrewDisp.Contains(id) => playerList.FirstOrDefault(x => x.Is(Faction.Crew)),
        _ when OutcastDisp.Contains(id) => playerList.FirstOrDefault(x => x.GetFaction().IsOutcast()),
        _ => null
    };

    protected override int GetPlayerCountCap() => AllPlayers().Count(x => x != Pure);
}