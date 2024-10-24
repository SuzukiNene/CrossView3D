using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class AnimationDialog : MonoBehaviour
{
    // ref) https://nekojara.city/unity-animated-dialog

    public Animator animator;

    protected bool IsOpened => gameObject.activeSelf;
    protected bool IsTransition { get; private set; }

    private const string ANIM_STATE_PARAM = "IsOpened";
    private const string ANIM_STATE_SHOWN = "Shown";
    private const string ANIM_STATE_HIDDEN = "Hidden";

    public void Open()
    {
        if (IsOpened || IsTransition)
        {
            return;
        }
        gameObject.SetActive(true);
        animator.SetBool(ANIM_STATE_PARAM, true);

        StartCoroutine(WaitAnimation(ANIM_STATE_SHOWN));
    }

    public void Close()
    {
        if (!IsOpened || IsTransition)
        {
            return;
        }
        animator.SetBool(ANIM_STATE_PARAM, false);

        StartCoroutine(WaitAnimation(ANIM_STATE_HIDDEN, () => gameObject.SetActive(false)));
    }

    private IEnumerator WaitAnimation(string stateName, UnityAction onCompleted = null)
    {
        IsTransition = true;

        yield return new WaitUntil(() =>
        {
            var state = animator.GetCurrentAnimatorStateInfo(0);
            bool ret = state.IsName(stateName) && state.normalizedTime >= 1;
            return ret;
        });

        IsTransition = false;
        onCompleted?.Invoke();
    }
}
