using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DemoBehaviour : MonoBehaviour
{
    public GameObject Demonstrator1;
    public GameObject Demonstrator2;

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

    private void OnEnable()
    {
        FieldPlayerBehaviour field;
        PlayerAnimBehaviour player;

        // 1
        field = Demonstrator1.GetComponent<FieldPlayerBehaviour>();
        player = Demonstrator1.GetComponentInChildren<PlayerAnimBehaviour>();
        StartCoroutine(Demonstration1(field, player));

        // 2
        field = Demonstrator2.GetComponent<FieldPlayerBehaviour>();
        player = Demonstrator2.GetComponentInChildren<PlayerAnimBehaviour>();
        StartCoroutine(Demonstration2(field, player));
    }

    private void OnDisable()
    {
        StopCoroutine(nameof(Demonstration1));
        StopCoroutine(nameof(Demonstration2));
    }

    private IEnumerator Demonstration1(FieldPlayerBehaviour field, PlayerAnimBehaviour player)
    {
        // Demo Scenario
        player.Idle();
        yield return new WaitForSeconds(1f);

        // Top
        player.Dash();
        field.TurnTo(180f, 0.5f);
        field.MoveTo(new Vector3(0f, 0f, 20f), 5f);
        yield return new WaitForSeconds(5f);

        player.Idle();
        field.TurnTo(-135f, 0.5f);
        player.TurnNeckTo(-45f, 0.5f);
        yield return new WaitForSeconds(3f);

        player.Kick();
        yield return new WaitForSeconds(3f);
        player.Kick();
        yield return new WaitForSeconds(3f);

        while (true)
        {
            // Left
            player.Dash();
            field.MoveTo(new Vector3(-20f, 0f, 0f), 5f);
            yield return new WaitForSeconds(5f);

            player.Idle();
            field.TurnTo(135f, 0.5f);
            yield return new WaitForSeconds(3f);

            player.Kick();
            yield return new WaitForSeconds(3f);
            player.Kick();
            yield return new WaitForSeconds(3f);

            // Bottom
            player.Dash();
            field.MoveTo(new Vector3(0f, 0f, -20f), 5f);
            yield return new WaitForSeconds(5f);

            player.Idle();
            field.TurnTo(45f, 0.5f);
            yield return new WaitForSeconds(3f);

            player.Kick();
            yield return new WaitForSeconds(3f);
            player.Kick();
            yield return new WaitForSeconds(3f);

            // Right
            player.Dash();
            field.MoveTo(new Vector3(20f, 0f, 0f), 5f);
            yield return new WaitForSeconds(5f);

            player.Idle();
            field.TurnTo(-45f, 0.5f);
            yield return new WaitForSeconds(3f);

            player.Kick();
            yield return new WaitForSeconds(3f);
            player.Kick();
            yield return new WaitForSeconds(3f);

            // Top
            player.Dash();
            field.MoveTo(new Vector3(0f, 0f, 20f), 5f);
            yield return new WaitForSeconds(5f);

            player.Idle();
            field.TurnTo(-135f, 0.5f);
            yield return new WaitForSeconds(3f);

            player.Kick();
            yield return new WaitForSeconds(3f);
            player.Kick();
            yield return new WaitForSeconds(3f);
        }
    }

    private IEnumerator Demonstration2(FieldPlayerBehaviour field, PlayerAnimBehaviour player)
    {
        // Demo Scenario
        player.Idle();
        yield return new WaitForSeconds(1f);

        // Bottom
        player.Dash();
        field.TurnTo(0f, 0.5f);
        field.MoveTo(new Vector3(0f, 0f, -20f), 5f);
        yield return new WaitForSeconds(5f);

        player.Idle();
        field.TurnTo(90f, 0.5f);
        player.TurnNeckTo(-45f, 0.5f);
        yield return new WaitForSeconds(3f);

        player.Kick();
        yield return new WaitForSeconds(3f);
        player.Kick();
        yield return new WaitForSeconds(3f);

        while (true)
        {
            // Left
            player.Dash();
            field.MoveTo(new Vector3(20f, 0f, -20f), 5f);
            yield return new WaitForSeconds(5f);

            player.Idle();
            field.TurnTo(0f, 0.5f);
            yield return new WaitForSeconds(3f);

            player.Kick();
            yield return new WaitForSeconds(3f);
            player.Kick();
            yield return new WaitForSeconds(3f);

            // Bottom
            player.Dash();
            field.MoveTo(new Vector3(20f, 0f, 20f), 5f);
            yield return new WaitForSeconds(5f);

            player.Idle();
            field.TurnTo(-90f, 0.5f);
            yield return new WaitForSeconds(3f);

            player.Kick();
            yield return new WaitForSeconds(3f);
            player.Kick();
            yield return new WaitForSeconds(3f);

            // Right
            player.Dash();
            field.MoveTo(new Vector3(-20f, 0f, 20f), 5f);
            yield return new WaitForSeconds(5f);

            player.Idle();
            field.TurnTo(-180f, 0.5f);
            yield return new WaitForSeconds(3f);

            player.Kick();
            yield return new WaitForSeconds(3f);
            player.Kick();
            yield return new WaitForSeconds(3f);

            // Top
            player.Dash();
            field.MoveTo(new Vector3(-20f, 0f, -20f), 5f);
            yield return new WaitForSeconds(5f);

            player.Idle();
            field.TurnTo(90f, 0.5f);
            yield return new WaitForSeconds(3f);

            player.Kick();
            yield return new WaitForSeconds(3f);
            player.Kick();
            yield return new WaitForSeconds(3f);
        }
    }
}
