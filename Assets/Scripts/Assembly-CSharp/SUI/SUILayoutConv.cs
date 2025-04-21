using UnityEngine;

public class SUILayoutConv {
	public static string FormatNewlines(string str) {
		str = str.Replace("&#10;", "\r");
		str = str.Replace("\\r", "\r");
		str = str.Replace("&#13;", "\n");
		str = str.Replace("\\n", "\n");
		return str;
	}

	public static bool GetBool(string str) {
		str = str.ToLower(); // Store to save processing power.
		switch (str) {
			case "true": case "yes": case "on": case "1":
				return true;
		}
		return false;
	}

	public static int[] GetIntegerList(string strList) {
		string[] input = strList.Split(',');
		int[] result = new int[input.Length];
		for (int i = 0; i < input.Length; i++) {
			result[i] = int.Parse(input[i]);
		}
		return result;
	}

	public static float[] GetFloatList(string strList) {
		string[] input = strList.Split(',');
		float[] result = new float[input.Length];
		for (int i = 0; i < input.Length; i++) {
			result[i] = float.Parse(input[i]);
		}
		return result;
	}

	// Convert relative coordinate to absolute.
	private static float ConvertRelativeCoordinate(string input, float maxLength) {
		float result = 0f;
		string buffer = input.Trim(); // Save original input for debugging.
		bool multiply = false;

		while (buffer.Length > 0) {
			switch (buffer[0]) {
				case 'E': // Edge, at max width/height.
					result = maxLength;
					buffer = buffer.Substring(1).Trim();
					break;
				case 'C': // Centre
					result = maxLength / 2f;
					buffer = buffer.Substring(1).Trim();
					break;
				case '*': // Multiplication
					multiply = true;
					buffer = buffer.Substring(1).Trim();
					break;
				case '+': // Addition
					buffer = buffer.Substring(1).Trim();
					break;
				default:
					if (buffer[0] == '-' || buffer[0] == '.' || (buffer[0] >= '0' && buffer[0] <= '9')) {
						float number = float.Parse(buffer); // Get entire value.
						if (multiply) result *= number;
							else result += number;
						return result;
					} else {
						Debug.LogWarningFormat("Invalid relative coordinate: {}", input);
						return result;
					}
			}
		}
		return result;
	}

	private static float[] GetAbsoluteCoordinates(string input) {
		string[] relativeCoordinates = input.Split(','); // Coordinates are separated by a comma (,)
		float[] absoluteCoordinates = new float[relativeCoordinates.Length]; // Not limited to single points (x,y), as could be rectangle.
		for (int i = 0; i < relativeCoordinates.Length; i++) {
			absoluteCoordinates[i] = ConvertRelativeCoordinate(relativeCoordinates[i], (i % 2 == 1) ? SUIScreen.Height : SUIScreen.Width);
		}
		return absoluteCoordinates;
	}

	public static Vector2 GetVector2(string strList) {
		float[] coordinates = GetAbsoluteCoordinates(strList);
		if (coordinates.Length == 2) {
			return new Vector2(coordinates[0], coordinates[1]);
		}
		Debug.LogWarningFormat("Vector2 requires 2 coordinates, but was given {1}: {2}", coordinates.Length, strList);
		return Vector2.zero;
	}

	public static SUILayout.NormalRange GetNormalRange(string strList) {
		float[] coordinates = GetAbsoluteCoordinates(strList);
		if (coordinates.Length == 2) {
			SUILayout.NormalRange result = new SUILayout.NormalRange(coordinates[0], coordinates[1]);
			if (result.min <= result.max) {
				return result;
			}
		}
		Debug.LogWarningFormat("NormalRange requires 2 coordinates, but was given {1}: {2}", coordinates.Length, strList);
		return default(SUILayout.NormalRange);
	}

