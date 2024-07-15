using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMove : MonoBehaviour
{
    [SerializeField]
    private float cameraSpeed;

    // Update is called once per frame
    void Update()
    {
        Move();
    }

    private void Move()
    {
        // horizontal 및 vertical 입력 값을 가져옵니다.
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");

        // 현재 위치를 가져옵니다.
        Vector3 position = transform.position;

        // 입력 값에 따라 위치를 업데이트합니다.
        position.x += horizontal * cameraSpeed ;
        position.y += vertical * cameraSpeed ;

        // 새로운 위치로 이동합니다.
        transform.position = position;
    }









}
