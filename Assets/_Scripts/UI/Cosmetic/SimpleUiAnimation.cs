using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;

public class SimpleUiAnimation : MonoBehaviour,IPointerEnterHandler,IPointerExitHandler,IPointerUpHandler,IPointerDownHandler
{
    [SerializeField] private Transform[] upElements;
    [SerializeField] private Transform[] downElements;
    
    private Vector3[] defaultScalesUpElements = new Vector3[8];
    private Vector3[] defaultScalesDownElements = new Vector3[8];

    
    [Space] 
    
    [SerializeField] private float upElementsMultiplier;
    [SerializeField] private float downElementsMultiplier;
    [SerializeField] private float animationTime;

    [Space] 
    
    [SerializeField] private AudioCastData buttonDown;
    [SerializeField] private AudioCastData buttonUp;

    private void Awake()
    {
        WriteDefaultScales();
        void WriteDefaultScales()
        {
            for (var i = 0; i < upElements.Length; i++)
                defaultScalesUpElements[i] = upElements[i].localScale;
            
            for (var i = 0; i < downElements.Length; i++)
                defaultScalesDownElements[i] = downElements[i].localScale;
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        MoveElements(upElements,defaultScalesUpElements,upElementsMultiplier);
        MoveElements(downElements,defaultScalesDownElements,downElementsMultiplier);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        MoveElements(upElements,defaultScalesUpElements,1);
        MoveElements(downElements,defaultScalesDownElements,1);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        MoveElements(upElements,defaultScalesUpElements,upElementsMultiplier);
        MoveElements(downElements,defaultScalesDownElements,downElementsMultiplier);
        
        AudioPoolService.audioPoolServiceInstance.CastAudio(buttonUp);
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        MoveElements(upElements,defaultScalesUpElements,downElementsMultiplier);
        MoveElements(downElements,defaultScalesDownElements,upElementsMultiplier);
        
        AudioPoolService.audioPoolServiceInstance.CastAudio(buttonDown);
    }

    private void MoveElements(Transform[] elements, Vector3[] defaultScales, float power)
    {
        for (var i = 0; i < elements.Length; i++)
        {
            var element = elements[i];

            element.DOScale(defaultScales[i] * power, animationTime);
        }
    }
    
}
