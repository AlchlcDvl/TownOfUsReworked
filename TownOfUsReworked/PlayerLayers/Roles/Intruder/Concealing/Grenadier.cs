namespace TownOfUsReworked.PlayerLayers.Roles;

[HeaderOption(MultiMenu.LayerSubOptions)]
public class Grenadier : Intruder, IFlasher
{
    [NumberOption(10f, 60f, 2.5f, Format.Time)]
    public static Number FlashCd = 25;

    [NumberOption(5f, 30f, 1f, Format.Time)]
    public static Number FlashDur = 10;

    [NumberOption(0.5f, 10f, 0.5f, Format.Distance)]
    public static Number FlashRadius = 4.5f;

    [ToggleOption]
    public static bool GrenadierIndicators = false;

    [ToggleOption]
    public static bool SaboFlash = false;

    [ToggleOption]
    public static bool GrenadierVent = false;

    public CustomButton FlashButton { get; set; }
    public IEnumerable<byte> FlashedPlayers { get; set; }

    public override UColor Color => ClientOptions.CustomIntColors ? CustomColorManager.Grenadier : FactionColor;
    public override LayerEnum Type => LayerEnum.Grenadier;
    public override Func<string> StartText => () => "Blind The <#8CFFFFFF>Crew</color> With Your Magnificent Figure";
    public override Func<string> Description => () => $"- You can drop a flashbang which blinds players around you\n{CommonAbilities}";

    public override void Init()
    {
        base.Init();
        Alignment = Alignment.Concealing;
        FlashedPlayers = [];
        FlashButton ??= new(this, new SpriteName("Flash"), AbilityTypes.Targetless, KeybindType.Secondary, (OnClickTargetless)HitFlash, new Cooldown(FlashCd), (EffectStartVoid)StartFlash,
            "FLASH", new Duration(FlashDur), (EffectVoid)Flash, (EffectEndVoid)UnFlash, (ConditionFunc)Condition);
    }

    public void Flash()
    {
        var hud = HUD();

        foreach (var id in FlashedPlayers)
        {
            var player = PlayerById(id);

            if (player.AmOwner)
            {
                if (FlashButton.EffectTime > FlashDur - 0.5f)
                {
                    var fade = (FlashButton.EffectTime - FlashDur) * -2f;

                    if (ShouldPlayerBeBlinded(player))
                        hud.FullScreen.color = Color32.Lerp(CustomColorManager.NormalVision, CustomColorManager.BlindVision, fade);
                    else if (ShouldPlayerBeDimmed(player))
                        hud.FullScreen.color = Color32.Lerp(CustomColorManager.NormalVision, CustomColorManager.DimVision, fade);
                    else
                        hud.FullScreen.color = CustomColorManager.NormalVision;
                }
                else if (FlashButton.EffectTime.IsInRange(0.5f, FlashDur - 0.5f, true, true))
                {
                    if (ShouldPlayerBeBlinded(player))
                        hud.FullScreen.color = CustomColorManager.BlindVision;
                    else if (ShouldPlayerBeDimmed(player))
                        hud.FullScreen.color = CustomColorManager.DimVision;
                    else
                        hud.FullScreen.color = CustomColorManager.NormalVision;
                }
                else if (FlashButton.EffectTime < 0.5f)
                {
                    var fade2 = (FlashButton.EffectTime * -2) + 1;

                    if (ShouldPlayerBeBlinded(player))
                        hud.FullScreen.color = Color32.Lerp(CustomColorManager.BlindVision, CustomColorManager.NormalVision, fade2);
                    else if (ShouldPlayerBeDimmed(player))
                        hud.FullScreen.color = Color32.Lerp(CustomColorManager.DimVision, CustomColorManager.NormalVision, fade2);
                    else
                        hud.FullScreen.color = CustomColorManager.NormalVision;
                }

                if (MapBehaviourPatches.MapActive)
                    Map().Close();

                if (ActiveTask())
                    ActiveTask().Close();
            }
        }
    }

    private bool ShouldPlayerBeDimmed(PlayerControl player) => player.HasDied() || (((player.Is(Faction) && Faction is Faction.Intruder or Faction.Syndicate) || (player.Is(SubFaction) &&
        SubFaction != SubFaction.None)) && !Meeting()) || player == Player || Meeting();

    private bool ShouldPlayerBeBlinded(PlayerControl player) => !ShouldPlayerBeDimmed(player);

    public void UnFlash()
    {
        FlashedPlayers = [];
        SetFullScreenHUD();
    }

    public void HitFlash()
    {
        CallRpc(CustomRPC.Action, ActionsRPC.ButtonAction, FlashButton);
        FlashButton.Begin();
    }

    public void StartFlash() => FlashedPlayers = GetClosestPlayers(Player, FlashRadius, includeDead: true).Select(x => x.PlayerId);

    public bool Condition() => !Ship().Systems[SystemTypes.Sabotage].Cast<SabotageSystemType>().AnyActive && !SaboFlash;
}