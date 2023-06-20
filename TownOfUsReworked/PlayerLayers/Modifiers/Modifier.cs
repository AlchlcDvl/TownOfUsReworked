namespace TownOfUsReworked.PlayerLayers.Modifiers
{
    public class Modifier : PlayerLayer
    {
        public readonly static List<Modifier> AllModifiers = new();
        public static Modifier LocalModifier => GetModifier(CustomPlayer.Local);

        public Modifier(PlayerControl player) : base(player)
        {
            if (GetModifier(player))
                GetModifier(player).Player = null;

            Color = Colors.Modifier;
            LayerType = PlayerLayerEnum.Modifier;
            AllModifiers.Add(this);
        }

        public Func<string> TaskText = () => "- None";
        public bool Hidden;

        public static Modifier GetModifier(PlayerControl player) => AllModifiers.Find(x => x.Player == player);

        public static T GetModifier<T>(PlayerControl player) where T : Modifier => GetModifier(player) as T;

        public static Modifier GetModifier(PlayerVoteArea area) => GetModifier(Utils.PlayerByVoteArea(area));

        public static List<Modifier> GetModifiers(ModifierEnum modifierType) => AllModifiers.Where(x => x.ModifierType == modifierType).ToList();

        public static List<T> GetModifiers<T>(ModifierEnum modifierType) where T : Modifier => GetModifiers(modifierType).Cast<T>().ToList();
    }
}