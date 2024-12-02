using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

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

    private AudioSource audioBgm;
    
    // BGM ����
    [SerializeField][Range(0f, 1f)] private float bgmVolume;

    // SFX ���� �� �� ������ ����
    [SerializeField][Range(0f, 1f)] private float sfxVolume;
    [SerializeField][Range(0f, 1f)] private float sfxPitchVariance; // ���� ���� ����

    public bool IsPlayBGM { get; private set; } // BGM ��� ���� (On / Off)
    public bool IsPlaySFX { get; private set; }// SFX ��� ���� (On / Off)

    protected override void Awake()
    {
        base.Awake();

        audioBgm = GetComponent<AudioSource>();
        audioBgm.volume = bgmVolume;
        audioBgm.loop = true;
        IsPlayBGM = true;
        IsPlaySFX = true;

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

    // ��� ���� ����
    public void PlayBGM(BgmType bgmType)
    {
        if (IsPlayBGM)
        {
            audioBgm.clip = bgmDict[bgmType];
            audioBgm.mute = false;
            audioBgm.Play();
        }
    }

    // ��� ���� ����
    public void StopBGM()
    {
        audioBgm.Stop();
    }

    // ȿ���� ���
    public void PlaySFX(SfxType sfxType)
    {
        if (IsPlaySFX)
        {
            // enum -> int �� ����ȯ
            GameObject obj = PoolManager.Instance.SpawnFromPool(sfxType.ToString());
            obj.SetActive(true);
            SfxSoundSource soundSource = obj.GetComponent<SfxSoundSource>();
            soundSource.Play(sfxDict[sfxType], sfxType, sfxVolume, sfxPitchVariance);
        }
    }

    // BGM On / Off
    public void ChangeIsPlayBGM()
    {
        IsPlayBGM = !IsPlayBGM;
    }

    // SFX On / Off
    public void ChangeIsPlaySFX()
    {
        IsPlaySFX = !IsPlaySFX;
    }

    public void OnClick(InputAction.CallbackContext context)
    {
        // IsPointerOverGameObject() -> UI�� �۵��ϵ��� ����
        if(context.started && EventSystem.current != null && EventSystem.current.IsPointerOverGameObject())
        {
            PlaySFX(SfxType.Click);
        }
    }
}
