using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum GameViewMode { MainCamera = 0, SubCamera = 1, PersonalCamera = 2 }
public enum GameNightMode { DayLight = 0, Night = 1 }

public class GameControl : MonoBehaviour
{
    public class SelectedPlayer
    {
        public string Side { get; set; }
        public int Unum { get; set; }

        public SelectedPlayer(string side, int unum)
        {
            Side = side;
            Unum = unum;
        }

        public bool Equals(string side, int unum)
        {
            return (Side == side && Unum == unum);
        }
    }

    public const string SIDE_L = "l";
    public const string SIDE_R = "r";

    private const float NIGHT_MODE_INTENSITY = 4f;
    private const string INFO_MSG_PREFIX = "INFO: ";

    public GameObject Demo;
    public GameObject PlayerPrefab_L;
    public GameObject PlayerPrefab_R;
    public GameObject MovingObject;
    public GameObject Ball;
    public GameObject Goal_l;
    public GameObject Goal_r;
    public Text LeftTeam;
    public Text LeftScore;
    public Text RightTeam;
    public Text RightScore;
    public Button SelectFile;
    public Button Backword;
    public Button Prev;
    public Button Pause;
    public Button Play;
    public Button Next;
    public Button Forward;
    public Dropdown Speed;
    public InputField Cycle;
    public Text Total;
    public InputField PlayMode;
    public Button ZoomOut;
    public Button ZoomIn;
    public Button ViewMode;
    public Button NightMode;
    public Button Information;
    public Button Settings;
    public Camera MainCamera;
    public Camera SubCamera;
    public Camera MiniCamera;
    public Camera PersonalCamera;
    public Camera SettingsCamera;
    public Light DirectionalLight;
    public GameObject Panel_Top;
    public GameObject Panel_Main;
    public GameObject Panel_Middle_Left;
    public GameObject Panel_Middle_Right;
    public Dropdown ChoosePlayer;
    public Text PlayerInfo;
    public Text FPSViewer;
    public GameObject Flags;
    public GameObject Panel_Settings;
    public GameObject Spinner;
    public Dictionary<string, Dictionary<int, GameObject>> PlayerObjects;

    public GameViewMode viewMode;
    public GameNightMode nightMode;

    public SelectedPlayer selectedPlayer;

    public delegate void NotifyPlayerObjectsUpdated(Dictionary<string, Dictionary<int, GameObject>> PlayerObjects);
    public event NotifyPlayerObjectsUpdated PlayerObjectsUpdatedEvent = delegate { };
    public delegate void NotifySelectedPlayerUpdated(string Side, int Unum);
    public event NotifySelectedPlayerUpdated SelectedPlayerUpdatedEvent = delegate { };

    private RuntimeAnimatorController goalKeeperAnimatorController;

    // Start is called before the first frame update
    void Start()
    {
        const string INFO_MSG_APP_STARTED = "please select log file...";

        // Team name and Score
        LeftTeam.text = "Team L";
        LeftScore.text = "0";
        RightTeam.text = "Team R";
        RightScore.text = "0";

        // Hide Pause button
        Pause.gameObject.SetActive(false);
        Play.gameObject.SetActive(true);

        // Disable buttons, input field
        SelectFile.interactable = true;
        Pause.interactable = false;
        Play.interactable = false;
        Prev.interactable = false;
        Next.interactable = false;
        Backword.interactable = false;
        Forward.interactable = false;
        Speed.interactable = false;
        Cycle.interactable = false;
        ZoomOut.interactable = true;
        ZoomIn.interactable = true;
        ViewMode.interactable = false;
        NightMode.interactable = false;
        Information.interactable = true;
        Settings.interactable = true;

        // Cycle and Total
        Cycle.text = "";
        Total.text = "6000";

        // PlayerObjects([l|r]->Unum->GameObject)
        PlayerObjects = new Dictionary<string, Dictionary<int, GameObject>>();
        PlayerObjects.Add(SIDE_L, new Dictionary<int, GameObject>());
        PlayerObjects.Add(SIDE_R, new Dictionary<int, GameObject>());

        viewMode = GameViewMode.MainCamera;
        nightMode = GameNightMode.DayLight;
        selectedPlayer = new SelectedPlayer(SIDE_L, 1);

        ShowMessage(INFO_MSG_APP_STARTED);

        PlayerObjectsUpdatedEvent += UpdatePlayerObjects;
        SelectedPlayerUpdatedEvent += UpdateSelectedPlayer;

        goalKeeperAnimatorController = Resources.Load<RuntimeAnimatorController>("Animators/Goal Keeper");
    }

