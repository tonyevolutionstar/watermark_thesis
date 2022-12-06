import org.apache.pdfbox.pdmodel.PDDocument;
import org.apache.pdfbox.text.PDFTextStripper;
import org.apache.pdfbox.text.TextPosition;

import java.io.*;
import java.nio.file.StandardOpenOption;
import java.util.List;

//https://www.tutorialkart.com/pdfbox/how-to-extract-coordinates-or-position-of-characters-in-pdf/
class PositionCharacter extends PDFTextStripper
{
    private static String file_name;
    public PositionCharacter() throws IOException {}

    /**
        * @throws IOException If there is an error parsing the document.
        */
    public static void main(String[] args) throws IOException {


        PDDocument document = null;
        file_name = args[0];
        String[] f = file_name.split(".pdf");

        File fi = new File(f[0]+"_pos.txt");

        if(fi.exists())
        {
            fi.delete();
        }
        else {
            fi.createNewFile();
        }

        String fileName = args[0];
        try {
            document = PDDocument.load( new File(fileName) );
            System.out.println(document);
            PDFTextStripper stripper = new PositionCharacter();
            stripper.setSortByPosition( true );
            stripper.setStartPage( 0 );
            stripper.setEndPage( document.getNumberOfPages() );

            Writer dummy = new OutputStreamWriter(new ByteArrayOutputStream());
            stripper.writeText(document, dummy);
        }
        finally {
            if( document != null ) {
                document.close();
            }
        }
    }

    /**
        * Override the default functionality of PDFTextStripper.writeString()
        */
    @Override
    protected void writeString(String string, List<TextPosition> textPositions) throws IOException {
        try{
            String[] f = file_name.split(".pdf");
            FileWriter file = new FileWriter(f[0]+"_pos.txt", true);

            for (TextPosition text : textPositions) {
                file.write(text.getUnicode() + "|" + Math.round(text.getXDirAdj()) + "," + Math.round(text.getYDirAdj()*2) + "\n");
            }
            file.close();

        } catch (IOException e){
            System.out.println("An error occurred");
            e.printStackTrace();
        }
    }
}