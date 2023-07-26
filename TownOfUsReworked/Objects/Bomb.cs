namespace TownOfUsReworked.Objects
{
    public class Bomb : Range
    {
        public List<PlayerControl> Players;
        public bool Drived;
        public PlayerControl Bomber;

        public Bomb(Vector2 position, bool drived, PlayerControl bomb) : base(position, Colors.Bomber, CustomGameOptions.BombRange + (drived ? CustomGameOptions.ChaosDriveBombRange : 0f),
            "Bomb")
        {
            Drived = drived;
            Bomber = bomb;
            Coroutines.Start(Timer());
        }

        public override IEnumerator Timer()
        {
            while (Transform)
            {
                yield return 0;
                Update();
            }
        }

        public override void Update()
        {
            base.Update();
            Players = GetClosestPlayers(Transform.position, CustomGameOptions.BombRange + (Drived ? CustomGameOptions.ChaosDriveBombRange : 0f));
        }

        public void Detonate()
        {
            foreach (var player in Players)
            {
                if (player.Is(RoleEnum.Pestilence) || player.IsOnAlert() || player.IsProtected() || player.IsShielded() || player.IsRetShielded() || player.IsProtectedMonarch() ||
                    (player.Is(Faction.Syndicate) && !CustomGameOptions.BombKillsSyndicate) || Bomber.IsLinkedTo(player))
                {
                    continue;
                }

                RpcMurderPlayer(Bomber, player, DeathReasonEnum.Bombed, false);
            }

            Destroy();
        }

        public static void DetonateBombs(List<Bomb> obj)
        {
            if (obj.Any(x => x.Drived))
            {
                foreach (var t in obj)
                {
                    t.Detonate();
                    obj.Remove(t);
                }

                Clear(obj);
            }
            else
            {
                var bomb = obj[^1];
                bomb.Detonate();
                obj.Remove(bomb);
            }
        }

        public static void Clear(List<Bomb> obj)
        {
            foreach (var t in obj)
                t.Destroy();

            obj.Clear();
        }
    }
}