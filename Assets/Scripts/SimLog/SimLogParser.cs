using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text.RegularExpressions;

public class SimLogParser
{
    private const string MAGIC = "ULG";
    private const int MIN_VERSION = 4;
    private const int MAX_VERSION = 6;

    public static bool CheckVersion(string line)
    {
        if (line.StartsWith(MAGIC))
        {
            int version = System.Int32.Parse(line.Replace(MAGIC, ""));
            if (version >= MIN_VERSION && version <= MAX_VERSION)
            {
                return true;
            }
        }
        Debug.Log("Version Error");
        return false;
    }

    public static string GetElementName(string line)
    {
        string element = string.Empty;

        if (line.StartsWith("(") && line.EndsWith(")"))
        {
            int pos = line.IndexOf(" ");
            if (pos >= 0)
            {
                element = line.Substring(1, pos - 1);
            }
        }
        return element;
    }

    private static ParamDictionary Parse_ParamDictionary(string line, string param_name)
    {
        string header = "(" + param_name + " ";
        string footer = ")";

        if (line.StartsWith(header) && line.EndsWith(footer))
        {
            try
            {
                ParamDictionary dict = new ParamDictionary();

                line = line.Substring(header.Length, (line.Length - header.Length - footer.Length));

                string pattern = @"\(.+? .+?\)";
                MatchCollection matches = Regex.Matches(line, pattern);
                if (matches.Count > 0)
                {
                    foreach (Match match in matches)
                    {
                        line = line.Replace(match.Value, "");

                        string param = match.Value.Substring(1, match.Value.Length - 2);
                        string[] result = param.Split(' ');
                        string key = result[0];
                        string value = result[1];

                        if (value.StartsWith("\"") && value.EndsWith("\""))
                        {
                            value = value.Substring(1, value.Length - 2);
                        }
                        if (!dict.AddParam(key, value))
                        {
                            Debug.LogFormat("Failed to add param({0}, {1}) into Dictionary.", key, value);
                            dict = null;
                            break;
                        }
                    }
                }
                line = line.Replace(" ", "");
                if (line != "")
                {
                    return null;
                }
                return dict;
            }
            catch (System.Exception e)
            {
                Debug.Log(e.Message);
            }
        }
        return null;
    }

    public static ParamDictionary Parse_ServerParam(string line)
    {
        return Parse_ParamDictionary(line, "server_param");
    }

    public static ParamDictionary Parse_PlayerParams(string line)
    {
        return Parse_ParamDictionary(line, "player_param");
    }

    public static ParamDictionary Parse_PlayerType(string line)
    {
        return Parse_ParamDictionary(line, "player_type");
    }

    public static PlayModeCycleObject Parse_PlayMode(string line)
    {
        string header = "(playmode ";
        string footer = ")";

        if (line.StartsWith(header) && line.EndsWith(footer))
        {
            try
            {
                PlayModeCycleObject obj = new PlayModeCycleObject();

                line = line.Substring(header.Length, (line.Length - header.Length - footer.Length));

                string[] result = line.Split(' ');
                string time = result[0];
                string mode = result[1];

                obj.Time = System.Int32.Parse(time);
                obj.PlayMode = mode;

                return obj;
            }
            catch (System.Exception e)
            {
                Debug.Log(e.Message);
            }
        }
        Debug.Log("Invalid string of PlayMode.");
        return null;
    }

    public static TeamCycleObject Parse_Team(string line)
    {
        string header = "(team ";
        string footer = ")";

        if (line.StartsWith(header) && line.EndsWith(footer))
        {
            try
            {
                TeamCycleObject obj = new TeamCycleObject();

                line = line.Substring(header.Length, (line.Length - header.Length - footer.Length));

                string[] result = line.Split(' ');
                string time = result[0];
                string team_name_l = result[1];
                string team_name_r = result[2];
                string team_score_l = result[3];
                string team_score_r = result[4];

                obj.Time = System.Int32.Parse(time);
                obj.LeftName = team_name_l;
                obj.LeftScore = System.Int32.Parse(team_score_l);
                obj.RightName = team_name_r;
                obj.RightScore = System.Int32.Parse(team_score_r);

                return obj;
            }
            catch (System.Exception e)
            {
                Debug.Log(e.Message);
            }
        }
        Debug.Log("Invalid string of Team.");
        return null;
    }

