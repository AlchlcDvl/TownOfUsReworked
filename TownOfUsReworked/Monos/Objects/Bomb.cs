﻿namespace TownOfUsReworked.Monos;

public class Bomb : Range
{
    public bool Drived { get; set; }

    public void Detonate()
    {
        var players = GetClosestPlayers(transform.position, Size);

        foreach (var player in players)
        {
            if (CanAttack(AttackEnum.Powerful, player.GetDefenseValue()))
                Owner.RpcMurderPlayer(player, DeathReasonEnum.Bombed, false);
        }

        gameObject.Destroy();
    }

    public static Bomb CreateBomb(PlayerControl owner, bool drived)
    {
        var range = Bomber.BombRange + (drived ? Bomber.ChaosDriveBombRange : 0f);
        var gameObject = CreateRange(CustomColorManager.Bomber, range, "Bomb");
        var bomb = gameObject.AddComponent<Bomb>();
        bomb.Owner = owner;
        bomb.Drived = drived;
        bomb.Size = range;
        var position = owner.GetTruePosition();
        gameObject.transform.position = new(position.x, position.y, (position.y / 1000f) + 0.001f);
        return bomb;
    }
}