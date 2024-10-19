namespace TownOfUsReworked.PlayerLayers.Roles;

[HeaderOption(MultiMenu.LayerSubOptions)]
public class Cryomaniac : Neutral
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
        BaseStart();
        Objectives = () => "- Freeze anyone who can oppose you";
        Alignment = Alignment.NeutralKill;
        Doused = [];
        DouseButton = CreateButton(this, new SpriteName("CryoDouse"), AbilityType.Alive, KeybindType.ActionSecondary, (OnClick)Douse, new Cooldown(CryoDouseCd), "DOUSE",
            (PlayerBodyExclusion)Exception);
        FreezeButton = CreateButton(this, new SpriteName("Freeze"), AbilityType.Targetless, KeybindType.Secondary, (OnClick)FreezeUnFreeze, (LabelFunc)Label, (UsableFunc)Doused.Any);

        if (CryoLastKillerBoost)
        {
            KillButton = CreateButton(this, new SpriteName("CryoKill"), AbilityType.Alive, KeybindType.Tertiary, (OnClick)Kill, new Cooldown(VaporiseCd), "VAPORISE", (UsableFunc)Usable,
                (PlayerBodyExclusion)Exception);
        }

    }

    public void Kill() => KillButton.StartCooldown(Interact(Player, KillButton.GetTarget<PlayerControl>(), true));

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
                if (cryo != this && !CryoFreezeAll)
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
        var cooldown = Interact(Player, DouseButton.GetTarget<PlayerControl>());

        if (cooldown != CooldownType.Fail)
            RpcSpreadDouse(Player, DouseButton.GetTarget<PlayerControl>());

        DouseButton.StartCooldown(cooldown);
    }

    public void FreezeUnFreeze() => FreezeUsed = !FreezeUsed;

    public string Label() => (FreezeUsed ? "UN" : "") + "FREEZE";

    public bool Usable() => LastKiller;

    public override void ReadRPC(MessageReader reader) => Doused.Add(reader.ReadByte());
}