using UnityEngine;

public class PerlinSphere : MonoBehaviour
{
    public float Radius = 1f;
    //How far each sampling step jumps (increase spikiness, reduce waviness)
    public float PerlinInterval = 0.1f;
    //How far starting of sampling is from previous iteration when animating
    public float PerlinShift = 0.01f;
    public int Longitude = 24;
    public int Latitude = 16;

    private Vector2 initialPerlin;
    

    void Awake()
    {
        Vector2 initialPerlin = Random.insideUnitCircle * Random.Range(0, 1000f);
    }

    // Use this for initialization
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void MakePlant(Mesh mesh)
    {
        mesh.Clear();

        float radius = Radius;
        // Longitude |||
        int nbLong = Longitude;
        // Latitude ---
        int nbLat = Latitude;

        Vector2 pc1 = initialPerlin + (Vector2.one*PerlinShift);
        initialPerlin = pc1;
        float perlinIter = PerlinInterval;

        #region Vertices
        Vector3[] vertices = new Vector3[(nbLong + 1) * nbLat + 2];
        float _pi = Mathf.PI;
        float _2pi = _pi * 2f;

        Vector3 northPole = (Vector3.up * radius);
        vertices[0] = northPole * Mathf.PerlinNoise(pc1.x, pc1.y) + (Vector3.up * radius);//+ new Vector3(Mathf.PerlinNoise(northPole.x, northPole.y), 0, Mathf.PerlinNoise(northPole.y, northPole.z));       

        for (int lat = 0; lat < nbLat; lat++)   //Iterate along the upward axis of the sphere, using trig to generate circular vertices
        {                                                   //Perlin effects can operate by perturbing these trigonometric expressions
            float a1 = _pi * (float)(lat + 1) / (nbLat + 1);
            float sin1 = Mathf.Sin(a1);
            float cos1 = Mathf.Cos(a1);

            for (int lon = 0; lon <= nbLong; lon++)
            {
                float a2 = _2pi * (float)(lon == nbLong ? 0 : lon) / nbLong;
                float sin2 = Mathf.Sin(a2);
                float cos2 = Mathf.Cos(a2);

                //Vertices are indexed by lat/long
                //Maintain spherical structure by only affecting x, z directions with perlin noise;
                Vector3 sphereVec = new Vector3(sin1 * cos2, cos1, sin1 * sin2) * radius;
                //sphereVec += Random.insideUnitSphere.normalized * randomOffset;
                //Vector3 perlinVec = new Vector3(Mathf.PerlinNoise(sphereVec.x, sphereVec.y), 0, Mathf.PerlinNoise(sphereVec.y, sphereVec.z));
                //Vector3 perlinVec = new Vector3(Mathf.PerlinNoise(pc1.x, pc1.y), 0, Mathf.PerlinNoise(pc2.x, pc2.y));

                //vertices[lon + lat * (nbLong + 1) + 1] = sphereVec + perlinVec;
                //vertices[lon + lat * (nbLong + 1) + 1] = sphereVec + (sphereVec * Mathf.PerlinNoise(pc1.x, pc1.x));
                vertices[lon + lat * (nbLong + 1) + 1] = sphereVec * Mathf.PerlinNoise(pc1.x, pc1.x) + (Vector3.up * radius);
                pc1 += Vector2.one * perlinIter;
            }
        }
        Vector3 southPole = Vector3.up * -radius;   //"South pole"
        vertices[vertices.Length - 1] = southPole * Mathf.PerlinNoise(pc1.x, pc1.y) + (Vector3.up * radius);//+ new Vector3(Mathf.PerlinNoise(southPole.x, southPole.y), 0, Mathf.PerlinNoise(southPole.y, southPole.z));
        #endregion

        #region Normales
        Vector3[] normales = new Vector3[vertices.Length];  //Normalizing the vertex positions for a sphere centered at 0 gives accurate normal vectors
        for (int n = 0; n < vertices.Length; n++)
            normales[n] = vertices[n].normalized;
        #endregion

        #region UVs
        Vector2[] uvs = new Vector2[vertices.Length];   //UV coordinates indexed identically as vertices on an unwrapped sphere?
        uvs[0] = Vector2.up;
        uvs[uvs.Length - 1] = Vector2.zero;
        for (int lat = 0; lat < nbLat; lat++)
            for (int lon = 0; lon <= nbLong; lon++)
                uvs[lon + lat * (nbLong + 1) + 1] = new Vector2((float)lon / nbLong, 1f - (float)(lat + 1) / (nbLat + 1));
        #endregion

        #region Triangles
        int nbFaces = vertices.Length;      //Hard part: stitching vertices into triangles
        int nbTriangles = nbFaces * 2;
        int nbIndexes = nbTriangles * 3;
        int[] triangles = new int[nbIndexes];

        //Top Cap - 
        int i = 0;
        for (int lon = 0; lon < nbLong; lon++)
        {
            triangles[i++] = lon + 2;
            triangles[i++] = lon + 1;
            triangles[i++] = 0;         //"North pole" is a ring of triangles whose points meet at the pole
        }

        //Middle                                        
        for (int lat = 0; lat < nbLat - 1; lat++)           //Circles are concentric, and connected by the vertex 'next' like so:
        {
            for (int lon = 0; lon < nbLong; lon++)                    //current ---------------------- current + 1
            {                                                        // \                             /        |
                int current = lon + lat * (nbLong + 1) + 1;         //   \                           /         |
                int next = current + nbLong + 1;                   //     \                         /          |
                //       \                       /           |
                triangles[i++] = current;                        //         \                     /            |n
                triangles[i++] = current + 1;                   //           \                   /             |b
                triangles[i++] = next + 1;                     //             \                 /              |L
                //               \               /               |o
                triangles[i++] = current;                    //                 \             /                |n
                triangles[i++] = next + 1;                  //                   \           /                 |g
                triangles[i++] = next;                     //                     \         /                  |
            }                                             //                       \       /                   |
        }                                                //                         \     /                    |
        //                           \   /                     |
        //Bottom Cap                                  next                          next + 1
        for (int lon = 0; lon < nbLong; lon++)
        {
            triangles[i++] = vertices.Length - 1;           //"South pole" is a ring of triangles whose points meet at this pole
            triangles[i++] = vertices.Length - (lon + 2) - 1;
            triangles[i++] = vertices.Length - (lon + 1) - 1;
        }
        #endregion

        mesh.vertices = vertices;
        mesh.normals = normales;
        mesh.uv = uvs;
        mesh.triangles = triangles;

        mesh.RecalculateBounds();
    }

