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
    private AudioSource m_Music;
    private SoundTable m_SoundTable;
    private SoundData m_SoundData;
    private AudioSource m_Source;


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
        m_Music = Array.Find(
                GetComponents<AudioSource>(),
                delegate (AudioSource a) { return a.outputAudioMixerGroup.name == "Music"; }
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

        if (m_SoundData.type == "Voice")
            m_Source = m_Voice;
        else if (m_SoundData.type == "Effect")
            m_Source = m_Effects;
        else
            m_Source = m_Music;
        
        m_Source.priority = m_SoundData.priority;
        m_Source.volume = m_SoundData.volume;
        m_Source.pitch = m_SoundData.pitch;
        m_Source.PlayOneShot(Resources.Load(clip, typeof(AudioClip)) as AudioClip);

    }


    public void StopSound()
    {

        if (m_Source != null)
            m_Source.Stop();

    }


    public void PlaySoundSet(string soundName)
    {

        PlaySound(soundName);

    }

}
