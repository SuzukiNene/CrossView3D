using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;

#if UNITY_STANDALONE || UNITY_EDITOR
using SFB;
#endif

#if UNITY_IOS || UNITY_ANDROID
using Keiwando.NFSO;
#endif

public class SelectFileDialog : AnimationDialog
{
    [SerializeField] private Toggle uiIsFile;
    [SerializeField] private Toggle uiIsURL;
    [SerializeField] private InputField uiFilePath;
    [SerializeField] private InputField uiURL;
    [SerializeField] private Button uiBrowse;
    [SerializeField] private Button uiOpenSite;
    [SerializeField] private Dropdown uiSampleURL;

    public bool IsFile { get { return uiIsFile.isOn; } }
    public string FilePath { get { return uiFilePath.text; } }
    public string URL { get { return uiURL.text; } }

    // Start is called before the first frame update
    void Start()
    {
#if UNITY_WEBGL
        // Disable FilePath controls
        uiIsURL.isOn = true;
        uiIsFile.interactable = false;
        uiFilePath.interactable = false;
        uiBrowse.interactable = false;

        // Switch URL controls for WebGL version
        uiOpenSite.gameObject.SetActive(false);
        uiSampleURL.gameObject.SetActive(true);
#else
        uiSampleURL.gameObject.SetActive(false);
#endif
    }

    /*
    // Update is called once per frame
    void Update()
    {

    }
    */

    public void OnBrowse()
    {
#if UNITY_STANDALONE || UNITY_EDITOR
        var extensions = new[]
        {
            new ExtensionFilter("Log Files", "rcg"),
            new ExtensionFilter("All Files", "*")
        };
        string dir = FilePath;
        if (dir != "")
        {
            try
            {
                dir = Path.GetDirectoryName(FilePath);
            }
            catch { }
        }
        string[] path = StandaloneFileBrowser.OpenFilePanel("Browse File", dir, extensions, false);

        if (path.Length == 1 && path[0].Length > 0)
        {
            uiFilePath.text = path[0];
            //Debug.LogFormat("SelectFileDialog.OnBrowse(path={0}", path[0]);
        }
#elif UNITY_IOS || UNITY_ANDROID
        // FIXME
        /*
        SupportedFileType[] supportedFileTypes = {
            SupportedFileType.Any
        };
        NativeFileSO.shared.OpenFile(supportedFileTypes,
            delegate (bool fileWasOpened, OpenedFile file)
            {
                if (fileWasOpened)
                {
                    uiFilePath.text = Application.persistentDataPath + $"/{file.Name}";    // FIXME
                }
            });
        */
#endif
    }

    public void OnOpenSite()
    {
        const string url = "https://archive.robocup.info/Soccer/Simulation/2D/logs/RoboCup/";

        Application.OpenURL(url);
    }

    public void OnSampleURL(int index)
    {
        const string sampleURL_1 = "https://archive.robocup.info/Soccer/Simulation/2D/logs/RoboCup/2008/MainRound/GroupF/200807181000-Oxsy_4-vs-HELIOS2008_3.rcg.gz";
        const string sampleURL_2 = "https://archive.robocup.info/Soccer/Simulation/2D/logs/RoboCup/2014/MainRound/201407231109-YuShan2014_2-vs-Infographics_0.rcg.gz";
        const string sampleURL_3 = "https://archive.robocup.info/Soccer/Simulation/2D/logs/RoboCup/2023/Finals/202307090739-Hermes2D_2-vs-R3CESBU_6.rcg.gz";
        string[] SampleURLs =
        {
            sampleURL_1,
            sampleURL_2,
            sampleURL_3
        };

        if (index >= 0 && index < SampleURLs.Length)
        {
            uiURL.text = SampleURLs[index];
        }
    }

    public string GetUri()
    {
        try
        {
            if (IsFile)
            {
                return new System.Uri(FilePath).AbsoluteUri;
            }
            else
            {
                return new System.Uri(URL).AbsoluteUri;
            }
        }
        catch { }

        return string.Empty;
    }
}
