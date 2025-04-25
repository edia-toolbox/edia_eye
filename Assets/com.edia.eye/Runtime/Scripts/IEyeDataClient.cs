using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UXF;

namespace Edia.Eye {

	/// <summary>
	/// Interface to be implemented by data clients that receive data from the `EyeDataHandler`. 
	/// </summary>
	public interface IEyeDataClient
	{
		/// <summary>
		/// This method is called by the `EyeDataHandler` to push the current samples to the data clients. 
		/// </summary>
		/// <param name="currentSamples"></param>
		public void ProcessCurrentSamples (List<EyeDataPackage> currentSamples);

		// public void EnableRecording (bool onOff);
	}

}