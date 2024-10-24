using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using System.IO;
using System.Text;

public class SimLogReader : MonoBehaviour
{
    public SimLogData simLogData = new SimLogData();

    public enum Status { Idle = 0, Started = 1, Success = 2, IOError = -1, ParseError = -2 }
    public delegate void ReadResultEvent(SimLogReader reader, Status status);

    public class SimLogDownloadHandler : DownloadHandlerScript
    {
        public Status status;

        private byte[] buffer;
        private SimLogReader simLogReader;
        private event ReadResultEvent ReadResult;
        private Encoding encoding;

        public SimLogDownloadHandler(SimLogReader reader, ReadResultEvent callback, Encoding enc) : base()
        {
            status = Status.Idle;
            buffer = new byte[0];
            simLogReader = reader;
            ReadResult += callback;
            encoding = enc;
        }

        protected override bool ReceiveData(byte[] data, int dataLength)
        {
            // Returning false will stop receiving data.
            // then CompleteContent() callback not called,
            // and UnityWebRequest.Result.ConnectionError occured.

            if (AppendData(ref data, dataLength) == false)
            {
                ReadResult(simLogReader, status = Status.IOError);
                return false;
            }
            if (ParseData() == false)
            {
                ReadResult(simLogReader, status = Status.ParseError);
                return false;
            }
            return true;
        }

        protected override void CompleteContent()
        {
            status = Status.Success;

            // if there is remaining buffer data to have to be parsed...
            if (buffer.Length > 0)
            {
                status = Status.ParseError;
            }
            // event callback
            ReadResult(simLogReader, status);
        }

        private bool AppendData(ref byte[] data, int dataLength)
        {
            int len = buffer.Length;
            int size = len + dataLength;

            try
            {
                System.Array.Resize<byte>(ref buffer, size);
                System.Array.Copy(data, 0, buffer, len, dataLength);
            }
            catch (System.Exception e)
            {
                Debug.Log(e.Message);
                return false;
            }
            return true;
        }

        private bool PurgeData(int size)
        {
            int len = buffer.Length - size;

            try
            {
                System.Array.Copy(buffer, size, buffer, 0, len);
                System.Array.Resize<byte>(ref buffer, len);
            }
            catch (System.Exception e)
            {
                Debug.Log(e.Message);
                return false;
            }
            return true;
        }

        private bool ParseData()
        {
            using (var stream = new MemoryStream(buffer, false))
            {
                int size = buffer.Length;
                var reader = new StreamReader(stream, encoding);

                while (!reader.EndOfStream)
                {
                    string line = reader.ReadLine();

                    if (status == Status.Idle)
                    {
                        if (simLogReader.CheckVersion(line))
                        {
                            status = Status.Started;
                            continue;
                        }
                        else
                        {
                            // Version error.
                            return false;
                        }
                    }
                    if (simLogReader.ParseLine(line) == false)
                    {
                        if (reader.EndOfStream)
                        {
                            // continue receiving data...
                            size = buffer.Length - line.Length;
                            break;
                        }
                        else
                        {
                            // parse error by data format.
                            return false;
                        }
                    }
                }
                PurgeData(size);
            }
            return true;
        }
    }

    public void Read(string uri, Encoding encoding, ReadResultEvent callback)
    {
        simLogData.Cleanup();

        StartCoroutine(ReadFile(uri, encoding, callback));
    }

    IEnumerator ReadFile(string uri, Encoding encoding, ReadResultEvent callback)
    {
        UnityWebRequest req = new UnityWebRequest(uri);
        SimLogDownloadHandler handler = new SimLogDownloadHandler(this, callback, encoding);

        req.downloadHandler = handler ;
        yield return req.SendWebRequest();

        if (req.result != UnityWebRequest.Result.Success)
        {
            if (handler.status >= Status.Idle)
            {
                handler.status = Status.IOError;
                callback(this, handler.status);
            }
        }
    }

    public bool CheckVersion(string line)
    {
        return SimLogParser.CheckVersion(line);
    }

