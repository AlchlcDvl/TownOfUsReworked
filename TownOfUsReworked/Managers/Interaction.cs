// ReSharper disable ConditionIsAlwaysTrueOrFalse

namespace TownOfUsReworked.Utils;

public static class InteractionManager
{
    private static bool TargetShouldAttack(PlayerControl player, PlayerControl target, bool harmful)
    {
        if (target.IsOnAlert())
            return true;

        if (target.IsAmbushed(out var amb) && !amb.Exception1(player))
            return true;

        if (target.GetRole() is SerialKiller sk && sk.BloodlustButton.EffectActive && player.GetRole() is IBlocker && !harmful)
            return true;

        return target.IsCrusaded(out var crus) && !crus.Exception1(player);
    }

    private static void Trigger(PlayerControl player, PlayerControl target, bool harmful, out PlayerControl trapper)
    {
        trapper = harmful ? PlayerLayer.GetLayers<ITrapper>().FirstOrDefault(x => x.Trapped.Contains(target.PlayerId))?.Player : null;
        PlayerLayer.GetLayers<ITrapper>().Do(x => x.TriggerTrap(target, player, harmful));
    }

    public static void Spread(PlayerControl interactor, PlayerControl target)
    {
        PlayerLayer.GetLayers<Plaguebearer>().Do(pb => pb.RpcSpreadInfection(target, interactor));
        PlayerLayer.GetLayers<Arsonist>().Do(arso => arso.RpcSpreadDouse(target, interactor));
        PlayerLayer.GetLayers<Cryomaniac>().Do(cryo => cryo.RpcSpreadDouse(target, interactor));

        if (!PlayerLayer.GetLayers<Pestilence>().Any())
            return;

        var targetId = target.Is(Alignment.Deity) || target.Is(Alignment.Harbinger) ? interactor.PlayerId : target.PlayerId;

        if (!Pestilence.Infected.ContainsKey(targetId))
            return;

        if (interactor.Is<Pestilence>())
            Pestilence.Infected[target.PlayerId] = Pestilence.MaxStacks;
        else if (target.Is<Pestilence>())
            Pestilence.Infected[interactor.PlayerId] = Pestilence.MaxStacks;
        else
            Pestilence.Infected[targetId]++;

        if (Pestilence.Infected.TryGetValue(target.PlayerId, out var count) && count >= Pestilence.MaxStacks)
            interactor.RpcMurderPlayer(target, DeathReasonEnum.Infected, false);
        else if (Pestilence.Infected.TryGetValue(interactor.PlayerId, out count) && count >= Pestilence.MaxStacks)
            target.RpcMurderPlayer(interactor, DeathReasonEnum.Infected, false);
        else
            CallRpc(CustomRPC.Action, ActionsRPC.Infect, targetId, Pestilence.Infected[targetId]);
    }

