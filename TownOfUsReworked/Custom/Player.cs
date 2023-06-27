namespace TownOfUsReworked.Custom
{
    public class CustomPlayer
    {
        public PlayerControl Player;
        public byte PlayerId => Player.PlayerId;
        public GameData.PlayerInfo Data => Player.Data;
        public bool IsDead => Data.IsDead;
        public bool Disconnected => Data.Disconnected;
        public float SpeedFactor => IsDead && (!Player.IsPostmortal() || (Player.IsPostmortal() && Player.Caught())) ? CustomGameOptions.GhostSpeed : (CustomGameOptions.PlayerSpeed *
            Player.GetModifiedSpeed());
        public Vector3 SizeFactor => new Vector3(0.7f, 0.7f, 1f) * Player.GetModifiedSize();

        public static PlayerControl Local => PlayerControl.LocalPlayer;
        public static CustomPlayer LocalCustom => Custom(Local);
        public static List<PlayerControl> AllPlayers => PlayerControl.AllPlayerControls.Il2CppToSystem();
        public readonly static List<CustomPlayer> AllCustomPlayers = new();

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