package exercise;

import java.io.BufferedReader;
import java.io.FileInputStream;
import java.io.IOException;
import java.io.InputStreamReader;
import java.util.ArrayList;
import java.util.List;

public class Exercise {
	private List<Topic> topics = new ArrayList<Topic>();
	private List<Topic> rightList = new ArrayList<Topic>();
	private List<Topic> wrongList = new ArrayList<Topic>();
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
		System.out.println("Total topic num is " + topics.size());
		for(int i = 0; i < topics.size(); i++) {
			 Topic topic = topics.get(i);
			 if(answerOneTopic(topic))
				 rightList.add(topic);
			 else {
				 wrongList.add(topic);
			 }
		}
		System.out.println("wrong num is " + wrongList.size());
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
