using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CycleAction
{
    public CycleObject cycleObj;

    public virtual void PreProcess(GameControl game, SimLogData data, float CycleStep = 0.0f) { }
    public virtual void Process(GameControl game, SimLogData data, float CycleStep = 0.0f) { }

    public CycleAction(CycleObject obj)
    {
        cycleObj = obj;
    }
}

public class PlayModeCycleAction : CycleAction
{
    public PlayModeCycleAction(CycleObject obj) : base(obj) { }

    public override void Process(GameControl game, SimLogData data, float CycleStep = 0.0f)
    {
        if (cycleObj is PlayModeCycleObject)
        {
            PlayModeCycleObject obj = cycleObj as PlayModeCycleObject;
            game.PlayMode.text = obj.PlayMode;
            game.UpdateScoreBoardBackcolor(obj.PlayMode);
        }
    }
}

public class TeamCycleAction : CycleAction
{
    public TeamCycleAction(CycleObject obj) : base(obj) { }

    public override void Process(GameControl game, SimLogData data, float CycleStep = 0.0f)
    {
        if (cycleObj is TeamCycleObject)
        {
            TeamCycleObject obj = cycleObj as TeamCycleObject;
            game.LeftTeam.text = obj.LeftName;
            game.LeftScore.text = obj.LeftScore.ToString();
            game.RightTeam.text = obj.RightName;
            game.RightScore.text = obj.RightScore.ToString();
        }
    }
}

public class MsgCycleAction : CycleAction
{
    public MsgCycleAction(CycleObject obj) : base(obj) { }
}

public class BallCycleAction : CycleAction
{
    public BallCycleAction(CycleObject obj) : base(obj) { }

    public override void PreProcess(GameControl game, SimLogData data, float CycleStep = 0.0f)
    {
        if (cycleObj is BallCycleObject)
        {
            BallCycleObject obj = cycleObj as BallCycleObject;

            float height = game.Ball.transform.position.y;
            Vector3 pos = new Vector3(obj.X, height, -(obj.Y));     // Z: Log Coordinate to Unity Coordinate

            if (game.Ball.transform.position != pos)
            {
                BallBehaviour ball = game.Ball.GetComponent<BallBehaviour>();
                ball.MoveTo(pos, CycleStep);
                ball.RollTo(pos);
            }
        }
    }
}

public class PlayerCycleAction : CycleAction
{
    public PlayerCycleAction(CycleObject obj) : base(obj) { }

    public override void PreProcess(GameControl game, SimLogData data, float CycleStep = 0.0f)
    {
        if (cycleObj is PlayerCycleObject)
        {
            PlayerCycleObject obj = cycleObj as PlayerCycleObject;

            GameObject gameObj = game.PlayerObjects[obj.Side][obj.Unum];
            FieldPlayerBehaviour player = gameObj.GetComponent<FieldPlayerBehaviour>();

            float height = gameObj.transform.position.y;
            Vector3 pos = new Vector3(obj.X, height, -(obj.Y));     // Z: Log Coordinate to Unity Coordinate

            float body = obj.Body + 90f;                            // Body: Log Direction to Unity Direction
            body = (body > 180) ? (body - 360) : (body < -180) ? (body + 360) : body;

            float range_def = gameObj.GetComponentInChildren<ViewAngleBehaviour>().viewRange;
            float range = data.PlayerTypes.GetPlayerType(obj.Type).GetFloatParam("unum_far_length", range_def);

            // Move, Turn, TurnNeck, ChangeAngle
            player.MoveTo(pos, CycleStep);
            player.TurnBodyNeckViewAngleTo(body, obj.Neck, obj.ViewWidth, range, CycleStep);

            // Animation
            if (gameObj.transform.position != pos)
            {
                if (CycleStep > 0f) player.Dash();
            }
            else
            {
                if (CycleStep > 0f) player.Idle();
            }
            if ((obj.State & PlayerCycleObject.STATE_KICK) != 0)
            {
                if (CycleStep > 0f) player.Kick();
            }
            else if ((obj.State & PlayerCycleObject.STATE_TACKLE) != 0)
            {
                if (CycleStep > 0f) player.Tackle();
            }
            else if ((obj.State & PlayerCycleObject.STATE_CATCH) != 0)
            {
                if (CycleStep > 0f) player.Catch();
            }
        }
    }

