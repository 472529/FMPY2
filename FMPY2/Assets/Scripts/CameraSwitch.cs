using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public static class CameraSwitch 
{
    static List<CinemachineVirtualCamera> cameras = new List<CinemachineVirtualCamera>();

    public static void SwitchCamera(CinemachineVirtualCamera camera)
    {
        camera.Priority = 10;
        foreach(CinemachineVirtualCamera c in cameras)
        {
            if(c != camera)
            {
                c.Priority = 0;
            }
        }
    }
    public static void Register(CinemachineVirtualCamera camera)
    {
        cameras.Add(camera);
    }

    public static void UnRegister(CinemachineVirtualCamera camera)
    {
        cameras.Remove(camera);
    }
}
