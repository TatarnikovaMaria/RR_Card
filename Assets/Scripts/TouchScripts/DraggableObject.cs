using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class DraggableObject : MonoBehaviour, ITouchable, IDraggable
{
    private float maxMoveSpeed = 50;
    private float eps = 0.1f;

    private bool isMoving = false;
    private Vector3 startPosition;
    private Quaternion startRotation;
    private int startSiblingIndex;
    private Vector3 targetPosition;
    private Transform _transform;
    private Camera mainCamera;

    private static Transform activeObject;

    void Start()
    {
        _transform = transform;
        mainCamera = Camera.main;
    }

    void Update()
    {
        if (isMoving)
        {
            _transform.position = Vector3.Lerp(_transform.position, targetPosition, maxMoveSpeed * Time.deltaTime);

            if (Vector3.Distance(_transform.position, targetPosition) < eps)
            {
                _transform.position = targetPosition;
                isMoving = false;
            }
        }
    }

    public void OnTouchDown(object[] arg)
    {
        NewActiveObject();
        Vector2 hitPoint = (Vector2)arg[2];
        MoveTo(hitPoint);
    }

    public void OnTouchExit(object[] arg)
    {
    }

    public void OnTouchUp(object[] arg)
    {
        if (activeObject == _transform)
        {
            MoveToStart();
        }
        activeObject = null;
    }

    public void OnTouchStay(object[] arg)
    {
    }

    public void OnTouchDrag(object[] arg)
    {
        if (activeObject == _transform)
        {
            Vector2 touchPos = (Vector2)arg[3];
            MoveTo(mainCamera.ScreenToWorldPoint(touchPos));
        }
    }

    public void OnTouchMove(object[] arg)
    {
        if (activeObject == null)
        {
            NewActiveObject();
        }

        if(activeObject == _transform)
        { 
            Vector2 hitPoint = (Vector2)arg[2];
            MoveTo(hitPoint);
        }
    }

    private void NewActiveObject()
    {
        activeObject = _transform;
        startPosition = _transform.position;
        startRotation = _transform.rotation;
        startSiblingIndex = _transform.GetSiblingIndex();
        _transform.SetAsLastSibling();
        _transform.DORotateQuaternion(Quaternion.identity, 0.1f);
    }

    private void MoveTo(Vector2 point)
    {
        targetPosition = point;
        isMoving = true;
    }

    private void MoveToStart()
    {
        isMoving = false;
        _transform.SetSiblingIndex(startSiblingIndex);
        _transform.DOMove(startPosition, 0.3f).SetEase(Ease.OutBack);
        _transform.DORotateQuaternion(startRotation, 0.3f);
    }
}
