namespace TownOfUsReworked.PlayerLayers;

[HarmonyPatch(typeof(EndGameManager), nameof(EndGameManager.Start))]
public static class Outro
{
    public static void Postfix(EndGameManager __instance)
    {
        if (!SoundEffects.ContainsKey("CrewWin"))
            SoundEffects.Add("CrewWin", __instance.CrewStinger);

        if (!SoundEffects.ContainsKey("IntruderWin"))
            SoundEffects.Add("IntruderWin", __instance.ImpostorStinger);

        if (!SoundEffects.ContainsKey("Stalemate"))
            SoundEffects.Add("Stalemate", __instance.DisconnectStinger);

        if (!GameHasEnded)
            return;

        var text = UObject.Instantiate(__instance.WinText);
        SoundManager.Instance.StopSound(__instance.ImpostorStinger);
        Play("IntruderWin");

        foreach (var player in UObject.FindObjectsOfType<PoolablePlayer>())
        {
            var role = Role.AllRoles.Find(x => x.PlayerName == player.NameText().text);
            player.NameText().text = $"{role.ColorString}<size=75%>{role}</size>\n<size=90%>{player.NameText().text}</size></color>";
        }

        if (PlayerLayer.NobodyWins)
        {
            __instance.BackgroundBar.material.color = Colors.Stalemate;
            text.text = "Stalemate";
            text.color = Colors.Stalemate;
            Stop("IntruderWin");
            Play("Stalemate");
        }
        else if (Role.SyndicateWin)
        {
            var role = Role.AllRoles.Find(x => x.Faction == Faction.Syndicate && Role.SyndicateWin);

            if (role == null)
                return;

            __instance.BackgroundBar.material.color = role.FactionColor;
            text.text = "The Syndicate Wins";
            text.color = role.FactionColor;
        }
        else if (Role.IntruderWin)
        {
            var role = Role.AllRoles.Find(x => x.Faction == Faction.Intruder);

            if (role == null)
                return;

            __instance.BackgroundBar.material.color = role.FactionColor;
            text.text = "Intruders Win";
            text.color = role.FactionColor;
        }
        else if (Role.AllNeutralsWin)
        {
            var role = Role.AllRoles.Find(x => x.Faction == Faction.Neutral && Role.AllNeutralsWin);

            if (role == null)
                return;

            __instance.BackgroundBar.material.color = role.FactionColor;
            text.text = "Neutrals Win";
            text.color = role.FactionColor;
        }
        else if (Role.CrewWin)
        {
            var role = Role.AllRoles.Find(x => x.Faction == Faction.Crew && Role.CrewWin);

            if (role == null)
                return;

            __instance.BackgroundBar.material.color = role.FactionColor;
            text.text = "Crew Wins";
            text.color = role.FactionColor;
            Stop("IntruderWin");
            Play("CrewWin");
        }
        else if (Role.NKWins)
        {
            var role = Role.AllRoles.Find(x => x.RoleAlignment == RoleAlignment.NeutralKill && x.Winner);

            if (role == null)
                return;

            __instance.BackgroundBar.material.color = Colors.Alignment;
            text.text = "Neutral Killers Win";
            text.color = Colors.Alignment;
        }
        else if (Role.InfectorsWin)
        {
            var role = Role.AllRoles.Find(x => x.Type is LayerEnum.Plaguebearer or LayerEnum.Pestilence && x.Winner);

            if (role == null)
                return;

            __instance.BackgroundBar.material.color = Colors.Infector;
            text.text = "Infectors Win";
            text.color = Colors.Infector;
        }
        else if (Role.UndeadWin)
        {
            var role = Role.AllRoles.Find(x => x.SubFaction == SubFaction.Undead && Role.UndeadWin);

            if (role == null)
                return;

            __instance.BackgroundBar.material.color = role.SubFactionColor;
            text.text = "The Undead Win";
            text.color = role.SubFactionColor;
        }
        else if (Role.CabalWin)
        {
            var role = Role.AllRoles.Find(x => x.SubFaction == SubFaction.Cabal && Role.CabalWin);

            if (role == null)
                return;

            __instance.BackgroundBar.material.color = role.SubFactionColor;
            text.text = "The Cabal Wins";
            text.color = role.SubFactionColor;
        }
        else if (Role.SectWin)
        {
            var role = Role.AllRoles.Find(x => x.SubFaction == SubFaction.Sect && Role.SectWin);

            if (role == null)
                return;

            __instance.BackgroundBar.material.color = role.SubFactionColor;
            text.text = "The Sect Wins";
            text.color = role.SubFactionColor;
        }
        else if (Role.ReanimatedWin)
        {
            var role = Role.AllRoles.Find(x => x.SubFaction == SubFaction.Reanimated && Role.ReanimatedWin);

            if (role == null)
                return;

            __instance.BackgroundBar.material.color = role.SubFactionColor;
            text.text = "The Reanimated Win";
            text.color = role.SubFactionColor;
        }
        else if (Role.CryomaniacWins)
        {
            var role = Role.AllRoles.Find(x => x.Type == LayerEnum.Cryomaniac && x.Winner);

            if (role == null)
                return;

            __instance.BackgroundBar.material.color = role.Color;
            text.text = "Cryomaniac Wins";
            text.color = role.Color;
        }
        else if (Role.ArsonistWins)
        {
            var role = Role.AllRoles.Find(x => x.Type == LayerEnum.Arsonist && x.Winner);

            if (role == null)
                return;

            __instance.BackgroundBar.material.color = role.Color;
            text.text = "Aronist Wins";
            text.color = role.Color;
        }
        else if (Role.GlitchWins)
        {
            var role = Role.AllRoles.Find(x => x.Type == LayerEnum.Glitch && x.Winner);

            if (role == null)
                return;

            __instance.BackgroundBar.material.color = role.Color;
            text.text = "Glitch Wins";
            text.color = role.Color;
            Play("GlitchWin");
        }
        else if (Role.JuggernautWins)
        {
            var role = Role.AllRoles.Find(x => x.Type == LayerEnum.Juggernaut && x.Winner);

            if (role == null)
                return;

            __instance.BackgroundBar.material.color = role.Color;
            text.text = "Juggernaut Wins";
            text.color = role.Color;
        }
        else if (Role.MurdererWins)
        {
            var role = Role.AllRoles.Find(x => x.Type == LayerEnum.Murderer && x.Winner);

            if (role == null)
                return;

            __instance.BackgroundBar.material.color = role.Color;
            text.text = "Murderer Wins";
            text.color = role.Color;
        }
        else if (Role.SerialKillerWins)
        {
            var role = Role.AllRoles.Find(x => x.Type == LayerEnum.SerialKiller && x.Winner);

            if (role == null)
                return;

            __instance.BackgroundBar.material.color = role.Color;
            text.text = "Serial Killer Wins";
            text.color = role.Color;
        }
        else if (Role.WerewolfWins)
        {
            var role = Role.AllRoles.Find(x => x.Type == LayerEnum.Werewolf && x.Winner);

            if (role == null)
                return;

            __instance.BackgroundBar.material.color = role.Color;
            text.text = "Werewolf Wins";
            text.color = role.Color;
        }
        else if (Role.PhantomWins)
        {
            var role = Role.AllRoles.Find(x => x.Type == LayerEnum.Phantom && x.Winner);

            if (role == null)
                return;

            __instance.BackgroundBar.material.color = role.Color;
            text.text = "Phantom Wins";
            text.color = role.Color;
        }
        else if (Role.ActorWins)
        {
            var role = Role.AllRoles.Find(x => x.Type == LayerEnum.Actor && x.Winner);

            if (role == null)
                return;

            __instance.BackgroundBar.material.color = role.Color;
            text.text = "Actor Wins";
            text.color = role.Color;
        }
        else if (Role.BountyHunterWins)
        {
            var role = Role.AllRoles.Find(x => x.Type == LayerEnum.BountyHunter && x.Winner);

            if (role == null)
                return;

            __instance.BackgroundBar.material.color = role.Color;
            text.text = "Bounty Hunter Wins";
            text.color = role.Color;
        }
        else if (Role.CannibalWins)
        {
            var role = Role.AllRoles.Find(x => x.Type == LayerEnum.Cannibal && x.Winner);

            if (role == null)
                return;

            __instance.BackgroundBar.material.color = role.Color;
            text.text = "Cannibal Wins";
            text.color = role.Color;
        }
        else if (Role.ExecutionerWins)
        {
            var role = Role.AllRoles.Find(x => x.Type == LayerEnum.Executioner && x.Winner);

            if (role == null)
                return;

            __instance.BackgroundBar.material.color = role.Color;
            text.text = "Executioner Wins";
            text.color = role.Color;
        }
        else if (Role.GuesserWins)
        {
            var role = Role.AllRoles.Find(x => x.Type == LayerEnum.Guesser && x.Winner);

            if (role == null)
                return;

            __instance.BackgroundBar.material.color = role.Color;
            text.text = "Guesser Wins";
            text.color = role.Color;
        }
        else if (Role.JesterWins)
        {
            var role = Role.AllRoles.Find(x => x.Type == LayerEnum.Jester && x.Winner);

            if (role == null)
                return;

            __instance.BackgroundBar.material.color = role.Color;
            text.text = "Jester Wins";
            text.color = role.Color;
        }
        else if (Role.TrollWins)
        {
            var role = Role.AllRoles.Find(x => x.Type == LayerEnum.Troll && x.Winner);

            if (role == null)
                return;

            __instance.BackgroundBar.material.color = role.Color;
            text.text = "Troll Wins";
            text.color = role.Color;
        }
        else if (Objectifier.CorruptedWins)
        {
            var obj = Objectifier.AllObjectifiers.Find(x => x.Type == LayerEnum.Corrupted && x.Winner);

            if (obj == null)
                return;

            __instance.BackgroundBar.material.color = obj.Color;
            text.text = "Corrupted Wins";
            text.color = obj.Color;
        }
        else if (Objectifier.LoveWins)
        {
            var obj = Objectifier.AllObjectifiers.Find(x => x.Type == LayerEnum.Lovers && x.Winner);

            if (obj == null)
                return;

            __instance.BackgroundBar.material.color = obj.Color;
            text.text = "Love Wins";
            text.color = obj.Color;
        }
        else if (Objectifier.RivalWins)
        {
            var obj = Objectifier.AllObjectifiers.Find(x => x.Type == LayerEnum.Rivals && x.Winner);

            if (obj == null)
                return;

            __instance.BackgroundBar.material.color = obj.Color;
            text.text = "Rival Wins";
            text.color = obj.Color;
        }
        else if (Objectifier.TaskmasterWins)
        {
            var obj = Objectifier.AllObjectifiers.Find(x => x.Type == LayerEnum.Taskmaster && x.Winner);

            if (obj == null)
                return;

            __instance.BackgroundBar.material.color = obj.Color;
            text.text = "Taskmaster Wins";
            text.color = obj.Color;
        }
        else if (Objectifier.OverlordWins)
        {
            var obj = Objectifier.AllObjectifiers.Find(x => x.Type == LayerEnum.Overlord && x.Winner);

            if (obj == null)
                return;

            __instance.BackgroundBar.material.color = obj.Color;
            text.text = "Overlord Wins";
            text.color = obj.Color;
        }
        else if (Objectifier.MafiaWins)
        {
            var obj = Objectifier.AllObjectifiers.Find(x => x.Type == LayerEnum.Mafia && x.Winner);

            if (obj == null)
                return;

            __instance.BackgroundBar.material.color = obj.Color;
            text.text = "The Mafia Wins";
            text.color = obj.Color;
        }

        var pos = __instance.WinText.transform.localPosition;
        pos.y += 1.5f;
        __instance.WinText.transform.localPosition = pos;
        text.text = $"<size=50%>{text.text}</size>";
    }
}