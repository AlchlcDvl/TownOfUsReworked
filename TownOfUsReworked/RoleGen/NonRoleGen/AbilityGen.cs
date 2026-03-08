using static TownOfUsReworked.Managers.RoleGenManager;

namespace TownOfUsReworked.RoleGen;

public sealed class AbilityGen : BaseNonRoleLayerGen
{
    private static readonly Layer[] CrewAb = [Layer.Bullseye, Layer.Swapper];
    private static readonly Layer[] Tasked = [Layer.Insider, Layer.Multitasker];
    private static readonly Layer[] GlobalAb = [Layer.Radar, Layer.Tiebreaker];

    protected override PlayerLayerEnum LayerType => PlayerLayerEnum.Ability;
    protected override Layer MinLayer => Layer.Bullseye;
    protected override Layer MaxLayer => Layer.Underdog;
    protected override List<RoleOptionData> TargetList => AllAbilities;
    protected override int MinSetting => AbilitiesSettings.MinAbilities.Value;
    protected override int MaxSetting => AbilitiesSettings.MaxAbilities.Value;
    protected override string PluralName => "Abilities";

    protected override bool HasLayer(PlayerControl player) => player.GetAbility();

    protected override PlayerControl? GetAssignee(Layer id, List<PlayerControl> playerList) => id switch
    {
        Layer.Snitch => playerList.FirstOrDefault(x => x.Is(Faction.Crew) && !x.Is<Traitor>() && !x.Is<Fanatic>()),
        Layer.Sniper => playerList.FirstOrDefault(x => x.Is(Faction.Syndicate)),
        Layer.Ritualist => playerList.FirstOrDefault(x => x.Is(Faction.Apocalypse)),
        Layer.Slayer => playerList.FirstOrDefault(x => x.GetFaction().IsFactionedEvil()),
        Layer.Hitman => playerList.FirstOrDefault(x => x.Is(Faction.Intruder) && (!x.Is<Consigliere>() || Consigliere.ConsigInfo != ConsigInfo.Role)),
        Layer.Ninja => playerList.FirstOrDefault(x => x.GetFaction().IsFactionedEvil() || x.Is<Corrupted>()),
        Layer.Torch => playerList.FirstOrDefault(x => x.GetRole().AffectedByLights),
        Layer.Underdog => playerList.FirstOrDefault(x => x.GetFaction().IsFactionedEvil(true)),
        Layer.Tunneler => playerList.FirstOrDefault(x => x.Is(Faction.Crew)),
        Layer.ButtonBarry => playerList.FirstOrDefault(x => !((x.Is<Democrat>() && (!Mayor.MayorButton || !Democrat.DemocratButton)) || (x.Is<Jester>() && !Jester.JesterButton) ||
            (x.Is<Actor>() && !Actor.ActorButton) || (x.Is<Guesser>() && !Guesser.GuesserButton) || (x.Is<Executioner>() && !Executioner.ExecutionerButton) || (!Monarch.MonarchButton &&
            x.Is<Monarch>()) || (x.Is<Dictator>() && !Dictator.DictatorButton) || (x.Is<Mayor>() && !Mayor.MayorButton))),
        Layer.Politician => playerList.FirstOrDefault(x => !(x.Is(Alignment.Evil) || x.Is(Alignment.Benign) || x.Is(Alignment.Neophyte))),
        Layer.Ruthless => playerList.FirstOrDefault(x => (x.GetFaction().IsFactionedEvil() && !x.Is<Juggernaut>()) || x.Is<Corrupted>() || x.Is<CKilling>()),
        _ when GlobalAb.Contains(id) => playerList.FirstOrDefault(),
        _ when Tasked.Contains(id) => playerList.FirstOrDefault(x => x.CanDoTasks()),
        _ when CrewAb.Contains(id) => playerList.FirstOrDefault(x => x.Is(Faction.Crew)),
        _ => null
    };
}