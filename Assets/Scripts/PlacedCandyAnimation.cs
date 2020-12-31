using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlacedCandyAnimation : MonoBehaviour
{
    [SerializeField] Animator animator;
    public void SetCandyToIdle()
    {
        animator.SetBool("idle", true);
    }
}
