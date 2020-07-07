//================================================================================================================================
//
//  Copyright (c) 2015-2020 VisionStar Information Technology (Shanghai) Co., Ltd. All Rights Reserved.
//  EasyAR is the registered trademark or trademark of VisionStar Information Technology (Shanghai) Co., Ltd in China
//  and other countries for the augmented reality technology developed by VisionStar Information Technology (Shanghai) Co., Ltd.
//
//================================================================================================================================

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Common
{
    public class TouchController : MonoBehaviour
    {
        private const float rotateSpeed = 270;
        private const float gestureEnableDistanceThreshold = 10;

        private Transform controlTarget;
        private Camera cameraTarget;
        private bool isOneFingerDraggable;
        private bool isTwoFingerDraggable;
        private bool isTwoFingerScalable;
        private bool isTwoFingerRotatable;
        private Dictionary<int, Vector2> originalPosition = new Dictionary<int, Vector2>();
        private GestureControl curGesture;
        private float targetCamDistance;

        private enum GestureControl
        {
            NoTouch,
            OneMove,
            TwoWait,
            TwoMove,
            TwoRotate,
            TwoScale,
            OutOfControl,
        }

        private void Update()
        {
            if (!controlTarget) { return; }
            foreach (var touch in Input.touches)
            {
                if (touch.phase == TouchPhase.Began)
                {
                    originalPosition[touch.fingerId] = touch.position;
                }
                else if (touch.phase == TouchPhase.Ended)
                {
                    originalPosition.Remove(touch.fingerId);
                }
            }

            if (curGesture == GestureControl.OutOfControl)
            {
                if (Input.touchCount == 0)
                {
                    StopAllCoroutines();
                    curGesture = GestureControl.NoTouch;
                }
            }
            else if (curGesture == GestureControl.TwoMove || curGesture == GestureControl.TwoRotate || curGesture == GestureControl.TwoScale)
            {
                if (Input.touchCount != 2)
                {
                    StopAllCoroutines();
                    curGesture = GestureControl.OutOfControl;
                }
            }
            else if (curGesture == GestureControl.TwoWait)
            {
                if (Input.touchCount != 2)
                {
                    StopAllCoroutines();
                    curGesture = GestureControl.OutOfControl;
                }
                else
                {
                    Vector2 touch1Delta = Input.GetTouch(0).position - originalPosition[Input.GetTouch(0).fingerId];
                    Vector2 touch2Delta = Input.GetTouch(1).position - originalPosition[Input.GetTouch(1).fingerId];
                    if (touch1Delta.magnitude > gestureEnableDistanceThreshold && touch2Delta.magnitude > gestureEnableDistanceThreshold)
                    {
                        StopAllCoroutines();
                        if (Vector2.Dot(touch1Delta, touch2Delta) > 0)
                        {
                            Vector3 xMov;
                            Vector3 yMov;
                            GetRelativeTouch(touch1Delta + touch2Delta, out xMov, out yMov);
                            if (xMov.sqrMagnitude > yMov.sqrMagnitude)
                            {
                                curGesture = GestureControl.TwoRotate;
                                if (isTwoFingerRotatable)
                                {
                                    StartCoroutine(OnTwoRotate());
                                }
                            }
                            else
                            {
                                curGesture = GestureControl.TwoMove;
                                if (isTwoFingerDraggable)
                                {
                                    StartCoroutine(OnTwoMove());
                                }
                            }
                        }
                        else
                        {
                            curGesture = GestureControl.TwoScale;
                            if (isTwoFingerScalable)
                            {
                                StartCoroutine(OnTwoScale());
                            }
                        }
                    }
                }
            }
            else if (curGesture == GestureControl.OneMove)
            {
                if (Input.touchCount == 2)
                {
                    StopAllCoroutines();
                    curGesture = GestureControl.TwoWait;
                }
                else if (Input.touchCount != 1)
                {
                    StopAllCoroutines();
                    curGesture = GestureControl.OutOfControl;
                }
            }
            else if (curGesture == GestureControl.NoTouch)
            {
                if (Input.touchCount == 1)
                {
                    curGesture = GestureControl.OneMove;
                    if (isOneFingerDraggable)
                    {
                        StopAllCoroutines();
                        StartCoroutine(OnOneMove());
                    }
                }
                else if (Input.touchCount == 2)
                {
                    curGesture = GestureControl.TwoWait;
                    StopAllCoroutines();
                }
            }
            if (controlTarget != null)
            {
                targetCamDistance = (cameraTarget.transform.position - controlTarget.position).magnitude;
            }
        }

        public void TurnOn(Transform target, Camera cam, bool isOneFingerDraggable, bool isTwoFingerDraggable, bool isTwoFingerScalable, bool isTwoFingerRotatable)
        {
            StopAllCoroutines();
            controlTarget = target;
            cameraTarget = cam;
            this.isOneFingerDraggable = isOneFingerDraggable;
            this.isTwoFingerDraggable = isTwoFingerDraggable;
            this.isTwoFingerScalable = isTwoFingerScalable;
            this.isTwoFingerRotatable = isTwoFingerRotatable;
            curGesture = GestureControl.NoTouch;
        }

        public void TurnOff()
        {
            StopAllCoroutines();
            controlTarget = null;
            cameraTarget = null;
            curGesture = GestureControl.NoTouch;
        }

        private IEnumerator OnOneMove()
        {
            yield return new WaitForSeconds(0.1f);
            foreach (var touch in Input.touches)
            {
                originalPosition[touch.fingerId] = touch.position;
            }

            while (Input.touchCount == 1 && originalPosition.ContainsKey(Input.GetTouch(0).fingerId))
            {
                if (!controlTarget) { yield break; }
                var touchV3 = new Vector3(Input.GetTouch(0).deltaPosition.x / Screen.width, Input.GetTouch(0).deltaPosition.y / Screen.height, 0);
                var addV3 = cameraTarget.transform.localToWorldMatrix.MultiplyVector(touchV3);
                var newPos = controlTarget.position + addV3 * targetCamDistance;
                controlTarget.position = newPos;
                yield return 0;
            }
        }

        private IEnumerator OnTwoMove()
        {
            Vector3 xMovement;
            Vector3 yMovement;
            var rawTargetPos = controlTarget.position;

            foreach (var touch in Input.touches)
            {
                originalPosition[touch.fingerId] = touch.position;
            }

            while (Input.touchCount == 2 &&
                    originalPosition.ContainsKey(Input.GetTouch(0).fingerId) &&
                    originalPosition.ContainsKey(Input.GetTouch(1).fingerId))
            {
                if (!controlTarget) { yield break; }
                Vector2 fixedDelta = (Input.GetTouch(0).position + Input.GetTouch(1).position - originalPosition[Input.GetTouch(0).fingerId] - originalPosition[Input.GetTouch(1).fingerId]);
                GetRelativeTouch(fixedDelta, out xMovement, out yMovement);
                if (yMovement != Vector3.zero)
                {
                    var Cam_Forward = cameraTarget.transform.forward;
                    var Cam_Forward_XZ = Vector3.ProjectOnPlane(Cam_Forward, Vector3.up);

                    var newPos = rawTargetPos + (Vector3.Dot(yMovement, Cam_Forward) > 0 ? Cam_Forward_XZ : -Cam_Forward_XZ) * yMovement.magnitude * targetCamDistance * 2 / 1000;
                    controlTarget.position = newPos;
                }
                yield return 0;
            }
        }

        private IEnumerator OnTwoRotate()
        {
            Vector3 xMovement;
            Vector3 yMovement;
            Quaternion rawRotation = controlTarget.rotation;
            foreach (var touch in Input.touches)
            {
                originalPosition[touch.fingerId] = touch.position;
            }

            while (Input.touchCount == 2 &&
                    originalPosition.ContainsKey(Input.GetTouch(0).fingerId) &&
                    originalPosition.ContainsKey(Input.GetTouch(1).fingerId))
            {
                if (!controlTarget) { yield break; }
                Vector2 fixedDelta = (Input.GetTouch(0).position + Input.GetTouch(1).position - originalPosition[Input.GetTouch(0).fingerId] - originalPosition[Input.GetTouch(1).fingerId]) * 0.5f / Screen.width * rotateSpeed;
                GetRelativeTouch(fixedDelta, out xMovement, out yMovement);
                if (xMovement != Vector3.zero)
                {
                    if (Vector3.Dot(Vector3.Cross(xMovement.normalized, Vector3.up), Camera.main.transform.forward) < 0f)
                    {
                        controlTarget.rotation = rawRotation * Quaternion.Euler(0f, xMovement.sqrMagnitude / Mathf.PI, 0f);
                    }
                    else
                    {
                        controlTarget.rotation = rawRotation * Quaternion.Euler(0f, -xMovement.sqrMagnitude / Mathf.PI, 0f);
                    }
                }
                yield return 0;
            }
        }

        private IEnumerator OnTwoScale()
        {
            Vector3 rawScale = controlTarget.localScale;
            foreach (var touch in Input.touches)
            {
                originalPosition[touch.fingerId] = touch.position;
            }

            float rawFingersDistance = Vector2.Distance(originalPosition[Input.GetTouch(0).fingerId], originalPosition[Input.GetTouch(1).fingerId]);

            while (Input.touchCount == 2 &&
                    originalPosition.ContainsKey(Input.GetTouch(0).fingerId) &&
                    originalPosition.ContainsKey(Input.GetTouch(1).fingerId))
            {
                if (!controlTarget) { yield break; }
                float scaleFactor = rawFingersDistance / Vector2.Distance(Input.GetTouch(0).position, Input.GetTouch(1).position);
                controlTarget.localScale = rawScale / scaleFactor;
                yield return 0;
            }
        }

        private void GetRelativeTouch(Vector2 delta, out Vector3 xMovement, out Vector3 yMovement)
        {
            if (delta != Vector2.zero)
            {
                Vector3 startPoint = Camera.main.ScreenToWorldPoint(new Vector3(0f, 0f, 300f));
                Vector3 endPoint = Camera.main.ScreenToWorldPoint(new Vector3(delta.x, delta.y, 300f));
                Vector3 moveDirection = endPoint - startPoint;
                Vector3 relaForward = Vector3.ProjectOnPlane(Camera.main.transform.forward, Vector3.up).normalized;
                if (relaForward == Vector3.zero)
                {
                    xMovement = moveDirection;
                    yMovement = Vector3.zero;
                }
                else
                {
                    Vector3 relaRight = Vector3.Cross(Vector3.up, relaForward);
                    xMovement = Vector3.Project(moveDirection, relaRight);
                    Vector3 temp = moveDirection - xMovement;
                    if (temp != Vector3.zero)
                    {
                        if (Vector3.Dot(Vector3.up, temp.normalized) == 0)
                        {
                            yMovement = temp.magnitude * ((Vector3.Dot(relaForward, temp.normalized) > 0 ? relaForward : -relaForward));
                        }
                        else
                        {
                            yMovement = temp.magnitude * ((Vector3.Dot(Vector3.up, temp.normalized) > 0 ? relaForward : -relaForward));
                        }
                    }
                    else
                    {
                        yMovement = Vector3.zero;
                    }
                }
            }
            else
            {
                xMovement = Vector3.zero;
                yMovement = Vector3.zero;
            }
        }
    }
}
