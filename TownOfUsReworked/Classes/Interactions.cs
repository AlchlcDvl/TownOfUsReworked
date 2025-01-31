namespace TownOfUsReworked.Classes;

public static class Interactions
{
    private static bool TargetShouldAttack(PlayerControl player, PlayerControl target, bool harmful)
    {
        if (target.IsOnAlert())
            return true;
        else if (target.IsAmbushed() && (!player.Is(Faction.Intruder, Faction.Pandorica) || Ambusher.AmbushMates))
            return true;
        else if (target.GetRole() is SerialKiller sk && sk.BloodlustButton.EffectActive && player.GetRole() is Escort or Consort or Glitch && !harmful)
            return true;
        else if (target.IsCrusaded() && (!player.Is(Faction.Syndicate, Faction.Pandorica) || Crusader.CrusadeMates))
            return true;
        else
            return false;
    }

    private static void Trigger(PlayerControl player, PlayerControl target, bool harmful, out PlayerControl trapper)
    {
        trapper = harmful ? PlayerLayer.GetILayers<ITrapper>().FirstOrDefault(x => x.Trapped.Contains(target.PlayerId))?.Player : null;
        PlayerLayer.GetILayers<ITrapper>().ForEach(x => x.TriggerTrap(target, player, harmful));
    }

    public static void Spread(PlayerControl interacter, PlayerControl target)
    {
        PlayerLayer.GetLayers<Plaguebearer>().ForEach(pb => pb.RpcSpreadInfection(target, interacter));
        PlayerLayer.GetLayers<Arsonist>().ForEach(arso => arso.RpcSpreadDouse(target, interacter));
        PlayerLayer.GetLayers<Cryomaniac>().ForEach(cryo => cryo.RpcSpreadDouse(target, interacter));

        if (!PlayerLayer.GetLayers<Pestilence>().Any())
            return;

        var targetId = target.Is(Alignment.Apocalypse) || target.Is(Alignment.Harbinger) ? interacter.PlayerId : target.PlayerId;

        if (!Pestilence.Infected.ContainsKey(targetId))
            return;

        if (interacter.Is<Pestilence>())
            Pestilence.Infected[target.PlayerId] = Pestilence.MaxStacks;
        else if (target.Is<Pestilence>())
            Pestilence.Infected[interacter.PlayerId] = Pestilence.MaxStacks;
        else
            Pestilence.Infected[targetId]++;

        if (Pestilence.Infected.TryGetValue(target.PlayerId, out var count) && count >= Pestilence.MaxStacks)
            interacter.RpcMurderPlayer(target, DeathReasonEnum.Infected, false);
        else if (Pestilence.Infected.TryGetValue(interacter.PlayerId, out count) && count >= Pestilence.MaxStacks)
            target.RpcMurderPlayer(interacter, DeathReasonEnum.Infected, false);
        else
            CallRpc(CustomRPC.Action, ActionsRPC.Infect, targetId, Pestilence.Infected[targetId]);
    }

    public static CooldownType Interact(PlayerControl source, PlayerControl target, bool isAttack = false, bool astral = false, bool bypass = false, bool delayed = false, DeathReasonEnum reason
        = DeathReasonEnum.Killed)
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
                    source.RpcMurderPlayer(target, reason);
                else if (target.IsUnturnedFanatic() && faction is Faction.Intruder or Faction.Syndicate)
                {
                    CustomStatsManager.IncrementStat(CustomStatsManager.StatsHitImmune);
                    var fan = target.GetLayer<Fanatic>();
                    CallRpc(CustomRPC.Misc, MiscRPC.ChangeRoles, fan, false, faction);
                    fan.TurnFanatic(faction);
                }
                else
                {
                    abilityUsed = !(target.IsTrapped() && trapper && !astral);

                    if (target.IsTrapped() && trapper && !astral)
                    {
                        if (source.IsShielded() || source.IsProtected())
                            abilityUsed = false;
                        else
                            trapper.RpcMurderPlayer(source, false);
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
            else
            {
                if (attack > AttackEnum.None)
                {
                    CustomStatsManager.IncrementStat(CustomStatsManager.StatsHitImmune);
                    CustomAchievementManager.RpcUnlockAchievement(target, "Resilient");
                }

                if (target.IsShielded())
                {
                    RpcBreakShield(target);
                    abilityUsed = false;
                }
            }
        }

        if (TargetShouldAttack(source, target, isAttack && !bypass))
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
                                trapper.RpcMurderPlayer(target, false);
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

        if (target.CrusadeActive())
            Crusader.RadialCrusade(target);

        return abilityUsed ? CooldownType.Reset : CooldownType.Fail;
    }

    public static CooldownType Interact(PlayerControl player, Vent target)
    {
        var abilityUsed = true;

        if (target.IsBombed())
        {
            abilityUsed = false;

            if (!CanAttack(AttackEnum.Powerful, player.GetDefenseValue()))
            {
                abilityUsed = true;
                RpcBreakShield(player);
            }
            else if (PlayerLayer.GetILayers<IVentBomber>().TryFinding(x => x.BombedIDs.Contains(target.Id), out var bastion))
                bastion.Player.RpcMurderPlayer(player, DeathReasonEnum.Bombed, false);

            Role.BastionBomb(target, Bastion.BombRemovedOnKill);
            CallRpc(CustomRPC.Misc, MiscRPC.BastionBomb, target);
        }

        return abilityUsed ? CooldownType.Reset : CooldownType.Fail;
    }

    public static bool CanAttack(AttackEnum attack, DefenseEnum defense) => (int)attack > (int)defense;
}