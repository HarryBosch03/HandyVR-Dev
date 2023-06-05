using System;
using System.Collections;
using HandyVR.Bindables;
using HandyVR.Bindables.Pickups;
using HandyVR.Interfaces;
using HandyVR.Player;
using HandyVR.Player.Input;
using UnityEngine;

[SelectionBase]
[DisallowMultipleComponent]
public sealed class Gun : MonoBehaviour, IVRBindableListener
{
    [SerializeField] private GameObject projectilePrefab;
    [SerializeField] private Transform muzzle;
    [SerializeField] private AudioClip clip;
    [SerializeField] private ParticleSystem[] shootFX;

    private Animator animator;
    
    private void Awake()
    {
        animator = GetComponentInChildren<Animator>();
    }
    
    public void InputCallback(VRHand hand, VRBindable bindable, IVRBindable.InputType type, HandInput.InputWrapper input)
    {
        if (type == IVRBindable.InputType.Trigger && input.State == HandInput.InputWrapper.InputState.PressedThisFrame)
        {
            Shoot();
        }
    }

    private void Shoot()
    {
        if (projectilePrefab)
        {
            Instantiate(projectilePrefab, muzzle.position, muzzle.rotation);
        }

        if (clip) AudioSource.PlayClipAtPoint(clip, muzzle.position);
        foreach (var fx in shootFX) fx.Play();

        animator.Play("Fire", 0, 0.0f);
    }
}