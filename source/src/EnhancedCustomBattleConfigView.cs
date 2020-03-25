﻿using TaleWorlds.Core;
using TaleWorlds.Engine.GauntletUI;
using TaleWorlds.MountAndBlade.View.Missions;

namespace EnhancedBattleTest
{
    public class EnhancedCustomBattleConfigView : MissionView
    {
        private GauntletLayer _gauntletLayer;
        private EnhancedCustomBattleConfigVM _dataSource;
        private CharacterSelectionView _selectionView;

        public EnhancedCustomBattleConfigView(CharacterSelectionView selectionView)
        {
            this._selectionView = selectionView;
            this.ViewOrderPriorty = 22;
        }

        public override void OnMissionScreenInitialize()
        {
            base.OnMissionScreenInitialize();
            Open();
        }
        public override void OnMissionScreenFinalize()
        {
            Close();
            base.OnMissionScreenFinalize();
        }
        public override bool OnEscape()
        {
            base.OnEscape();
            this._dataSource.GoBack();
            return true;
        }

        public void Open()
        {
            this._dataSource = new EnhancedCustomBattleConfigVM(_selectionView, Mission.GetMissionBehaviour<MissionMenuView>(), config =>
            {
                this.Mission.EndMission();
                GameStateManager.Current.PopStateRPC(0);
                EnhancedBattleTestMissions.OpenCustomBattleMission(config);
            }, (config) =>
            {
                TopState.status = TopStateStatus.exit;
                this.Mission.EndMission();
            });

            this._gauntletLayer = new GauntletLayer(this.ViewOrderPriorty, "GauntletLayer");
            this._gauntletLayer.LoadMovie(nameof(EnhancedCustomBattleConfigView), this._dataSource);
            this._gauntletLayer.InputRestrictions.SetInputRestrictions(true, TaleWorlds.Library.InputUsageMask.All);
            this.MissionScreen.AddLayer(this._gauntletLayer);
        }

        public void Close()
        {
            if (this._gauntletLayer != null)
            {
                this._gauntletLayer.InputRestrictions.ResetInputRestrictions();
                this.MissionScreen.RemoveLayer(_gauntletLayer);
                this._gauntletLayer = null;
            }
            if (this._dataSource != null)
            {
                this._dataSource.OnFinalize();
                this._dataSource = null;
            }
        }
    }
}