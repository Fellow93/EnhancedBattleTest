﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Serialization;
using TaleWorlds.Core;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade;

namespace EnhancedBattleTest
{
    public class ClassInfo
    {
        public string classStringId;
        public int selectedFirstPerk;
        public int selectedSecondPerk;
        public int troopCount;
    }
    public abstract class BattleConfigBase<T> where T : BattleConfigBase<T>
    {
        public string ConfigVersion { get; set; }


        public ClassInfo playerClass;
        public ClassInfo enemyClass;
        public bool spawnEnemyCommander;
        public ClassInfo[] playerTroops;
        public ClassInfo[] enemyTroops;
        public bool useFreeCamera;

        [XmlIgnore]
        public MultiplayerClassDivisions.MPHeroClass PlayerHeroClass
        {
            get => MBObjectManager.Instance.GetObject<MultiplayerClassDivisions.MPHeroClass>(playerClass.classStringId);
            set => playerClass.classStringId = value.StringId;
        }
        [XmlIgnore]
        public MultiplayerClassDivisions.MPHeroClass EnemyHeroClass
        {
            get => MBObjectManager.Instance.GetObject<MultiplayerClassDivisions.MPHeroClass>(enemyClass.classStringId);
            set => enemyClass.classStringId = value.StringId;
        }

        public void SetPlayerTroopHeroClass(int i, MultiplayerClassDivisions.MPHeroClass heroClass)
        {
            playerTroops[i].classStringId = heroClass.StringId;
        }

        public MultiplayerClassDivisions.MPHeroClass GetPlayerTroopHeroClass(int i)
        {
            return MBObjectManager.Instance.GetObject<MultiplayerClassDivisions.MPHeroClass>(playerTroops[i]
                .classStringId);
        }

        public void SetEnemyTroopHeroClass(int i, MultiplayerClassDivisions.MPHeroClass heroClass)
        {
            enemyTroops[i].classStringId = heroClass.StringId;
        }

        public MultiplayerClassDivisions.MPHeroClass GetEnemyTroopHeroClass(int i)
        {
            return MBObjectManager.Instance.GetObject<MultiplayerClassDivisions.MPHeroClass>(enemyTroops[i]
                .classStringId);
        }

        public BasicCultureObject GetPlayerTeamCulture()
        {
            if (!useFreeCamera)
                return PlayerHeroClass.Culture;
            for (int i = 0; i < 3; ++i)
            {
                if (playerTroops[i].troopCount != 0)
                    return GetPlayerTroopHeroClass(i).Culture;
            }

            return null;
        }

        public BasicCultureObject GetEnemyTeamCulture()
        {
            if (!useFreeCamera)
                return EnemyHeroClass.Culture;
            for (int i = 0; i < 3; ++i)
            {
                if (enemyTroops[i].troopCount != 0)
                    return GetEnemyTroopHeroClass(i).Culture;
            }

            return null;
        }

        public virtual bool Validate()
        {
            return PlayerHeroClass != null && enemyClass != null
                   && playerTroops.All(classInfo =>
                       MBObjectManager.Instance.GetObject<MultiplayerClassDivisions.MPHeroClass>(
                           classInfo.classStringId) != null && classInfo.troopCount >= 0 &&
                       classInfo.selectedFirstPerk >= 0 && classInfo.selectedFirstPerk <= 2 &&
                       classInfo.selectedSecondPerk >= 0 && classInfo.selectedSecondPerk <= 2);
        }


        public abstract bool Serialize();

        public abstract bool Deserialize();

        public abstract void ReloadSavedConfig();

        public abstract void ResetToDefault();

        protected virtual void CopyFrom(T other)
        {
            ConfigVersion = other.ConfigVersion;
            if (other.playerClass != null)
                this.playerClass = other.playerClass;
            if (other.enemyClass != null)
                this.enemyClass = other.enemyClass;
            this.spawnEnemyCommander = other.spawnEnemyCommander;
            if (other.playerTroops != null)
                this.playerTroops = other.playerTroops;
            if (other.enemyTroops != null)
                this.enemyTroops = other.enemyTroops;
            this.useFreeCamera = other.useFreeCamera;
        }

        protected void EnsureSaveDirectory()
        {
            Directory.CreateDirectory(SavePath);
        }

        protected void SyncWithSave()
        {
            if (File.Exists(SaveName) && Deserialize())
            {
                RemoveOldConfig();
                return;
            }

            MoveOldConfig();
            if (File.Exists(SaveName) && Deserialize())
                return;
            Utility.DisplayMessage("No config file found.\nCreate default config.");
            ResetToDefault();
            Serialize();
        }

        private void RemoveOldConfig()
        {
            foreach (var oldName in OldNames)
            {
                if (File.Exists(oldName))
                {
                    Utility.DisplayMessage($"Found old config file: \"{oldName}\".");
                    Utility.DisplayMessage("Delete the old config file.");
                    File.Delete(oldName);
                }
            }
        }

        private void MoveOldConfig()
        {
            string firstOldName = OldNames.FirstOrDefault(File.Exists);
            if (firstOldName != null && !firstOldName.IsEmpty())
            {
                Utility.DisplayMessage($"Found old config file: \"{firstOldName}\".");
                Utility.DisplayMessage("Rename old config file to new name...");
                File.Move(firstOldName, SaveName);
            }
            RemoveOldConfig();
        }


        private static string ApplicationName = "Mount and Blade II Bannerlord";
        private static string ModuleName = "EnhancedBattleTest";

        protected static string SavePath => Environment.GetFolderPath(Environment.SpecialFolder.Personal) + "\\" +
                                          ApplicationName + "\\Configs\\" + ModuleName + "\\";

        protected abstract string SaveName { get; }
        protected abstract string[] OldNames { get; }
    }
}