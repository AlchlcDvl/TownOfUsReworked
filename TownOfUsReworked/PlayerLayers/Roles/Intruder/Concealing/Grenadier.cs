namespace TownOfUsReworked.PlayerLayers.Roles;

[HeaderOption(MultiMenu.LayerSubOptions)]
public class Grenadier : Intruder
{
    [NumberOption(MultiMenu.LayerSubOptions, 10f, 60f, 2.5f, Format.Time)]
    public static Number FlashCd { get; set; } = new(25);

    [NumberOption(MultiMenu.LayerSubOptions, 5f, 30f, 1f, Format.Time)]
    public static Number FlashDur { get; set; } = new(10);

    [NumberOption(MultiMenu.LayerSubOptions, 0.5f, 10f, 0.5f, Format.Distance)]
    public static Number FlashRadius { get; set; } = new(4.5f);

    [ToggleOption(MultiMenu.LayerSubOptions)]
    public static bool GrenadierIndicators { get; set; } = false;

    [ToggleOption(MultiMenu.LayerSubOptions)]
    public static bool SaboFlash { get; set; } = false;

    [ToggleOption(MultiMenu.LayerSubOptions)]
    public static bool GrenadierVent { get; set; } = false;

    public CustomButton FlashButton { get; set; }
    public List<byte> FlashedPlayers { get; set; }

    public override UColor Color => ClientOptions.CustomIntColors ? CustomColorManager.Grenadier : CustomColorManager.Intruder;
    public override string Name => "Grenadier";
    public override LayerEnum Type => LayerEnum.Grenadier;
    public override Func<string> StartText => () => "Blind The <color=#8CFFFFFF>Crew</color> With Your Magnificent Figure";
    public override Func<string> Description => () => $"- You can drop a flashbang which blinds players around you\n{CommonAbilities}";

    public override void Init()
    {
        BaseStart();
        Alignment = Alignment.IntruderConceal;
        FlashedPlayers = [];
        FlashButton ??= CreateButton(this, new SpriteName("Flash"), AbilityTypes.Targetless, KeybindType.Secondary, (OnClick)HitFlash, new Cooldown(FlashCd), (EffectStartVoid)StartFlash,
            new Duration(FlashDur), (EffectVoid)Flash, (EffectEndVoid)UnFlash, (ConditionFunc)Condition, "FLASH");
    }

    public void Flash()
    {
        foreach (var id in FlashedPlayers)
        {
            var player = PlayerById(id);

            if (player.AmOwner)
            {
                if (FlashButton.EffectTime > FlashDur - 0.5f)
                {
                    var fade = (FlashButton.EffectTime - FlashDur) * -2f;

                    if (ShouldPlayerBeBlinded(player))
                        HUD().FullScreen.color = Color32.Lerp(CustomColorManager.NormalVision, CustomColorManager.BlindVision, fade);
                    else if (ShouldPlayerBeDimmed(player))
                        HUD().FullScreen.color = Color32.Lerp(CustomColorManager.NormalVision, CustomColorManager.DimVision, fade);
                    else
                        HUD().FullScreen.color = CustomColorManager.NormalVision;
                }
                else if (FlashButton.EffectTime.IsInRange(0.5f, FlashDur - 0.5f, true, true))
                {
                    if (ShouldPlayerBeBlinded(player))
                        HUD().FullScreen.color = CustomColorManager.BlindVision;
                    else if (ShouldPlayerBeDimmed(player))
                        HUD().FullScreen.color = CustomColorManager.DimVision;
                    else
                        HUD().FullScreen.color = CustomColorManager.NormalVision;
                }
                else if (FlashButton.EffectTime < 0.5f)
                {
                    var fade2 = (FlashButton.EffectTime * -2) + 1;

                    if (ShouldPlayerBeBlinded(player))
                        HUD().FullScreen.color = Color32.Lerp(CustomColorManager.BlindVision, CustomColorManager.NormalVision, fade2);
                    else if (ShouldPlayerBeDimmed(player))
                        HUD().FullScreen.color = Color32.Lerp(CustomColorManager.DimVision, CustomColorManager.NormalVision, fade2);
                    else
                        HUD().FullScreen.color = CustomColorManager.NormalVision;
                }

                if (MapPatch.MapActive)
                    Map().Close();

                if (ActiveTask())
                    ActiveTask().Close();
            }
        }
    }

    private bool ShouldPlayerBeDimmed(PlayerControl player) => (((player.Is(Faction) && Faction is Faction.Intruder or Faction.Syndicate) || (player.Is(SubFaction) && SubFaction !=
        SubFaction.None) || player.Data.IsDead) && !Meeting()) || player == Player;

    private bool ShouldPlayerBeBlinded(PlayerControl player) => !ShouldPlayerBeDimmed(player) && !Meeting();

    public void UnFlash()
    {
        FlashedPlayers.Clear();
        SetFullScreenHUD();
    }

    public void HitFlash()
    {
        CallRpc(CustomRPC.Action, ActionsRPC.ButtonAction, FlashButton);
        FlashButton.Begin();
    }

    public void StartFlash() => FlashedPlayers = [ .. GetClosestPlayers(Player.transform.position, FlashRadius).Select(x => x.PlayerId) ];

    public bool Condition() => !Ship().Systems[SystemTypes.Sabotage].Cast<SabotageSystemType>().AnyActive && !SaboFlash;
}