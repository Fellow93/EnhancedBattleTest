using System.Collections.Generic;
using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.LegacyGUI.Missions;
using TaleWorlds.MountAndBlade.View.Missions;

namespace EnhancedBattleTest
{
    [ViewCreatorModule]
    public class EnhancedTestBattleViews
    {
        [ViewMethod("EnhancedTestBattleConfig")]
        public static MissionView[] OpenInitialMission(Mission mission)
        {
            var selectionView = new CharacterSelectionView(true);
            return new MissionView[]
            {
                selectionView,
                new EnhancedTestBattleConfigView(selectionView),
                new MissionMenuView(EnhancedTestBattleConfig.Get()),
            };
        }

        [ViewMethod("EnhancedTestBattle")]
        public static MissionView[] OpenTestMission(Mission mission)
        {
            var config = EnhancedTestBattleConfig.Get();
            var missionViewList = new List<MissionView>
            {
                new MissionMenuView(config),
                new MissionTestBattlePreloadView(config),
                new PauseView(),
                ViewCreator.CreateMissionAgentStatusUIHandler(mission),
                ViewCreator.CreateMissionMainAgentEquipmentController(mission),
                ViewCreator.CreateMissionLeaveView(),
                ViewCreator.CreateMissionSingleplayerEscapeMenu(),
                ViewCreator.CreateMissionOrderUIHandler(mission),
                ViewCreator.CreateOrderTroopPlacerView(mission),
                ViewCreator.CreateSingleplayerMissionKillNotificationUIHandler(),
                new MissionItemContourControllerView(),
                new MissionAgentContourControllerView(),
                ViewCreator.CreateOptionsUIHandler(),
                new SpectatorCameraView(),
                new InitializeCameraPosView(config.FormationPosition, config.FormationDirection),
            };
            if (config.hasBoundary)
            {
                missionViewList.Add(ViewCreator.CreateMissionBoundaryCrossingView());
                missionViewList.Add(new MissionBoundaryWallView());
            }

            if (!config.noAgentLabel)
            {
                missionViewList.Add(ViewCreator.CreateMissionAgentLabelUIHandler(mission));
            }
            return missionViewList.ToArray();
        }
    }
}