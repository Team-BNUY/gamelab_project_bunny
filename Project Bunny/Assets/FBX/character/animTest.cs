using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
//temporary
public class animTest : MonoBehaviour
{
    public PlayerInput _playerInput;
    public Animator anim;

    // Start is called before the first frame update
    void Start()
    {
        anim = GetComponentInChildren<Animator>();
        _playerInput.actionEvents[0].AddListener(Animate);
    }

    private void Update()
    {
    }
    // Update is called once per frame
    void Animate(InputAction.CallbackContext context)
    {

        if (context.ReadValue<Vector2>().x!=0|| context.ReadValue<Vector2>().y!=0)
        {
            anim.SetFloat("Blend", 1f);
        }
        else
        {
            anim.SetFloat("Blend", 0f);
        }
    }
}
