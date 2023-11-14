using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class subirTile : MonoBehaviour
{
        private Vector3 originalScale;
        private float levitationAmount = 0.1f;

        void Start()
        {
             Tilemap tilemap = GetComponent<Tilemap>();
            originalScale = transform.localScale;
        }

        void OnMouseEnter()
        {
            Matrix4x4 matrix = Matrix4x4.TRS(Vector3.up * levitationAmount, Quaternion.identity, Vector3.one);
            transform.localScale = matrix.MultiplyPoint(originalScale);
        }

        void OnMouseExit()
        {
            transform.localScale = originalScale;
        }
}
