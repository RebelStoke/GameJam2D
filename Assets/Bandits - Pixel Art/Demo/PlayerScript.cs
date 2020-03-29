using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class PlayerScript : MonoBehaviour
{

    [SerializeField] float m_speed = 1.0f;
    [SerializeField] float m_jumpForce = 2.0f;

    private Animator m_animator;
    private Rigidbody2D m_body2d;
    private Sensor_Bandit m_groundSensor;
    private bool m_grounded = false;
    private bool m_combatIdle = false;
    private bool m_isDead = false;
    private int cash = 0;
    private int health = 3;
    private float attackTimer = 0.5f;
    public bool attacking = false;

    // Use this for initialization
    void Start()
    {
        m_animator = GetComponent<Animator>();
        m_body2d = GetComponent<Rigidbody2D>();
        m_groundSensor = transform.Find("GroundSensor").GetComponent<Sensor_Bandit>();
    }

    void OnCollisionEnter2D(Collision2D col)
    {
        if (col.gameObject.tag == "Coin")
        {
            col.gameObject.GetComponent<CoinScript>().DestroyCoin();
            cash++;
            UpdateCash();
        }
        if (col.gameObject.tag == "Enemy" && col.gameObject.GetComponent<Bandit>().attacking) {
            TakeDamage();
        }
    }

    void Attack()
    {
        m_animator.SetTrigger("Attack");
        attacking = true;
        attackTimer = 0.5f;
    }

    void UpdateCash() {
       GameObject text = GameObject.Find("CoinCount");
        text.GetComponent<Text>().text = cash + "";

    }

    void UpdateHealth()
    {
        if (health <= 0)
            Death();

        GameObject text = GameObject.Find("LifeCount");
        text.GetComponent<Text>().text = health + "";

    }

    public void TakeDamage() {
        if (!m_isDead) {
        health--;
        UpdateHealth();
        m_animator.SetTrigger("Hurt");
        }
        
    }

    void Death() {
        {
            if (!m_isDead)
                m_animator.SetTrigger("Death");

            m_isDead = !m_isDead;
        }
    }

    void Update()
    {

        if (attacking) {
            if (attackTimer > 0)
            {
                attackTimer -= Time.deltaTime;
            }
            else
                attacking = false;
        }
        float inputX = 0;
        //Check if character just landed on the ground
        if (!m_grounded && m_groundSensor.State())
        {
            m_grounded = true;
            m_animator.SetBool("Grounded", m_grounded);
        }

        //Check if character just started falling
        if (m_grounded && !m_groundSensor.State())
        {
            m_grounded = false;
            m_animator.SetBool("Grounded", m_grounded);
        }

        if (!m_isDead) {
             inputX = Input.GetAxis("Horizontal");
        }
        // Swap direction of sprite depending on walk direction
        if (inputX > 0)
            GetComponent<SpriteRenderer>().flipX = true;
        else if (inputX < 0)
            GetComponent<SpriteRenderer>().flipX = false;

        // Move
        m_body2d.velocity = new Vector2(inputX * m_speed, m_body2d.velocity.y);

        //Set AirSpeed in animator
        m_animator.SetFloat("AirSpeed", m_body2d.velocity.y);

        // -- Handle Animations --
        //Death
        if (Input.GetKeyDown("e"))
        {
            if (!m_isDead)
                m_animator.SetTrigger("Death");
            else
                m_animator.SetTrigger("Recover");

            m_isDead = !m_isDead;
        }

        //Hurt
        else if (Input.GetKeyDown("q"))
            m_animator.SetTrigger("Hurt");

        //Attack
        else if (Input.GetMouseButtonDown(0) && !attacking)
        {
            Attack();
        }

        //Change between idle and combat idle
        else if (Input.GetKeyDown("f"))
            m_combatIdle = !m_combatIdle;

        //Jump
        else if (Input.GetKeyDown("space") && m_grounded)
        {
            m_animator.SetTrigger("Jump");
            m_grounded = false;
            m_animator.SetBool("Grounded", m_grounded);
            m_body2d.velocity = new Vector2(m_body2d.velocity.x, m_jumpForce);
            m_groundSensor.Disable(0.2f);
        }

        //Run
        else if (Mathf.Abs(inputX) > Mathf.Epsilon)
            m_animator.SetInteger("AnimState", 2);

        //Combat Idle
        else if (m_combatIdle)
            m_animator.SetInteger("AnimState", 1);

        //Idle
        else
            m_animator.SetInteger("AnimState", 0);
    }
}