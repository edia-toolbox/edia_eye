using System.Collections.Generic;
using System.Reflection;
using NUnit.Framework;
using UnityEngine;

namespace Edia.Eye.Tests
{
    [TestFixture]
    public class EyeDataHandlerTests
    {
        private EyeDataHandler _handler;

        [SetUp]
        public void Setup()
        {
            // Ensure no instance exists before test
            var existing = UnityEngine.Object.FindFirstObjectByType<EyeDataHandler>();
            if (existing != null)
            {
                UnityEngine.Object.DestroyImmediate(existing.gameObject);
            }
            
            _handler = EyeDataHandler.Instance;
        }

        [TearDown]
        public void TearDown()
        {
            if (_handler != null)
            {
                UnityEngine.Object.DestroyImmediate(_handler.gameObject);
            }
        }

        /// <summary>
        /// Verifies that adding an IEyeDataClient correctly registers it in the internal client list.
        /// </summary>
        [Test]
        public void AddDataClient_AddsClientToInternalList()
        {
            var go = new GameObject("MockClient");
            var client = go.AddComponent<MockClient>();
            _handler.AddDataClient(client);

            var fieldInfo = typeof(EyeDataHandler).GetField("_dataClients", BindingFlags.NonPublic | BindingFlags.Instance);
            var dataClients = (List<IEyeDataClient>)fieldInfo.GetValue(_handler);

            Assert.Contains(client, dataClients);
            Object.DestroyImmediate(go);
        }

        /// <summary>
        /// Verifies that adding an EyeDataPackage correctly registers it in the internal sample list.
        /// </summary>
        [Test]
        public void AddEyeDataPackage_AddsPackageToInternalList()
        {
            var package = new EyeDataPackage { eye = "left", isValid = true };
            _handler.AddEyeDataPackage(package);

            var propertyInfo = typeof(EyeDataHandler).GetProperty("_currentSamples", BindingFlags.NonPublic | BindingFlags.Instance);
            var currentSamples = (List<EyeDataPackage>)propertyInfo.GetValue(_handler);

            Assert.Contains(package, currentSamples);
        }


        private class MockClient : MonoBehaviour, IEyeDataClient
        {
            public void ProcessCurrentSamples(List<EyeDataPackage> currentSamples) { }
        }
    }
}
