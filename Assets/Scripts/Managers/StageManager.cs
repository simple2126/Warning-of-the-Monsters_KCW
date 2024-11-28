using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class StageManager : SingletonBase<StageManager>
{
    [Header("UI")]

    [SerializeField] private TextMeshProUGUI waveTxt;
    [SerializeField] private TextMeshProUGUI healthTxt;
    [SerializeField] private TextMeshProUGUI goldTxt;
    [SerializeField] private GameObject optionPanel;

    [Header("Stat")]

    public StageSO stageSO;
    [SerializeField] private int totalWave; // �� ���̺�
    [SerializeField] private int currWave; // ���� ���̺�
    [SerializeField] private int currHealth; // ���� ü��
    [SerializeField] private int currGold; // ���� ���

    protected override void Awake()
    {
        base.Awake();

        SetStageStat();
        ChangeUI();
    }

    private void Start()
    {
        SoundManager.Instance.PlayBGM(BgmType.Stage);
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
    private void ChangeUI()
    {
        waveTxt.text = $"Wave {currWave} / {totalWave}";
        healthTxt.text = currHealth.ToString();
        goldTxt.text = currGold.ToString();
    }

    // health ����
    public void ChangeHealth(int health)
    {
        currHealth += health;
        ChangeUI();
    }

    // Wave ������Ʈ
    public void UpdateWave()
    {
        // ���̺� ���� �� �ؽ�Ʈ ����
        currWave++;
        ChangeUI();
    }

    // ���� �������� Ŭ���� �ƴ��� üũ
    public bool CheckEndStage()
    {
        return (currWave == totalWave);
    }

    // Stop ��ư ������ �� optionPanel Ȱ��ȭ
    public void ShowOptionPanel()
    {
        optionPanel.SetActive(true);
    }

    public void ClickBgmButton()
    {
        if (SoundManager.Instance.isPlayBGM)
        {
            SoundManager.Instance.StopBGM();
        }
        else
        {
            SoundManager.Instance.PlayBGM(BgmType.Stage);
        }
    }
}
