using UnityEngine;

public class spawner : MonoBehaviour
{

    public float maxX = 0;
    public float minX = 0;
    public GameObject[] slime_types;
    public float deltaFreshTime;
    float deltaTimebtwFleh = 0;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

        if (deltaTimebtwFleh >= deltaFreshTime)
        {

            deltaTimebtwFleh = 0;
            Instantiate(slime_types[Random.Range(0, 2)],
                        new Vector3(Random.Range(minX, maxX), 3, 0),
                        Quaternion.identity
                        );
        }else deltaTimebtwFleh += Time.deltaTime;
    }
}
