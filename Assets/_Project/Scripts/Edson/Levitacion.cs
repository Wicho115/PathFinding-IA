using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Levitacion : MonoBehaviour
{
    public Queue<Vector3Int> frontier = new();
    public int range;
    public Vector3Int startingPoint;
    public Vector3Int objective;

    public Set reached = new Set();

    public Tilemap tilemap;

    //public TileBase pintador;

    //public TileBase camino;

    public float delay;

    public Dictionary<Vector3Int, Vector3Int> cameFrom = new();

    public bool canstop;

    public bool canRun = true;
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space) && canRun)
        {
            FloodFillStartCoroutine();
            canRun = false;
        }
    }
    public void FloodFillStartCoroutine()
    {

        frontier.Enqueue(startingPoint);
        cameFrom.Add(startingPoint, Vector3Int.zero);


        StartCoroutine(FloodFillCoroutine());


    }

    IEnumerator FloodFillCoroutine()
    {
        while (frontier.Count > 0)
        {
            Vector3Int current = frontier.Dequeue();

            Debug.Log(frontier.Count);


            List<Vector3Int> neighbours = GetNeighbours(current);

            if (current == objective && canstop) break;

            foreach (Vector3Int next in neighbours)
            {

                if (!reached.set.Contains(next) && tilemap.GetSprite(next) != null)
                {


                    if (next != startingPoint && next != objective)
                    {
                        //estas 2 líneas sirven para las animaciones, son Traslación, Rotación y Escala
                        Matrix4x4 matrix = Matrix4x4.TRS(new Vector3(0, -0.12f, 0), quaternion.Euler(0, 0, 0), Vector3.one);
                        tilemap.SetTransformMatrix(next, matrix);


                    }
                    reached.Add(next);

                    if (frontier.Count > range)
                    {
                        frontier.Enqueue(next);
                    }


                    if (!cameFrom.ContainsKey(next))
                    {
                        cameFrom.Add(next, current);

                    }


                }

            }
            yield return new WaitForSeconds(delay);
        }
        Deselect();
        frontier.Enqueue(startingPoint);
        cameFrom.Add(startingPoint, Vector3Int.zero);
        StartCoroutine(ClearPower());
    }

    IEnumerator ClearPower()
    {
        Debug.Log("Clear");

        while (frontier.Count > 0)
        {


            Vector3Int current = frontier.Dequeue();




            List<Vector3Int> neighbours = GetNeighbours(current);



            foreach (Vector3Int next in neighbours)
            {

                if (!reached.set.Contains(next) && tilemap.GetSprite(next) != null)
                {




                    Matrix4x4 matrix = Matrix4x4.TRS(new Vector3(0, 1f, 0), Quaternion.Euler(0f, 0f, 0f), Vector3.one);
                    tilemap.SetTransformMatrix(next, matrix);



                    reached.Add(next);
                    if (Vector3Int.Distance(startingPoint, next) < range)
                    {
                        frontier.Enqueue(next);
                    }

                    if (!cameFrom.ContainsKey(next))
                    {
                        cameFrom.Add(next, current);

                    }


                }

            }
            yield return new WaitForSeconds(delay);
        }
        Deselect();
    }


    public void Deselect()
    {
        reached.set.Clear();
        frontier.Clear();
        cameFrom.Clear();
    }

    private List<Vector3Int> GetNeighbours(Vector3Int current)
    {
        List<Vector3Int> neighbours = new List<Vector3Int>();
        neighbours.Add(current + new Vector3Int(0, 1, 0));
        neighbours.Add(current + new Vector3Int(0, -1, 0));
        neighbours.Add(current + new Vector3Int(1, 0, 0));
        neighbours.Add(current + new Vector3Int(-1, 0, 0));

        return neighbours;
    }


}