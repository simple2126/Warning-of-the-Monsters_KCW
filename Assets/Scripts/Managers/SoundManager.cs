using System.Collections.Generic;
using UnityEngine;

public class SoundManager : SingletonBase<SoundManager>
{
    [Header("BGM List")]
    Dictionary<BgmType, AudioClip> bgmDict = new Dictionary<BgmType, AudioClip>();
    
    [System.Serializable]
    public class BgmKeyValuePair
    {
        public BgmType bgmType;
        public AudioClip clip;
    }
    [SerializeField] private List<BgmKeyValuePair> bgmList;

    [Header("SFX List")]
    Dictionary<SfxType, AudioClip> sfxDict = new Dictionary<SfxType, AudioClip>();

    [System.Serializable]
    public class SfxKeyValuePair
    {
        public SfxType sfxType;
        public AudioClip clip;
    }
    [SerializeField] private List<SfxKeyValuePair> SfxList;

    [SerializeField] AudioSource audioBgm;
    [SerializeField] AudioSource audioSfx;

    public bool isPlayBGM;

    protected override void Awake()
    {
        base.Awake();

        DontDestroyOnLoad(this);
        SetBgmDictionary();
        SetSfxDictionary();
    }

    private void SetBgmDictionary()
    {
        if (bgmList == null) return; 

        foreach(BgmKeyValuePair bgm in bgmList)
        {
            if (!bgmDict.ContainsKey(bgm.bgmType))
            {
                bgmDict.Add(bgm.bgmType, bgm.clip);
            }
        }
    }

    private void SetSfxDictionary()
    {
        if (SfxList == null) return;

        foreach (SfxKeyValuePair sfx in SfxList)
        {
            if (!sfxDict.ContainsKey(sfx.sfxType)) 
            {
                sfxDict.Add(sfx.sfxType, sfx.clip);
            }
        }
    }

    // ��� ���� ���
    public void PlayBGM(BgmType bgmType)
    {
        isPlayBGM = true;
        audioBgm.clip = bgmDict[bgmType];
        audioBgm.Play();
    }

    // ��� ���� ����
    public void StopBGM()
    {
        isPlayBGM = false;
        audioBgm.Stop();
    }

    // ȿ���� ���
    public void PlaySFX(SfxType sfxType)
    {
        // enum -> int �� ����ȯ
        audioSfx.PlayOneShot(sfxDict[sfxType]);
    }
}