    public override void Process(GameControl game, SimLogData data, float CycleStep = 0.0f)
    {
        if (cycleObj is PlayerCycleObject)
        {
            PlayerCycleObject obj = cycleObj as PlayerCycleObject;

            GameObject gameObj = game.PlayerObjects[obj.Side][obj.Unum];
            FieldPlayerBehaviour player = gameObj.GetComponent<FieldPlayerBehaviour>();

            // Update Information
            if (game.selectedPlayer.Equals(obj.Side, obj.Unum))
            {
                PersonalInformationBehaviour info = game.PlayerInfo.GetComponent<PersonalInformationBehaviour>();
                info.UpdatePlayerInformation(obj);
            }

            // Last Play has done
            int last = data.CycleDetails.PlayerCycles[obj.Side][obj.Unum].Count - 1;
            if (obj.Time == data.CycleDetails.PlayerCycles[obj.Side][obj.Unum][last].Time)
            {
                player.Idle();
            }
        }
    }
}

public class CycleActionFactory
{
    public static CycleAction Create(CycleObject obj)
    {
        try
        {
            switch (obj.ObjType)
            {
                case CycleObject.ObjectType.PlayMode:
                    return new PlayModeCycleAction(obj);
                case CycleObject.ObjectType.Team:
                    return new TeamCycleAction(obj);
                case CycleObject.ObjectType.Msg:
                    return new MsgCycleAction(obj);
                case CycleObject.ObjectType.Ball:
                    return new BallCycleAction(obj);
                case CycleObject.ObjectType.Player:
                    return new PlayerCycleAction(obj);
                case CycleObject.ObjectType.Show:
                    Debug.Log("Not Supported, ObjectType.Show for CycleAction.");
                    break;
                default:
                    Debug.Log("Invalid ObjectType of CycleObject.");
                    break;
            }
        }
        catch (System.Exception e)
        {
            Debug.Log(e.Message);
        }
        return null;
    }
}

public class SimLogCycle
{
    public int Time;
    public List<CycleAction> Actions;

    public SimLogCycle(int time)
    {
        Time = time;
        Actions = new List<CycleAction>();
    }
}

public class SimLogTimeline
{
    public int Index;
    public List<SimLogCycle> CycleList;

    public SimLogTimeline()
    {
        Index = 0;
        CycleList = new List<SimLogCycle>();
    }

    public bool Add(CycleObject cycle)
    {
        SimLogCycle last = null;

        if (CycleList.Count > 0)
        {
            last = CycleList[CycleList.Count - 1];

            if (last.Time == cycle.Time)
            {
                // Check if exists same type object in SimLogCycle already.
                CycleObject.ObjectType type = cycle.ObjType;
                if (type == CycleObject.ObjectType.Show)
                {
                    type = CycleObject.ObjectType.Ball;
                }
                foreach (CycleAction action in last.Actions)
                {
                    if (action.cycleObj.ObjType == type)
                    {
                        last = null;
                        break;
                    }
                }
            }
            else
            {
                last = null;
            }
        }

        try
        {
            if (last == null)
            {
                // Add new item in CycleList.
                last = new SimLogCycle(cycle.Time);
                if (last == null)
                {
                    return false;
                }
                CycleList.Add(last);
            }

            List<CycleObject> cycles = new List<CycleObject>();
            if (cycles == null)
            {
                return false;
            }

            if (cycle is ShowCycleObject)
            {
                ShowCycleObject show = cycle as ShowCycleObject;

                // Ball
                cycles.Add(show.ball);

                // Player
                foreach (PlayerCycleObject player in show.players)
                {
                    cycles.Add(player);
                }
            }
            else
            {
                cycles.Add(cycle);
            }
            foreach(CycleObject item in cycles)
            {
                CycleAction action = CycleActionFactory.Create(item);
                if (action == null)
                {
                    return false;
                }
                last.Actions.Add(action);
            }
        }
        catch (System.Exception e)
        {
            Debug.Log(e.Message);
            return false;
        }
        return true;
    }

    public void Cleanup()
    {
        Index = 0;
        CycleList.Clear();
    }
}
