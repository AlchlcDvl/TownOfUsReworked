namespace TownOfUsReworked.PlayerLayers.Abilities;

public class Bullseye : Assassin
{
    public override LayerEnum Type => LayerEnum.Bullseye;
}

public class Hitman : Assassin
{
    public override LayerEnum Type => LayerEnum.Hitman;
}

public class Slayer : Assassin
{
    public override LayerEnum Type => LayerEnum.Slayer;
}

public class Sniper : Assassin
{
    public override LayerEnum Type => LayerEnum.Sniper;
}

[HeaderOption(MultiMenu.LayerSubOptions)]
public abstract class Assassin : Ability, IGuesser
{
    [NumberOption(0, 15, 1, zeroIsInf: true)]
    public static Number AssassinKills = 0;

    [ToggleOption]
    public static bool AssassinMultiKill = false;

    [ToggleOption]
    public static bool AssassinGuessNeutralBenign = false;

    [ToggleOption]
    public static bool AssassinGuessNeutralEvil = false;

    [ToggleOption]
    public static bool AssassinGuessInvestigative = false;

    [ToggleOption]
    public static bool AssassinGuessApoc = false;

    [ToggleOption]
    public static bool AssassinGuessModifiers = false;

    [ToggleOption]
    public static bool AssassinGuessDispositions = false;

    [ToggleOption]
    public static bool AssassinGuessAbilities = false;

    public static int RemainingKills { get; set; }

    public CustomMeeting GuessMenu { get; set; }
    public CustomRolesMenu GuessingMenu { get; set; }

    public override UColor Color => ClientOptions.CustomAbColors ? CustomColorManager.Assassin : CustomColorManager.Ability;
    public override Func<string> Description => () => "- You can guess players mid-meetings";
    public override AttackEnum AttackVal => AttackEnum.Powerful;

    public override void Init()
    {
        GuessMenu = new(Player, "Guess", Guess, IsExempt, SetLists);
        GuessingMenu = new(Player, GuessPlayer);
    }

    public override void OnMeetingStart(MeetingHud __instance)
    {
        base.OnMeetingStart(__instance);
        GuessMenu.GenButtons(__instance, RemainingKills > 0);
    }

    public override void UpdateMeeting(MeetingHud __instance) => GuessMenu.Update(__instance);

    public override void VoteComplete(MeetingHud __instance)
    {
        GuessMenu.HideButtons();
        GuessingMenu.Close();
    }

    public override void ReadRPC(MessageReader reader) => MurderPlayer(reader.ReadPlayer(), reader.ReadEnum<LayerEnum>(), reader.ReadPlayer());

