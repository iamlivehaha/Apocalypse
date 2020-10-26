using System.Collections;
using System.Collections.Generic;
using Assets.Scripts.GamePlay.CharacterController.Enemy;
using Assets.Scripts.Managers;
using Spine.Unity.Examples;
using StarPlatinum.Base;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

[System.Serializable]
public struct CameraSetting
{
    public Transform target;
    public Vector3 offset;
    public Vector3 min;
    public Vector3 max;
    public float size;
    public float smoothing;
};

public class CameraManager : MonoSingleton<CameraManager>
{
    /// <summary>移动控制</summary>
    private GameObject m_cameraObj;
    public Camera m_camera;
    public ConstrainedCamera m_constrainedCamera;
    public CameraSetting m_defaultSetting;
    private float temporthographicSize;

    public override void SingletonInit()
    {
        m_cameraObj = Camera.main.gameObject;
        m_camera = Camera.main;
        m_constrainedCamera = m_cameraObj.GetComponent<ConstrainedCamera>();
        m_defaultSetting = new CameraSetting();
        m_defaultSetting.target = m_constrainedCamera.target;
        m_defaultSetting.offset = m_constrainedCamera.offset;
        m_defaultSetting.min = m_constrainedCamera.min;
        m_defaultSetting.max = m_constrainedCamera.max;
        m_defaultSetting.smoothing = m_constrainedCamera.smoothing;
        m_defaultSetting.size = m_camera.orthographicSize;
        temporthographicSize = m_camera.orthographicSize;
    }

    public void SetCameraSetting(CameraSetting cameraSetting)
    {
        m_constrainedCamera.target = cameraSetting.target;
        m_constrainedCamera.offset = cameraSetting.offset;
        m_constrainedCamera.min = cameraSetting.min;
        m_constrainedCamera.max = cameraSetting.max;
        m_constrainedCamera.smoothing = cameraSetting.smoothing;
        temporthographicSize = cameraSetting.size;
    }

    public void ReturnDefaultSetting()
    {
        m_constrainedCamera.target = m_defaultSetting.target;
        m_constrainedCamera.offset = m_defaultSetting.offset;
        m_constrainedCamera.min = m_defaultSetting.min;
        m_constrainedCamera.max = m_defaultSetting.max;
        m_constrainedCamera.smoothing = m_defaultSetting.smoothing;
        temporthographicSize = m_defaultSetting.size;
    }
    void LateUpdate()
    {
        m_camera.orthographicSize = Mathf.Lerp(m_camera.orthographicSize, temporthographicSize,
            m_constrainedCamera.smoothing * Time.deltaTime);
    }
}
