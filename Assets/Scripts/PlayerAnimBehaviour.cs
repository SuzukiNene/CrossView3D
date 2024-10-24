using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimBehaviour : PlayerNeckBehaviour
{
    private Animator animator;

    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
    }

    /*
    // Update is called once per frame
    void Update()
    {
    }
    */

    private void FixedUpdate()
    {
        UpdateNeckRotation(Time.fixedDeltaTime);
    }

    public void OnAnimatorIK(int layerIndex)
    {
        UpdateNeckAnimator(animator);
    }

    public void Dash()
    {
        if (animator)
        {
            animator.SetBool("Back", false);
            animator.SetBool("Dash", true);
        }
    }

    public void Back()
    {
        if (animator)
        {
            animator.SetBool("Dash", false);
            animator.SetBool("Back", true);
        }
    }

    public void Idle()
    {
        if (animator)
        {
            animator.SetBool("Dash", false);
            animator.SetBool("Back", false);
        }
    }

    public void Kick()
    {
        if (animator)
        {
            if (animator.GetBool("Back"))
            {
                return;
            }
            if (animator.GetCurrentAnimatorStateInfo(0).IsName("Soccer Pass") == false &&
                animator.GetCurrentAnimatorStateInfo(0).IsName("Goalkeeper Pass") == false)
            {
                if (animator.GetBool("IsCatching"))
                {
                    animator.SetTrigger("Pass");
                    animator.SetBool("IsCatching", false);
                }
                else
                {
                    animator.SetTrigger("Kick");
                }
            }
        }
    }

    public void Tackle()
    {
        if (animator)
        {
            if (animator.GetBool("Back"))
            {
                return;
            }
            if (animator.GetCurrentAnimatorStateInfo(0).IsName("Soccer Tackle") == false)
            {
                animator.SetTrigger("Tackle");
            }
        }
    }

    public void Catch()
    {
        if (animator)
        {
            if (animator.GetBool("Back"))
            {
                return;
            }
            if (animator.GetCurrentAnimatorStateInfo(0).IsName("Goalkeeper Catch") == false)
            {
                animator.SetTrigger("Catch");
                animator.SetBool("IsCatching", true);
            }
        }
    }

    public void Speed(float speed)
    {
        if (animator)
        {
            animator.speed = speed;
        }
    }
}
