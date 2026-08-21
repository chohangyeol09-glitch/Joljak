using CHG.Scripts.SkillSystem.SkillDatas;
using DevLib.DatabaseSystem.Runtime;
using GGMLib.DatabaseSystem.Runtime;
using UnityEngine;

namespace CHG.Scripts.SkillSystem
{
    public static class SkillTableExtensions
    {
        public static SkillDataSO GetSkill(this DataTableSO table, int index)
        {
            IndexedAsset asset = table.GetAsset(index);

            if (asset == null)
            {
                Debug.LogError($"{table.tableName} {index} not found");
                return null;
            }

            if (asset is not SkillDataSO skill)
            {
                Debug.LogError($"{table.tableName} {index} is not SkillDataSO : {asset.GetType().Name}");
                return null;
            }
            return skill;
        }
    }
}