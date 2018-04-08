using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.Characters.ThirdPerson;
using UnityStandardAssets.CrossPlatformInput;
using System.IO;



public class Action : MonoBehaviour
{

    [System.Serializable]
    private class ActionData
    {

        public string name;
        public string animation;
        public bool rotation;
        public bool movement;
        public bool blendSpace;
        public Vector3 velocity;
        public float speed;
        public float damage;

    }

    [System.Serializable]
    private class ActionTable
    {
        public ActionData[] actions;
    }

    private Character m_Character;
    private string filename = "Actions.json";
    private ActionTable table;


    void Start ()
    {

        /*var table = new ActionTable();
        var data = new ActionData();

        table.actions = new ActionData[] { data };

        print(JsonUtility.ToJson(table, true));*/

        LoadGameData();


    }


    private void LoadGameData()
    {

        string filePath = Path.Combine(Application.streamingAssetsPath, filename);
        
        if (File.Exists(filePath))
        {
            string dataAsJson = File.ReadAllText(filePath);

            table = JsonUtility.FromJson<ActionTable>(dataAsJson);
        }
        else
            Debug.LogError("Cannot load game data!");

    }


    void Update ()
    {
		
	}


}
