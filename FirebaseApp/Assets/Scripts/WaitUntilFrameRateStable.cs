using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// フレームレートが安定になるまで待機
/// </summary>
public class WaitUntilFrameRateStable : CustomYieldInstruction
{
    private readonly List<float> deltaTimes = new List<float>(SAMPLE);
    private const int SAMPLE = 5;
    private const float DEVIATION = 12f;

    /// <summary>
    /// フレームレートが安定になるまで待機
    /// </summary>
    private bool IsFrameRatesStable()
    {
        int average = 0;
        foreach (int val in deltaTimes)
        {
            average += val;
        }
        average /= deltaTimes.Count;
        foreach (int val in deltaTimes)
        {
            if (val < average - DEVIATION)
            {
                return false;
            }
            else if (val > average + DEVIATION)
            {
                return false;
            }
        }

        return true;
    }

    public override bool keepWaiting
    {
        get
        {
            deltaTimes.Add(Time.deltaTime);

            if (deltaTimes.Count >= SAMPLE)
            {
                if (IsFrameRatesStable())
                {
                    return false;
                }

                deltaTimes.RemoveAt(0);
            }

            return true;
        }
    }
}
