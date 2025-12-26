using System;
using TMPro;
using UnityEngine;
using YG;

public class ObserverDefault : MonoBehaviour
{
    [SerializeField] private NumObserveReference target;
    private IObserveNum observeNum;

    [SerializeField] private bool saveDataDependence;
    [SerializeField] private TMP_Text label;
    [SerializeField] private TextCounterAnimator textCounterAnimator;
    private bool isStartMethodInvoke;
    
    [Space]
    
    [SerializeField] private int observedNumId;
    [SerializeField] private float animationTime;
    
    private void Start()
    {
        observeNum = target.GetComponent<IObserveNum>();
            
        if(!saveDataDependence || GameDataSaver.instance.IsDataLoaded)
            SetNum();
        else 
            GameDataSaver.instance.OnDataLoad += SetNum;
        
        observeNum.OnObserveNumChange += (int id, int difference) =>
        {
            var num = observeNum.GetNum(observedNumId);
            
            if (id != observedNumId || !gameObject.activeSelf)
                return;

            if (animationTime > 0)
                textCounterAnimator.PlayAnimation(label, num - difference, num, animationTime, AnimationType.integer);
            else
                textCounterAnimator.PlayAnimation(label, num - difference, num, 0, AnimationType.integer);
        };

        isStartMethodInvoke = true;
    }

    private void OnEnable()
    {
        if(isStartMethodInvoke)
            SetNum();
    }
    
    private void SetNum()
    {
        var currentNum = observeNum.GetNum(observedNumId);
        
        textCounterAnimator.PlayAnimation(label, currentNum, currentNum, 0, AnimationType.integer);
        textCounterAnimator.SetCurrentNum(currentNum);
        textCounterAnimator.StopAnimate();
    }
}
