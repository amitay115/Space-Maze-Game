using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class SwipeControl : MonoBehaviour
{
    [HideInInspector]
    public bool tap, doubletap, swipeLeft, swipeRight, swipeUp, swipeDown;
    private bool isDraging = false, _swipetap;
    private Vector2 startTouch, swipeDelta;
    public float DOUBLE_CLICK_TIME = .2f;
    private float lastClickTime;

    private void Update()
    {
        if (Time.timeScale == 0f || !GameController.manager.InGame) return;
        tap = swipeDown = swipeUp = swipeLeft = swipeRight = false;
        #region Standalone Inputs
        if (Input.GetMouseButtonDown(0))
        {
            tap = true;
            isDraging = true;
            startTouch = Input.mousePosition;
        }
        else if (Input.GetMouseButtonUp(0))
        {
            isDraging = false;
            Reset();
        }
        #endregion
        #region Mobile Input
        if (Input.touches.Length > 0)
        {
            if (Input.touches[0].phase == TouchPhase.Began)
            {
                tap = true;
                isDraging = true;
                startTouch = Input.touches[0].position;
            }
            else if (Input.touches[0].phase == TouchPhase.Ended || Input.touches[0].phase == TouchPhase.Canceled)
            {
                isDraging = false;
                Reset();
            }
        }
        #endregion
        swipeDelta = Vector2.zero;
        if (isDraging)
        {
            if (Input.touches.Length < 0)
                swipeDelta = Input.touches[0].position - startTouch;
            else if (Input.GetMouseButton(0))
                swipeDelta = (Vector2)Input.mousePosition - startTouch;
        }
        if (swipeDelta.magnitude > 100)
        {
            float x = swipeDelta.x;
            float y = swipeDelta.y;
            if (Mathf.Abs(x) > Mathf.Abs(y))
            {
                if (x < 0)
                    swipeLeft = true;
                else
                    swipeRight = true;
            }
            else
            {
                if (y < 0)
                    swipeDown = true;
                else
                    swipeUp = true;
            }
            StartCoroutine(SwipeTap());
            Reset();
        }
        if (tap)
        {
            DoubleTapped();
        }
    }
    public void DoubleTapped()
    {
        if (Time.timeScale == 0f) return;
        if ((Time.time - lastClickTime) <= DOUBLE_CLICK_TIME && !_swipetap)
        {
            StopAllCoroutines();
            StartCoroutine(DoubleTap());
        }
        lastClickTime = Time.time;
    }
    private void Reset()
    {
        startTouch = swipeDelta = Vector2.zero;
        isDraging = false;
    }
    IEnumerator DoubleTap()
    {
        doubletap = true;
        yield return new WaitForSeconds(0.1f);
        doubletap = false;
    }
    IEnumerator SwipeTap()
    {
        _swipetap = true;
        yield return new WaitForSeconds(0.2f);
        _swipetap = false;
    }
}