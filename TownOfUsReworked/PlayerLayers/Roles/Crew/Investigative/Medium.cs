namespace TownOfUsReworked.PlayerLayers.Roles;

public class Medium : Crew
{
    public Dictionary<byte, CustomArrow> MediateArrows { get; set; }
    public CustomButton MediateButton { get; set; }
    //public CustomButton SeanceButton { get; set; }
    public List<byte> MediatedPlayers { get; set; }

    public override Color Color => ClientGameOptions.CustomCrewColors ? Colors.Medium : Colors.Crew;
    public override string Name => "Medium";
    public override LayerEnum Type => LayerEnum.Medium;
    public override Func<string> StartText => () => "<size=80%>Spooky Scary Ghosties Send Shivers Down Your Spine</size>";
    public override Func<string> Description => () => "- You can mediate which makes ghosts visible to you" + (CustomGameOptions.ShowMediumToDead == ShowMediumToDead.No ? "" : ("\n- When" +
        " mediating, dead players will be able to see you"));
    public override InspectorResults InspectorResults => InspectorResults.NewLens;

    public Medium(PlayerControl player) : base(player)
    {
        MediatedPlayers = new();
        MediateArrows = new();
        Alignment = Alignment.CrewInvest;
        MediateButton = new(this, "Mediate", AbilityTypes.Targetless, "ActionSecondary", Mediate, CustomGameOptions.MediateCd);
        //SeanceButton = new(this, "Seance", AbilityTypes.Targetless, "ActionSecondary", Seance, CustomGameOptions.SeanceCd, true);
    }

    //private static void Seance() { Currently blank, gonna work on this later }

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
        MediateButton.Update2("MEDIATE");
        //SeanceButton.Update2("SEANCE");

        if (!IsDead)
        {
            foreach (var player in CustomPlayer.AllPlayers)
            {
                if (MediateArrows.ContainsKey(player.PlayerId))
                {
                    MediateArrows[player.PlayerId]?.Update(player.transform.position, player.GetPlayerColor(false, CustomGameOptions.ShowMediatePlayer));
                    player.Visible = true;

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
        MediateButton.StartCooldown(CooldownType.Reset);
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
                    CallRpc(CustomRPC.Action, ActionsRPC.LayerAction2, this, dead.PlayerId);

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
                    CallRpc(CustomRPC.Action, ActionsRPC.LayerAction2, this, dead.PlayerId);
            }
        }
    }

    public override void ReadRPC(MessageReader reader)
    {
        var playerid2 = reader.ReadByte();
        MediatedPlayers.Add(playerid2);

        if (CustomPlayer.Local.PlayerId == playerid2 || (CustomPlayer.LocalCustom.IsDead && CustomGameOptions.ShowMediumToDead == ShowMediumToDead.AllDead))
            LocalRole.DeadArrows.Add(PlayerId, new(CustomPlayer.Local, Colors.Retributionist));
    }
}