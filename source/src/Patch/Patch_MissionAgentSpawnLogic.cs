using System;
using System.Collections.Generic;
using System.Reflection;
using EnhancedBattleTest.Config;
using EnhancedBattleTest.Data.MissionData;
using EnhancedBattleTest.Multiplayer.Data.MissionData;
using EnhancedBattleTest.SinglePlayer.Data.MissionData;
using TaleWorlds.Core;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade;

namespace EnhancedBattleTest.Patch
{
    public class Patch_MissionAgentSpawnLogic
    {
        private static readonly FieldInfo HasBeenPositionedProperty =
            typeof(Formation).GetField("HasBeenPositioned", BindingFlags.Instance | BindingFlags.NonPublic);

        public static bool SpawnTroops_Prefix(int number, bool isReinforcement, bool enforceSpawningOnInitialPoint, ref int __result,
            IMissionTroopSupplier ____troopSupplier,
            bool ____spawnWithHorses, BattleSideEnum ____side, MBList<Formation> ____spawnedFormations, ref List<IAgentOriginBase> ____preSuppliedTroops)
        {
			if (number <= 0)
			{
				__result = 0;
			}
			int num = 0;
			List<IAgentOriginBase> list = new List<IAgentOriginBase>();
			int num2 = Math.Min(____preSuppliedTroops.Count, number);
			if (num2 > 0)
			{
				for (int i = 0; i < num2; i++)
				{
					list.Add(____preSuppliedTroops[i]);
				}
				____preSuppliedTroops.RemoveRange(0, num2);
			}
			int numberToAllocate = number - num2;
			list.AddRange(____troopSupplier.SupplyTroops(numberToAllocate));
			List<EnhancedBattleTestAgentOrigin> list2 = new List<EnhancedBattleTestAgentOrigin>();
			for (int j = 0; j < 8; j++)
			{
				list2.Clear();
				EnhancedBattleTestAgentOrigin agentOriginBase = null;
				FormationClass formationClass = (FormationClass)j;
				foreach (EnhancedBattleTestAgentOrigin item in list)
				{
					if (formationClass == item.Troop.GetFormationClass(item.BattleCombatant))
					{
						if (item.Troop == Game.Current.PlayerTroop)
						{
							agentOriginBase = item;
						}
						else
						{
							list2.Add(item);
						}
					}
				}
				if (agentOriginBase != null)
				{
					list2.Add(agentOriginBase);
				}
				int count = list2.Count;
				if (count > 0)
				{
					foreach (EnhancedBattleTestAgentOrigin item2 in list2)
					{
						Formation formation;
						if (((SPTroopSupplier)____troopSupplier)._isPlayerSide)
                        {
							formation = Mission.GetAgentTeam(item2, true).GetFormation(formationClass);
						}
						else
                        {
							formation = Mission.GetAgentTeam(item2, false).GetFormation(formationClass);

						}
						bool isMounted = ____spawnWithHorses && (formationClass == FormationClass.Cavalry || formationClass == FormationClass.LightCavalry || formationClass == FormationClass.HeavyCavalry || formationClass == FormationClass.HorseArcher);
						if (formation != null && !(bool)HasBeenPositionedProperty.GetValue(formation))
						{
							formation.BeginSpawn(count, isMounted);
							Mission.Current.SpawnFormation(formation, count, ____spawnWithHorses, isMounted, isReinforcement);
							____spawnedFormations.Add(formation);
						}
						if (((SPTroopSupplier)____troopSupplier)._isPlayerSide)
						{
							Mission.Current.SpawnTroop(item2, true, hasFormation: true, ____spawnWithHorses, isReinforcement, enforceSpawningOnInitialPoint, count, num, isAlarmed: true, wieldInitialWeapons: true);

						}
						else
                        {
							Mission.Current.SpawnTroop(item2, false, hasFormation: true, ____spawnWithHorses, isReinforcement, enforceSpawningOnInitialPoint, count, num, isAlarmed: true, wieldInitialWeapons: true);

						}
						num++;
					}
				}
			}
			if (num > 0)
			{
				foreach (Team team in Mission.Current.Teams)
				{
                    //team.QuerySystem.Expire();
				}
				Debug.Print(string.Concat(num, " troops spawned on ", ____side, " side."), 0, Debug.DebugColor.DarkGreen, 64uL);
			}
			foreach (Team team2 in Mission.Current.Teams)
			{
				foreach (Formation formation2 in team2.Formations)
				{
					typeof(Formation).GetField("GroupSpawnIndex", BindingFlags.NonPublic | BindingFlags.Instance).SetValue(formation2, 0);
				}
			}
			__result = num;
			return false;
        }
    }
}
