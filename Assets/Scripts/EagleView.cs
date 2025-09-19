using System.Collections;
using UnityEngine;
using UnityEngine.Rendering;

public class EagleView : MonoBehaviour
{
    
    [SerializeField] private Volume _EagleViewVolume;
    [SerializeField] private Volume _ATRVolume;

    // Start is called once before the first execution of Update after the MonoBehaviour is created


    public void SwitchEagleView()
    {
        _EagleViewVolume.weight = _EagleViewVolume.weight == 1.0f ? 0.0f : 1.0f;
        _ATRVolume.weight = _ATRVolume.weight == 1.0f ? 0.0f : 1.0f;
    }
}
