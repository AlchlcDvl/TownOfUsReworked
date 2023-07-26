namespace TownOfUsReworked.PlayerLayers.Roles
{
    public class Operative : Crew
    {
        public List<Bug> Bugs = new();
        public DateTime LastBugged;
        public int UsesLeft;
        public List<RoleEnum> BuggedPlayers = new();
        public bool ButtonUsable => UsesLeft > 0;
        public CustomButton BugButton;
        public Dictionary<byte, TMP_Text> PlayerNumbers = new();

        public override Color32 Color => ClientGameOptions.CustomCrewColors ? Colors.Operative : Colors.Crew;
        public override string Name => "Operative";
        public override LayerEnum Type => LayerEnum.Operative;
        public override RoleEnum RoleType => RoleEnum.Operative;
        public override Func<string> StartText => () => "Detect Which Roles Are Here";
        public override Func<string> AbilitiesText => () => "- You can place bugs around the map\n- Upon triggering the bugs, the player's role will be included in a list to be shown in " +
            "the next meeting\n- You cansee which colors are where on the admin table\n- On Vitals, the time of death for each player will be shown";
        public override InspectorResults InspectorResults => InspectorResults.DropsItems;

        public Operative(PlayerControl player) : base(player)
        {
            UsesLeft = CustomGameOptions.MaxBugs;
            RoleAlignment = RoleAlignment.CrewInvest;
            PlayerNumbers = new();
            BuggedPlayers = new();
            Bugs = new();
            BugButton = new(this, "Bug", AbilityTypes.Effect, "ActionSecondary", PlaceBug, true);
        }

        public override void OnLobby()
        {
            base.OnLobby();
            Bug.Clear(Bugs);
            Bugs.Clear();
        }

        public float BugTimer()
        {
            var timespan = DateTime.UtcNow - LastBugged;
            var num = Player.GetModifiedCooldown(CustomGameOptions.BugCooldown) * 1000f;
            var time = num - (float)timespan.TotalMilliseconds;
            var flag2 = time < 0f;
            return (flag2 ? 0f : time) / 1000f;
        }

        public void GenNumber(PlayerVoteArea voteArea)
        {
            var targetId = voteArea.TargetPlayerId;
            var nameText = UObject.Instantiate(voteArea.NameText, voteArea.transform);
            nameText.transform.localPosition = new(-1.211f, -0.18f, -0.1f);
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
                HUD.Chat.AddChat(CustomPlayer.Local, "No one triggered your bugs.");
            else if (BuggedPlayers.Count < CustomGameOptions.MinAmountOfPlayersInBug)
                HUD.Chat.AddChat(CustomPlayer.Local, "Not enough players triggered your bugs.");
            else if (BuggedPlayers.Count == 1)
                HUD.Chat.AddChat(CustomPlayer.Local, $"A {BuggedPlayers[0]} triggered your bug.");
            else
            {
                var message = "The following roles triggered your bug: ";
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

                HUD.Chat.AddChat(CustomPlayer.Local, message);
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
            Bugs.Add(new(Player.GetTruePosition()));
        }
    }
}
