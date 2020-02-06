﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TerrainFace
{
    Mesh mesh;
    int resolution;
    // 1. 어떤 방향으로 나아가는지 알기 위한 백터
    Vector3 localUp;
    // 2. localUp 방향을 제외한 axisA,axisB 방향
    Vector3 axisA;
    Vector3 axisB;

    public TerrainFace(Mesh mesh, int resolution, Vector3 localUp)
    {
        this.mesh = mesh;
        this.resolution = resolution;
        this.localUp = localUp;

        axisA = new Vector3(localUp.y, localUp.z, localUp.x);
        axisB = Vector3.Cross(localUp, axisA);
    }

    public void ConstructMesh()
    {
        Vector3[] vertices = new Vector3[resolution * resolution];
        // resolution 값이 4라고 생각하면 (4-1)^2는 사각형의 갯수가 되고 * 2 를해주면 삼각형의 갯수가 된다 삼각형의 꼭지점의 갯수가 3이므로 삼각형의 갯수 (4-1)*2 에 *3을 해주면 꼭지점의 갯수가 나온다
        // 꼭지점의 갯수를 구한이유는 삼각형의 각 점의 위치를 알기 위해서 이다
        int[] triangles = new int[(resolution - 1) * (resolution - 1) * 6];
        int triIndex = 0; 
        for(int y = 0; y < resolution; y++)
        {
            for(int x = 0; x < resolution; x++)
            {
                int i = x + y * resolution;
                Vector2 percent = new Vector2(x, y) / (resolution - 1);
                Vector3 pointOnUnitCube = localUp + (percent.x - .5f) * 2 * axisA + (percent.y - .5f) * 2 * axisB;
                Vector3 pointOnUnitSphere = pointOnUnitCube.normalized;
                vertices[i] = pointOnUnitSphere;

                if (x != resolution - 1 && y != resolution - 1)
                {
                    triangles[triIndex] = i;
                    triangles[triIndex+1] = i+resolution+1;
                    triangles[triIndex+2] = i+resolution;
                    
                    triangles[triIndex+3] = i;
                    triangles[triIndex+4] = i+1;
                    triangles[triIndex+5] = i+resolution +1;
                    triIndex += 6;
                }
            }
        }
        mesh.Clear();
        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.RecalculateNormals();
    }
}