    /*
    // Update is called once per frame
    void Update()
    {
    }
    */

    public bool Setup(SimLogData data)
    {
        const string INFO_MSG_READY = "ready to play...";

        // TeamName and Score board with match results
        int last = data.CycleDetails.TeamCycles.Count - 1;
        LeftTeam.text = data.CycleDetails.TeamCycles[last].LeftName;
        LeftScore.text = data.CycleDetails.TeamCycles[last].LeftScore.ToString();
        RightTeam.text = data.CycleDetails.TeamCycles[last].RightName;
        RightScore.text = data.CycleDetails.TeamCycles[last].RightScore.ToString();

        // Total
        Total.text = data.CycleObjects[data.CycleObjects.Count - 1].Time.ToString();

        // Goal's width
        Goal_l.transform.localScale = new Vector3(1f, 1f, 1f);
        Goal_r.transform.localScale = new Vector3(1f, 1f, 1f);
        float goal_width_def = GetWidthOfGoalObject(Goal_l);
        float goal_width = data.ServerParams.GetFloatParam("goal_width", goal_width_def);
        float goal_rate = goal_width / goal_width_def;
        Goal_l.transform.localScale = new Vector3(goal_rate, 1f, 1f);
        Goal_r.transform.localScale = new Vector3(goal_rate, 1f, 1f);
        //Debug.Log("goal_width=" + goal_width + ", goal_width_def=" + goal_width_def);

        // Ball initial position
        float height = Ball.transform.position.y;
        Ball.transform.position = new Vector3(0f, height, 0f);
        Ball.GetComponent<BallBehaviour>().RollTo(Ball.transform.position);     // Stop rotation

        // Instance of Players...
        SetupPlayerObjects(data);

        ShowMessage(INFO_MSG_READY);

        return true;
    }

    private void SetupPlayerObjects(SimLogData data)
    {
        const float ALIGNMENT_COORD_Z = -35f;
        const float ALIGNMENT_STEP_X = 3f;

        ClearPlayerObjects();

        // Left side
        Vector3 pos = new Vector3(0f, 0f, ALIGNMENT_COORD_Z);
        Vector3 step = new Vector3(-(ALIGNMENT_STEP_X), 0f, 0f);

        foreach (KeyValuePair<int, List<PlayerCycleObject>> kvp in data.CycleDetails.PlayerCycles[SIDE_L])
        {
            PlayerCycleObject player = kvp.Value[0];

            // position of each player
            pos += step;
            // Instantiate by prefab user selected
            GameObject obj = InstantiatePlayerObject(data, player, PlayerPrefab_L, pos);
            if (obj)
            {
                obj.transform.parent = MovingObject.transform;
                PlayerObjects[SIDE_L].Add(player.Unum, obj);
            }
        }

        // Right side
        pos = new Vector3(0f, 0f, ALIGNMENT_COORD_Z);
        step = new Vector3(ALIGNMENT_STEP_X, 0f, 0f);

        foreach (KeyValuePair<int, List<PlayerCycleObject>> kvp in data.CycleDetails.PlayerCycles[SIDE_R])
        {
            PlayerCycleObject player = kvp.Value[0];

            // position of each player
            pos += step;
            // Instantiate by prefab user selected
            GameObject obj = InstantiatePlayerObject(data, player, PlayerPrefab_R, pos);
            if (obj)
            {
                obj.transform.parent = MovingObject.transform;
                PlayerObjects[SIDE_R].Add(player.Unum, obj);
            }
        }

        // Event
        PlayerObjectsUpdatedEvent(PlayerObjects);
    }

