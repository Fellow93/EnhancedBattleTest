using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using TaleWorlds.Core;
using TaleWorlds.Engine;
using TaleWorlds.MountAndBlade;
using TL = TaleWorlds.Library;

namespace EnhancedBattleTest
{
    public class EnhancedTestBattleMissionController : MissionLogic
    {
        private EnhancedTestBattleConfig _testBattleConfig;
        private SoundEvent _musicSoundEvent;
        public EnhancedTestBattleConfig TestBattleConfig
        {
            get => _testBattleConfig;
            set => _testBattleConfig = value;
        }

        //private bool _ended = false;
        public TL.Vec3 initialFreeCameraPos;
        public TL.Vec3 initialFreeCameraTarget;
        private AgentVictoryLogic _victoryLogic;
        private MakeGruntVoiceLogic _makeGruntVoiceLogic;

        public EnhancedTestBattleMissionController(EnhancedTestBattleConfig config)
        {
            this.TestBattleConfig = config;
            _victoryLogic = null;
        }

        public override void EarlyStart()
        {
            this.Mission.MissionTeamAIType = Mission.MissionTeamAITypeEnum.FieldBattle;
            this.Mission.SetMissionMode(MissionMode.Battle, true);
            _makeGruntVoiceLogic = this.Mission.GetMissionBehaviour<MakeGruntVoiceLogic>();
            AdjustScene();
            var playerTeamCulture = this.TestBattleConfig.GetPlayerTeamCulture();
            var enemyTeamCulture = this.TestBattleConfig.GetEnemyTeamCulture();
            AddTeams(playerTeamCulture, enemyTeamCulture);
        }

        public override void AfterStart()
        {
            SpawnAgents();
        }

        public void SpawnAgents()
        {
            
            var playerTeamCulture = this.TestBattleConfig.GetPlayerTeamCulture();
            var enemyTeamCulture = this.TestBattleConfig.GetEnemyTeamCulture();
            SpawnPlayerTeamAgents(playerTeamCulture);
            SpawnEnemyTeamAgents(enemyTeamCulture);

            _musicSoundEvent = SoundEvent.CreateEvent(SoundEvent.GetEventIdFromString("event:/mission/ambient/area/multiplayer/city_mp_02"), Mission.Scene);
            _musicSoundEvent.Play();
        }

        private void AdjustScene()
        {
            var scene = this.Mission.Scene;
            if (this.TestBattleConfig.SkyBrightness >= 0)
            {
                scene.SetSkyBrightness(this.TestBattleConfig.SkyBrightness);
            }

            if (this.TestBattleConfig.RainDensity >= 0)
            {
                scene.SetRainDensity(this.TestBattleConfig.RainDensity);
            }
        }

