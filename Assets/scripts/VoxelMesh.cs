using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;


[ExecuteInEditMode]
[RequireComponent (typeof (MeshFilter), typeof (MeshRenderer))]
public class VoxelMesh : MonoBehaviour {
	
	private MeshFilter meshFilter;
	private MeshRenderer meshRenderer;

	public Material material;

	public Vector3 size = new Vector3(16,8,16);

	private float voxelSize = 1f;

	// Use this for initialization
	void Start () {
		UpdateMesh ();
	}

	private void Update() {
		if (Application.isEditor) {
			UpdateMesh ();
		}
	}

	private void UpdateMesh() {
		Debug.Log ("Updating mesh!");
		meshFilter = gameObject.GetComponent<MeshFilter> ();

		List<List<List<int>>> map = GenerateMap (size);
		meshFilter.mesh = GenerateMesh (map);

		//GenerateMapPerlinNoise (new Vector3(32,8,32));

		meshRenderer = gameObject.GetComponent<MeshRenderer> ();

		if (material) {
			meshRenderer.material = material;
		} else {
			meshRenderer.material = new Material (Shader.Find ("Diffuse"));
		}	
	}

	private Mesh GenerateMesh(List<List<List<int>>> map) {

		//do this in another thread when optimizing laters

		Mesh mesh = new Mesh ();

		//Face UV's the same for all of them for now
		Vector2[] uvTemplate = new Vector2[] {
			new Vector2 (0, 0),
			new Vector2 (1, 0),
			new Vector2 (0, 1),
			new Vector2 (1, 1)
		};

		//Vertice faces
		Vector3[] verticesFront= new Vector3[] {
			new Vector3(0, 0, 0), 
			new Vector3(voxelSize, 0, 0), 
			new Vector3(0, voxelSize, 0), 
			new Vector3(voxelSize, voxelSize, 0)
		};
		Vector3[] verticesTop = new Vector3[] {
			new Vector3(0, voxelSize, 0), 
			new Vector3(voxelSize, voxelSize, 0), 
			new Vector3(0, voxelSize, voxelSize), 
			new Vector3(voxelSize, voxelSize, voxelSize)
		};
		Vector3[] verticesBottom = new Vector3[] {
			new Vector3(0, 0, 0), 
			new Vector3(voxelSize, 0, 0), 
			new Vector3(0, 0, voxelSize), 
			new Vector3(voxelSize, 0, voxelSize)
		};
		Vector3[] verticesRight = new Vector3[4] {
			new Vector3(voxelSize, 0, 0), 
			new Vector3(voxelSize, 0, voxelSize), 
			new Vector3(voxelSize, voxelSize, 0), 
			new Vector3(voxelSize, voxelSize, voxelSize)
		};
		Vector3[] verticesLeft = new Vector3[] {
			new Vector3(0, 0, 0), 
			new Vector3(0, 0, voxelSize), 
			new Vector3(0, voxelSize, 0), 
			new Vector3(0, voxelSize, voxelSize)
		};
		Vector3[] verticesBack= new Vector3[] {
			new Vector3(0, 0, voxelSize), 
			new Vector3(voxelSize, 0, voxelSize), 
			new Vector3(0, voxelSize, voxelSize), 
			new Vector3(voxelSize, voxelSize, voxelSize)
		};

		//triangles
		int[] triFront = new int[6] {
			0,2,1,
			2,3,1,
		};
		int[] triTop = new int[6] {
			0,2,1,
			2,3,1,
		};
		int[] triBottom = new int[6] {
			0,1,2,
			3,2,1
		};
		int[] triRight = new int[6] {
			0,2,1,
			2,3,1,
		};
		int[] triLeft = new int[6] {
			0,1,2,
			3,2,1
		};
		int[] triBack = new int[6] {
			0,1,2,
			3,2,1
		};

		Vector3[] normalFront = new Vector3[4] {
			Vector3.back,
			Vector3.back,
			Vector3.back,
			Vector3.back
		};
		Vector3[] normalTop = new Vector3[] {
			Vector3.up,
			Vector3.up,
			Vector3.up,
			Vector3.up
		};
		Vector3[] normalBottom = new Vector3[] {
			Vector3.down,
			Vector3.down,
			Vector3.down,
			Vector3.down
		};
		Vector3[] normalRight = new Vector3[] {
			Vector3.right,
			Vector3.right,
			Vector3.right,
			Vector3.right
		};
		Vector3[] normalLeft = new Vector3[] {
			Vector3.left,
			Vector3.left,
			Vector3.left,
			Vector3.left
		};
		Vector3[] normalBack = new Vector3[] {
			Vector3.forward,
			Vector3.forward,
			Vector3.forward,
			Vector3.forward
		};

		Vector3[] vertices = new Vector3[0];
		int[] triangles = new int[0];
		Vector3[] normals = new Vector3[0];
		Vector2[] uv = new Vector2[0];


		for (int x = 0; x < map.Count; x++) {
			for (int z = 0; z < map[x].Count; z++) {
				for (int y = 0; y < map [x] [z].Count; y++) {
					//Debug.Log ("Y");

					//check all faces and if we need it add it to the mesh data arrays

					//Add all the top ones
					if (!existsInMap(new Vector3(x,y+1,z), map)) {
						vertices = AddVerticesAt(verticesTop, vertices, new Vector3(x,y,z));

						triangles = AddTriangles (triTop, triangles, vertices.Length);

						Vector3[] newNormals = new Vector3[normals.Length + normalTop.Length];
						normals.CopyTo(newNormals, 0);
						normalTop.CopyTo(newNormals, normals.Length);
						normals = newNormals;

						Vector2[] newUvs = new Vector2[uv.Length + uvTemplate.Length];
						uv.CopyTo(newUvs, 0);
						uvTemplate.CopyTo(newUvs, uv.Length);
						uv = newUvs;
					}

					if (!existsInMap(new Vector3(x,y-1,z), map)) {
						vertices = AddVerticesAt(verticesBottom, vertices, new Vector3(x,y,z));

						triangles = AddTriangles (triBottom, triangles, vertices.Length);

						Vector3[] newNormals = new Vector3[normals.Length + normalBottom.Length];
						normals.CopyTo(newNormals, 0);
						normalTop.CopyTo(newNormals, normals.Length);
						normals = newNormals;

						Vector2[] newUvs = new Vector2[uv.Length + uvTemplate.Length];
						uv.CopyTo(newUvs, 0);
						uvTemplate.CopyTo(newUvs, uv.Length);
						uv = newUvs;
					}

					if (!existsInMap(new Vector3(x-1,y,z), map)) {
						vertices = AddVerticesAt(verticesLeft, vertices, new Vector3(x,y,z));

						triangles = AddTriangles (triLeft, triangles, vertices.Length);

						Vector3[] newNormals = new Vector3[normals.Length + normalLeft.Length];
						normals.CopyTo(newNormals, 0);
						normalLeft.CopyTo(newNormals, normals.Length);
						normals = newNormals;

						Vector2[] newUvs = new Vector2[uv.Length + uvTemplate.Length];
						uv.CopyTo(newUvs, 0);
						uvTemplate.CopyTo(newUvs, uv.Length);
						uv = newUvs;
					}

					if ( !existsInMap(new Vector3(x+1,y,z), map)) {
						vertices = AddVerticesAt(verticesRight, vertices, new Vector3(x,y,z));

						triangles = AddTriangles (triRight, triangles, vertices.Length);

						Vector3[] newNormals = new Vector3[normals.Length + normalRight.Length];
						normals.CopyTo(newNormals, 0);
						normalLeft.CopyTo(newNormals, normals.Length);
						normals = newNormals;

						Vector2[] newUvs = new Vector2[uv.Length + uvTemplate.Length];
						uv.CopyTo(newUvs, 0);
						uvTemplate.CopyTo(newUvs, uv.Length);
						uv = newUvs;
					}

					if ( !existsInMap(new Vector3(x,y,z-1), map)) {
						vertices = AddVerticesAt(verticesFront, vertices, new Vector3(x,y,z));

						triangles = AddTriangles (triFront, triangles, vertices.Length);

						Vector3[] newNormals = new Vector3[normals.Length + normalFront.Length];
						normals.CopyTo(newNormals, 0);
						normalLeft.CopyTo(newNormals, normals.Length);
						normals = newNormals;

						Vector2[] newUvs = new Vector2[uv.Length + uvTemplate.Length];
						uv.CopyTo(newUvs, 0);
						uvTemplate.CopyTo(newUvs, uv.Length);
						uv = newUvs;
					}

					if ( !existsInMap(new Vector3(x,y,z+1), map)) {
						vertices = AddVerticesAt(verticesBack, vertices, new Vector3(x,y,z));

						triangles = AddTriangles (triBack, triangles, vertices.Length);

						Vector3[] newNormals = new Vector3[normals.Length + normalBack.Length];
						normals.CopyTo(newNormals, 0);
						normalLeft.CopyTo(newNormals, normals.Length);
						normals = newNormals;

						Vector2[] newUvs = new Vector2[uv.Length + uvTemplate.Length];
						uv.CopyTo(newUvs, 0);
						uvTemplate.CopyTo(newUvs, uv.Length);
						uv = newUvs;
					}

				}
			}			
		}


		mesh.vertices = vertices;
		mesh.triangles = triangles;
		mesh.normals = normals;
		mesh.uv = uv;

		return mesh;
	}

