using HarmonyLib;
using TownOfUsReworked.Enums;
using TownOfUsReworked.Classes;
using Random = UnityEngine.Random;
using System.Collections.Generic;

namespace TownOfUsReworked.PlayerLayers.Roles.NeutralRoles.GuesserMod
{
    [HarmonyPatch(typeof(MeetingHud), nameof(MeetingHud.Start))]
    public class StartMeetingPatch
    {
        public static int lettersGiven = 0;
        public static bool lettersExhausted = false;
        public static string roleName = "";
        public static List<string> letters = new List<string>();

        public static void Prefix()
        {
            if (Utils.NoButton(PlayerControl.LocalPlayer, RoleEnum.Guesser))
                return;

            var role = Role.GetRole<Guesser>(PlayerControl.LocalPlayer);
            var targetRole = Role.GetRole(role.TargetPlayer);
            var something = "";
            var newRoleName = targetRole.Name;
            bool rolechanged = false;

            if (roleName != newRoleName && roleName != "")
            {
                rolechanged = true;
                roleName = newRoleName;
            }
            else if (roleName == "")
                roleName = newRoleName;

            if (rolechanged)
                something = "Your target's role changed!";
            else if (!lettersExhausted)
            {
                var random = Random.RandomRangeInt(0, roleName.Length);
                var random2 = Random.RandomRangeInt(0, roleName.Length);
                var random3 = Random.RandomRangeInt(0, roleName.Length);

                if (lettersGiven <= roleName.Length - 3)
                {
                    while (random == random2 || random2 == random3 || random == random3 || letters.Contains($"{roleName[random]}") || letters.Contains($"{roleName[random2]}") ||
                        letters.Contains($"{roleName[random3]}"))
                    {
                        if (random == random2 || letters.Contains($"{roleName[random2]}"))
                            random2 = Random.RandomRangeInt(0, roleName.Length);

                        if (random2 == random3 || letters.Contains($"{roleName[random3]}"))
                            random3 = Random.RandomRangeInt(0, roleName.Length);

                        if (random == random3 || letters.Contains($"{roleName[random]}"))
                            random = Random.RandomRangeInt(0, roleName.Length);
                    }

                    something = $"Your target's role as the letters {roleName[random]}, {roleName[random2]} and {roleName[random3]} in it!";
                }
                else if (lettersGiven == roleName.Length - 2)
                {
                    while (random == random2 || letters.Contains($"{roleName[random]}") || letters.Contains($"{roleName[random2]}"))
                    {
                        if (letters.Contains($"{roleName[random2]}"))
                            random2 = Random.RandomRangeInt(0, roleName.Length);

                        if (letters.Contains($"{roleName[random]}"))
                            random = Random.RandomRangeInt(0, roleName.Length);
                        
                        if (random == random2)
                            random = Random.RandomRangeInt(0, roleName.Length);
                    }

                    something = $"Your target's role as the letters {roleName[random]} and {roleName[random2]} in it!";
                }
                else if (lettersGiven == roleName.Length - 1)
                {
                    while (letters.Contains($"{roleName[random]}"))
                        random = Random.RandomRangeInt(0, roleName.Length);

                    something = $"Your target's role as the letter {roleName[random]} in it!";
                }
                else if (lettersGiven == roleName.Length)
                    lettersExhausted = true;

                if (!lettersExhausted)
                {
                    if (lettersGiven <= roleName.Length - 3)
                    {
                        letters.Add($"{roleName[random]}");
                        letters.Add($"{roleName[random2]}");
                        letters.Add($"{roleName[random3]}");
                        lettersGiven += 3;
                    }
                    else if (lettersGiven == roleName.Length - 2)
                    {
                        letters.Add($"{roleName[random]}");
                        letters.Add($"{roleName[random2]}");
                        lettersGiven += 2;
                    }
                    else if (lettersGiven == roleName.Length - 1)
                    {
                        letters.Add($"{roleName[random]}");
                        lettersGiven++;
                    }
                }
            }
            else if (!role.FactionHintGiven && lettersExhausted)
            {
                something = $"Your target belongs to the {targetRole.FactionName}!";
                role.FactionHintGiven = true;
            }
            else if (!role.SubFactionHintGiven && lettersExhausted)
            {
                something = $"Your target belongs to the {targetRole.SubFactionName}!";
                role.SubFactionHintGiven = true;
            }
            else if (!role.AlignmentHintGiven && lettersExhausted)
            {
                something = $"Your target is a {targetRole.AlignmentName} Role!";
                role.AlignmentHintGiven = true;
            }

            //Ensures only the Guesser sees this
            if (HudManager.Instance && something != "")
                HudManager.Instance.Chat.AddChat(PlayerControl.LocalPlayer, something);
        }
    }
}