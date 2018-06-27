using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using UnityEngine;
using UnityEngine.AI;
using UnityStandardAssets.Characters.ThirdPerson;


public class Phase : MonoBehaviour {

    private object this[string propertyName]
    {
        get
        {
            Type myType = typeof(Phase);
            MethodInfo myPropInfo = myType.GetMethod(propertyName);
            return myPropInfo == null ? null : myPropInfo;
        }
    }

    [System.Serializable]
    public class CharacterData
    {

        public string prefab;
        public int id;
        public bool active;
        public string[] scripts;
        public Vector3 position;
        public Vector3 rotation;

    }

    [System.Serializable]
    public class PhaseData
    {

        public string name;
        public string next_trigger;
        public CharacterData[] spawned;
        public CharacterData[] existing;

    }

    [System.Serializable]
    public class PhaseTable
    {
        public PhaseData[] phases;
    }

    public string m_Source = "Phases.json";
    public NavMeshData m_NavMeshData;
    public List<string> m_Phases = new List<string>();

    private PhaseTable m_PhaseTable;
    private PhaseData m_PhaseData;
    private GameObject m_Characters;
    private delegate bool Trigger();
    private Trigger m_NextTrigger;


    void Start () {

        NavMesh.AddNavMeshData(m_NavMeshData);
        NavMesh.CalculateTriangulation();

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

        if (m_PhaseData.next_trigger != null)
        {
            m_NextTrigger = delegate ()
                {
                    return (bool)((MethodInfo)this[m_PhaseData.next_trigger]).Invoke(this, null);
                };
        }
        else
        {
            m_NextTrigger = delegate ()
                {
                    return false;
                };
        }

        if (m_PhaseData.spawned != null)
        {
            foreach (CharacterData data in m_PhaseData.spawned)
            {

                GameObject character = Instantiate(
                    Resources.Load(data.prefab),
                    data.position,
                    Quaternion.Euler(data.rotation)
                    ) as GameObject;

                character.GetComponent<Character>().SetActive(data.active);

                foreach (string script in data.scripts)
                {
                    Type t = Type.GetType(script);
                    character.AddComponent(t);
                }

            }
        }

        if (m_PhaseData.existing != null)
        {
            foreach (CharacterData data in m_PhaseData.existing)
            {

                GameObject character = Array.Find(
                    GameObject.FindGameObjectsWithTag("Character"),
                    delegate (GameObject go) { return go.GetComponent<Character>().GetID() == data.id; }
                    );

                if (data.position != null)
                    character.transform.position = data.position;
                if (data.rotation != null)
                    character.transform.rotation = Quaternion.Euler(data.rotation);

                character.GetComponent<Character>().SetActive(data.active);

                foreach (string script in data.scripts)
                {
                    Type t = Type.GetType(script);
                    character.AddComponent(t);
                }

            }
        }
        
    }


    public bool AllBlueDead()
    {

        foreach (GameObject go in GameObject.FindGameObjectsWithTag("Character"))
        {
            Character character = go.GetComponent<Character>();
            if (character.GetGroup() == Group.Blue && (character.HasLives() || character.HasHealth()))
                return false;
        }

        return true;

    }


    void Update () {

        if (m_NextTrigger())
            NextPhase();

	}

}
