using UnityEngine;

[CreateAssetMenu(fileName = "StageSO", menuName = "StageSO")]

public class StageSO : ScriptableObject
{
    public int wave; // ���������� ���̺�
    public int health; // ���������� ü��
    public int gold; // ���������� �⺻ ���
    public float interWaveDelay; // ���� óġ�� ������ �� ���� ���� ���̺������ ���� �ð�
}