	public static Rect GetRect(string strList) {
		float[] coordinates = GetAbsoluteCoordinates(strList);
		if (coordinates.Length == 4) {
			return new Rect(coordinates[0], coordinates[1], coordinates[2], coordinates[3]);
		}
		Debug.LogWarningFormat("Rectangle requires 4 coordinates, but was given {1}: {2}", coordinates.Length, strList);
		return default(Rect);
	}

	public static Color GetColor(string strList) {
		switch (strList.Trim().ToLower()) {
			case "clear":
				return Color.clear;
			case "black":
				return Color.black;
			case "gray":
			case "grey":
				return Color.gray;
			case "white":
				return Color.white;
			case "red":
				return Color.red;
			case "yellow":
				return Color.yellow;
			case "green":
				return Color.green;
			case "cyan":
				return Color.cyan;
			case "blue":
				return Color.blue;
			case "magenta":
				return Color.magenta;
			default: {
				// Get RGB or RGBA
				float[] floatList = GetFloatList(strList);
				if (floatList.Length == 3) // RGB
					return new Color(floatList[0], floatList[1], floatList[2]);
				if (floatList.Length == 4) // RGBA
					return new Color(floatList[0], floatList[1], floatList[2], floatList[3]);
				Debug.LogWarningFormat("Invalid colour: {1}", strList);
				return default(Color);
			}
		}
	}

	public static TextAnchor GetAnchor(string str) {
		switch (str.ToLower()) {
			case "lowercenter":
			case "lowercentre":
				return TextAnchor.LowerCenter;
			case "lowerleft":
				return TextAnchor.LowerLeft;
			case "lowerright":
				return TextAnchor.LowerRight;
			case "middlecenter":
			case "middlecentre":
				return TextAnchor.MiddleCenter;
			case "middleleft":
				return TextAnchor.MiddleLeft;
			case "middleright":
				return TextAnchor.MiddleRight;
			case "uppercenter":
			case "uppercentre":
				return TextAnchor.UpperCenter;
			case "upperleft":
				return TextAnchor.UpperLeft;
			case "upperright":
				return TextAnchor.UpperRight;
			default:
				Debug.LogWarningFormat("Invalid anchor: {1}", str);
				return TextAnchor.LowerCenter;
		}
	}

	public static TextAlignment GetAlignment(string str) {
		switch (str.ToLower()) {
			case "center":
			case "centre":
				return TextAlignment.Center;
			case "right":
				return TextAlignment.Right;
			default:
				return TextAlignment.Left;
		}
	}

	public static Ease.Function GetEaseFunc(string easeFuncStr) {
		switch (easeFuncStr.ToLower()) {
			case "sinein":
				return Ease.SineIn;
			case "sineout":
				return Ease.SineOut;
			case "sineinout":
				return Ease.SineInOut;
			case "quadin":
				return Ease.QuadIn;
			case "quadout":
				return Ease.QuadOut;
			case "quadinout":
				return Ease.QuadInOut;
			case "cubicin":
				return Ease.CubicIn;
			case "cubicout":
				return Ease.CubicOut;
			case "cubicinout":
				return Ease.CubicInOut;
			case "quartin":
				return Ease.QuartIn;
			case "quartout":
				return Ease.QuartOut;
			case "quartinout":
				return Ease.QuartInOut;
			case "quintin":
				return Ease.QuintIn;
			case "quintout":
				return Ease.QuintOut;
			case "quintinout":
				return Ease.QuintInOut;
			case "expoin":
				return Ease.ExpoIn;
			case "expoout":
				return Ease.ExpoOut;
			case "expoinout":
				return Ease.ExpoInOut;
			case "circin":
				return Ease.CircIn;
			case "circout":
				return Ease.CircOut;
			case "circinout":
				return Ease.CircInOut;
			case "backin":
				return Ease.BackIn;
			case "backout":
				return Ease.BackOut;
			case "backinout":
				return Ease.BackInOut;
			case "bouncein":
				return Ease.BounceIn;
			case "bounceout":
				return Ease.BounceOut;
			case "bounceinout":
				return Ease.BounceInOut;
			default:
				return Ease.Linear;
		}
	}
}
