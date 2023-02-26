using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    private UIInput input;
    public GameObject book;
    private CharacterMovement movement;

    private void Awake()
    {
        input = new UIInput();
        input.UI.Tab.performed += ctx =>
        {
            book.SetActive(!book.activeSelf);
        };
        movement = FindObjectOfType<CharacterMovement>();
    }

    void OnEnable()
    {
        input.Enable();
    }

    void OnDisable()
    {
        input.Disable();
    }

    void Update()
    {
        if (book.activeSelf)
        {
            Time.timeScale = 0;
            movement.enabled = false;
        }
        else
        {
            Time.timeScale = 1;
            movement.enabled = true;
        }
    }
}
