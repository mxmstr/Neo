using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class Phase : MonoBehaviour {

    [System.Serializable]
    public class CharacterData
    {

        public string prefab;
        public string[] scripts;
        public Vector3 position;
        public Vector3 rotation;

    }

    [System.Serializable]
    public class PhaseData
    {

        public string name;
        public CharacterData[] characters;
        
    }

    [System.Serializable]
    public class PhaseTable
    {
        public PhaseData[] phases;
    }

    public string m_Source = "Phases.json";
    public List<string> m_Phases = new List<string>();

    private PhaseTable m_PhaseTable;
    private PhaseData m_PhaseData;
    private GameObject m_Characters;


    void Start () {

        LoadPhasesData();

        NextPhase();

    }


    void LoadPhasesData()
    {

        string filePath = Path.Combine(Application.streamingAssetsPath, m_Source);
        m_PhaseTable = JsonUtility.FromJson<PhaseTable>(File.ReadAllText(filePath));

    }


    void NextPhase()
    {

        if (m_Phases.Count == 0)
            return;

        string phaseName = m_Phases[0];
        m_Phases.RemoveAt(0);


        foreach (PhaseData data in m_PhaseTable.phases)
        {
            if (data.name == phaseName)
            {
                m_PhaseData = data;
                break;
            }
        }


        foreach (CharacterData data in m_PhaseData.characters)
        {

            GameObject character = Instantiate(Resources.Load(data.prefab), data.position, Quaternion.Euler(data.rotation)) as GameObject;

            foreach (string script in data.scripts)
            {
                Type t = Type.GetType(script);
                character.AddComponent(t);
            }
            
        }
        

    }


    void Update () {
		

	}

}