    private void SetLists()
    {
        GuessingMenu.Mapping.Clear();

        // Adds all the roles that have a non-zero chance of being in the game
        if ((!Player.Is(Faction.Crew) || !Player.Is(SubFaction.None)) && CrewSettings.CrewMax > 0 && CrewSettings.CrewMin > 0)
        {
            if (!Player.Is(Faction.Crew) || !Player.Is(SubFaction.None))
                GuessingMenu.Mapping.Add(LayerEnum.Crewmate);

            foreach (var layer in GetValuesFromTo(LayerEnum.Altruist, LayerEnum.Vigilante, x => x is not (LayerEnum.Revealer or LayerEnum.Crewmate)))
            {
                if (layer is LayerEnum.Coroner or LayerEnum.Detective or LayerEnum.Medium or LayerEnum.Operative or LayerEnum.Seer or LayerEnum.Sheriff or LayerEnum.Tracker &&
                    !AssassinGuessInvestigative)
                {
                    continue;
                }

                if (RoleGenManager.GetSpawnItem(layer).IsActive() || (layer == LayerEnum.Sheriff && !GuessingMenu.Mapping.Contains(LayerEnum.Sheriff) &&
                    GuessingMenu.Mapping.Contains(LayerEnum.Seer)) || (layer == LayerEnum.Seer && !GuessingMenu.Mapping.Contains(LayerEnum.Seer) &&
                    GuessingMenu.Mapping.Contains(LayerEnum.Mystic)))
                {
                    GuessingMenu.Mapping.Add(layer);
                }
            }
        }

        if ((!Player.Is(Faction.Intruder) || !Player.Is(SubFaction.None)) && !SyndicateSettings.AltImps && IntruderSettings.IntruderMax > 0 && IntruderSettings.IntruderMin > 0)
        {
            if (!Player.Is(Faction.Intruder) || !Player.Is(SubFaction.None))
                GuessingMenu.Mapping.Add(LayerEnum.Impostor);

            foreach (var layer in GetValuesFromTo(LayerEnum.Ambusher, LayerEnum.Wraith, x => x is not (LayerEnum.PromotedGodfather or LayerEnum.Ghoul or LayerEnum.Impostor)))
            {
                if (RoleGenManager.GetSpawnItem(layer).IsActive() || (layer == LayerEnum.Mafioso && !GuessingMenu.Mapping.Contains(LayerEnum.Mafioso) &&
                    GuessingMenu.Mapping.Contains(LayerEnum.Godfather)))
                {
                    GuessingMenu.Mapping.Add(layer);
                }
            }
        }

        if ((!Player.Is(Faction.Syndicate) || !Player.Is(SubFaction.None)) && SyndicateSettings.SyndicateCount > 0)
        {
            if (!Player.Is(Faction.Syndicate) || !Player.Is(SubFaction.None))
                GuessingMenu.Mapping.Add(LayerEnum.Anarchist);

            foreach (var layer in GetValuesFromTo(LayerEnum.Anarchist, LayerEnum.Warper, x => x is not (LayerEnum.PromotedRebel or LayerEnum.Anarchist or LayerEnum.Banshee)))
            {
                if (RoleGenManager.GetSpawnItem(layer).IsActive() || (layer == LayerEnum.Sidekick && !GuessingMenu.Mapping.Contains(LayerEnum.Sidekick) &&
                    GuessingMenu.Mapping.Contains(LayerEnum.Rebel)))
                {
                    GuessingMenu.Mapping.Add(layer);
                }
            }
        }

        if (NeutralSettings.NeutralMax > 0 && NeutralSettings.NeutralMin > 0 && !(Player.Is(Faction.Neutral) && NeutralSettings.NoSolo == NoSolo.AllNeutrals))
        {
            var nks = new List<LayerEnum>() { LayerEnum.Arsonist, LayerEnum.Glitch, LayerEnum.SerialKiller, LayerEnum.Juggernaut, LayerEnum.Murderer, LayerEnum.Cryomaniac, LayerEnum.Werewolf };

            foreach (var layer in nks)
            {
                if (RoleGenManager.GetSpawnItem(layer).IsActive() && (!Player.Is(layer) || NeutralSettings.NoSolo == NoSolo.Never))
                    GuessingMenu.Mapping.Add(layer);
            }

            if (RoleGenManager.GetSpawnItem(LayerEnum.Plaguebearer).IsActive() && !Player.Is(Alignment.Harbinger) && !Player.Is(Alignment.Apocalypse))
            {
                GuessingMenu.Mapping.Add(LayerEnum.Plaguebearer);

                if (AssassinGuessApoc)
                    GuessingMenu.Mapping.Add(LayerEnum.Pestilence);
            }

            if (RoleGenManager.GetSpawnItem(LayerEnum.Dracula).IsActive() && !Player.Is(SubFaction.Undead))
            {
                GuessingMenu.Mapping.Add(LayerEnum.Dracula);
                GuessingMenu.Mapping.Add(LayerEnum.Undead);
            }

            if (RoleGenManager.GetSpawnItem(LayerEnum.Jackal).IsActive() && !Player.Is(SubFaction.Cabal))
            {
                GuessingMenu.Mapping.Add(LayerEnum.Jackal);
                GuessingMenu.Mapping.Add(LayerEnum.Cabal);
            }

            if (RoleGenManager.GetSpawnItem(LayerEnum.Necromancer).IsActive() && !Player.Is(SubFaction.Reanimated))
            {
                GuessingMenu.Mapping.Add(LayerEnum.Necromancer);
                GuessingMenu.Mapping.Add(LayerEnum.Reanimated);
            }

            if (RoleGenManager.GetSpawnItem(LayerEnum.Whisperer).IsActive() && !Player.Is(SubFaction.Cult))
            {
                GuessingMenu.Mapping.Add(LayerEnum.Whisperer);
                GuessingMenu.Mapping.Add(LayerEnum.Cult);
            }

            // Add certain Neutral roles if enabled
            if (AssassinGuessNeutralBenign)
            {
                var nbs = new List<LayerEnum>() { LayerEnum.Amnesiac, LayerEnum.GuardianAngel, LayerEnum.Survivor, LayerEnum.Thief };

                foreach (var layer in nbs)
                {
                    if (RoleGenManager.GetSpawnItem(layer).IsActive() || (layer == LayerEnum.Survivor && GuessingMenu.Mapping.Contains(LayerEnum.GuardianAngel) &&
                        !GuessingMenu.Mapping.Contains(LayerEnum.Survivor)) || (layer  == LayerEnum.Thief && GuessingMenu.Mapping.Contains(LayerEnum.Amnesiac) &&
                        !GuessingMenu.Mapping.Contains(LayerEnum.Thief)))
                    {
                        GuessingMenu.Mapping.Add(layer);
                    }
                }
            }

            if (AssassinGuessNeutralEvil)
            {
                var nes = new List<LayerEnum>() { LayerEnum.Cannibal, LayerEnum.Executioner, LayerEnum.Guesser, LayerEnum.BountyHunter, LayerEnum.Troll, LayerEnum.Actor, LayerEnum.Jester };

                foreach (var layer in nes)
                {
                    if (RoleGenManager.GetSpawnItem(layer).IsActive() || (layer == LayerEnum.Troll && GuessingMenu.Mapping.Contains(LayerEnum.BountyHunter) &&
                        !GuessingMenu.Mapping.Contains(LayerEnum.Troll)) || (layer == LayerEnum.Actor && GuessingMenu.Mapping.Contains(LayerEnum.Guesser) &&
                        !GuessingMenu.Mapping.Contains(LayerEnum.Actor)) || (layer == LayerEnum.Jester && GuessingMenu.Mapping.Contains(LayerEnum.Executioner) &&
                        !GuessingMenu.Mapping.Contains(LayerEnum.Jester)))
                    {
                        GuessingMenu.Mapping.Add(layer);
                    }
                }
            }
        }

        // Add Modifiers if enabled
        if (AssassinGuessModifiers)
        {
            var mods = new List<LayerEnum>() { LayerEnum.Bait, LayerEnum.Diseased, LayerEnum.Professional, LayerEnum.VIP };

            foreach (var layer in mods)
            {
                if (RoleGenManager.GetSpawnItem(layer).IsActive())
                    GuessingMenu.Mapping.Add(layer);
            }
        }

        // Add Dispositions if enabled
        if (AssassinGuessDispositions)
        {
            foreach (var layer in GetValuesFromTo(LayerEnum.Allied, LayerEnum.Traitor))
            {
                if (RoleGenManager.GetSpawnItem(layer).IsActive())
                {
                    if (Player.Is(layer) && ((layer is LayerEnum.Lovers or LayerEnum.Rivals or LayerEnum.Linked or LayerEnum.Mafia) || (layer == LayerEnum.Corrupted &&
                        Corrupted.AllCorruptedWin)))
                    {
                        continue;
                    }

                    GuessingMenu.Mapping.Add(layer);
                }
            }
        }

        // Add Abilities if enabled
        if (AssassinGuessAbilities)
        {
            foreach (var layer in GetValuesFromTo(LayerEnum.Bullseye, LayerEnum.Underdog))
            {
                if (RoleGenManager.GetSpawnItem(layer).IsActive())
                {
                    if ((layer == LayerEnum.Hitman && Player.Is(Faction.Intruder)) || (layer == LayerEnum.Sniper && Player.Is(Faction.Syndicate)) || (layer is LayerEnum.Bullseye or
                        LayerEnum.Snitch && Player.Is(Faction.Crew)))
                    {
                        continue;
                    }

                    GuessingMenu.Mapping.Add(layer);
                }
            }
        }
    }

