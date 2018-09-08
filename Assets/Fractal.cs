using System.Collections;
using UnityEngine;

public class Fractal : MonoBehaviour
{

    public Mesh mesh;
    public Material material;
    public int maxDepth;
    public float depthScale;
    public int rotateIfDepthIs;

    private static Tuple<Vector3, Quaternion>[] childrenData =
    {
        new Tuple<Vector3, Quaternion>(Vector3.up, Quaternion.Euler(0f, 0f, 0f)),
        new Tuple<Vector3, Quaternion>(Vector3.right, Quaternion.Euler(0f, 0f, -90f)),
        new Tuple<Vector3, Quaternion>(Vector3.left, Quaternion.Euler(0f, 0f, 90f)),
        new Tuple<Vector3, Quaternion>(Vector3.down, Quaternion.Euler(0f, 180f, 0f)),
        new Tuple<Vector3, Quaternion>(Vector3.back, Quaternion.Euler(90f, 0f, 0f)),
        new Tuple<Vector3, Quaternion>(Vector3.forward, Quaternion.Euler(-90f, 0f, 0f)),
    };

    private static Material[] materials;

    private int depth;

    private void Start()
    {
        if (Fractal.materials == null)
        {
            InitMaterials();
        }
        CreateMe();
        if (this.depth < maxDepth)
        {
            StartCoroutine(CreateChildren());
        }
    }

    private void InitMaterials()
    {
        Fractal.materials = new Material[maxDepth + 1];
        for (int i = 0; i <= maxDepth; i++)
        {
            Fractal.materials[i] = new Material(this.material);
            Fractal.materials[i].color = Color.Lerp(Color.red, Color.white, (float)i / (maxDepth * 2));
        }
    }

    private void CreateMe()
    {
        this.gameObject.AddComponent<MeshFilter>().mesh = this.mesh;
        this.gameObject.AddComponent<MeshRenderer>().material = Fractal.materials[this.depth];
    }

    private IEnumerator CreateChildren()
    {
        for (int i = 0; i < childrenData.Length; i++)
        {
            float pauseTime = (Random.Range(0.1f, 0.5f)) * (childrenData.Length - i); // decay pause time

            yield return new WaitForSeconds(pauseTime);
            new GameObject("Fractal Child").AddComponent<Fractal>().Initialize(this, childrenData[i].left, childrenData[i].right);
        }
    }

    private void Initialize(Fractal parent,
                            Vector3 direction,
                            Quaternion orientation)
    {
        this.mesh = parent.mesh;
        this.maxDepth = parent.maxDepth;
        this.depthScale = parent.depthScale;
        this.rotateIfDepthIs = parent.rotateIfDepthIs;

        this.depth = parent.depth + 1;
        this.transform.parent = parent.transform;
        this.transform.localScale = Vector3.one * this.depthScale;
        this.transform.localPosition = direction * (0.5f + 0.5f * this.depthScale);
        this.transform.localRotation = orientation;
    }

    private void Update()
    {
        if (this.depth <= this.rotateIfDepthIs)
        {
            float dt = Time.deltaTime;
            this.transform.Rotate(new Vector3(5f * dt, 10f * dt, 0f));
        }

    }

    private struct Tuple<L, R>
    {
        public L left;
        public R right;

        public Tuple(L left, R right)
        {
            this.left = left;
            this.right = right;
        }
    }
}
