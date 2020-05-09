using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InteractMenu : MonoBehaviour
{
    public Player player;
    GameObject Move;
    GameObject Interact;
    GameObject Inspect;
    public float wheelRadius = 50.0f;
    GameObject currentSelected;
    bool wheelOpened;
    List<Coroutine> allCoroutines;
    // Start is called before the first frame update
    void Start()
    {
        allCoroutines = new List<Coroutine>();

        Move = transform.Find("Move").gameObject;
        Interact = transform.Find("Interact").gameObject;
        Inspect = transform.Find("Inspect").gameObject;

        Move.GetComponent<Button>().onClick.AddListener(onMove);
        Interact.GetComponent<Button>().onClick.AddListener(onInteract);
        Inspect.GetComponent<Button>().onClick.AddListener(onInspect);
        closeAllWheel();

        EventManager.addListener(Events.CLICK, click);
    }

    void click(UnityEngine.Object n)
    {
        if (currentSelected == n)
        {
            closeAllWheel();
            currentSelected = null;
        }
        else
        {
            currentSelected = (GameObject)n;
            NPC interactData = currentSelected.GetComponent<NPC>();
            if (interactData != null)
            {
                List<GameObject> selectables = new List<GameObject>();
                if (interactData.Move) { selectables.Add(Move); }
                if (interactData.Interact) { selectables.Add(Interact); }
                if (interactData.Inspect) { selectables.Add(Inspect); }

                if (wheelOpened)
                {
                    closeAllWheel();
                }

                openWheel(selectables);
            }
        }
    }

    void openWheel(List<GameObject> selection)
    {
        wheelOpened = true;
        Vector3 screenPos = Camera.main.WorldToScreenPoint(currentSelected.transform.position);
        RectTransform thisRect = gameObject.GetComponent<RectTransform>();
        thisRect.position = screenPos;
        float angleAdjustment = 0;
        if (selection.Count == 2)
        {
            angleAdjustment = (float)-Math.PI / 2;
        } else if (selection.Count == 3)
        {
            angleAdjustment = (float) (-Math.PI / (2 / 0.65));
        }
        for (int i = 0; i < selection.Count; i++)
        {
            RectTransform go = selection[i].GetComponent<RectTransform>();
            float targetAngle = (float) (2 * Math.PI / selection.Count) * i;

            Vector3 newPos = new Vector3(
                Mathf.Sin(targetAngle + angleAdjustment) * wheelRadius,
                Mathf.Cos(targetAngle + angleAdjustment) * wheelRadius,
                0f
            );
            // go.localPosition = newPos;
            selection[i].SetActive(true);
            IEnumerator radiate = radiateOutwards(go, newPos);
            allCoroutines.Add(StartCoroutine(radiate));
        }
    }
    IEnumerator radiateOutwards(RectTransform button, Vector3 end)
    {
        while (Vector3.Distance(button.localPosition, end) > 0.1f)
        {
            button.localPosition = Vector3.Lerp(button.localPosition, end, 10 * Time.deltaTime);
            yield return null;
        }

        yield return null;
    }

    void onMove()
    {
        EventManager.triggerEvent(Events.MOVE_TO, currentSelected);
        closeAllWheel();
    }

    void onInspect()
    {
        if (!player.isPlayerCloseTo(currentSelected))
        {
            // Event queue not implemented
        }
        EventManager.triggerEvent(Events.INSPECT, currentSelected);
        closeAllWheel();
    }
    void onInteract()
    {
        if (!player.isPlayerCloseTo(currentSelected))
        {
            // Event queue not implemented
        }
        EventManager.triggerEvent(Events.INTERACT, currentSelected);
        closeAllWheel();
    }

    void closeAllWheel()
    {
        foreach (Coroutine coro in allCoroutines)
        {
            StopCoroutine(coro);
        }
        allCoroutines = new List<Coroutine>();
        wheelOpened = false;
        Move.GetComponent<RectTransform>().localPosition = Vector3.zero;
        Interact.GetComponent<RectTransform>().localPosition = Vector3.zero;
        Inspect.GetComponent<RectTransform>().localPosition = Vector3.zero;
        Move.SetActive(false);
        Interact.SetActive(false);
        Inspect.SetActive(false);
    }
}
