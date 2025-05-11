using UnityEngine;

public class SlashAnimation : MonoBehaviour
{
    private ParticleSystem ps;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void Awake()
    {
        ps = GetComponent<ParticleSystem>();
    }

    // Update is called once per frame
    void Update()
    {
        if(ps && !ps.IsAlive())
        {
            Destroy(gameObject);
        }
    }
   
}
