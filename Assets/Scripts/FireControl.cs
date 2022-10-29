using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class FireControl : MonoBehaviour
{
    public GameObject m_crossHair;
    public GameObject m_missile;
    public GameObject m_bomb;
    public GameObject m_city;
    public int m_numCity = 3;
    public float m_bombMinTime = 0.25f;
    public float m_bombMaxTime = 1.0f;
    public GameObject m_pauseMenu;

    List<GameObject> m_targets;     // a list of all the targeting reticles (one for each finger that's touching the screen)
    List<City> m_cities;

    bool m_isPaused = false;

    // TODO create class MissileInput
    public class MissleInput{
        public List<Vector3> aim_pos;
        public List<Vector3> fire_pos; 
        public MissleInput(){
            aim_pos = new List<Vector3>();
            fire_pos = new List<Vector3>();
        }
    }

    private void Start()
    {
        m_targets = new List<GameObject>();
        m_cities = new List<City>();
        for (int i = 0; i < m_numCity; ++i)
        {
            GameObject obj = Instantiate(m_city);
            obj.name = "City_" + i.ToString();
            float x = 0.8f * i / m_numCity + 0.2f;
            Vector3 pos = new Vector3(x, 0.0f, 0.0f);
            pos = Camera.main.ViewportToScreenPoint(pos);
            pos = Utility.ScreenToWorldPos(pos);
            obj.transform.position = pos;
            m_cities.Add(obj.GetComponent<City>());
        }
        SetPause(false);
        StartCoroutine(DropBomb());
    }

    void Update()
    {
        if (false == m_isPaused)
        {
            // TODO Read the input
            MissleInput m = ReadInput();

            // TODO Fire Missiles
            while (m.fire_pos.Count != 0){
                FireMissile(m.fire_pos[0]);
                m.fire_pos.RemoveAt(0);
            }
            // for (int i = 0; i < m.fire_pos.Count; i++){
            //     FireMissile(m.fire_pos[i]);
            // }

            // TODO Make enough target reticles
            // Delete any extra target reticles
            while (m_targets.Count < m.aim_pos.Count){
                GameObject cross = Instantiate(m_crossHair);
                m_targets.Add(cross);
            }
            while (m_targets.Count > m.aim_pos.Count){
                Destroy(m_targets[m_targets.Count-1]);
                m_targets.RemoveAt(m_targets.Count-1);
            }

            // TODO Update the position of all the target reticles
            for (int i = 0; i < m.aim_pos.Count; i++){
                m_targets[i].transform.position = m.aim_pos[i];
            }
        }

        // keys
        if (Input.GetKeyDown(KeyCode.Escape))
        {   // this doubles as the option key in the android navigation bar
            SetPause(!m_isPaused);
        }
    }

    MissleInput ReadInput(){
        MissleInput mi = new MissleInput();

        // mouse emulation
        // if (Input.GetMouseButtonDown(0) || Input.GetMouseButton(0)){
        //     Vector3 temp = Utility.ScreenToWorldPos(Input.mousePosition);
        //     if (mi.aim_pos.Count > 0){
        //         mi.aim_pos[0] = temp;
        //     }
        //     else{
        //         mi.aim_pos.Add(temp);
        //     }
        // }

        // if (Input.GetMouseButtonUp(0)){
        //     mi.fire_pos.Add(Utility.ScreenToWorldPos(Input.mousePosition));
        //     mi.aim_pos.Remove(Input.mousePosition);
        // }

        // touch emulation
        Touch[] touches = Input.touches;
        for (int i = 0; i < touches.Length; i++){
            if (touches[i].phase == TouchPhase.Began){
                mi.aim_pos.Add(Utility.ScreenToWorldPos(touches[i].position));
                //Debug.Log("touch began" + touches.Length.ToString());
                //Debug.Log("Rectiles: " + m_targets.Count.ToString());

            }
            else if (touches[i].phase == TouchPhase.Ended){
                mi.fire_pos.Add(Utility.ScreenToWorldPos(touches[i].position));
                mi.aim_pos.Remove(Utility.ScreenToWorldPos(touches[i].position));
                //Debug.Log("touch ended");
            }
            else if (touches[i].phase == TouchPhase.Moved){
                mi.aim_pos.Add(Utility.ScreenToWorldPos(touches[i].position));
                //Debug.Log("touch moved" + mi.aim_pos.Count.ToString());
                //Debug.Log("Rectiles: " + m_targets.Count.ToString());

            }
        }
        return mi;
    }

    void FireMissile(Vector3 targetPos)
    {
        City closest = null;
        float bestDist = float.MaxValue;
        foreach (City city in m_cities)
        {
            if (city.CanLaunch())
            {
                float dist = Vector3.Distance(targetPos, city.transform.position);
                if (dist < bestDist)
                {
                    bestDist = dist;
                    closest = city;
                }
            }
        }

        if (null != closest)
        {
            GameObject gameObject = Instantiate(m_missile);
            gameObject.transform.position = closest.GetLaunchPos();
            Missile missile = gameObject.GetComponent<Missile>();
            if (null != missile)
                missile.Fire(targetPos);
        }
    }

    IEnumerator DropBomb()
    {
        int numCity = m_cities.Count;

        float[] bomb_seq = {0.2f, 0.6f, 0.8f, 0.5f, 0.4f, 0.1f, 0.6f, 0.1f, 0.3f, 0.7f, 0.8f, 0.1f};
        int num = 0;
        while (numCity > 0)
        {
            // frequency sequence
            // float delay = Random.Range(m_bombMinTime, m_bombMaxTime);
            float delay = (float)(3 - 2.9*(Time.realtimeSinceStartup%30/30));


            yield return new WaitForSeconds(delay);
            if (null != m_bomb)
            {
                // bomb starting location sequence 
                // Vector3 pos = new Vector3(Random.Range(0.2f, 0.8f), 1.0f, 0.0f);
                if (num >= bomb_seq.Length){num = 0;}
                Vector3 pos = new Vector3(bomb_seq[num], 1.0f, 0.0f);
                num++;

                pos = Camera.main.ViewportToScreenPoint(pos);
                pos = Utility.ScreenToWorldPos(pos);
                GameObject bomb = Instantiate(m_bomb);
                bomb.transform.position = pos;
            }
            numCity = 0;
            foreach (City city in m_cities)
            {
                if (city.IsAlive())
                    ++numCity;
            }
        }

        // We've run out of cities
        // Wait for the bombs to run out
        while (null != FindObjectOfType<Bomb>())
            yield return null;

        StartCoroutine(GameOver());
    }

    IEnumerator GameOver()
    {
        // wait 3 seconds
        yield return new WaitForSecondsRealtime(3.0f);
        // and reload the scene
        SceneManager.LoadScene(0);
    }

    public void SetPause(bool setPause)
    {
        if (setPause)
        {
            Time.timeScale = 0.0f;
        }
        else
        {
            Time.timeScale = 1.0f;
        }
        m_pauseMenu.SetActive(setPause);
        m_isPaused = setPause;
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    // TODO Create function ReadInput()
}
