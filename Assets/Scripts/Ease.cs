public class Ease {

	public static float InOutCubic (float time, float begin, float change, float duration) {
		time /= duration / 2;
		if (time < 1) return change / 2 * time * time * time + begin;
		time -= 2;
		return change / 2 * (time * time * time + 2) + begin;
	}

}
