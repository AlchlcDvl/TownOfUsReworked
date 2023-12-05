namespace TownOfUsReworked.Modules;

public class InteractionData
{
    public readonly bool Reset;
    public readonly bool Vested;
    public readonly bool Protected;
    public readonly bool AbilityUsed;

    public InteractionData(bool reset, bool vested, bool gaProtected, bool abilityUsed)
    {
        Reset = reset;
        Vested = vested;
        Protected = gaProtected;
        AbilityUsed = abilityUsed;
    }

    private static bool TargetShouldAttack(PlayerControl player, PlayerControl target, bool harmful)
    {
        if (target.IsOnAlert())
            return true;
        else if (target.IsAmbushed() && (!player.Is(Faction.Intruder) || (player.Is(Faction.Intruder) && CustomGameOptions.AmbushMates)))
            return true;
        else if (target.Is(LayerEnum.VampireHunter) && player.Is(SubFaction.Undead))
            return true;
        else if (target.Is(LayerEnum.SerialKiller) && (player.Is(LayerEnum.Escort) || player.Is(LayerEnum.Consort) || player.Is(LayerEnum.Glitch)) && !harmful)
            return true;
        else if (target.IsCrusaded() && (!player.Is(Faction.Syndicate) || (player.Is(Faction.Syndicate) && CustomGameOptions.CrusadeMates)))
            return true;
        else
            return false;
    }

    private static void Trigger(PlayerControl player, PlayerControl target, bool harmful)
    {
        PlayerLayer.GetLayers<Trapper>().ForEach(x => x.TriggerTrap(target, player, harmful));
        PlayerLayer.GetLayers<Retributionist>().ForEach(x => x.TriggerTrap(target, player, harmful));
    }

    public static InteractionData Interact(PlayerControl player, PlayerControl target, bool toKill = false, bool toConvert = false, bool bypass = false, bool poisoning = false)
    {
        var fullReset = false;
        var gaReset = false;
        var survReset = false;
        var abilityUsed = false;
        bypass |= player.Is(LayerEnum.Ruthless);
        Spread(player, target);

        if ((target == CachedFirstDead || target.IsProtectedMonarch() || player.IsOtherRival(target) || player.NotTransformed()) && (toConvert || toKill || poisoning))
            fullReset = true;
        else if (TargetShouldAttack(player, target, toKill) && !bypass)
        {
            if (player.Is(LayerEnum.Pestilence))
            {
                if (target.IsShielded() && (toKill || toConvert || poisoning))
                    fullReset = RpcBreakShield(target);
                else if (target.IsProtected())
                    gaReset = true;
            }
            else if (player.IsShielded() && !target.Is(LayerEnum.Ruthless))
                fullReset = RpcBreakShield(player);
            else if (player.IsProtected() && !target.Is(LayerEnum.Ruthless))
                gaReset = true;
            else
                RpcMurderPlayer(target, player);

            if (target.IsShielded() && (toKill || toConvert || poisoning))
                fullReset = RpcBreakShield(target);

            if (target.CrusadeActive())
                Crusader.RadialCrusade(target);
        }
        else if (target.IsTrapped())
        {
            Trigger(player, target, toKill || toConvert || poisoning);
            abilityUsed = !(toKill || toConvert || poisoning);

            if (!abilityUsed)
            {
                if (player.IsShielded())
                    fullReset = RpcBreakShield(player);
                else if (player.IsProtected())
                    gaReset = true;
                else
                    RpcMurderPlayer(target, player);

                if (target.IsShielded())
                    fullReset = RpcBreakShield(target);
            }
        }
        else if (target.IsShielded() && (toKill || toConvert || poisoning) && !bypass)
            fullReset = RpcBreakShield(target);
        else if (target.IsVesting() && (toKill || toConvert || poisoning) && !bypass)
            survReset = true;
        else if (target.IsProtected() && (toKill || toConvert || poisoning) && !bypass)
            gaReset = true;
        else
        {
            if (toKill)
            {
                if (target.Is(LayerEnum.Fanatic) && (player.Is(Faction.Intruder) || player.Is(Faction.Syndicate)) && target.IsUnturnedFanatic() && !bypass)
                {
                    var fact = player.GetFaction();
                    var fanatic = Objectifier.GetObjectifier<Fanatic>(target);
                    fanatic.TurnFanatic(fact);
                    CallRpc(CustomRPC.Change, TurnRPC.TurnFanatic, fanatic, fact);
                }
                else if (!poisoning)
                    RpcMurderPlayer(player, target);
            }
            else if (toConvert && !target.Is(SubFaction.None))
                RpcMurderPlayer(player, target, DeathReasonEnum.Failed);

            abilityUsed = true;
            fullReset = true;
        }

        if (TargetShouldAttack(player, target, toKill || poisoning) && bypass)
            RpcMurderPlayer(target, player);

        return new(fullReset, survReset, gaReset, abilityUsed);
    }

    public static InteractionData Interact(PlayerControl player, Vent target)
    {
        var fullReset = false;
        var gaReset = false;
        var abilityUsed = false;

        if (target.IsBombed())
        {
            if (player.IsShielded())
                fullReset = RpcBreakShield(player);
            else if (player.IsProtected())
                gaReset = true;
            else
                RpcMurderPlayer(player, PlayerLayer.GetLayers<Bastion>().First(x => x.BombedIDs.Contains(target.Id)).Player, DeathReasonEnum.Bombed, false);

            Role.BastionBomb(target, CustomGameOptions.BombRemovedOnKill);
            CallRpc(CustomRPC.Misc, MiscRPC.BastionBomb, target);
        }
        else
        {
            abilityUsed = true;
            fullReset = true;
        }

        return new(fullReset, false, gaReset, abilityUsed);
    }
}