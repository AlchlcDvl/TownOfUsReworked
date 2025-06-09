namespace TownOfUsReworked.PlayerLayers.Roles;

[LayerHeaderOption(LayerEnum.Cryomaniac)]
public sealed class Cryomaniac : OKilling, IDouser
{
    [NumberOption(10f, 60f, 2.5f, Format.Time)]
    private static Number CryoDouseCd = 25;

    [ToggleOption]
    private static bool CryoFreezeAll = false;

    [ToggleOption]
    private static bool CryoLastKillerBoost = false;

    [NumberOption(10f, 60f, 2.5f, Format.Time)]
    private static Number VaporiseCd = 25;

    [ToggleOption]
    private static bool CryoVent = false;

    private CustomButton FreezeButton { get; set; }
    private CustomButton DouseButton { get; set; }
    private CustomButton KillButton { get; set; }
    public HashSet<byte> Doused { get; } = [];
    public bool FreezeUsed { get; set; }
    private bool LastKiller => !AllPlayers().Any(x => !x.HasDied() && (x.GetFaction() is not (Faction.Crew or Faction.Outcast) || x.GetAlignment() is Alignment.Killing or Alignment.Proselyte
        or Alignment.Neophyte) && x != Player) && CryoLastKillerBoost;

    protected override UColor MainColor => CustomColorManager.Cryomaniac;
    public override LayerEnum Type => LayerEnum.Cryomaniac;
    public override Func<string> StartText { get; } = () => "Who Likes Ice Cream?";
    public override Func<string> Description => () => "- You can douse players in coolant\n- Doused players can be frozen, which kills all of them at once at the start of the next " +
        $"meeting\n- People who interact with you will also get doused{(LastKiller ? "\n- You can kill normally" : "")}";
    public override AttackEnum AttackVal => AttackEnum.Unstoppable;
    public override DefenseEnum DefenseVal => Doused.Count is 1 or 2 ? DefenseEnum.Basic : DefenseEnum.None;
    public override bool CanVent => base.CanVent && CryoVent;
    protected override Faction ActualFaction => Faction.Cryomaniac;

    protected override void Init()
    {
        base.Init();
        Objectives = () => "- Freeze anyone who can oppose you";
        Doused.Clear();
        DouseButton ??= new(this, new SpriteName("CryoDouse"), AbilityTypes.Player, KeybindType.ActionSecondary, (OnClickPlayer)Douse, new Cooldown(CryoDouseCd), "DOUSE",
            (PlayerBodyExclusion)Exception);
        FreezeButton ??= new(this, new SpriteName("Freeze"), AbilityTypes.Targetless, KeybindType.Secondary, (OnClickTargetless)FreezeUnFreeze, (LabelFunc)Label, (UsableFunc)Doused.Any);

        if (CryoLastKillerBoost)
        {
            KillButton ??= new(this, new SpriteName("CryoKill"), AbilityTypes.Player, KeybindType.Tertiary, (OnClickPlayer)Kill, new Cooldown(VaporiseCd), "VAPORISE", (UsableFunc)Usable,
                (PlayerBodyExclusion)Exception);
        }
    }

    public override void UpdatePlayerName(LayerHandler handler, PlayerControl player, bool meeting, ref string name, ref UColor color, ref bool revealed, ref bool removeFromConsig)
    {
        if (Doused.Contains(player.PlayerId))
            name += " <#642DEAFF>λ</color>";
    }

    private void Kill(PlayerControl target) => KillButton.StartCooldown(Interact(Player, target, true));

    public void RpcSpreadDouse(PlayerControl source, PlayerControl target)
    {
        if (!source.Is<Cryomaniac>() || Doused.Contains(target.PlayerId) || source != Player)
            return;

        Doused.Add(target.PlayerId);
        CallRpc(CustomRPC.Action, ActionsRPC.LayerAction, this, DouseActionsRPC.Douse, target.PlayerId);
    }

    public override void BeforeMeeting()
    {
        if (!FreezeUsed)
            return;

        foreach (var cryo in GetLayers<Cryomaniac>())
        {
            if (cryo != this && !CryoFreezeAll)
                continue;

            foreach (var player2 in from player in cryo.Doused select PlayerById(player) into player2 where !player2.HasDied() && !player2.TryReversingDouses<Arsonist>() && CanAttack(AttackVal, player2.GetDefenseValue()) select player2)
                Player.RpcMurderPlayer(player2, DeathReasonEnum.Frozen, false);

            cryo.Doused.Clear();
        }

        FreezeUsed = false;
    }

    private bool Exception(PlayerControl player) => Doused.Contains(player.PlayerId) || (player.Is(Faction) && Faction is not
        (Faction.Crew or Faction.Outcast)) || Player.IsLinkedTo(player);

    private void Douse(PlayerControl target)
    {
        var cooldown = Interact(Player, target);

        if (cooldown != CooldownType.Fail)
            RpcSpreadDouse(Player, target);

        DouseButton.StartCooldown(cooldown);
    }

    private void FreezeUnFreeze() => FreezeUsed = !FreezeUsed;

    private string Label() => (FreezeUsed ? "UN" : "") + "FREEZE";

    private bool Usable() => LastKiller;

    public override void ReadRPC(RpcReader reader)
    {
        var cryoAction = reader.Read<DouseActionsRPC>();

        switch (cryoAction)
        {
            case DouseActionsRPC.Douse:
            {
                Doused.Add(reader.ReadByte());
                break;
            }
            case DouseActionsRPC.UnDouse:
            {
                Doused.Remove(reader.ReadByte());
                break;
            }
            default:
            {
                Failure($"Received unknown RPC - {cryoAction}");
                break;
            }
        }
    }
}