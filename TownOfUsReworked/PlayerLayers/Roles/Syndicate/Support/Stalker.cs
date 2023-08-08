namespace TownOfUsReworked.PlayerLayers.Roles;

public class Stalker : Syndicate
{
    public Dictionary<byte, CustomArrow> StalkerArrows { get; set; }
    public DateTime LastStalked { get; set; }
    public CustomButton StalkButton { get; set; }

    public override Color32 Color => ClientGameOptions.CustomSynColors ? Colors.Stalker : Colors.Syndicate;
    public override string Name => "Stalker";
    public override LayerEnum Type => LayerEnum.Stalker;
    public override Func<string> StartText => () => "Stalk Everyone To Monitor Their Movements";
    public override Func<string> Description => () => $"- You always know where your targets are\n{CommonAbilities}";
    public override InspectorResults InspectorResults => InspectorResults.TracksOthers;
    public float Timer => ButtonUtils.Timer(Player, LastStalked, CustomGameOptions.StalkCd);

    public Stalker(PlayerControl player) : base(player)
    {
        StalkerArrows = new();
        RoleAlignment = RoleAlignment.SyndicateSupport;
        StalkButton = new(this, "Stalk", AbilityTypes.Direct, "ActionSecondary", Stalk, Exception1);
    }

    public void DestroyArrow(byte targetPlayerId)
    {
        StalkerArrows.FirstOrDefault(x => x.Key == targetPlayerId).Value?.Destroy();
        StalkerArrows.Remove(targetPlayerId);
    }

    public override void OnLobby()
    {
        base.OnLobby();
        StalkerArrows.Values.ToList().DestroyAll();
        StalkerArrows.Clear();
    }

    public void Stalk()
    {
        if (IsTooFar(Player, StalkButton.TargetPlayer) || Timer != 0f)
            return;

        var interact = Interact(Player, StalkButton.TargetPlayer);

        if (interact[3])
            StalkerArrows.Add(StalkButton.TargetPlayer.PlayerId, new(Player, StalkButton.TargetPlayer.GetPlayerColor(!HoldsDrive)));

        if (interact[0])
            LastStalked = DateTime.UtcNow;
        else if (interact[1])
            LastStalked.AddSeconds(CustomGameOptions.ProtectKCReset);
    }

    public bool Exception1(PlayerControl player) => StalkerArrows.ContainsKey(player.PlayerId);

    public override void UpdateHud(HudManager __instance)
    {
        base.UpdateHud(__instance);
        StalkButton.Update("STALK", Timer, CustomGameOptions.StalkCd, true, !HoldsDrive);

        if (IsDead)
            OnLobby();
        else
        {
            foreach (var pair in StalkerArrows)
            {
                var player = PlayerById(pair.Key);
                var body = BodyById(pair.Key);

                if (player == null || player.Data.Disconnected || (player.Data.IsDead && !body))
                    DestroyArrow(pair.Key);
                else
                    pair.Value?.Update(player.Data.IsDead ? body.transform.position : player.transform.position, player.GetPlayerColor(!HoldsDrive));
            }

            if (HoldsDrive)
            {
                foreach (var player in CustomPlayer.AllPlayers)
                {
                    if (!StalkerArrows.ContainsKey(player.PlayerId))
                        StalkerArrows.Add(player.PlayerId, new(Player, player.GetPlayerColor(false)));
                }
            }
        }
    }
}