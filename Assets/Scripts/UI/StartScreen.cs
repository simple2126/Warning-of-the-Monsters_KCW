using UnityEngine;
using UnityEngine.UI;

public class StartScreen : UIBase
{
    public Button startButton;
    // Start is called before the first frame update
    void Start()
    {
        Debug.Log("��ŸƮȭ�����");
        startButton.onClick.AddListener(OnButtonClicked);
    }

    public void OnButtonClicked()
    {
        Debug.Log("��ŸƮŬ��");
    }
}
