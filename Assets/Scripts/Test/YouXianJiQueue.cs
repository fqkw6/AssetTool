using System.Collections;
using System.Collections.Generic;
using Priority_Queue;
using UnityEngine;
public class YouXianJiQueue : MonoBehaviour {
    Queue<CData> qu = new Queue<CData> ();
    /// <summary>
    /// 快速优先级队列
    /// </summary>
    /// <typeparam name="CData"></typeparam>
    /// <returns></returns>
    FastPriorityQueue<CData> quw = new FastPriorityQueue<CData> (2);
    /// <summary>
    /// 稳定优先级队列
    /// </summary>
    /// <typeparam name="CData"></typeparam>
    /// <returns></returns>
    Priority_Queue.StablePriorityQueue<CData> queue = new Priority_Queue.StablePriorityQueue<CData> (2);

    void Start () {

        AddData (new CData ("ww"), 2);
        AddData (new CData ("ddd"), 10);
        AddData (new CData ("ws"), 80);
        AddData (new CData ("ss"), 90);
        AddData (new CData ("ddffd"), 100);

    }

    void Update () {
        if (queue.Count > 0) {
            Debug.LogError (queue.Dequeue ().label);
        }
    }

    /// <summary>
    /// 自动扩容
    /// </summary>
    /// <param name="data"></param>
    /// <param name="f"></param>
    void AddData (CData data, float f) {
        if (queue.Count >= queue.MaxSize) {
            queue.Resize (queue.MaxSize * 2);
        }
        queue.Enqueue (data, f);
    }
}
class CData : StablePriorityQueueNode {
    public string label;
    int index;
    public CData (string label) {
        this.label = label;
    }
}