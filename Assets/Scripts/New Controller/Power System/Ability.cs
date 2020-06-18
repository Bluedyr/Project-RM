using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * Title: Ability
 * Author: Marc Askew
 */

public abstract class Ability : ScriptableObject{

    

    public new string name;
    public string description;

    public AudioClip sound;

    public float cooldown = 1f;
    [HideInInspector] public float currentTime;
    [HideInInspector] public bool onCooldown;

    public abstract void Initialise(GameObject obj);
    
    public abstract void Update();//Used to lower cooldown as well as update things not reliant on buttons being held
    public abstract void OnDown();
    public abstract void OnHold();
    public abstract void OnUp();
    

}
