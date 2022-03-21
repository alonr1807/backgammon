using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class BoardUI : MonoBehaviour
{
    public Material backgroundMaterial;
    public Material outerBackgroundMaterial;
    public Material spikeAMaterial;
    public Material spikeBMaterial;

    // Board width
    public float boardWidth = 2f;

    // Margin around board
    public float outerMargin = 0.04f;

    // Margin from center to spike tip
    public float spikeTipMargin = 0.04f;

    // Ratio of 1 means no space between spikes
    public float spikeWidthRatio = 1.0f;

    // Distance between layers
    public float zStep = 0.01f;

    private bool should_generate_board = false;

    private void OnValidate()
    {
        should_generate_board = true;
    }

    private void LateUpdate()
    {
        if (should_generate_board)
        {
            GenerateBoard();
            should_generate_board = false;
        }
    }

    private void ClearChildren()
    {
        while (transform.childCount != 0)
        {
            DestroyImmediate(transform.GetChild(0).gameObject);
        }
    }

    private GameObject GenerateTriangle(Vector3[] vertices)
    {
        GameObject g = new GameObject();
        g.name = "Triangle";
        g.AddComponent<MeshFilter>();
        g.AddComponent<MeshRenderer>();
        Mesh mesh = new Mesh();
        g.GetComponent<MeshFilter>().mesh = mesh;
        mesh.vertices = vertices;
        mesh.uv = new Vector2[] { new Vector2(0, 0), new Vector2(0, 1f), new Vector2(1f, 1f) };
        mesh.triangles = new int[] { 0, 1, 2 };
        return g;
    }

    private GameObject AddTriangle(Vector3[] vertices, Material material, Transform parent)
    {
        GameObject g = GenerateTriangle(vertices);
        g.transform.parent = parent;
        g.transform.localPosition = Vector3.zero;
        g.GetComponent<MeshRenderer>().material = material;
        return g;
    }

    private GameObject AddTriangle(Vector3[] vertices, Material material)
    {
        return AddTriangle(vertices, material, transform);
    }

    private GameObject AddRectangle(Vector3 bottomLeft, Vector3 topRight, Material material, Transform parent)
    {
        Vector3 topLeft = new Vector3(bottomLeft.x, topRight.y, topRight.z);
        Vector3 bottomRight = new Vector3(topRight.x, bottomLeft.y, bottomLeft.z);
        GameObject rectangle = new GameObject();
        rectangle.name = "Rectangle";
        rectangle.transform.parent = parent;
        rectangle.transform.localPosition = Vector3.zero;
        AddTriangle(new Vector3[] {
            bottomLeft, topLeft, topRight
        }, material, rectangle.transform);
        AddTriangle(new Vector3[] {
            bottomLeft, topRight, bottomRight
        }, material, rectangle.transform);
        return rectangle;
    }

    private GameObject AddRectangle(Vector3 bottomLeft, Vector3 topRight, Material material)
    {
        return AddRectangle(bottomLeft, topRight, material, transform);
    }

    private float Layer(int index)
    {
        return -zStep * index;
    }

    private void GenerateBoard()
    {
        ClearChildren();

        float halfBoard = boardWidth / 2;

        // Outermost rectangle
        AddRectangle(
            new Vector3(-halfBoard, -0.5f, Layer(0)),
            new Vector3(halfBoard, 0.5f, Layer(0)),
            outerBackgroundMaterial
        );

        // Inner rectangles
        AddRectangle(
            new Vector3(-halfBoard + outerMargin, -0.5f + outerMargin, Layer(1)),
            new Vector3(0 - outerMargin, 0.5f - outerMargin, Layer(1)),
            backgroundMaterial
        );
        AddRectangle(
            new Vector3(0 + outerMargin, -0.5f + outerMargin, Layer(1)),
            new Vector3(halfBoard - outerMargin, 0.5f - outerMargin, Layer(1)),
            backgroundMaterial
        );

        // Width of one inner playing area
        float innerBoardWidth = halfBoard - outerMargin * 2;

        // Width of a spike triangle
        // Design invariants:
        // 6 * spikeWidth + 7 * spikeSeparation = innerBoardWidth
        // spikeWidthRatio determines how much of innerBoardWidth is allocated for spikes, so 1 = everything
        float spaceForSpikes = innerBoardWidth * spikeWidthRatio;
        float spikeWidth = spaceForSpikes / 6;
        float spikeSeparation = (innerBoardWidth - spaceForSpikes) / 7;

        // Draw top spikes
        for (int i = 0; i < 12; ++i)
        {
            float x_offset = 0;
            // Add x-offset for spikes > 6 so they skip margin in center
            if (i >= 6)
            {
                x_offset = 2 * outerMargin + spikeSeparation;
            }

            AddTriangle(new Vector3[]
            {
                new Vector3(x_offset + -halfBoard + outerMargin + spikeWidth * (i+0.5f) + spikeSeparation * (i+1), 0 + spikeTipMargin, Layer(2)),
                new Vector3(x_offset + -halfBoard + outerMargin + spikeWidth * i + spikeSeparation * (i+1), 0.5f - outerMargin, Layer(2)),
                new Vector3(x_offset + -halfBoard + outerMargin + spikeWidth * (i+1) + spikeSeparation * (i+1), 0.5f - outerMargin, Layer(2))
            }, i % 2 == 0 ? spikeAMaterial : spikeBMaterial);
        }

        // Draw bottom spikes
        for (int i = 0; i < 12; ++i)
        {
            float x_offset = 0;
            // Add x-offset for spikes > 6 so they skip margin in center
            if (i >= 6)
            {
                x_offset = 2 * outerMargin + spikeSeparation;
            }

            AddTriangle(new Vector3[]
            {
                new Vector3(x_offset + -halfBoard + outerMargin + spikeWidth * i + spikeSeparation * (i+1), -0.5f + outerMargin, Layer(2)),
                new Vector3(x_offset + -halfBoard + outerMargin + spikeWidth * (i+0.5f) + spikeSeparation * (i+1), 0 - spikeTipMargin, Layer(2)),
                new Vector3(x_offset + -halfBoard + outerMargin + spikeWidth * (i+1) + spikeSeparation * (i+1), -0.5f + outerMargin, Layer(2))
            }, i % 2 == 1 ? spikeAMaterial : spikeBMaterial);
        }

        /*AddTriangle(new Vector3[] {
            new Vector3(0, 0, Layer(1)),
            new Vector3(0, 1, Layer(1)),
            new Vector3(1, 1, Layer(1))
        }, spikeWhiteMaterial);
        AddTriangle(new Vector3[] {
            new Vector3(0, 0, Layer(1)),
            new Vector3(0, 1, Layer(1)),
            new Vector3(1, 2, Layer(1))
        }, spikeBlackMaterial);*/
    }
}