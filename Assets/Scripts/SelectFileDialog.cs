using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using SFB;
using System.IO;
//using Keiwando.NFSO;

public class SelectFileDialog : AnimationDialog
{
    [SerializeField] private Toggle uiFile;
    [SerializeField] private InputField uiFilePath;
    [SerializeField] private InputField uiURL;

    public bool IsFile { get { return uiFile.isOn; } }
    public string FilePath { get { return uiFilePath.text; } }
    public string URL { get { return uiURL.text; } }

    /*
    // Start is called before the first frame update
    void Start()
    {
        
    }

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
#elif UNITY_WEBGL
        
#elif UNITY_IOS || UNITY_ANDROID
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
