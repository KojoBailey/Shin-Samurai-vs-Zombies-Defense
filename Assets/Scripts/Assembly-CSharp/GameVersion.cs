using System;
using UnityEngine;

public class GameVersion : Singleton<GameVersion> {
	public const string OBB_FILE_NAME_MASK = "main.{0}.com.glu.samuzombie.obb";

	public const int VersionCode = 340;
	public int Major = 3;
	public int Minor = 4;
	public int Revision = 0;
	public int bundleMajor = 3;
	public int bundleMinor = 4;
	public int bundleRevision = 0;

	private BuildInfo buildInfo;

	public string onlineAssetBundlePath {
		get {
			string empty = string.Empty;
			empty = (Debug.isDebugBuild ? ((!ApplicationUtilities.IsBuildType("amazon")) ? "https://s3.amazonaws.com/griptonite/svz/android/DLC/google/stage/version{0}_{1}_{2}/Registry.assetbundle" : "https://s3.amazonaws.com/griptonite/svz/android/DLC/amazon/stage/version{0}_{1}_{2}/Registry.assetbundle") : ((!ApplicationUtilities.IsBuildType("amazon")) ? "https://d2lk18ssnvgdhj.cloudfront.net/svz/android/DLC/google/live/version{0}_{1}_{2}/Registry.assetbundle" : "https://d2lk18ssnvgdhj.cloudfront.net/svz/android/DLC/amazon/live/version{0}_{1}_{2}/Registry.assetbundle"));
			return string.Format(empty, bundleMajor.ToString(), bundleMinor.ToString(), bundleRevision.ToString());
		}
	}

	public string language {
		set {
			MultiLanguages.SetCurrentLanguage(value);
		}
		get {
			if (MultiLanguages.isMultiLanguages) {
				return MultiLanguages.GetCurrentLanguageToString();
			}
			return "Default";
		}
	}

	public string fontSet {
		get {
			if (MultiLanguages.isMultiLanguages) {
				return MultiLanguages.GetCurrentLanguageToString();
			}
			return "Default";
		}
	}

	public BuildInfo BuildInfo {
		get {
			return buildInfo;
		}
	}

	public GameVersion() {
		buildInfo = Resources.Load("BuildInfo") as BuildInfo;
		if (buildInfo == null) {
			buildInfo = ScriptableObject.CreateInstance<BuildInfo>();
			buildInfo.AppVersion = ToString();
			buildInfo.BuildTag = "SvZ_" + DateTime.Now.ToString("yyyyMMdd-HHmm") + "_A";
		}
	}

	public override string ToString() {
		string result = string.Format("{0}.{1}.{2}", Major.ToString(), Minor.ToString(), Revision.ToString());
		if (Debug.isDebugBuild)
				result += "D";
		return result;
	}
}
