﻿using System;
using System.Collections.Generic;
using System.Linq;
using TaleWorlds.CampaignSystem.SandBox.Issues;
using TaleWorlds.Core;
using TaleWorlds.Core.ViewModelCollection;
using TaleWorlds.Library;
using TaleWorlds.Localization;
using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.CustomBattle.CustomBattle;
using TaleWorlds.MountAndBlade.CustomBattle.CustomBattle.SelectionItem;

namespace EnhancedBattleTest
{
    public class MapSelectionGroupVM : ViewModel
    {
        private bool _isCurrentMapSiege;
        private bool _isSallyOutSelected;
        private string _searchText;
        private SelectorVM<SceneLevelItemVM> _sceneLevelSelection;
        private SelectorVM<WallHitpointItemVM> _wallHitpointSelection;
        private SelectorVM<SeasonItemVM> _seasonSelection;
        private MBBindingList<MapItemVM> _mapSearchResults;
        private string _titleText;
        private string _seasonText;
        private string _sceneLevelText;
        private string _wallHitpointsText;
        private string _attackerSiegeMachinesText;
        private string _defenderSiegeMachinesText;
        private string _sallyoutText;
        private readonly List<SceneData> _scenes;

        public int SelectedWallHitpoint { get; private set; }

        public int SelectedSceneLevel { get; private set; }

        public string SelectedSeasonId { get; private set; }

        public string SelectedMapId
        {
            get
            {
                MapItemVM mapItemVm = _availableMaps.Find(m => m.MapName.ToLower() == SearchText.ToLower());
                return mapItemVm != null ? mapItemVm.MapId : SelectedMap.MapId;
            }
        }

        public MapItemVM SelectedMap { get; private set; }

        private readonly List<MapItemVM> _battleMaps;

        private readonly List<MapItemVM> _villageMaps;

        private readonly List<MapItemVM> _siegeMaps;

        private List<MapItemVM> _availableMaps;

        public MapSelectionGroupVM(List<SceneData> scenes)
        {
            _scenes = scenes;
            MapSearchResults = new MBBindingList<MapItemVM>();
            _battleMaps = new List<MapItemVM>();
            _villageMaps = new List<MapItemVM>();
            _siegeMaps = new List<MapItemVM>();
            WallHitpointSelection = new SelectorVM<WallHitpointItemVM>(0, OnWallHitpointSelection);
            SceneLevelSelection = new SelectorVM<SceneLevelItemVM>(0, OnSceneLevelSelection);
            SeasonSelection = new SelectorVM<SeasonItemVM>(0, OnSeasonSelection);
            RefreshValues();
        }

        public override void RefreshValues()
        {
            base.RefreshValues();
            PrepareMapLists();
            TitleText = new TextObject("{=*}Map").ToString();
            SeasonText = new TextObject("{=*}Season").ToString();
            SceneLevelText = new TextObject("{=*}Scene Level").ToString();
            WallHitpointsText = new TextObject("{=*}Wall Hitpoints").ToString();
            AttackerSiegeMachinesText = new TextObject("{=*}Attacker Siege Machines").ToString();
            DefenderSiegeMachinesText = new TextObject("{=*}Defender Siege Machines").ToString();
            SalloutText = new TextObject("{=*}Sallyout").ToString();
            WallHitpointSelection.ItemList.Clear();
            SceneLevelSelection.ItemList.Clear();
            SeasonSelection.ItemList.Clear();
            foreach (int wallHitpoint in CustomBattleData.WallHitpoints)
                WallHitpointSelection.AddItem(new WallHitpointItemVM(wallHitpoint));
            foreach (int sceneLevel in CustomBattleData.SceneLevels)
                SceneLevelSelection.AddItem(new SceneLevelItemVM(sceneLevel));
            foreach (Tuple<string, string> season in CustomBattleData.Seasons)
                SeasonSelection.AddItem(new SeasonItemVM(season.Item1, season.Item2));
            WallHitpointSelection.SelectedIndex = 0;
            SceneLevelSelection.SelectedIndex = 0;
            SeasonSelection.SelectedIndex = 0;
        }

        public void ExecuteSallyOutChange()
        {
            IsSallyOutSelected = !IsSallyOutSelected;
        }

