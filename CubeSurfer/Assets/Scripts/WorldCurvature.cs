using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldCurvature : MonoBehaviour
{
    [Range(-0.015f, 0.015f)]
    public float Curvature;

    [Range(-0.015f, 0.015f)]
    public float Turn;

    [Range(-0.015f, 0.015f)]
    public float Rotation;


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
        transform.Translate(Vector3.forward * 3 * Time.deltaTime);
    }
}
