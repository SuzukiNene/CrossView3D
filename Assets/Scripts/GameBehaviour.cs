using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Text;

public class GameBehaviour : MonoBehaviour
{
    public GameControl gameControl;
    public SimLogReader simLogReader;
    public SimLogPlayer simLogPlayer;
    public SelectFileDialog SelectFileDialog;

    private readonly float[] playSpeedList = { 2.0f, 1.5f, 1.25f, 1.0f, 0.75f, 0.5f, 0.25f };
    private readonly float[] animSpeedList = { 1.5f, 1.34f, 1.17f, 1.0f, 0.84f, 0.67f, 0.5f };

    // Start is called before the first frame update
    void Start()
    {
        SelectFileDialog.gameObject.SetActive(false);
        ChangeViewMode(GameViewMode.MainCamera);
        ChangeNightMode(GameNightMode.DayLight);
    }

    // Update is called once per frame
    void Update()
    {
        if (SelectFileDialog.isActiveAndEnabled == false)
        {
            // Keyboard Input
            CheckKeyboardInput();

            // Zoom In/Out
            float wheel = Input.GetAxis("Mouse ScrollWheel");
            if (wheel != 0)
            {
                if (IsOnUI(Input.mousePosition) == false)
                {
                    // ScrollWheel Up(+0.1), Down(-0.1)
                    wheel = (wheel > 0) ? 0.1f : -0.1f;
                    OnZoom(wheel);
                }
            }
        }
    }

