using System.Collections;
using System.Collections.Generic;
using UnityEngine; 

public class myNpcInteraction : MonoBehaviour {

    public static bool interactPermit = false;
    public static bool isIteracting = false;

    public static bool turnPermit = false;
    //交互
    private Interactable currentInteractable;

    private void Update()
    {
        if(interactPermit && !isIteracting)
        {
            if (Input.GetKeyDown(KeyCode.F))
            {
                isIteracting = true;
            }
            currentInteractable.Interact();
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if(other.tag == "NPC")
        {
            interactPermit = true;
            currentInteractable = other.GetComponent<Interactable>();
        }
        if(other.tag == "Gate")
        {
            turnPermit = true;
            currentInteractable = other.GetComponent<Interactable>();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if(other.tag == "NPC")
        {
            interactPermit = false;
            currentInteractable.Interact();
        }
        if(other.tag == "Gate")
        {

        }
    }


}
