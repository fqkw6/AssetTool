#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;
using UnityEngine.Profiling;

namespace Leyoutech.Utility
{
    public static class DebugUtility
    {
        public const string LOG_ASSERT_EDITOR_CONDITIONAL = "LAE_E";

        public const string LOG_ASSERT_CONDITIONAL = LOG_ASSERT_EDITOR_CONDITIONAL;
        private const string DEFAULT_TAG = "Default";


        private const double BYTES_TO_MBYTES = 1.0 / 1024.0 / 1024.0;

        /// <summary>
        /// 相当于<see cref="Time.realtimeSinceStartup"/>
        /// 但是编辑器下也能使用
        /// </summary>
        private static System.Diagnostics.Stopwatch ms_Stopwatch;
        /// <summary>
        /// 只能在这个类中使用
        /// </summary>
        private static System.Text.StringBuilder ms_StringBuilderCache;

        static DebugUtility()
        {
            ms_Stopwatch = new System.Diagnostics.Stopwatch();
            ms_Stopwatch.Start();
            ms_StringBuilderCache = new System.Text.StringBuilder();
        }

        #region Time
        /// <summary>
        /// <see cref="Time.realtimeSinceStartup"/>
        /// </summary>
        public static long GetMillisecondsSinceStartup()
        {
            return ms_Stopwatch.ElapsedMilliseconds;
        }

        /// <summary>
        /// <see cref="Time.realtimeSinceStartup"/>
        /// </summary>
        public static long GetTicksSinceStartup()
        {
            return ms_Stopwatch.ElapsedTicks;
        }

        /// <summary>
        /// Tick to FPS
        /// </summary>
        /// <param name="ticks"></param>
        /// <returns></returns>
        public static float ConvertTicksToFPS(long ticks)
        {
            return 10000000.0f / ticks;
        }

        /// <summary>
        /// Convert to *h *m *s
        /// </summary>
        /// <param name="milliseconds"></param>
        /// <returns></returns>
        public static string FormatMilliseconds(long milliseconds)
        {
            long totalS = (long)(milliseconds / 1000.0);
            long h = (long)(totalS / 3600.0);
            long m = (long)((totalS - h * 3600) / 60.0);
            long s = totalS - h * 3600 - m * 60;
            return $"({h}h {m}m {s}s)";
        }
        #endregion



    }
}