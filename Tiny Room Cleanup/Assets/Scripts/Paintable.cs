using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class Paintable : MonoBehaviour
{
    public GameObject brush;
    public float brushSize = 0.1f;

    public RenderTexture texture;

    public List<GameObject> objectPool;

    Renderer rend;
    void Start()
    {
        rend = GetComponent<Renderer>();
        objectPool = new List<GameObject>();

    }

    // Update is called once per frame
    void Update()
    {

        if (Input.GetMouseButton(0))
        {
            RaycastHit hit;
            Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit);
            if (hit.collider != null)
            {
                GameObject go = Instantiate(brush, hit.point + Vector3.up * 0.1f, Quaternion.identity, transform);
                // we added Vector3.up to our hitpoint so it won't contact with ground
                objectPool.Add(go);
                CheckPool();
                go.transform.localScale = Vector3.one * brushSize;
            }


        }
    }

    private void CheckPool()
    {
        if (objectPool.Count >= 100)
        {
            Save();
            foreach (GameObject obj in objectPool)
            {
                Destroy(obj);
            }
            objectPool.Clear();
        }
    }

    private void Save()
    {
        RenderTexture.active = texture;

        var Texture2D = new Texture2D(texture.width, texture.height, TextureFormat.RGBA32, false);
        
        Texture2D.ReadPixels(new Rect(0, 0, texture.width, texture.height), 0, 0);
    
        Texture2D.Apply();

        var data = Texture2D.EncodeToPNG();
        File.WriteAllBytes(Application.dataPath + "/" + transform.name + ".png", data);
        rend.material.mainTexture = Texture2D;
    }

}
