namespace TownOfUsReworked.PlayerLayers.Modifiers
{
    public class Modifier : PlayerLayer
    {
        public static readonly List<Modifier> AllModifiers = new();
        public static Modifier LocalModifier => GetModifier(CustomPlayer.Local);

        public override Color32 Color => Colors.Modifier;
        public override PlayerLayerEnum LayerType => PlayerLayerEnum.Modifier;

        public virtual ModifierEnum ModifierType => ModifierEnum.None;
        public virtual Func<string> TaskText => () => "- None";
        public virtual bool Hidden => false;

        public Modifier(PlayerControl player) : base(player)
        {
            if (GetModifier(player))
                GetModifier(player).Player = null;

            AllModifiers.Add(this);
        }

        public static Modifier GetModifier(PlayerControl player) => AllModifiers.Find(x => x.Player == player);

        public static T GetModifier<T>(PlayerControl player) where T : Modifier => GetModifier(player) as T;

        public static Modifier GetModifier(PlayerVoteArea area) => GetModifier(PlayerByVoteArea(area));

        public static List<Modifier> GetModifiers(ModifierEnum modifierType) => AllModifiers.Where(x => x.ModifierType == modifierType).ToList();

        public static List<T> GetModifiers<T>(ModifierEnum modifierType) where T : Modifier => GetModifiers(modifierType).Cast<T>().ToList();
    }
}