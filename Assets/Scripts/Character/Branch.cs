using UnityEngine;
using System.IO;
using System;
using System.Reflection;


public enum Input
{
    AnyInput,
    Primary,
    Secondary,
    Jump
}

public enum Direction
{
    AnyDirection,
    Stand,
    Forward,
    ForwardLeft,
    ForwardRight,
    Left,
    Right,
    Backward,
    BackwardLeft,
    BackwardRight
}

public enum Speed
{
    AnySpeed,
    Stand,
    Walk,
    Run
}


public class Branch : MonoBehaviour
{
    
    [System.Serializable]
    private class ResultSpeed
    {

        public BranchData[] this[Speed speed]
        {
            get { return results[(int)speed]; }
        }

        public string[] AnySpeed;
        public string[] Stand;
        public string[] Walk;
        public string[] Run;

        private BranchData[][] results = null;

        private BranchData GetBranch(BranchData[] branches, string branchName)
        {

            foreach (BranchData data in branches)
                if (data.name == branchName)
                    return data;

            return null;

        }

        public void OverrideResults()
        {

            if (AnySpeed != null)
                foreach (FieldInfo field in GetType().GetFields())
                    if (field.Name != "AnySpeed" && field.Name != "branches")
                        field.SetValue(this, AnySpeed);

        }

        public void SetResults(BranchData[] branches)
        {

            BranchData[][] arr = {
                AnySpeed == null ? null : Array.ConvertAll(AnySpeed, x => GetBranch(branches, x)),
                Stand == null ? null : Array.ConvertAll(Stand, x => GetBranch(branches, x)),
                Walk == null ? null : Array.ConvertAll(Walk, x => GetBranch(branches, x)),
                Run == null ? null :  Array.ConvertAll(Run, x => GetBranch(branches, x))
            };
            results = arr;

        }

    }

    [System.Serializable]
    private class ResultDirection
    {

        public ResultSpeed this[Direction direction]
        {
            get { return speeds[(int)direction]; }
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

        public ResultSpeed[] speeds = null;
        
        public void OverrideResults()
        {

            bool hasResults = false;

            if (AnyDirection != null)
            {

                foreach (FieldInfo field in AnyDirection.GetType().GetFields())
                    if (field.GetValue(AnyDirection) != null)
                        hasResults = true;

                foreach (FieldInfo field in GetType().GetFields())
                {

                    if (hasResults && field.Name != "AnyDirection" && field.Name != "speeds")
                        field.SetValue(this, AnyDirection);

                    if (field.GetValue(this) != null)
                        ((ResultSpeed)field.GetValue(this)).OverrideResults();

                }

            }

        }

        public void SetSpeeds(BranchData[] branches)
        {

            ResultSpeed[] arr = {
                AnyDirection, Stand, Forward, ForwardLeft, ForwardRight, Left, Right, Backward, BackwardLeft, BackwardRight
            };
            speeds = arr;

            foreach (ResultSpeed speed in speeds)
                if (speed != null)
                    speed.SetResults(branches);

        }

    }

    [System.Serializable]
    private class ResultInput
    {

        public ResultDirection this[Input input]
        {
            get { return directions[(int)input]; }
        }

        public ResultDirection AnyInput;
        public ResultDirection Primary;
        public ResultDirection Secondary;
        public ResultDirection Jump;

        private ResultDirection[] directions = null;
        
        public void OverrideResults()
        {

            bool hasResults = false;

            if (AnyInput == null)
                return;

            foreach (FieldInfo field in AnyInput.GetType().GetFields())
                if (field.GetValue(AnyInput) != null)
                    hasResults = true;
            
            foreach (FieldInfo field in GetType().GetFields())
            {

                if (hasResults && field.Name != "AnyInput" && field.Name != "speeds")
                    field.SetValue(this, AnyInput);

                if (field.GetValue(this) != null)
                    ((ResultDirection)field.GetValue(this)).OverrideResults();
                
            }

        }

        public void SetInputs(BranchData[] branches)
        {

            ResultDirection[] arr = { AnyInput, Primary, Secondary, Jump };
            directions = arr;

            foreach (ResultDirection direction in directions)
                if (direction != null)
                    direction.SetSpeeds(branches);

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

    public string source = "Branches_Neo.json";
    public string defaultBranch = "B_Punch_Default";

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


    public string GetName()
    {
        return branch.name;
    }


    private void LoadBranchData()
    {

        string filePath = Path.Combine(Application.streamingAssetsPath, source);
        table = JsonUtility.FromJson<BranchTable>(File.ReadAllText(filePath));
        
        foreach (BranchData data in table.branches)
        {
            data.results.OverrideResults();
            data.results.SetInputs(table.branches);
        }

    }


    private BranchData GetBranch(string branchName)
    {
        
        foreach (BranchData data in table.branches)
            if (data.name == branchName)
                return data;

        return null;

    }


    private void StartBranch(string branchName)
    {

        branch = GetBranch(branchName);

        m_Cooldown = branch.cooldown;

    }


    private void ResetBranch()
    {

        StartBranch(defaultBranch);

    }


    private string GetAction()
    {

        System.Random rand = new System.Random();

        return branch.actions[rand.Next(branch.actions.Length)];

    }

    
    public void StartAction(Input input, Direction direction, Speed speed)
    {
        
        try
        {
            System.Random rand = new System.Random();

            if (!m_Action.IsReset())
                return;

            BranchData[] branches = branch.results[input][direction][speed];
            branch = branches[rand.Next(branches.Length)];
            m_Cooldown = branch.cooldown;

            m_Action.StartAction(GetAction());
        }
        catch (NullReferenceException e) { }

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
 