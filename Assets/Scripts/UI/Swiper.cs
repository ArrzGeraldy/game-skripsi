using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Swiper : MonoBehaviour
{
    [SerializeField] int maxPage;
    int currPage;
    Vector3 targetPos;
    [SerializeField] Vector3 pageStep;
    [SerializeField] float tweenTime;
    [SerializeField] LeanTweenType tweenType;

    [SerializeField] GameObject parent;
    [SerializeField] GameObject btnNext;
    [SerializeField] GameObject btnPrev;
    [SerializeField] RectTransform levelPageRect;
    List<List<AnimatedMotion>> pageMotions = new();
    


    void Awake()
    {
        currPage = 1;
        targetPos = levelPageRect.localPosition;


    }

    void Start()
    {
        HandleVisibleButton();

        foreach (Transform child in levelPageRect)
        {
            List<AnimatedMotion> listMotion = new();

            foreach (Transform c in child)
            {
                AnimatedMotion motion = c.GetComponent<AnimatedMotion>();

                if (motion != null)
                {
                    listMotion.Add(motion);
                }
            }

            pageMotions.Add(listMotion);
        }

        PauseMotion();

        maxPage = pageMotions.Count;

        ActivatePageMotion(currPage - 1);

        parent.SetActive(false);

    }

    public void Next()
    {
        if(currPage < maxPage)
        {
            currPage++;
            targetPos += pageStep;
            MovePage();

            PauseMotion();
            ActivatePageMotion(currPage - 1);
        }
    }

    public void Prev()
    {
        
        if(currPage > 1)
        {
            currPage--;
            targetPos -= pageStep;
            MovePage();

            PauseMotion();
            ActivatePageMotion(currPage - 1);

            
        }
    }

    void MovePage()
    {
        Debug.Log("mov: "+ currPage);
        levelPageRect.LeanMoveLocal(targetPos, tweenTime).setEase(tweenType);
        HandleVisibleButton();
    }

    void PauseMotion()
    {
        foreach(var m in pageMotions)
        {
           foreach(var a in m)
            {
                a.ResetAnimation();
                a.isAnimated = false;
            }
        }
    }

    void ActivatePageMotion(int pageIndex)
    {
        foreach (var m in pageMotions[pageIndex])
        {
            m.isAnimated = true;
        }
    }

    void HandleVisibleButton()
    {
        bool hasNext = currPage < maxPage;
        bool hasPrev = currPage > 1;

        btnNext.SetActive(hasNext);
        btnPrev.SetActive(hasPrev);
    } 
}
