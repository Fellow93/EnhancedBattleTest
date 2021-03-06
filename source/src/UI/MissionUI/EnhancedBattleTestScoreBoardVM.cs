﻿using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.Source.Missions.Handlers;
using TaleWorlds.MountAndBlade.ViewModelCollection;

namespace EnhancedBattleTest.UI.MissionUI
{
    public class EnhancedBattleTestScoreBoardVM : CustomBattleScoreboardVM
    {
        public override void ExecuteQuitAction()
        {
            BattleEndLogic battleEndLogic = _mission.GetMissionBehaviour<BattleEndLogic>();
            BasicMissionHandler basicMissionHandler = _mission.GetMissionBehaviour<BasicMissionHandler>();
            BattleEndLogic.ExitResult exitResult = battleEndLogic?.TryExit() ?? (_mission.MissionEnded() ? BattleEndLogic.ExitResult.True : BattleEndLogic.ExitResult.NeedsPlayerConfirmation);
            switch (exitResult)
            {
                case BattleEndLogic.ExitResult.False:
                    goto case BattleEndLogic.ExitResult.NeedsPlayerConfirmation;
                case BattleEndLogic.ExitResult.NeedsPlayerConfirmation:
                    OnToggle(false);
                    basicMissionHandler.CreateWarningWidget();
                    break;
                default:
                    if (battleEndLogic != null || exitResult != BattleEndLogic.ExitResult.True)
                        break;
                    _mission.EndMission();
                    break;
            }
        }
    }
}