    private void ClearPlayerObjects()
    {
        foreach (KeyValuePair<int, GameObject> kvp in PlayerObjects[SIDE_L])
        {
            Destroy(kvp.Value);
        }
        PlayerObjects[SIDE_L].Clear();

        foreach (KeyValuePair<int, GameObject> kvp in PlayerObjects[SIDE_R])
        {
            Destroy(kvp.Value);
        }
        PlayerObjects[SIDE_R].Clear();
    }

    private GameObject InstantiatePlayerObject(SimLogData data, PlayerCycleObject player, GameObject prefab, Vector3 pos)
    {
        const string PARAM_VISIBLE_ANGLE = "visible_angle";
        const string PARAM_UNUM_FAR_LENGTH = "unum_far_length";

        GameObject obj = Instantiate(prefab, pos, Quaternion.identity);

        if (obj)
        {
            // Side, Unum
            FieldPlayerBehaviour field = obj.GetComponent<FieldPlayerBehaviour>();
            field.Side = player.Side;
            field.Unum = player.Unum;

            // Attribute of ViewAngle by ServerParams, PlayerTypes
            ViewAngleBehaviour view = obj.GetComponentInChildren<ViewAngleBehaviour>();
            view.viewAngle = data.ServerParams.GetFloatParam(PARAM_VISIBLE_ANGLE, view.viewAngle);
            ParamDictionary dict = data.PlayerTypes.GetPlayerType(player.Type);
            if (dict != null)
            {
                view.viewRange = dict.GetFloatParam(PARAM_UNUM_FAR_LENGTH, view.viewRange);
            }

            // if Goalie...
            if ((player.State & PlayerCycleObject.STATE_GOALIE) != 0)
            {
                Animator animator = obj.GetComponentInChildren<Animator>();
                animator.runtimeAnimatorController = goalKeeperAnimatorController;
            }

            // SpotLight intensity
            Light light = obj.GetComponentInChildren<Light>();
            SetLightIntensity(light, player.Side, player.Unum);
        }
        return obj;
    }

    public void SetLightIntensity(Light light, string side, int unum)
    {
        float intensity = 0f;

        if (nightMode == GameNightMode.Night)
        {
            if (viewMode != GameViewMode.PersonalCamera || selectedPlayer.Equals(side, unum))
            {
                intensity = NIGHT_MODE_INTENSITY;
            }
        }
        light.intensity = intensity;
    }

    private float GetWidthOfGoalObject(GameObject obj)
    {
        float width = 0f;

        Bounds bounds = new Bounds(obj.transform.position, Vector3.zero);
        Renderer[] renderers = obj.GetComponentsInChildren<Renderer>();

        foreach (Renderer renderer in renderers)
        {
            bounds.Encapsulate(renderer.bounds);
        }
        width = bounds.size.x > bounds.size.z ? bounds.size.x : bounds.size.z;

        return width;
    }

    public void ShowMessage(string msg)
    {
        PlayMode.text = INFO_MSG_PREFIX + msg;
    }

    public void UpdateScoreBoardBackcolor(string PlayMode)
    {
        // Panel Color
        Image img = Panel_Top.GetComponent<Image>();

        if (PlayMode == "goal_l" || PlayMode == "goal_r")
        {
            // White
            img.color = new Color(1f, 1f, 1f, 0.7843f);
        }
        else if (PlayMode == "offside_l" || PlayMode == "offside_r")
        {
            // Light Blue
            img.color = new Color(0f, 0.5019f, 1f, 0.7843f);
        }
        else if (PlayMode.StartsWith("foul_"))
        {
            // Yellow
            img.color = new Color(1f, 1f, 0f, 0.7843f);
        }
        else
        {
            // Default
            img.color = new Color(1f, 1f, 1f, 0.3921f);
        }
    }

    public void OnUpdateCycle(int cycle)
    {
        // Cycle
        Cycle.text = cycle.ToString();

        // Button status in Pause status.
        if (Play.interactable == true)
        {
            if (cycle == 1)
            {
                Backword.interactable = false;
                Prev.interactable = false;
            }
            else
            {
                Backword.interactable = true;
                Prev.interactable = true;
            }
        }
    }

