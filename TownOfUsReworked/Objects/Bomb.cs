namespace TownOfUsReworked.Objects;

public class Bomb(PlayerControl bomb, bool drived) : Range(bomb, CustomColorManager.Bomber, Bomber.BombRange + (drived ? Bomber.ChaosDriveBombRange : 0f), "Bomb")
{
    public bool Drived { get; set; } = drived;

    public void Detonate()
    {
        var players = GetClosestPlayers(Transform.position, Size);

        foreach (var player in players)
        {
            if (CanAttack(AttackEnum.Powerful, player.GetDefenseValue()))
                RpcMurderPlayer(Owner, player, DeathReasonEnum.Bombed, false);
        }

        Destroy();
    }
}