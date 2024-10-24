using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DisplayUnumBehaviour : MonoBehaviour
{
    public GameControl gameControl;
    public Text UnumPrefab;
    public GameObject displayPanel;
    public int fontSize = 14;

    private Camera cameraObj;
    private Dictionary<GameObject, Text> PlayerDictionary;
    private Color defaultColor;

    /*
    // Start is called before the first frame update
    void Start()
    {
    }
    */

    void Awake()
    {
        cameraObj = GetComponent<Camera>();
        PlayerDictionary = new Dictionary<GameObject, Text>();

        gameControl.PlayerObjectsUpdatedEvent += UpdatePlayerObjects;
        gameControl.SelectedPlayerUpdatedEvent += UpdateSelectedPlayer;
    }

    // Update is called once per frame
    void Update()
    {
        UpdatePlayerUnumPosition();
    }

    private void OnEnable()
    {
        if (gameControl.PlayerObjects != null)
        {
            CreateUnumObjects(gameControl.PlayerObjects);
        }
    }

    private void OnDisable()
    {
        ClearUnumObjects();
    }

    private void UpdatePlayerObjects(Dictionary<string, Dictionary<int, GameObject>> PlayerObjects)
    {
        if (cameraObj.isActiveAndEnabled)
        {
            CreateUnumObjects(PlayerObjects);
        }
        else
        {
            ClearUnumObjects();
        }
    }

    private void UpdatePlayerUnumPosition()
    {
        foreach (KeyValuePair<GameObject, Text> kvp in PlayerDictionary)
        {
            Vector3 world_pos = GetPlayerHeadPosition(kvp.Key);
            Vector2 screen_pos = cameraObj.WorldToScreenPoint(world_pos);
            Vector2 local_pos;

            RectTransform rect = displayPanel.GetComponent<RectTransform>();
            RectTransformUtility.ScreenPointToLocalPointInRectangle(rect, screen_pos, null, out local_pos);

            local_pos += new Vector2(0f, kvp.Value.rectTransform.rect.height / 2);
            kvp.Value.transform.localPosition = local_pos;

            // If Player is hidden(places backward)
            bool isFront = Vector3.Dot(world_pos - cameraObj.transform.position, cameraObj.transform.forward) > 0;
            kvp.Value.gameObject.SetActive(isFront);
        }
    }

    private Vector3 GetPlayerHeadPosition(GameObject obj)
    {
        Bounds bounds = new Bounds(obj.transform.position, Vector3.zero);
        Renderer[] renderers = obj.GetComponentsInChildren<Renderer>();

        foreach (Renderer renderer in renderers)
        {
            bounds.Encapsulate(renderer.bounds);
        }
        Vector3 head = new Vector3(bounds.center.x, bounds.max.y, bounds.center.z);

        return head;
    }

    private void CreateUnumObjects(Dictionary<string, Dictionary<int, GameObject>> PlayerObjects)
    {
        ClearUnumObjects();

        string side = GameControl.SIDE_L;
        foreach (KeyValuePair<int, GameObject> kvp in PlayerObjects[side])
        {
            int unum = kvp.Key;
            Text unumObj = CreatePlayerUnum(side, unum);
            PlayerDictionary.Add(kvp.Value, unumObj);
        }
        side = GameControl.SIDE_R;
        foreach (KeyValuePair<int, GameObject> kvp in PlayerObjects[side])
        {
            int unum = kvp.Key;
            Text unumObj = CreatePlayerUnum(side, unum);
            PlayerDictionary.Add(kvp.Value, unumObj);
        }
    }

    private void ClearUnumObjects()
    {
        foreach (KeyValuePair<GameObject, Text> kvp in PlayerDictionary)
        {
            try
            {
                if (kvp.Value.gameObject != null)
                {
                    Destroy(kvp.Value.gameObject);
                }
            }
            catch (MissingReferenceException) { }
        }
        PlayerDictionary.Clear();
    }

    private Text CreatePlayerUnum(string side, int unum)
    {
        Text obj = Instantiate(UnumPrefab, Vector3.zero, Quaternion.identity);
        if (obj)
        {
            obj.text = unum.ToString();
            obj.fontSize = this.fontSize;
            obj.transform.SetParent(displayPanel.transform);
            obj.transform.localPosition = Vector3.zero;

            if (cameraObj.name == gameControl.MiniCamera.name || 
                cameraObj.name == gameControl.PersonalCamera.name)
            {
                if (gameControl.selectedPlayer.Equals(side, unum))
                {
                    obj.color = Color.yellow;
                    obj.fontStyle = FontStyle.Bold;
                }
                else
                {
                    defaultColor = obj.color;
                }
            }

            return obj;
        }
        return null;
    }

    private void UpdateSelectedPlayer(string side, int unum)
    {
        if (cameraObj.name == gameControl.MiniCamera.name ||
            cameraObj.name == gameControl.PersonalCamera.name)
        {
            foreach (KeyValuePair<GameObject, Text> kvp in PlayerDictionary)
            {
                FieldPlayerBehaviour field = kvp.Key.GetComponent<FieldPlayerBehaviour>();
                Text obj = kvp.Value;

                if (field.Side == side && field.Unum == unum)
                {
                    obj.color = Color.yellow;
                    obj.fontStyle = FontStyle.Bold;
                }
                else
                {
                    obj.color = defaultColor;
                    obj.fontStyle = FontStyle.Normal;
                }
            }
        }
    }
}
