using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Drawmouse : MonoBehaviour
{
    public Camera _camera;
    public Shader _drawtrackshader;
    private RenderTexture _splatmap; 
    //splat is name for texture using to drive alpha blending between other textures so depending on how end up setting this up may not be accurate :)
        private Material _trackmaterial, _drawMaterial;
    private RaycastHit _hit;
    // Start is called before the first frame update
    void Start()
    {
        _drawMaterial = new Material (_drawtrackshader);
        _drawMaterial.SetVector("_Color", Color.white);

        _trackmaterial = GetComponent<MeshRenderer>().material;
        _splatmap = new RenderTexture(1024,1024,0,RenderTextureFormat.ARGBFloat);
        _trackmaterial.SetTexture("_albedo", _splatmap);      
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey(KeyCode.Mouse0))
        {
            if (Physics.Raycast(_camera.ScreenPointToRay(Input.mousePosition), out _hit))
            {
                _drawMaterial.SetVector("_Coordinate", new Vector4(_hit.textureCoord.x, _hit.textureCoord.y,0,0));
                RenderTexture temp = RenderTexture.GetTemporary(_splatmap.width, _splatmap.height,0,RenderTextureFormat.ARGBFloat);
                Graphics.Blit(_splatmap, temp);
                Graphics.Blit(temp, _splatmap, _drawMaterial);
                RenderTexture.ReleaseTemporary(temp);      
                //Blit name for bitwise texture operations in this case the textures we are updating
            }
    
        }
    }

    // Below is just to see if getting input to rendertexture ok regardless of shader that should use that texture
    //Not needed in game
    private void OnGUI()
    {
        GUI.DrawTexture(new Rect(0,0,256,256), _splatmap, ScaleMode.ScaleToFit,false,1 );
    }


}
