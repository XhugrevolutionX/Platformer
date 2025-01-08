using System.Collections;
using UnityEngine;

public class PlatformManager : MonoBehaviour
{
    private FallingPlatform[] _fallingPlatforms;
    private Vector3[] _platformStartPositions;
    private Rigidbody2D[] _platformRigidbodies;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _fallingPlatforms = GetComponentsInChildren<FallingPlatform>();
        _platformRigidbodies = GetComponentsInChildren<Rigidbody2D>();
        _platformStartPositions = new Vector3[_fallingPlatforms.Length];
    
        for (int i = 0; i < _fallingPlatforms.Length; i++)
        {
            _platformStartPositions[i] = _fallingPlatforms[i].transform.position;
        }
    }
    
    // Update is called once per frame
    void Update()
    {
        for (int i = 0; i < _fallingPlatforms.Length; i++)
        {
            if (!_fallingPlatforms[i].is_dead) continue;
            _platformRigidbodies[i].constraints = _platformRigidbodies[i].constraints ^ RigidbodyConstraints2D.FreezePositionY;
            _platformRigidbodies[i].bodyType = RigidbodyType2D.Static;
            _fallingPlatforms[i].transform.position = _platformStartPositions[i];
            _fallingPlatforms[i].is_dead = false;
        }
    }
}
