using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FPSViewer : MonoBehaviour
{
    // ref) https://deve-cat.com/unity-frame-rate/

    [SerializeField]
    private float Interval = 0.1f;

    private Text _tex;

    private float _time_cnt;
    private int _frames;
    private float _time_mn;
    private float _fps;
    
    // Start is called before the first frame update
    void Start()
    {
        //Application.targetFrameRate = 30;
        
        _tex = this.GetComponent<Text>();
    }

    // Update is called once per frame
    void Update()
    {
        _time_mn -= Time.deltaTime;
        _time_cnt += Time.timeScale / Time.deltaTime;
        _frames++;

        if (0 < _time_mn) return;

        _fps = _time_cnt / _frames;
        _time_mn = Interval;
        _time_cnt = 0;
        _frames = 0;

        _tex.text = "FPS: " + _fps.ToString("f2");
    }
}
