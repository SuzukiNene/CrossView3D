using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PersonalInformationBehaviour : MonoBehaviour
{
    public GameControl gameControl;

    private Text textObj;

    // Start is called before the first frame update
    void Start()
    {
        textObj = GetComponent<Text>();
        gameControl.SelectedPlayerUpdatedEvent += UpdateSelectedPlayer;
    }

    /*
    // Update is called once per frame
    void Update()
    {
    }
    */

    private void UpdateSelectedPlayer(string Side, int Unum)
    {
        // FIXME
        string info = "";
        textObj.text = info;
    }

    public void UpdatePlayerInformation(PlayerCycleObject obj)
    {
        if (textObj)
        {
            string info = string.Format("[Information]\n" +
                "Player({0}, {1}, {2}, 0x{3})\n" +
                "Position({4}, {5}) Velocity({6}, {7})\n" +
                "Direction({8}, {9}) View({10}, {11})\n" +
                "PointTo({12}, {13}) Focus({14}, {15}) AttentionTo({16}, {17})\n" +
                "Stamina({18}, {19}, {20}, {21})\n" +
                "Count({22}, {23}, {24}, {25}, {26}, {27}, {28}, {29}, {30}, {31}, {32}, {33})",
                obj.Side, obj.Unum, obj.Type, obj.State.ToString("X2"), obj.X, obj.Y, obj.VelX, obj.VelY, obj.Body, obj.Neck,
                obj.ViewQuality, obj.ViewWidth,
                obj.PointToX, obj.PointToY, obj.FocusPointDist, obj.FocusPointDir, obj.AttentionToSide, obj.AttentionToUnum,
                obj.Stamina, obj.Effort, obj.Recovery, obj.Capacity,
                obj.KickCount, obj.DashCount, obj.TurnCount, obj.CatchCount, obj.MoveCount, obj.TurnNeckCount, obj.ChangeViewCount, obj.SayCount, obj.TackleCount, obj.PointToCount, obj.AttentionToCount, obj.ChangeFocusCount);

            textObj.text = info;
        }
    }
}
