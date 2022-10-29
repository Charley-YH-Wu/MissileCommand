using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MissileExplosion : MonoBehaviour
{
    public float m_radius = 1.0f;
    public float m_time = 0.75f;

    float m_timer = 0.0f;

    void Update()
    {
        float dt = Time.deltaTime;
        
        m_timer += dt;
        float radius = m_radius * m_timer / m_time;
        if (transform.childCount > 0)
        {
            Transform child = transform.GetChild(0);
            child.localScale = 2.0f * radius / transform.lossyScale.x * Vector3.one;
        }

        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, radius);
        foreach (var hit in hits)
        {
            Bomb bomb = hit.GetComponent<Bomb>();
            if (null != bomb)
            {
                bomb.Intercept();
            }
        }

        if (m_timer >= m_time)
        {
            enabled = false;
        }
    }
}
