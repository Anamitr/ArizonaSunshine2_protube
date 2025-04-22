using MelonLoader;
using System.Text;
using HarmonyLib;
using Il2CppVertigo.AZS2.Client;
using Newtonsoft.Json;

[assembly: MelonInfo(typeof(ArizonaSunshine2_protube.ArizonaSunshine2_protube), "ArizonaSunshine2_protube", "1.0.0", "Astien & Florian Fahrenberger")]
[assembly: MelonGame("Vertigo Games", "ArizonaSunshine2")]

namespace ArizonaSunshine2_protube
{
    public class ArizonaSunshine2_protube : MelonMod
    {
        private enum WeaponType
        {
            Pistol, Revolver, Tommy, Rifle, M16, Shotgun1Hand, Shotgun2Hand, Undefined,
            TommyUpgraded, // I saw this one at the end of chapter 7, but it dissapeared (bug)
            GrenageLauncher
        }

        private static readonly string devicesIDsConfigPath = Directory.GetCurrentDirectory() + "\\Mods\\rifleBoltButtDevices\\";

        public override void OnInitializeMelon()
        {
            InitializeProTube();
        }

        private static void SaveChannel(string channelName, string proTubeName)
        {
            System.IO.Directory.CreateDirectory(devicesIDsConfigPath);
            string fileName = devicesIDsConfigPath + channelName + ".pro";
            File.WriteAllText(fileName, proTubeName, Encoding.UTF8);
        }

        private static string ReadChannel(string channelName)
        {
            string fileName = devicesIDsConfigPath + channelName + ".pro";
            if (!File.Exists(fileName)) return "";
            return File.ReadAllText(fileName, Encoding.UTF8);
        }

        private static void RifleBoltButtSort()
        // For me assigning rifleBolt and rifleButt by ForceTubeVRInterface seemed random,
        // so if when shooting pistol your stock respond then swap contents of "Arizona Sunshine 2\Mods\rifleBoltButtDevices" .pro files.
        // Then it should be good as long as the same devices are used.
        {
            ForceTubeVRInterface.FTChannelFile myChannels = JsonConvert.DeserializeObject<ForceTubeVRInterface.FTChannelFile>(ForceTubeVRInterface.ListChannels());
            MelonLogger.Msg($"myChannels: {myChannels}");
            if (myChannels == null)
            {
                MelonLogger.Warning("myChannels is null!");
            }
            if (myChannels.channels == null)
            {
                MelonLogger.Warning("myChannels.channels is null!");
            }
            if (myChannels.channels.pistol1 == null)
            {
                MelonLogger.Warning("myChannels.channels.pistol1 is null!");
            }
            MelonLogger.Msg($"myChannels.channels: {myChannels.channels}");

            var pistol1 = myChannels.channels.pistol1;
            var pistol2 = myChannels.channels.pistol2;
            MelonLogger.Msg($"{myChannels.channels.ToString()}");
            MelonLogger.Msg("rifleButt");
            MelonLogger.Msg($"{myChannels.channels.rifleButt.Count}");
            MelonLogger.Msg("rifleBolt");
            MelonLogger.Msg($"{myChannels.channels.rifleBolt.Count}");
            MelonLogger.Msg("pistol1");
            MelonLogger.Msg($"{myChannels.channels.pistol1.Count}");
            MelonLogger.Msg("pistol2");
            MelonLogger.Msg($"{myChannels.channels.pistol2.Count}");
            MelonLogger.Msg("other");
            MelonLogger.Msg($"{myChannels.channels.other.Count}");
            MelonLogger.Msg("vest");
            MelonLogger.Msg($"{myChannels.channels.vest.Count}");

            var rifleBolt = myChannels.channels.rifleBolt;
            var rifleButt = myChannels.channels.rifleButt;
            if ((ReadChannel("rifleBolt") == "") || (ReadChannel("rifleButt") == ""))
            {
                MelonLogger.Msg("No configuration files found, saving current rifleBolt and rifleButt.");
                SaveChannel("rifleBolt", rifleBolt[0].name);
                SaveChannel("rifleButt", rifleButt[0].name);
            }
            else
            {
                string rifleBoltID = ReadChannel("rifleBolt");
                string rifleButtId = ReadChannel("rifleButt");
                MelonLogger.Msg("Found and loaded configuration. Rifle bolt: " + rifleBoltID + ", Rifle butt: " + rifleButtId);
                // Channels 2 and 3 are ForceTubeVRChannel.rifleButt and rifleBoltID
                // https://github.com/Astienth/Provolver_HalfLifeAlyx/blob/8a0d9760cd87c818cde391d87c0a743bc1e968c7/ForceTubeVRInterface.cs#L18
                ForceTubeVRInterface.ClearChannel(2);
                ForceTubeVRInterface.ClearChannel(3);
                ForceTubeVRInterface.AddToChannel(2, rifleButtId);
                ForceTubeVRInterface.AddToChannel(3, rifleBoltID);
            }
        }
        private static async void InitializeProTube()
        {
            MelonLogger.Msg("Initializing ProTube gear...");
            await ForceTubeVRInterface.InitAsync(pistolsFirst: false);
            MelonLogger.Msg("Listing devices and channels:");
            MelonLogger.Msg("ListConnectedForceTube: " + ForceTubeVRInterface.ListConnectedForceTube());
            MelonLogger.Msg("ListChannels: " + ForceTubeVRInterface.ListChannels());
            Thread.Sleep(10000);
            RifleBoltButtSort();
        }


