namespace TownOfUsReworked.PlayerLayers
{
    [HarmonyPatch(typeof(EndGameManager), nameof(EndGameManager.Start))]
    public static class Outro
    {
        public static void Postfix(EndGameManager __instance)
        {
            if (!ConstantVariables.GameHasEnded)
                return;

            var text = UObject.Instantiate(__instance.WinText);
            var players = UObject.FindObjectsOfType<PoolablePlayer>();

            if (players.Count > 0)
            {
                foreach (var player in players)
                {
                    var role = Role.AllRoles.Find(x => x.PlayerName == player.NameText().text);
                    player.NameText().text = $"{role.ColorString}<size=75%>{role.Name}</size>\n<size=90%>{player.NameText().text}</size></color>";
                }
            }

            if (PlayerLayer.NobodyWins)
            {
                __instance.BackgroundBar.material.color = Colors.Stalemate;
                text.text = "Stalemate";
                text.color = Colors.Stalemate;
                SoundManager.Instance.StopSound(__instance.ImpostorStinger);
                SoundManager.Instance.PlaySound(__instance.DisconnectStinger, false);
            }
            else if (Role.CrewWin || Role.SyndicateWin || Role.IntruderWin || Role.AllNeutralsWin)
            {
                var role = Role.AllRoles.Find(x => (x.Faction == Faction.Crew && Role.CrewWin) || (x.Faction == Faction.Syndicate && Role.SyndicateWin) || (x.Faction == Faction.Intruder &&
                    Role.IntruderWin) || (x.Faction == Faction.Neutral && Role.AllNeutralsWin));

                if (role == null)
                    return;

                __instance.BackgroundBar.material.color = role.FactionColor;
                text.text = $"{role.FactionName + (role.Faction is Faction.Neutral or Faction.Intruder ? "s" : "")} Win{(role.Faction is Faction.Syndicate or Faction.Crew ? "s" : "")}!";
                text.color = role.FactionColor;

                if (Role.CrewWin)
                {
                    SoundManager.Instance.StopSound(__instance.ImpostorStinger);
                    SoundManager.instance.PlaySound(__instance.CrewStinger, false);
                }
            }
            else if (Role.NKWins)
            {
                var role = Role.AllRoles.Find(x => x.RoleAlignment == RoleAlignment.NeutralKill && x.Winner);

                if (role == null)
                    return;

                __instance.BackgroundBar.material.color = Colors.Alignment;
                text.text = "Neutral Killers Win!";
                text.color = Colors.Alignment;
            }
            else if (Role.InfectorsWin)
            {
                var role = Role.AllRoles.Find(x => x.RoleType is RoleEnum.Plaguebearer or RoleEnum.Pestilence && x.Winner);

                if (role == null)
                    return;

                __instance.BackgroundBar.material.color = Colors.Infector;
                text.text = "Infectors Win!";
                text.color = Colors.Infector;
            }
            else if (Role.UndeadWin || Role.CabalWin || Role.SectWin || Role.ReanimatedWin)
            {
                var role = Role.AllRoles.Find(x => (x.SubFaction == SubFaction.Undead && Role.UndeadWin) || (x.SubFaction == SubFaction.Cabal && Role.CabalWin) ||
                    (x.SubFaction == SubFaction.Sect && Role.SectWin) || (x.SubFaction == SubFaction.Reanimated && Role.ReanimatedWin));

                if (role == null)
                    return;

                __instance.BackgroundBar.material.color = role.SubFactionColor;
                text.text = $"The {role.SubFactionName} Win{(role.SubFaction is SubFaction.Sect or SubFaction.Cabal ? "s" : "")}!";
                text.color = role.SubFactionColor;
            }
            else if (Role.RoleWins)
            {
                var role = Role.AllRoles.Find(x => ((x.RoleType == RoleEnum.Arsonist && Role.ArsonistWins) || (x.RoleType == RoleEnum.Cryomaniac && Role.CryomaniacWins) ||
                    (x.RoleType == RoleEnum.Glitch && Role.GlitchWins) || (x.RoleType == RoleEnum.Juggernaut && Role.JuggernautWins) || (x.RoleType == RoleEnum.Murderer &&
                    Role.MurdererWins) || (x.RoleType == RoleEnum.SerialKiller && Role.SerialKillerWins) || (x.RoleType == RoleEnum.Werewolf && Role.WerewolfWins) || (x.RoleType ==
                    RoleEnum.Phantom && Role.PhantomWins)) && x.Winner);

                if (role == null)
                    return;

                __instance.BackgroundBar.material.color = role.Color;
                text.text = $"{role.Name} Wins!";
                text.color = role.Color;
            }
            else if (Objectifier.ObjectifierWins)
            {
                var obj = Objectifier.AllObjectifiers.Find(x => ((x.ObjectifierType == ObjectifierEnum.Corrupted && Objectifier.CorruptedWins) || (x.ObjectifierType ==
                    ObjectifierEnum.Lovers && Objectifier.LoveWins) || (x.ObjectifierType == ObjectifierEnum.Rivals && Objectifier.RivalWins) || (x.ObjectifierType ==
                    ObjectifierEnum.Taskmaster && Objectifier.TaskmasterWins) || (x.ObjectifierType == ObjectifierEnum.Overlord && Objectifier.OverlordWins) || (x.ObjectifierType ==
                    ObjectifierEnum.Mafia && Objectifier.MafiaWins)) && x.Winner);

                if (obj == null)
                    return;

                __instance.BackgroundBar.material.color = obj.Color;
                text.text = (Objectifier.LoveWins ? "Love" : (Objectifier.RivalWins ? "Rival" : (Objectifier.MafiaWins ? "The Mafia" : $"{obj.Name}"))) + " Wins!";
                text.color = obj.Color;
            }

            var pos = __instance.WinText.transform.localPosition;
            pos.y = 1.5f;
            text.transform.position = pos;
            text.text = $"<size=4>{text.text}</size>";
        }
    }
}