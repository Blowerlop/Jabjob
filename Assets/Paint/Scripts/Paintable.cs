using UnityEngine;

public class Paintable : MonoBehaviour {
    const int TEXTURE_SIZE = 1048;

    public float extendsIslandOffset = 1;

    RenderTexture extendIslandsRenderTexture;
    RenderTexture uvIslandsRenderTexture;
    RenderTexture maskRenderTexture;
    RenderTexture supportTexture;
    
    Renderer rend;

    int maskTextureID = Shader.PropertyToID("_MaskTexture");
    int alphaID = Shader.PropertyToID("_alpha");

    public RenderTexture getMask() => maskRenderTexture;
    public RenderTexture getUVIslands() => uvIslandsRenderTexture;
    public RenderTexture getExtend() => extendIslandsRenderTexture;
    public RenderTexture getSupport() => supportTexture;
    public Renderer getRenderer() => rend;

    void Start() {
        Initialize();
    }

    public void Initialize()
    {
        maskRenderTexture = new RenderTexture(TEXTURE_SIZE, TEXTURE_SIZE, 0);
        maskRenderTexture.filterMode = FilterMode.Bilinear;

        extendIslandsRenderTexture = new RenderTexture(TEXTURE_SIZE, TEXTURE_SIZE, 0);
        extendIslandsRenderTexture.filterMode = FilterMode.Bilinear;

        uvIslandsRenderTexture = new RenderTexture(TEXTURE_SIZE, TEXTURE_SIZE, 0);
        uvIslandsRenderTexture.filterMode = FilterMode.Bilinear;

        supportTexture = new RenderTexture(TEXTURE_SIZE, TEXTURE_SIZE, 0);
        supportTexture.filterMode = FilterMode.Bilinear;

        rend = GetComponent<Renderer>();
        rend.material.SetTexture(maskTextureID, extendIslandsRenderTexture);

        PaintManager.instance.initTextures(this);
    }

    void OnDisable(){
        if (maskRenderTexture != null) maskRenderTexture.Release();
        if (uvIslandsRenderTexture != null) uvIslandsRenderTexture.Release();
        if (extendIslandsRenderTexture != null) extendIslandsRenderTexture.Release();
        if (supportTexture != null) supportTexture.Release();
    }
    

    public void SetAlpha(float alpha)
    {
        //Debug.Log(GetComponent<SkinnedMeshRenderer>().material.GetFloat(alphaID));
        Material[] materials = GetComponent<Renderer>().materials;
        for(int i = 0; i < materials.Length; i++)
        {
            materials[i].SetFloat(alphaID, alpha);
        }
        //Debug.Log(GetComponent<SkinnedMeshRenderer>().material.GetFloat(alphaID));
    }
}