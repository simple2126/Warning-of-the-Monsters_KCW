using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class StageManager : SingletonBase<StageManager>
{
    [Header("UI")]

    [SerializeField] private TextMeshProUGUI waveTxt;
    [SerializeField] private TextMeshProUGUI healthTxt;
    [SerializeField] private TextMeshProUGUI goldTxt;

    [Header("Stat")]

    public StageSO stageSO;
    [SerializeField] private int totalWave; // 총 웨이브
    [SerializeField] private int currWave; // 현재 웨이브
    [SerializeField] private int currHealth; // 현재 체력
    [SerializeField] private int currGold; // 현재 골드

    protected override void Awake()
    {
        base.Awake();

        SetStageStat();
        ChangeUI();
    }

    // stageSO를 통해 기본 값 초기화
    private void SetStageStat()
    {
        totalWave = stageSO.wave;
        currWave = 0;
        currHealth = stageSO.health;
        currGold = stageSO.gold;
    }

    // 현재 값들로 UI 조정
    private void ChangeUI()
    {
        waveTxt.text = $"Wave {currWave} / {totalWave}";
        healthTxt.text = currHealth.ToString();
        goldTxt.text = currGold.ToString();
    }

    public void UpdateWave()
    {
        // 웨이브 증가 후 텍스트 변경
        currWave++;
        ChangeUI();
    }

    public bool CheckEndStage()
    {
        return (currWave == totalWave);
    }
}
