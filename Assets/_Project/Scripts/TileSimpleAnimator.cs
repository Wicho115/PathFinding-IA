using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.Tilemaps;

[RequireComponent(typeof(TileSelector))]
public class TileSimpleAnimator : MonoBehaviour
{
    [SerializeField] private Tilemap tileMap;
    private TileSelector _selector;
    private Dictionary<Vector3Int, AnimationHandler> _activeTween = new Dictionary<Vector3Int, AnimationHandler>();

    

    private void Awake()
    {
        _selector = GetComponent<TileSelector>();
    }

    private void OnSelectedTile(CustomTile tile, Vector3Int pos)
    {
        if (_activeTween.ContainsKey(pos)) return;

        StartCoroutine(Animation(pos));
    }

    private IEnumerator Animation(Vector3Int pos)
    {
        var matrix = tileMap.GetTransformMatrix(pos);
        

        yield return null;
    }

    private class AnimationHandler
    {
        public readonly Vector3 originalPos = new Vector3(0, -0.12f, 0);
        public readonly Quaternion originalRot = Quaternion.identity;
        public readonly Vector3 originalScale = Vector3.one;

        public Vector3 pos;
        public Quaternion rot;
        public Vector3 scale;

        public Tween tweenPos = null;
        public Tween tweenRot = null;
        public Tween tweenScale = null;

        private bool _interruptTweens;

        public AnimationHandler(bool interruptTweens = false)
        {
            pos = originalPos;
            rot = originalRot;
            scale = originalScale;

            _interruptTweens = interruptTweens;
        }

        public void MovePos(Vector3 toPos)
        {
            if(tweenPos != null && tweenPos.active)
            {
                if (_interruptTweens) tweenPos.Kill();
                else return;
            }

        }

        public void MoveRot(Quaternion toRot)
        {
            if (tweenRot != null && tweenRot.active)
            {
                if (_interruptTweens) tweenRot.Kill();
                else return;
            }
        }

        public void MoveScale(Vector3 toScale)
        {
            if (tweenScale != null && tweenScale.active)
            {
                if (_interruptTweens) tweenScale.Kill();
                else return;
            }
        }
    }


    private void OnEnable()
    {
        _selector.OnSelectTile += OnSelectedTile;
    }

    private void OnDisable()
    {
        _selector.OnSelectTile -= OnSelectedTile;
    }
}
