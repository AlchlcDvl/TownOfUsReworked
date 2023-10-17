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
        else if ((target.IsOnAlert() || ((target.IsAmbushed() || target.IsGFAmbushed()) && (!player.Is(Faction.Intruder) || (player.Is(Faction.Intruder) && CustomGameOptions.AmbushMates)))
            || target.Is(LayerEnum.Pestilence) || (target.Is(LayerEnum.VampireHunter) && player.Is(SubFaction.Undead)) || (target.Is(LayerEnum.SerialKiller) && (player.Is(LayerEnum.Escort)
            || player.Is(LayerEnum.Consort) || player.Is(LayerEnum.Glitch)) && !toKill)) && !bypass)
        {
            if (player.Is(LayerEnum.Pestilence))
            {
                if ((target.IsShielded() || target.IsRetShielded()) && (toKill || toConvert || poisoning))
                {
                    fullReset = CustomGameOptions.ShieldBreaks;
                    Role.BreakShield(target, CustomGameOptions.ShieldBreaks);
                    CallRpc(CustomRPC.Misc, MiscRPC.AttemptSound, target);
                }
                else if (target.IsProtected())
                    gaReset = true;
            }
            else if ((player.IsShielded() || player.IsRetShielded()) && !target.Is(LayerEnum.Ruthless))
            {
                fullReset = CustomGameOptions.ShieldBreaks;
                Role.BreakShield(player, CustomGameOptions.ShieldBreaks);
                CallRpc(CustomRPC.Misc, MiscRPC.AttemptSound, player);
            }
            else if (player.IsProtected() && !target.Is(LayerEnum.Ruthless))
                gaReset = true;
            else
                RpcMurderPlayer(target, player, target.IsAmbushed() ? DeathReasonEnum.Ambushed : DeathReasonEnum.Killed);

            if ((target.IsShielded() || target.IsRetShielded()) && (toKill || toConvert || poisoning))
            {
                fullReset = CustomGameOptions.ShieldBreaks;
                Role.BreakShield(target, CustomGameOptions.ShieldBreaks);
                CallRpc(CustomRPC.Misc, MiscRPC.AttemptSound, target);
            }
        }
        else if ((target.IsCrusaded() || target.IsRebCrusaded()) && (!player.Is(Faction.Syndicate) || (player.Is(Faction.Syndicate) && CustomGameOptions.CrusadeMates)) && !bypass)
        {
            if (player.Is(LayerEnum.Pestilence))
            {
                if ((target.IsShielded() || target.IsRetShielded()) && (toKill || toConvert || poisoning))
                {
                    fullReset = CustomGameOptions.ShieldBreaks;
                    Role.BreakShield(target, CustomGameOptions.ShieldBreaks);
                    CallRpc(CustomRPC.Misc, MiscRPC.AttemptSound, target);
                }
                else if (target.IsProtected())
                    gaReset = true;
            }
            else if ((player.IsShielded() || player.IsRetShielded()) && !target.Is(LayerEnum.Ruthless))
            {
                fullReset = CustomGameOptions.ShieldBreaks;
                Role.BreakShield(player, CustomGameOptions.ShieldBreaks);
                CallRpc(CustomRPC.Misc, MiscRPC.AttemptSound, player);
            }
            else if (player.IsProtected() && !target.Is(LayerEnum.Ruthless))
                gaReset = true;
            else
            {
                var crus = target.GetCrusader();
                var reb = target.GetRebCrus();

                if (crus?.HoldsDrive == true || reb?.HoldsDrive == true)
                    Crusader.RadialCrusade(target);
                else
                    RpcMurderPlayer(target, player, DeathReasonEnum.Crusaded);
            }

            if ((target.IsShielded() || target.IsRetShielded()) && (toKill || toConvert || poisoning))
            {
                fullReset = CustomGameOptions.ShieldBreaks;
                Role.BreakShield(target, CustomGameOptions.ShieldBreaks);
                CallRpc(CustomRPC.Misc, MiscRPC.AttemptSound, target);
            }
        }
        else if ((target.IsShielded() || target.IsRetShielded()) && (toKill || toConvert || poisoning) && !bypass)
        {
            fullReset = CustomGameOptions.ShieldBreaks;
            Role.BreakShield(target, CustomGameOptions.ShieldBreaks);
            CallRpc(CustomRPC.Misc, MiscRPC.AttemptSound, target);
        }
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

        if ((target.IsOnAlert() || ((target.IsAmbushed() || target.IsGFAmbushed()) && (!player.Is(Faction.Intruder) || (player.Is(Faction.Intruder) && CustomGameOptions.AmbushMates))) ||
            target.Is(LayerEnum.Pestilence) || (target.Is(LayerEnum.VampireHunter) && player.Is(SubFaction.Undead)) || (target.Is(LayerEnum.SerialKiller) && (player.Is(LayerEnum.Escort) ||
            player.Is(LayerEnum.Consort) || player.Is(LayerEnum.Glitch)) && !toKill) || ((target.IsCrusaded() || target.IsRebCrusaded()) && (!player.Is(Faction.Syndicate) ||
            (player.Is(Faction.Syndicate) && CustomGameOptions.CrusadeMates)))) && bypass && !target.Is(LayerEnum.Pestilence))
        {
            RpcMurderPlayer(target, player);
        }

        return new(fullReset, survReset, gaReset, abilityUsed);
    }

    public static InteractionData Interact(PlayerControl player, Vent target)
    {
        var fullReset = false;
        var gaReset = false;
        var abilityUsed = false;

        if (target.IsBombed())
        {
            if (player.IsShielded() || player.IsRetShielded())
            {
                fullReset = CustomGameOptions.ShieldBreaks;
                Role.BreakShield(player, CustomGameOptions.ShieldBreaks);
                CallRpc(CustomRPC.Misc, MiscRPC.AttemptSound, player);
            }
            else if (player.IsProtected())
                gaReset = true;
            else
                RpcMurderPlayer(player, PlayerLayer.GetLayers<Bastion>().First(x => x.BombedIDs.Contains(target.Id)).Player, DeathReasonEnum.Bombed, false);

            Role.BastionBomb(target, CustomGameOptions.BombRemovedOnKill);
            CallRpc(CustomRPC.Misc, MiscRPC.BastionBomb, target);
            Flash(Colors.Bastion);
        }
        else
        {
            abilityUsed = true;
            fullReset = true;
        }

        return new(fullReset, false, gaReset, abilityUsed);
    }
}