using UnityEngine;

namespace SauceSort.Gameplay
{
    /// <summary>
    /// Builds low-cost 2D mesh fills for the bottle's existing vertical liquid slots.
    /// The owning BottleController supplies slot transforms and fill progress.
    /// </summary>
    internal sealed class BottleLiquidRenderer
    {
        private const int MeshColumns = 13;
        private const int MeshRows = 5;
        private const float BodyBottomHeight = 0.12f;
        private const float ShoulderStartHeight = 0.72f;
        private const float NeckStartHeight = 0.9f;
        private const float HeelWidth = 0.66f;
        private const float BodyWidth = 0.96f;
        private const float ShoulderWidth = 0.72f;
        private const float NeckWidth = 0.58f;
        private const float SurfaceCurve = 0.1f;
        private const float WobbleCurve = 0.08f;
        private const float WobbleFrequency = 2.4f;
        private const float SurfaceBandHeight = 0.2f;
        private const float SurfaceDepthOffset = -0.01f;

        private readonly Mesh[] _meshes;
        private readonly Vector3[][] _vertices;
        private readonly Vector2[][] _segmentUvs;
        private readonly float[] _fills;
        private readonly bool[] _exposedSurfaces;
        private readonly GameObject[] _layerObjects;
        private readonly Transform _bottleTransform;
        private readonly int _segmentCapacity;
        private readonly Mesh _surfaceMesh;
        private readonly Vector3[] _surfaceVertices;
        private readonly Transform _surfaceTransform;
        private readonly MeshRenderer _surfaceRenderer;
        private float _wobbleAmount;
        private float _surfaceTilt;
        private int _surfaceSegmentIndex = -1;

        public BottleLiquidRenderer(Transform bottleTransform, GameObject[] layerObjects)
        {
            _bottleTransform = bottleTransform;
            _layerObjects = layerObjects;
            _segmentCapacity = layerObjects.Length;
            _meshes = new Mesh[layerObjects.Length];
            _vertices = new Vector3[layerObjects.Length][];
            _segmentUvs = new Vector2[layerObjects.Length][];
            _fills = new float[layerObjects.Length];
            _exposedSurfaces = new bool[layerObjects.Length];

            for (int i = 0; i < layerObjects.Length; i++)
            {
                MeshFilter filter = layerObjects[i].GetComponent<MeshFilter>();
                if (filter == null)
                {
                    continue;
                }

                _meshes[i] = new Mesh
                {
                    name = layerObjects[i].name + "_LiquidMesh",
                    hideFlags = HideFlags.DontSave
                };
                _vertices[i] = new Vector3[MeshColumns * MeshRows];
                _segmentUvs[i] = CreateUvs(i, _segmentCapacity);
                _meshes[i].vertices = _vertices[i];
                _meshes[i].uv = _segmentUvs[i];
                _meshes[i].triangles = CreateTriangles();
                filter.sharedMesh = _meshes[i];
            }

            GameObject surfaceObject = new GameObject("LiquidSurface");
            _surfaceTransform = surfaceObject.transform;
            _surfaceTransform.SetParent(bottleTransform, false);
            PositionSurfaceForSegment(0);
            _surfaceMesh = new Mesh
            {
                name = "Bottle_LiquidSurfaceMesh",
                hideFlags = HideFlags.DontSave
            };
            _surfaceVertices = new Vector3[MeshColumns * 2];
            _surfaceMesh.vertices = _surfaceVertices;
            _surfaceMesh.uv = CreateSurfaceUvs();
            _surfaceMesh.triangles = CreateSurfaceTriangles();
            surfaceObject.AddComponent<MeshFilter>().sharedMesh = _surfaceMesh;
            _surfaceRenderer = surfaceObject.AddComponent<MeshRenderer>();
            _surfaceRenderer.enabled = false;
        }

        public void SetMotion(float amount)
        {
            _wobbleAmount = Mathf.Clamp01(amount);
            float rotation = _bottleTransform.eulerAngles.z;
            _surfaceTilt = -Mathf.Tan(rotation * Mathf.Deg2Rad);

            for (int i = 0; i < _meshes.Length; i++)
            {
                if (_meshes[i] != null && _fills[i] > 0f)
                {
                    RebuildSegment(i);
                }
            }

            RebuildSurface();
        }

