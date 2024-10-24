using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SettingsBehaviour : MonoBehaviour
{
    public GameControl gameControl;
    public Transform[] CameraPositions;
    public List<GameObject> PlayerObjects;
    public List<GameObject> PlayerPrefabs;

    private int CameraIndex;

    // Start is called before the first frame update
    void Start()
    {
        CameraIndex = 0;
        gameControl.SettingsCamera.transform.position = CameraPositions[CameraIndex].position;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.I))
        {
            foreach (GameObject obj in PlayerObjects)
            {
                PlayerAnimBehaviour player = obj.GetComponent<PlayerAnimBehaviour>();
                player.Idle();
            }
        }
        else if (Input.GetKeyDown(KeyCode.D))
        {
            foreach (GameObject obj in PlayerObjects)
            {
                PlayerAnimBehaviour player = obj.GetComponent<PlayerAnimBehaviour>();
                player.Dash();
            }
        }
        else if (Input.GetKeyDown(KeyCode.K))
        {
            foreach (GameObject obj in PlayerObjects)
            {
                PlayerAnimBehaviour player = obj.GetComponent<PlayerAnimBehaviour>();
                player.Kick();
            }
        }
        else if (Input.GetKeyDown(KeyCode.T))
        {
            foreach (GameObject obj in PlayerObjects)
            {
                PlayerAnimBehaviour player = obj.GetComponent<PlayerAnimBehaviour>();
                player.Tackle();
            }
        }
        else if (Input.GetKeyDown(KeyCode.C))
        {
            foreach (GameObject obj in PlayerObjects)
            {
                PlayerAnimBehaviour player = obj.GetComponent<PlayerAnimBehaviour>();
                player.Catch();
            }
        }
    }

    public void OnBack()
    {
        // Update PlayerPrefab_[L|R]
        switch(CameraIndex)
        {
            case 0:
                gameControl.PlayerPrefab_L = PlayerPrefabs[0];
                gameControl.PlayerPrefab_R = PlayerPrefabs[1];
                break;
            case 1:
                gameControl.PlayerPrefab_L = PlayerPrefabs[2];
                gameControl.PlayerPrefab_R = PlayerPrefabs[3];
                break;
            default:
                break;
        }

        // Camera
        gameControl.SettingsCamera.gameObject.SetActive(false);

        // UI Panel
        gameControl.Panel_Main.SetActive(true);
        gameControl.Panel_Settings.SetActive(false);
    }

    public void OnNext()
    {
        int newIndex = CameraIndex + 1;

        if (newIndex < CameraPositions.Length)
        {
            gameControl.SettingsCamera.transform.position = CameraPositions[newIndex].position;
            CameraIndex = newIndex;
        }
    }

    public void OnPrev()
    {
        int newIndex = CameraIndex - 1;

        if (newIndex >= 0)
        {
            gameControl.SettingsCamera.transform.position = CameraPositions[newIndex].position;
            CameraIndex = newIndex;
        }
    }
}
