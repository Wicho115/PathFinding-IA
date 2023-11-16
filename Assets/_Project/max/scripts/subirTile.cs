using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class subirTile : MonoBehaviour
{
        private Vector3 originalScale;
        private float levitationAmount = 0.1f;
        private Tilemap tilemap;
        private Matrix4x4 originalMatrix;

        void Start()
        {
            tilemap = GetComponent<Tilemap>();
            originalScale = transform.localScale;
            originalMatrix = tilemap.GetTransformMatrix(new Vector3Int(5, 2, 0));
        }

        void OnMouseEnter()
        {             
            Matrix4x4 matrix = Matrix4x4.TRS(Vector3.up * levitationAmount, Quaternion.identity, Vector3.one);

            tilemap.SetTransformMatrix(new Vector3Int(5, 2, 0), matrix);
            //transform.localScale = matrix.MultiplyPoint(originalScale);
        }

        void OnMouseExit()
        {
            Matrix4x4 matrix = Matrix4x4.TRS(new Vector3(0,-0.12f,0), Quaternion.identity, Vector3.one);
            tilemap.SetTransformMatrix(new Vector3Int(5, 2, 0), matrix);
        }   
}
