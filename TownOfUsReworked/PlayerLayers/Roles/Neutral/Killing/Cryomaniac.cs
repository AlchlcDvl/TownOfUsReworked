﻿namespace TownOfUsReworked.PlayerLayers.Roles;

[HeaderOption(MultiMenu.LayerSubOptions)]
public class Cryomaniac : NKilling
{
    [NumberOption(MultiMenu.LayerSubOptions, 10f, 60f, 2.5f, Format.Time)]
    public static Number CryoDouseCd { get; set; } = new(25);

    [ToggleOption(MultiMenu.LayerSubOptions)]
    public static bool CryoFreezeAll { get; set; } = false;

    [ToggleOption(MultiMenu.LayerSubOptions)]
    public static bool CryoLastKillerBoost { get; set; } = false;

    [NumberOption(MultiMenu.LayerSubOptions, 10f, 60f, 2.5f, Format.Time)]
    public static Number VaporiseCd { get; set; } = new(25);

    [ToggleOption(MultiMenu.LayerSubOptions)]
    public static bool CryoVent { get; set; } = false;

    public CustomButton FreezeButton { get; set; }
    public CustomButton DouseButton { get; set; }
    public CustomButton KillButton { get; set; }
    public List<byte> Doused { get; set; }
    public bool FreezeUsed { get; set; }
    public bool LastKiller => !AllPlayers().Any(x => !x.HasDied() && (x.Is(Faction.Intruder) || x.Is(Faction.Syndicate) || x.Is(Alignment.CrewKill) || x.Is(Alignment.CrewAudit) ||
        x.Is(Alignment.NeutralPros) || x.Is(Alignment.NeutralNeo) || (x.Is(Alignment.NeutralKill) && x != Player))) && CryoLastKillerBoost;

    public override UColor Color => ClientOptions.CustomNeutColors ? CustomColorManager.Cryomaniac : CustomColorManager.Neutral;
    public override string Name => "Cryomaniac";
    public override LayerEnum Type => LayerEnum.Cryomaniac;
    public override Func<string> StartText => () => "Who Likes Ice Cream?";
    public override Func<string> Description => () => "- You can douse players in coolant\n- Doused players can be frozen, which kills all of them at once at the start of the next " +
        $"meeting\n- People who interact with you will also get doused{(LastKiller ? "\n- You can kill normally" : "")}";
    public override AttackEnum AttackVal => AttackEnum.Unstoppable;
    public override DefenseEnum DefenseVal => Doused.Count is 1 or 2 ? DefenseEnum.Basic : DefenseEnum.None;

    public override void Init()
    {
        base.Init();
        Objectives = () => "- Freeze anyone who can oppose you";
        Doused = [];
        DouseButton ??= new(this, new SpriteName("CryoDouse"), AbilityTypes.Alive, KeybindType.ActionSecondary, (OnClick)Douse, new Cooldown(CryoDouseCd), "DOUSE",
            (PlayerBodyExclusion)Exception);
        FreezeButton ??= new(this, new SpriteName("Freeze"), AbilityTypes.Targetless, KeybindType.Secondary, (OnClick)FreezeUnFreeze, (LabelFunc)Label, (UsableFunc)Doused.Any);

        if (CryoLastKillerBoost)
        {
            KillButton ??= new(this, new SpriteName("CryoKill"), AbilityTypes.Alive, KeybindType.Tertiary, (OnClick)Kill, new Cooldown(VaporiseCd), "VAPORISE", (UsableFunc)Usable,
                (PlayerBodyExclusion)Exception);
        }

    }

    public void Kill() => KillButton.StartCooldown(Interact(Player, KillButton.GetTarget<PlayerControl>(), true));

    public void RpcSpreadDouse(PlayerControl source, PlayerControl target)
    {
        if (!source.Is(Type) || Doused.Contains(target.PlayerId) || source != Player)
            return;

        Doused.Add(target.PlayerId);
        CallRpc(CustomRPC.Action, ActionsRPC.LayerAction, this, CryoActionsRPC.Douse, target.PlayerId);
    }

    public override void BeforeMeeting()
    {
        if (FreezeUsed && Local)
        {
            foreach (var cryo in GetLayers<Cryomaniac>())
            {
                if (cryo != this && !CryoFreezeAll)
                    continue;

                foreach (var player in cryo.Doused)
                {
                    var player2 = PlayerById(player);

                    if (player2.HasDied())
                        continue;

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
        var cooldown = Interact(Player, DouseButton.GetTarget<PlayerControl>());

        if (cooldown != CooldownType.Fail)
            RpcSpreadDouse(Player, DouseButton.GetTarget<PlayerControl>());

        DouseButton.StartCooldown(cooldown);
    }

    public void FreezeUnFreeze()
    {
        FreezeUsed = !FreezeUsed;
        CallRpc(CustomRPC.Action, ActionsRPC.LayerAction, this, CryoActionsRPC.Freeze);
    }

    public string Label() => (FreezeUsed ? "UN" : "") + "FREEZE";

    public bool Usable() => LastKiller;

    public override void ReadRPC(MessageReader reader)
    {
        var cryoAction = reader.ReadEnum<CryoActionsRPC>();

        switch (cryoAction)
        {
            case CryoActionsRPC.Douse:
                Doused.Add(reader.ReadByte());
                break;

            case CryoActionsRPC.UnDouse:
                Doused.Remove(reader.ReadByte());
                break;

            case CryoActionsRPC.Freeze:
                FreezeUsed = !FreezeUsed;
                break;

            default:
                Error($"Received unknown RPC - {(int)cryoAction}");
                break;
        }
    }
}