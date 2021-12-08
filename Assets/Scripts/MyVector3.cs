using UnityEngine;

[System.Serializable]
public struct MyVector3
{
    public float x;
    public float y;
    public float z;

    public MyVector3(float x, float y, float z)
    {
        this.x = x;
        this.y = y;
        this.z = z;
    }

    public MyVector3(Vector3 v)
    {
        this.x = v.x;
        this.y = v.y;
        this.z = v.z;
    }

    public static implicit operator Vector3(MyVector3 v) => new Vector3(v.x,v.y,v.z);
    public static implicit operator MyVector3(Vector3 v) => new MyVector3(v.x, v.y, v.z);
}
// Potrzbene do zapisywania do pliku ze wzglêdu na charakterystykê silnika.
