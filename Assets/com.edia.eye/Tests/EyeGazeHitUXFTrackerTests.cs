using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using UnityEngine;
using Edia.Eye;
using UXF;

namespace Edia.Eye.Tests
{
    [TestFixture]
    public class EyeGazeHitUXFTrackerTests
    {
        private EyeGazeHitUXFTracker _tracker;

        [SetUp]
        public void Setup()
        {
            var go = new GameObject("Tracker");
            _tracker = go.AddComponent<EyeGazeHitUXFTracker>();
        }

        [TearDown]
        public void TearDown()
        {
            if (_tracker != null)
                Object.DestroyImmediate(_tracker.gameObject);
        }

        [Test]
        public void AddSample_IncreasesSampleCount()
        {
            float[] sample = new float[] { 1f, 2f, 3f, 0.5f, 0.5f };
            _tracker.AddSample(sample);

            var fieldInfo = typeof(EyeGazeHitUXFTracker).GetField("receivedGazeHitSamples", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            var samples = (List<float[]>)fieldInfo.GetValue(_tracker);
            
            Assert.AreEqual(1, samples.Count);
            Assert.AreEqual(sample, samples[0]);
        }

        [Test]
        public void GetCurrentValues_ReturnsCorrectDataRow()
        {
            float[] sample = new float[] { 1.1f, 2.2f, 3.3f, 0.4f, 0.5f };
            _tracker.AddSample(sample);

            // GetCurrentValues is protected
            var methodInfo = typeof(EyeGazeHitUXFTracker).GetMethod("GetCurrentValues", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            var row = (UXFDataRow)methodInfo.Invoke(_tracker, null);

            Assert.AreEqual(1.1f, (float)row.First(i => i.columnName == "hit_x_local").value);
            Assert.AreEqual(2.2f, (float)row.First(i => i.columnName == "hit_y_local").value);
            Assert.AreEqual(3.3f, (float)row.First(i => i.columnName == "hit_z_local").value);
            Assert.AreEqual(0.4f, (float)row.First(i => i.columnName == "uv_x").value);
            Assert.AreEqual(0.5f, (float)row.First(i => i.columnName == "uv_y").value);
        }
        
        [Test]
        public void GetCurrentValues_WithNoSamples_ReturnsNaNRow()
        {
            var methodInfo = typeof(EyeGazeHitUXFTracker).GetMethod("GetCurrentValues", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            var row = (UXFDataRow)methodInfo.Invoke(_tracker, null);

            Assert.IsTrue(float.IsNaN((float)row.First(i => i.columnName == "hit_x_local").value));
            Assert.IsTrue(float.IsNaN((float)row.First(i => i.columnName == "hit_y_local").value));
            Assert.IsTrue(float.IsNaN((float)row.First(i => i.columnName == "hit_z_local").value));
            Assert.IsTrue(float.IsNaN((float)row.First(i => i.columnName == "uv_x").value));
            Assert.IsTrue(float.IsNaN((float)row.First(i => i.columnName == "uv_y").value));
        }
    }
}
