using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CamControl : MonoBehaviour
{
    public Camera playerCam;
    public Camera worldCam;
    public static int currentCamera;

    private void Start()
    {
        if(currentCamera == 0)
        {
            playerCam.enabled = true;
            worldCam.enabled = false;
        }
        else
        {
            playerCam.enabled = false;
            worldCam.enabled = true;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.C))
        {
            if(currentCamera == 0)
            {
                playerCam.enabled = false;
                worldCam.enabled = true;
                currentCamera = 1;
            }
            else
            {
                playerCam.enabled = true;
                worldCam.enabled = false;
                currentCamera = 0;
            }
        }
    }
}
