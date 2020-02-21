
using System;
using System.IO;
using System.Xml.Serialization;
using TaleWorlds.Library;

namespace EnhancedBattleTest
{
    public class EnhancedBattleTestConfig : BattleConfigBase<EnhancedBattleTestConfig>
    {
        public class SceneInfo
        {
            public string name;
            public Vec2 formationPosition;
            public Vec2 formationDirection;
            public float skyBrightness = -1;
            public int soldiersPerRow = 20;
            public float rainDensity = -1;
        }

        private static EnhancedBattleTestConfig _instance;

        public SceneInfo[] sceneList;
        public int sceneIndex;
        public float distance;
        public float soldierXInterval, soldierYInterval;

        protected static Version BinaryVersion => new Version(1, 0);

        protected void UpgradeToCurrentVersion()
        {
            switch (ConfigVersion?.ToString())
            {
                case "1.0": break;
                default:
                    Utility.DisplayMessage("Config version not compatible.\nReset config.");
                    ResetToDefault();
                    Serialize();
                    break;
            }
        }

        [XmlIgnore]
        public int SoldiersPerRow
        {
            get => sceneList[sceneIndex].soldiersPerRow;
            set => sceneList[sceneIndex].soldiersPerRow = value;
        }

        [XmlIgnore]
        public Vec2 FormationPosition
        {
            get => sceneList[sceneIndex].formationPosition;
            set => sceneList[sceneIndex].formationPosition = value;
        }

        [XmlIgnore]
        public Vec2 FormationDirection
        {
            get => sceneList[sceneIndex].formationDirection;
            set => sceneList[sceneIndex].formationDirection = value;
        }

        [XmlIgnore]
        public float SkyBrightness
        {
            get => sceneList[sceneIndex].skyBrightness;
            set => sceneList[sceneIndex].skyBrightness = value;
        }

        [XmlIgnore]
        public float RainDensity
        {
            get => sceneList[sceneIndex].rainDensity;
            set => sceneList[sceneIndex].rainDensity = value;
        }
        
        public bool useFreeCamera;

        [XmlIgnore]
        public string SceneName => sceneList[sceneIndex].name;

