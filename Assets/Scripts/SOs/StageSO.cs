using UnityEngine;

[CreateAssetMenu(fileName = "StageSO", menuName = "StageSO")]

public class StageSO : ScriptableObject
{
    public int wave; // 스테이지별 웨이브
    public int health; // 스테이지별 체력
    public int gold; // 스테이지별 기본 골드
    public float waveStartDelay; // 웨이브 시작 전 지연 시간
    public float interWaveDelay; // 몬스터 처치가 끝나고 난 후의 다음 웨이브까지의 지연 시간
    public string story; // 스테이지별 스토리
}
