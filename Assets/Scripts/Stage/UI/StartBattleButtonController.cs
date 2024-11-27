using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class StartBattleButtonController : MonoBehaviour
{
    [SerializeField] private Image timeCircleImage;

    private bool isWaveEnd = false; // 웨이브가 끝났는지 확인
    private float interWaveDelay; // 웨이브 간 지연 시간
    private float currInterDelayTime = 0f; // 현재 지연 시간 설정

    private Coroutine coroutine; // 지연 시간 계산 코루틴
    private WaitForSeconds coroutineSeconds; // 지연 시간 캐싱 필드

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

    // StartBattleBtn 클릭
    public void StartWave()
    {
        isWaveEnd = false;
        timeCircleImage.fillAmount = 0f;
        currInterDelayTime = 0f;
        gameObject.SetActive(false);
        if (coroutine != null) StopCoroutine(coroutine);

        StageManager.Instance.UpdateWave();
    }

    // 다음 웨이브 시간 계산
    private IEnumerator CoInterWaveDelay()
    {
        yield return coroutineSeconds;
        isWaveEnd = true;
    }

    // StartBattleBtn 이미지 변화
    private void ChangeStartBattleBtn()
    {
        timeCircleImage.fillAmount = currInterDelayTime / interWaveDelay;
    }

    // 웨이브가 끝났을 때
    public void EndWave()
    {
        isWaveEnd = true;
        gameObject.SetActive(true);
        coroutine = StartCoroutine(CoInterWaveDelay());
        CheckClear();
    }

    // 현재 스테이지를 클릭어 했는지 확인
    private void CheckClear()
    {
        if (StageManager.Instance.CheckEndStage())
        {
            Time.timeScale = 0f;
            // 클리어 팝업 추가
        }
        else ChangeStartBattleBtn();
    }
}
