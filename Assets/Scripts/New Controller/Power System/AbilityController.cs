using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * Title: AbilityController
 * Author: Marc Askew
 */

public class AbilityController : MonoBehaviour
{
    // Start is called before the first frame update

    public KeyCode useAbility;
    
    public Ability[] abilities;
    int selectedAbility;







    [HideInInspector] public bool isGrappling;

    void Start(){
        selectedAbility = 0;
        
        //print("Selected Ability: " + abilities[selectedAbility].name);

        for (int i = 0; i < abilities.Length; i++) {
            abilities[i].Initialise(this.gameObject);
        }
    }

    // Update is called once per frame
    void Update() {
        bool selectedOnCooldown = abilities[selectedAbility].onCooldown;
        //Debug.Log(abilities[selectedAbility].name +" cooldown: " +selectedOnCooldown);
        if (Input.GetKeyDown(useAbility)&&!selectedOnCooldown) {
            abilities[selectedAbility].OnDown();
        }
        else if (Input.GetKey(useAbility) && !selectedOnCooldown) {
            abilities[selectedAbility].OnHold();
        }
        else if (Input.GetKeyUp(useAbility) && !selectedOnCooldown) {
            abilities[selectedAbility].OnUp();
        }
        else if(Input.GetAxisRaw("Mouse ScrollWheel")!=0){
            if (Input.GetAxisRaw("Mouse ScrollWheel") >0) {
                selectedAbility += 1;
                if (selectedAbility > abilities.Length-1) {
                    selectedAbility = 0;
                }
            }
            else {
                selectedAbility -= 1;
                if (selectedAbility <0) {
                    selectedAbility = abilities.Length-1;
                }
            }
            
            //print("Selected Ability: " +abilities[selectedAbility].name);
        }

        for (int i = 0; i < abilities.Length; i++) {
            if (abilities[i].onCooldown) {
                abilities[i].Update();
            }
        }


    }

}
