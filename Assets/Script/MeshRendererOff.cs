using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeshRendererOff : MonoBehaviour
{
    [SerializeField] MeshRenderer _meshRenderer;

    void Start()
    {
        if(_meshRenderer == null)
        {
            _meshRenderer = GetComponent<MeshRenderer>();
        }

        _meshRenderer.enabled = false;
    }
}
