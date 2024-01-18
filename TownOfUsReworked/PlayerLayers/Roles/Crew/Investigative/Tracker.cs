namespace TownOfUsReworked.PlayerLayers.Roles;

public class Tracker : Crew
{
    public Dictionary<byte, CustomArrow> TrackerArrows { get; set; }
    public CustomButton TrackButton { get; set; }

    public override UColor Color => ClientGameOptions.CustomCrewColors ? CustomColorManager.Tracker : CustomColorManager.Crew;
    public override string Name => "Tracker";
    public override LayerEnum Type => LayerEnum.Tracker;
    public override Func<string> StartText => () => "Track Everyone's Movements";
    public override Func<string> Description => () => "- You can track players which creates arrows that update every now and then with the target's position";

    public Tracker(PlayerControl player) : base(player)
    {
        TrackerArrows = new();
        Alignment = Alignment.CrewInvest;
        TrackButton = new(this, "Track", AbilityTypes.Alive, "ActionSecondary", Track, CustomGameOptions.TrackCd, Exception, CustomGameOptions.MaxTracks);
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
            TrackerArrows.Add(TrackButton.TargetPlayer.PlayerId, new(Player, TrackButton.TargetPlayer.GetPlayerColor(), CustomGameOptions.UpdateInterval));

        TrackButton.StartCooldown(cooldown);
    }

    public override void UpdateHud(HudManager __instance)
    {
        base.UpdateHud(__instance);
        TrackButton.Update2("TRACK");

        if (IsDead)
            OnLobby();
        else
        {
            foreach (var pair in TrackerArrows)
            {
                var player = PlayerById(pair.Key);
                var body = BodyById(pair.Key);

                if (player == null || player.Data.Disconnected || (player.Data.IsDead && body == null))
                    DestroyArrow(pair.Key);
                else
                    pair.Value?.Update(player.Data.IsDead ? body.transform.position : player.transform.position, player.GetPlayerColor());
            }
        }
    }
}