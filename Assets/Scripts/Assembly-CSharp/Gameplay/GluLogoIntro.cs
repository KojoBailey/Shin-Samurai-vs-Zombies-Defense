using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Video;

public class GluLogoIntro : MonoBehaviour {
	private int updateCounter;
	private bool videoStarted = false;

	[SerializeField] private VideoPlayer videoPlayer;
	[SerializeField] private AudioSource audioSource;

	private void Update() {
		updateCounter++;
		switch (updateCounter) {
			case 4:
				NUF.AllowPortrait(false);
				break;
			case 8:
				PlayIntro();
				break;
			case 15:
				NUF.AllowPortrait(true);
				break;
		}

		if (videoStarted && (!videoPlayer.isPlaying || Input.GetMouseButton(0) || Input.GetKeyDown(KeyCode.Return)))
			SceneManager.LoadScene("OpeningCinematic");
	}

	private void PlayIntro() {
		string resolution = "1024x768";
		// switch (Screen.width) {
		// 	case 640:
		// 		resolution = "960x640";
		// 		break;
		// 	case 320:
		// 		resolution = "480x320";
		// 		break;
		// }
		string fileName = "Glu_logo_" + resolution + ".mp4";
		Debug.Log("Playing Glu intro:" + fileName);

		videoPlayer.renderMode = VideoRenderMode.CameraNearPlane;
		videoPlayer.targetCamera = Camera.main;
		videoPlayer.audioOutputMode = VideoAudioOutputMode.AudioSource;
		videoPlayer.EnableAudioTrack(0, true);
		videoPlayer.SetTargetAudioSource(0, audioSource);
		videoPlayer.Play();
		audioSource.Play();
		videoStarted = true;
	}
}
