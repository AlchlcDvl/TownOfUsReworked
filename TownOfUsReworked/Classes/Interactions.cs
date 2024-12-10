namespace TownOfUsReworked.Classes;

public static class Interactions
{
    private static bool TargetShouldAttack(PlayerControl player, PlayerControl target, bool harmful)
    {
        if (target.IsOnAlert())
            return true;
        else if (target.IsAmbushed() && (!player.Is(Faction.Intruder) || (player.Is(Faction.Intruder) && Ambusher.AmbushMates)))
            return true;
        else if (target.GetRole() is SerialKiller sk && sk.BloodlustButton.EffectActive && player.GetRole() is Escort or Consort or Glitch && !harmful)
            return true;
        else if (target.IsCrusaded() && (!player.Is(Faction.Syndicate) || (player.Is(Faction.Syndicate) && Crusader.CrusadeMates)))
            return true;
        else if (target.Is(LayerEnum.VampireHunter) && player.Is(SubFaction.Undead))
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

        var targetId = target.Is(Alignment.NeutralApoc) || target.Is(Alignment.NeutralHarb) ? interacter.PlayerId : target.PlayerId;

        if (!Pestilence.Infected.ContainsKey(targetId))
            return;

        if (interacter.Is(LayerEnum.Pestilence))
            Pestilence.Infected[target.PlayerId] = Pestilence.MaxStacks;
        else if (target.Is(LayerEnum.Pestilence))
            Pestilence.Infected[interacter.PlayerId] = Pestilence.MaxStacks;
        else
            Pestilence.Infected[targetId]++;

        if (Pestilence.Infected.TryGetValue(target.PlayerId, out var count) && count >= Pestilence.MaxStacks)
            RpcMurderPlayer(interacter, target, DeathReasonEnum.Infected, false);
        else if (Pestilence.Infected.TryGetValue(interacter.PlayerId, out count) && count >= Pestilence.MaxStacks)
            RpcMurderPlayer(target, interacter, DeathReasonEnum.Infected, false);
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
        bypass |= source.Is(LayerEnum.Ruthless);
        PlayerControl trapper = null;

        if (!astral)
        {
            Spread(source, target);
            Trigger(source, target, CanAttack(attack, defense) && isAttack && !bypass, out trapper);
        }

        if (CanAttack(attack, defense) && isAttack)
        {
            if (bypass)
            {
                RpcMurderPlayer(source, target, reason);
                abilityUsed = true;
            }
            else
            {
                if (target.IsUnturnedFanatic() && faction is Faction.Intruder or Faction.Syndicate)
                {
                    var fan = target.GetLayer<Fanatic>();
                    CallRpc(CustomRPC.Misc, MiscRPC.ChangeRoles, fan, false, faction);
                    fan.TurnFanatic(faction);
                    abilityUsed = true;
                }
                else
                {
                    abilityUsed = !(target.IsTrapped() && trapper && !astral);

                    if (target.IsTrapped() && trapper && !astral)
                    {
                        if (source.IsShielded() || source.IsProtected())
                            abilityUsed = false;
                        else
                            RpcMurderPlayer(trapper, source, false);
                    }
                    else if (source.IsShielded() || source.IsProtected())
                    {
                        abilityUsed = false;
                        RpcBreakShield(source);
                    }
                    else if (!delayed)
                        RpcMurderPlayer(source, target, reason);
                }
            }
        }
        else if (target.IsShielded() && isAttack)
        {
            RpcBreakShield(target);
            abilityUsed = false;
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

            if (CanAttack(attack, defense) && isAttack)
            {
                if (bypass)
                {
                    RpcMurderPlayer(source, target);
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
                            RpcMurderPlayer(trapper, target, false);
                    }
                    else if (target.IsShielded() || target.IsProtected())
                    {
                        abilityUsed = false;
                        RpcBreakShield(target);
                    }
                    else if (!delayed)
                        RpcMurderPlayer(target, source);
                }
            }
            else if (source.IsShielded() && isAttack)
            {
                RpcBreakShield(source);
                abilityUsed = false;
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
            var bastion = PlayerLayer.GetILayers<IVentBomber>().Find(x => x.BombedIDs.Contains(target.Id));

            if (!CanAttack(AttackEnum.Powerful, player.GetDefenseValue()))
            {
                abilityUsed = true;
                RpcBreakShield(player);
            }
            else if (bastion != null)
                RpcMurderPlayer(bastion.Player, player, DeathReasonEnum.Bombed, false);

            Role.BastionBomb(target, Bastion.BombRemovedOnKill);
            CallRpc(CustomRPC.Misc, MiscRPC.BastionBomb, target);
        }

        return abilityUsed ? CooldownType.Reset : CooldownType.Fail;
    }

    public static bool CanAttack(AttackEnum attack, DefenseEnum defense) => (int)attack > (int)defense;
}