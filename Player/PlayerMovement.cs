using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Cinemachine;

public class PlayerMovement : MonoBehaviour
{
    PhotonView view;

    [Header("Movement")]
    public float moveSpeed;
    public float runSpeed;
    private bool isRunning;

    public float groundDrag;

    [Header("Jump")]
    public float jumpFroce;
    public float jumpCooldown;
    public float airMultiplier;
    bool readyToJump;

    [Header("Hp")]
    public float hp;
    public Text hpText;

    [Header("Animation")]
    public Animator anim;

    public GameObject L_hand;
    public GameObject R_hand;

    private bool isAttacking = false;
    private bool attackTriggered = false;
    private float attackTime = 1.0f;

    [Header("Keybinds")]
    public KeyCode jumpKey = KeyCode.Space;

    [Header("Ground Check")]
    public float playerHeight;
    public LayerMask WhatIsGround;
    bool grounded;

    public Transform orientation;

    float horizontalInput;
    float verticalInput;

    Vector3 moveDirection;

    Rigidbody rb;

    public CinemachineFreeLook cinemachineFreeLookCamera;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;

        readyToJump = true;

        L_hand.SetActive(false);
        R_hand.SetActive(false);

        view = GetComponent<PhotonView>();

        hpText = GameObject.Find("Player HP").GetComponent<Text>();

        if (view.IsMine)
        {
            if (cinemachineFreeLookCamera != null)
            {
                cinemachineFreeLookCamera.gameObject.SetActive(true);
                cinemachineFreeLookCamera.Follow = transform;
                cinemachineFreeLookCamera.LookAt = transform;
            }
        }
        else
        {
            if (cinemachineFreeLookCamera != null)
            {
                cinemachineFreeLookCamera.gameObject.SetActive(false);
            }
        }
    }

    private void Update()
    {
        if (view.IsMine)
        {
            hpText.text = hp.ToString();

            MyInput();
            HandleRotation();
        }
    }

    private void FixedUpdate()
    {
        if (view.IsMine)
        {
            grounded = Physics.Raycast(transform.position, Vector3.down, playerHeight * 0.5f + 0.2f, WhatIsGround);

            SpeedControl();

            if (grounded)
            {
                rb.drag = groundDrag;
            }
            else
            {
                rb.drag = 0;
            }

            if (Input.GetMouseButtonDown(0))
            {
                L_hand.SetActive(true);
                R_hand.SetActive(true);

                if (!isAttacking)
                {
                    isAttacking = true;
                    anim.SetBool("attack", true);
                    StartCoroutine(ResetAttack());
                }
                else if (isAttacking && !attackTriggered)
                {
                    attackTriggered = true;
                    anim.SetBool("attack2", true);
                }
            }

            MovePlayer();
        }
    }

    private void MyInput()
    {
        horizontalInput = Input.GetAxisRaw("Horizontal");
        verticalInput = Input.GetAxisRaw("Vertical");

        isRunning = Input.GetKey(KeyCode.LeftShift) && (Input.GetAxis("Vertical") != 0 || Input.GetAxis("Horizontal") != 0);// Input.GetKey(KeyCode.W);

        if (Input.GetKey(jumpKey) && readyToJump && grounded)
        {
            readyToJump = false;

            Jump();

            Invoke(nameof(ResetJump), jumpCooldown);
        }
    }

    private void HandleRotation()
    {
        float mouseX = Input.GetAxis("Mouse X") * 10f * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * 10f * Time.deltaTime;

        Vector3 rotation = transform.localRotation.eulerAngles;
        float desiredYAngle = rotation.y + mouseX;
        float desiredXAngle = rotation.x - mouseY;

        transform.localRotation = Quaternion.Euler(desiredXAngle, desiredYAngle, 0);
    }

    private void MovePlayer()
    {
        moveDirection = orientation.forward * verticalInput + orientation.right * horizontalInput;

        float currentSpeed = isRunning ? runSpeed : moveSpeed;

        if (grounded)
        {
            rb.AddForce(moveDirection.normalized * currentSpeed * 10f, ForceMode.Force);
        }
        else if (!grounded)
        {
            rb.AddForce(moveDirection.normalized * currentSpeed * 10f * airMultiplier, ForceMode.Force);
        }

        if(moveDirection != Vector3.zero)
        {
            anim.SetBool("walk", true);
            anim.SetBool("run", isRunning);
        }
        else
        {
            anim.SetBool("walk", false);
            anim.SetBool("run", false);
        }
    }

    private IEnumerator ResetAttack()
    {
        yield return new WaitForSeconds(attackTime);

        if (attackTriggered)
        {
            anim.SetBool("attack2", false);
        }
        else
        {
            anim.SetBool("attack", false);
        }

        anim.SetBool("attack", false);
        anim.SetBool("attack2", false);

        L_hand.SetActive(false);
        R_hand.SetActive(false);

        isAttacking = false;
        attackTriggered = false;
    }

    private void SpeedControl()
    {
        Vector3 flatVel = new Vector3(rb.velocity.x, 0f, rb.velocity.z);

        if(flatVel.magnitude > moveSpeed)
        {
            Vector3 limitedVel = flatVel.normalized * moveSpeed;
            rb.velocity = new Vector3(limitedVel.x, rb.velocity.y, limitedVel.z);
        }
    }

    private void Jump()
    {
        rb.velocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z);

        rb.AddForce(transform.up * jumpFroce, ForceMode.Impulse);

        anim.SetBool("jump", true);

        Debug.Log("jumping");
    }

    private void ResetJump()
    {
        readyToJump = true;
        anim.SetBool("jump", false);
    }
}