	private bool existsInMap(Vector3 position,  List<List<List<int>>> map) {
		if (map.ElementAtOrDefault ((int)position.x) == null) {
			return false;
		}

		if (map[(int)position.x].ElementAtOrDefault ((int)position.z) == null) {
			return false;
		}

		if (map[(int)position.x][(int)position.z].ElementAtOrDefault ((int)position.y) == 0) {
			return false;
		}


		return true;
	}

	private Vector3[] AddVerticesAt(Vector3[] items, Vector3[] addTo, Vector3 offset) {
		items = (Vector3[]) items.Clone ();
		for(int c = 0; c < items.Length; c++) {
			items [c] = items [c] + offset;
		}

		Vector3[] newVertices = new Vector3[addTo.Length + items.Length];
		addTo.CopyTo(newVertices, 0);
		items.CopyTo(newVertices, addTo.Length);

		return newVertices;
	}

	private int[] AddTriangles(int[] items, int[] addTo, int vertexCount) {
		int[] moddedTri = new int[6];
		for(int c = 0; c < 6;c++) {
			moddedTri [c] = items [c] + vertexCount-4;
			//Debug.Log (addTo.Length);
		}

		int[] newTriangles = new int[addTo.Length + items.Length];
		addTo.CopyTo(newTriangles, 0);
		moddedTri.CopyTo(newTriangles, addTo.Length);
		return newTriangles;	
	}


	private List<List<List<int>>> GenerateMap(Vector3 size) {
		List<List<List<int>>> map = new List<List<List<int>>> ();

		for (int x = 0; x < size.x; x++) {
			map.Add (new List<List<int>> ());
			for (int z = 0; z < size.z; z++) {
				int maxY = (int) (Mathf.PerlinNoise (x/size.x,z/size.z) * size.y)+1;
				map [x].Add (new List<int> ());

				for (int y = 0; y < maxY; y++) {
					map [x] [z].Add (1);
				}


			}			
		}

		return map;
	}
}
