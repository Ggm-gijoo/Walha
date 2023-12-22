using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using UnityEngine.Events;

public class CinemachineCameraShaking : MonoSingleton<CinemachineCameraShaking>
{

    //public float ShakeDuration = 0.3f;          // Time the Camera Shake effect will last
    [SerializeField]
    private float ShakeFrequency = 2.0f;         // Cinemachine Noise Profile Parameter

    private float ShakeElapsedTime = 0f;

    // Cinemachine Shake
    public Camera mainCam;
    public CinemachineVirtualCamera VirtualCamera;
    private CinemachineBasicMultiChannelPerlin virtualCameraNoise;

    void Start()
    {
        mainCam = Camera.main;

        if (VirtualCamera == null)
            VirtualCamera = CinemachineCore.Instance.GetActiveBrain(0).ActiveVirtualCamera.VirtualCameraGameObject.GetComponent<CinemachineVirtualCamera>();
        // Get Virtual Camera Noise Profile
        if (VirtualCamera != null)
            virtualCameraNoise = VirtualCamera.GetCinemachineComponent<Cinemachine.CinemachineBasicMultiChannelPerlin>();

        virtualCameraNoise.m_AmplitudeGain = 0f;
        virtualCameraNoise.m_FrequencyGain = 0f;
    }

    public void ChangeCam()
    {
        VirtualCamera = CinemachineCore.Instance.GetActiveBrain(0).ActiveVirtualCamera.VirtualCameraGameObject.GetComponent<CinemachineVirtualCamera>();
        if (VirtualCamera != null)
            virtualCameraNoise = VirtualCamera.GetCinemachineComponent<Cinemachine.CinemachineBasicMultiChannelPerlin>();
    }
    public void CameraShake(float amplitude = 2f, float duration = 0.1f)
    {
        ChangeCam();
        StopCoroutine(IECameraShake(amplitude, duration));
        StartCoroutine(IECameraShake(amplitude, duration));
    }
    private IEnumerator IECameraShake(float amplitude, float duration)
    {
        ShakeElapsedTime = duration;

        if (VirtualCamera != null && virtualCameraNoise != null)
        {
            virtualCameraNoise.m_AmplitudeGain = amplitude;
            virtualCameraNoise.m_FrequencyGain = ShakeFrequency;

            yield return new WaitForSeconds(ShakeElapsedTime);

            virtualCameraNoise.m_AmplitudeGain = 0f;
        }
        else
        {
            Debug.LogError("VirtualCamera is null or virtualCameraNoise is null");
        }

        yield break;
    }
}