    private void GuessPlayer(ShapeshifterPanel panel, PlayerControl player, LayerEnum guess)
    {
        if (Dead || Meeting().state == MeetingHud.VoteStates.Discussion || !panel || RemainingKills <= 0)
            return;

        if (GuessingMenu.SelectedPanel != panel)
        {
            if (GuessingMenu.SelectedPanel)
                GuessingMenu.SelectedPanel.Background.color = UColor.white;

            GuessingMenu.SelectedPanel = panel;
            GuessingMenu.SelectedPanel.Background.color = LayerDictionary[guess].Color.Alternate(0.4f);
        }
        else
        {
            var layerflag = player.GetLayers().Any(x => x.Type == guess);
            var subfactionflag = $"{player.GetSubFaction()}" == $"{guess}";
            var framedflag = player.IsFramed();

            if (guess != LayerEnum.Actor && player.TryGetLayer<Actor>(out var actor) && actor.PretendRoles.Any(x => x.Type == guess))
            {
                actor.Guessed = true;
                CallRpc(CustomRPC.WinLose, WinLose.ActorWins, actor);

                if (!NeutralSettings.AvoidNeutralKingmakers)
                    RpcMurderPlayer(Player, guess, player);
            }

            var flag = layerflag || subfactionflag || framedflag;
            var toDie = flag ? player : Player;
            RpcMurderPlayer(toDie, guess, player);

            if (RemainingKills <= 0 || !AssassinMultiKill)
                GuessMenu.HideButtons();
            else
                GuessMenu.HideSingle(player.PlayerId);

            GuessingMenu.SelectedPanel = null;
        }
    }