        private void PrepareMapLists()
        {
            _battleMaps.Clear();
            _villageMaps.Clear();
            _siegeMaps.Clear();
            foreach (var sceneData in _scenes)
            {
                MapItemVM mapItemVm = new MapItemVM(sceneData.Name.ToString(), sceneData.Id, OnMapSelection);
                if (sceneData.IsVillageMap)
                    _villageMaps.Add(mapItemVm);
                else if (sceneData.IsSiegeMap)
                    _siegeMaps.Add(mapItemVm);
                else
                    _battleMaps.Add(mapItemVm);
            }
            Comparer<MapItemVM> comparer = Comparer<MapItemVM>.Create((x, y) => -x.MapName.CompareTo(y.MapName));
            _battleMaps.Sort(comparer);
            _villageMaps.Sort(comparer);
            _siegeMaps.Sort(comparer);
        }

        private void OnMapSelection(MapItemVM item)
        {
            SelectedMap = item;
            SearchText = item.MapName;
        }

        private void OnWallHitpointSelection(SelectorVM<WallHitpointItemVM> selector)
        {
            SelectedWallHitpoint = selector.SelectedItem.Hitpoint;
        }

        private void OnSceneLevelSelection(SelectorVM<SceneLevelItemVM> selector)
        {
            SelectedSceneLevel = selector.SelectedItem.Level;
        }

        private void OnSeasonSelection(SelectorVM<SeasonItemVM> selector)
        {
            SelectedSeasonId = selector.SelectedItem.SeasonId;
        }

        public void OnGameTypeChange(CustomBattleGameType gameType)
        {
            MapSearchResults.Clear();
            switch (gameType)
            {
                case CustomBattleGameType.Battle:
                    IsCurrentMapSiege = false;
                    _availableMaps = _battleMaps;
                    break;
                case CustomBattleGameType.Village:
                    IsCurrentMapSiege = false;
                    _availableMaps = _villageMaps;
                    break;
                case CustomBattleGameType.Siege:
                    IsCurrentMapSiege = true;
                    _availableMaps = _siegeMaps;
                    break;
            }
            foreach (MapItemVM availableMap in _availableMaps)
                MapSearchResults.Add(availableMap);
            _searchText = new TextObject("{=*}Select a Map").ToString();
            OnPropertyChanged(nameof(SearchText));
        }

        public void RandomizeAll()
        {
            MBBindingList<MapItemVM> mapSearchResults = MapSearchResults;
            // ISSUE: explicit non-virtual call
            if (mapSearchResults != null && mapSearchResults.Count > 0)
            {
                SearchText = "";
                MapSearchResults[MBRandom.RandomInt(MapSearchResults.Count)].ExecuteSelection();
            }
            SceneLevelSelection.ExecuteRandomize();
            SeasonSelection.ExecuteRandomize();
            WallHitpointSelection.ExecuteRandomize();
        }

        public void RandomizeMap()
        {
            MBBindingList<MapItemVM> mapSearchResults = MapSearchResults;
            // ISSUE: explicit non-virtual call
            if (mapSearchResults != null && mapSearchResults.Count > 0)
            {
                MapSearchResults[MBRandom.RandomInt(MapSearchResults.Count)].ExecuteSelection();
            }
        }

        private void RefreshSearch(bool isAppending)
        {
            if (isAppending)
            {
                foreach (MapItemVM mapItemVm in MapSearchResults.ToList())
                {
                    if (mapItemVm.MapName.IndexOf(_searchText, StringComparison.OrdinalIgnoreCase) < 0)
                        MapSearchResults.Remove(mapItemVm);
                    else
                        mapItemVm.UpdateSearchedText(_searchText);
                }
            }
            else
            {
                MapSearchResults.Clear();
                foreach (MapItemVM availableMap in _availableMaps)
                {
                    MapItemVM map = availableMap;
                    if (map.MapName.IndexOf(_searchText, StringComparison.OrdinalIgnoreCase) >= 0 && MapSearchResults.All(m => m.MapName != map.MapName))
                        MapSearchResults.Add(map);
                }
            }
        }

