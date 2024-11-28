using UnityEngine;

public class SoundManager : SingletonBase<SoundManager>
{

    [SerializeField] AudioClip[] bgms;
    [SerializeField] AudioClip[] sfxs;

    [SerializeField] AudioSource audioBgm;
    [SerializeField] AudioSource audioSfx;

    protected override void Awake()
    {
        base.Awake();

        DontDestroyOnLoad(this);
    }

    // ��� ���� ���
    public void PlayBGM(BgmType bgmType)
    {
        // enum -> int �� ����ȯ
        audioBgm.clip = bgms[(int)bgmType];
        audioBgm.Play();
    }

    // ��� ���� ����
    public void StopBGM()
    {
        audioBgm.Stop();
    }

    // ȿ���� ���
    public void PlaySFX(SfxType sfxType)
    {
        // enum -> int �� ����ȯ
        audioSfx.PlayOneShot(sfxs[(int)sfxType]);
    }
}
