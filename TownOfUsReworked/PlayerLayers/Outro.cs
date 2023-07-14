using static TownOfUsReworked.Languages.Language;
namespace TownOfUsReworked.PlayerLayers
{
    [HarmonyPatch(typeof(EndGameManager), nameof(EndGameManager.Start))]
    public static class Outro
    {
        public static void Postfix(EndGameManager __instance)
        {
            if (!AssetManager.SoundEffects.ContainsKey("CrewWin"))
                AssetManager.SoundEffects.Add("CrewWin", __instance.CrewStinger);

            if (!AssetManager.SoundEffects.ContainsKey("IntruderWin"))
                AssetManager.SoundEffects.Add("IntruderWin", __instance.ImpostorStinger);

            if (!AssetManager.SoundEffects.ContainsKey("Stalemate"))
                AssetManager.SoundEffects.Add("Stalemate", __instance.DisconnectStinger);

            if (!ConstantVariables.GameHasEnded)
                return;

            var text = UObject.Instantiate(__instance.WinText);
            SoundManager.Instance.StopSound(__instance.ImpostorStinger);
            AssetManager.Play("IntruderWin");

            foreach (var player in UObject.FindObjectsOfType<PoolablePlayer>())
            {
                var role = Role.AllRoles.Find(x => x.PlayerName == player.NameText().text);
                player.NameText().text = $"{role.ColorString}<size=75%>{role.Name}</size>\n<size=90%>{player.NameText().text}</size></color>";
            }

            if (PlayerLayer.NobodyWins)
            {
                __instance.BackgroundBar.material.color = Colors.Stalemate;
                text.text = GetString("Stalemate");
                text.color = Colors.Stalemate;
                AssetManager.Stop("IntruderWin");
                AssetManager.Play("Stalemate");
            }
            else if (Role.SyndicateWin)
            {
                var role = Role.AllRoles.Find(x => x.Faction == Faction.Syndicate && Role.SyndicateWin);

                if (role == null)
                    return;

                __instance.BackgroundBar.material.color = role.FactionColor;
                text.text = GetString("SyndicateWins");
                text.color = role.FactionColor;
            }
            else if (Role.IntruderWin)
            {
                var role = Role.AllRoles.Find(x => x.Faction == Faction.Intruder);

                if (role == null)
                    return;

                __instance.BackgroundBar.material.color = role.FactionColor;
                text.text = GetString("IntrudersWin");
                text.color = role.FactionColor;
            }
            else if (Role.AllNeutralsWin)
            {
                var role = Role.AllRoles.Find(x => x.Faction == Faction.Neutral && Role.AllNeutralsWin);

                if (role == null)
                    return;

                __instance.BackgroundBar.material.color = role.FactionColor;
                text.text = GetString("NeutralsWin");
                text.color = role.FactionColor;
            }
            else if (Role.CrewWin)
            {
                var role = Role.AllRoles.Find(x => x.Faction == Faction.Crew && Role.CrewWin);

                if (role == null)
                    return;

                __instance.BackgroundBar.material.color = role.FactionColor;
                text.text = GetString("CrewWins");
                text.color = role.FactionColor;
                AssetManager.Stop("IntruderWin");
                AssetManager.Play("CrewWin");
            }
            else if (Role.NKWins)
            {
                var role = Role.AllRoles.Find(x => x.RoleAlignment == RoleAlignment.NeutralKill && x.Winner);

                if (role == null)
                    return;

                __instance.BackgroundBar.material.color = Colors.Alignment;
                text.text = GetString("NKWin");
                text.color = Colors.Alignment;
            }
            else if (Role.InfectorsWin)
            {
                var role = Role.AllRoles.Find(x => x.RoleType is RoleEnum.Plaguebearer or RoleEnum.Pestilence && x.Winner);

                if (role == null)
                    return;

                __instance.BackgroundBar.material.color = Colors.Infector;
                text.text = GetString("IFWin");
                text.color = Colors.Infector;
            }
            else if (Role.UndeadWin)
            {
                var role = Role.AllRoles.Find(x => x.SubFaction == SubFaction.Undead && Role.UndeadWin);

                if (role == null)
                    return;

                __instance.BackgroundBar.material.color = role.SubFactionColor;
                text.text = GetString("TUWin");
                text.color = role.SubFactionColor;
            }
            else if (Role.CabalWin)
            {
                var role = Role.AllRoles.Find(x => x.SubFaction == SubFaction.Cabal && Role.CabalWin);

                if (role == null)
                    return;

                __instance.BackgroundBar.material.color = role.SubFactionColor;
                text.text = GetString("TheCabalWins");
                text.color = role.SubFactionColor;
            }
            else if (Role.SectWin)
            {
                var role = Role.AllRoles.Find(x => x.SubFaction == SubFaction.Sect && Role.SectWin);

                if (role == null)
                    return;

                __instance.BackgroundBar.material.color = role.SubFactionColor;
                text.text = GetString("TSWin");
                text.color = role.SubFactionColor;
            }
            else if (Role.ReanimatedWin)
            {
                var role = Role.AllRoles.Find(x => x.SubFaction == SubFaction.Reanimated && Role.ReanimatedWin);

                if (role == null)
                    return;

                __instance.BackgroundBar.material.color = role.SubFactionColor;
                text.text = GetString("TheReanimatedWin");
                text.color = role.SubFactionColor;
            }
            else if (Role.CryomaniacWins)
            {
                var role = Role.AllRoles.Find(x => x.RoleType == RoleEnum.Cryomaniac && x.Winner);

                if (role == null)
                    return;

                __instance.BackgroundBar.material.color = role.Color;
                text.text = GetString("CryomaniacWins");
                text.color = role.Color;
            }
            else if (Role.ArsonistWins)
            {
                var role = Role.AllRoles.Find(x => x.RoleType == RoleEnum.Arsonist && x.Winner);

                if (role == null)
                    return;

                __instance.BackgroundBar.material.color = role.Color;
                text.text = GetString("zhfWin");
                text.color = role.Color;
            }
            else if (Role.GlitchWins)
            {
                var role = Role.AllRoles.Find(x => x.RoleType == RoleEnum.Glitch && x.Winner);

                if (role == null)
                    return;

                __instance.BackgroundBar.material.color = role.Color;
                text.text = GetString("GlitchWins");
                text.color = role.Color;
                AssetManager.Play("GlitchWin");
            }
            else if (Role.JuggernautWins)
            {
                var role = Role.AllRoles.Find(x => x.RoleType == RoleEnum.Juggernaut && x.Winner);

                if (role == null)
                    return;

                __instance.BackgroundBar.material.color = role.Color;
                text.text = GetString("JuggernautWins");
                text.color = role.Color;
            }
            else if (Role.MurdererWins)
            {
                var role = Role.AllRoles.Find(x => x.RoleType == RoleEnum.Murderer && x.Winner);

                if (role == null)
                    return;

                __instance.BackgroundBar.material.color = role.Color;
                text.text = GetString("MurdererWins");
                text.color = role.Color;
            }
            else if (Role.SerialKillerWins)
            {
                var role = Role.AllRoles.Find(x => x.RoleType == RoleEnum.SerialKiller && x.Winner);

                if (role == null)
                    return;

                __instance.BackgroundBar.material.color = role.Color;
                text.text = GetString("SerialKillerWins");
                text.color = role.Color;
            }
            else if (Role.WerewolfWins)
            {
                var role = Role.AllRoles.Find(x => x.RoleType == RoleEnum.Werewolf && x.Winner);

                if (role == null)
                    return;

                __instance.BackgroundBar.material.color = role.Color;
                text.text = GetString("WerewolfWins");
                text.color = role.Color;
            }
            else if (Role.PhantomWins)
            {
                var role = Role.AllRoles.Find(x => x.RoleType == RoleEnum.Phantom && x.Winner);

                if (role == null)
                    return;

                __instance.BackgroundBar.material.color = role.Color;
                text.text = GetString("PhantomWins");
                text.color = role.Color;
            }
            else if (Role.ActorWins)
            {
                var role = Role.AllRoles.Find(x => x.RoleType == RoleEnum.Actor && x.Winner);

                if (role == null)
                    return;

                __instance.BackgroundBar.material.color = role.Color;
                text.text = GetString("ActorWins");
                text.color = role.Color;
            }
            else if (Role.BountyHunterWins)
            {
                var role = Role.AllRoles.Find(x => x.RoleType == RoleEnum.BountyHunter && x.Winner);

                if (role == null)
                    return;

                __instance.BackgroundBar.material.color = role.Color;
                text.text = GetString("BountyHunterWins");
                text.color = role.Color;
            }
            else if (Role.CannibalWins)
            {
                var role = Role.AllRoles.Find(x => x.RoleType == RoleEnum.Cannibal && x.Winner);

                if (role == null)
                    return;

                __instance.BackgroundBar.material.color = role.Color;
                text.text = GetString("CannibalWins");
                text.color = role.Color;
            }
            else if (Role.ExecutionerWins)
            {
                var role = Role.AllRoles.Find(x => x.RoleType == RoleEnum.Executioner && x.Winner);

                if (role == null)
                    return;

                __instance.BackgroundBar.material.color = role.Color;
                text.text = GetString("ExecutionerWins");
                text.color = role.Color;
            }
            else if (Role.GuesserWins)
            {
                var role = Role.AllRoles.Find(x => x.RoleType == RoleEnum.Guesser && x.Winner);

                if (role == null)
                    return;

                __instance.BackgroundBar.material.color = role.Color;
                text.text = GetString("GuesserWins");
                text.color = role.Color;
            }
            else if (Role.JesterWins)
            {
                var role = Role.AllRoles.Find(x => x.RoleType == RoleEnum.Jester && x.Winner);

                if (role == null)
                    return;

                __instance.BackgroundBar.material.color = role.Color;
                text.text = GetString("JesterWins");
                text.color = role.Color;
            }
            else if (Role.TrollWins)
            {
                var role = Role.AllRoles.Find(x => x.RoleType == RoleEnum.Troll && x.Winner);

                if (role == null)
                    return;

                __instance.BackgroundBar.material.color = role.Color;
                text.text = GetString("TrollWins");
                text.color = role.Color;
            }
            else if (Objectifier.CorruptedWins)
            {
                var obj = Objectifier.AllObjectifiers.Find(x => x.ObjectifierType == ObjectifierEnum.Corrupted && x.Winner);

                if (obj == null)
                    return;

                __instance.BackgroundBar.material.color = obj.Color;
                text.text = GetString("CorruptedWins");
                text.color = obj.Color;
            }
            else if (Objectifier.LoveWins)
            {
                var obj = Objectifier.AllObjectifiers.Find(x => x.ObjectifierType == ObjectifierEnum.Lovers && x.Winner);

                if (obj == null)
                    return;

                __instance.BackgroundBar.material.color = obj.Color;
                text.text = GetString("LoWin");
                text.color = obj.Color;
            }
            else if (Objectifier.RivalWins)
            {
                var obj = Objectifier.AllObjectifiers.Find(x => x.ObjectifierType == ObjectifierEnum.Rivals && x.Winner);

                if (obj == null)
                    return;

                __instance.BackgroundBar.material.color = obj.Color;
                text.text = GetString("RivalWins");
                text.color = obj.Color;
            }
            else if (Objectifier.TaskmasterWins)
            {
                var obj = Objectifier.AllObjectifiers.Find(x => x.ObjectifierType == ObjectifierEnum.Taskmaster && x.Winner);

                if (obj == null)
                    return;

                __instance.BackgroundBar.material.color = obj.Color;
                text.text = GetString("TaskmasterWins");
                text.color = obj.Color;
            }
            else if (Objectifier.OverlordWins)
            {
                var obj = Objectifier.AllObjectifiers.Find(x => x.ObjectifierType == ObjectifierEnum.Overlord && x.Winner);

                if (obj == null)
                    return;

                __instance.BackgroundBar.material.color = obj.Color;
                text.text = GetString("OverlordWins");
                text.color = obj.Color;
            }
            else if (Objectifier.MafiaWins)
            {
                var obj = Objectifier.AllObjectifiers.Find(x => x.ObjectifierType == ObjectifierEnum.Mafia && x.Winner);

                if (obj == null)
                    return;

                __instance.BackgroundBar.material.color = obj.Color;
                text.text = GetString("MafiaWin");
                text.color = obj.Color;
            }

            var pos = __instance.WinText.transform.localPosition;
            pos.y += 1.5f;
            __instance.WinText.transform.localPosition = pos;
            text.text = $"<size=50%>{text.text}</size>";
        }
    }
}
