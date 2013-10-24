#define UEPROFILER_ENABLED

using System;
using UnityEngine;

namespace UniverseEngine
{
    public class UEProfiler : MonoBehaviour
    {
#if UEPROFILER_ENABLED
        static private bool profilerEnabled;
        
        public void Start()
        {
            profilerEnabled = true;
        }
        
        public void Update()
        {
            sampledFrames++;
            //Clear();
        }
        
        private const int MAX_SAMPLES = 8192;
        
        private struct Sample
        {
            public string id;
            public int parent;
            public long startTicks;
            public long endTicks;
        }
        
        static private Sample[] samples = new Sample[MAX_SAMPLES];
        static private int currentIndex = 0;
        static private int maxSamples = 0;
        static private int sampledFrames = 0;
#endif
        
        static public void Clear()
        {
#if UEPROFILER_ENABLED            
            currentIndex = 0;
            maxSamples = 0;
            sampledFrames = 0;
#endif
        }
        
        static public void BeginSample(string id)
        {
#if UEPROFILER_ENABLED
            if (!profilerEnabled)
                return;
            
            samples[maxSamples].id = id;
            samples[maxSamples].startTicks = DateTime.Now.Ticks;
            samples[maxSamples].endTicks = 0;
            samples[maxSamples].parent = currentIndex;
            
            currentIndex = maxSamples;
            
            maxSamples++;
            
            //Disable the profiler if we run out of samples
            if (maxSamples == samples.Length)
            {
                Clear();
                profilerEnabled = false;
            }
#endif
        }
        
        static public void EndSample()
        {
#if UEPROFILER_ENABLED            
            if (!profilerEnabled)
                return;
            
            samples[currentIndex].endTicks = DateTime.Now.Ticks;
            currentIndex = samples[currentIndex].parent;
#endif
        }
        
        static public TimeSpan GetSampleTime(string id)
        {
#if UEPROFILER_ENABLED            
            if (!profilerEnabled)
                return new TimeSpan(0);
            
            long ticks = 0;
            
            for (int i = 0; i < maxSamples; i++)
                if (samples[i].id == id)
                    ticks += samples[i].endTicks - samples[i].startTicks;
            
            if (sampledFrames > 0)
                ticks /= sampledFrames;
            
            return new TimeSpan(ticks);
#else
            return new TimeSpan(0);
#endif
        }
    }
}

