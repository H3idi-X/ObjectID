// (C) UTJ
using UnityEngine;
using System.Collections;
using Utj.ObjectIdRendererHelper;

[RequireComponent(typeof(Camera))]
public class ObjectIdRenderer : MonoBehaviour
{
	public Shader replaceShader = null;
	readonly string ShaderName = "Utj/ObjectIdRenderer";
	public Color backgroundColor = new Color(0f, 0f, 0f, 0f);
	public Color defaultColor = new Color(0.5f, 0.5f, 0.5f, 0f);

	[PathAttribute]
	public string csvFilename;

	Table table = new Table();

	public Camera masterCamera;
    private Camera thisCamera;

    private void Awake() {
		if(replaceShader == null) {
			replaceShader = Shader.Find(ShaderName);
		}

		thisCamera = GetComponent<Camera>();
		thisCamera.clearFlags = CameraClearFlags.SolidColor;
        thisCamera.backgroundColor = backgroundColor;
        thisCamera.SetReplacementShader(replaceShader, null);
    }

	void InitWildcard() {
		table.defaultColor = defaultColor;
		table.Load(csvFilename);
	}

	void Start() {
		if(masterCamera != null) {
			thisCamera.CopyFrom(masterCamera);
		}
		InitWildcard();

        SceneObjectTraverser.TraverseAllScenesAndGameObjects((scene, go, depth) => {
            if(go == null) {
            } else {
                foreach(Renderer renderer in go.GetComponents<Renderer>()) {
                    var a = renderer.GetType().Name;
					var goName = renderer.gameObject.name;
					var mpb = table.Get(goName);
					renderer.SetPropertyBlock(mpb);
				}
            }
        });
	}

    void OnPreRender() {
		if(thisCamera != null && masterCamera != null) {
			thisCamera.transform.position			= masterCamera.transform.position;
			thisCamera.transform.rotation			= masterCamera.transform.rotation;
	        thisCamera.rect				= masterCamera.rect;
	        thisCamera.fieldOfView		= masterCamera.fieldOfView;
	        thisCamera.nearClipPlane	= masterCamera.nearClipPlane;
	        thisCamera.farClipPlane		= masterCamera.farClipPlane;
		}
    }
}
