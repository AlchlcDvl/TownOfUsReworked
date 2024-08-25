namespace TownOfUsReworked.PlayerLayers.Roles;

[HeaderOption(MultiMenu.LayerSubOptions)]
public class Tracker : Crew
{
    [NumberOption(MultiMenu.LayerSubOptions, 1, 15, 1)]
    public static int MaxTracks { get; set; } = 5;

    [NumberOption(MultiMenu.LayerSubOptions, 10f, 60f, 2.5f, Format.Time)]
    public static float TrackCd { get; set; } = 25f;

    [ToggleOption(MultiMenu.LayerSubOptions)]
    public static bool ResetOnNewRound { get; set; } = false;

    [NumberOption(MultiMenu.LayerSubOptions, 0f, 15f, 0.5f, Format.Time)]
    public static float UpdateInterval { get; set; } = 5f;

    public Dictionary<byte, CustomArrow> TrackerArrows { get; set; }
    public CustomButton TrackButton { get; set; }

    public override UColor Color => ClientOptions.CustomCrewColors ? CustomColorManager.Tracker : CustomColorManager.Crew;
    public override string Name => "Tracker";
    public override LayerEnum Type => LayerEnum.Tracker;
    public override Func<string> StartText => () => "Track Everyone's Movements";
    public override Func<string> Description => () => "- You can track players which creates arrows that update every now and then with the target's position";

    public override void Init()
    {
        BaseStart();
        TrackerArrows = [];
        Alignment = Alignment.CrewInvest;
        Data.Role.IntroSound = GetAudio("TrackerIntro");
        TrackButton = CreateButton(this, "TRACK", new SpriteName("Track"), AbilityTypes.Alive, KeybindType.ActionSecondary, (OnClick)Track, new Cooldown(TrackCd), MaxTracks,
            (PlayerBodyExclusion)Exception);
    }

    public void DestroyArrow(byte targetPlayerId)
    {
        TrackerArrows.FirstOrDefault(x => x.Key == targetPlayerId).Value?.Destroy();
        TrackerArrows.Remove(targetPlayerId);
    }

    public override void OnLobby()
    {
        base.OnLobby();
        TrackerArrows.Values.ToList().DestroyAll();
        TrackerArrows.Clear();
    }

    public bool Exception(PlayerControl player) => TrackerArrows.ContainsKey(player.PlayerId);

    public void Track()
    {
        var cooldown = Interact(Player, TrackButton.TargetPlayer);

        if (cooldown != CooldownType.Fail)
            TrackerArrows.Add(TrackButton.TargetPlayer.PlayerId, new(Player, TrackButton.TargetPlayer.GetPlayerColor(), UpdateInterval));

        TrackButton.StartCooldown(cooldown);
    }

    public override void UpdateHud(HudManager __instance)
    {
        base.UpdateHud(__instance);

        if (Dead)
            OnLobby();
        else
        {
            foreach (var pair in TrackerArrows)
            {
                var player = PlayerById(pair.Key);
                var body = BodyById(pair.Key);

                if (!player || player.Data.Disconnected || (player.Data.IsDead && !body))
                    DestroyArrow(pair.Key);
                else
                    pair.Value?.Update(player.Data.IsDead ? body.transform.position : player.transform.position, player.GetPlayerColor());
            }
        }
    }
}