        private void AddTeams(BasicCultureObject playerTeamCulture, BasicCultureObject enemyTeamCulture)
        {
            var playerTeamBackgroundColor =
                Utility.BackgroundColor(playerTeamCulture, TestBattleConfig.isPlayerAttacker);
            var playerTeamForegroundColor =
                Utility.ForegroundColor(playerTeamCulture, TestBattleConfig.isPlayerAttacker);
            // see TaleWorlds.MountAndBlade.dll/MissionMultiplayerFlagDomination.cs : TaleWorlds.MountAndBlade.MissionMultiplayerFlagDomination.AfterStart();
            Banner playerTeamBanner = new Banner(playerTeamCulture.BannerKey, playerTeamBackgroundColor,
                playerTeamForegroundColor);
            var playerSide = TestBattleConfig.isPlayerAttacker ? BattleSideEnum.Attacker : BattleSideEnum.Defender;
            var playerTeam = this.Mission.Teams.Add(playerSide, color: playerTeamBackgroundColor, color2: playerTeamForegroundColor, banner: playerTeamBanner);

            var enemyTeamBackgroundColor =
                Utility.BackgroundColor(enemyTeamCulture, !TestBattleConfig.isPlayerAttacker);
            var enemyTeamForegroundColor =
                Utility.ForegroundColor(enemyTeamCulture, !TestBattleConfig.isPlayerAttacker);
            Banner enemyTeamBanner = new Banner(enemyTeamCulture.BannerKey,
                enemyTeamBackgroundColor, enemyTeamForegroundColor);
            var enemySide = playerSide.GetOppositeSide();
            var enemyTeam = this.Mission.Teams.Add(enemySide, color: enemyTeamBackgroundColor, color2: enemyTeamForegroundColor, banner: enemyTeamBanner);

            playerTeam.AddTeamAI(new TeamAIGeneral(this.Mission, playerTeam));
            enemyTeam.AddTeamAI(new TeamAIGeneral(this.Mission, enemyTeam));
            using (IEnumerator<Team> enumerator = Mission.Teams.Where<Team>((Func<Team, bool>)(t => t.HasTeamAi)).GetEnumerator())
            {
                while (enumerator.MoveNext())
                {
                    Team team = enumerator.Current;
                    if (team.Side == BattleSideEnum.Defender)
                    {
                        bool flag = false;
                        foreach (var defenderTacticOption in _testBattleConfig.defenderTacticOptions)
                        {
                            if (defenderTacticOption.isEnabled)
                            {
                                flag = true;
                                TacticOptionHelper.AddTacticComponent(team, defenderTacticOption.tacticOption);
                            }
                        }
                        if (!flag)
                            team.AddTacticOption(new TacticCharge(team));
                    }

                    if (team.Side == BattleSideEnum.Attacker)
                    {
                        bool flag = false;
                        foreach (var attackerTacticOption in _testBattleConfig.attackerTacticOptions)
                        {
                            if (attackerTacticOption.isEnabled)
                            {
                                flag = true;
                                TacticOptionHelper.AddTacticComponent(team, attackerTacticOption.tacticOption);
                            }
                        }
                        if (!flag)
                            team.AddTacticOption(new TacticCharge(team));
                    }
                }
            }

            foreach (Team team in (ReadOnlyCollection<Team>)this.Mission.Teams)
            {
                team.ExpireAIQuerySystem();
                team.ResetTactic();
                team.OnOrderIssued += OrderIssuedDelegate;
            }

            this.Mission.PlayerTeam = playerTeam;
        }

