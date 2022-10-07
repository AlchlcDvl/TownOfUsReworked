using HarmonyLib;
using System.Collections.Generic;
using UnityEngine;
using System.Text;
using System.Linq;
using Reactor.Extensions;
using TownOfUs.Roles;

namespace TownOfUs.Patches
{

    static class AdditionalTempData
    {
        public static List<PlayerRoleInfo> playerRoles = new List<PlayerRoleInfo>();

        public static void clear()
        {
            playerRoles.Clear();
        }

        internal class PlayerRoleInfo {
            public string PlayerName { get; set; }
            public string Role {get;set;}
        }
    }


    [HarmonyPatch(typeof(AmongUsClient), nameof(AmongUsClient.OnGameEnd))]
    public class OnGameEndPatch {

        public static void Postfix(AmongUsClient __instance, [HarmonyArgument(0)] EndGameResult endGameResult)
        {
            AdditionalTempData.clear();
            var playerRole = "";
            // Theres a better way of doing this e.g. switch statement or dictionary. But this works for now.
            foreach (var playerControl in PlayerControl.AllPlayerControls)
            {
                playerRole = "";
                foreach (var role in Role.RoleHistory.Where(x => x.Key == playerControl.PlayerId))
                {
                    if (role.Value == RoleEnum.Crewmate) {playerRole += "<color=#" + Patches.Colors.Crew.ToHtmlStringRGBA() + ">Crewmate</color> > ";}
                    else if (role.Value == RoleEnum.Impostor) {playerRole += "<color=#" + Patches.Colors.Impostor.ToHtmlStringRGBA() + ">Impostor</color> > ";}
                    //else if (role.Value == RoleEnum.Anarchist) {playerRole += "<color=#" + Patches.Colors.Syndicate.ToHtmlStringRGBA() + ">Anarchist</color> >";}

                    if (CustomGameOptions.CustomCrewColors)
                    {
                        if (role.Value == RoleEnum.Altruist) {playerRole += "<color=#" + Patches.Colors.Altruist.ToHtmlStringRGBA() + ">Altruist</color> > ";}
                        else if (role.Value == RoleEnum.Engineer) {playerRole += "<color=#" + Patches.Colors.Engineer.ToHtmlStringRGBA() + ">Engineer</color> > ";}
                        else if (role.Value == RoleEnum.Investigator) {playerRole += "<color=#" + Patches.Colors.Investigator.ToHtmlStringRGBA() + ">Investigator</color> > ";}
                        else if (role.Value == RoleEnum.Mayor) {playerRole += "<color=#" + Patches.Colors.Mayor.ToHtmlStringRGBA() + ">Mayor</color> > ";}
                        else if (role.Value == RoleEnum.Medic) {playerRole += "<color=#" + Patches.Colors.Medic.ToHtmlStringRGBA() + ">Medic</color> > ";}
                        else if (role.Value == RoleEnum.Sheriff) {playerRole += "<color=#" + Patches.Colors.Sheriff.ToHtmlStringRGBA() + ">Sheriff</color> > ";}
                        else if (role.Value == RoleEnum.Swapper) {playerRole += "<color=#" + Patches.Colors.Swapper.ToHtmlStringRGBA() + ">Swapper</color> > ";}
                        else if (role.Value == RoleEnum.TimeLord) {playerRole += "<color=#" + Patches.Colors.TimeLord.ToHtmlStringRGBA() + ">Time Lord</color> > ";}
                        else if (role.Value == RoleEnum.Snitch) {playerRole += "<color=#" + Patches.Colors.Snitch.ToHtmlStringRGBA() + ">Snitch</color> > ";}
                        else if (role.Value == RoleEnum.Agent) {playerRole += "<color=#" + Patches.Colors.Agent.ToHtmlStringRGBA() + ">Agent</color> > ";}
                        else if (role.Value == RoleEnum.Vigilante) {playerRole += "<color=#" + Patches.Colors.Vigilante.ToHtmlStringRGBA() + ">Vigilante</color> > ";}
                        else if (role.Value == RoleEnum.Haunter) {playerRole += "<color=#" + Patches.Colors.Haunter.ToHtmlStringRGBA() + ">Haunter</color> > ";}
                        else if (role.Value == RoleEnum.Veteran) {playerRole += "<color=#" + Patches.Colors.Veteran.ToHtmlStringRGBA() + ">Veteran</color> > ";}
                        else if (role.Value == RoleEnum.Tracker) {playerRole += "<color=#" + Patches.Colors.Tracker.ToHtmlStringRGBA() + ">Tracker</color> > ";}
                        else if (role.Value == RoleEnum.Transporter) {playerRole += "<color=#" + Patches.Colors.Transporter.ToHtmlStringRGBA() + ">Transporter</color> > ";}
                        else if (role.Value == RoleEnum.Medium) {playerRole += "<color=#" + Patches.Colors.Medium.ToHtmlStringRGBA() + ">Medium</color> > ";}
                        else if (role.Value == RoleEnum.Operative) {playerRole += "<color=#" + Patches.Colors.Operative.ToHtmlStringRGBA() + ">Operative</color> > ";}
                        else if (role.Value == RoleEnum.Mystic) {playerRole += "<color=#" + Patches.Colors.Mystic.ToHtmlStringRGBA() + ">Mystic</color> > ";}
                        else if (role.Value == RoleEnum.Detective) {playerRole += "<color=#" + Patches.Colors.Detective.ToHtmlStringRGBA() + ">Detective</color> > ";}
                        else if (role.Value == RoleEnum.Shifter) {playerRole += "<color=#" + Patches.Colors.Shifter.ToHtmlStringRGBA() + ">Shifter</color> > ";}
                    }
                    else
                    {
                        if (role.Value == RoleEnum.Altruist) {playerRole += "<color=#" + Patches.Colors.Crew.ToHtmlStringRGBA() + ">Altruist</color> > ";}
                        else if (role.Value == RoleEnum.Engineer) {playerRole += "<color=#" + Patches.Colors.Crew.ToHtmlStringRGBA() + ">Engineer</color> > ";}
                        else if (role.Value == RoleEnum.Investigator) {playerRole += "<color=#" + Patches.Colors.Crew.ToHtmlStringRGBA() + ">Investigator</color> > ";}
                        else if (role.Value == RoleEnum.Mayor) {playerRole += "<color=#" + Patches.Colors.Crew.ToHtmlStringRGBA() + ">Mayor</color> > ";}
                        else if (role.Value == RoleEnum.Medic) {playerRole += "<color=#" + Patches.Colors.Crew.ToHtmlStringRGBA() + ">Medic</color> > ";}
                        else if (role.Value == RoleEnum.Sheriff) {playerRole += "<color=#" + Patches.Colors.Crew.ToHtmlStringRGBA() + ">Sheriff</color> > ";}
                        else if (role.Value == RoleEnum.Swapper) {playerRole += "<color=#" + Patches.Colors.Crew.ToHtmlStringRGBA() + ">Swapper</color> > ";}
                        else if (role.Value == RoleEnum.TimeLord) {playerRole += "<color=#" + Patches.Colors.Crew.ToHtmlStringRGBA() + ">Time Lord</color> > ";}
                        else if (role.Value == RoleEnum.Snitch) {playerRole += "<color=#" + Patches.Colors.Crew.ToHtmlStringRGBA() + ">Snitch</color> > ";}
                        else if (role.Value == RoleEnum.Agent) {playerRole += "<color=#" + Patches.Colors.Crew.ToHtmlStringRGBA() + ">Agent</color> > ";}
                        else if (role.Value == RoleEnum.Vigilante) {playerRole += "<color=#" + Patches.Colors.Crew.ToHtmlStringRGBA() + ">Vigilante</color> > ";}
                        else if (role.Value == RoleEnum.Haunter) {playerRole += "<color=#" + Patches.Colors.Crew.ToHtmlStringRGBA() + ">Haunter</color> > ";}
                        else if (role.Value == RoleEnum.Veteran) {playerRole += "<color=#" + Patches.Colors.Crew.ToHtmlStringRGBA() + ">Veteran</color> > ";}
                        else if (role.Value == RoleEnum.Tracker) {playerRole += "<color=#" + Patches.Colors.Crew.ToHtmlStringRGBA() + ">Tracker</color> > ";}
                        else if (role.Value == RoleEnum.Transporter) {playerRole += "<color=#" + Patches.Colors.Crew.ToHtmlStringRGBA() + ">Transporter</color> > ";}
                        else if (role.Value == RoleEnum.Medium) {playerRole += "<color=#" + Patches.Colors.Crew.ToHtmlStringRGBA() + ">Medium</color> > ";}
                        else if (role.Value == RoleEnum.Operative) {playerRole += "<color=#" + Patches.Colors.Crew.ToHtmlStringRGBA() + ">Operative</color> > ";}
                        else if (role.Value == RoleEnum.Mystic) {playerRole += "<color=#" + Patches.Colors.Crew.ToHtmlStringRGBA() + ">Mystic</color> > ";}
                        else if (role.Value == RoleEnum.Detective) {playerRole += "<color=#" + Patches.Colors.Crew.ToHtmlStringRGBA() + ">Detective</color> > ";}
                        else if (role.Value == RoleEnum.Shifter) {playerRole += "<color=#" + Patches.Colors.Crew.ToHtmlStringRGBA() + ">Shifter</color> > ";}
                    }

                    if (CustomGameOptions.CustomImpColors)
                    {
                        if (role.Value == RoleEnum.Camouflager) {playerRole += "<color=#" + Patches.Colors.Camouflager.ToHtmlStringRGBA() + ">Camouflager</color> > ";}
                        else if (role.Value == RoleEnum.Grenadier) {playerRole += "<color=#" + Patches.Colors.Grenadier.ToHtmlStringRGBA() + ">Grenadier</color> > ";}
                        else if (role.Value == RoleEnum.Janitor) {playerRole += "<color=#" + Patches.Colors.Janitor.ToHtmlStringRGBA() + ">Janitor</color> > ";}
                        else if (role.Value == RoleEnum.Miner) {playerRole += "<color=#" + Patches.Colors.Miner.ToHtmlStringRGBA() + ">Miner</color> > ";}
                        else if (role.Value == RoleEnum.Morphling) {playerRole += "<color=#" + Patches.Colors.Morphling.ToHtmlStringRGBA() + ">Morphling</color> > ";}
                        else if (role.Value == RoleEnum.Wraith) {playerRole += "<color=#" + Patches.Colors.Wraith.ToHtmlStringRGBA() + ">Wraith</color> > ";}
                        else if (role.Value == RoleEnum.Underdog) {playerRole += "<color=#" + Patches.Colors.Underdog.ToHtmlStringRGBA() + ">Underdog</color> > ";}
                        else if (role.Value == RoleEnum.Undertaker) {playerRole += "<color=#" + Patches.Colors.Undertaker.ToHtmlStringRGBA() + ">Undertaker</color> > ";}
                        else if (role.Value == RoleEnum.Poisoner) {playerRole += "<color=#" + Patches.Colors.Poisoner.ToHtmlStringRGBA() + ">Poisoner</color> > ";}
                        else if (role.Value == RoleEnum.Traitor) {playerRole += "<color=#" + Patches.Colors.Traitor.ToHtmlStringRGBA() + ">Traitor</color> > ";}
                        else if (role.Value == RoleEnum.Blackmailer) {playerRole += "<color=#" + Patches.Colors.Blackmailer.ToHtmlStringRGBA() + ">Blackmailer</color> > ";}
                        else if (role.Value == RoleEnum.Disguiser) {playerRole += "<color=#" + Patches.Colors.Disguiser.ToHtmlStringRGBA() + ">Disguiser</color> > ";}
                        else if (role.Value == RoleEnum.TimeMaster) {playerRole += "<color=#" + Patches.Colors.TimeMaster.ToHtmlStringRGBA() + ">Time Master</color> > ";}
                        else if (role.Value == RoleEnum.Consigliere) {playerRole += "<color=#" + Patches.Colors.Consigliere.ToHtmlStringRGBA() + ">Consigliere</color> > ";}
                    }
                    else
                    {
                        if (role.Value == RoleEnum.Camouflager) {playerRole += "<color=#" + Patches.Colors.Impostor.ToHtmlStringRGBA() + ">Camouflager</color> > ";}
                        else if (role.Value == RoleEnum.Grenadier) {playerRole += "<color=#" + Patches.Colors.Impostor.ToHtmlStringRGBA() + ">Grenadier</color> > ";}
                        else if (role.Value == RoleEnum.Janitor) {playerRole += "<color=#" + Patches.Colors.Impostor.ToHtmlStringRGBA() + ">Janitor</color> > ";}
                        else if (role.Value == RoleEnum.Miner) {playerRole += "<color=#" + Patches.Colors.Impostor.ToHtmlStringRGBA() + ">Miner</color> > ";}
                        else if (role.Value == RoleEnum.Morphling) {playerRole += "<color=#" + Patches.Colors.Impostor.ToHtmlStringRGBA() + ">Morphling</color> > ";}
                        else if (role.Value == RoleEnum.Wraith) {playerRole += "<color=#" + Patches.Colors.Impostor.ToHtmlStringRGBA() + ">Wraith</color> > ";}
                        else if (role.Value == RoleEnum.Underdog) {playerRole += "<color=#" + Patches.Colors.Impostor.ToHtmlStringRGBA() + ">Underdog</color> > ";}
                        else if (role.Value == RoleEnum.Undertaker) {playerRole += "<color=#" + Patches.Colors.Impostor.ToHtmlStringRGBA() + ">Undertaker</color> > ";}
                        else if (role.Value == RoleEnum.Poisoner) {playerRole += "<color=#" + Patches.Colors.Impostor.ToHtmlStringRGBA() + ">Poisoner</color> > ";}
                        else if (role.Value == RoleEnum.Traitor) {playerRole += "<color=#" + Patches.Colors.Impostor.ToHtmlStringRGBA() + ">Traitor</color> > ";}
                        else if (role.Value == RoleEnum.Blackmailer) {playerRole += "<color=#" + Patches.Colors.Impostor.ToHtmlStringRGBA() + ">Blackmailer</color> > ";}
                        else if (role.Value == RoleEnum.Disguiser) {playerRole += "<color=#" + Patches.Colors.Impostor.ToHtmlStringRGBA() + ">Disguiser</color> > ";}
                        else if (role.Value == RoleEnum.TimeMaster) {playerRole += "<color=#" + Patches.Colors.Impostor.ToHtmlStringRGBA() + ">Time Master</color> > ";}
                        else if (role.Value == RoleEnum.Consigliere) {playerRole += "<color=#" + Patches.Colors.Impostor.ToHtmlStringRGBA() + ">Consigliere</color> > ";}
                    }

                    if (CustomGameOptions.CustomNeutColors)
                    {
                        if (role.Value == RoleEnum.Plaguebearer) {playerRole += "<color=#" + Patches.Colors.Plaguebearer.ToHtmlStringRGBA() + ">Plaguebearer</color> > ";}
                        else if (role.Value == RoleEnum.Pestilence) {playerRole += "<color=#" + Patches.Colors.Pestilence.ToHtmlStringRGBA() + ">Pestilence</color> > ";}
                        else if (role.Value == RoleEnum.Werewolf) {playerRole += "<color=#" + Patches.Colors.Werewolf.ToHtmlStringRGBA() + ">Werewolf</color> > ";}
                        else if (role.Value == RoleEnum.Survivor) {playerRole += "<color=#" + Patches.Colors.Survivor.ToHtmlStringRGBA() + ">Survivor</color> > ";}
                        else if (role.Value == RoleEnum.GuardianAngel) {playerRole += "<color=#" + Patches.Colors.GuardianAngel.ToHtmlStringRGBA() + ">Guardian Angel</color> > ";}
                        else if (role.Value == RoleEnum.Arsonist) {playerRole += "<color=#" + Patches.Colors.Arsonist.ToHtmlStringRGBA() + ">Arsonist</color> > ";}
                        else if (role.Value == RoleEnum.Executioner) {playerRole += "<color=#" + Patches.Colors.Executioner.ToHtmlStringRGBA() + ">Executioner</color> > ";}
                        else if (role.Value == RoleEnum.Glitch) {playerRole += "<color=#" + Patches.Colors.Glitch.ToHtmlStringRGBA() + ">Glitch</color> > ";}
                        else if (role.Value == RoleEnum.Jester) {playerRole += "<color=#" + Patches.Colors.Jester.ToHtmlStringRGBA() + ">Jester</color> > ";}
                        else if (role.Value == RoleEnum.Phantom) {playerRole += "<color=#" + Patches.Colors.Phantom.ToHtmlStringRGBA() + ">Phantom</color> > ";}
                        else if (role.Value == RoleEnum.Amnesiac) {playerRole += "<color=#" + Patches.Colors.Amnesiac.ToHtmlStringRGBA() + ">Amnesiac</color> > ";}
                        else if (role.Value == RoleEnum.Juggernaut) {playerRole += "<color=#" + Patches.Colors.Juggernaut.ToHtmlStringRGBA() + ">Juggernaut</color> > ";}
                        else if (role.Value == RoleEnum.Cannibal) {playerRole += "<color=#" + Patches.Colors.Cannibal.ToHtmlStringRGBA() + ">Cannibal</color> > ";}
                        else if (role.Value == RoleEnum.Taskmaster) {playerRole += "<color=#" + Patches.Colors.Taskmaster.ToHtmlStringRGBA() + ">Taskmaster</color> > ";}
                        else if (role.Value == RoleEnum.Dracula) {playerRole += "<color=#" + Patches.Colors.Dracula.ToHtmlStringRGBA() + ">Dracula</color> > ";}
                        else if (role.Value == RoleEnum.Vampire) {playerRole += "<color=#" + Patches.Colors.Vampire.ToHtmlStringRGBA() + ">Vampire</color> > ";}
                    }
                    else
                    {
                        if (role.Value == RoleEnum.Plaguebearer) {playerRole += "<color=#" + Patches.Colors.Neutral.ToHtmlStringRGBA() + ">Plaguebearer</color> > ";}
                        else if (role.Value == RoleEnum.Pestilence) {playerRole += "<color=#" + Patches.Colors.Neutral.ToHtmlStringRGBA() + ">Pestilence</color> > ";}
                        else if (role.Value == RoleEnum.Werewolf) {playerRole += "<color=#" + Patches.Colors.Neutral.ToHtmlStringRGBA() + ">Werewolf</color> > ";}
                        else if (role.Value == RoleEnum.Survivor) {playerRole += "<color=#" + Patches.Colors.Neutral.ToHtmlStringRGBA() + ">Survivor</color> > ";}
                        else if (role.Value == RoleEnum.GuardianAngel) {playerRole += "<color=#" + Patches.Colors.Neutral.ToHtmlStringRGBA() + ">Guardian Angel</color> > ";}
                        else if (role.Value == RoleEnum.Arsonist) {playerRole += "<color=#" + Patches.Colors.Neutral.ToHtmlStringRGBA() + ">Arsonist</color> > ";}
                        else if (role.Value == RoleEnum.Executioner) {playerRole += "<color=#" + Patches.Colors.Neutral.ToHtmlStringRGBA() + ">Executioner</color> > ";}
                        else if (role.Value == RoleEnum.Glitch) {playerRole += "<color=#" + Patches.Colors.Neutral.ToHtmlStringRGBA() + ">Glitch</color> > ";}
                        else if (role.Value == RoleEnum.Jester) {playerRole += "<color=#" + Patches.Colors.Neutral.ToHtmlStringRGBA() + ">Jester</color> > ";}
                        else if (role.Value == RoleEnum.Phantom) {playerRole += "<color=#" + Patches.Colors.Neutral.ToHtmlStringRGBA() + ">Phantom</color> > ";}
                        else if (role.Value == RoleEnum.Amnesiac) {playerRole += "<color=#" + Patches.Colors.Neutral.ToHtmlStringRGBA() + ">Amnesiac</color> > ";}
                        else if (role.Value == RoleEnum.Juggernaut) {playerRole += "<color=#" + Patches.Colors.Neutral.ToHtmlStringRGBA() + ">Juggernaut</color> > ";}
                        else if (role.Value == RoleEnum.Cannibal) {playerRole += "<color=#" + Patches.Colors.Neutral.ToHtmlStringRGBA() + ">Cannibal</color> > ";}
                        else if (role.Value == RoleEnum.Taskmaster) {playerRole += "<color=#" + Patches.Colors.Neutral.ToHtmlStringRGBA() + ">Taskmaster</color> > ";}
                        else if (role.Value == RoleEnum.Dracula) {playerRole += "<color=#" + Patches.Colors.Neutral.ToHtmlStringRGBA() + ">Dracula</color> > ";}
                        else if (role.Value == RoleEnum.Vampire) {playerRole += "<color=#" + Patches.Colors.Neutral.ToHtmlStringRGBA() + ">Vampire</color> > ";}
                    }
                }

                playerRole = playerRole.Remove(playerRole.Length - 3);

                if (CustomGameOptions.CustomModifierColors)
                {
                    if (playerControl.Is(ModifierEnum.Giant)) {playerRole += " (<color=#" + Patches.Colors.Giant.ToHtmlStringRGBA() + ">Giant</color>)";}
                    else if (playerControl.Is(ModifierEnum.ButtonBarry)) {playerRole += " (<color=#" + Patches.Colors.ButtonBarry.ToHtmlStringRGBA() + ">Button Barry</color>)";}
                    else if (playerControl.Is(ModifierEnum.Coward)) {playerRole += " (<color=#" + Patches.Colors.Coward.ToHtmlStringRGBA() + ">Coward</color>)";}
                    else if (playerControl.Is(ModifierEnum.Bait)) {playerRole += " (<color=#" + Patches.Colors.Bait.ToHtmlStringRGBA() + ">Bait</color>)";}
                    else if (playerControl.Is(ModifierEnum.Diseased)) {playerRole += " (<color=#" + Patches.Colors.Diseased.ToHtmlStringRGBA() + ">Diseased</color>)";}
                    else if (playerControl.Is(ModifierEnum.Dwarf)) {playerRole += " (<color=#" + Patches.Colors.Dwarf.ToHtmlStringRGBA() + ">Dwarf</color>)";}
                    else if (playerControl.Is(ModifierEnum.Tiebreaker)) {playerRole += " (<color=#" + Patches.Colors.Tiebreaker.ToHtmlStringRGBA() + ">Tiebreaker</color>)";}
                    else if (playerControl.Is(ModifierEnum.Torch)) {playerRole += " (<color=#" + Patches.Colors.Torch.ToHtmlStringRGBA() + ">Torch</color>)";}
                    else if (playerControl.Is(ModifierEnum.Lighter)) {playerRole += " (<color=#" + Patches.Colors.Lighter.ToHtmlStringRGBA() + ">Lighter</color>)";}
                    else if (playerControl.Is(ModifierEnum.Lover)) {playerRole += " (<color=#" + Patches.Colors.Lovers.ToHtmlStringRGBA() + ">Lover</color>)";}
                    else if (playerControl.Is(ModifierEnum.Drunk)) {playerRole += " (<color=#" + Patches.Colors.Drunk.ToHtmlStringRGBA() + ">Drunk</color>)";}
                    else if (playerControl.Is(ModifierEnum.Volatile)) {playerRole += " (<color=#" + Patches.Colors.Volatile.ToHtmlStringRGBA() + ">Volatile</color>)";}
                    else if (playerControl.Is(ModifierEnum.Lighter)) {playerRole += " (<color=#" + Patches.Colors.Lighter.ToHtmlStringRGBA() + ">Lighter</color>)";}
                }
                else
                {
                    if (playerControl.Is(ModifierEnum.Giant)) {playerRole += " (<color=#" + Patches.Colors.Modifier.ToHtmlStringRGBA() + ">Giant</color>)";}
                    else if (playerControl.Is(ModifierEnum.ButtonBarry)) {playerRole += " (<color=#" + Patches.Colors.Modifier.ToHtmlStringRGBA() + ">Button Barry</color>)";}
                    else if (playerControl.Is(ModifierEnum.Coward)) {playerRole += " (<color=#" + Patches.Colors.Modifier.ToHtmlStringRGBA() + ">Coward</color>)";}
                    else if (playerControl.Is(ModifierEnum.Bait)) {playerRole += " (<color=#" + Patches.Colors.Modifier.ToHtmlStringRGBA() + ">Bait</color>)";}
                    else if (playerControl.Is(ModifierEnum.Diseased)) {playerRole += " (<color=#" + Patches.Colors.Modifier.ToHtmlStringRGBA() + ">Diseased</color>)";}
                    else if (playerControl.Is(ModifierEnum.Dwarf)) {playerRole += " (<color=#" + Patches.Colors.Modifier.ToHtmlStringRGBA() + ">Dwarf</color>)";}
                    else if (playerControl.Is(ModifierEnum.Tiebreaker)) {playerRole += " (<color=#" + Patches.Colors.Modifier.ToHtmlStringRGBA() + ">Tiebreaker</color>)";}
                    else if (playerControl.Is(ModifierEnum.Torch)) {playerRole += " (<color=#" + Patches.Colors.Modifier.ToHtmlStringRGBA() + ">Torch</color>)";}
                    else if (playerControl.Is(ModifierEnum.Lighter)) {playerRole += " (<color=#" + Patches.Colors.Modifier.ToHtmlStringRGBA() + ">Lighter</color>)";}
                    else if (playerControl.Is(ModifierEnum.Lover)) {playerRole += " (<color=#" + Patches.Colors.Modifier.ToHtmlStringRGBA() + ">Lover</color>)";}
                    else if (playerControl.Is(ModifierEnum.Drunk)) {playerRole += " (<color=#" + Patches.Colors.Modifier.ToHtmlStringRGBA() + ">Drunk</color>)";}
                    else if (playerControl.Is(ModifierEnum.Volatile)) {playerRole += " (<color=#" + Patches.Colors.Modifier.ToHtmlStringRGBA() + ">Volatile</color>)";}
                    else if (playerControl.Is(ModifierEnum.Lighter)) {playerRole += " (<color=#" + Patches.Colors.Modifier.ToHtmlStringRGBA() + ">Lighter</color>)";}
                }

                if (CustomGameOptions.CustomObjectifierColors)
                {
                    if (playerControl.Is(AbilityEnum.Assassin)) {playerRole += " [<color=#" + Patches.Colors.Assassin.ToHtmlStringRGBA() + ">Assassin</color>]";}
                }
                else
                {
                    if (playerControl.Is(AbilityEnum.Assassin)) {playerRole += " [<color=#" + Patches.Colors.Objectifier.ToHtmlStringRGBA() + ">Assassin</color>]";}
                }
                AdditionalTempData.playerRoles.Add(new AdditionalTempData.PlayerRoleInfo() { PlayerName = playerControl.Data.PlayerName, Role = playerRole });
            }
        }
    }