    public void MakeCloud(Mesh mesh)
    {
        mesh.Clear();

        float radius = Radius;
        // Longitude |||
        int nbLong = Longitude;
        // Latitude ---
        int nbLat = Latitude;

        //Vector2 pc1 = Random.insideUnitCircle * Random.Range(0, 1000f);
        Vector2 pc1 = initialPerlin + (Vector2.one * PerlinShift);
        initialPerlin = pc1;

        float randomOffset = PerlinInterval;

        #region Vertices
        Vector3[] vertices = new Vector3[(nbLong + 1) * nbLat + 2];
        float _pi = Mathf.PI;
        float _2pi = _pi * 2f;

        Vector3 northPole = (Vector3.up * radius);
        vertices[0] = northPole;// +new Vector3(Mathf.PerlinNoise(northPole.x, northPole.y), 0, Mathf.PerlinNoise(northPole.y, northPole.z));

        for (int lat = 0; lat < nbLat; lat++)   //Iterate along the upward axis of the sphere, using trig to generate circular vertices
        {                                                   //Perlin effects can operate by perturbing these trigonometric expressions
            float a1 = _pi * (float)(lat + 1) / (nbLat + 1);
            float sin1 = Mathf.Sin(a1);
            float cos1 = Mathf.Cos(a1);

            for (int lon = 0; lon <= nbLong; lon++)
            {
                float a2 = _2pi * (float)(lon == nbLong ? 0 : lon) / nbLong;
                float sin2 = Mathf.Sin(a2);
                float cos2 = Mathf.Cos(a2);

                //Vertices are indexed by lat/long
                //Maintain spherical structure by only affecting x, z directions with perlin noise;
                Vector3 sphereVec = new Vector3(sin1 * cos2, cos1, sin1 * sin2) * radius;
                Vector3 perlinVec = new Vector3(Mathf.PerlinNoise(pc1.x, pc1.y), 0, Mathf.PerlinNoise(pc1.y, pc1.x));
                sphereVec = new Vector3(sphereVec.x * perlinVec.x, sphereVec.y, sphereVec.z * perlinVec.z);
                
                vertices[lon + lat * (nbLong + 1) + 1] = sphereVec;

                pc1 += Vector2.one * randomOffset;
            }
        }
        Vector3 southPole = Vector3.up * -radius;   //"South pole"
        vertices[vertices.Length - 1] = southPole;// +new Vector3(Mathf.PerlinNoise(southPole.x, southPole.y), 0, Mathf.PerlinNoise(southPole.y, southPole.z));
        #endregion

        #region Normales
        Vector3[] normales = new Vector3[vertices.Length];  //Normalizing the vertex positions for a sphere centered at 0 gives accurate normal vectors
        for (int n = 0; n < vertices.Length; n++)
            normales[n] = vertices[n].normalized;
        #endregion

        #region UVs
        Vector2[] uvs = new Vector2[vertices.Length];   //UV coordinates indexed identically as vertices on an unwrapped sphere?
        uvs[0] = Vector2.up;
        uvs[uvs.Length - 1] = Vector2.zero;
        for (int lat = 0; lat < nbLat; lat++)
            for (int lon = 0; lon <= nbLong; lon++)
                uvs[lon + lat * (nbLong + 1) + 1] = new Vector2((float)lon / nbLong, 1f - (float)(lat + 1) / (nbLat + 1));
        #endregion

        #region Triangles
        int nbFaces = vertices.Length;      //Hard part: stitching vertices into triangles
        int nbTriangles = nbFaces * 2;
        int nbIndexes = nbTriangles * 3;
        int[] triangles = new int[nbIndexes];

        //Top Cap - 
        int i = 0;
        for (int lon = 0; lon < nbLong; lon++)
        {
            triangles[i++] = lon + 2;
            triangles[i++] = lon + 1;
            triangles[i++] = 0;         //"North pole" is a ring of triangles whose points meet at the pole
        }

        //Middle                                        
        for (int lat = 0; lat < nbLat - 1; lat++)           //Circles are concentric, and connected by the vertex 'next' like so:
        {
            for (int lon = 0; lon < nbLong; lon++)                    //current ---------------------- current + 1
            {                                                        // \                             /        |
                int current = lon + lat * (nbLong + 1) + 1;         //   \                           /         |
                int next = current + nbLong + 1;                   //     \                         /          |
                //       \                       /           |
                triangles[i++] = current;                        //         \                     /            |n
                triangles[i++] = current + 1;                   //           \                   /             |b
                triangles[i++] = next + 1;                     //             \                 /              |L
                //               \               /               |o
                triangles[i++] = current;                    //                 \             /                |n
                triangles[i++] = next + 1;                  //                   \           /                 |g
                triangles[i++] = next;                     //                     \         /                  |
            }                                             //                       \       /                   |
        }                                                //                         \     /                    |
        //                           \   /                     |
        //Bottom Cap                                  next                          next + 1
        for (int lon = 0; lon < nbLong; lon++)
        {
            triangles[i++] = vertices.Length - 1;           //"South pole" is a ring of triangles whose points meet at this pole
            triangles[i++] = vertices.Length - (lon + 2) - 1;
            triangles[i++] = vertices.Length - (lon + 1) - 1;
        }
        #endregion

        mesh.vertices = vertices;
        mesh.normals = normales;
        mesh.uv = uvs;
        mesh.triangles = triangles;

        mesh.RecalculateBounds();
    }