        private static EnhancedBattleTestConfig CreateDefault()
        {
            //string sceneIndex = "mp_skirmish_map_001a";
            //string sceneIndex = "mp_sergeant_map_001";
            // string sceneIndex = "mp_test_bora";
            // string sceneIndex = "battle_test";
            // string sceneIndex = "mp_duel_001_winter";
            // string sceneIndex = "mp_sergeant_map_001";
            // string sceneIndex = "mp_tdm_map_001";
            // string sceneIndex = "scn_world_map";
            // string sceneIndex = "mp_compact";
            SceneInfo[] list = new[]
            {
                //"mp_skirmish_map_001a",
                //"mp_tdm_map_001",
                //"mp_duel_001",
                //"mp_duel_001_winter",
                //"mp_duel_002",
                //"mp_ruins_2",
                new SceneInfo{name = "mp_skirmish_map_001a", formationPosition = new Vec2(100, 100), formationDirection = new Vec2(1, 0)},
                new SceneInfo{name = "mp_tdm_map_001", formationPosition = new Vec2(385, 570), formationDirection = new Vec2(0.7f, -0.3f).Normalized()},
                new SceneInfo{name = "mp_duel_001", formationPosition = new Vec2(567, 600), formationDirection = new Vec2(0, 10)},
                new SceneInfo{name = "mp_duel_001_winter", formationPosition = new Vec2(567, 600), formationDirection = new Vec2(0, 1)},
                new SceneInfo{name = "mp_ruins_2", formationPosition = new Vec2(514, 470), formationDirection = new Vec2(1, 0)},
                //"mp_sergeant_map_001"
                //"mp_sergeant_map_005",
                //"mp_sergeant_map_007",
                //"mp_sergeant_map_008",
                //"mp_sergeant_map_009",
                //"mp_sergeant_map_010",
                //"mp_sergeant_map_011",
                //"mp_sergeant_map_011s",
                //"mp_sergeant_map_012",
                //"mp_sergeant_map_013",
                //"mp_sergeant_map_vlandia_01",
                //"mp_siege_map_001",
                //"mp_siege_map_002",
                //"mp_siege_map_003",
                //"mp_siege_map_004",
                //"mp_siege_map_005",
                new SceneInfo{name = "mp_sergeant_map_001", formationPosition = new Vec2(250, 500), formationDirection = new Vec2(1, 0)},
                //new SceneInfo{name = "mp_sergeant_map_005", formationPosition = new Vec2(330, 439), formationDirection = new Vec2(1, 0)},
                new SceneInfo{name = "mp_sergeant_map_007", formationPosition = new Vec2(330, 439), formationDirection = new Vec2(1, 0)},
                new SceneInfo{name = "mp_sergeant_map_008", formationPosition = new Vec2(471,505), formationDirection = new Vec2(1, 0)},
                new SceneInfo{name = "mp_sergeant_map_009", formationPosition = new Vec2(530,503), formationDirection = new Vec2(0,1)},
                //new SceneInfo{name = "mp_sergeant_map_010", formationPosition = new Vec2(391,376), formationDirection = new Vec2(0,1)},
                new SceneInfo{name = "mp_sergeant_map_011", formationPosition = new Vec2(485,364), formationDirection = new Vec2(0.4f,0.6f)},
                new SceneInfo{name = "mp_sergeant_map_011s", formationPosition = new Vec2(485,364), formationDirection = new Vec2(0.4f,0.6f)},
                //new SceneInfo{name = "mp_sergeant_map_012", formationPosition = new Vec2(580,576), formationDirection = new Vec2(1, 0)},
                new SceneInfo{name = "mp_sergeant_map_013", formationPosition = new Vec2(580,576), formationDirection = new Vec2(1, 0)},
                new SceneInfo{name = "mp_sergeant_map_vlandia_01", formationPosition = new Vec2(485,364), formationDirection = new Vec2(0.4f,0.6f)},
                //new SceneInfo{name = "mp_siege_map_001", formationPosition = new Vec2(100, 100), formationDirection = new Vec2(1, 0)},
                //new SceneInfo{name = "mp_siege_map_002", formationPosition = new Vec2(100, 100), formationDirection = new Vec2(1, 0)},
                //new SceneInfo{name = "mp_siege_map_003", formationPosition = new Vec2(100, 100), formationDirection = new Vec2(1, 0)},
                //new SceneInfo{name = "mp_siege_map_004", formationPosition = new Vec2(100, 100), formationDirection = new Vec2(1, 0)},
                //new SceneInfo{name = "mp_siege_map_005", formationPosition = new Vec2(100, 100), formationDirection = new Vec2(1, 0)},
                //"mp_skirmish_map_002f",
                //"mp_skirmish_map_002_winter",
                //"mp_skirmish_map_004",
                //"mp_skirmish_map_005",
                //"mp_skirmish_map_006",
                //"mp_skirmish_map_007",
                //"mp_skirmish_map_007_winter",
                //"mp_skirmish_map_008",
                //"mp_skirmish_map_009",
                //"mp_skirmish_map_010",
                //"mp_skirmish_map_013",
                //"mp_skirmish_map_battania_02",
                //"mp_skirmish_map_battania_03"
                new SceneInfo{name = "mp_skirmish_map_002f", formationPosition = new Vec2(415,490), formationDirection = new Vec2(0.3f, 0.7f).Normalized(), soldiersPerRow = 10},
                new SceneInfo{name = "mp_skirmish_map_002_winter", formationPosition = new Vec2(415,490), formationDirection = new Vec2(0.3f, 0.7f), soldiersPerRow = 10},
                new SceneInfo{name = "mp_skirmish_map_004", formationPosition = new Vec2(320,288), formationDirection = new Vec2(0,1), soldiersPerRow = 10},
                new SceneInfo{name = "mp_skirmish_map_005", formationPosition = new Vec2(477,496), formationDirection = new Vec2(1, 0), soldiersPerRow = 10},
                new SceneInfo{name = "mp_skirmish_map_006", formationPosition = new Vec2(480,561), formationDirection = new Vec2(1, 0), skyBrightness = 0},
                new SceneInfo{name = "mp_skirmish_map_007", formationPosition = new Vec2(192,185), formationDirection = new Vec2(0, 1)},
                new SceneInfo{name = "mp_skirmish_map_007_winter", formationPosition = new Vec2(192,185), formationDirection = new Vec2(0, 1)},
                //new SceneInfo{name = "mp_skirmish_map_008", formationPosition = new Vec2(100, 100), formationDirection = new Vec2(1, 0)},
                //new SceneInfo{name = "mp_skirmish_map_009", formationPosition = new Vec2(100, 100), formationDirection = new Vec2(1, 0)},
                //new SceneInfo{name = "mp_skirmish_map_010", formationPosition = new Vec2(100, 100), formationDirection = new Vec2(1, 0)},
                new SceneInfo{name = "mp_skirmish_map_013", formationPosition = new Vec2(250, 500), formationDirection = new Vec2(1, 0)},
                new SceneInfo{name = "mp_skirmish_map_battania_02", formationPosition = new Vec2(360,186), formationDirection = new Vec2(0.6f,-0.4f), soldiersPerRow = 10},
                new SceneInfo{name = "mp_skirmish_map_battania_03", formationPosition = new Vec2(360,186), formationDirection = new Vec2(0.6f,-0.4f), soldiersPerRow = 10},
            };
            int defaultIndex = 0;
            var p = new EnhancedBattleTestConfig
            {
                ConfigVersion = BinaryVersion.ToString(2),
                sceneList = list,
                sceneIndex = defaultIndex,
                playerSoldierCount = 20,
                enemySoldierCount = 20,
                distance = 50,
                soldierXInterval = 1.5f,
                soldierYInterval = 1f,
                playerClass = new ClassInfo{ classStringId = "mp_light_cavalry_vlandia", selectedFirstPerk = 0, selectedSecondPerk = 0},
                playerTroopClass = new ClassInfo{classStringId = "mp_heavy_infantry_vlandia", selectedFirstPerk = 0, selectedSecondPerk = 0 },
                enemyTroopClass = new ClassInfo { classStringId = "mp_shock_infantry_vlandia", selectedFirstPerk = 0, selectedSecondPerk = 0 },
                useFreeCamera = false
            };
            return p;
        }

