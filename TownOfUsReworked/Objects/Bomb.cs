namespace TownOfUsReworked.Objects;

public class Bomb : Range
{
    public List<PlayerControl> Players { get; set; }
    public bool Drived { get; set; }

    public Bomb(PlayerControl bomb, bool drived) : base(bomb, CustomColorManager.Bomber, CustomGameOptions.BombRange + (drived ? CustomGameOptions.ChaosDriveBombRange : 0f), "Bomb")
    {
        Drived = drived;
        Players = new();
    }

    public void Detonate()
    {
        Players = GetClosestPlayers(Transform.position, Size);

        foreach (var player in Players)
        {
            if (CanAttack(AttackEnum.Powerful, player.GetDefenseValue()))
                RpcMurderPlayer(Owner, player, DeathReasonEnum.Bombed, false);
        }

        Destroy();
    }
}