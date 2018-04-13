using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace KinectExercise
{
    public class DisplayManager : MonoBehaviour
    {
        public Camera MainCamera;
        public GameObject DisplayPlane;
        public int Depth = 10;
        public Texture2D Texture;
        public ColorImageManager colorImageManager;

        public void Awake()
        {
            DisplayPlane = CreateDisplayPlane(MainCamera, Depth);
            DisplayPlane.transform.parent = gameObject.transform;
        }

        public void Update()
        {
            Texture = colorImageManager.Texture;
            RefreshShaderTexture();
        }

        public static GameObject CreateDisplayPlane(Camera cam, int depth)
        {
            GameObject planeGO = new GameObject();
            planeGO.SetActive(false);
            planeGO.name = "Display Plane";

            MeshFilter mf = planeGO.AddComponent<MeshFilter>();
            mf.mesh = GenerateDisplayMesh(cam, depth);

            MeshRenderer mr = planeGO.AddComponent<MeshRenderer>();
            mr.material = GenerateDisplayMaterial();

            planeGO.SetActive(true);

            return planeGO;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="cam">The camera that will be viewing this display plane.</param>
        /// <returns></returns>
        private static Mesh GenerateDisplayMesh(Camera cam, int depth)
        {
            Mesh m = new Mesh();

            List<Vector3> vertices = new List<Vector3>();
            
            Vector3 LL = cam.ScreenToWorldPoint(new Vector3(0, 0, depth));
            Vector3 UL = cam.ScreenToWorldPoint(new Vector3(0, cam.pixelHeight, depth));
            Vector3 UR = cam.ScreenToWorldPoint(new Vector3(cam.pixelWidth, cam.pixelHeight, depth));
            Vector3 LR = cam.ScreenToWorldPoint(new Vector3(cam.pixelWidth, 0, depth));
            vertices.AddRange(new Vector3[4] { LL, UL, UR, LR });

            List<int> triangleList = new List<int>();
            triangleList.AddRange(new int[6]{0,1,2,0,2,3});

            List<Vector2> uvs = new List<Vector2>();
            Vector2 llUV = new Vector2(0, 0);
            Vector2 ulUV = new Vector2(0, 1);
            Vector2 urUV = new Vector2(1, 1);
            Vector2 lrUV = new Vector2(1, 0);
            uvs.AddRange(new Vector2[4] { llUV, ulUV, urUV, lrUV });

            m.SetVertices(vertices);
            m.SetTriangles(triangleList, 0);
            m.SetUVs(0, uvs);
            m.SetUVs(1, uvs);

            return m;
        }

        private static Material GenerateDisplayMaterial()
        {
            Material planeMat = new Material(Shader.Find("Custom/ImageShader"));
            planeMat.name = "DisplayMaterial";
            planeMat.SetTextureScale("_MainTex", new Vector2(1, -1)); // Flips the image

            return planeMat;
        }

        public void RefreshShaderTexture()
        {
            var mr = DisplayPlane.GetComponent<MeshRenderer>();
            mr.material.SetTexture("_MainTex", Texture);
        }
    }
}