        public void SetSurfaceMaterial(Material material)
        {
            _surfaceRenderer.sharedMaterial = material;
        }

        public void SetSegment(int segmentIndex, float fill, bool exposedSurface)
        {
            if (segmentIndex < 0 || segmentIndex >= _meshes.Length || _meshes[segmentIndex] == null)
            {
                return;
            }

            fill = Mathf.Clamp01(fill);
            _fills[segmentIndex] = fill;
            _exposedSurfaces[segmentIndex] = exposedSurface;
            RebuildSegment(segmentIndex);

            if (exposedSurface && fill > 0f)
            {
                _surfaceSegmentIndex = segmentIndex;
                PositionSurfaceForSegment(segmentIndex);
                _surfaceRenderer.enabled = true;
                RebuildSurface();
            }
            else if (_surfaceSegmentIndex == segmentIndex)
            {
                _surfaceSegmentIndex = -1;
                _surfaceRenderer.enabled = false;
            }
        }

        private void PositionSurfaceForSegment(int segmentIndex)
        {
            Transform segmentTransform = _layerObjects[segmentIndex].transform;
            Vector3 rootScale = _bottleTransform.lossyScale;
            Vector3 segmentScale = segmentTransform.lossyScale;
            _surfaceTransform.localPosition = _bottleTransform.InverseTransformPoint(segmentTransform.position)
                + Vector3.forward * SurfaceDepthOffset;
            _surfaceTransform.localRotation = Quaternion.Inverse(_bottleTransform.rotation)
                * segmentTransform.rotation;
            _surfaceTransform.localScale = new Vector3(
                segmentScale.x / rootScale.x,
                segmentScale.y / rootScale.y,
                segmentScale.z / rootScale.z);
        }

        private void RebuildSurface()
        {
            int index = _surfaceSegmentIndex;
            if (index < 0 || index >= _fills.Length || _fills[index] <= 0f)
            {
                _surfaceRenderer.enabled = false;
                return;
            }

            float fill = _fills[index];
            int vertex = 0;
            for (int row = 0; row < 2; row++)
            {
                for (int column = 0; column < MeshColumns; column++)
                {
                    float horizontal = column / (float)(MeshColumns - 1);
                    float centered = horizontal * 2f - 1f;
                    float envelope = 1f - centered * centered;
                    float halfWidth = BottleWidthAt((index + fill) / _segmentCapacity) * 0.5f;
                    float edgeCurve = (SurfaceCurve + Mathf.Sin(
                        centered * Mathf.PI * 1.5f + Time.time * WobbleFrequency)
                        * WobbleCurve * _wobbleAmount) * envelope;
                    float tilt = centered * halfWidth * _surfaceTilt;
                    float top = -0.5f + fill + tilt + edgeCurve;
                    float y = row == 0 ? top - SurfaceBandHeight : top;
                    _surfaceVertices[vertex++] = new Vector3(centered * halfWidth, y, 0f);
                }
            }

            _surfaceMesh.vertices = _surfaceVertices;
            _surfaceMesh.RecalculateBounds();
            _surfaceRenderer.enabled = true;
        }

        private void RebuildSegment(int segmentIndex)
        {
            float fill = _fills[segmentIndex];
            bool exposedSurface = _exposedSurfaces[segmentIndex];
            Vector3[] vertices = _vertices[segmentIndex];
            Vector2[] uvs = _segmentUvs[segmentIndex];
            int vertex = 0;

            for (int row = 0; row < MeshRows; row++)
            {
                float rowProgress = row / (float)(MeshRows - 1);
                float localY = Mathf.Lerp(-0.5f, -0.5f + fill, rowProgress);
                float bottleHeight = (segmentIndex + fill * rowProgress) / _segmentCapacity;
                float halfWidth = BottleWidthAt(bottleHeight) * 0.5f;

                for (int column = 0; column < MeshColumns; column++)
                {
                    float horizontal = column / (float)(MeshColumns - 1);
                    float centered = horizontal * 2f - 1f;
                    float envelope = 1f - centered * centered;
                    float surfaceOffset = 0f;

                    if (exposedSurface && row == MeshRows - 1)
                    {
                        float phase = Time.time * WobbleFrequency + segmentIndex * 0.6f;
                        float wobble = Mathf.Sin(centered * Mathf.PI * 1.5f + phase)
                            * WobbleCurve * _wobbleAmount;
                        surfaceOffset = centered * halfWidth * _surfaceTilt
                            + (SurfaceCurve + wobble) * envelope;
                    }

                    vertices[vertex++] = new Vector3(centered * halfWidth, localY + surfaceOffset, 0f);
                    uvs[(row * MeshColumns) + column] = new Vector2(
                        horizontal,
                        (segmentIndex + fill * rowProgress) / _segmentCapacity);
                }
            }

            Mesh mesh = _meshes[segmentIndex];
            mesh.vertices = vertices;
            mesh.uv = uvs;
            mesh.RecalculateBounds();
        }

