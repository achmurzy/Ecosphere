using UnityEngine;
using System.Collections;
using System.Collections.Generic;

//Makes the fluxes delete somehow
public class Bezier3D : MonoBehaviour, ITouchable
{
    [SerializeField] 
    private Vector3 start;
    public Vector3 Start { get { return start; } set { start = value; GetComponent<MeshFilter>().mesh = CreateMesh(); } }
    
    [SerializeField]
    private Vector3 end;
    public Vector3 End { get { return end; } set { end = value; GetComponent<MeshFilter>().mesh = CreateMesh(); } }
    
    [SerializeField]
    private Vector3 handle1;
    public Vector3 Handle1 { get { return handle1; } set { handle1 = value; BezierMesh = CreateMesh(); } }
    
    [SerializeField]
    private Vector3 handle2;
    public Vector3 Handle2 { get { return handle2; } set { handle2 = value; BezierMesh = CreateMesh(); } }

    private Vector3 upNormal;
    public Vector3 UpNormal { get { return upNormal; } set { upNormal = value; } }

    public Mesh BezierMesh { get { return GetComponent<MeshFilter>().mesh; } set { GetComponent<MeshFilter>().mesh = value; } }
    
    public int resolution = 12;
    public float thickness = 0.25f;

    public string Touch()
    {
        return transform.parent.name;
    }

    //cacluates point coordinates on a quadratic curve
    public static Vector3 PointOnPath(float t, Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3)
    {
        float u, uu, uuu, tt, ttt;
        Vector3 p;

        u = 1 - t;
        uu = u * u;
        uuu = uu * u;

        tt = t * t;
        ttt = tt * t;

        p = uuu * p0;
        p += 3 * uu * t * p1;
        p += 3 * u * tt * p2;
        p += ttt * p3;

        return p;
    }

    public Mesh CreateMesh()
    {
        Mesh mesh;

        mesh = new Mesh();

        float scaling = 1;
        float width = thickness / 2f;
        List<Vector3> vertList = new List<Vector3>();
        List<int> triList = new List<int>();
        List<Vector2> uvList = new List<Vector2>();
        
        triList.AddRange(new int[] {                     
			2, 1, 0,    //start face
			0, 3, 2
		});

        for (int s = 0; s < resolution; s++)
        {
            float t = ((float)s) / resolution;
            float futureT = ((float)s + 1) / resolution;

            Vector3 segmentStart = PointOnPath(t, start, handle1, handle2, end);
            Vector3 segmentEnd = PointOnPath(futureT, start, handle1, handle2, end);

            Vector3 segmentDirection = segmentEnd - segmentStart;
            if (s == 0 || s == resolution - 1)
                segmentDirection = new Vector3(0, 1, 0);
            segmentDirection.Normalize();
            Vector3 segmentRight = Vector3.Cross(upNormal, segmentDirection);
            segmentRight *= width;
            Vector3 offset = segmentRight.normalized * (width / 2) * scaling;
            Vector3 br = segmentRight + upNormal * width + offset;
            Vector3 tr = segmentRight + upNormal * -width + offset;
            Vector3 bl = -segmentRight + upNormal * width + offset;
            Vector3 tl = -segmentRight + upNormal * -width + offset;

            int curTriIdx = vertList.Count;

            Vector3[] segmentVerts = new Vector3[] 
			{
				segmentStart + br,
				segmentStart + bl,
				segmentStart + tl,
				segmentStart + tr,
			};
            vertList.AddRange(segmentVerts);

            Vector2[] uvs = new Vector2[]
			{
				new Vector2(0, 0), 
				new Vector2(0, 1), 
				new Vector2(1, 1),
				new Vector2(1, 1)
			};
            uvList.AddRange(uvs);

            int[] segmentTriangles = new int[]
			{
				curTriIdx + 6, curTriIdx + 5, curTriIdx + 1, //left face
				curTriIdx + 1, curTriIdx + 2, curTriIdx + 6,
				curTriIdx + 7, curTriIdx + 3, curTriIdx + 0, //right face
				curTriIdx + 0, curTriIdx + 4, curTriIdx + 7,
				curTriIdx + 1, curTriIdx + 5, curTriIdx + 4, //top face
				curTriIdx + 4, curTriIdx + 0, curTriIdx + 1,
				curTriIdx + 3, curTriIdx + 7, curTriIdx + 6, //bottom face
				curTriIdx + 6, curTriIdx + 2, curTriIdx + 3
			};
            triList.AddRange(segmentTriangles);

            // final segment fenceposting: finish segment and add end face
            if (s == resolution - 1)
            {
                curTriIdx = vertList.Count;

                vertList.AddRange(new Vector3[] {
					segmentEnd + br,
					segmentEnd + bl,
					segmentEnd + tl,
					segmentEnd + tr
				});

                uvList.AddRange(new Vector2[] { 
						new Vector2(0, 0), 
						new Vector2(0, 1), 
						new Vector2(1, 1),
						new Vector2(1, 1)
					}
                );
                triList.AddRange(new int[] {
					curTriIdx + 0, curTriIdx + 1, curTriIdx + 2, //end face
					curTriIdx + 2, curTriIdx + 3, curTriIdx + 0
				});
            }
        }

        mesh.vertices = vertList.ToArray();
        mesh.triangles = triList.ToArray();
        mesh.uv = uvList.ToArray();
        mesh.RecalculateNormals();
        mesh.RecalculateBounds();

        return mesh;
    }

    public void OnTriggerExit(Collider other)
    {
        SendMessageUpwards("TriggerExit", other);
    }
}