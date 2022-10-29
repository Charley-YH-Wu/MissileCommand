using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class City : MonoBehaviour
{
    public GameObject m_explosion;
    public Vector3 m_explosionOffset;
    public Vector3 m_launchOffset;

    Animator m_anim;
    bool m_isAlive = true;

    // Start is called before the first frame update
    void Start()
    {
        m_anim = GetComponent<Animator>();
    }

    public void Explode()
    {
        if (m_isAlive)
        {
            if (null != m_explosion)
            {
                GameObject gameObject = Instantiate(m_explosion);
                gameObject.transform.position = transform.position + m_explosionOffset;
            }
            if (null != m_anim)
            {
                m_anim.SetBool("IsDead", true);
            }
            m_isAlive = false;
        }
    }

    public void Restore()
    {
        if (null != m_anim)
        {
            m_anim.SetBool("IsDead", false);
        }
        m_isAlive = true;
    }

    public bool CanLaunch()
    {
        return m_isAlive;
    }

    public bool IsAlive()
    {
        return m_isAlive;
    }

    public Vector3 GetLaunchPos()
    {
        return transform.position + m_launchOffset;
    }

    public Vector3 GetTargetPos()
    {
        return transform.position + m_explosionOffset;
    }
}
