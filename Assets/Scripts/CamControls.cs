using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UI;

public class CamControls : MonoBehaviour
{

    private enum PLACEMENTMODE
    {
        none,
        place,
        delete,
    }

    private PLACEMENTMODE mouseMode;
    private List<Button> buttonList;
    public bool play;
    private GameObject placingObject;

    public Controls controls;

    public float normalSpeed;
    public float fastSpeed;
    private float actualSpeed;
    public float rotSpeed;

    private Vector3 camRotation;
    private Vector3 moveDir;
    private Vector2 rotDir;

    private void Awake()
    {
        buttonList = new List<Button>();
        foreach (Transform child in GameObject.FindGameObjectWithTag("Canvas").transform)
        {
            buttonList.Add(child.GetComponent<Button>());
        }

        Cursor.lockState = CursorLockMode.Locked;
        actualSpeed = normalSpeed;
        controls = new Controls();

        controls.Player.Enable();
        Cursor.lockState = CursorLockMode.Locked;

        controls.UI.ChangeMode.started += context => SwitchToPlayer();
        controls.Player.ChangeMode.started += context => SwitchToUI();

        controls.Player.Quit.started += context => QuitApplication();
        controls.UI.Quit.started += context => QuitApplication();

        controls.UI.Click.performed += context => ClickToPlace();

        controls.Player.Move.performed += context => moveDir = context.ReadValue<Vector3>();
        controls.Player.Move.canceled += context => moveDir = Vector3.zero;
        controls.Player.Look.performed += context => rotDir = context.ReadValue<Vector2>();
        controls.Player.Look.canceled += context => rotDir = Vector3.zero;
        controls.Player.Fast.performed += context => actualSpeed = fastSpeed;
        controls.Player.Fast.canceled += context => actualSpeed = normalSpeed;
    }

    private void QuitApplication()
    {
        Application.Quit();
    }

    private void SwitchToPlayer()
    {
        controls.Player.Enable();
        controls.UI.Disable();
        Cursor.lockState = CursorLockMode.Locked;
    }
    private void SwitchToUI()
    {
        controls.Player.Disable();
        controls.UI.Enable();
        Cursor.lockState = CursorLockMode.Confined;
    }

    private void moveCam()
    {
        transform.Translate(moveDir * actualSpeed);
        camRotation += new Vector3(rotDir.x * rotSpeed, -rotDir.y * rotSpeed, 0);
        transform.rotation = Quaternion.Euler(camRotation.y, camRotation.x, 0);
    }


    //events invoked when each button is pressed
    public void PlacePressed()
    {
        mouseMode = PLACEMENTMODE.place;
        EnableDisableButtons();
        GameObject obj = Resources.Load<GameObject>("Prefabs/Building");
        placingObject = Instantiate(obj);
    }

    public void DeletePressed()
    {
        mouseMode = PLACEMENTMODE.delete;
        EnableDisableButtons();
    }

    public void OriginPressed()
    {
        mouseMode = PLACEMENTMODE.place;
        EnableDisableButtons();
        if (GameObject.FindGameObjectWithTag("Origin") != null)
        {
            placingObject = GameObject.FindGameObjectWithTag("Origin");
        }
        else
        {
            GameObject obj = Resources.Load<GameObject>("Prefabs/Origin");
            placingObject = Instantiate(obj);
        }
        placingObject.GetComponent<Collider>().enabled = false;
    }

    public void DestinationPressed()
    {
        mouseMode = PLACEMENTMODE.place;
        EnableDisableButtons();
        if (GameObject.FindGameObjectWithTag("Destination") != null)
        {
            placingObject = GameObject.FindGameObjectWithTag("Destination");
        }
        else
        {
            GameObject obj = Resources.Load<GameObject>("Prefabs/Destination");
            placingObject = Instantiate(obj);
        }
        placingObject.GetComponent<SetDestinationPull>().placed = false;
        placingObject.GetComponent<Collider>().enabled = false;
    }

    public void GoPressed()
    {
        play = true;
    }

    public void StopPressed()
    {
        play = false;
    }


    //events for after button is pressed 
    private void PlaceObject()
    {
        if (placingObject)
        {
            if (placingObject.tag == "Destination")
            {
                placingObject.GetComponent<SetDestinationPull>().placed = true;
            }
            placingObject.GetComponent<Collider>().enabled = true;
            placingObject = null;
        }
    }

    private void DeleteObject()
    {
        Ray ray = GetComponent<Camera>().ScreenPointToRay(Input.mousePosition); 
        RaycastHit hit; 
        Physics.Raycast(ray, out hit, Mathf.Infinity);
        if (hit.transform.gameObject.name.Contains("Building"))
        {
            GameObject.Destroy(hit.transform.gameObject); 
        } 
    }


    private void EnableDisableButtons()
    {
        foreach (Button obj in buttonList)
        {
            obj.interactable = !obj.interactable;
        }
    }

    private void Update()
    {
        moveCam();
        if (placingObject != null)
        {
            Ray ray = GetComponent<Camera>().ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            int layerMask = ~LayerMask.GetMask("IgnoreRaycast");
            Physics.Raycast(ray, out hit, Mathf.Infinity, layerMask);
            placingObject.transform.position = hit.point + new Vector3(0, (placingObject.transform.localScale.y / 2) - 1, 0);
        }
    }

    private void ClickToPlace()
    {
        switch (mouseMode)
        {
            case PLACEMENTMODE.none:
                break;
            case PLACEMENTMODE.place:
                PlaceObject();
                if (placingObject = null)
                {
                    mouseMode = PLACEMENTMODE.none;
                }
                break;
            case PLACEMENTMODE.delete:
                DeleteObject();
                break;
        }
        EnableDisableButtons();
    }
}



