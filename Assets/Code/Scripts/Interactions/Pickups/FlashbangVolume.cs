using UnityEngine;
using UnityEngine.Rendering;

public class FlashbangVolume : MonoBehaviour
{
    [SerializeField] private Volume volume;
    [SerializeField] private float fadeOutTime;

    private float startTime;
    private float duration;

    public void Show(float duration)
    {
        this.duration = duration;
        startTime = Time.time;
        
        gameObject.SetActive(true);
    }

    private void Update()
    {
        var p = (Time.time - startTime) / duration;
        
        // TODO
        //volume.weight = Mathf.Clamp() p;
    }
}
