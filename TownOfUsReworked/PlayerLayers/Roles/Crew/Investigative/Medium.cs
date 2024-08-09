namespace TownOfUsReworked.PlayerLayers.Roles;

public class Medium : Crew
{
    public Dictionary<byte, CustomArrow> MediateArrows { get; set; }
    public CustomButton MediateButton { get; set; }
    // public CustomButton SeanceButton { get; set; }
    public List<byte> MediatedPlayers { get; set; }

    public override UColor Color => ClientOptions.CustomCrewColors ? CustomColorManager.Medium : CustomColorManager.Crew;
    public override string Name => "Medium";
    public override LayerEnum Type => LayerEnum.Medium;
    public override Func<string> StartText => () => "<size=80%>Spooky Scary Ghosties Send Shivers Down Your Spine</size>";
    public override Func<string> Description => () => "- You can mediate which makes ghosts visible to you" + (CustomGameOptions.ShowMediumToDead == ShowMediumToDead.No ? "" : ("\n- When" +
        " mediating, dead players will be able to see you"));

    public override void Init()
    {
        BaseStart();
        MediatedPlayers = [];
        MediateArrows = [];
        Alignment = Alignment.CrewInvest;
        MediateButton = CreateButton(this, "MEDIATE", new SpriteName("Mediate"), AbilityTypes.Targetless, KeybindType.ActionSecondary, (OnClick)Mediate,
            new Cooldown(CustomGameOptions.MediateCd));/*
        SeanceButton = CreateButton(this, "SEANCE", new SpriteName("Seance"), AbilityTypes.Targetless, KeybindType.ActionSecondary, (OnClick)Seance, new Cooldown(CustomGameOptions.SeanceCd),
            new PostDeath(true));*/
    }

    // private void Seance() { Currently blank, gonna work on this later }

    public override void OnLobby()
    {
        base.OnLobby();
        MediateArrows.Values.ToList().DestroyAll();
        MediateArrows.Clear();
        MediatedPlayers.Clear();
    }

    public override void UpdateHud(HudManager __instance)
    {
        base.UpdateHud(__instance);

        if (!Dead)
        {
            foreach (var player in CustomPlayer.AllPlayers)
            {
                if (MediateArrows.ContainsKey(player.PlayerId))
                {
                    MediateArrows[player.PlayerId]?.Update(player.transform.position, player.GetPlayerColor(false, CustomGameOptions.ShowMediatePlayer));

                    if (!CustomGameOptions.ShowMediatePlayer)
                    {
                        player.SetOutfit(CustomPlayerOutfitType.Camouflage, BlankOutfit(player));
                        PlayerMaterial.SetColors(UColor.grey, player.MyRend());
                    }
                }
            }
        }
    }

    public void Mediate()
    {
        MediateButton.StartCooldown();
        var playersDead = KilledPlayers.GetRange(0, KilledPlayers.Count);

        if (playersDead.Count == 0)
            return;

        if (CustomGameOptions.DeadRevealed != DeadRevealed.Random)
        {
            if (CustomGameOptions.DeadRevealed == DeadRevealed.Newest)
                playersDead.Reverse();

            foreach (var dead in playersDead)
            {
                if (AllBodies.Any(x => x.ParentId == dead.PlayerId && !MediateArrows.ContainsKey(x.ParentId)))
                {
                    MediateArrows.Add(dead.PlayerId, new(Player, Color));
                    MediatedPlayers.Add(dead.PlayerId);
                    CallRpc(CustomRPC.Action, ActionsRPC.LayerAction, this, dead.PlayerId);

                    if (CustomGameOptions.DeadRevealed != DeadRevealed.All)
                        break;
                }
            }
        }
        else
        {
            var dead = playersDead.Random();

            if (AllBodies.Any(x => x.ParentId == dead.PlayerId && !MediateArrows.ContainsKey(x.ParentId)))
            {
                MediateArrows.Add(dead.PlayerId, new(Player, Color));
                MediatedPlayers.Add(dead.PlayerId);
                    CallRpc(CustomRPC.Action, ActionsRPC.LayerAction, this, dead.PlayerId);
            }
        }
    }

    public override void ReadRPC(MessageReader reader)
    {
        var playerid2 = reader.ReadByte();
        MediatedPlayers.Add(playerid2);

        if (CustomPlayer.Local.PlayerId == playerid2 || (CustomPlayer.LocalCustom.Dead && CustomGameOptions.ShowMediumToDead == ShowMediumToDead.AllDead))
            LocalRole.DeadArrows.Add(PlayerId, new(CustomPlayer.Local, Color));
    }
}