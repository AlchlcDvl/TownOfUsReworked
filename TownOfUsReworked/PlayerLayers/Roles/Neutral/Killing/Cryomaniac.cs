﻿namespace TownOfUsReworked.PlayerLayers.Roles;

public class Cryomaniac : Neutral
{
    public CustomButton FreezeButton { get; set; }
    public CustomButton DouseButton { get; set; }
    public CustomButton KillButton { get; set; }
    public List<byte> Doused { get; set; }
    public bool FreezeUsed { get; set; }
    public bool LastKiller => !CustomPlayer.AllPlayers.Any(x => !x.HasDied() && (x.Is(Faction.Intruder) || x.Is(Faction.Syndicate) || x.Is(Alignment.CrewKill) || x.Is(Alignment.CrewAudit) ||
        x.Is(Alignment.NeutralPros) || x.Is(Alignment.NeutralNeo) || (x.Is(Alignment.NeutralKill) && x != Player))) && CustomGameOptions.CryoLastKillerBoost;

    public override UColor Color => ClientGameOptions.CustomNeutColors ? CustomColorManager.Cryomaniac : CustomColorManager.Neutral;
    public override string Name => "Cryomaniac";
    public override LayerEnum Type => LayerEnum.Cryomaniac;
    public override Func<string> StartText => () => "Who Likes Ice Cream?";
    public override Func<string> Description => () => "- You can douse players in coolant\n- Doused players can be frozen, which kills all of them at once at the start of the next " +
        $"meeting\n- People who interact with you will also get doused{(LastKiller ? "\n- You can kill normally" : "")}";
    public override AttackEnum AttackVal => AttackEnum.Unstoppable;
    public override DefenseEnum DefenseVal => Doused.Count is 1 or 2 ? DefenseEnum.Basic : DefenseEnum.None;

    public override void Init()
    {
        BaseStart();
        Objectives = () => "- Freeze anyone who can oppose you";
        Alignment = Alignment.NeutralKill;
        Doused = [];
        DouseButton = CreateButton(this, new SpriteName("CryoDouse"), AbilityTypes.Alive, KeybindType.ActionSecondary, (OnClick)Douse, new Cooldown(CustomGameOptions.CryoDouseCd), "DOUSE",
            (PlayerBodyExclusion)Exception);
        FreezeButton = CreateButton(this, new SpriteName("Freeze"), AbilityTypes.Targetless, KeybindType.Secondary, (OnClick)FreezeUnFreeze, (LabelFunc)Label, (UsableFunc)Doused.Any);

        if (CustomGameOptions.CryoLastKillerBoost)
        {
            KillButton = CreateButton(this, new SpriteName("CryoKill"), AbilityTypes.Alive, KeybindType.Tertiary, (OnClick)Kill, new Cooldown(CustomGameOptions.CryoKillCd), "KILL",
                (PlayerBodyExclusion)Exception, (UsableFunc)Usable);
        }

    }

    public void Kill() => KillButton.StartCooldown(Interact(Player, KillButton.TargetPlayer, true));

    public void RpcSpreadDouse(PlayerControl source, PlayerControl target)
    {
        if (!source.Is(Type) || Doused.Contains(target.PlayerId) || source != Player)
            return;

        Doused.Add(target.PlayerId);
        CallRpc(CustomRPC.Action, ActionsRPC.LayerAction, this, target.PlayerId);
    }

    public override void OnMeetingStart(MeetingHud __instance)
    {
        base.OnMeetingStart(__instance);

        if (FreezeUsed)
        {
            foreach (var cryo in GetLayers<Cryomaniac>())
            {
                if (cryo != this && !CustomGameOptions.CryoFreezeAll)
                    continue;

                foreach (var player in cryo.Doused)
                {
                    var player2 = PlayerById(player);

                    if (CanAttack(AttackVal, player2.GetDefenseValue()))
                        RpcMurderPlayer(Player, player2, DeathReasonEnum.Frozen, false);
                }

                cryo.Doused.Clear();
            }
        }

        FreezeUsed = false;
    }

    public bool Exception(PlayerControl player) => Doused.Contains(player.PlayerId) || (player.Is(SubFaction) && SubFaction != SubFaction.None) || (player.Is(Faction) && Faction is
        Faction.Intruder or Faction.Syndicate) || Player.IsLinkedTo(player);

    public void Douse()
    {
        var cooldown = Interact(Player, DouseButton.TargetPlayer);

        if (cooldown != CooldownType.Fail)
            RpcSpreadDouse(Player, DouseButton.TargetPlayer);

        DouseButton.StartCooldown(cooldown);
    }

    public void FreezeUnFreeze() => FreezeUsed = !FreezeUsed;

    public string Label() => (FreezeUsed ? "UN" : "") + "FREEZE";

    public bool Usable() => LastKiller;

    public override void ReadRPC(MessageReader reader) => Doused.Add(reader.ReadByte());
}