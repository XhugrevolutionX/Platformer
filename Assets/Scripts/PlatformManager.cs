using System.Collections;
using UnityEngine;

public class PlatformManager : MonoBehaviour
{
    // private FallingPlatform[] _fallingPlatforms;
    // private Vector3[] _platformStartPositions;
    // // Start is called once before the first execution of Update after the MonoBehaviour is created
    // void Start()
    // {
    //     _fallingPlatforms = GetComponentsInChildren<FallingPlatform>();
    //
    //     for (int i = 0; i < _fallingPlatforms.Length-1; i++)
    //     {
    //         _platformStartPositions[i] = _fallingPlatforms[i].transform.position;
    //     }
    // }
    //
    // // Update is called once per frame
    // void Update()
    // {
    //
    //     for (int i = 0; i < _fallingPlatforms.Length-1; i++)
    //     {
    //         if (!_fallingPlatforms[i].is_dead) continue;
    //         _fallingPlatforms[i].transform.position = _platformStartPositions[i];
    //         _fallingPlatforms[i].is_dead = false;
    //         //Destroy(_fallingPlatforms[i].gameObject);
    //     }
    // }

}
