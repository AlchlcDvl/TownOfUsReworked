using System;
using System.Collections.Generic;
using TownOfUsReworked.Objects;
using TownOfUsReworked.CustomOptions;
using TownOfUsReworked.Data;
using TMPro;
using TownOfUsReworked.Custom;
using UnityEngine;
using Object = UnityEngine.Object;
using TownOfUsReworked.Extensions;

namespace TownOfUsReworked.PlayerLayers.Roles
{
    public class Operative : CrewRole
    {
        public List<Bug> Bugs = new();
        public DateTime LastBugged;
        public int UsesLeft;
        public List<RoleEnum> BuggedPlayers = new();
        public bool ButtonUsable => UsesLeft > 0;
        public CustomButton BugButton;
        public Dictionary<byte, TMP_Text> PlayerNumbers = new();

        public Operative(PlayerControl player) : base(player)
        {
            Name = "Operative";
            StartText = "Detect Which Roles Are Here";
            AbilitiesText = "- You can place bugs around the map\n- Upon triggering the bugs, the player's role will be included in a list to be shown in the next meeting\n- " +
                "You can see which colors are where on the admin table\n- On Vitals, the time of death for each player will be shown";
            Color = CustomGameOptions.CustomCrewColors ? Colors.Operative : Colors.Crew;
            RoleType = RoleEnum.Operative;
            BuggedPlayers = new();
            UsesLeft = CustomGameOptions.MaxBugs;
            RoleAlignment = RoleAlignment.CrewInvest;
            AlignmentName = CI;
            Bugs = new();
            InspectorResults = InspectorResults.DropsItems;
            PlayerNumbers = new();
            Type = LayerEnum.Operative;
            BugButton = new(this, "Bug", AbilityTypes.Effect, "ActionSecondary", PlaceBug, true);
        }

        public float BugTimer()
        {
            var utcNow = DateTime.UtcNow;
            var timespan = utcNow - LastBugged;
            var num = Player.GetModifiedCooldown(CustomGameOptions.BugCooldown) * 1000f;
            var flag2 = num - (float)timespan.TotalMilliseconds < 0f;
            return flag2 ? 0f : (num - (float)timespan.TotalMilliseconds) / 1000f;
        }

        public void GenNumber(PlayerVoteArea voteArea)
        {
            var targetId = voteArea.TargetPlayerId;
            var nameText = Object.Instantiate(voteArea.NameText, voteArea.transform);
            nameText.transform.localPosition = new Vector3(-1.211f, -0.18f, -0.1f);
            nameText.text = GameData.Instance.GetPlayerById(targetId).DefaultOutfit.ColorId.ToString();
            PlayerNumbers[targetId] = nameText;
        }

        public override void OnMeetingStart(MeetingHud __instance)
        {
            base.OnMeetingStart(__instance);

            foreach (var role2 in GetRoles<Operative>(RoleEnum.Operative))
                role2.PlayerNumbers.Clear();

            foreach (var voteArea in __instance.playerStates)
                GenNumber(voteArea);

            if (BuggedPlayers.Count == 0)
                HudManager.Instance.Chat.AddChat(PlayerControl.LocalPlayer, "No one triggered your bugs.");
            else if (BuggedPlayers.Count < CustomGameOptions.MinAmountOfPlayersInBug)
                HudManager.Instance.Chat.AddChat(PlayerControl.LocalPlayer, "Not enough players triggered your bugs.");
            else if (BuggedPlayers.Count == 1)
                HudManager.Instance.Chat.AddChat(PlayerControl.LocalPlayer, $"A {BuggedPlayers[0]} triggered your bug.");
            else
            {
                var message = "The following roles triggered your bug:\n";
                var position = 0;
                BuggedPlayers.Shuffle();

                foreach (var role in BuggedPlayers)
                {
                    if (position < BuggedPlayers.Count - 1)
                        message += $" {role},";
                    else
                        message += $" and {role}.";

                    position++;
                }

                HudManager.Instance.Chat.AddChat(PlayerControl.LocalPlayer, message);
            }
        }

        public override void UpdateHud(HudManager __instance)
        {
            base.UpdateHud(__instance);
            BugButton.Update("BUG", BugTimer(), CustomGameOptions.BugCooldown, UsesLeft, ButtonUsable);
        }

        public void PlaceBug()
        {
            if (BugTimer() != 0f || !ButtonUsable)
                return;

            UsesLeft--;
            LastBugged = DateTime.UtcNow;
            Bugs.Add(BugExtensions.CreateBug(PlayerControl.LocalPlayer.GetTruePosition()));
        }
    }
}
