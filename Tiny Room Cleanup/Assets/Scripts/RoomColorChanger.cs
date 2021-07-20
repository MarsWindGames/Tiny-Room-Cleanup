using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomColorChanger : MonoBehaviour
{

    public GameObject groundObject;

    Material instantiatedMatWall;
    Material objectMatWall;

    Renderer rendWall;
    Material newMatWall = null;

    public Color[] colors;

    private void Start()
    {
        rendWall = GetComponent<Renderer>();
        objectMatWall = rendWall.material;
        SetWallColor();
    }

    private void OnEnable()
    {
        SetWallColor();
    }

    public void SetWallColor()
    {
        if (newMatWall == null && objectMatWall != null)
        {
            newMatWall = Instantiate(objectMatWall);
        }

        if (newMatWall != null)
        {
            newMatWall.color = colors[Random.Range(0, colors.Length)];
            rendWall.material = newMatWall;
        }

    }

}