        private static void ShootProtube(WeaponType weaponType)
        // Set reactions to my feeling, I was using weaker basic ProVolver and ForceTube (there are stronger versions),
        // so I prefered stronger responses
        // Only guide I found is this: https://github.com/ProTubeVR/ForceTubeVR-Unreal-Engine-Plugin?tab=readme-ov-file#weapons-settings
        {
            MelonLogger.Msg("shootProtube");
            MelonLogger.Msg(weaponType);

            switch (weaponType)
            {
                case WeaponType.Rifle:
                case WeaponType.M16:
                    ForceTubeVRInterface.Shoot(255, 125, 0.1f, ForceTubeVRChannel.rifleBolt);
                    ForceTubeVRInterface.Kick(255, ForceTubeVRChannel.rifleButt);
                    break;

                case WeaponType.Tommy:
                    ForceTubeVRInterface.Shoot(135, 100, 0.1f, ForceTubeVRChannel.rifleBolt);
                    ForceTubeVRInterface.Kick(180, ForceTubeVRChannel.rifleButt);
                    break;

                case WeaponType.Pistol:
                    ForceTubeVRInterface.Kick(190, ForceTubeVRChannel.rifleBolt);
                    break;

                case WeaponType.Revolver:
                    ForceTubeVRInterface.Kick(230, ForceTubeVRChannel.rifleBolt);
                    break;

                case WeaponType.Shotgun1Hand:
                    ForceTubeVRInterface.Kick(255, ForceTubeVRChannel.rifleBolt);
                    break;

                case WeaponType.Shotgun2Hand:
                    ForceTubeVRInterface.Kick(255, ForceTubeVRChannel.rifleBolt);
                    ForceTubeVRInterface.Kick(255, ForceTubeVRChannel.rifleButt);
                    break;

                case WeaponType.GrenageLauncher:
                    ForceTubeVRInterface.Rumble(200, 0.2f, ForceTubeVRChannel.rifleBolt);
                    ForceTubeVRInterface.Kick(255, ForceTubeVRChannel.rifleButt);
                    break;

                default:
                    ForceTubeVRInterface.Kick(190, ForceTubeVRChannel.rifleBolt);
                    break;
            }

            return;
        }


