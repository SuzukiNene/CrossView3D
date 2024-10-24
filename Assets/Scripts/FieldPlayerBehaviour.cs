using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FieldPlayerBehaviour : PlayerBehaviour
{
    public string Side { get; set; }
    public int Unum { get; set; }

    private PlayerAnimBehaviour playerAnim;
    private ViewAngleBehaviour viewAngle;

    // Start is called before the first frame update
    void Start()
    {
        playerAnim = GetComponentInChildren<PlayerAnimBehaviour>();
        viewAngle = GetComponentInChildren<ViewAngleBehaviour>();
    }

    /*
    // Update is called once per frame
    void Update()
    {
    }
    */

    private void FixedUpdate()
    {
        UpdatePosition(Time.fixedDeltaTime);
        UpdateRotation(Time.fixedDeltaTime);
    }

    public void TurnBodyNeckViewAngleTo(float body, float neck, float angle, float range, float seconds)
    {
        TurnTo(body, seconds);
        playerAnim.TurnNeckTo(neck, seconds);
        viewAngle.TurnTo(body, neck, seconds);
        viewAngle.ChangeAngle(angle, range, seconds);
    }

    public void Idle()
    {
        playerAnim.Idle();
    }

    public void Dash()
    {
        playerAnim.Dash();
    }

    public void Kick()
    {
        playerAnim.Kick();
    }

    public void Tackle()
    {
        playerAnim.Tackle();
    }

    public void Catch()
    {
        playerAnim.Catch();
    }

    public void Speed(float speed)
    {
        playerAnim.Speed(speed);
    }
}
