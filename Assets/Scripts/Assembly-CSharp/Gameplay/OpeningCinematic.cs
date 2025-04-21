using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Video;

public class OpeningCinematic : MonoBehaviour {
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
				PlayCinematic();
				break;
			case 15:
				NUF.AllowPortrait(true);
				break;
		}

		if (videoStarted && (!videoPlayer.isPlaying || Input.GetMouseButton(0) || Input.GetKeyDown(KeyCode.Return)))
			SceneManager.LoadScene("TitleScreen");
	}

	private void PlayCinematic() {
		string fileName = "Video/SvZRough.mp4";
		Debug.Log("Playing cinematic: " + fileName);

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
