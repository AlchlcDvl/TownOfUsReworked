namespace TownOfUsReworked.PlayerLayers.Roles;

public class Concealer : Syndicate
{
    public CustomButton ConcealButton { get; set; }
    public PlayerControl ConcealedPlayer { get; set; }
    public CustomMenu ConcealMenu { get; set; }

    public override Color Color => ClientGameOptions.CustomSynColors ? Colors.Concealer : Colors.Syndicate;
    public override string Name => "Concealer";
    public override LayerEnum Type => LayerEnum.Concealer;
    public override Func<string> StartText => () => "Turn The <color=#8CFFFFFF>Crew</color> Invisible For Some Chaos";
    public override Func<string> Description => () => $"- You can turn {(HoldsDrive ? "everyone" : "a player")} invisible\n{CommonAbilities}";

    public Concealer(PlayerControl player) : base(player)
    {
        Alignment = Alignment.SyndicateDisrup;
        ConcealMenu = new(Player, Click, Exception1);
        ConcealedPlayer = null;
        ConcealButton = new(this, "Conceal", AbilityTypes.Targetless, "Secondary", HitConceal, CustomGameOptions.ConcealCd, CustomGameOptions.ConcealDur, (CustomButton.EffectVoid)Conceal,
            UnConceal);
    }

    public void Conceal()
    {
        if (HoldsDrive)
            CustomPlayer.AllPlayers.ForEach(x => Invis(x, CustomPlayer.Local.Is(Faction.Syndicate)));
        else
            Invis(ConcealedPlayer, CustomPlayer.Local.Is(Faction.Syndicate));
    }

    public void UnConceal()
    {
        if (HoldsDrive)
            DefaultOutfitAll();
        else
            DefaultOutfit(ConcealedPlayer);

        ConcealedPlayer = null;
    }

    public void Click(PlayerControl player)
    {
        var interact = Interact(Player, player);

        if (interact.AbilityUsed)
            ConcealedPlayer = player;
        else if (interact.Reset)
            ConcealButton.StartCooldown();
        else if (interact.Protected)
            ConcealButton.StartCooldown(CooldownType.GuardianAngel);
    }

    public void HitConceal()
    {
        if (HoldsDrive)
        {
            CallRpc(CustomRPC.Action, ActionsRPC.LayerAction1, ConcealButton);
            ConcealButton.Begin();
        }
        else if (ConcealedPlayer == null)
            ConcealMenu.Open();
        else
        {
            CallRpc(CustomRPC.Action, ActionsRPC.LayerAction1, ConcealButton, ConcealedPlayer);
            ConcealButton.Begin();
        }
    }

    public bool Exception1(PlayerControl player) => player == ConcealedPlayer || player == Player || (player.Is(Faction) && !CustomGameOptions.ConcealMates && Faction is Faction.Intruder or
        Faction.Syndicate) || (player.Is(SubFaction) && SubFaction != SubFaction.None && !CustomGameOptions.ConcealMates);

    public override void UpdateHud(HudManager __instance)
    {
        base.UpdateHud(__instance);
        ConcealButton.Update2(ConcealedPlayer == null && !HoldsDrive ? "SET TARGET" : "CONCEAL");

        if (Input.GetKeyDown(KeyCode.Backspace))
        {
            if (ConcealedPlayer != null && !HoldsDrive && !ConcealButton.EffectActive)
                ConcealedPlayer = null;

            LogInfo("Removed a target");
        }
    }

    public override void TryEndEffect() => ConcealButton.Update3(!HoldsDrive && ConcealedPlayer != null && ConcealedPlayer.HasDied());

    public override void ReadRPC(MessageReader reader)
    {
        if (!HoldsDrive)
            ConcealedPlayer = reader.ReadPlayer();
    }
}