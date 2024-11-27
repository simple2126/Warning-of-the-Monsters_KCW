using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class StageManager : SingletonBase<StageManager>
{
    [SerializeField] private StageSO stageSO;
    [SerializeField] private TextMeshProUGUI waveTxt;
    [SerializeField] private TextMeshProUGUI healthTxt;
    [SerializeField] private TextMeshProUGUI goldTxt;
 
    [SerializeField] private int totalWave; // �� ���̺�
    [SerializeField] private int currWave; // ���� ���̺�
    [SerializeField] private int currHealth; // ���� ü��
    [SerializeField] private int currGold; // ���� ���

    private bool isStart = false;

    protected override void Awake()
    {
        base.Awake();

        SetStageStat();
        SetUI();
    }

    // stageSO�� ���� �⺻ �� �ʱ�ȭ
    private void SetStageStat()
    {
        totalWave = stageSO.wave;
        currWave = 0;
        currHealth = stageSO.health;
        currGold = stageSO.gold;
    }

    // ���� ����� UI ����
    private void SetUI()
    {
        waveTxt.text = $"Wave {currWave} / {totalWave}";
        healthTxt.text = currHealth.ToString();
        goldTxt.text = currGold.ToString();
    }

    // StartBattleBtn Ŭ��
    public void ClickStartBattleBtn()
    {
        isStart = true;
    }
}
