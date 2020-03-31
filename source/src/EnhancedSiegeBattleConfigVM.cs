﻿using System;
using System.Collections.Generic;
using System.Text;
using TaleWorlds.Core;
using TaleWorlds.Library;

namespace EnhancedBattleTest
{
    class EnhancedSiegeBattleConfigVM : BattleConfigVMBase<EnhancedSiegeBattleConfig>
    {
        private Action<EnhancedSiegeBattleConfig> startAction;
        private Action<EnhancedSiegeBattleConfig> backAction;

        private int _selectedSceneIndex;
        private string _distance;
        private string _soldierXInterval, _soldierYInterval;
        private string _soldiersPerRow;
        private string _formationPosition;
        private string _formationDirection;
        private string _skyBrightness;
        private string _rainDensity;

        private string _selectedMapName;

        public string BattleModeString { get; } = "EnhancedBattleTest e" + BattleConfigBase.ModVersion.ToString(2) +
                                                  ": " + GameTexts.FindText("str_siege_battle").ToString();
        [DataSourceProperty]
        public string SelectedMapName
        {
            get => this._selectedMapName;
            set
            {
                this._selectedMapName = value;
                this.OnPropertyChanged(nameof(SelectedMapName));
            }
        }

        public int SelectedSceneIndex
        {
            get => this._selectedSceneIndex;
            set
            {
                if (value < 0 || value >= CurrentConfig.sceneList.Length || value == this._selectedSceneIndex)
                    return;

                this.CurrentConfig.sceneIndex = value;
                this._selectedSceneIndex = value;
                UpdateSceneContent();
            }
        }

        [DataSourceProperty]
        public string Distance
        {
            get => this._distance;
            set
            {
                if (value == this._distance)
                    return;
                this._distance = value;
                this.OnPropertyChanged(nameof(Distance));
            }
        }

        [DataSourceProperty]
        public string SoldierXInterval
        {
            get => this._soldierXInterval;
            set
            {
                if (value == this._soldierXInterval)
                    return;
                this._soldierXInterval = value;
                this.OnPropertyChanged(nameof(SoldierXInterval));
            }
        }

        [DataSourceProperty]
        public string SoldierYInterval
        {
            get => this._soldierYInterval;
            set
            {
                if (value == this._soldierYInterval)
                    return;
                this._soldierYInterval = value;
                this.OnPropertyChanged(nameof(SoldierYInterval));
            }
        }

        [DataSourceProperty]
        public string SoldiersPerRow
        {
            get => this._soldiersPerRow;
            set
            {
                if (value == this._soldiersPerRow)
                    return;
                this._soldiersPerRow = value;
                this.OnPropertyChanged(nameof(SoldiersPerRow));
            }
        }

        [DataSourceProperty]
        public string SkyBrightness
        {
            get => this._skyBrightness;
            set
            {
                if (this._skyBrightness == value)
                    return;
                this._skyBrightness = value;
                this.OnPropertyChanged(nameof(SkyBrightness));
            }
        }

        [DataSourceProperty]
        public string RainDensity
        {
            get => _rainDensity;
            set
            {
                if (this._rainDensity == value)
                    return;
                this._rainDensity = value;
                this.OnPropertyChanged(nameof(RainDensity));
            }
        }

        [DataSourceProperty]
        public string FormationPosition
        {
            get => this._formationPosition;
            set
            {
                if (this._formationPosition == value)
                    return;
                this._formationPosition = value;
                this.OnPropertyChanged(nameof(FormationPosition));
            }
        }

        public string FormationDirection
        {
            get => this._formationDirection;
            set
            {
                if (this._formationDirection == value)
                    return;
                this._formationDirection = value;
                this.OnPropertyChanged(nameof(FormationDirection));
            }
        }

        [DataSourceProperty]
        public bool HasBoundary
        {
            get => this.CurrentConfig.hasBoundary;
            set
            {
                if (this.CurrentConfig.hasBoundary == value)
                    return;
                this.CurrentConfig.hasBoundary = value;
                this.OnPropertyChanged(nameof(HasBoundary));
            }
        }

