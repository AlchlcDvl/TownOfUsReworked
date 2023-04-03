using HarmonyLib;
using UnityEngine;
using TownOfUsReworked.Classes;
using TownOfUsReworked.PlayerLayers.Objectifiers;
using TownOfUsReworked.PlayerLayers.Roles;
using TownOfUsReworked.Data;

namespace TownOfUsReworked.PlayerLayers
{
    [HarmonyPatch(typeof(EndGameManager), nameof(EndGameManager.Start))]
    public static class Outro
    {
        public static void Postfix(EndGameManager __instance)
        {
            if (!ConstantVariables.GameHasEnded)
                return;

            var text = Object.Instantiate(__instance.WinText);
            var players = Object.FindObjectsOfType<PoolablePlayer>();

            if (players.Count > 0)
            {
                foreach (var player in players)
                {
                    var role = Role.AllRoles.Find(x => x.PlayerName == player.NameText().text);
                    player.NameText().text = $"{role.ColorString}<size=75%>{role.Name}</size>\n<size=90%>{player.NameText().text}</size></color>";
                }
            }

            if (!Role.IntruderWin)
                SoundManager.Instance.StopSound(__instance.ImpostorStinger);

            if (Role.NobodyWins || Objectifier.NobodyWins)
            {
                __instance.BackgroundBar.material.color = Colors.Stalemate;
                text.text = "Stalemate!";
                text.color = Colors.Stalemate;
                SoundManager.Instance.PlaySound(__instance.DisconnectStinger, false);
            }
            else if (Role.CrewWin)
            {
                var role = Role.AllRoles.Find(x => x.Faction == Faction.Crew);

                if (role == null)
                    return;

                __instance.BackgroundBar.material.color = Colors.Crew;
                text.text = "Crew Wins!";
                text.color = Colors.Crew;
                SoundManager.Instance.PlaySound(__instance.CrewStinger, false);
            }
            else if (Role.SyndicateWin)
            {
                var role = Role.AllRoles.Find(x => x.Faction == Faction.Syndicate);

                if (role == null)
                    return;

                __instance.BackgroundBar.material.color = Colors.Syndicate;
                text.text = "Syndicate Wins!";
                text.color = Colors.Syndicate;
            }
            else if (Role.IntruderWin)
            {
                var role = Role.AllRoles.Find(x => x.Faction == Faction.Intruder);

                if (role == null)
                    return;

                __instance.BackgroundBar.material.color = Colors.Intruder;
                text.text = "Intruders Win!";
                text.color = Colors.Intruder;
            }
            else if (Role.AllNeutralsWin)
            {
                var role = Role.AllRoles.Find(x => x.Faction == Faction.Neutral);

                if (role == null)
                    return;

                __instance.BackgroundBar.material.color = Colors.Neutral;
                text.text = "Neutrals Win!";
                text.color = Colors.Neutral;
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
                var role = Role.AllRoles.Find(x => x.Type == RoleEnum.Plaguebearer || x.Type == RoleEnum.Pestilence);

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
                text.text = $"The {role.SubFactionName} Win!";
                text.color = role.SubFactionColor;
            }
            else if (Role.RoleWins)
            {
                var role = Role.AllRoles.Find(x => ((x.Type == RoleEnum.Arsonist && Role.ArsonistWins) || (x.Type == RoleEnum.Cryomaniac && Role.CryomaniacWins) ||
                    (x.Type == RoleEnum.Glitch && Role.GlitchWins) || (x.Type == RoleEnum.Juggernaut && Role.JuggernautWins) || (x.Type == RoleEnum.Murderer &&
                    Role.MurdererWins) || (x.Type == RoleEnum.SerialKiller && Role.SerialKillerWins) || (x.Type == RoleEnum.Werewolf && Role.WerewolfWins) || (x.Type ==
                    RoleEnum.Phantom && Role.PhantomWins)) && x.Winner);

                if (role == null)
                    return;

                __instance.BackgroundBar.material.color = role.Color;
                text.text = $"{role.Name} Wins!";
                text.color = role.Color;
            }
            else if (Objectifier.ObjectifierWins)
            {
                var obj = Objectifier.AllObjectifiers.Find(x => ((x.Type == ObjectifierEnum.Corrupted && Objectifier.CorruptedWins) || (x.Type ==
                    ObjectifierEnum.Lovers && Objectifier.LoveWins) || (x.Type == ObjectifierEnum.Rivals && Objectifier.RivalWins) || (x.Type ==
                    ObjectifierEnum.Taskmaster && Objectifier.TaskmasterWins) || (x.Type == ObjectifierEnum.Overlord && Objectifier.OverlordWins)) && x.Winner);

                if (obj == null)
                    return;

                __instance.BackgroundBar.material.color = obj.Color;
                text.text = (obj.Type == ObjectifierEnum.Lovers ? "Love" : obj.Type == ObjectifierEnum.Rivals ? "Rival" : $"{obj.Name}") + " Wins!";
                text.color = obj.Color;
            }

            var pos = __instance.WinText.transform.localPosition;
            pos.y = 1.5f;
            text.transform.position = pos;
            text.text = $"<size=4>{text.text}</size>";
        }
    }
}