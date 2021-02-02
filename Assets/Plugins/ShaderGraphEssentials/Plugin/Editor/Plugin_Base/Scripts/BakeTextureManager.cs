//
// ShaderGraphEssentials for Unity
// (c) 2019 PH Graphics
// Source code may be used and modified for personal or commercial projects.
// Source code may NOT be redistributed or sold.
// 
// *** A NOTE ABOUT PIRACY ***
// 
// If you got this asset from a pirate site, please consider buying it from the Unity asset store. This asset is only legally available from the Unity Asset Store.
// 
// I'm a single indie dev supporting my family by spending hundreds and thousands of hours on this and other assets. It's very offensive, rude and just plain evil to steal when I (and many others) put so much hard work into the software.
// 
// Thank you.
//
// *** END NOTE ABOUT PIRACY ***
//

using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Graphing;
using UnityEditor.ShaderGraph;
using UnityEngine;
using UnityEngine.Rendering;

namespace ShaderGraphEssentials
{
    class BakeShaderData
    {
        public BakeTextureNode Node { get; set; }
        internal GraphData Graph { get; set; }
        public Shader Shader { get; set; }
        public string ShaderString { get; set; }
        public bool HasError { get; set; }
        public string OutputIdName { get; set; }
    }

    class BakeTextureManager
    {
        private const string DefaultPath = "SGE_DefaultBakedTexture.png";

        BakeTextureManager()
        {
        }

        internal static void BakeShaderIntoTexture(BakeShaderData shaderData)
        {
            var node = shaderData.Node;
            var renderTexture = new RenderTexture(node.Width, node.Height, 16, RenderTextureFormat.ARGB32, RenderTextureReadWrite.Default) { hideFlags = HideFlags.HideAndDontSave };
            renderTexture.Create();

            // setup mesh
            Mesh quadMesh = Resources.GetBuiltinResource(typeof(Mesh), "Quad.fbx") as Mesh;
            Material bakeMaterial = new Material(shaderData.Shader);

            // setup camera
            GameObject cameraGo = new GameObject();
            var camera = cameraGo.AddComponent<Camera>();
            camera.cameraType = CameraType.Preview;
            camera.enabled = false;
            camera.clearFlags = CameraClearFlags.Depth;
            camera.fieldOfView = 15;
            camera.farClipPlane = 10.0f;
            camera.nearClipPlane = 2.0f;
            camera.backgroundColor = new Color(49.0f / 255.0f, 49.0f / 255.0f, 49.0f / 255.0f, 1.0f);
            camera.renderingPath = RenderingPath.Forward;
            camera.useOcclusionCulling = false;
            camera.clearFlags = CameraClearFlags.Depth;
            camera.transform.position = -Vector3.forward * 2;
            camera.transform.rotation = Quaternion.identity;
            camera.orthographicSize = 0.5f;
            camera.orthographic = true;
            camera.targetTexture = renderTexture;

            // setup material and fill properties from all the previous nodes
            HashSet<Identifier> propertyNodes = new HashSet<Identifier>
            {
                node.tempId
            };

            PropagateNodeSet(propertyNodes, new List<Identifier> { node.tempId }, shaderData.Graph);

            MaterialPropertyBlock materialPropertyBlock = new MaterialPropertyBlock();
            materialPropertyBlock.SetFloat(shaderData.OutputIdName, node.tempId.index);

            List<PreviewProperty> previewProperties = new List<PreviewProperty>();
            node.CollectPreviewMaterialProperties(previewProperties);

            foreach (var id in propertyNodes)
            {
                var tempNode = shaderData.Graph.GetNodeFromTempId(id) as AbstractMaterialNode;
                if (tempNode == null)
                    continue;
                tempNode.CollectPreviewMaterialProperties(previewProperties);
                foreach (var prop in shaderData.Graph.properties)
                    previewProperties.Add(prop.GetPreviewMaterialProperty());

                foreach (var previewProperty in previewProperties)
                {
                    // from https://github.com/Unity-Technologies/ScriptableRenderPipeline/commit/3b28421204badded8c0d14315f10c256de3345a0#diff-52bd31870846010ea070163214aac090
                    bakeMaterial.SetPreviewProperty(previewProperty);
                }
                    
                previewProperties.Clear();
            }

            // draw the quad into the camera's render texture
            Graphics.DrawMesh(quadMesh, Matrix4x4.identity, bakeMaterial, 1, camera, 0, materialPropertyBlock, ShadowCastingMode.Off, false, null, false);

            camera.Render();

            // get the render texture from GPU to CPU and save it as an asset
            Texture2D texture = new Texture2D(renderTexture.width, renderTexture.height, TextureFormat.RGB24, false);
            RenderTexture.active = renderTexture;
            texture.ReadPixels(new Rect(0, 0, renderTexture.width, renderTexture.height), 0, 0);
            texture.Apply();

            var textureBytes = texture.EncodeToPNG();

            string path = "";
            bool textureFound = false;
            if (node.OutputTexture != null && AssetDatabase.Contains(node.OutputTexture))
            {
                try
                {
                    path = AssetDatabase.GetAssetPath(node.OutputTexture);
                    System.IO.File.WriteAllBytes(path, textureBytes);
                    AssetDatabase.ImportAsset(path);
                    textureFound = true;
                } catch (Exception e)
                {
                    Debug.LogException(e);
                }
            }

            if (!textureFound)
            {
                path = Application.dataPath + "/" + DefaultPath;
                System.IO.File.WriteAllBytes(path, textureBytes);
                path = "Assets/" + DefaultPath;
                AssetDatabase.ImportAsset(path);
                Debug.LogWarning("No previous baked texture was found so a new one was created at Assets/" + DefaultPath + ". Please rename or move the texture as this specific texture path might be overriden.");
            }

            node.OutputTexture = (Texture2D)AssetDatabase.LoadAssetAtPath(path, typeof(Texture2D));

            // cleanup
            RenderTexture.active = null;
            UnityEngine.Object.DestroyImmediate(cameraGo);
            UnityEngine.Object.DestroyImmediate(bakeMaterial);
        }

