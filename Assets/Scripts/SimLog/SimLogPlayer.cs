using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimLogPlayer : MonoBehaviour
{
    public float Speed;

    public enum Status { Idle, Ready, Playing, Pause, Stopped }

    public delegate void NotifyStatusChanged(Status status);
    public event NotifyStatusChanged StatusChangedEvent = delegate { };

    public delegate void NotifyCycleChanged(int cycle);
    public event NotifyCycleChanged CycleChangedEvent = delegate { };

    private Status status;
    private SimLogTimeline timeline;
    private GameControl gameControl;
    private SimLogData logData;
    private float cycleStep;
    private bool stopTimer;

    public SimLogPlayer()
    {
        Speed = 1f;
        status = Status.Idle;
        timeline = new SimLogTimeline();
        gameControl = null;
        logData = null;
        cycleStep = 0f;
        stopTimer = false;
    }

    public bool Setup(GameControl game, SimLogData data)
    {
        if (status == Status.Playing)
        {
            return false;
        }

        gameControl = game;
        logData = data;

        // Regist Event handler
        CycleChangedEvent += gameControl.OnUpdateCycle;
        StatusChangedEvent += gameControl.OnUpdateStatus;
        
        // Cycle Step
        cycleStep = data.ServerParams.GetFloatParam("simulator_step", 100f) / 1000;   // [msec] -> [sec]

        // Timeline
        timeline.Cleanup();
        foreach (CycleObject obj in data.CycleObjects)
        {
            if (timeline.Add(obj) == false)
            {
                Debug.Log("Failed to add Cycle objects into Timeline.");
                return false;
            }
        }
        // Timeline - increment Index until (Cycle == 1).
        timeline.Index = Seek(1, true);
        if (timeline.Index == -1)
        {
            return false;
        }

        // Event(Cycle)
        CycleChangedEvent(1);
        // Event(Status)
        StatusChangedEvent(status = Status.Ready);

        return true;
    }

    public bool Play()
    {
        if (status == Status.Idle || status == Status.Playing)
        {
            return false;
        }
        if (status == Status.Stopped)
        {
            timeline.Index = Seek(1);
            CycleChangedEvent(1);
        }

        // PreProcess()
        foreach (CycleAction action in timeline.CycleList[timeline.Index].Actions)
        {
            action.PreProcess(gameControl, logData, GetInterval());
        }

        // Event(Status)
        StatusChangedEvent(status = Status.Playing);
        // Start timer
        InvokeRepeating(nameof(DoProcess), GetInterval(), GetInterval());

        return true;
    }

    private float GetInterval()
    {
        return cycleStep / Speed;
    }

    private void DoProcess()
    {
        if (status != Status.Playing)
        {
            return;
        }

        // Process()
        foreach (CycleAction action in timeline.CycleList[timeline.Index].Actions)
        {
            action.Process(gameControl, logData, GetInterval());
        }

        // Event(Cycle)
        CycleChangedEvent(timeline.CycleList[timeline.Index].Time);

        // Play ends
        if (timeline.Index == timeline.CycleList.Count - 1)
        {
            // Stop timer
            CancelInvoke();
            // Event(Status)
            StatusChangedEvent(status = Status.Stopped);
        }
        else if (stopTimer)
        {
            // Stop timer
            stopTimer = false;
            CancelInvoke();
            // Event(Status)
            StatusChangedEvent(status = Status.Pause);
        }
        else
        {
            // Increment Index for next DoProcess().
            timeline.Index++;
            foreach (CycleAction action in timeline.CycleList[timeline.Index].Actions)
            {
                action.PreProcess(gameControl, logData, GetInterval());
            }
        }
        return;
    }

    public bool Pause()
    {
        if (status != Status.Playing)
        {
            return false;
        }
        stopTimer = true;

        return true;
    }

    public bool Next()
    {
        if (status != Status.Pause)
        {
            return false;
        }

        // Next cycle
        int next = timeline.CycleList[timeline.Index].Time + 1;
        if (next > timeline.CycleList[timeline.CycleList.Count - 1].Time)
        {
            return false;
        }
        int index = GetIndexOfCycle(next, true);
        if (index == -1)
        {
            return false;
        }
        timeline.Index = index;

        // Step to Next cycle action
        RestoreLastCycleAction(CycleObject.ObjectType.PlayMode);
        RestoreLastCycleAction(CycleObject.ObjectType.Team);
        RestoreLastCycleAction(CycleObject.ObjectType.Ball);
        RestoreLastCycleAction(CycleObject.ObjectType.Player);

        // Event(Cycle)
        CycleChangedEvent(timeline.CycleList[timeline.Index].Time);

        if (timeline.Index == timeline.CycleList.Count - 1)
        {
            // Event(Status)
            StatusChangedEvent(status = Status.Stopped);
        }

        return true;
    }

    public bool Prev()
    {
        if (status != Status.Pause)
        {
            return false;
        }

        // Prev cycle
        int prev = timeline.CycleList[timeline.Index].Time - 1;
        if (prev < 1)
        {
            return false;
        }
        int index = GetIndexOfCycle(prev, false);
        if (index == -1)
        {
            return false;
        }
        timeline.Index = index;

        // Back to Prev cycle action
        RestoreLastCycleAction(CycleObject.ObjectType.PlayMode);
        RestoreLastCycleAction(CycleObject.ObjectType.Team);
        RestoreLastCycleAction(CycleObject.ObjectType.Ball);
        RestoreLastCycleAction(CycleObject.ObjectType.Player);

        // Event(Cycle)
        CycleChangedEvent(timeline.CycleList[timeline.Index].Time);

        return true;
    }

    public bool Forward()
    {
        if (status != Status.Pause)
        {
            return false;
        }

        // Next event
        int cycle = timeline.CycleList[timeline.Index].Time;
        int index = GetIndexOfNextEvent(timeline.Index);
        if (index == -1)
        {
            // Do Nothing...
            return true;
        }
        timeline.Index = index;

        // Step to Next cycle action
        RestoreLastCycleAction(CycleObject.ObjectType.PlayMode);
        RestoreLastCycleAction(CycleObject.ObjectType.Team);
        RestoreLastCycleAction(CycleObject.ObjectType.Ball);
        RestoreLastCycleAction(CycleObject.ObjectType.Player);

        // Event(Cycle)
        CycleChangedEvent(timeline.CycleList[timeline.Index].Time);

        if (timeline.Index == timeline.CycleList.Count - 1)
        {
            // Event(Status)
            StatusChangedEvent(status = Status.Stopped);
        }

        return true;
    }

    public bool Backword()
    {
        if (status != Status.Pause)
        {
            return false;
        }

        // Prev event
        int cycle = timeline.CycleList[timeline.Index].Time;
        int index = GetIndexOfPrevEvent(timeline.Index);
        if (index == -1)
        {
            // Do Nothing...
            return true;
        }
        timeline.Index = index;

        // Back to Prev cycle action
        RestoreLastCycleAction(CycleObject.ObjectType.PlayMode);
        RestoreLastCycleAction(CycleObject.ObjectType.Team);
        RestoreLastCycleAction(CycleObject.ObjectType.Ball);
        RestoreLastCycleAction(CycleObject.ObjectType.Player);

        // Event(Cycle)
        CycleChangedEvent(timeline.CycleList[timeline.Index].Time);

        return true;
    }

    public bool Jump(int cycle)
    {
        if (status != Status.Pause)
        {
            return false;
        }
        if (cycle < 1)
        {
            return false;
        }

        // Specified cycle
        int index = GetIndexOfCycle(cycle, true);
        if (index == -1)
        {
            return false;
        }
        timeline.Index = index;

        // Back to Prev cycle action
        RestoreLastCycleAction(CycleObject.ObjectType.PlayMode);
        RestoreLastCycleAction(CycleObject.ObjectType.Team);
        RestoreLastCycleAction(CycleObject.ObjectType.Ball);
        RestoreLastCycleAction(CycleObject.ObjectType.Player);

        // Event(Cycle)
        CycleChangedEvent(timeline.CycleList[timeline.Index].Time);

        if (timeline.Index == timeline.CycleList.Count - 1)
        {
            // Event(Status)
            StatusChangedEvent(status = Status.Stopped);
        }

        return true;
    }

    private int Seek(int cycle, bool exec = false)
    {
        int index = 0;

        if (GetIndexOfCycle(cycle) == -1)
        {
            return -1;
        }
        while (timeline.CycleList[index].Time < cycle)
        {
            if (exec)
            {
                foreach (CycleAction action in timeline.CycleList[index].Actions)
                {
                    action.PreProcess(gameControl, logData);
                    action.Process(gameControl, logData);
                }
            }
            index++;
        }
        return index;
    }

    private int GetIndexOfCycle(int cycle)
    {
        for (int index = 0; index < timeline.CycleList.Count; index++)
        {
            if (timeline.CycleList[index].Time == cycle)
            {
                return index;
            }
        }
        return -1;
    }

    private int GetIndexOfCycle(int cycle, bool forward)
    {
        for (int index = 0; index < timeline.CycleList.Count; index++)
        {
            if (timeline.CycleList[index].Time == cycle)
            {
                return index;
            }
            else if (timeline.CycleList[index].Time > cycle)
            {
                // Return nearest index
                if (forward)
                {
                    return index;
                }
                else if (index > 0)
                {
                    return index - 1;
                }
                break;
            }
        }
        return -1;
    }

    private int GetIndexOfNextEvent(int current)
    {
        // Search Next PlayMode event
        for (int next = (current + 1); next < timeline.CycleList.Count; next++)
        {
            for (int index = 0; index < timeline.CycleList[next].Actions.Count; index++)
            {
                if (timeline.CycleList[next].Actions[index] is PlayModeCycleAction)
                {
                    return next;
                }
            }
        }
        return (timeline.CycleList.Count - 1);
    }

    private int GetIndexOfPrevEvent(int current)
    {
        int head = GetIndexOfCycle(1);

        // Search Prev|Current PlayMode event
        for (int prev = (current - 1); prev >= head; prev--)
        {
            for (int index = 0; index < timeline.CycleList[prev].Actions.Count; index++)
            {
                if (timeline.CycleList[prev].Actions[index] is PlayModeCycleAction)
                {
                    return prev;
                }
            }
        }
        return head;
    }

    private void RestoreLastCycleAction(CycleObject.ObjectType type)
    {
        int index = timeline.Index;
        int head = GetIndexOfCycle(1);

        while(index >= head)
        {
            bool found = false;

            foreach (CycleAction action in timeline.CycleList[index].Actions)
            {
                if (action.cycleObj.ObjType == type)
                {
                    action.PreProcess(gameControl, logData);
                    action.Process(gameControl, logData);
                    found = true;
                }
            }
            if (found)
            {
                break;
            }
            index--;
        }
        return;
    }
}