        private void SpawnPlayerTeamAgents(BasicCultureObject playerTeamCulture)
        {
            var scene = this.Mission.Scene;
            // see TaleWorlds.MountAndBlade.dll/FlagDominationSpawningBehaviour.cs: TaleWorlds.MountAndBlade.FlagDominationSpawningBehaviour.SpawnEnemyTeamAgents()
            // see TaleWorlds.MountAndBlade.dll/SpawningBehaviourBase.cs: TaleWorlds.MountAndBlade.SpawningBehaviourBase.OnTick()
            var playerTeamCombatant = CreateBattleCombatant(playerTeamCulture, BattleSideEnum.Attacker);

            var xInterval = this.TestBattleConfig.soldierXInterval;
            var yInterval = this.TestBattleConfig.soldierYInterval;
            var soldiersPerRow = this.TestBattleConfig.SoldiersPerRow;
            var startPos = this.TestBattleConfig.FormationPosition;
            var xDir = this.TestBattleConfig.FormationDirection;
            var yDir = this.TestBattleConfig.FormationDirection.LeftVec();
            var spawnPlayer = this.TestBattleConfig.SpawnPlayer;

            var playerTeam = this.Mission.PlayerTeam;

            if (spawnPlayer)
            {
                MultiplayerClassDivisions.MPHeroClass playerHeroClass = this.TestBattleConfig.PlayerHeroClass;
                BasicCharacterObject playerCharacter = playerHeroClass.HeroCharacter;
                Game.Current.PlayerTroop = playerCharacter;
                var playerFormation = playerTeam.GetFormation(Utility.CommanderFormationClass());
                SetFormationRegion(playerFormation, 1, true, -2);
                Agent player = this.SpawnAgent(TestBattleConfig.playerClass, true,
                    playerCharacter, true, playerFormation, playerTeam,
                    playerTeamCombatant, playerTeamCulture, true, 1, 0);

                player.AllowFirstPersonWideRotation();
            }
            else
            {
                var c = this.TestBattleConfig.playerTroops[0].troopCount;
                if (c <= 0)
                {
                    initialFreeCameraTarget = startPos.ToVec3(Utility.GetSceneHeightForAgent(Mission.Scene, startPos));
                    initialFreeCameraPos = initialFreeCameraTarget + new TL.Vec3(0, 0, 10) - xDir.ToVec3();
                }
                else
                {
                    var rowCount = (c + soldiersPerRow - 1) / soldiersPerRow;
                    var p = startPos + (System.Math.Min(soldiersPerRow, c) - 1) / 2 * yInterval * yDir - rowCount * xInterval * xDir;
                    initialFreeCameraTarget = p.ToVec3(Utility.GetSceneHeightForAgent(Mission.Scene, p));
                    initialFreeCameraPos = initialFreeCameraTarget + new TL.Vec3(0, 0, 10) - xDir.ToVec3();
                }

                initialFreeCameraPos -= this._testBattleConfig.FormationDirection.ToVec3();
            }

            float distanceToInitialPosition = 0;

            for (var formationIndex = 0; formationIndex < 3; ++formationIndex)
            {
                int playerTroopCount = TestBattleConfig.playerTroops[formationIndex].troopCount;
                if (playerTroopCount <= 0)
                    continue;
                BasicCharacterObject playerTroopCharacter = this.TestBattleConfig.GetPlayerTroopHeroClass(formationIndex).TroopCharacter;
                var playerTroopFormation = playerTeam.GetFormation((FormationClass)formationIndex);
                var (width, length) =
                    GetInitialFormationArea(formationIndex, true, playerTroopCharacter.CurrentFormationClass);
                SetFormationRegion(playerTroopFormation, width, true, distanceToInitialPosition);
                distanceToInitialPosition += length;
                BasicCultureObject troopCulture = spawnPlayer ? playerTeamCulture : playerTroopCharacter.Culture;
                for (var troopIndex = 0; troopIndex < playerTroopCount; ++troopIndex)
                {
                    var agent = this.SpawnAgent(TestBattleConfig.playerTroops[formationIndex], false, playerTroopCharacter, false,
                        playerTroopFormation, playerTeam, playerTeamCombatant, troopCulture, true, playerTroopCount,
                        troopIndex);
                }
            }

            //var matrixFrame = Utility.ToMatrixFrame(Mission.Scene, GetFormationPosition(true, -2), TL.Vec2.Forward);

            //MissionWeapon weapon = new MissionWeapon(MBObjectManager.Instance.GetObject<ItemObject>("mp_hatchet_axe"), (Banner)null);
            //Mission.Current.SpawnWeaponWithNewEntity(ref weapon, Mission.WeaponSpawnFlags.WithPhysics, matrixFrame);

            //var hat = MBObjectManager.Instance.GetObject<ItemObject>("mp_northern_padded_cloth");
            //var gameEntity = GameEntity.CreateEmpty(Mission.Scene);
            //gameEntity.AddMesh(Mesh.GetFromResource(hat.MultiMeshName));
            //gameEntity.SetGlobalFrame(matrixFrame);
            //Mission.Scene.AttachEntity(gameEntity);

            //var horse = MBObjectManager.Instance.GetObject<ItemObject>("mp_sturgia_horse");

            //Mission.Scene.AddEntityWithMesh(Mesh.GetFromResource(horse.MultiMeshName), ref matrixFrame);

            //var house = GameEntity.Instantiate(Mission.Scene, "hawk_stand_b", matrixFrame);
            //Mission.Scene.AttachEntity(house);
        }

