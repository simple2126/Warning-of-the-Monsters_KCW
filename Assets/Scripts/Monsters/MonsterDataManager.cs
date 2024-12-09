using System.Collections.Generic;
using UnityEngine;

public class MonsterDataManager : MonoBehaviour
{
    public static MonsterDataManager Instance { get; private set; }
    private List<MonsterSO> _monsterSOs;
    
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }
    
    public MonsterSO[] LoadMonsterData()
    {
        List<Monster_Data.Monster_Data> monsterDataList = Monster_Data.Monster_Data.GetList();
        List<Monster_Data.Upgrade_Data> upgradeDataList = Monster_Data.Upgrade_Data.GetList();

        _monsterSOs = new List<MonsterSO>();

        foreach (var monsterData in monsterDataList)
        {
            MonsterSO baseMonsterSo = ScriptableObject.CreateInstance<MonsterSO>();
            baseMonsterSo.id = monsterData.id;
            baseMonsterSo.upgradeLevel = 0;
            baseMonsterSo.poolTag = monsterData.name;
            baseMonsterSo.fatigue = monsterData.fatigue;
            baseMonsterSo.fearInflicted = monsterData.fearInflicted;
            baseMonsterSo.cooldown = monsterData.cooldown;
            baseMonsterSo.humanScaringRange = monsterData.humanScaringRange;
            baseMonsterSo.requiredCoins = monsterData.requiredCoins;

            _monsterSOs.Add(baseMonsterSo);

            foreach (var upgradeData in upgradeDataList)
            {
                if (monsterData.id == Mathf.FloorToInt(upgradeData.monster_id))
                {
                    MonsterSO upgradedMonsterSo = ScriptableObject.CreateInstance<MonsterSO>();
                    upgradedMonsterSo.monsterId = upgradeData.monster_id;
                    upgradedMonsterSo.upgradeLevel = upgradeData.upgrade_level;
                    upgradedMonsterSo.poolTag = monsterData.name + "_Upgrade" + upgradeData.upgrade_level;
                    upgradedMonsterSo.fatigue = upgradeData.fatigue;
                    upgradedMonsterSo.fearInflicted = upgradeData.fearInflicted;
                    upgradedMonsterSo.cooldown = upgradeData.cooldown;
                    upgradedMonsterSo.humanScaringRange = monsterData.humanScaringRange;
                    upgradedMonsterSo.requiredCoins = upgradeData.requiredCoins;

                    _monsterSOs.Add(upgradedMonsterSo);
                }
            }
        }
        return _monsterSOs.ToArray();
    }
    
    public Monster_Data.Upgrade_Data GetUpgradeData(int monsterId, int upgradeLevel)
    {
        var upgrades = Monster_Data.Upgrade_Data.GetList();
        Debug.Log($"Number of upgrades loaded: {upgrades.Count}");

        foreach (var upgrade in upgrades)
        {
            int baseMonsterId = Mathf.FloorToInt(upgrade.monster_id); //base id (1, 2, etc.)
            int upgradePart = (int)((upgrade.monster_id - baseMonsterId) * 10); //upgrade level (1, 2, etc.)
            
            Debug.Log($"Looking for baseMonsterId: {monsterId}, upgradeLevel: {upgradeLevel} (Current: {baseMonsterId}, Upgrade: {upgradePart})");

            if (baseMonsterId == monsterId && upgradePart == upgradeLevel)
            {
                return upgrade;
            }
        }
        return null;
    }
}