        public EnhancedSiegeBattleConfigVM(CharacterSelectionView selectionView, MissionMenuView missionMenuView,Action<EnhancedSiegeBattleConfig> startAction,
            Action<EnhancedSiegeBattleConfig> backAction)
            : base(selectionView, missionMenuView, EnhancedSiegeBattleConfig.Get())
        {
            InitializeContent();

            this.startAction = startAction;
            this.backAction = backAction;
        }

        private void PreviousMap()
        {
            if (this.SelectedSceneIndex == 0)
                return;
            this.SelectedSceneIndex--;
        }
        private void NextMap()
        {
            if (this.SelectedSceneIndex + 1 >= CurrentConfig.sceneList.Length)
                return;
            this.SelectedSceneIndex++;
        }

        private new void SelectPlayerCharacter()
        {
            base.SelectPlayerCharacter();
        }

        private new void SelectEnemyCharacter()
        {
            base.SelectEnemyCharacter();
        }

        private new void SelectPlayerTroopCharacter1()
        {
            base.SelectPlayerTroopCharacter1();
        }

        private new void SelectEnemyTroopCharacter1()
        {
            base.SelectEnemyTroopCharacter1();
        }

        protected new void OpenMissionMenu()
        {
            base.OpenMissionMenu();
        }

        private void Start()
        {
            if (SaveConfig() != SaveParamResult.success)
                return;
            this.startAction(CurrentConfig);
        }

        private void Save()
        {
            SaveConfig();
        }

        private void LoadConfig()
        {
            this.CurrentConfig.ReloadSavedConfig();
            this.InitializeContent();
        }

        public void GoBack()
        {
            backAction(this.CurrentConfig);
        }

        private void InitializeContent()
        {
            this._selectedSceneIndex = CurrentConfig.sceneIndex;
            UpdateSceneContent();

            this.SoldierXInterval = CurrentConfig.soldierXInterval.ToString();
            this.SoldierYInterval = CurrentConfig.soldierYInterval.ToString();
        }

        private void UpdateSceneContent()
        {
            this.SelectedMapName = CurrentConfig.SceneName;
            this.SoldiersPerRow = CurrentConfig.SoldiersPerRow.ToString();
            this.FormationPosition = Vec2ToString(CurrentConfig.FormationPosition);
            this.FormationDirection = Vec2ToString(CurrentConfig.FormationDirection);
            this.Distance = CurrentConfig.Distance.ToString();
            this.SkyBrightness = CurrentConfig.SkyBrightness.ToString();
            this.RainDensity = CurrentConfig.RainDensity.ToString();
        }

        protected override void ApplyConfig()
        {
            base.ApplyConfig();
            CurrentConfig.sceneIndex = this.SelectedSceneIndex;
            CurrentConfig.SoldiersPerRow = System.Convert.ToInt32(this.SoldiersPerRow);
            CurrentConfig.FormationPosition = StringToVec2(this.FormationPosition);
            CurrentConfig.FormationDirection = StringToVec2(this.FormationDirection).Normalized();
            CurrentConfig.SkyBrightness = System.Convert.ToSingle(this.SkyBrightness);
            CurrentConfig.RainDensity = System.Convert.ToSingle(this.RainDensity);

            CurrentConfig.Distance = System.Convert.ToSingle(this.Distance);
            CurrentConfig.soldierXInterval = System.Convert.ToSingle(this.SoldierXInterval);
            CurrentConfig.soldierYInterval = System.Convert.ToSingle(this.SoldierYInterval);
        }


        private string Vec2ToString(Vec2 vec2)
        {
            return $"{vec2.x},{vec2.y}";
        }

        private Vec2 StringToVec2(string str)
        {
            var posParts = str.Split(',');
            return new Vec2(System.Convert.ToSingle(posParts[0]), System.Convert.ToSingle(posParts[1]));
        }
    }
}
