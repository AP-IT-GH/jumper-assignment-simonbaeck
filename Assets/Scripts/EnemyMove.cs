using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMove : MonoBehaviour
{
    [SerializeField]
    float _Speed = 1.0f;

    [SerializeField]
    float _endPoint = 9.0f;

    private static GameObject _Enemy;
    private static Vector3 _spawnPoint;

    // Start is called before the first frame update
    void Start()
    {
        _Enemy = this.gameObject;
        _spawnPoint = this.gameObject.transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        _Enemy.transform.position = transform.position + (Vector3.right * _Speed / 100);

        if (_Enemy.transform.position.x > _endPoint)
        {
            _Enemy.transform.position = _spawnPoint; 
        }
    }

    public static void resetPosition()
    {
        // _Enemy.transform.position = _spawnPoint;
    }
}
