using System.Collections;
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

    public TerrainFace(ShapeGenerator shapeGenerator, Mesh mesh, int resolution, Vector3 localUp)
    {
        this.mesh = mesh;
        this.resolution = resolution;
        this.localUp = localUp;
        // 3. axisA의 위치를 localUp의 y,z,x에 위치한곳이 axisA이다
        axisA = new Vector3(localUp.y, localUp.z, localUp.x);
        // 4. axisB는 localUp과 axisA의 Cross지점 즉 플레밍의 왼손법칙을 사용하여 axisB지점을 찾아준느것이다
        axisB = Vector3.Cross(localUp, axisA);
    }

    public void ConstructMesh()
    {
        // 5. 해상도는 면의 단일 모서리의 따라 정점의 개수가 된다. 하여 정점의 총개수는 resolution의 제곱이 된다. 
        Vector3[] vertices = new Vector3[resolution * resolution];
        // 6. 메시에 얼마나 많은 삼각형이 있는지 알아내야된다 
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