        private void SpawnEnemyTeamAgents(BasicCultureObject enemyTeamCulture)
        {
            var scene = this.Mission.Scene;

            var enemyTeamCombatant = CreateBattleCombatant(enemyTeamCulture, BattleSideEnum.Defender);

            var enemyTeam = this.Mission.PlayerEnemyTeam;

            var xDir = this.TestBattleConfig.FormationDirection;

            if (TestBattleConfig.SpawnEnemyCommander)
            {
                MultiplayerClassDivisions.MPHeroClass enemyHeroClass = this.TestBattleConfig.EnemyHeroClass;
                BasicCharacterObject enemyCharacter = enemyHeroClass.HeroCharacter;
                var formation = enemyTeam.GetFormation(Utility.CommanderFormationClass());
                SetFormationRegion(formation, 1, false, -2);
                Agent agent = this.SpawnAgent(TestBattleConfig.enemyClass, false,
                    enemyCharacter, true, formation, enemyTeam,
                    enemyTeamCombatant, enemyTeamCulture, false, 1, 0);
            }

            float distanceToInitialPosition = 0;
            for (var formationIndex = 0; formationIndex < 3; ++formationIndex)
            {
                int enemyTroopCount = this.TestBattleConfig.enemyTroops[formationIndex].troopCount;
                if (enemyTroopCount <= 0)
                    continue;
                BasicCharacterObject enemyTroopCharacter = this.TestBattleConfig.GetEnemyTroopHeroClass(formationIndex).TroopCharacter;
                var enemyTroopFormation = enemyTeam.GetFormation((FormationClass)formationIndex);
                var (width, length) =
                    GetInitialFormationArea(formationIndex, false, enemyTroopCharacter.CurrentFormationClass);
                SetFormationRegion(enemyTroopFormation, width, false, distanceToInitialPosition);
                distanceToInitialPosition += length;
                var troopCulture = this.TestBattleConfig.SpawnEnemyCommander
                    ? enemyTeamCulture
                    : enemyTroopCharacter.Culture;
                for (var troopIndex = 0; troopIndex < enemyTroopCount; ++troopIndex)
                {
                    var agent = SpawnAgent(TestBattleConfig.enemyTroops[formationIndex], false, enemyTroopCharacter, false, enemyTroopFormation,
                        enemyTeam, enemyTeamCombatant, troopCulture, false, enemyTroopCount, troopIndex);
                }
            }
        }

        public override void OnMissionTick(float dt)
        {
            CheckVictory();
        }

        private void CheckVictory()
        {
            //if (!_ended && _shouldCelebrateVictory)
            //{
            //    bool playerVictory = this.Mission.PlayerEnemyTeam.ActiveAgents.IsEmpty();
            //    bool enemyVictory = this.Mission.PlayerTeam.ActiveAgents.IsEmpty();
            //    if (!playerVictory && !enemyVictory)
            //        return;
            //    _ended = true;
            //    _victoryLogic = this.Mission.GetMissionBehaviour<AgentVictoryLogic>();
            //    if (_victoryLogic == null)
            //        return;
            //    if (enemyVictory)
            //    {
            //        _ended = true;
            //        _victoryLogic.SetTimersOfVictoryReactions(this.Mission.PlayerEnemyTeam.Side);
            //    }
            //    else
            //    {
            //        _ended = true;
            //        _victoryLogic.SetTimersOfVictoryReactions(this.Mission.PlayerTeam.Side);
            //    }
            //}
        }

        void OrderIssuedDelegate(
            OrderType orderType,
            IEnumerable<Formation> appliedFormations,
            params object[] delegateParams)
        {
            foreach (var formation in appliedFormations)
            {
                if (_victoryLogic != null)
                {
                    foreach (var agent in formation.Units)
                    {
                        _victoryLogic.OnAgentDeleted(agent);
                        if (!agent.IsPlayerControlled)
                        {
                            //EquipmentIndex index = agent.GetWieldedItemIndex(Agent.HandIndex.MainHand);
                            agent.SetActionChannel(1, ActionIndexCache.act_none, true);
                            //agent.TryToSheathWeaponInHand(Agent.HandIndex.MainHand, Agent.WeaponWieldActionType.Instant);
                            //agent.TryToWieldWeaponInSlot(index, Agent.WeaponWieldActionType.WithAnimation, false);
                        }
                    }
                }

                _makeGruntVoiceLogic?.AddFormation(formation, 1f);
            }
        }