        [HarmonyPatch(typeof(ProjectileShootStrategyBehaviourData), "PlayShootHapticsForHand", new Type[] { typeof(AZS2Hand) })]
        public class bhaptics_Recoil
        {
            [HarmonyPostfix]
            public static void Postfix(ProjectileShootStrategyBehaviourData __instance, AZS2Hand hand)
            {
                if (hand.IsLeftHand)
                {
                    MelonLogger.Warning("Firing with left hand is not handled.");
                    return;
                }
                // I noticed differences between weapons only in those 2 parameters
                float fireRate = __instance.shootStrategy.fireRate;
                float spreadAngle = __instance.shootStrategy.spreadAngle;
                MelonLogger.Msg("fireRate: " + fireRate);
                MelonLogger.Msg("spreadAngle: " + spreadAngle);
                // I didn't notice differences between any weapons in those 2
                MelonLogger.Msg("projectilesPerBurst: " + __instance.shootStrategy.projectilesPerBurst); // Saw always 3
                MelonLogger.Msg("hasSpreadPattern: " + __instance.shootStrategy.hasSpreadPattern); // Saw always False
                // I didn't print those through most of the game
                MelonLogger.Msg("burstFireCooldownDuration: " + __instance.shootStrategy.burstFireCooldownDuration);
                MelonLogger.Msg("bulletInChamberCooldownDuration: " + __instance.shootStrategy.bulletInChamberCooldownDuration);
                MelonLogger.Msg("maxIndividualBulletRandomizedAngle: " + __instance.shootStrategy.maxIndividualBulletRandomizedAngle);

                WeaponType weaponType = WeaponType.Undefined;
                switch (fireRate)
                {
                    case 10:
                    case 11:
                        weaponType = WeaponType.Rifle;
                        break;

                    case 15:
                        // Watch out, big UZI is the same, but I was using fast firing and shotgun weapons in my left hand
                        weaponType = WeaponType.M16;
                        break;

                    case 12:
                        weaponType = WeaponType.Tommy;
                        break;

                    case 30:
                        weaponType = WeaponType.Pistol;
                        break;

                    case 5.5f:
                        weaponType = WeaponType.Revolver;
                        break;

                    case 100 when spreadAngle == 5f:
                        weaponType = WeaponType.Shotgun1Hand;
                        break;

                    case 100 when spreadAngle == 2.75f:
                        weaponType = WeaponType.Shotgun2Hand;
                        break;

                    case 1 when spreadAngle == 20:
                        weaponType = WeaponType.GrenageLauncher;
                        break;
                }

                ShootProtube(weaponType);
            }
        }
    }
}
// UZI
//[20:31:32.295] fireRate: 15
//[20:31:32.295] projectilesPerBurst: 3
//[20:31:32.296] spreadAngle: 0
//[20:31:32.296] hasSpreadPattern: False
//[20:31:32.297][ArizonaSunshine2_protube] shootProtube
//[20:31:32.297][ArizonaSunshine2_protube] Undefined
// Seems exactly the same as M16

// Minigun
//[19:15:41.202] fireRate: 25
//[19:15:41.203] projectilesPerBurst: 3
//[19:15:41.203] spreadAngle: 0
//[19:15:41.203] hasSpreadPattern: False

// LMG
//[16:56:14.082] fireRate: 12
//[16:56:14.083] projectilesPerBurst: 3
//[16:56:14.083] spreadAngle: 0
//[16:56:14.083] hasSpreadPattern: False
//[16:56:14.084][ArizonaSunshine2_protube] shootProtube
//[16:56:14.084][ArizonaSunshine2_protube] Tommy

// Grenade launcher
//[19:34:38.126] fireRate: 1
//[19:34:38.127] projectilesPerBurst: 3
//[19:34:38.127] spreadAngle: 20
//[19:34:38.127] hasSpreadPattern: False
//[19:34:38.127][ArizonaSunshine2_protube] shootProtube
//[19:34:38.127][ArizonaSunshine2_protube] Undefined
