using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class StartBattleButtonController : MonoBehaviour
{
    [SerializeField] private Image timeCircleImage;

    private bool isWaveEnd = false; // ���̺갡 �������� Ȯ��
    private float interWaveDelay; // ���̺� �� ���� �ð�
    private float currInterDelayTime = 0f; // ���� ���� �ð� ����

    private Coroutine coroutine; // ���� �ð� ��� �ڷ�ƾ
    private WaitForSeconds coroutineSeconds; // ���� �ð� ĳ�� �ʵ�

    private void Awake()
    {
        interWaveDelay = StageManager.Instance.stageSO.interWaveDelay;
        coroutineSeconds = new WaitForSeconds(interWaveDelay);
    }

    private void Update()
    {
        if (isWaveEnd)
        {
            currInterDelayTime += Time.deltaTime;
            ChangeStartBattleBtn();
            if (currInterDelayTime >= interWaveDelay)
            {
                StartWave();
            }
        }
    }

    // StartBattleBtn Ŭ��
    public void StartWave()
    {
        isWaveEnd = false;
        timeCircleImage.fillAmount = 0f;
        currInterDelayTime = 0f;
        gameObject.SetActive(false);
        if (coroutine != null) StopCoroutine(coroutine);

        StageManager.Instance.UpdateWave();
    }

    // ���� ���̺� �ð� ���
    private IEnumerator CoInterWaveDelay()
    {
        yield return coroutineSeconds;
        isWaveEnd = true;
    }

    // StartBattleBtn �̹��� ��ȭ
    private void ChangeStartBattleBtn()
    {
        timeCircleImage.fillAmount = currInterDelayTime / interWaveDelay;
    }

    // ���̺갡 ������ ��
    public void EndWave()
    {
        if (!gameObject.activeSelf && !CheckClear())
        {
            isWaveEnd = true;
            gameObject.SetActive(true);
            coroutine = StartCoroutine(CoInterWaveDelay());
        }
    }

    // ���� ���������� Ŭ���� �ߴ��� Ȯ��
    private bool CheckClear()
    {
        if (StageManager.Instance.CheckEndStage())
        {
            Time.timeScale = 0f;
            SoundManager.Instance.StopBGM();
            return true;
            // Ŭ���� �˾� �߰�
        }
        else
        {
            ChangeStartBattleBtn();
            return false;
        }
    }
}
