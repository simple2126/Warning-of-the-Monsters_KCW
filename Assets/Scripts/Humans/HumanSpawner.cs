using System.Collections;
using UnityEngine;

public class HumanSpawner : MonoBehaviour
{
    private Human _human;
    private WaitForSeconds _spawnDelay = new WaitForSeconds(1.0f);  // 인간 스폰 되는 간격
    private int spawnPerWave = 10;  // 웨이브 당 인간 스폰 수 조정값
    
    public void StartSpawningHumans(int waveIdx)
    {
        StartCoroutine(SpawnHumansCoroutine(waveIdx));
    }

    private IEnumerator SpawnHumansCoroutine(int waveIdx)
    {
        int spawnedHumans = 0;
        int countPerUnit = waveIdx * spawnPerWave;
        while (spawnedHumans < countPerUnit)
        {
            SpawnHuman(waveIdx * spawnPerWave);
            spawnedHumans++;
            yield return _spawnDelay;
        }
    }

    private void SpawnHuman(int waveIdx)
    {
        GameObject obj = PoolManager.Instance.SpawnFromPool("Human", transform.position, Quaternion.identity);
        Human human = obj.GetComponent<Human>();
        human.WaveIdx = waveIdx;
    }
}