using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class CoroutineUtility : MonoBehaviour {
    // Use this for initialization
    public Animator anim;
    public static CoroutineUtility Instance;
    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    public static CoroutineUtility GetInstance()
    {
        if (Instance == null)
        {
            Instance = new GameObject("CoroutineUtility").AddComponent<CoroutineUtility>();
            return Instance;
        }
        else
            return Instance;
    }

    void Start()
    {
    }

    // Update is called once per frame
    void Update() {
        //if (Input.GetKeyDown(KeyCode.A))
        //{
        //    Do().Play(anim, "hurt_salmon")
        //        .Wait(0.5f)
        //        .UIMove(anim.gameObject, new Vector2(0, 500), 1)
        //        .Play(anim, "cure")
        //        .Play(anim, "hurt_salmon")
        //        .Wait(1)
        //        .Play(anim, "hurt_salmon")
        //        .Go();
        //}
    }
    public CoroutineQueue Do()
    {
        CoroutineQueue animObj = new CoroutineQueue(this);        
        return animObj;
    }
}

public class CoroutineQueue
{
    public delegate void DefaultDelegate();
    public delegate void ReplenishDelegate(int[] index, int[] targetIndex);

    List<IEnumerator> waitQueue;
    CoroutineUtility coroutineUtility;
    bool isStartPlaying = false;
    public CoroutineQueue(CoroutineUtility animUtility)
    {
        waitQueue = new List<IEnumerator>();
        coroutineUtility = animUtility;
    }

    public CoroutineQueue Play(Animator animator, string animStateName)
    {
        waitQueue.Add(PlayAnimation(animator, animStateName));       

        return this;
    }

    public CoroutineQueue Wait(float time)
    {
        waitQueue.Add(WaitTime(time));
        return this;
    }

    public CoroutineQueue Move(GameObject obj, Vector3 newPos, float time)
    {
        waitQueue.Add(MoveObj(obj, newPos, time));
        return this;
    }

    public CoroutineQueue UIMove(GameObject obj, Vector2 newPos, float time)
    {
        waitQueue.Add(MoveUIObj(obj, newPos, time));
        return this;
    }

    public CoroutineQueue RadiusUIMove(GameObject obj, Vector2 newPos, float xRadius, float yRadius, float time)
    {
        waitQueue.Add(RadiusMoveUIObj(obj, newPos, xRadius, yRadius, time));
        return this;
    }

    public CoroutineQueue Then(DefaultDelegate call)
    {
        waitQueue.Add(DoCall(call));
        return this;
    }

    public CoroutineQueue DoEnumerator(IEnumerator ie)
    {
        waitQueue.Add(ie);
        return this;
    }

    public CoroutineQueue Then(ReplenishDelegate call, int[] index, int[] targetIndex)
    {
        waitQueue.Add(DoCall(call, index, targetIndex));
        return this;
    }

    public void Go()
    {
        if (!isStartPlaying)
        {
            isStartPlaying = true;
            coroutineUtility.StartCoroutine(DoQueue());
        }
    }

    

    IEnumerator PlayAnimation(Animator animator, string animStateName) {
        Debug.Log("Play");
        animator.Play(animStateName);
        yield return new WaitUntil(() => { return animator.GetCurrentAnimatorStateInfo(0).IsName(animStateName); });
        yield return new WaitUntil(() => { return !animator.GetCurrentAnimatorStateInfo(0).IsName(animStateName); });
    }

    IEnumerator WaitTime(float time)
    {
        yield return new WaitForSeconds(time);
    }

    IEnumerator MoveObj(GameObject obj, Vector3 newPos, float time)
    {
        Vector3 prePos = obj.transform.position;
        float t = 0;
        while (t < 1)
        {
            t += Time.deltaTime / time;
            if (t > 1)
                t = 1;
            obj.transform.position = Vector3.Lerp(prePos, newPos, t);
            yield return null;
        }
    }

    IEnumerator MoveUIObj(GameObject obj, Vector2 newPos, float time)
    {
        RectTransform rect = obj.GetComponent<RectTransform>();
        Vector2 prePos = rect.anchoredPosition;
        float t = 0;
        while (t < 1)
        {
            t += Time.deltaTime / time;
            if (t > 1)
                t = 1;
            rect.anchoredPosition = Vector2.Lerp(prePos, newPos, t);
            yield return null;
        }
    }

    IEnumerator RadiusMoveUIObj(GameObject obj, Vector2 newPos, float xRadius, float yRadius, float time)
    {
        RectTransform rect = obj.GetComponent<RectTransform>();
        Vector2 prePos = rect.anchoredPosition;
        float t = 0;
        while (t < 1)
        {
            t += Time.deltaTime / time;
            if (t > 1)
                t = 1;
            float x = Mathf.Lerp(rect.anchoredPosition.x, newPos.x, xRadius);
            float y = Mathf.Lerp(rect.anchoredPosition.y, newPos.y, yRadius);
            rect.anchoredPosition = new Vector2(x, y);
            yield return null;
        }
    }

    IEnumerator DoQueue()
    {
        while (waitQueue.Count > 0)
        {
            yield return coroutineUtility.StartCoroutine(waitQueue[0]);
            waitQueue.RemoveAt(0);
        }
    }

    IEnumerator DoCall(DefaultDelegate func)
    {
        func.Invoke();
        yield return null;
    }

    IEnumerator DoCall(ReplenishDelegate func, int[] index, int[] targetIndex)
    {
        func.Invoke(index, targetIndex);
        yield return null;
    }

}

public struct WaitForAnimatorState : IEnumerator
{
    Animator animator;
    string stateName;

    public WaitForAnimatorState(Animator animator, string stateName)
    {
        this.animator = animator;
        this.stateName = stateName;
    }

    bool IEnumerator.MoveNext()
    {
        return animator.GetCurrentAnimatorStateInfo(0).IsName(this.stateName);
    }

    object IEnumerator.Current
    {
        get
        {
            return null;
        }
    }

    void IEnumerator.Reset()
    {
    }
}