namespace TownOfUsReworked.PlayerLayers.Roles;

public class Grenadier : Intruder
{
    public CustomButton FlashButton { get; set; }
    private static Color32 NormalVision => new(212, 212, 212, 0);
    private static Color32 DimVision => new(212, 212, 212, 51);
    private static Color32 BlindVision => new(212, 212, 212, 255);
    public List<PlayerControl> FlashedPlayers { get; set; }

    public override Color Color => ClientGameOptions.CustomIntColors ? Colors.Grenadier : Colors.Intruder;
    public override string Name => "Grenadier";
    public override LayerEnum Type => LayerEnum.Grenadier;
    public override Func<string> StartText => () => "Blind The <color=#8CFFFFFF>Crew</color> With Your Magnificent Figure";
    public override Func<string> Description => () => $"- You can drop a flashbang which blinds players around you\n{CommonAbilities}";
    public override InspectorResults InspectorResults => InspectorResults.DropsItems;

    public Grenadier(PlayerControl player) : base(player)
    {
        Alignment = Alignment.IntruderConceal;
        FlashedPlayers = new();
        FlashButton = new(this, "Flash", AbilityTypes.Targetless, "Secondary", HitFlash, CustomGameOptions.FlashCd, CustomGameOptions.FlashDur, Flash, StartFlash, UnFlash);
    }

    public void Flash()
    {
        foreach (var player in FlashedPlayers)
        {
            if (CustomPlayer.Local == player)
            {
                if (FlashButton.EffectTime > CustomGameOptions.FlashDur - 0.5f)
                {
                    var fade = (FlashButton.EffectTime - CustomGameOptions.FlashDur) * -2f;

                    if (ShouldPlayerBeBlinded(player))
                        HUD.FullScreen.color = Color32.Lerp(NormalVision, BlindVision, fade);
                    else if (ShouldPlayerBeDimmed(player))
                        HUD.FullScreen.color = Color32.Lerp(NormalVision, DimVision, fade);
                    else
                        HUD.FullScreen.color = NormalVision;
                }
                else if (FlashButton.EffectTime.IsInRange(0.5f, CustomGameOptions.FlashDur - 0.5f))
                {
                    if (ShouldPlayerBeBlinded(player))
                        HUD.FullScreen.color = BlindVision;
                    else if (ShouldPlayerBeDimmed(player))
                        HUD.FullScreen.color = DimVision;
                    else
                        HUD.FullScreen.color = NormalVision;
                }
                else if (FlashButton.EffectTime < 0.5f)
                {
                    var fade2 = (FlashButton.EffectTime * -2.0f) + 1.0f;

                    if (ShouldPlayerBeBlinded(player))
                        HUD.FullScreen.color = Color32.Lerp(BlindVision, NormalVision, fade2);
                    else if (ShouldPlayerBeDimmed(player))
                        HUD.FullScreen.color = Color32.Lerp(DimVision, NormalVision, fade2);
                    else
                        HUD.FullScreen.color = NormalVision;
                }
                else
                {
                    SetFullScreenHUD();
                    FlashButton.EffectTime = 0f;
                }

                if (Map)
                    Map.Close();

                if (Minigame.Instance)
                    Minigame.Instance.Close();
            }
        }
    }

    private bool ShouldPlayerBeDimmed(PlayerControl player) => ((player.Is(Faction) && Faction is Faction.Intruder or Faction.Syndicate) || (player.Is(SubFaction) && SubFaction !=
        SubFaction.None) || player.Data.IsDead) && Meeting;

    private bool ShouldPlayerBeBlinded(PlayerControl player) => !((player.Is(Faction) && Faction is Faction.Intruder or Faction.Syndicate) || (player.Is(SubFaction) && SubFaction !=
        SubFaction.None) || player.Data.IsDead || Meeting);

    public void UnFlash()
    {
        FlashedPlayers.Clear();
        SetFullScreenHUD();
    }

    public void HitFlash()
    {
        CallRpc(CustomRPC.Action, ActionsRPC.LayerAction1, FlashButton);
        FlashButton.Begin();
    }

    public bool Condition()
    {
        var system = ShipStatus.Instance.Systems[SystemTypes.Sabotage].Cast<SabotageSystemType>();
        var dummyActive = system?.dummy?.IsActive;
        var sabActive = system?.specials?.Any(s => s.IsActive);
        return dummyActive == false && sabActive == false;
    }

    public void StartFlash() => FlashedPlayers = GetClosestPlayers(Player.transform.position, CustomGameOptions.FlashRadius);

    public override void UpdateHud(HudManager __instance)
    {
        base.UpdateHud(__instance);
        FlashButton.Update2("FLASH", condition: Condition());
    }
}