    private bool IsExempt(PlayerVoteArea voteArea)
    {
        var player = PlayerByVoteArea(voteArea);
        return player.HasDied() || (voteArea.NameText.text.Contains('\n') && ((Player.GetFaction() != player.GetFaction()) || (Player.GetSubFaction() != Player.GetSubFaction()))) || Dead ||
            (player == Player && Local) || (Player.GetFaction() == player.GetFaction() && Player.GetFaction() != Faction.Crew) | RemainingKills <= 0 || (Player.GetSubFaction() ==
            player.GetSubFaction() && Player.GetSubFaction() != SubFaction.None) || Player.IsLinkedTo(player);
    }

    private void Guess(PlayerVoteArea voteArea, MeetingHud __instance)
    {
        if (__instance.state == MeetingHud.VoteStates.Discussion || IsExempt(voteArea))
            return;

        GuessingMenu.Open(PlayerByVoteArea(voteArea));
    }

    public void RpcMurderPlayer(PlayerControl player, LayerEnum guess, PlayerControl guessTarget)
    {
        MurderPlayer(player, guess, guessTarget);
        CallRpc(CustomRPC.Action, ActionsRPC.LayerAction, this, player, guess, guessTarget);
    }

    public void MurderPlayer(PlayerControl player, LayerEnum guess, PlayerControl guessTarget)
    {
        Spread(Player, guessTarget);
        var guessString = LayerDictionary[guess].Name;

        if (Local && player == Player)
            GuessMenu.HideButtons();

        if (player.TryGetLayer<Indomitable>(out var ind) && player != Player)
        {
            if (Local)
                Run("<#EC1C45FF>∮ Assassination ∮</color>", $"You failed to assassinate {guessTarget.name}!");
            else if (player.AmOwner)
                Run("<#EC1C45FF>∮ Assassination ∮</color>", $"Someone tried to assassinate you!");

            Flash(CustomColorManager.Indomitable);
            ind.AttemptedGuess = true;
        }

        if (Player == player && Player.TryGetLayer<Professional>(out var modifier) && !modifier.LifeUsed)
        {
            modifier.LifeUsed = true;
            GuessMenu.HideSingle(guessTarget.PlayerId);

            if (Local)
            {
                Flash(modifier.Color);
                Run("<#EC1C45FF>∮ Assassination ∮</color>", $"You incorrectly guessed {guessTarget.name} as {guessString} and lost a life!");
            }
            else if ((Player.GetFaction() == CustomPlayer.Local.GetFaction() && (Player.GetFaction() is Faction.Intruder or Faction.Syndicate)) || DeadSeeEverything())
                Run("<#EC1C45FF>∮ Assassination ∮</color>", $"{Player.name} incorrectly guessed {guessTarget.name} as {guessString} and lost a life!");

            return;
        }

        if (CanAttack(AttackVal, player.GetDefenseValue(Player)) || player == Player)
        {
            RemainingKills--;
            MarkMeetingDead(player, Player);

            if (Lovers.BothLoversDie && AmongUsClient.Instance.AmHost && player.TryGetLayer<Lovers>(out var lovers) && (!lovers.OtherLover.Is(Alignment.Apocalypse) || AssassinGuessApoc))
                RpcMurderPlayer(lovers.OtherLover, guess, guessTarget);
        }

        if (Local)
        {
            if (Player != player)
                Run("<#EC1C45FF>∮ Assassination ∮</color>", $"You guessed {guessTarget.name} as {guessString}!");
            else
                Run("<#EC1C45FF>∮ Assassination ∮</color>", $"You incorrectly guessed {guessTarget.name} as {guessString} and died!");
        }
        else if (Player != player && player.AmOwner)
            Run("<#EC1C45FF>∮ Assassination ∮</color>", $"{Player.name} guessed you as {guessString}!");
        else if ((Player.GetFaction() == CustomPlayer.Local.GetFaction() && (Player.GetFaction() is Faction.Intruder or Faction.Syndicate)) || DeadSeeEverything())
        {
            if (Player != player)
                Run("<#EC1C45FF>∮ Assassination ∮</color>", $"{Player.name} guessed {guessTarget.name} as {guessString}!");
            else
                Run("<#EC1C45FF>∮ Assassination ∮</color>", $"{Player.name} incorrectly guessed {guessTarget.name} as {guessString} and died!");
        }
        else
            Run("<#EC1C45FF>∮ Assassination ∮</color>", $"{player.name} has been assassinated!");
    }
}