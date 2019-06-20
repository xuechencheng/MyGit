package exercise;

import java.io.BufferedReader;
import java.io.FileInputStream;
import java.io.IOException;
import java.io.InputStreamReader;
import java.util.ArrayList;
import java.util.Iterator;
import java.util.List;

public class Exercise {
	private List<Topic> topics = new ArrayList<Topic>();
	private BufferedReader bf = null;
	private void loadTopicFromFile(String title, int chapter) throws IOException {
		topics.clear();
		String filePath = "src\\files\\" + title;
		if(chapter != 0) {
			filePath += chapter;
		}
		filePath += ".txt";
		//FileInputStream inputStream = new FileInputStream(filePath);
		BufferedReader bufferedReader=new BufferedReader(new InputStreamReader(new FileInputStream(filePath),"UTF-8"));
		//BufferedReader bufferedReader = new BufferedReader(new InputStreamReader(inputStream));
		String question = null;
		String answer = null;
		while((question = bufferedReader.readLine()) != null &&
				(answer = bufferedReader.readLine()) != null)
		{
			System.out.println(question + "  ---   " + answer);
			topics.add(new Topic(question, answer));
		}
		//inputStream.close();
		bufferedReader.close();
	}

	public void startExercise(String title, int chapter) throws IOException {
		loadTopicFromFile(title, chapter);
		
		while(topics.size() > 0) {
			System.out.println("Total topic num is " + topics.size());
			Iterator<Topic> iterator = topics.iterator();
			while(iterator.hasNext()) {
				Topic topic = iterator.next();
				if(answerOneTopic(topic)) {
					iterator.remove();
				}
			}
		}
		endExercise();
	}
	
	private boolean answerOneTopic(Topic topic) throws IOException {
		System.out.println(topic.getQuestion());
		int time = 0;
		while(time++ < 3) {
			if(readOneLine().equals(topic.getAnswer())) {
				System.out.println("Right!!!");
				return true;
			}else {
				System.out.println("Wrong!!!    Try Again!!!");
			}
		}
		System.out.println(topic.getAnswer());
		return false;
	}
	
	private void endExercise() throws IOException {
		bf.close();
	}
	
	private String readOneLine() throws IOException {
		if(bf == null)
			bf = new BufferedReader(new InputStreamReader(System.in));
		return bf.readLine();
	}
}
