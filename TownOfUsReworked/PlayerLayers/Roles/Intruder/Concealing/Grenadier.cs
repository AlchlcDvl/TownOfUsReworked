namespace TownOfUsReworked.PlayerLayers.Roles;

public class Grenadier : Intruder
{
    public CustomButton FlashButton { get; set; }
    public List<PlayerControl> FlashedPlayers { get; set; }

    public override UColor Color => ClientGameOptions.CustomIntColors ? CustomColorManager.Grenadier : CustomColorManager.Intruder;
    public override string Name => "Grenadier";
    public override LayerEnum Type => LayerEnum.Grenadier;
    public override Func<string> StartText => () => "Blind The <color=#8CFFFFFF>Crew</color> With Your Magnificent Figure";
    public override Func<string> Description => () => $"- You can drop a flashbang which blinds players around you\n{CommonAbilities}";

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
                        HUD.FullScreen.color = Color32.Lerp(CustomColorManager.NormalVision, CustomColorManager.BlindVision, fade);
                    else if (ShouldPlayerBeDimmed(player))
                        HUD.FullScreen.color = Color32.Lerp(CustomColorManager.NormalVision, CustomColorManager.DimVision, fade);
                    else
                        HUD.FullScreen.color = CustomColorManager.NormalVision;
                }
                else if (FlashButton.EffectTime.IsInRange(0.5f, CustomGameOptions.FlashDur - 0.5f))
                {
                    if (ShouldPlayerBeBlinded(player))
                        HUD.FullScreen.color = CustomColorManager.BlindVision;
                    else if (ShouldPlayerBeDimmed(player))
                        HUD.FullScreen.color = CustomColorManager.DimVision;
                    else
                        HUD.FullScreen.color = CustomColorManager.NormalVision;
                }
                else if (FlashButton.EffectTime < 0.5f)
                {
                    var fade2 = (FlashButton.EffectTime * -2) + 1;

                    if (ShouldPlayerBeBlinded(player))
                        HUD.FullScreen.color = Color32.Lerp(CustomColorManager.BlindVision, CustomColorManager.NormalVision, fade2);
                    else if (ShouldPlayerBeDimmed(player))
                        HUD.FullScreen.color = Color32.Lerp(CustomColorManager.DimVision, CustomColorManager.NormalVision, fade2);
                    else
                        HUD.FullScreen.color = CustomColorManager.NormalVision;
                }

                if (MapPatch.MapActive)
                    Map.Close();

                if (ActiveTask)
                    ActiveTask.Close();
            }
        }
    }

    private bool ShouldPlayerBeDimmed(PlayerControl player) => ((player.Is(Faction) && Faction is Faction.Intruder or Faction.Syndicate) || (player.Is(SubFaction) && SubFaction !=
        SubFaction.None) || player.Data.IsDead) && !Meeting;

    private bool ShouldPlayerBeBlinded(PlayerControl player) => !ShouldPlayerBeDimmed(player) && !Meeting;

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

    public void StartFlash() => FlashedPlayers = GetClosestPlayers(Player.transform.position, CustomGameOptions.FlashRadius);

    public override void UpdateHud(HudManager __instance)
    {
        base.UpdateHud(__instance);
        FlashButton.Update2("FLASH", condition: !Ship.Systems[SystemTypes.Sabotage].Cast<SabotageSystemType>().AnyActive && !CustomGameOptions.SaboFlash);
    }
}