        public void Dispose()
        {
            for (int i = 0; i < _meshes.Length; i++)
            {
                if (_meshes[i] != null)
                {
                    Object.Destroy(_meshes[i]);
                }
            }

            if (_surfaceMesh != null)
            {
                Object.Destroy(_surfaceMesh);
            }
            if (_surfaceTransform != null)
            {
                Object.Destroy(_surfaceTransform.gameObject);
            }
        }

        private static float BottleWidthAt(float height)
        {
            if (height < BodyBottomHeight)
            {
                return Mathf.Lerp(HeelWidth, BodyWidth, height / BodyBottomHeight);
            }

            if (height < ShoulderStartHeight)
            {
                return BodyWidth;
            }

            if (height < NeckStartHeight)
            {
                return Mathf.Lerp(BodyWidth, ShoulderWidth,
                    (height - ShoulderStartHeight) / (NeckStartHeight - ShoulderStartHeight));
            }

            return Mathf.Lerp(ShoulderWidth, NeckWidth,
                (height - NeckStartHeight) / (1f - NeckStartHeight));
        }

        private static Vector2[] CreateUvs(int segmentIndex, int segmentCapacity)
        {
            Vector2[] uvs = new Vector2[MeshColumns * MeshRows];
            int vertex = 0;
            for (int row = 0; row < MeshRows; row++)
            {
                float rowProgress = row / (float)(MeshRows - 1);
                for (int column = 0; column < MeshColumns; column++)
                {
                    uvs[vertex++] = new Vector2(
                        column / (float)(MeshColumns - 1),
                        (segmentIndex + rowProgress) / segmentCapacity);
                }
            }

            return uvs;
        }

        private static int[] CreateTriangles()
        {
            int[] triangles = new int[(MeshColumns - 1) * (MeshRows - 1) * 6];
            int triangle = 0;
            for (int row = 0; row < MeshRows - 1; row++)
            {
                for (int column = 0; column < MeshColumns - 1; column++)
                {
                    int lowerLeft = row * MeshColumns + column;
                    int lowerRight = lowerLeft + 1;
                    int upperLeft = lowerLeft + MeshColumns;
                    int upperRight = upperLeft + 1;
                    triangles[triangle++] = lowerLeft;
                    triangles[triangle++] = upperLeft;
                    triangles[triangle++] = lowerRight;
                    triangles[triangle++] = lowerRight;
                    triangles[triangle++] = upperLeft;
                    triangles[triangle++] = upperRight;
                }
            }

            return triangles;
        }

        private static Vector2[] CreateSurfaceUvs()
        {
            Vector2[] uvs = new Vector2[MeshColumns * 2];
            int vertex = 0;
            for (int row = 0; row < 2; row++)
            {
                for (int column = 0; column < MeshColumns; column++)
                {
                    uvs[vertex++] = new Vector2(column / (float)(MeshColumns - 1), row * 0.12f + 0.86f);
                }
            }

            return uvs;
        }

        private static int[] CreateSurfaceTriangles()
        {
            int[] triangles = new int[(MeshColumns - 1) * 6];
            int triangle = 0;
            for (int column = 0; column < MeshColumns - 1; column++)
            {
                int lowerLeft = column;
                int lowerRight = column + 1;
                int upperLeft = MeshColumns + column;
                int upperRight = upperLeft + 1;
                triangles[triangle++] = lowerLeft;
                triangles[triangle++] = upperLeft;
                triangles[triangle++] = lowerRight;
                triangles[triangle++] = lowerRight;
                triangles[triangle++] = upperLeft;
                triangles[triangle++] = upperRight;
            }

            return triangles;
        }
    }
}