        private CustomBattleCombatant CreateBattleCombatant(BasicCultureObject culture, BattleSideEnum side)
        {
            return new CustomBattleCombatant(culture.Name, culture,
                new Banner(culture.BannerKey, culture.BackgroundColor1, culture.ForegroundColor1))
            {
                Side = side
            };
        }

        private Agent SpawnAgent(ClassInfo classInfo, bool isPlayer, BasicCharacterObject character, bool isHero, Formation formation, Team team, CustomBattleCombatant combatant, BasicCultureObject culture, bool isPlayerSide, int formationTroopCount, int formationTroopIndex, TL.MatrixFrame? matrix = null)
        {
            bool isAttacker = isPlayerSide ? TestBattleConfig.isPlayerAttacker : !TestBattleConfig.isPlayerAttacker;
            AgentBuildData agentBuildData = new AgentBuildData(Utility.CreateOrigin(combatant, Utility.ApplyPerks(classInfo, isHero)))
                .Team(team)
                .Formation(formation)
                .FormationTroopCount(formationTroopCount).FormationTroopIndex(formationTroopIndex)
                .Banner(team.Banner)
                .ClothingColor1(isAttacker ? culture.Color : culture.ClothAlternativeColor)
                .ClothingColor2(isAttacker ? culture.Color2 : culture.ClothAlternativeColor2);
            if (matrix.HasValue)
                agentBuildData.InitialFrame(matrix.Value);
            var agent = this.Mission.SpawnAgent(agentBuildData, false, 0);
            agent.SetWatchState(AgentAIStateFlagComponent.WatchState.Alarmed);
            agent.WieldInitialWeapons();
            agent.Controller = isPlayer ? Agent.ControllerType.Player : Agent.ControllerType.AI;
            return agent;
        }


        private void SetFormationRegion(Formation formation, float width, bool isPlayerSide, float distanceToInitialPosition)
        {
            bool isAttackerSide = isPlayerSide ? TestBattleConfig.isPlayerAttacker : !TestBattleConfig.isPlayerAttacker;
            var position = GetFormationPosition(isAttackerSide, distanceToInitialPosition);
            var direction = GetFormationDirection(isAttackerSide);
            formation.SetPositioning(position.ToWorldPosition(Mission.Scene), direction);
            formation.FormOrder = FormOrder.FormOrderCustom(width);
        }

        private Tuple<float, float> GetInitialFormationArea(int formationIndex, bool isPlayerSide, FormationClass fc)
        {
            var config = this.TestBattleConfig;
            int troopCount = isPlayerSide
                ? config.playerTroops[formationIndex].troopCount
                : config.enemyTroops[formationIndex].troopCount;
            return Utility.GetFormationArea(fc, troopCount, config.SoldiersPerRow);
        }

        private TL.Vec2 GetFormationDirection(bool isAttackerSide)
        {
            return isAttackerSide ? TestBattleConfig.FormationDirection : -TestBattleConfig.FormationDirection;
        }

        private TL.Vec3 GetFormationPosition(bool isAttackerSide, float distanceToInitialPosition)
        {
            var xDir = this.TestBattleConfig.FormationDirection;
            var pos = this.TestBattleConfig.FormationPosition
                      + distanceToInitialPosition * (isAttackerSide ? -xDir : xDir);
            if (!isAttackerSide)
                pos += xDir * this.TestBattleConfig.Distance;
            return pos.ToVec3(Utility.GetSceneHeightForAgent(Mission.Scene, pos));
        }

    }
}