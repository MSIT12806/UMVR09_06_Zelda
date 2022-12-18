using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//�D�B��ɤ]Ĳ�o�ĪG
[ExecuteInEditMode]

//�ù���B�z�S�ĸj�b��v���W
[RequireComponent(typeof(Camera))]
public class PostEffectBase : MonoBehaviour
{
    public Shader Shader = null;
    private Material _material = null;
    public Material _Material 
    {
        get
        {
            if (_material == null)
                _material = GenerateMaterial(Shader);
            return _material;
        }
    }

    protected Material GenerateMaterial(Shader shader) 
    {
        if (shader == null)
            return null;
        if (shader.isSupported == false)
            return null;
        Material material = new Material(shader);
        material.hideFlags = HideFlags.DontSave;
        if (material)
            return material;
        return null;
    }

}
