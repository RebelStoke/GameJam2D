using UnityEngine;
using System.Collections;

public class Bandit : MonoBehaviour {

    [SerializeField] float      m_speed = 1.0f;
    [SerializeField] float      m_jumpForce = 2.0f;

    private Animator            m_animator;
    private Rigidbody2D         m_body2d;
    private Sensor_Bandit       m_groundSensor;
    private bool                m_grounded = false;
    private bool                m_combatIdle = false;
    private bool                m_isDead = false;
    public bool attacking = false;
    public float attackTimer = 0.5f;

    // Use this for initialization
    void Start () {
        m_animator = GetComponent<Animator>();
        m_body2d = GetComponent<Rigidbody2D>();
        m_groundSensor = transform.Find("GroundSensor").GetComponent<Sensor_Bandit>();
    }

	void OnCollisionEnter2D(Collision2D col)
    {
        if(col.gameObject.tag == "Player" )
 	{
            if (!m_isDead) { 
                Attack();
            }
            
            if (col.gameObject.GetComponent<PlayerScript>().attacking) {
                Death();
            };

	 }
    }

    void Jump()
    {
        m_animator.SetTrigger("Jump");
        m_grounded = false;
        m_animator.SetBool("Grounded", m_grounded);
        m_body2d.velocity = new Vector2(m_body2d.velocity.x, m_jumpForce);
        m_groundSensor.Disable(0.2f);
    }

     void Death()
    {
        if (!m_isDead)
            m_animator.SetTrigger("Death");
        m_isDead = !m_isDead;
    }

     void CombatIdle()
    {
        m_combatIdle = !m_combatIdle;
    }

     void Hurt() { 
        m_animator.SetTrigger("Hurt"); 
    }

   void Attack()
    {
            m_animator.SetTrigger("Attack");
            attacking = true;
            attackTimer = 0.5f;
    }
    

    void Update () {

        if (attacking)
        {
            if (attackTimer > 0)
            {
                attackTimer -= Time.deltaTime;
            }
            else
                attacking = false;
        }
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

        //Set AirSpeed in animator
        m_animator.SetFloat("AirSpeed", m_body2d.velocity.y);

     

        //Combat Idle
       if (m_combatIdle)
            m_animator.SetInteger("AnimState", 1);

        //Idle
        else
            m_animator.SetInteger("AnimState", 0);
    }
}

