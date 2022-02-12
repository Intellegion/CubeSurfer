using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldCurvature : MonoBehaviour
{
    [Range(-0.02f, 0.02f)]
    public float Curvature;

    [Range(-0.02f, 0.02f)]
    public float Turn;

    public Material[] Materials;

    private void Start()
    {

    }
    private void OnValidate()
    {
        foreach(Material mat in Materials)
        {
            mat.SetFloat("_Curvature", Curvature);
            mat.SetFloat("_Turn", Turn);
        }
    }

    private void Update()
    {
        transform.Translate(Vector3.forward * 7 * Time.deltaTime, Space.World);
    }
}
