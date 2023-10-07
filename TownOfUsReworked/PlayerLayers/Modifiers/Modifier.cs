namespace TownOfUsReworked.PlayerLayers.Modifiers;

public abstract class Modifier : PlayerLayer
{
    public static readonly List<Modifier> AllModifiers = new();
    public static Modifier LocalModifier => GetModifier(CustomPlayer.Local);

    public override Color Color => Colors.Modifier;
    public override PlayerLayerEnum LayerType => PlayerLayerEnum.Modifier;

    public virtual bool Hidden => false;

    protected Modifier(PlayerControl player) : base(player)
    {
        if (GetModifier(player))
            GetModifier(player).Player = null;

        AllModifiers.Add(this);
    }

    public static Modifier GetModifier(PlayerControl player) => AllModifiers.Find(x => x.Player == player);

    public static T GetModifier<T>(PlayerControl player) where T : Modifier => GetModifier(player) as T;

    public static Modifier GetModifier(PlayerVoteArea area) => GetModifier(PlayerByVoteArea(area));

    public static List<Modifier> GetModifiers(LayerEnum modifierType) => AllModifiers.Where(x => x.Type == modifierType).ToList();

    public static List<T> GetModifiers<T>(LayerEnum modifierType) where T : Modifier => GetModifiers(modifierType).Cast<T>().ToList();
}