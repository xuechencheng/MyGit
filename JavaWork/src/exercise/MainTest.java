package exercise;

import java.io.IOException;

public class MainTest {

	public static void main(String[] args) throws IOException {
		// TODO Auto-generated method stub
		Exercise exercise = new Exercise();
		exercise.Init();
		for(int i = 6; i < 7; i++) {
			exercise.startExercise(i);
		}
	}
}
