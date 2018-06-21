using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using UnityEngine;


public class Sound : MonoBehaviour {


    [System.Serializable]
    public class SoundData
    {
        
        public string name;
        public string type;
        public string[] clips;
        public int priority;
        public float volume;
        public float pitch;

    }

    [System.Serializable]
    public class SoundTable
    {
        public SoundData[] sounds;
    }


    public string m_SoundSource = "Sounds.json";

    private Animator m_Animator;
    private AudioSource m_Voice;
    private AudioSource m_Effects;
    private SoundTable m_SoundTable;
    private SoundData m_SoundData;


    void Start () {

        m_Animator = GetComponent<Animator>();
        m_Voice = Array.Find(
                GetComponents<AudioSource>(),
                delegate (AudioSource a) { return a.outputAudioMixerGroup.name == "Voice"; }
                );
        m_Effects = Array.Find(
                GetComponents<AudioSource>(),
                delegate (AudioSource a) { return a.outputAudioMixerGroup.name == "Effects"; }
                );

        LoadSoundData();
        
    }


    private void LoadSoundData()
    {

        string filePath = Path.Combine(Application.streamingAssetsPath, m_SoundSource);
        m_SoundTable = JsonUtility.FromJson<SoundTable>(File.ReadAllText(filePath));

    }


    public void PlaySound(string soundName)
    {
        
        foreach (SoundData data in m_SoundTable.sounds)
        {
            if (data.name == soundName)
            {
                m_SoundData = data;
                break;
            }
        }
        
        System.Random rand = new System.Random();
        string clip = m_SoundData.clips[rand.Next(m_SoundData.clips.Length)];
        AudioSource source;

        if (m_SoundData.type == "Voice")
            source = m_Voice;
        else
            source = m_Effects;

        source.priority = m_SoundData.priority;
        source.volume = m_SoundData.volume;
        source.pitch = m_SoundData.pitch;
        source.PlayOneShot(Resources.Load(clip, typeof(AudioClip)) as AudioClip);

    }


    public void PlaySoundSet(string soundName)
    {

        PlaySound(soundName);

    }

}
