using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [SerializeField] float speed = 5.0f;

    Transform target;

    // Start is called before the first frame update
    void Start()
    {
        target = GameObject.Find("Player").transform;
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 targetPos = new Vector3(target.position.x, target.position.y, transform.position.z);

        transform.position = Vector3.Lerp(transform.position, targetPos, Time.deltaTime * speed);
    }
}
