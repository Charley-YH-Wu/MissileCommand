<p>Design Path</p>

<p>Added a pre-determined bomb sequence and implemented increasing diffculty in 30s. Here are the details: </p>
<ul>
  <li>Speed of bomb = m_minSpeed + 1.5 * (Time.realtimeSinceStartup%30/30)</li>
  <li>Bomb delay = 3 - 2.9*(Time.realtimeSinceStartup%30/30)</li>
  <li>Starting x location of bomb = {0.2f, 0.6f, 0.8f, 0.5f, 0.4f, 0.1f, 0.6f, 0.1f, 0.3f, 0.7f, 0.8f, 0.1f}</li>
  <li>Target city = {0,1,2,2,2,0,1,2,0,0,1,1,1,0,2,1,2,1,2,2}</li>
</ul>