    public bool ParseLine(string line)
    {
        bool ret = false;

        if (string.IsNullOrEmpty(line))
        {
            // buffer starts with CR|LF|CRLF.
            return true;
        }
        try
        {
            switch (SimLogParser.GetElementName(line))
            {
                case "server_param":
                    ret = Parse_ServerParam(line);
                    break;
                case "player_param":
                    ret = Parse_PlayerParam(line);
                    break;
                case "player_type":
                    ret = Parse_PlayerType(line);
                    break;
                case "playmode":
                    ret = Parse_PlayMode(line);
                    break;
                case "team":
                    ret = Parse_Team(line);
                    break;
                case "msg":
                    ret = Parse_Msg(line);
                    break;
                case "show":
                    ret = Parse_Show(line);
                    break;
                default:
                    break;
            }
        }
        catch (System.Exception e)
        {
            Debug.Log(line);
            Debug.Log(e.Message);
        }
        return ret;
    }

    private bool Parse_ServerParam(string line)
    {
        simLogData.ServerParams = SimLogParser.Parse_ServerParam(line);

        return (simLogData.ServerParams != null);
    }

    private bool Parse_PlayerParam(string line)
    {
        simLogData.PlayerParams = SimLogParser.Parse_PlayerParams(line);

        return (simLogData.PlayerParams != null);
    }

    private bool Parse_PlayerType(string line)
    {
        ParamDictionary type = SimLogParser.Parse_PlayerType(line);

        if (type == null)
        {
            return false;
        }
        int id = type.GetIntParam("id", -1);
        if (id < 0)
        {
            return false;
        }
        if (simLogData.PlayerTypes.AddPlayerType(id, type) == false)
        {
            return false;
        }
        return true;
    }

    private bool Parse_PlayMode(string line)
    {
        PlayModeCycleObject obj = SimLogParser.Parse_PlayMode(line);

        if (obj == null)
        {
            return false;
        }
        try
        {
            simLogData.CycleObjects.Add(obj);
            simLogData.CycleDetails.PlayModeCycles.Add(obj);
        }
        catch (System.Exception e)
        {
            Debug.Log(e.Message);
            return false;
        }
        return true;
    }

    private bool Parse_Team(string line)
    {
        TeamCycleObject obj = SimLogParser.Parse_Team(line);

        if (obj == null)
        {
            return false;
        }
        try
        {
            simLogData.CycleObjects.Add(obj);
            simLogData.CycleDetails.TeamCycles.Add(obj);
        }
        catch (System.Exception e)
        {
            Debug.Log(e.Message);
            return false;
        }
        return true;
    }

    private bool Parse_Msg(string line)
    {
        MsgCycleObject obj = SimLogParser.Parse_Msg(line);

        if (obj == null)
        {
            return false;
        }
        try
        {
            simLogData.CycleObjects.Add(obj);
            simLogData.CycleDetails.MsgCycles.Add(obj);
        }
        catch (System.Exception e)
        {
            Debug.Log(e.Message);
            return false;
        }
        return true;
    }

    private bool Parse_Show(string line)
    {
        ShowCycleObject obj = SimLogParser.Parse_Show(line);

        if (obj == null)
        {
            return false;
        }
        try
        {
            simLogData.CycleObjects.Add(obj);

            // Ball
            simLogData.CycleDetails.BallCycles.Add(obj.ball);

            // Player
            foreach (PlayerCycleObject player in obj.players)
            {
                string side = player.Side;
                int unum = player.Unum;

                if (!simLogData.CycleDetails.PlayerCycles.ContainsKey(side))
                {
                    simLogData.CycleDetails.PlayerCycles.Add(side, new Dictionary<int, List<PlayerCycleObject>>());
                }
                if (!simLogData.CycleDetails.PlayerCycles[side].ContainsKey(unum))
                {
                    simLogData.CycleDetails.PlayerCycles[side].Add(unum, new List<PlayerCycleObject>());
                }
                simLogData.CycleDetails.PlayerCycles[side][unum].Add(player);
            }
        }
        catch (System.Exception e)
        {
            Debug.Log(e.Message);
            return false;
        }
        return true;
    }
}
