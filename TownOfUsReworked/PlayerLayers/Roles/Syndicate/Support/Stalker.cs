namespace TownOfUsReworked.PlayerLayers.Roles;

public class Stalker : Syndicate
{
    public Dictionary<byte, CustomArrow> StalkerArrows { get; set; }
    public CustomButton StalkButton { get; set; }

    public override Color Color => ClientGameOptions.CustomSynColors ? Colors.Stalker : Colors.Syndicate;
    public override string Name => "Stalker";
    public override LayerEnum Type => LayerEnum.Stalker;
    public override Func<string> StartText => () => "Stalk Everyone To Monitor Their Movements";
    public override Func<string> Description => () => $"- You always know where your targets are" + (HoldsDrive ? "\n- Camouflages do not stop you seeing who's where" : "") + "\n" +
        CommonAbilities;
    public override InspectorResults InspectorResults => InspectorResults.TracksOthers;

    public Stalker(PlayerControl player) : base(player)
    {
        StalkerArrows = new();
        Alignment = Alignment.SyndicateSupport;
        StalkButton = new(this, "Stalk", AbilityTypes.Target, "ActionSecondary", Stalk, CustomGameOptions.StalkCd, Exception1);
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
        var interact = Interact(Player, StalkButton.TargetPlayer);
        var cooldown = CooldownType.Reset;

        if (interact.AbilityUsed)
            StalkerArrows.Add(StalkButton.TargetPlayer.PlayerId, new(Player, StalkButton.TargetPlayer.GetPlayerColor(!HoldsDrive)));

        if (interact.Protected)
            cooldown = CooldownType.GuardianAngel;

        StalkButton.StartCooldown(cooldown);
    }

    public bool Exception1(PlayerControl player) => StalkerArrows.ContainsKey(player.PlayerId);

    public override void UpdateHud(HudManager __instance)
    {
        base.UpdateHud(__instance);
        StalkButton.Update2("STALK", !HoldsDrive);

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