    public static MsgCycleObject Parse_Msg(string line)
    {
        string header = "(msg ";
        string footer = ")";

        if (line.StartsWith(header) && line.EndsWith(footer))
        {
            try
            {
                MsgCycleObject obj = new MsgCycleObject();

                line = line.Substring(header.Length, (line.Length - header.Length - footer.Length));

                // Time, Board
                string prefix = @"^\d+ \d+ ";
                var m = Regex.Match(line, prefix);
                if (m.Success)
                {
                    line = line.Substring(m.Value.Length);

                    string param = m.Value.Substring(0, m.Value.Length - 1);
                    string[] result = param.Split(' ');
                    string time = result[0];
                    string board = result[1];

                    obj.Time = System.Int32.Parse(time);
                    obj.Board = System.Int32.Parse(board);

                    // Message+
                    string pattern;
                    MatchCollection matches;
                    if (line.StartsWith("\"("))
                    {
#if (false)
                        pattern = @"""\(.+?\)""";
                        matches = Regex.Matches(line, pattern);
                        foreach (Match match in matches)
                        {
                            line = line.Replace(match.Value, "");

                            obj.Message.Add(match.Value);
                        }
#else
                        if (line.EndsWith(")\""))
                        {
                            obj.Message.Add(line);
                            line = "";
                        }
#endif
                    }
                    else if (line.StartsWith("\""))
                    {
                        pattern = @""".+?""";
                        matches = Regex.Matches(line, pattern);
                        foreach (Match match in matches)
                        {
                            line = line.Replace(match.Value, "");

                            obj.Message.Add(match.Value);
                        }
                    }
                    line = line.Replace(" ", "");
                    if (line != "")
                    {
                        return null;
                    }
                    return obj;
                }
            }
            catch (System.Exception e)
            {
                Debug.Log(e.Message);
            }
        }
        Debug.Log("Invalid string of Msg.");
        return null;
    }

