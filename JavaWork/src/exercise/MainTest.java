package exercise;

import java.io.IOException;

public class MainTest {

	public static void main(String[] args) throws IOException {
		// TODO Auto-generated method stub
		Exercise exercise = new Exercise();
		exercise.Init();
		int start = 13;
		int length = 1;
		for(int i = 0; i < length; i++) {
			exercise.startExercise(i + start);
		}
	}
}
