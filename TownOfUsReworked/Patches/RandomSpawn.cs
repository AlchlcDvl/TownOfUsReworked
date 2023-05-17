namespace TownOfUsReworked.Patches
{
    //The code is from The Other Roles: Community Edition with slight modifications; link :- https://github.com/JustASysAdmin/TheOtherRoles2/blob/main/TheOtherRoles/Patches/IntroPatch.cs
    //under GPL v3
    [HarmonyPatch]
    public static class RandomSpawns
    {
        [HarmonyPatch(typeof(IntroCutscene), nameof(IntroCutscene.OnDestroy))]
        public static class IntroCutsceneOnDestroyPatch
        {
            public static void Prefix() => RandomSpawn();
        }

        [HarmonyPatch(typeof(ExileController), nameof(ExileController.WrapUp))]
        public static class BaseExileControllerPatch
        {
            public static void Postfix() => RandomSpawn();
        }

        private static void RandomSpawn()
        {
            if (!AmongUsClient.Instance.AmHost || !CustomGameOptions.RandomSpawns || TownOfUsReworked.VanillaOptions.MapId is 4 or 5)
                return;

            var skeldSpawn = new List<Vector3>()
            {
                new(-2.2f, 2.2f, 0f), //cafeteria. botton. top left.
                new(0.7f, 2.2f, 0f), //caffeteria. button. top right.
                new(-2.2f, -0.2f, 0f), //caffeteria. button. bottom left.
                new(0.7f, -0.2f, 0f), //caffeteria. button. bottom right.
                new(4.3f, 0f, 0f), //cafeteria vent
                new(10f, 3f, 0f), //weapons top
                new(9.5f, -1f, 0f), //weapons bottom
                new(6.5f, -3.5f, 0f), //O2
                new(11.5f, -3.5f, 0f), //O2-nav hall
                new(17.0f, -3.5f, 0f), //navigation top
                new(18.2f, -5.7f, 0f), //navigation bottom
                new(16f, -2f, 0f), //navigation vent
                new(11.5f, -6.5f, 0f), //nav-shields top
                new(9.5f, -8.5f, 0f), //nav-shields bottom
                new(9.2f, -12.2f, 0f), //shields top
                new(8.0f, -14.3f, 0f), //shields bottom
                new(2.5f, -16f, 0f), //coms left
                new(4.2f, -16.4f, 0f), //coms middle
                new(5.5f, -16f, 0f), //coms right
                new(-1.5f, -10.0f, 0f), //storage top
                new(-1.5f, -15.5f, 0f), //storage bottom
                new(-4.5f, -12.5f, 0f), //storrage left
                new(0.3f, -12.5f, 0f), //storrage right
                new(4.5f, -7.5f, 0f), //admin top
                new(4.5f, -9.5f, 0f), //admin bottom
                new(-9.0f, -8.0f, 0f), //elec top left
                new(-6.0f, -8.0f, 0f), //elec top right
                new(-8.0f, -11.0f, 0f), //elec bottom
                new(-12.0f, -13.0f, 0f), //elec-lower hall
                new(-17f, -10f, 0f), //lower engine top
                new(-17.0f, -13.0f, 0f), //lower engine bottom
                new(-21.5f, -3.0f, 0f), //reactor top
                new(-21.5f, -8.0f, 0f), //reactor bottom
                new(-13.0f, -3.0f, 0f), //security top
                new(-12.6f, -5.6f, 0f), // security bottom
                new(-17.0f, 2.5f, 0f), //upper engibe top
                new(-17.0f, -1.0f, 0f), //upper engine bottom
                new(-10.5f, 1.0f, 0f), //upper-mad hall
                new(-10.5f, -2.0f, 0f), //medbay top
                new(-6.5f, -4.5f, 0f) //medbay bottom
            };

            var miraSpawn = new List<Vector3>()
            {
                new(-4.5f, 3.5f, 0f), //launchpad top
                new(-4.5f, -1.4f, 0f), //launchpad bottom
                new(8.5f, -1f, 0f), //launchpad- med hall
                new(14f, -1.5f, 0f), //medbay
                new(16.5f, 3f, 0f), // comms
                new(10f, 5f, 0f), //lockers
                new(6f, 1.5f, 0f), //locker room
                new(2.5f, 13.6f, 0f), //reactor
                new(6f, 12f, 0f), //reactor middle
                new(9.5f, 13f, 0f), //lab
                new(15f, 9f, 0f), //bottom left cross
                new(17.9f, 11.5f, 0f), //middle cross
                new(14f, 17.3f, 0f), //office
                new(19.5f, 21f, 0f), //admin
                new(14f, 24f, 0f), //greenhouse left
                new(22f, 24f, 0f), //greenhouse right
                new(21f, 8.5f, 0f), //bottom right cross
                new(28f, 3f, 0f), //caf right
                new(22f, 3f, 0f), //caf left
                new(19f, 4f, 0f), //storage
                new(22f, -2f, 0f), //balcony
            };

            var polusSpawn = new List<Vector3>()
            {
                new(16.6f, -1f, 0f), //dropship top
                new(16.6f, -5f, 0f), //dropship bottom
                new(20f, -9f, 0f), //above storrage
                new(22f, -7f, 0f), //right fuel
                new(25.5f, -6.9f, 0f), //drill
                new(29f, -9.5f, 0f), //lab lockers
                new(29.5f, -8f, 0f), //lab weather notes
                new(35f, -7.6f, 0f), //lab table
                new(40.4f, -8f, 0f), //lab scan
                new(33f, -10f, 0f), //lab toilet
                new(39f, -15f, 0f), //specimen hall top
                new(36.5f, -19.5f, 0f), //specimen top
                new(36.5f, -21f, 0f), //specimen bottom
                new(28f, -21f, 0f), //specimen hall bottom
                new(24f, -20.5f, 0f), //admin tv
                new(22f, -25f, 0f), //admin books
                new(16.6f, -17.5f, 0f), //office coffe
                new(22.5f, -16.5f, 0f), //office projector
                new(24f, -17f, 0f), //office figure
                new(27f, -16.5f, 0f), //office lifelines
                new(32.7f, -15.7f, 0f), //lavapool
                new(31.5f, -12f, 0f), //snowmad below lab
                new(10f, -14f, 0f), //below storrage
                new(21.5f, -12.5f, 0f), //storrage vent
                new(19f, -11f, 0f), //storrage toolrack
                new(12f, -7f, 0f), //left fuel
                new(5f, -7.5f, 0f), //above elec
                new(10f, -12f, 0f), //elec fence
                new(9f, -9f, 0f), //elec lockers
                new(5f, -9f, 0f), //elec window
                new(4f, -11.2f, 0f), //elec tapes
                new(5.5f, -16f, 0f), //elec-O2 hall
                new(1f, -17.5f, 0f), //O2 tree hayball
                new(3f, -21f, 0f), //O2 middle
                new(2f, -19f, 0f), //O2 gas
                new(1f, -24f, 0f), //O2 water
                new(7f, -24f, 0f), //under O2
                new(9f, -20f, 0f), //right outside of O2
                new(7f, -15.8f, 0f), //snowman under elec
                new(11f, -17f, 0f), //comms table
                new(12.7f, -15.5f, 0f), //coms antenna pult
                new(13f, -24.5f, 0f), //weapons window
                new(15f, -17f, 0f), //between coms-office
                new(17.5f, -25.7f, 0f), //snowman under office
            };

            var dleksSpawn = new List<Vector3>()
            {
                new(2.2f, 2.2f, 0f), //cafeteria. botton. top left.
                new(-0.7f, 2.2f, 0f), //caffeteria. button. top right.
                new(2.2f, -0.2f, 0f), //caffeteria. button. bottom left.
                new(-0.7f, -0.2f, 0f), //caffeteria. button. bottom right.
                new(-10.0f, 3.0f, 0f), //weapons top
                new(-9.0f, 1.0f, 0f), //weapons bottom
                new(-6.5f, -3.5f, 0f), //O2
                new(-11.5f, -3.5f, 0f), //O2-nav hall
                new(-17.0f, -3.5f, 0f), //navigation top
                new(-18.2f, -5.7f, 0f), //navigation bottom
                new(-11.5f, -6.5f, 0f), //nav-shields top
                new(-9.5f, -8.5f, 0f), //nav-shields bottom
                new(-9.2f, -12.2f, 0f), //shields top
                new(-8.0f, -14.3f, 0f), //shields bottom
                new(-2.5f, -16f, 0f), //coms left
                new(-4.2f, -16.4f, 0f), //coms middle
                new(-5.5f, -16f, 0f), //coms right
                new(1.5f, -10.0f, 0f), //storage top
                new(1.5f, -15.5f, 0f), //storage bottom
                new(4.5f, -12.5f, 0f), //storrage left
                new(-0.3f, -12.5f, 0f), //storrage right
                new(-4.5f, -7.5f, 0f), //admin top
                new(-4.5f, -9.5f, 0f), //admin bottom
                new(9.0f, -8.0f, 0f), //elec top left
                new(6.0f, -8.0f, 0f), //elec top right
                new(8.0f, -11.0f, 0f), //elec bottom
                new(12.0f, -13.0f, 0f), //elec-lower hall
                new(17f, -10f, 0f), //lower engine top
                new(17.0f, -13.0f, 0f), //lower engine bottom
                new(21.5f, -3.0f, 0f), //reactor top
                new(21.5f, -8.0f, 0f), //reactor bottom
                new(13.0f, -3.0f, 0f), //security top
                new(12.6f, -5.6f, 0f), // security bottom
                new(17.0f, 2.5f, 0f), //upper engibe top
                new(17.0f, -1.0f, 0f), //upper engine bottom
                new(10.5f, 1.0f, 0f), //upper-mad hall
                new(10.5f, -2.0f, 0f), //medbay top
                new(6.5f, -4.5f, 0f) //medbay bottom
            };

            foreach (var player in PlayerControl.AllPlayerControls)
            {
                if (player.Data.Disconnected || player.Data.IsDead)
                    continue;

                var location = TownOfUsReworked.VanillaOptions.MapId switch
                {
                    0 => skeldSpawn.Random(),
                    1 => miraSpawn.Random(),
                    2 => polusSpawn.Random(),
                    3 => dleksSpawn.Random(),
                    _ => throw new NotImplementedException(),
                };

                player.NetTransform.SnapTo(new(location.x, location.y));
            }
        }
    }
}