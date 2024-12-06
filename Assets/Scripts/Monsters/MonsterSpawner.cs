using System.Linq;
using UnityEngine;

public class MonsterSpawner : MonoBehaviour
{
    [SerializeField] private Transform spawnPoint;
    [SerializeField] private StageManager stageManager;
    private Monster _monster;
    private Transform[] _spawnPoints;
    
    private void Start()
    {
        GameObject[] spawnPointObjects = GameObject.FindGameObjectsWithTag("MonsterSpawnPoint"); //NO FIND...
        _spawnPoints = spawnPointObjects.Select(go => go.transform).ToArray();
    }
    
    private bool IsSpawnPointOccupied(Vector3 spawnPosition, float checkRadius)
    {
        Collider2D[] colliders = Physics2D.OverlapCircleAll(spawnPosition, checkRadius, LayerMask.GetMask("Monster"));
        foreach (var collider in colliders)
        {
            if (Vector3.Distance(spawnPosition, collider.transform.position) < checkRadius)
            {
                return true;
            }
        }
        return false;
    }
    
    private void SpawnMonster(Vector3 spawnPosition, MonsterSO selectedMonsterData)
    {
        if (IsSpawnPointOccupied(spawnPosition, 0.5f))
        {
            print("Spawn point is already occupied by another monster.");
            return;
        }
        
        GameObject monster = PoolManager.Instance.SpawnFromPool(selectedMonsterData.poolTag, spawnPosition, Quaternion.identity);
        if (monster != null)
        {
            monster.transform.position = spawnPosition;
            monster.SetActive(true);
            
            var monsterComponent = monster.GetComponent<Monster>();
            if (monsterComponent != null)
            {
                monsterComponent.data = selectedMonsterData;
                monsterComponent.Reset();
            }
        }
    }
    
    void Update()
    {
        if (Input.GetMouseButtonDown(0)) //when player touch
        {
            Vector3 touchPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            touchPosition.z = 0f;
            
            foreach (Transform spawnPoint in _spawnPoints)
            {
                MonsterSO selectedMonsterData = MonsterManager.Instance.GetSelectedMonsterData();
                if (Vector2.Distance(touchPosition, spawnPoint.position) < 0.5f)
                {
                    if (MonsterManager.Instance.SelectedMonsterId != 0)
                    {
                        if (stageManager.CurrGold >= selectedMonsterData.requiredCoins)
                        {
                            stageManager.ChangeGold(-selectedMonsterData.requiredCoins);
                            Vector3 spawnPosition = spawnPoint.position;
                            SpawnMonster(spawnPosition, selectedMonsterData);
                            // 나중에 big 몬스터인지 small 몬스터인지 판별하는 조건 추가
                            SoundManager.Instance.PlaySFX(SfxType.SpawnSmallMonster);
                        }
                        else
                        {
                            print("You do not have enough gold to spawn monster");
                        }
                    }
                }
            }
        }
    }
}