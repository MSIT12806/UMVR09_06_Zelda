using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GhostEffect : MonoBehaviour
{
    [Header("openGhoseEffect")] //是否開啟殘影
    public bool openGhoseEffect;

    private SkinnedMeshRenderer smr;//SkinnedMeshRenderer

    private List<Ghost> ghostList = new List<Ghost>();//殘影列表

    [Header("durationTime")] //顯示殘影的持續時間
    public float durationTime;
    [Header("spawnTimeval")] //生成殘影與殘影之間的時間間隔
    public float spawnTimeval;
    private float spawnTimer;//生成殘影的時間計時器

    [Header("ghostColor")] //殘影顏色
    public Color ghostColor;

    public Material GhostMaterial; //殘影材質球

    public enum RenderingMode 
    {
        Opaque,
        Cutout,
        Fade,
        Transparent,
    }

    private void Awake()
    {
        smr = GetComponent<SkinnedMeshRenderer>();
    }

    private void Update()
    {
        if (openGhoseEffect == false)
        {
            return;
        }

        CreateGhost();
        DrawGhost();
    }

    /// <summary>
    /// 創建殘影
    /// </summary>
    private void CreateGhost()
    {
        //創建出殘影並加入到殘影列表中
        if (spawnTimer >= spawnTimeval)
        {
            spawnTimer = 0;

            Mesh mesh = new Mesh();
            smr.BakeMesh(mesh);
            Material mat = new Material(GhostMaterial);
            mat.color = ghostColor;
            SetMaterialRenderingMode(mat, RenderingMode.Fade);

            ghostList.Add(new Ghost(mesh, mat, transform.localToWorldMatrix, Time.realtimeSinceStartup));
        }
        else
        {
            spawnTimer += Time.deltaTime;
        }
    }

    /// <summary>
    /// 繪製殘影
    /// </summary>
    private void DrawGhost()
    {
        for (int i = 0; i < ghostList.Count; i++)
        {
            float time = Time.realtimeSinceStartup - ghostList[i].beginTime;
            if (time >= durationTime)
            {
                Ghost _ghost = ghostList[i];
                ghostList.Remove(_ghost);
                Destroy(_ghost);
            }
            else
            {
                //褪色
                float fadePerSecond = (ghostList[i].mat.color.a / durationTime);
                Color tempColor = ghostList[i].mat.color;
                tempColor.a -= fadePerSecond * Time.deltaTime;
                ghostList[i].mat.color = tempColor;

                Graphics.DrawMesh(ghostList[i].mesh, ghostList[i].matrix, ghostList[i].mat, gameObject.layer);
            }
        }
    }

    /// <summary>
    /// 設置紋理渲染模式
    /// </summary>
    private void SetMaterialRenderingMode(Material material, RenderingMode renderingMode)
    {
        switch (renderingMode)
        {
            case RenderingMode.Opaque:
                material.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.One);
                material.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.Zero);
                material.SetInt("_ZWrite", 1);
                material.DisableKeyword("_ALPHATEST_ON");
                material.DisableKeyword("_ALPHABLEND_ON");
                material.DisableKeyword("_ALPHAPREMULTIPLY_ON");
                material.renderQueue = -1;
                break;
            case RenderingMode.Cutout:
                material.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.One);
                material.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.Zero);
                material.SetInt("_ZWrite", 1);
                material.EnableKeyword("_ALPHATEST_ON");
                material.DisableKeyword("_ALPHABLEND_ON");
                material.DisableKeyword("_ALPHAPREMULTIPLY_ON");
                material.renderQueue = 2450;
                break;
            case RenderingMode.Fade:
                material.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
                material.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
                material.SetInt("_ZWrite", 0);
                material.DisableKeyword("_ALPHATEST_ON");
                material.EnableKeyword("_ALPHABLEND_ON");
                material.DisableKeyword("_ALPHAPREMULTIPLY_ON");
                material.renderQueue = 3000;
                break;
            case RenderingMode.Transparent:
                material.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.One);
                material.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
                material.SetInt("_ZWrite", 0);
                material.DisableKeyword("_ALPHATEST_ON");
                material.DisableKeyword("_ALPHABLEND_ON");
                material.EnableKeyword("_ALPHAPREMULTIPLY_ON");
                material.renderQueue = 3000;
                break;
        }
    }
}

/// <summary>
/// 每一個殘影的數據類
/// </summary>
public class Ghost : Object
{
    public Mesh mesh;
    public Material mat;
    public Matrix4x4 matrix;
    public float beginTime;

    public Ghost(Mesh _mesh, Material _mat, Matrix4x4 _matrix, float _beginTime)
    {
        mesh = _mesh;
        mat = _mat;
        matrix = _matrix;
        beginTime = _beginTime;
    }
}