    public void MakeCrown(Mesh mesh)
    {
        mesh.Clear();

        float radius = Radius;
        // Longitude |||
        int nbLong = Longitude;
        // Latitude ---
        int nbLat = Latitude;

        Vector2 pc1 = initialPerlin + (Vector2.one * PerlinShift);
        initialPerlin = pc1;
        float perlinIter = PerlinInterval;

        #region Vertices
        Vector3[] vertices = new Vector3[(nbLong + 1) * nbLat + 2];
        float _pi = Mathf.PI;
        float _2pi = _pi * 2f;

        Vector3 northPole = (Vector3.up * radius);
        vertices[0] = northPole * Mathf.PerlinNoise(pc1.x, pc1.y);//+ new Vector3(Mathf.PerlinNoise(northPole.x, northPole.y), 0, Mathf.PerlinNoise(northPole.y, northPole.z));       

        for (int lat = 0; lat < nbLat; lat++)   //Iterate along the upward axis of the sphere, using trig to generate circular vertices
        {                                                   //Perlin effects can operate by perturbing these trigonometric expressions
            float a1 = _pi * (float)(lat + 1) / (nbLat + 1);
            float sin1 = Mathf.Sin(a1);
            float cos1 = Mathf.Cos(a1);

            for (int lon = 0; lon <= nbLong; lon++)
            {
                float a2 = _2pi * (float)(lon == nbLong ? 0 : lon) / nbLong;
                float sin2 = Mathf.Sin(a2);
                float cos2 = Mathf.Cos(a2);

                //Vertices are indexed by lat/long
                //Maintain spherical structure by only affecting x, z directions with perlin noise;
                Vector3 sphereVec = new Vector3(sin1 * cos2, cos1, sin1 * sin2) * radius;
                //sphereVec += Random.insideUnitSphere.normalized * randomOffset;
                //Vector3 perlinVec = new Vector3(Mathf.PerlinNoise(sphereVec.x, sphereVec.y), 0, Mathf.PerlinNoise(sphereVec.y, sphereVec.z));
                //Vector3 perlinVec = new Vector3(Mathf.PerlinNoise(pc1.x, pc1.y), 0, Mathf.PerlinNoise(pc2.x, pc2.y));

                //vertices[lon + lat * (nbLong + 1) + 1] = sphereVec + perlinVec;
                //vertices[lon + lat * (nbLong + 1) + 1] = sphereVec + (sphereVec * Mathf.PerlinNoise(pc1.x, pc1.x));
                vertices[lon + lat * (nbLong + 1) + 1] = sphereVec * Mathf.PerlinNoise(pc1.x, pc1.x);
                pc1 += Vector2.one * perlinIter;
            }
        }
        Vector3 southPole = Vector3.up * -radius;   //"South pole"
        vertices[vertices.Length - 1] = southPole * Mathf.PerlinNoise(pc1.x, pc1.y);//+ new Vector3(Mathf.PerlinNoise(southPole.x, southPole.y), 0, Mathf.PerlinNoise(southPole.y, southPole.z));
        #endregion

        #region Normales
        Vector3[] normales = new Vector3[vertices.Length];  //Normalizing the vertex positions for a sphere centered at 0 gives accurate normal vectors
        for (int n = 0; n < vertices.Length; n++)
            normales[n] = vertices[n].normalized;
        #endregion

        #region UVs
        Vector2[] uvs = new Vector2[vertices.Length];   //UV coordinates indexed identically as vertices on an unwrapped sphere?
        uvs[0] = Vector2.up;
        uvs[uvs.Length - 1] = Vector2.zero;
        for (int lat = 0; lat < nbLat; lat++)
            for (int lon = 0; lon <= nbLong; lon++)
                uvs[lon + lat * (nbLong + 1) + 1] = new Vector2((float)lon / nbLong, 1f - (float)(lat + 1) / (nbLat + 1));
        #endregion

        #region Triangles
        int nbFaces = vertices.Length;      //Hard part: stitching vertices into triangles
        int nbTriangles = nbFaces * 2;
        int nbIndexes = nbTriangles * 3;
        int[] triangles = new int[nbIndexes];

        //Top Cap - 
        int i = 0;
        for (int lon = 0; lon < nbLong; lon++)
        {
            triangles[i++] = lon + 2;
            triangles[i++] = lon + 1;
            triangles[i++] = 0;         //"North pole" is a ring of triangles whose points meet at the pole
        }

        //Middle                                        
        for (int lat = 0; lat < nbLat - 1; lat++)           //Circles are concentric, and connected by the vertex 'next' like so:
        {
            for (int lon = 0; lon < nbLong; lon++)                    //current ---------------------- current + 1
            {                                                        // \                             /        |
                int current = lon + lat * (nbLong + 1) + 1;         //   \                           /         |
                int next = current + nbLong + 1;                   //     \                         /          |
                //       \                       /           |
                triangles[i++] = current;                        //         \                     /            |n
                triangles[i++] = current + 1;                   //           \                   /             |b
                triangles[i++] = next + 1;                     //             \                 /              |L
                //               \               /               |o
                triangles[i++] = current;                    //                 \             /                |n
                triangles[i++] = next + 1;                  //                   \           /                 |g
                triangles[i++] = next;                     //                     \         /                  |
            }                                             //                       \       /                   |
        }                                                //                         \     /                    |
        //                           \   /                     |
        //Bottom Cap                                  next                          next + 1
        for (int lon = 0; lon < nbLong; lon++)
        {
            triangles[i++] = vertices.Length - 1;           //"South pole" is a ring of triangles whose points meet at this pole
            triangles[i++] = vertices.Length - (lon + 2) - 1;
            triangles[i++] = vertices.Length - (lon + 1) - 1;
        }
        #endregion

        mesh.vertices = vertices;
        mesh.normals = normales;
        mesh.uv = uvs;
        mesh.triangles = triangles;

        mesh.RecalculateBounds();
    }
}
