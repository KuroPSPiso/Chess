package rb.org;

import javafx.scene.control.TextArea;

import java.io.IOException;
import java.io.OutputStream;

/**
 * Created by bogaa on 6/4/2017.
 */
public class Console extends OutputStream {

    private TextArea output;

    public Console(TextArea ta) {
        this.output = ta;
    }

    @Override
    public void write(int i) throws IOException {
        output.appendText(String.valueOf((char) i));
    }

    public void println(String s){

        String oldData = output.getText();
        if(oldData.length() < 50) {
            output.setText(s + "\n" + oldData);
        }
        else
        {
            output.setText(s + "\n" + oldData.substring(0,50));
        }
//        for (char c : s.toCharArray()) {
//            try {
//                write(c);
//            } catch (IOException e) {
//                e.printStackTrace();
//            }
//        }
//
//        String newLine = System.getProperty("line.separator");
//
//        for (char c : newLine.toCharArray()) {
//            try {
//                write(c);
//            } catch (IOException e) {
//                e.printStackTrace();
//            }
//        }
    }

    public void print(String s){
        for (char c : s.toCharArray()) {
            try {
                write(c);
            } catch (IOException e) {
                e.printStackTrace();
            }
        }
    }
}
