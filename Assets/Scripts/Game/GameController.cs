using UnityEngine;

public class GameController : MonoBehaviour
{
    private void OnEnable()
    {
        HumanManager.Instance.OnGameClear -= GameClear;
        HumanManager.Instance.OnGameClear += GameClear;
        StageManager.Instance.OnGameOver -= GameOver;
        StageManager.Instance.OnGameOver += GameOver;
    }
    
    private void GameClear()
    {
        //StartCoroutine(EndGame<WinPopup>());
        EndGame<WinPopup>();
    }
    
    private void GameOver()
    {
        //StartCoroutine(EndGame<LosePopup>());
        EndGame<LosePopup>();
    }

    // private IEnumerator EndGameProcess<T>() where T : UIBase
    // {
    //     Time.timeScale = 0;
    //     yield return new WaitForSecondsRealtime(0.5f);
    //     UIManager.Instance.Show<T>("UI/UIPopup/");
    // }
    
    private void EndGame<T>() where T : UIBase
    {
        Time.timeScale = 0;
        UIManager.Instance.Show<T>("UI/UIPopup/");
    }
}