using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//using DG.Tweening;

public class Movement : MonoBehaviour
{
    private Collision coll;
    [HideInInspector]
    public Rigidbody2D rb;
    //private AnimationScript anim;

    [Space]
    [Header("Stats")]
    public float speed = 10;
    public float jumpForce = 50;
    public float dashSpeed = 20;
    public float fallMultiplier = 1.5f;

    [Space]
    [Header("Booleans")]
    public bool canMove = true;
    public bool wallGrab;
    public bool isDashing;

    [Space]

    private bool groundTouch;
    public bool betterJumping = true;
    public bool hasDashed;

    public int side = 1;

    [Space]
    [Header("Polish")]
    public ParticleSystem dashParticle;
    public ParticleSystem jumpParticle;

    // Start is called before the first frame update
    void Start()
    {
        coll = GetComponent<Collision>();
        rb = GetComponent<Rigidbody2D>();
        //anim = GetComponentInChildren<AnimationScript>();
    }

    // Update is called once per frame
    [Obsolete]
    void Update()
    {
        float x = Input.GetAxis("Horizontal");
        float y = Input.GetAxis("Vertical");
        float xRaw = Input.GetAxisRaw("Horizontal");
        float yRaw = Input.GetAxisRaw("Vertical");
        Vector2 dir = new(x, y);

        Walk(dir);
        //anim.SetHorizontalMovement(x, y, rb.velocity.y);

        if (coll.onWall && Input.GetKey(KeyCode.Tab) && canMove && coll.onGround)
        {
            //if(side != coll.wallSide)
                //anim.Flip(side*-1);
            wallGrab = true;
        }

        if (Input.GetKeyUp(KeyCode.Tab) || !coll.onWall || !canMove)
        {
            wallGrab = false;
        }

        if (coll.onGround && !isDashing)
        {
            betterJumping = true;
        }
        
        if (wallGrab && !isDashing)
        {
            if(x > .2f || x < -.2f)
            rb.velocity = new Vector2(rb.velocity.x, 0);
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            //anim.SetTrigger("jump");

            if (coll.onGround && !wallGrab)
                Jump(Vector2.up);
        }

        if (Input.GetKeyDown(KeyCode.Q) && !hasDashed)
        {
            if(xRaw != 0 || yRaw != 0)
                Dash(xRaw, yRaw);
        }

        if (coll.onGround && !groundTouch)
        {
            GroundTouch();
            groundTouch = true;
        }

        if(!coll.onGround && groundTouch)
        {
            groundTouch = false;
        }

        if (wallGrab || !canMove)
            return;

        if(x > 0)
        {
            side = 1;
            //anim.Flip(side);
        }
        if (x < 0)
        {
            side = -1;
            //anim.Flip(side);
        }
        
        if(betterJumping){
            if(rb.velocity.y < 0){
                rb.velocity += fallMultiplier * Physics2D.gravity.y * Time.deltaTime * Vector2.up;
            }else if(rb.velocity.y > 0 && !Input.GetKey(KeyCode.Space)){
                rb.velocity += Physics2D.gravity.y * Time.deltaTime * Vector2.up;
            }
        }
        


    }

    void GroundTouch()
    {
        hasDashed = false;
        isDashing = false;

        //side = anim.sr.flipX ? -1 : 1;

        jumpParticle.Play();
    }

    private void Dash(float x, float y)
    {
        //Camera.main.transform.DOComplete();
        //Camera.main.transform.DOShakePosition(.2f, .5f, 14, 90, false, true);
        //GetComponent<RippleEffect>().Emit(Camera.main.WorldToViewportPoint(transform.position));

        hasDashed = true;

        //anim.SetTrigger("dash");

        rb.linearVelocity = Vector2.zero;
        Vector2 dir = new(x, y);

        rb.linearVelocity += dir.normalized * dashSpeed;
        StartCoroutine(DashWait());
    }

    IEnumerator DashWait()
    {
        StartCoroutine(GroundDash());
        //DOVirtual.Float(14, 0, .8f, RigidbodyDrag);

        //dashParticle.Play();
        rb.gravityScale = 0;
        betterJumping = false;
        isDashing = true;

        yield return new WaitForSeconds(.3f);

        //dashParticle.Stop();
        rb.gravityScale = 3;
        betterJumping = true;
        isDashing = false;
    }

    IEnumerator GroundDash()
    {
        yield return new WaitForSeconds(.15f);
        if (coll.onGround)
            hasDashed = false;
    }

    private void Walk(Vector2 dir)
    {
        if (!canMove)
            return;

        else
        {
            if(wallGrab)
                rb.linearVelocity = Vector2.Lerp(rb.linearVelocity, new Vector2(dir.x * speed/2, rb.linearVelocity.y), 10 * Time.deltaTime);
            else
                rb.linearVelocity = Vector2.Lerp(rb.linearVelocity, new Vector2(dir.x * speed, rb.linearVelocity.y), 10 * Time.deltaTime);
        }
    }

    private void Jump(Vector2 dir)
    {

        rb.linearVelocity = new Vector2(rb.linearVelocity.x, 0);
        rb.linearVelocity += dir * jumpForce;

        jumpParticle.Play();
    }

    IEnumerator DisableMovement(float time)
    {
        canMove = false;
        yield return new WaitForSeconds(time);
        canMove = true;
    }

   
    void RigidbodyDrag(float x)
    {
        rb.linearDamping = x;
    }

}