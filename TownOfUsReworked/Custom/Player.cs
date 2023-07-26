namespace TownOfUsReworked.Custom
{
    public class CustomPlayer
    {
        public readonly PlayerControl Player;
        public byte PlayerId => Player.PlayerId;
        public GameData.PlayerInfo Data => Player.Data;
        public Transform Transform => Player.transform;
        public Vector3 Position => Transform.position;
        public bool IsDead => Data.IsDead;
        public bool Disconnected => Data.Disconnected;
        public float SpeedFactor => IsDead && (!Player.IsPostmortal() || (Player.IsPostmortal() && Player.Caught())) ? CustomGameOptions.GhostSpeed : (CustomGameOptions.PlayerSpeed *
            Player.GetModifiedSpeed());
        public Vector3 SizeFactor => DefaultSize * Player.GetModifiedSize();

        public static PlayerControl Local => PlayerControl.LocalPlayer;
        public static CustomPlayer LocalCustom => Custom(Local);
        public static List<PlayerControl> AllPlayers => PlayerControl.AllPlayerControls.Il2CppToSystem();
        public static readonly List<CustomPlayer> AllCustomPlayers = new();
        private static Vector3 DefaultSize => new(0.7f, 0.7f, 1f);

        public CustomPlayer(PlayerControl player)
        {
            Player = player;
            AllCustomPlayers.Add(this);
        }

        public static implicit operator bool(CustomPlayer exists) => exists != null && exists.Player;

        public static CustomPlayer Custom(PlayerControl player) => AllCustomPlayers.Find(x => x.Player == player) ?? new(player);

        public override string ToString() => Player.ToString();
    }
}