    public static CooldownType Interact(PlayerControl source, PlayerControl target, bool isAttack = false, bool astral = false, bool bypass = false, bool delayed = false, DeathReasonEnum reason
        = DeathReasonEnum.Killed, bool lunge = true)
    {
        if (!source || !target)
            return CooldownType.Fail;

        var abilityUsed = true;
        var attack = source.GetAttackValue(target);
        var defense = target.GetDefenseValue(source);
        var faction = source.GetFaction();
        bypass |= source.Is<Ruthless>();
        PlayerControl trapper = null;

        if (!astral)
        {
            Spread(source, target);
            Trigger(source, target, CanAttack(attack, defense) && isAttack && !bypass, out trapper);
        }

        if (isAttack)
        {
            if (CanAttack(attack, defense))
            {
                if (bypass)
                    source.RpcMurderPlayer(target, reason, lunge);
                else if (target.Is<Fanatic>(out var fanatic) && !fanatic.Turned && faction is not (Faction.Crew or Faction.Outcast or Faction.GameMode))
                {
                    CustomStatsManager.IncrementStat(CustomStatsManager.StatsHitImmune);
                    CustomStatsManager.IncrementStat(CustomStatsManager.StatsConvertedFanatics);
                    CallRpc(CustomRPC.Misc, MiscRPC.ChangeRoles, fanatic, false, faction);
                    fanatic.TurnFaction(faction);
                }
                else
                {
                    abilityUsed = !(target.IsTrapped() && trapper && !astral);

                    if (target.IsTrapped() && trapper && !astral)
                    {
                        if (source.IsShielded() || source.IsProtected())
                            abilityUsed = false;
                        else
                            trapper.RpcCustomMurderPlayer(source, false);
                    }
                    else if (source.IsShielded() || source.IsProtected())
                    {
                        abilityUsed = false;
                        RpcBreakShield(source);
                    }
                    else if (!delayed)
                        source.RpcMurderPlayer(target, reason);
                }
            }
            else if (target.IsShielded())
            {
                RpcBreakShield(target);
                abilityUsed = false;
            }
        }

        var targetAttack = TargetShouldAttack(source, target, isAttack && !bypass);

        if (targetAttack)
        {
            attack = target.GetAttackValue(source);
            defense = source.GetDefenseValue(target);

            if (!astral)
            {
                Spread(target, source);
                Trigger(target, source, CanAttack(attack, defense) && isAttack, out trapper);
            }

            if (isAttack)
            {
                if (CanAttack(attack, defense))
                {
                    if (bypass)
                    {
                        source.RpcMurderPlayer(target);
                        abilityUsed = true;
                    }
                    else
                    {
                        abilityUsed = !(source.IsTrapped() && trapper && !astral);

                        if (source.IsTrapped() && trapper && !astral)
                        {
                            if (target.IsShielded() || target.IsProtected())
                                abilityUsed = false;
                            else
                                trapper.RpcCustomMurderPlayer(target, false);
                        }
                        else if (target.IsShielded() || target.IsProtected())
                        {
                            abilityUsed = false;
                            RpcBreakShield(target);
                        }
                        else if (!delayed)
                            target.RpcMurderPlayer(source);
                    }
                }
                else if (source.IsShielded())
                {
                    RpcBreakShield(source);
                    abilityUsed = false;
                }
            }
        }

        if (target.CrusadeActive(out var crus))
            crus.RadialCrusade(target);

        if (!delayed)
        {
            if (isAttack && !target.HasDied())
            {
                CustomStatsManager.IncrementStat(CustomStatsManager.StatsHitImmune);
                CustomAchievementManager.RpcUnlockAchievement(target, "Resilient");
            }

            if (targetAttack && !source.HasDied())
            {
                CustomStatsManager.RpcIncrementStat(target, CustomStatsManager.StatsHitImmune);
                CustomAchievementManager.UnlockAchievement("Resilient");
            }
        }

        return abilityUsed ? CooldownType.Reset : CooldownType.Fail;
    }

    public static CooldownType Interact(PlayerControl player, Vent target)
    {
        var abilityUsed = true;

        if (!target.IsBombed())
            return abilityUsed ? CooldownType.Reset : CooldownType.Fail;

        abilityUsed = false;

        if (!CanAttack(AttackEnum.Powerful, player.GetDefenseValue()))
        {
            abilityUsed = true;
            RpcBreakShield(player);
        }
        else if (PlayerLayer.GetLayers<IVentBomber>().TryFinding(x => x.BombedIDs.Contains(target.Id), out var bastion))
            bastion.Player.RpcMurderPlayer(player, DeathReasonEnum.Bombed, false);

        BastionBomb(target, Bastion.BombRemovedOnKill);
        CallRpc(CustomRPC.Misc, MiscRPC.BastionBomb, target);
        return abilityUsed ? CooldownType.Reset : CooldownType.Fail;
    }

    public static bool CanAttack(AttackEnum attack, DefenseEnum defense) => (int)attack > (int)defense;
}