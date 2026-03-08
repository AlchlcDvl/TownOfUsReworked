namespace TownOfUsReworked.Monos;

public sealed class Bomb : Range
{
    public void Detonate()
    {
        foreach (var player in GetClosestPlayers(transform.position, Size))
        {
            if (CanAttack(Attack.Powerful, player.GetDefenseValue()))
                Owner!.RpcMurderPlayer(player, DeathReasonEnum.Bombed, false);
        }

        gameObject.Destroy();
    }

    public static Bomb CreateBomb(PlayerControl owner, bool drived)
    {
        var range = Bomber.BombRange + (drived ? Bomber.ChaosDriveBombRange : 0f);
        var gameObject = CreateRange(CustomColorManager.Bomber, range, "Bomb", owner.GetTruePosition());
        var bomb = gameObject.AddComponent<Bomb>();
        bomb.Owner = owner;
        bomb.Size = range;
        return bomb;
    }
}