    public void OnUpdateStatus(SimLogPlayer.Status status)
    {
        switch (status)
        {
            case SimLogPlayer.Status.Ready:
                SelectFile.interactable = true;
                Pause.gameObject.SetActive(false);
                Play.gameObject.SetActive(true);
                Pause.interactable = false;
                Play.interactable = true;
                Prev.interactable = false;
                Next.interactable = false;
                Backword.interactable = false;
                Forward.interactable = false;
                Speed.interactable = true;
                Cycle.interactable = false;
                ZoomOut.interactable = true;
                ZoomIn.interactable = true;
                NightMode.interactable = true;
                ViewMode.interactable = true;
                Information.interactable = true;
                Settings.interactable = true;
                break;
            case SimLogPlayer.Status.Playing:
                SelectFile.interactable = false;
                Pause.gameObject.SetActive(true);
                Play.gameObject.SetActive(false);
                Pause.interactable = true;
                Play.interactable = false;
                Prev.interactable = false;
                Next.interactable = false;
                Backword.interactable = false;
                Forward.interactable = false;
                Speed.interactable = false;
                Cycle.interactable = false;
                ZoomOut.interactable = true;
                ZoomIn.interactable = true;
                NightMode.interactable = true;
                ViewMode.interactable = true;
                Information.interactable = true;
                Settings.interactable = false;
                break;
            case SimLogPlayer.Status.Pause:
                SelectFile.interactable = true;
                Pause.gameObject.SetActive(false);
                Play.gameObject.SetActive(true);
                Pause.interactable = false;
                Play.interactable = true;
                Prev.interactable = true;
                Next.interactable = true;
                Backword.interactable = true;
                Forward.interactable = true;
                Speed.interactable = true;
                Cycle.interactable = true;
                ZoomOut.interactable = true;
                ZoomIn.interactable = true;
                NightMode.interactable = true;
                ViewMode.interactable = true;
                Information.interactable = true;
                Settings.interactable = true;
                break;
            case SimLogPlayer.Status.Stopped:
                SelectFile.interactable = true;
                Pause.gameObject.SetActive(false);
                Play.gameObject.SetActive(true);
                Pause.interactable = false;
                Play.interactable = true;
                Prev.interactable = false;
                Next.interactable = false;
                Backword.interactable = false;
                Forward.interactable = false;
                Speed.interactable = true;
                Cycle.interactable = false;
                ZoomOut.interactable = true;
                ZoomIn.interactable = true;
                NightMode.interactable = true;
                ViewMode.interactable = true;
                Information.interactable = true;
                Settings.interactable = true;
                break;
            default:
                break;
        }
    }

    private void UpdatePlayerObjects(Dictionary<string, Dictionary<int, GameObject>> PlayerObjects)
    {
        if (viewMode == GameViewMode.PersonalCamera)
        {
            ChangeSelectedPlayer(ChoosePlayer.value);
        }
    }

    public void ChangeSelectedPlayer(int index)
    {
        int count = PlayerObjects[SIDE_L].Count + PlayerObjects[SIDE_R].Count;

        if (index >= 0 && index < count)
        {
            // e.g. "L-01"
            string caption = ChoosePlayer.captionText.text;

            string side = caption.Substring(0, 1).ToLower();
            int unum = int.Parse(caption.Substring(2, 2));

            PersonalCameraBehaviour personalCamera = PersonalCamera.GetComponent<PersonalCameraBehaviour>();
            if (personalCamera)
            {
                personalCamera.Target = PlayerObjects[side][unum].transform;
            }

            selectedPlayer.Side = side;
            selectedPlayer.Unum = unum;

            // Event
            SelectedPlayerUpdatedEvent(side, unum);
        }
    }

    private void UpdateSelectedPlayer(string Side, int Unum)
    {
        UpdatePlayersSpotLight();
    }

    public void UpdatePlayersSpotLight()
    {
        foreach (KeyValuePair<int, GameObject> kvp in PlayerObjects[SIDE_L])
        {
            Light spotLight = kvp.Value.GetComponentInChildren<Light>();
            SetLightIntensity(spotLight, SIDE_L, kvp.Key);
        }

        foreach (KeyValuePair<int, GameObject> kvp in PlayerObjects[SIDE_R])
        {
            Light spotLight = kvp.Value.GetComponentInChildren<Light>();
            SetLightIntensity(spotLight, SIDE_R, kvp.Key);
        }
    }
}
