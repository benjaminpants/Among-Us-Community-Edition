using System;
using System.Collections;
using UnityEngine;

public class WaitForSecondsRealtimeSkipable: CustomYieldInstruction
{
    public float waitTime { get; set; }
    float m_WaitUntilTime = -1;
    private bool CanSkip = true;

    public override bool keepWaiting
    {
        get
        {
            if (m_WaitUntilTime < 0)
            {
                m_WaitUntilTime = Time.realtimeSinceStartup + waitTime;
            }

            bool wait = Time.realtimeSinceStartup < m_WaitUntilTime;
            if (!wait)
            {
                // Reset so it can be reused.
                Reset();
            }
            return wait;
        }
    }

    public WaitForSecondsRealtimeSkipable(float time)
    {
        waitTime = time;
    }

    public void Skip()
    {
        if (!CanSkip) return;
        m_WaitUntilTime = 0;
        CanSkip = false;
    }

    public override void Reset()
    {
        m_WaitUntilTime = -1;
    }
}

public abstract class CustomYieldInstruction : IEnumerator
{
    public abstract bool keepWaiting
    {
        get;
    }

    public object Current
    {
        get
        {
            return null;
        }
    }
    public bool MoveNext() { return keepWaiting; }
    public virtual void Reset() { }
}
