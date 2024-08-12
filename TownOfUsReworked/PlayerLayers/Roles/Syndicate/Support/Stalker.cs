namespace TownOfUsReworked.PlayerLayers.Roles;

[HeaderOption(MultiMenu2.LayerSubOptions)]
public class Stalker : Syndicate
{
    public Dictionary<byte, CustomArrow> StalkerArrows { get; set; }
    public CustomButton StalkButton { get; set; }

    public override UColor Color => ClientOptions.CustomSynColors ? CustomColorManager.Stalker : CustomColorManager.Syndicate;
    public override string Name => "Stalker";
    public override LayerEnum Type => LayerEnum.Stalker;
    public override Func<string> StartText => () => "Stalk Everyone To Monitor Their Movements";
    public override Func<string> Description => () => $"- You always know where your targets are" + (HoldsDrive ? "\n- Camouflages do not stop you seeing who's where" : "") + "\n" +
        CommonAbilities;

    public override void Init()
    {
        BaseStart();
        StalkerArrows = [];
        Alignment = Alignment.SyndicateSupport;
        StalkButton = CreateButton(this, new SpriteName("Stalk"), AbilityTypes.Alive, KeybindType.ActionSecondary, (OnClick)Stalk, new Cooldown(CustomGameOptions.StalkCd), "STALK",
            (PlayerBodyExclusion)Exception1, (UsableFunc)Usable);
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
        var cooldown = Interact(Player, StalkButton.TargetPlayer);

        if (cooldown != CooldownType.Fail)
            StalkerArrows.Add(StalkButton.TargetPlayer.PlayerId, new(Player, StalkButton.TargetPlayer.GetPlayerColor(!HoldsDrive)));

        StalkButton.StartCooldown(cooldown);
    }

    public bool Exception1(PlayerControl player) => StalkerArrows.ContainsKey(player.PlayerId);

    public bool Usable() => !HoldsDrive;

    public override void UpdateHud(HudManager __instance)
    {
        base.UpdateHud(__instance);

        if (Dead && StalkerArrows.Count > 0)
            OnLobby();
        else
        {
            foreach (var pair in StalkerArrows)
            {
                var player = PlayerById(pair.Key);
                var body = BodyById(pair.Key);

                if (!player || player.Data.Disconnected || (player.Data.IsDead && !body))
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