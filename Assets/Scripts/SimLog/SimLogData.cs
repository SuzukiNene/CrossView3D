using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParamDictionary
{
    private Dictionary<string, string> dict;

    public ParamDictionary()
    {
        dict = new Dictionary<string, string>();
    }

    public void Cleanup()
    {
        dict.Clear();
    }

    public bool AddParam(string name, string value)
    {
        if (!dict.ContainsKey(name))
        {
            try
            {
                dict.Add(name, value);
                return true;
            }
            catch (System.Exception e)
            {
                Debug.Log(e.Message);
            }
        }
        return false;
    }

    public string GetStrParam(string name, string defaults = "")
    {
        if (dict.ContainsKey(name))
        {
            return dict[name];
        }
        return defaults;
    }

    public int GetIntParam(string name, int defaults = 0)
    {
        if (dict.ContainsKey(name))
        {
            return System.Int32.Parse(dict[name]);
        }
        return defaults;
    }

    public float GetFloatParam(string name, float defaults = 0.0f)
    {
        if (dict.ContainsKey(name))
        {
            return (float)System.Double.Parse(dict[name]);
        }
        return defaults;
    }

    public bool GetBoolParam(string name, bool defaults = false)
    {
        if (dict.ContainsKey(name))
        {
            return System.Boolean.Parse(dict[name]);
        }
        return defaults;
    }
}

public class PlayerTypeDictionary
{
    private Dictionary<int, ParamDictionary> dict;

    public PlayerTypeDictionary()
    {
        dict = new Dictionary<int, ParamDictionary>();
    }

    public void Cleanup()
    {
        dict.Clear();
    }

    public bool AddPlayerType(int id, ParamDictionary type)
    {
        if (!dict.ContainsKey(id))
        {
            try
            {
                dict.Add(id, type);
                return true;
            }
            catch (System.Exception e)
            {
                Debug.Log(e.Message);
            }
        }
        return false;
    }

    public ParamDictionary GetPlayerType(int id)
    {
        if (dict.ContainsKey(id))
        {
            return dict[id];
        }
        return null;
    }
}

public class CycleObject
{
    public enum ObjectType { PlayMode, Team, Msg, Show, Ball, Player }
    public ObjectType ObjType;
    public int Time;

    public CycleObject(ObjectType type)
    {
        ObjType = type;
        Time = 0;
    }
}

public class PlayModeCycleObject : CycleObject
{
    public string PlayMode;

    public PlayModeCycleObject() : base(ObjectType.PlayMode)
    {
        PlayMode = string.Empty;
    }
}

public class TeamCycleObject : CycleObject
{
    public string LeftName;
    public int LeftScore;
    public string RightName;
    public int RightScore;

    public TeamCycleObject() : base(ObjectType.Team)
    {
        LeftName = string.Empty;
        LeftScore = 0;
        RightName = string.Empty;
        RightScore = 0;
    }
}

public class MsgCycleObject : CycleObject
{
    public int Board;
    public List<string> Message;

    public MsgCycleObject() : base(ObjectType.Msg)
    {
        Board = 0;
        Message = new List<string>();
    }
}

public class BallCycleObject : CycleObject
{
    public float X;
    public float Y;
    public float VelX;
    public float VelY;

    public BallCycleObject() : base(ObjectType.Ball)
    {
        X = 0f;
        Y = 0f;
        VelX = 0f;
        VelY = 0f;
    }
}

public class PlayerCycleObject : CycleObject
{
    public const int STATE_DISABLED = (0);              // 0x0000
    public const int STATE_STAND = (1);                 // 0x0001
    public const int STATE_KICK = (1 << 1);             // 0x0002
    public const int STATE_GOALIE = (1 << 3);           // 0x0008
    public const int STATE_CATCH = (1 << 4);            // 0x0010
    public const int STATE_TACKLE = (1 << 12);          // 0x1000

