package exercise;

import java.io.IOException;

public class MainTest {

	public static void main(String[] args) throws IOException {
		// TODO Auto-generated method stub
		Exercise exercise = new Exercise();
		exercise.Init();
		int start = 16;
		int length = 5;
		for(int i = 0; i < length; i++) {
			exercise.startExercise(i + start);
		}
	}
}
