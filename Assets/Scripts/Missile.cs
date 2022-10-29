using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Missile : MonoBehaviour
{
    public float m_speed = 10.0f;
    public GameObject m_explosion;

    Vector3 m_targetPos;

    public void Fire(Vector3 targetPos)
    {
        m_targetPos = targetPos;
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 pos = transform.position;
        Vector3 vel = m_targetPos - pos;
        float len = vel.magnitude;
        if (len <= m_speed * Time.deltaTime)
        {   // explode
            if (null != m_explosion)
            {
                GameObject explode = Instantiate(m_explosion);
                explode.transform.position = m_targetPos;
            }
            Destroy(gameObject);
        }
        else
        {
            vel = vel * m_speed / len;
            pos += vel * Time.deltaTime;
            transform.position = pos;
            float ang = Mathf.Rad2Deg * Mathf.Atan2(vel.y, vel.x);
            transform.localEulerAngles = new Vector3(0.0f, 0.0f, ang);
        }
    }
}
