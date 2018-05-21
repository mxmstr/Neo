using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;
using System.IO;
using System;
using System.Reflection;


public class Branch : MonoBehaviour
{

    [System.Serializable]
    private class ResultSpeed
    {

        public object this[string propertyName]
        {
            get
            {
                Type myType = typeof(ResultSpeed);
                FieldInfo myPropInfo = myType.GetField(propertyName);
                return myPropInfo == null ? null : myPropInfo.GetValue(this);
            }
            set
            {
                GetType().GetField(propertyName).SetValue(this, value);
            }
        }

        public string[] AnySpeed;
        public string[] Stand;
        public string[] Walk;
        public string[] Run;

        public void OverrideResults()
        {
            
            if (AnySpeed != null)
                foreach (FieldInfo field in GetType().GetFields())
                    if (field.Name != "AnySpeed")
                        this[field.Name] = AnySpeed;

        }

    }

    [System.Serializable]
    private class ResultDirection
    {

        public object this[string propertyName]
        {
            get
            {
                Type myType = typeof(ResultDirection);
                FieldInfo myPropInfo = myType.GetField(propertyName);
                return myPropInfo == null ? null : myPropInfo.GetValue(this);
            }
            set
            {
                GetType().GetField(propertyName).SetValue(this, value);
            }
        }

        public ResultSpeed AnyDirection;
        public ResultSpeed Stand;
        public ResultSpeed Forward;
        public ResultSpeed ForwardLeft;
        public ResultSpeed ForwardRight;
        public ResultSpeed Left;
        public ResultSpeed Right;
        public ResultSpeed Backward;
        public ResultSpeed BackwardLeft;
        public ResultSpeed BackwardRight;

        public void OverrideResults()
        {

            bool hasResults = false;

            if (AnyDirection == null)
                return;

            foreach (FieldInfo field in AnyDirection.GetType().GetFields())
                if (AnyDirection[field.Name] != null)
                    hasResults = true;

            foreach (FieldInfo field in GetType().GetFields())
            {

                if (hasResults && field.Name != "AnyDirection")
                    this[field.Name] = AnyDirection;

                if (this[field.Name] != null)
                    ((ResultSpeed)this[field.Name]).OverrideResults();

            }

        }

    }

    [System.Serializable]
    private class ResultInput
    {

        public object this[string propertyName]
        {
            get
            {
                Type myType = typeof(ResultInput);
                FieldInfo myPropInfo = myType.GetField(propertyName);
                return myPropInfo == null ? null : myPropInfo.GetValue(this);
            }
            set
            {
                GetType().GetField(propertyName).SetValue(this, value);
            }
        }

        public ResultDirection AnyInput;
        public ResultDirection Primary;
        public ResultDirection Secondary;
        public ResultDirection Jump;

        public void OverrideResults()
        {

            bool hasResults = false;

            if (AnyInput == null)
                return;

            foreach (FieldInfo field in AnyInput.GetType().GetFields())
                if (AnyInput[field.Name] != null)
                    hasResults = true;
            
            foreach (FieldInfo field in GetType().GetFields()) {

                if (hasResults && field.Name != "AnyInput")
                    this[field.Name] = AnyInput;

                if (this[field.Name] != null)
                    ((ResultDirection)this[field.Name]).OverrideResults();

            }

        }

    }

    [System.Serializable]
    private class BranchData
    {

        public string name;
        public float cooldown;
        public string[] actions;
        public ResultInput results;

    }

    [System.Serializable]
    private class BranchTable
    {
        public BranchData[] branches;
    }

    public string filename = "Branches.json";

    private Action m_Action;
    private BranchTable table;
    private BranchData branch;
    private float m_Cooldown = -1.0f;


    void Start()
    {
        
        m_Action = GetComponent<Action>();

        LoadBranchData();
        ResetBranch();

    }


    private void LoadBranchData()
    {

        string filePath = Path.Combine(Application.streamingAssetsPath, filename);
        table = JsonUtility.FromJson<BranchTable>(File.ReadAllText(filePath));

        Type inputType = typeof(ResultInput);
        Type dirType = typeof(ResultDirection);
        Type speedType = typeof(ResultSpeed);

        foreach (BranchData data in table.branches)
            data.results.OverrideResults();

    }


    private void StartBranch(string branchName)
    {
        
        foreach (BranchData data in table.branches)
        {
            if (data.name == branchName)
            {
                branch = data;
                break;
            }
        }

        m_Cooldown = branch.cooldown;

    }


    private void ResetBranch()
    {

        StartBranch("B_Default");

    }


    private string GetAction()
    {

        System.Random rand = new System.Random();

        return branch.actions[rand.Next(branch.actions.Length)];

    }

    
    public void StartAction(string input, string direction, string speed)
    {
        
        try
        {
            System.Random rand = new System.Random();

            if (!m_Action.IsReset())
                return;
            
            string[] branches = (string[])(
                (ResultSpeed)((ResultDirection)branch.results[input])[direction]
                )[speed];

            StartBranch(branches[rand.Next(branches.Length)]);
            m_Action.StartAction(GetAction());
        }
        catch (NullReferenceException e) { }

        

        /*bool hasInputAny = branch.results.AnyInput != null;
        bool hasInputParam = branch.results[input] != null;

        if (hasInputAny || hasInputParam)
            inputs = branch.results;
        else
            return;

        Debug.Log("1");

        bool hasDirectionAny = inputs.AnyInput.AnyDirection != null || inputs.AnyInput[direction] != null;
        bool hasDirectionParam = ((ResultDirection)inputs[input])[direction] != null;

        if (hasDirectionAny)
            directions = inputs.AnyInput;
        else if (hasDirectionParam)
            directions = (ResultDirection)inputs[input];
        else
            return;

        Debug.Log("2");

        bool hasSpeedAny = directions.AnyDirection.AnySpeed != null || directions.AnyDirection[speed] != null;
        bool hasSpeedParam = ((ResultSpeed)directions[direction])[speed] != null;

        if (hasSpeedAny)
            speeds = directions.AnyDirection;
        else if (hasSpeedParam)
            speeds = (ResultSpeed)directions[direction];
        else
            return;

        Debug.Log("3");

        if (speeds.AnySpeed == null)
            branches = (string[])speeds[speed];
        else
            branches = speeds.AnySpeed;*/

        

    }


    public void LateUpdate()
    {
        
        if (m_Cooldown != -1.0 && m_Action.IsReset()) {
            m_Cooldown -= Time.deltaTime;
            if (m_Cooldown < 0)
                ResetBranch();
        }

    }

}