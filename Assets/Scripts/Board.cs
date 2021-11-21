using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Board : MonoBehaviour
{
    private Vector3 size;
    public Vector2 startSize;
    float timer = 0.0f;
    Vector3 pos;

    void Awake()
    {
        SetSize(startSize);
    }


    public void SetSize(Vector2 size)
    {
        if(size.x > 0.0f && size.x < 50.0f && size.y > 0.0f && size.y < 50.0f)
        {
            this.size = new Vector3(size.x,size.y,1.0f);
            UpdateBoardSize();
        }
        else
        {
            Debug.LogWarning("Incorrect size value.");
        }
    }

    void UpdateBoardSize()
    {
        gameObject.transform.localScale = size;
    }

    public Vector3 GetRandomBoundryPosition()
    {
        Vector3 position = Vector3.zero;
        float circum = Random.Range(0, 2*(size.x + size.y));
        if(circum < size.x)
        {
            position.x = circum - size.x/2;
            position.z = size.y / 2;
        }
        else if(circum < size.y + size.x)
        {
            circum -= size.x;
            position.z = circum - size.y / 2;
            position.x = size.x / 2;
        }
        else if (circum < 2 * size.x + size.y)
        {
            circum -= size.y + size.x;
            position.x = - circum + size.x / 2;
            position.z = - size.y / 2;
        }
        else
        {
            circum -= 2 * size.x + size.y;
            position.z = - circum + size.y / 2;
            position.x = - size.x / 2;
        }
        return position;
    }

    public Vector3 getRandomFieldPosition()
    {
        return new Vector3(Random.Range(-size.x / 2, size.x/2), 0.0f, Random.Range(-size.y / 2, size.y/2));
    }

    void Update()
    {

        /*
        timer += Time.deltaTime;
        if(timer>1)
        {
            pos = GetRandomBoundryPosition();
            timer = 0;
            Debug.Log("T");
        }
        Debug.DrawLine(Vector3.zero, pos);
        */
    }
}
