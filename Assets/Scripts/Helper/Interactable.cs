using UnityEngine;
using UnityEngine.Events;
using DG.Tweening;

public class Interactable : MonoBehaviour
{
    public UnityEvent onPlayerEnter;

    bool hasCollided;

    public float val;
    public Transform target;
    public float duration = 2;
    public Ease ease = Ease.OutCubic;


    void OnTriggerEnter(Collider col)
    {
        if(col.CompareTag("Player") && !hasCollided)
        {
            target.DOLocalMoveY(val, duration).SetEase(ease);
            hasCollided = true;
        }
    }

}