        // function from ShaderGraph's PreviewManager with a few modifications
        // it recurse through the graph, starting from IDs in initialWavefront and will go back.
        // Every node found will go into the nodeSet list hashset.
        static void PropagateNodeSet(HashSet<Identifier> nodeSet, IEnumerable<Identifier> initialWavefront, GraphData graph)
        {
            Stack<Identifier> wavefront = new Stack<Identifier>();
            List<IEdge> edges = new List<IEdge>();
            List<MaterialSlot> slots = new List<MaterialSlot>();

            wavefront.Clear();
            if (initialWavefront != null)
            {
                foreach (var id in initialWavefront)
                    wavefront.Push(id);
            }

            while (wavefront.Count > 0)
            {
                var index = wavefront.Pop();
                var node = graph.GetNodeFromTempId(index);
                if (node == null)
                    continue;

                // Loop through all nodes that the node feeds into.
                slots.Clear();

                node.GetInputSlots(slots);
                foreach (var slot in slots)
                {
                    edges.Clear();
                    graph.GetEdges(slot.slotReference, edges);
                    foreach (var edge in edges)
                    {
                        // We look at each node we feed into.
                        var connectedSlot = edge.outputSlot;
                        var connectedNodeGuid = connectedSlot.nodeGuid;
                        var connectedNode = graph.GetNodeFromGuid(connectedNodeGuid);

                        // If the input node is already in the set of time-dependent nodes, we don't need to process it.
                        if (nodeSet.Contains(connectedNode.tempId))
                            continue;

                        // Add the node to the set of time-dependent nodes, and to the wavefront such that we can process the nodes that it feeds into.
                        nodeSet.Add(connectedNode.tempId);
                        wavefront.Push(connectedNode.tempId);
                    }
                }
            }
        }
    }
}