    public static ShowCycleObject Parse_Show(string line)
    {
        string header = "(show ";
        string footer = ")";

        if (line.StartsWith(header) && line.EndsWith(footer))
        {
            try
            {
                ShowCycleObject obj = new ShowCycleObject();

                line = line.Substring(header.Length, (line.Length - header.Length - footer.Length));

                // Time
                string prefix = @"^\d+ ";
                var m = Regex.Match(line, prefix);
                if (m.Success)
                {
                    line = line.Substring(m.Value.Length);

                    string time = m.Value.Substring(0, m.Value.Length - 1);
                    obj.Time = System.Int32.Parse(time);

                    // Ball(b)
                    string pattern_b = @"^\(\(b\) [\+\-0-9\.eE]+ [\+\-0-9\.eE]+ [\+\-0-9\.eE]+ [\+\-0-9\.eE]+\)";
                    var n = Regex.Match(line, pattern_b);
                    if (n.Success)
                    {
                        line = line.Substring(n.Value.Length);

                        string prefix_b = "((b) ";
                        string suffix_b = ")";

                        string param = n.Value.Substring(prefix_b.Length, (n.Value.Length - prefix_b.Length - suffix_b.Length));
                        string[] result = param.Split(' ');
                        string x = result[0];
                        string y = result[1];
                        string vel_x = result[2];
                        string vel_y = result[3];

                        obj.ball.Time = obj.Time;
                        obj.ball.X = (float)System.Double.Parse(x);
                        obj.ball.Y = (float)System.Double.Parse(y);
                        obj.ball.VelX = (float)System.Double.Parse(vel_x);
                        obj.ball.VelY = (float)System.Double.Parse(vel_y);
                    }
                    // Player(l|r)
                    string pattern_p = @"\(\((l|r) \d+\) \d+ (0|0x[0-9a-zA-Z]+) [\+\-0-9\.eE]+ [\+\-0-9\.eE]+ [\+\-0-9\.eE]+ [\+\-0-9\.eE]+ [\+\-0-9\.eE]+ [\+\-0-9\.eE]+( [\+\-0-9\.eE]+ [\+\-0-9\.eE]+)? \(v . \d+\)( \(fp [\+\-0-9\.eE]+ [\+\-0-9\.eE]+\))? \(s [\+\-0-9\.eE]+ [\+\-0-9\.eE]+ [\+\-0-9\.eE]+( [\+\-0-9\.eE]+)?\)( \(f (l|r) \d+\))? \(c \d+ \d+ \d+ \d+ \d+ \d+ \d+ \d+ \d+ \d+ \d+( \d+)?\)\)";
                    MatchCollection matches = Regex.Matches(line, pattern_p);
                    foreach (Match match in matches)
                    {
                        line = line.Replace(match.Value, "");
                        
                        PlayerCycleObject p = new PlayerCycleObject();

                        string side, unum;
                        string type, state, x, y, vel_x, vel_y, body, neck, point_x = "", point_y = "";
                        string v_quality, v_width;
                        string fp_dist = "", fp_dir = "";
                        string stamina, effort, recovery, capacity = "";
                        string f_side = "", f_unum = "";
                        string c_kick, c_dash, c_turn, c_catch, c_move, c_turnneck, c_changeview, c_say, c_tackle, c_pointto, c_attentionto, c_changefocus = "";

                        string value = match.Value;
                        int startIndex = 0, endIndex = 0;

                        // Side, Unum
                        {
                            string prefix_p = "((";
                            string suffix_p = ") ";
                            startIndex = value.IndexOf(prefix_p, startIndex) + prefix_p.Length;
                            endIndex = value.IndexOf(suffix_p, startIndex);

                            string param = value.Substring(startIndex, endIndex - startIndex);
                            string[] result = param.Split(' ');
                            side = result[0];
                            unum = result[1];

                            startIndex = endIndex + suffix_p.Length;
                        }
                        // Type, State, X, Y, VelX, VelY, Body, Neck, [PointX, PointY]
                        {
                            string next_prefix = "(v ";
                            endIndex = value.IndexOf(next_prefix);

                            string param = value.Substring(startIndex, endIndex - startIndex);
                            string[] result = param.Split(' ');
                            type = result[0];
                            state = result[1];
                            x = result[2];
                            y = result[3];
                            vel_x = result[4];
                            vel_y = result[5];
                            body = result[6];
                            neck = result[7];
                            if (result.Length > 9)
                            {
                                point_x = result[8];
                                point_y = result[9];
                            }

                            startIndex = endIndex;
                        }
                        // ViewQuality, ViewWidth
                        {
                            string prefix_p = "(v ";
                            string suffix_p = ") ";
                            startIndex = value.IndexOf(prefix_p, startIndex) + prefix_p.Length;
                            endIndex = value.IndexOf(suffix_p, startIndex);

                            string param = value.Substring(startIndex, endIndex - startIndex);
                            string[] result = param.Split(' ');
                            v_quality = result[0];
                            v_width = result[1];

                            startIndex = endIndex + suffix_p.Length;
                        }
                        // [Dist, Dir]
                        if (value.IndexOf("(fp ", startIndex) != -1)
                        {
                            string prefix_p = "(fp ";
                            string suffix_p = ") ";
                            startIndex = value.IndexOf(prefix_p, startIndex) + prefix_p.Length;
                            endIndex = value.IndexOf(suffix_p, startIndex);

                            string param = value.Substring(startIndex, endIndex - startIndex);
                            string[] result = param.Split(' ');
                            fp_dist = result[0];
                            fp_dir = result[1];

                            startIndex = endIndex + suffix_p.Length;
                        }
                        // Stamina Effort Recovery [Capacity]
                        {
                            string prefix_p = "(s ";
                            string suffix_p = ") ";
                            startIndex = value.IndexOf(prefix_p, startIndex) + prefix_p.Length;
                            endIndex = value.IndexOf(suffix_p, startIndex);

                            string param = value.Substring(startIndex, endIndex - startIndex);
                            string[] result = param.Split(' ');
                            stamina = result[0];
                            effort = result[1];
                            recovery = result[2];
                            if (result.Length > 3)
                            {
                                capacity = result[3];
                            }

                            startIndex = endIndex + suffix_p.Length;
                        }
                        // [FocusSide, FocusUnum]
                        if (value.IndexOf("(f ", startIndex) != -1)
                        {
                            string prefix_p = "(f ";
                            string suffix_p = ") ";
                            startIndex = value.IndexOf(prefix_p, startIndex) + prefix_p.Length;
                            endIndex = value.IndexOf(suffix_p, startIndex);

                            string param = value.Substring(startIndex, endIndex - startIndex);
                            string[] result = param.Split(' ');
                            f_side = result[0];
                            f_unum = result[1];

                            startIndex = endIndex + suffix_p.Length;
                        }
                        // KickCount, DashCount, TurnCount, CatchCount, MoveCount, TurnNeckCount, ChangeViewCount, SayCount, TackleCount, PointtoCount, AttentiontoCount, [ChangeFocusCount]
                        {
                            string prefix_p = "(c ";
                            string suffix_p = "))";
                            startIndex = value.IndexOf(prefix_p, startIndex) + prefix_p.Length;
                            endIndex = value.IndexOf(suffix_p, startIndex);

                            string param = value.Substring(startIndex, endIndex - startIndex);
                            string[] result = param.Split(' ');
                            c_kick = result[0];
                            c_dash = result[1];
                            c_turn = result[2];
                            c_catch = result[3];
                            c_move = result[4];
                            c_turnneck = result[5];
                            c_changeview = result[6];
                            c_say = result[7];
                            c_tackle = result[8];
                            c_pointto = result[9];
                            c_attentionto = result[10];
                            if (result.Length > 11)
                            {
                                c_changefocus = result[11];
                            }

                            startIndex = endIndex + suffix_p.Length;
                        }

                        p.Time = obj.Time;
                        p.Side = side;
                        p.Unum = System.Int32.Parse(unum);
                        p.Type = System.Int32.Parse(type);
                        if (state.StartsWith("0x"))
                        {
                            p.State = System.Convert.ToInt32(state, 16);
                        }
                        else
                        {
                            p.State = System.Int32.Parse(state);
                        }
                        p.X = (float)System.Double.Parse(x);
                        p.Y = (float)System.Double.Parse(y);
                        p.VelX = (float)System.Double.Parse(vel_x);
                        p.VelY = (float)System.Double.Parse(vel_y);
                        p.Body = (float)System.Double.Parse(body);
                        p.Neck = (float)System.Double.Parse(neck);
                        if (string.IsNullOrEmpty(point_x) == false)
                        {
                            p.PointToX = (float)System.Double.Parse(point_x);
                        }
                        if (string.IsNullOrEmpty(point_y) == false)
                        {
                            p.PointToY = (float)System.Double.Parse(point_y);
                        }
                        p.ViewQuality = v_quality;
                        p.ViewWidth = (float)System.Double.Parse(v_width);
                        if (string.IsNullOrEmpty(fp_dist) == false)
                        {
                            p.FocusPointDist = (float)System.Double.Parse(fp_dist);
                        }
                        if (string.IsNullOrEmpty(fp_dir) == false)
                        {
                            p.FocusPointDir = (float)System.Double.Parse(fp_dir);
                        }
                        p.Stamina = (float)System.Double.Parse(stamina);
                        p.Effort = (float)System.Double.Parse(effort);
                        p.Recovery = (float)System.Double.Parse(recovery);
                        if (string.IsNullOrEmpty(capacity) == false)
                        {
                            p.Capacity = (float)System.Double.Parse(capacity);
                        }
                        if (string.IsNullOrEmpty(f_side) == false)
                        {
                            p.AttentionToSide = f_side;
                        }
                        if (string.IsNullOrEmpty(f_unum) == false)
                        {
                            p.AttentionToUnum = System.Int32.Parse(f_unum);
                        }
                        p.KickCount = System.Int32.Parse(c_kick);
                        p.DashCount = System.Int32.Parse(c_dash);
                        p.TurnCount = System.Int32.Parse(c_turn);
                        p.CatchCount = System.Int32.Parse(c_catch);
                        p.MoveCount = System.Int32.Parse(c_move);
                        p.TurnNeckCount = System.Int32.Parse(c_turnneck);
                        p.ChangeViewCount = System.Int32.Parse(c_changeview);
                        p.SayCount = System.Int32.Parse(c_say);
                        p.TackleCount = System.Int32.Parse(c_tackle);
                        p.PointToCount = System.Int32.Parse(c_pointto);
                        p.AttentionToCount = System.Int32.Parse(c_attentionto);
                        if (string.IsNullOrEmpty(c_changefocus) == false)
                        {
                            p.ChangeFocusCount = System.Int32.Parse(c_changefocus);
                        }
                        obj.players.Add(p);
                    }
                    line = line.Replace(" ", "");
                    if (line != "")
                    {
                        return null;
                    }
                    return obj;
                }
            }
            catch (System.Exception e)
            {
                Debug.Log(e.Message);
            }
        }
        Debug.Log("Invalid string of Show.");
        return null;
    }
}
