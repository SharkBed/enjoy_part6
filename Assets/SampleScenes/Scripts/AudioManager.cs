using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;

public class AudioManager : SingletonMonoBehaviour<AudioManager>
{
    //鳴らしたいところで
    //AudioManager.Instance.PlayBGM(曲名);string型
    //を書く
    //止める時は
    //AudioManager.Instance.PlayBGM();

    //scene内でAudioManagerを作って曲をListに登録する必要がある

    public List<AudioClip> BGMList;
    public List<AudioClip> SEList;
    //同時にならせるseの最大数
    public int MaxSE = 10;

    private List<AudioSource> bgmSource = null;
    private List<AudioSource> seSources = null;

    private Dictionary<string, AudioClip> bgmDict = null;
    private Dictionary<string, AudioClip> seDict = null;

    public void Awake()
    {
        if (this != Instance)
        {
            Destroy(this);
            return;
        }

        DontDestroyOnLoad(this.gameObject);

        //create listener
        if (FindObjectsOfType(typeof(AudioListener)).All(o => !((AudioListener)o).enabled))
        {
            this.gameObject.AddComponent<AudioListener>();
        }
        //create audio sources
        this.bgmSource = new List<AudioSource>();
        this.seSources = new List<AudioSource>();

        //create clip dictionaries
        this.bgmDict = new Dictionary<string, AudioClip>();
        this.seDict = new Dictionary<string, AudioClip>();

        Action<Dictionary<string, AudioClip>, AudioClip> addClipDict = (dict, c) => {
            if (!dict.ContainsKey(c.name))
            {
                dict.Add(c.name, c);
            }
        };

        this.BGMList.ForEach(bgm => addClipDict(this.bgmDict, bgm));
        this.SEList.ForEach(se => addClipDict(this.seDict, se));
    }

    //SEを始める
    public void PlaySE(string seName)
    {
        if (!this.seDict.ContainsKey(seName)) throw new ArgumentException(seName + " not found", "seName");

        AudioSource source = this.seSources.FirstOrDefault(s => !s.isPlaying);
        if (source == null)
        {
            if (this.seSources.Count >= this.MaxSE)
            {
                Debug.Log("SE AudioSource is full");
                return;
            }

            source = this.gameObject.AddComponent<AudioSource>();
            this.seSources.Add(source);
        }

        source.clip = this.seDict[seName];
        source.Play();
    }

    //SEを止める
    public void StopSE()
    {
        this.seSources.ForEach(s => s.Stop());
    }

    //BGMを始める
    public void PlayBGM(string bgmName)
    {
        if (!this.bgmDict.ContainsKey(bgmName)) throw new ArgumentException(bgmName + " not found", "bgmName");
        AudioSource source = this.bgmSource.FirstOrDefault(s => !s.isPlaying);
        if (source == null)
        {
            source = this.gameObject.AddComponent<AudioSource>();
            this.bgmSource.Add(source);
        }
        source.clip = this.bgmDict[bgmName];
        //るーーーぷ
        source.loop = true;
        source.Play();
       
    }

    //BGMを止める
    public void StopBGM()
    {
        this.bgmSource.ForEach(s => s.Stop());
    }

    //bgmのボリューム変更
    //ネームは意味ないお
    //引数volumeの値は0f～100fの間
    public void BgmVolumeSet(string BGMname,float volume)
    {
        //if(this.bgmDict[BGMname] == this.bgmSource.FirstOrDefault(s => !s.isPlaying))
        //{
        if(volume > 100)
        {
            return;
        }
        this.bgmSource.ForEach(s => s.volume = volume/100f);
        //}
    }

    public void SEVolumeSet(string SEname, float volume)
    {
        //if(this.bgmDict[BGMname] == this.bgmSource.FirstOrDefault(s => !s.isPlaying))
        //{
        if (volume > 100)
        {
            return;
        }
        this.seSources.ForEach(s => s.volume = volume / 100f);
        //}
    }

}
