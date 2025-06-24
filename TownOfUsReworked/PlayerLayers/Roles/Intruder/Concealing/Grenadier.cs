namespace TownOfUsReworked.PlayerLayers.Roles;

[LayerHeaderOption(Layer.Grenadier)]
public sealed class Grenadier : Concealing
{
    [NumberOption(10f, 60f, 2.5f, Format.Time)]
    private static Number FlashCd = 25;

    [NumberOption(5f, 30f, 1f, Format.Time)]
    private static Number FlashDur = 10;

    [NumberOption(0.5f, 10f, 0.5f, Format.Distance)]
    private static Number FlashRadius = 4.5f;

    [ToggleOption]
    private static bool GrenadierIndicators = false;

    [ToggleOption]
    private static bool GrenadierVent = false;

    private CustomButton FlashButton;
    public readonly HashSet<byte> FlashedPlayers = [];

    protected override UColor MainColor => CustomColorManager.Grenadier;
    public override Layer Type => Layer.Grenadier;
    public override string StartText => "Blind The <#8CFFFFFF>Crew</color> With Your Magnificent Figure";
    public override string Description => $"- You can drop a flashbang which blinds players around you\n{CommonAbilities}";
    public override bool CanVent => GrenadierVent;

    public override void Init()
    {
        base.Init();
        FlashedPlayers.Clear();
        FlashButton ??= new(this, new SpriteName("Flash"), AbilityTypes.Targetless, KeybindType.Secondary, (OnClickTargetless)HitFlash, new Cooldown(FlashCd), (EffectStartVoid)StartFlash,
            "FLASH", new Duration(FlashDur), (EffectVoid)Flash, (EffectEndVoid)UnFlash, (ConditionFunc)Condition, new CanClickAgain(false));
    }

    public override void UpdatePlayerName(LayerHandler handler, PlayerControl player, bool meeting, ref string name, ref UColor color, ref bool revealed, ref bool removeFromConsig)
    {
        if (FlashedPlayers.Contains(player.PlayerId) && GrenadierIndicators)
            name += " <#85AA5BFF>ㅇ</color>";
    }

    private void Flash()
    {
        if (!FlashedPlayers.Any(x => PlayerById(x).AmOwner))
            return;

        if (MapBehaviourPatches.MapActive)
            Minimap().Close();

        if (ActiveTask())
            ActiveTask().Close();
    }

    private bool ShouldPlayerBeDimmed(PlayerControl player) => Meeting() || player.HasDied() || Player.IsBuddyWith(player, Handler.CurrentFaction) || player == Player;

    private void UnFlash() => FlashedPlayers.Clear();

    private void HitFlash() => FlashButton.TriggerRpcAndBegin();

    private void StartFlash()
    {
        FlashedPlayers.AddRange(GetClosestPlayers(Player, FlashRadius, includeDead: true).Select(x => x.PlayerId));
        FlashedPlayers.Add(PlayerId);

        if (FlashedPlayers.Contains(LocalPlayer.PlayerId))
            TransitionFlash(CustomColorManager.BlindVision, FlashDur, ShouldPlayerBeDimmed(LocalPlayer) ? 0.4f : 1f);
    }

    private static bool Condition() => !Ship().Systems[SystemTypes.Sabotage].Cast<SabotageSystemType>().AnyActive;
}