    private void CheckKeyboardInput()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            // [Space] key
            if (gameControl.Play.interactable)
            {
                OnPlay();
            }
            else if (gameControl.Pause.interactable)
            {
                OnPause();
            }
        }
        else if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            // [->] key
            if (gameControl.Next.interactable)
            {
                OnNext();
            }
        }
        else if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            // [<-] key
            if (gameControl.Prev.interactable)
            {
                OnPrev();
            }
        }
        else if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            // [up] key
            if (gameControl.Forward.interactable)
            {
                OnForward();
            }
        }
        else if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            // [down] key
            if (gameControl.Backword.interactable)
            {
                OnBackword();
            }
        }
        else if (Input.GetKeyDown(KeyCode.KeypadPlus))
        {
            // [+] key
            if (gameControl.ZoomIn.interactable)
            {
                OnZoomIn();
            }
        }
        else if (Input.GetKeyDown(KeyCode.KeypadMinus))
        {
            // [-] key
            if (gameControl.ZoomOut.interactable)
            {
                OnZoomOut();
            }
        }
        else if (Input.GetKeyDown(KeyCode.V))
        {
            // [v] key
            if (gameControl.ViewMode.interactable)
            {
                OnViewMode();
            }
        }
        else if (Input.GetKeyDown(KeyCode.N))
        {
            // [n] key
            if (gameControl.NightMode.interactable)
            {
                OnNightMode();
            }
        }
        else if (Input.GetKeyDown(KeyCode.I))
        {
            // [i] key
            if (gameControl.Information.interactable)
            {
                OnInformation();
            }
        }
        else if (Input.GetKeyDown(KeyCode.S))
        {
            // [s] key
            if (gameControl.Settings.interactable)
            {
                OnSettings();
            }
        }
    }

    public void OnSelectFile()
    {
        SelectFileDialog.Open();
    }

    public void OnSelectFile_OK()
    {
        SelectFileDialog.Close();

        // Wait until close animation ends.
        Invoke(nameof(ReadFile), 0.333f);
    }

    private void ReadFile()
    {
        const string INFO_MSG_READING = "reading log file...";

        string uri = SelectFileDialog.GetUri();

        // Read log file
        simLogReader.Read(uri, Encoding.UTF8, OnReadFile);
        gameControl.ShowMessage(INFO_MSG_READING);

        // Disable SelectFile and other buttons until OnReadFile().
        EnableGameControls(false);
        gameControl.Spinner.SetActive(true);
    }

    private void OnReadFile(SimLogReader reader, SimLogReader.Status status)
    {
        const string INFO_MSG_FAILED_READ_FILE = "Failed to read log file.";
        const string INFO_MSG_FAILED_SETUP_GAME = "Failed to setup display item.";
        const string INFO_MSG_FAILED_SETUP_PLAYER = "Failed to setup log player.";

        gameControl.Demo.SetActive(false);
        gameControl.Spinner.SetActive(false);
        EnableGameControls(true);

        if (status == SimLogReader.Status.Success)
        {
            if (gameControl.Setup(simLogReader.simLogData) == false)
            {
                gameControl.ShowMessage(INFO_MSG_FAILED_SETUP_GAME);
                Debug.Log("GameControl.Setup() failed.");
                return;
            }
            if (simLogPlayer.Setup(gameControl, simLogReader.simLogData) == false)
            {
                gameControl.ShowMessage(INFO_MSG_FAILED_SETUP_PLAYER);
                Debug.Log("SimLogPlayer.Setup() failed.");
                return;
            }
            Invoke(nameof(OnChangeSpeed), 0.1f);
        }
        else
        {
            gameControl.ShowMessage(INFO_MSG_FAILED_READ_FILE);
            Debug.LogFormat("GameBehaviour.OnReadFile(status={0})", status);
            return;
        }
    }

    private void EnableGameControls(bool enable)
    {
        gameControl.SelectFile.interactable = enable;
        gameControl.Backword.interactable = enable;
        gameControl.Prev.interactable = enable;
        gameControl.Pause.interactable = enable;
        gameControl.Play.interactable = enable;
        gameControl.Next.interactable = enable;
        gameControl.Forward.interactable = enable;
        gameControl.Speed.interactable = enable;
        gameControl.Cycle.interactable = enable;
        gameControl.ZoomOut.interactable = enable;
        gameControl.ZoomIn.interactable = enable;
        gameControl.ViewMode.interactable = enable;
        gameControl.NightMode.interactable = enable;
        gameControl.Information.interactable = enable;
        gameControl.Settings.interactable = enable;
    }

    public void OnPlay()
    {
        const string INFO_MSG_FAILED = "Failed to start log player.";

        if (simLogPlayer.Play() == false)
        {
            gameControl.ShowMessage(INFO_MSG_FAILED);
            Debug.Log("SimLogPlayer.Play() failed.");
            return;
        }
    }

    public void OnPause()
    {
        const string INFO_MSG_FAILED = "Failed to pause log player.";

        if (simLogPlayer.Pause() == false)
        {
            gameControl.ShowMessage(INFO_MSG_FAILED);
            Debug.Log("SimLogPlayer.Pause() failed.");
            return;
        }
    }

    public void OnPrev()
    {
        const string INFO_MSG_FAILED = "Failed to back to prev cycle.";

        if (simLogPlayer.Prev() == false)
        {
            gameControl.ShowMessage(INFO_MSG_FAILED);
            Debug.Log("SimLogPlayer.Prev() failed.");
            return;
        }
    }

    public void OnNext()
    {
        const string INFO_MSG_FAILED = "Failed to step to next cycle.";

        if (simLogPlayer.Next() == false)
        {
            gameControl.ShowMessage(INFO_MSG_FAILED);
            Debug.Log("SimLogPlayer.Next() failed.");
            return;
        }
    }

    public void OnBackword()
    {
        const string INFO_MSG_FAILED = "Failed to back to prev event.";

        if (simLogPlayer.Backword() == false)
        {
            gameControl.ShowMessage(INFO_MSG_FAILED);
            Debug.Log("SimLogPlayer.Backword() failed.");
            return;
        }
    }

    public void OnForward()
    {
        const string INFO_MSG_FAILED = "Failed to step to next event.";

        if (simLogPlayer.Forward() == false)
        {
            gameControl.ShowMessage(INFO_MSG_FAILED);
            Debug.Log("SimLogPlayer.Forward() failed.");
            return;
        }
    }

    public void OnChangeSpeed()
    {
        if (gameControl.Speed.value >= 0 && gameControl.Speed.value < playSpeedList.Length)
        {
            // Speed of Simulator
            simLogPlayer.Speed = playSpeedList[gameControl.Speed.value];

            // Speed of Animation
            string side = GameControl.SIDE_L;
            foreach (KeyValuePair<int, GameObject> kvp in gameControl.PlayerObjects[side])
            {
                FieldPlayerBehaviour player = kvp.Value.GetComponent<FieldPlayerBehaviour>();
                player.Speed(animSpeedList[gameControl.Speed.value]);
            }
            side = GameControl.SIDE_R;
            foreach (KeyValuePair<int, GameObject> kvp in gameControl.PlayerObjects[side])
            {
                FieldPlayerBehaviour player = kvp.Value.GetComponent<FieldPlayerBehaviour>();
                player.Speed(animSpeedList[gameControl.Speed.value]);
            }
        }
    }

    public void OnEndEditCycle(string text)
    {
        const string INFO_MSG_FAILED = "Failed to jump to specified cycle.";

        if (!Input.GetKeyDown(KeyCode.Return) && !Input.GetKeyDown(KeyCode.KeypadEnter))
        {
            return;
        }
        if (string.IsNullOrEmpty(text))
        {
            return;
        }

        int cycle;
        int cycle_max = simLogReader.simLogData.CycleObjects[simLogReader.simLogData.CycleObjects.Count - 1].Time;
        if (System.Int32.TryParse(text, out cycle) == false)
        {
            return;
        }
        if (cycle <= 0)
        {
            cycle = 1;
        }
        if (cycle > cycle_max)
        {
            cycle = cycle_max;
        }

        if (simLogPlayer.Jump(cycle) == false)
        {
            gameControl.ShowMessage(INFO_MSG_FAILED);
            Debug.Log("SimLogPlayer.Jump() failed.");
            return;
        }
    }

    public void OnZoomIn()
    {
        OnZoom(0.1f);
    }

    public void OnZoomOut()
    {
        OnZoom(-0.1f);
    }

    private void OnZoom(float wheel)
    {
        if (gameControl.ZoomIn.interactable &&
            gameControl.ZoomOut.interactable)
        {
            MainCameraBehaviour mainCamera = Camera.main.GetComponent<MainCameraBehaviour>();
            if (mainCamera)
            {
                mainCamera.Zoom(wheel);
            }
        }
    }
    private bool IsOnUI(Vector3 mousePosition)
    {
        PointerEventData pointer = new PointerEventData(EventSystem.current);
        List<RaycastResult> list = new List<RaycastResult>();

        pointer.position = mousePosition;
        EventSystem.current.RaycastAll(pointer, list);

        return (list.Count > 0);
    }

    public void OnNightMode()
    {
        switch(gameControl.nightMode)
        {
            case GameNightMode.DayLight:
                ChangeNightMode(GameNightMode.Night);
                break;
            case GameNightMode.Night:
                ChangeNightMode(GameNightMode.DayLight);
                break;
            default:
                Debug.Log("Invalid ViewMode");
                return;
        }
    }

    private void ChangeNightMode(GameNightMode newMode)
    {
        const float NIGHT_MODE_AMBIENT = 0.7f;
        const float NIGHT_MODE_DIRECTIONAL = 0.2f;

        gameControl.nightMode = newMode;

        float ambient;
        float directional;

        switch(newMode)
        {
            case GameNightMode.DayLight:
                ambient = 1f;
                directional = 1f;
                break;
            case GameNightMode.Night:
                ambient = NIGHT_MODE_AMBIENT;
                directional = NIGHT_MODE_DIRECTIONAL;
                break;
            default:
                Debug.Log("Invalid ViewMode");
                return;
        }

        // Environment
        RenderSettings.ambientIntensity = ambient;
        gameControl.DirectionalLight.intensity = directional;
        // SpotLight
        gameControl.UpdatePlayersSpotLight();
    }

    public void OnViewMode()
    {
        switch(gameControl.viewMode)
        {
            case GameViewMode.MainCamera:
                ChangeViewMode(GameViewMode.SubCamera);
                break;
            case GameViewMode.SubCamera:
                ChangeViewMode(GameViewMode.PersonalCamera);
                break;
            case GameViewMode.PersonalCamera:
                ChangeViewMode(GameViewMode.MainCamera);
                break;
            default:
                Debug.Log("Invalid ViewMode");
                return;
        }
        gameControl.UpdatePlayersSpotLight();
    }

    private void ChangeViewMode(GameViewMode newMode)
    {
        gameControl.viewMode = newMode;

        switch(newMode)
        {
            case GameViewMode.MainCamera:
                gameControl.MainCamera.gameObject.SetActive(true);
                gameControl.SubCamera.gameObject.SetActive(false);
                gameControl.MiniCamera.gameObject.SetActive(false);
                gameControl.PersonalCamera.gameObject.SetActive(false);
                gameControl.Panel_Middle_Left.gameObject.SetActive(false);
                gameControl.Panel_Middle_Right.gameObject.SetActive(false);
                break;
            case GameViewMode.SubCamera:
                gameControl.MainCamera.gameObject.SetActive(false);
                gameControl.SubCamera.gameObject.SetActive(true);
                gameControl.MiniCamera.gameObject.SetActive(false);
                gameControl.PersonalCamera.gameObject.SetActive(false);
                gameControl.Panel_Middle_Left.gameObject.SetActive(false);
                gameControl.Panel_Middle_Right.gameObject.SetActive(false);
                break;
            case GameViewMode.PersonalCamera:
                gameControl.MainCamera.gameObject.SetActive(false);
                gameControl.SubCamera.gameObject.SetActive(false);
                gameControl.MiniCamera.gameObject.SetActive(true);
                gameControl.PersonalCamera.gameObject.SetActive(true);
                gameControl.Panel_Middle_Left.gameObject.SetActive(true);
                gameControl.Panel_Middle_Right.gameObject.SetActive(true);
                break;
            default:
                Debug.Log("Invalid ViewMode");
                return;
        }
        if (newMode == GameViewMode.PersonalCamera)
        {
            // Update Player by Chooser
            gameControl.ChangeSelectedPlayer(gameControl.ChoosePlayer.value);
            // Update Flags by Information toggle
            if (gameControl.FPSViewer.isActiveAndEnabled)
            {
                gameControl.Flags.SetActive(true);
            }
        }
        else
        {
            // Update Flags
            gameControl.Flags.SetActive(false);
        }
    }

    public void OnValueChanged_ChoosePlayer(int index)
    {
        gameControl.ChangeSelectedPlayer(index);
    }

    public void OnInformation()
    {
        bool state = gameControl.FPSViewer.isActiveAndEnabled;

        if (state)
        {
            // Flags
            gameControl.Flags.SetActive(false);
            // Information
            gameControl.PlayerInfo.gameObject.SetActive(false);
            // FPSViewer
            gameControl.FPSViewer.gameObject.SetActive(false);
        }
        else
        {
            // Flags
            if (gameControl.viewMode == GameViewMode.PersonalCamera)
            {
                gameControl.Flags.SetActive(true);
            }
            // Information
            gameControl.PlayerInfo.gameObject.SetActive(true);
            // FPSViewer
            gameControl.FPSViewer.gameObject.SetActive(true);
        }
    }

    public void OnSettings()
    {
        // Camera
        gameControl.SettingsCamera.gameObject.SetActive(true);

        // UI Panel
        gameControl.Panel_Main.SetActive(false);
        gameControl.Panel_Settings.SetActive(true);
    }
}
