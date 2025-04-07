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
        public static string configPath = Directory.GetCurrentDirectory() + "\\Mods\\dualwield\\";
        public static bool dualWield = false;
        private MelonPreferences_Category config;
        public static bool leftHanded = false;

        public override void OnApplicationStart()
        {
            config = MelonPreferences.CreateCategory("provolver");
            config.CreateEntry<bool>("leftHanded", false);
            config.SetFilePath("Mods/Provolver/Provolver_config.cfg");
            leftHanded = bool.Parse(config.GetEntry("leftHanded").GetValueAsString());
            InitializeProTube();
        }

        public static void saveChannel(string channelName, string proTubeName)
        {
            string fileName = configPath + channelName + ".pro";
            File.WriteAllText(fileName, proTubeName, Encoding.UTF8);
        }

        public static string readChannel(string channelName)
        {
            string fileName = configPath + channelName + ".pro";
            if (!File.Exists(fileName)) return "";
            return File.ReadAllText(fileName, Encoding.UTF8);
        }

        public static void dualWieldSort()
        {
            ForceTubeVRInterface.FTChannelFile myChannels = JsonConvert.DeserializeObject<ForceTubeVRInterface.FTChannelFile>(ForceTubeVRInterface.ListChannels());
            MelonLogger.Msg($"myChannels: {myChannels}");
            if (myChannels == null)
            {
                MelonLogger.LogWarning("myChannels is null!");
            }
            if (myChannels.channels == null)
            {
                MelonLogger.LogWarning("myChannels.channels is null!");
            }
            if (myChannels.channels.pistol1 == null)
            {
                MelonLogger.LogWarning("myChannels.channels.pistol1 is null!");
            }
            MelonLogger.Msg($"myChannels.channels: {myChannels.channels}");

            var pistol1 = myChannels.channels.pistol1;
            MelonLogger.Msg("Bazinga 2");
            var pistol2 = myChannels.channels.pistol2;
            MelonLogger.Msg("Bazinga 3");
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
            if ((pistol1.Count > 0) && (pistol2.Count > 0))
            {
                dualWield = true;
                MelonLogger.Msg("Two ProTube devices detected, player is dual wielding.");
                if ((readChannel("rightHand") == "") || (readChannel("leftHand") == ""))
                {
                    MelonLogger.Msg("No configuration files found, saving current right and left hand pistols.");
                    saveChannel("rightHand", pistol1[0].name);
                    saveChannel("leftHand", pistol2[0].name);
                }
                else
                {
                    string rightHand = readChannel("rightHand");
                    string leftHand = readChannel("leftHand");
                    MelonLogger.Msg("Found and loaded configuration. Right hand: " + rightHand + ", Left hand: " + leftHand);
                    // Channels 4 and 5 are ForceTubeVRChannel.pistol1 and pistol2
                    ForceTubeVRInterface.ClearChannel(4);
                    ForceTubeVRInterface.ClearChannel(5);
                    ForceTubeVRInterface.AddToChannel(4, rightHand);
                    ForceTubeVRInterface.AddToChannel(5, leftHand);
                }
            }
            else
            {
                MelonLogger.Msg("SINGLE WIELD");
            }
        }
        private async void InitializeProTube()
        {
            MelonLogger.Msg("Initializing ProTube gear...");
            await ForceTubeVRInterface.InitAsync(pistolsFirst: false); 
            MelonLogger.Msg("Listing devices and channels:");
            MelonLogger.Msg("ListConnectedForceTube: " + ForceTubeVRInterface.ListConnectedForceTube());
            MelonLogger.Msg("ListChannels: " + ForceTubeVRInterface.ListChannels());
            Thread.Sleep(10000);
            //dualWieldSort();
        }

        public enum WeaponType
        {
            Pistol, Revolver, Tommy, Rifle, Shotgun1Hand, Shotgun2Hand, Undefined,
                TommyUpgraded // I saw this one at the end of chapter 7, but it dissapeared (bug)
        }

        public static void shootProtube(WeaponType weaponType, bool isRightHand)
        // My gun get's registered as rifleButt, stock as rifleBolt
        {
            MelonLogger.Msg("shootProtube");
            MelonLogger.Msg(weaponType);
            //ForceTubeVRChannel channel = ForceTubeVRChannel.pistol1;
            //if (isRightHand)
            //{
            //    channel = (leftHanded && !dualWield) ? ForceTubeVRChannel.pistol2 : ForceTubeVRChannel.pistol1;
            //}
            //else
            //{
            //    channel = (leftHanded && !dualWield) ? ForceTubeVRChannel.pistol1 : ForceTubeVRChannel.pistol2;
            //}
            if (!isRightHand)
            {
                MelonLogger.Warning("Firing with left hand is not handled.");
                return;
            }

            if (weaponType == WeaponType.Rifle)
            {
                ForceTubeVRInterface.Shoot(255, 125, 0.1f, ForceTubeVRChannel.rifleButt);
                ForceTubeVRInterface.Kick(255, ForceTubeVRChannel.rifleBolt);
            }
            else if (weaponType == WeaponType.Tommy)
            {
                ForceTubeVRInterface.Shoot(135, 100, 0.1f, ForceTubeVRChannel.rifleButt);
                ForceTubeVRInterface.Kick(180, ForceTubeVRChannel.rifleBolt);
            }
            else if (weaponType == WeaponType.Pistol)
            {
                ForceTubeVRInterface.Kick(190, ForceTubeVRChannel.rifleButt);
            }
            else if (weaponType == WeaponType.Revolver)
            {
                ForceTubeVRInterface.Kick(230, ForceTubeVRChannel.rifleButt);
            }
            else if (weaponType == WeaponType.Shotgun1Hand)
            {
                ForceTubeVRInterface.Kick(255, ForceTubeVRChannel.rifleButt);
            }
            else if (weaponType == WeaponType.Shotgun2Hand)
            {
                ForceTubeVRInterface.Kick(255, ForceTubeVRChannel.rifleButt);
                ForceTubeVRInterface.Kick(255, ForceTubeVRChannel.rifleBolt);
            }
            else
            {
                ForceTubeVRInterface.Kick(190, ForceTubeVRChannel.rifleButt);
            }
            return;
        }


        [HarmonyPatch(typeof(ProjectileShootStrategyBehaviourData), "PlayShootHapticsForHand", new Type[] { typeof(AZS2Hand) })]
        public class bhaptics_Recoil
        {
            [HarmonyPostfix]
            public static void Postfix(ProjectileShootStrategyBehaviourData __instance, AZS2Hand hand)
            {
                //string weapon = "Pistol";
                //if (__instance.shootStrategy.projectilesPerBurst > 1) weapon = "Shotgun";
                MelonLogger.Msg("fireRate: " + __instance.shootStrategy.fireRate);
                //MelonLogger.Msg("firingMode: " + __instance.shootStrategy.firingMode);
                //MelonLogger.Msg("onShootUpdate: " + __instance.shootStrategy.onShootUpdate);
                MelonLogger.Msg("projectilesPerBurst: " + __instance.shootStrategy.projectilesPerBurst);
                MelonLogger.Msg("spreadAngle: " + __instance.shootStrategy.spreadAngle);
                MelonLogger.Msg("hasSpreadPattern: " + __instance.shootStrategy.hasSpreadPattern);
                //__instance.shootStrategy.firingMode;
                ////__instance.shootStrategy.fireRate;
                bool isRightHand = (hand.IsRightHand);

                WeaponType weaponType = WeaponType.Undefined;
                if (__instance.shootStrategy.fireRate == 10) weaponType = WeaponType.Rifle;
                else if (__instance.shootStrategy.fireRate == 12) weaponType = WeaponType.Tommy;
                else if (__instance.shootStrategy.fireRate == 30) weaponType = WeaponType.Pistol;
                else if (__instance.shootStrategy.fireRate == 5.5f) weaponType = WeaponType.Revolver;
                else if (__instance.shootStrategy.fireRate == 100)
                {
                    if (__instance.shootStrategy.spreadAngle == 5f)
                        weaponType = WeaponType.Shotgun1Hand;
                    else if (__instance.shootStrategy.spreadAngle == 2.75f)
                        weaponType = WeaponType.Shotgun2Hand;
                }

                shootProtube(weaponType, isRightHand);
            }
        }
    }
}