    public string Side;
    public int Unum;
    public int Type;
    public int State;
    public float X;
    public float Y;
    public float VelX;
    public float VelY;
    public float Body;
    public float Neck;
    public float PointToX;                              // Optional
    public float PointToY;                              // Optional
    public string ViewQuality;
    public float ViewWidth;
    public float FocusPointDist;                        // Optional
    public float FocusPointDir;                         // Optional
    public float Stamina;
    public float Effort;
    public float Recovery;
    public float Capacity;                              // Optional
    public string AttentionToSide;                      // Optional
    public int AttentionToUnum;                         // Optional
    public int KickCount;
    public int DashCount;
    public int TurnCount;
    public int CatchCount;
    public int MoveCount;
    public int TurnNeckCount;
    public int ChangeViewCount;
    public int SayCount;
    public int TackleCount;
    public int PointToCount;
    public int AttentionToCount;
    public int ChangeFocusCount;                        // Optional

    public PlayerCycleObject() : base(ObjectType.Player)
    {
        Side = string.Empty;
        Unum = 0;
        Type = 0;
        State = 0;
        X = 0f;
        Y = 0f;
        VelX = 0f;
        VelY = 0f;
        Body = 0f;
        Neck = 0f;
        PointToX = 0f;
        PointToY = 0f;
        ViewQuality = string.Empty;
        ViewWidth = 0f;
        FocusPointDist = 0f;
        FocusPointDir = 0f;
        Stamina = 0f;
        Effort = 0f;
        Recovery = 0f;
        Capacity = 0f;
        AttentionToSide = "\"\"";
        AttentionToUnum = 0;
        KickCount = 0;
        DashCount = 0;
        TurnCount = 0;
        CatchCount = 0;
        MoveCount = 0;
        TurnNeckCount = 0;
        ChangeViewCount = 0;
        SayCount = 0;
        TackleCount = 0;
        PointToCount = 0;
        AttentionToCount = 0;
        ChangeFocusCount = 0;
}
}

public class ShowCycleObject : CycleObject
{
    public BallCycleObject ball;
    public List<PlayerCycleObject> players;

    public ShowCycleObject() : base(ObjectType.Show)
    {
        ball = new BallCycleObject();
        players = new List<PlayerCycleObject>();
    }
}

public class CycleObjectDetails
{
    public List<PlayModeCycleObject> PlayModeCycles;
    public List<TeamCycleObject> TeamCycles;
    public List<MsgCycleObject> MsgCycles;
    public List<BallCycleObject> BallCycles;
    public Dictionary<string, Dictionary<int, List<PlayerCycleObject>>> PlayerCycles;

    public CycleObjectDetails()
    {
        PlayModeCycles = new List<PlayModeCycleObject>();
        TeamCycles = new List<TeamCycleObject>();
        MsgCycles = new List<MsgCycleObject>();
        BallCycles = new List<BallCycleObject>();
        PlayerCycles = new Dictionary<string, Dictionary<int, List<PlayerCycleObject>>>();
    }

    public void Cleanup()
    {
        PlayModeCycles.Clear();
        TeamCycles.Clear();
        MsgCycles.Clear();
        BallCycles.Clear();
        PlayerCycles.Clear();
    }
}

public class SimLogData
{
    public ParamDictionary ServerParams;
    public ParamDictionary PlayerParams;
    public PlayerTypeDictionary PlayerTypes;
    public List<CycleObject> CycleObjects;
    public CycleObjectDetails CycleDetails;

    public SimLogData()
    {
        ServerParams = new ParamDictionary();
        PlayerParams = new ParamDictionary();
        PlayerTypes = new PlayerTypeDictionary();
        CycleObjects = new List<CycleObject>();
        CycleDetails = new CycleObjectDetails();
    }

    public void Cleanup()
    {
        ServerParams.Cleanup();
        PlayerParams.Cleanup();
        PlayerTypes.Cleanup();
        CycleObjects.Clear();
        CycleDetails.Cleanup();
    }
}
