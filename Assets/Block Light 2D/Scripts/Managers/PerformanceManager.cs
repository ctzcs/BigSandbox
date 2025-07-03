using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

/// <summary>
/// Tracks millisecond usage and allows scripts to request whether or not they are allowed to continue execution
/// for the current frame. Only applies to Coroutines of scripts that request HitFrameLimit().
/// </summary>
public class PerformanceManager : Singleton<PerformanceManager>
{
    public static Stopwatch Stopwatch = new Stopwatch();

    public bool ManagePerformance { get; set; }
    public int MaxUsableMilliseconds { get; set; }

    public SliderData performanceData;    


    private void Awake()
    {
        ManagePerformance = true;        
        OnUpdateMaxMilliseconds();

        Stopwatch.Start();
        StartCoroutine(PerformFrameManagement());
    }

    /// <summary>
    /// Called at script load and whenever the Max Milliseconds slider value has changed.
    /// Sets the maximum usable milliseconds before scripts are forced to yield.
    /// </summary>
    public void OnUpdateMaxMilliseconds()
    {
        MaxUsableMilliseconds = (int)performanceData.GetSliderData(SliderData.SliderField.PERFORMANCE_MS_MAX);
    }

    /// <summary>
    /// Returns whether the processing limit has been reached for the current frame.
    /// </summary>
    /// <returns></returns>
    public bool HitFrameLimit()
    {
        if (!ManagePerformance)
            return false;

        return MaxUsableMilliseconds - (int)Stopwatch.ElapsedMilliseconds > 0 ? false : true;
    }

    /// <summary>
    /// Infinite timer that executes at the end of each frame and manages frame time tracking setup.
    /// </summary>
    /// <returns></returns>
    private IEnumerator PerformFrameManagement()
    {
        while (true)
        {
            yield return new WaitForEndOfFrame();
            Stopwatch.Reset();
            Stopwatch.Start();
        }
    }
}