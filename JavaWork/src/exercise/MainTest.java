package exercise;

import java.io.IOException;

public class MainTest {

	public static void main(String[] args) throws IOException {
		// TODO Auto-generated method stub
		Exercise exercise = new Exercise();
		exercise.Init();
		for(int i = exercise.getFileNum() - 1; i >= 0; i--) {
			exercise.startExercise(i);
		}
	}
}