        public static EnhancedBattleTestConfig Get()
        {
            if (_instance == null)
            {
                _instance = new EnhancedBattleTestConfig();
                _instance.SyncWithSave();
            }

            return _instance;
        }

        public override bool Validate() {
            return base.Validate()
                && this.sceneIndex >= 0 && this.sceneIndex < this.sceneList.Length
                && this.distance > 0
                && soldierXInterval > 0
                && soldierYInterval > 0
                && SoldiersPerRow > 0
                && FormationDirection.Length > 0;
        }

        public override bool Serialize()
        {
            try
            {
                EnsureSaveDirectory();
                XmlSerializer serializer = new XmlSerializer(typeof(EnhancedBattleTestConfig));
                using (TextWriter writer = new StreamWriter(SaveName))
                {
                    serializer.Serialize(writer, this);
                }
                Utility.DisplayMessage("Config saved.");
                return true;
            }
            catch (Exception e)
            {
                Utility.DisplayMessage("Error: Saving config failed.");
                Console.WriteLine(e);
            }

            return false;
        }

        public override bool Deserialize()
        {
            try
            {
                EnsureSaveDirectory();
                XmlSerializer deserializer = new XmlSerializer(typeof(EnhancedBattleTestConfig));
                using (TextReader reader = new StreamReader(SaveName))
                {
                    var config = (EnhancedBattleTestConfig)deserializer.Deserialize(reader);
                    this.CopyFrom(config);
                }
                Utility.DisplayMessage("Config loaded.");
                UpgradeToCurrentVersion();
                return true;
            }
            catch (Exception e)
            {
                Utility.DisplayMessage("Error: Loading config failed.");
                Console.WriteLine(e);
            }

            return false;
        }

        public override void ReloadSavedConfig()
        {
            var loadedConfig = CreateDefault();
            if (loadedConfig.Deserialize())
                CopyFrom(loadedConfig);
        }

        public override void ResetToDefault()
        {
            CopyFrom(CreateDefault());
        }


        protected override void CopyFrom(EnhancedBattleTestConfig other)
        {
            //public SceneInfo[] sceneList { get; set; }
            //public int sceneIndex;
            //public int playerSoldierCount, enemySoldierCount;
            //public float distance;
            //public float soldierXInterval, soldierYInterval;
            ////public int soldiersPerRow;
            ////public Vec2 formationPosition;
            ////public Vec2 formationDirection;
            ////public float skyBrightness;
            ////public float rainDensity;
            //[XmlElement("PlayerStringId")]
            //public string playerHeroClassStringId;
            //public int playerSelectedPerk;
            //[XmlElement("PlayerTroopStringId")]
            //public string playerTroopHeroClassStringId;
            //public int playerTroopSelectedPerk;
            //[XmlElement("EnemyTroopStringId")]
            //public string enemyTroopHeroClassStringId;
            //public int enemyTroopSelectedPerk;
            //public bool useFreeCamera;
            base.CopyFrom(other);

            if (other.sceneList != null)
                this.sceneList = other.sceneList;
            this.sceneIndex = other.sceneIndex;
            this.distance = other.distance;
            this.soldierXInterval = other.soldierXInterval;
            this.soldierYInterval = other.soldierYInterval;
            this.useFreeCamera = other.useFreeCamera;
        }
        protected override string SaveName => SavePath + "EnhancedBattleTestConfig.xml";
        protected override string[] OldNames { get; } = { SavePath + "Param.xml" };
    }
}