    [HarmonyPatch(typeof(EndGameManager), nameof(EndGameManager.SetEverythingUp))]
    public class EndGameManagerSetUpPatch {
        public static void Postfix(EndGameManager __instance) {
            GameObject bonusText = UnityEngine.Object.Instantiate(__instance.WinText.gameObject);
            bonusText.transform.position = new Vector3(__instance.WinText.transform.position.x, __instance.WinText.transform.position.y - 0.8f, __instance.WinText.transform.position.z);
            bonusText.transform.localScale = new Vector3(0.7f, 0.7f, 1f);
            TMPro.TMP_Text textRenderer = bonusText.GetComponent<TMPro.TMP_Text>();
            textRenderer.text = "";

            var position = Camera.main.ViewportToWorldPoint(new Vector3(0f, 1f, Camera.main.nearClipPlane));
            GameObject roleSummary = UnityEngine.Object.Instantiate(__instance.WinText.gameObject);
            roleSummary.transform.position = new Vector3(__instance.Navigation.ExitButton.transform.position.x + 0.1f, position.y - 0.1f, -14f); 
            roleSummary.transform.localScale = new Vector3(1f, 1f, 1f);

            var roleSummaryText = new StringBuilder();
            roleSummaryText.AppendLine("End game summary:");
            foreach(var data in AdditionalTempData.playerRoles) {
                var role = string.Join(" ", data.Role);
                roleSummaryText.AppendLine($"{data.PlayerName} - {role}");
            }
            TMPro.TMP_Text roleSummaryTextMesh = roleSummary.GetComponent<TMPro.TMP_Text>();
            roleSummaryTextMesh.alignment = TMPro.TextAlignmentOptions.TopLeft;
            roleSummaryTextMesh.color = Color.white;
            roleSummaryTextMesh.fontSizeMin = 1.5f;
            roleSummaryTextMesh.fontSizeMax = 1.5f;
            roleSummaryTextMesh.fontSize = 1.5f;
             
            var roleSummaryTextMeshRectTransform = roleSummaryTextMesh.GetComponent<RectTransform>();
            roleSummaryTextMeshRectTransform.anchoredPosition = new Vector2(position.x + 3.5f, position.y - 0.1f);
            roleSummaryTextMesh.text = roleSummaryText.ToString();
            AdditionalTempData.clear();
        }
    }
}