using static TownOfUsReworked.Managers.RoleGenManager;

namespace TownOfUsReworked.RoleGen;

public sealed class ModifierGen : BaseNonRoleLayerGen
{
    private static readonly Layer[] GlobalMod = [Layer.Dwarf, Layer.Vip, Layer.Giant, Layer.Drunk, Layer.Coward, Layer.Volatile, Layer.Astral, Layer.Indomitable, Layer.Yeller, Layer.Colorblind];

    protected override PlayerLayerEnum LayerType => PlayerLayerEnum.Modifier;
    protected override Layer MinLayer => Layer.Astral;
    protected override Layer MaxLayer => Layer.Yeller;
    protected override List<RoleOptionData> TargetList => AllModifiers;
    protected override int MinSetting => ModifiersSettings.MinModifiers.Value;
    protected override int MaxSetting => ModifiersSettings.MaxModifiers.Value;

    protected override bool HasLayer(PlayerControl player) => player.GetModifier();

    protected override PlayerControl? GetAssignee(Layer id, List<PlayerControl> playerList) => id switch
    {
        Layer.Bait => playerList.FirstOrDefault(x => x.GetRole() is not (Thief or Troll)),
        Layer.Diseased => playerList.FirstOrDefault(x => x.GetRole() is Troll),
        Layer.Shy => playerList.FirstOrDefault(x => !((x.Is<Democrat>() && !Democrat.DemocratButton) || (x.Is<Jester>() && !Jester.JesterButton) || (x.Is<Swapper>() &&
            !Swapper.SwapperButton) || (x.Is<Actor>() && !Actor.ActorButton) || (x.Is<Guesser>() && !Guesser.GuesserButton) || (x.Is<Executioner>() && !Executioner.ExecutionerButton) ||
            (x.Is<Politician>() && !Politician.PoliticianButton) || x.Is<ButtonBarry>() || (!Dictator.DictatorButton && x.Is<Dictator>()) || (!Monarch.MonarchButton && x.Is<Monarch>()) ||
            (x.Is<Mayor>() && !Mayor.MayorButton))),
        _ => GlobalMod.Contains(id) ? playerList.FirstOrDefault() : null
    };
}