        [DataSourceProperty]
        public MBBindingList<MapItemVM> MapSearchResults
        {
            get => _mapSearchResults;
            set
            {
                if (value == _mapSearchResults)
                    return;
                _mapSearchResults = value;
                OnPropertyChanged(nameof(MapSearchResults));
            }
        }

        [DataSourceProperty]
        public SelectorVM<SceneLevelItemVM> SceneLevelSelection
        {
            get => _sceneLevelSelection;
            set
            {
                if (value == _sceneLevelSelection)
                    return;
                _sceneLevelSelection = value;
                OnPropertyChanged(nameof(SceneLevelSelection));
            }
        }

        [DataSourceProperty]
        public SelectorVM<WallHitpointItemVM> WallHitpointSelection
        {
            get => _wallHitpointSelection;
            set
            {
                if (value == _wallHitpointSelection)
                    return;
                _wallHitpointSelection = value;
                OnPropertyChanged(nameof(WallHitpointSelection));
            }
        }

        [DataSourceProperty]
        public SelectorVM<SeasonItemVM> SeasonSelection
        {
            get => _seasonSelection;
            set
            {
                if (value == _seasonSelection)
                    return;
                _seasonSelection = value;
                OnPropertyChanged(nameof(SeasonSelection));
            }
        }

        [DataSourceProperty]
        public bool IsCurrentMapSiege
        {
            get => _isCurrentMapSiege;
            set
            {
                if (value == _isCurrentMapSiege)
                    return;
                _isCurrentMapSiege = value;
                OnPropertyChanged(nameof(IsCurrentMapSiege));
            }
        }

        [DataSourceProperty]
        public bool IsSallyOutSelected
        {
            get => _isSallyOutSelected;
            set
            {
                if (value == _isSallyOutSelected)
                    return;
                _isSallyOutSelected = value;
                OnPropertyChanged(nameof(IsSallyOutSelected));
            }
        }

        [DataSourceProperty]
        public string SearchText
        {
            get => _searchText;
            set
            {
                if (value == _searchText)
                    return;
                bool isAppending = true;
                if (!string.IsNullOrEmpty(_searchText))
                    isAppending = value.ToLower().Contains(_searchText.ToLower());
                _searchText = value;
                RefreshSearch(isAppending);
                OnPropertyChanged(nameof(SearchText));
            }
        }

        [DataSourceProperty]
        public string TitleText
        {
            get => _titleText;
            set
            {
                if (value == _titleText)
                    return;
                _titleText = value;
                OnPropertyChanged(nameof(TitleText));
            }
        }

        [DataSourceProperty]
        public string SeasonText
        {
            get => _seasonText;
            set
            {
                if (value == _seasonText)
                    return;
                _seasonText = value;
                OnPropertyChanged(nameof(SeasonText));
            }
        }

        [DataSourceProperty]
        public string SceneLevelText
        {
            get => _sceneLevelText;
            set
            {
                if (value == _sceneLevelText)
                    return;
                _sceneLevelText = value;
                OnPropertyChanged(nameof(SceneLevelText));
            }
        }

        [DataSourceProperty]
        public string WallHitpointsText
        {
            get => _wallHitpointsText;
            set
            {
                if (value == _wallHitpointsText)
                    return;
                _wallHitpointsText = value;
                OnPropertyChanged(nameof(WallHitpointsText));
            }
        }

        [DataSourceProperty]
        public string AttackerSiegeMachinesText
        {
            get => _attackerSiegeMachinesText;
            set
            {
                if (value == _attackerSiegeMachinesText)
                    return;
                _attackerSiegeMachinesText = value;
                OnPropertyChanged(nameof(AttackerSiegeMachinesText));
            }
        }

        [DataSourceProperty]
        public string DefenderSiegeMachinesText
        {
            get => _defenderSiegeMachinesText;
            set
            {
                if (value == _defenderSiegeMachinesText)
                    return;
                _defenderSiegeMachinesText = value;
                OnPropertyChanged(nameof(DefenderSiegeMachinesText));
            }
        }

        [DataSourceProperty]
        public string SalloutText
        {
            get => _sallyoutText;
            set
            {
                if (value == _sallyoutText)
                    return;
                _sallyoutText = value;
                OnPropertyChanged(nameof(SalloutText));
            }
        }
    }
}
