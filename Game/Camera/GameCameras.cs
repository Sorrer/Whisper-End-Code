using Cinemachine;
using Game;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Game.Camera {
	public class GameCameras : MonoBehaviour, IGameInstance {

		private void Awake() {
			MainInstances.Add(this);
		}

		public CinemachineVirtualCamera MainCameraVC;
		public CameraShaker MainCameraShaker;

	}
}
