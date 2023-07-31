namespace TownOfUsReworked.Objects
{
    public class Bomb : Range
    {
        public List<PlayerControl> Players = new();
        public bool Drived;

        public Bomb(PlayerControl bomb, bool drived) : base(bomb, Colors.Bomber, CustomGameOptions.BombRange + (drived ? CustomGameOptions.ChaosDriveBombRange : 0f), "Bomb")
        {
            Drived = drived;
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
            Players = GetClosestPlayers(Transform.position, Size);
        }

        public void Detonate()
        {
            foreach (var player in Players)
            {
                if (player.Is(RoleEnum.Pestilence) || player.IsOnAlert() || player.IsProtected() || player.IsShielded() || player.IsRetShielded() || player.IsProtectedMonarch() ||
                    (player.Is(Faction.Syndicate) && !CustomGameOptions.BombKillsSyndicate) || Owner.IsLinkedTo(player))
                {
                    continue;
                }

                RpcMurderPlayer(Owner, player, DeathReasonEnum.Bombed, false);
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
            obj.ForEach(b => b.Destroy());
            obj.Clear();
        }
    }
}