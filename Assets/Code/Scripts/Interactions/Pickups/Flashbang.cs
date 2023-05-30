using System;
using System.Collections;
using System.Collections.Generic;
using HandyVR.Bindables;
using HandyVR.Bindables.Pickups;
using HandyVR.Interfaces;
using HandyVR.Player;
using HandyVR.Player.Input;
using UnityEngine;

public class Flashbang : MonoBehaviour, IVRBindableListener
{
    [SerializeField] private float brightness = 10.0f;
    [SerializeField] private AudioClip sound;
    [SerializeField] private float fuseTime = 3.0f;
    [SerializeField] private Rigidbody dynamicHandle;
    [SerializeField] private GameObject staticHandle;
    [SerializeField] private Vector3 throwForce;
    [SerializeField] private Vector3 throwTorque;

    private bool armed;
   
    private void Start()
    {
        staticHandle.gameObject.SetActive(true);
        dynamicHandle.gameObject.SetActive(false);
    }

    public void InputCallback(VRHand hand, VRBindable bindable, IVRBindable.InputType type, HandInput.InputWrapper input)
    {
        if (type != IVRBindable.InputType.Trigger) return;
        
        switch (input.Value)
        {
            case > 0.5f:
                armed = true;
                break;
            case < 0.5f when armed:
                StartCoroutine(ExplodeRoutine());
                break;
        }
    }

    private IEnumerator ExplodeRoutine()
    {
        staticHandle.SetActive(false);
        dynamicHandle.gameObject.SetActive(true);
        
        dynamicHandle.AddForce(throwForce, ForceMode.VelocityChange);
        dynamicHandle.AddTorque(throwTorque, ForceMode.VelocityChange);

        yield return new WaitForSeconds(fuseTime);
        
        AudioSource.PlayClipAtPoint(sound, transform.position);
        Destroy(gameObject);
    }
}
