using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;
using UnityEngine.VFX.Utility;

[ExecuteAlways]
[RequireComponent(typeof(VisualEffect))]
public class VfxEventUtil : VFXOutputEventAbstractHandler
{
    public override bool canExecuteInEditor => true;
    [SerializeField] private AudioClip audioClip;
    [SerializeField] private Vector2 cameraShake;
   
    public override void OnVFXOutputEvent(VFXEventAttribute eventAttribute)
    {
        if (outputEvent.ToString() == "SoundEvent")
        {
            Managers.Sound.Play(audioClip);
        }

        if (outputEvent.ToString() == "CameraShakingEvent")
            CinemachineCameraShaking.Instance.CameraShake(cameraShake.x, cameraShake.y);
    }
}
