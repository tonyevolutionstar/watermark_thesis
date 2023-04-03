using IronBarCode;
using iTextSharp.text.pdf;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;

namespace WatermarkApp
{
    /// <summary>
    /// Funções comuns entre classes
    /// </summary>
    /// 
    public class Commom
    {
        public int width; //comprimento do ficheiro
        public int height; //altura do ficheiro
        public string files;
        public string files_dir = @"Ficheiros\";
        public string extension_watermark = "watermark";
        public string extension_integrity = "integrity";
        public string extension_barcode = "_barcode.png";
        public int number_points = 9;
        public int height_barcode = 15; // o 0 começa em baixo
        public string errorReadBarcode = "insucesso";
        private string jar_file = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location) + @"\Ficheiros\thesis_watermark.jar";

        private TrackerServices trackerServices = new TrackerServices();

        public Commom()
        {
            string partialPath = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
            files = partialPath + files_dir;
        }

        /// <summary>
        /// Executa o ficheiro jar que vai gerar um ficheiro txt com as posições dos 
        /// caracteres que estão no pdf
        /// </summary>
        public void Get_PositionChar(string file_name)
        {
            System.Diagnostics.Process process_file = new System.Diagnostics.Process();
            process_file.StartInfo.UseShellExecute = false;
            process_file.StartInfo.RedirectStandardOutput = true;
            process_file.StartInfo.FileName = "java";
            process_file.StartInfo.Arguments = "-jar " + '"' + jar_file + '"' + " " + '"' + file_name + '"';
            process_file.Start();
            process_file.WaitForExit();
        }


        /// <summary>
        /// Lê o ficheiro gerado das posições dos caracteres no documento e colocar os valores numa variavel 
        /// </summary>
        /// <returns>Linhas do documento</returns>
        public string[] Read_positionChar_file(string file_name)
        {
            string[] s_doc = file_name.Split(new[] { ".pdf" }, StringSplitOptions.None);
            string posFile = s_doc[0] + "_pos.txt";
            string[] lines = System.IO.File.ReadAllLines(posFile, System.Text.Encoding.UTF7);
            return lines;
        }


        public string Get_file_name_using_split(string file_name)
        {
            string[] s_doc = file_name.Split(new[] { ".pdf" }, StringSplitOptions.None);
            return s_doc[0];
        }


        public string Get_file_name_without_directory(string file_name)
        {
            string[] show_doc = file_name.Split(new[] {files_dir }, StringSplitOptions.None);
            string[] file = show_doc[1].Split(new[] { ".pdf" }, StringSplitOptions.None);
            return file[0];
        }

        public string Convert_pdf_png(string file_name_png)
        {
            trackerServices.WriteFile($"converção do ficheiro {file_name_png} concluída");
            var dd = System.IO.File.ReadAllBytes(file_name_png);
            byte[] pngByte = Freeware.Pdf2Png.Convert(dd, 1);
            string[] filename = file_name_png.Split(new[] { ".pdf" }, StringSplitOptions.None);
            if(!System.IO.File.Exists(filename[0] + ".png"))
                System.IO.File.WriteAllBytes(filename[0]+ ".png", pngByte);
            return filename[0] + ".png";
        }

        public string Read_barcode(string file_name)
        {
            trackerServices.WriteFile($"lendo o código de barras do ficheiro {file_name}");
            string img_file = Convert_pdf_png(file_name);
            BarcodeReaderOptions readerOptions = new BarcodeReaderOptions()
            {
                ExpectMultipleBarcodes = true,
                ExpectBarcodeTypes = BarcodeEncoding.Code128
            };
            List<string> res = new List<string>();

            var result = BarcodeReader.Read(img_file, readerOptions);
            if(result != null)
            {
                foreach(var barcode in result)
                {
                    res.Add(barcode.Value);
                }
                // os valores que estão dentro do código são iguais, exceto as posições, por isso para obter o texto do código de barras tanto faz ser 0 como 1  
                return res[0];
            }
            else
                trackerServices.WriteFile("erro na leitura do código de barras");

            return errorReadBarcode;
        }


        public List<string> Return_PositionBarcode(string file_name)
        {
            GetDimensionsDocument(file_name);
            trackerServices.WriteFile("lendo as posições do código de barras");
            string img_file = Convert_pdf_png(file_name);
            List<string> res = new List<string>();

            BarcodeReaderOptions readerOptions = new BarcodeReaderOptions()
            {
                Speed = ReadingSpeed.ExtremeDetail,
                ExpectMultipleBarcodes= true,
                ExpectBarcodeTypes = BarcodeEncoding.Code128
            };

            var result = BarcodeReader.Read(img_file, readerOptions);

            var result2 = BarcodeReader.ReadASingleBarcode(img_file, BarcodeEncoding.Code39, BarcodeReader.BarcodeRotationCorrection.Medium,
            BarcodeReader.BarcodeImageCorrection.DeepCleanPixels);

            if (result != null || result2 != null)
            {
                using (Bitmap bmp = new Bitmap(img_file)) // garantir o fechar do bitmap - fecha todos os recursos usados
                {
                    //mudar aqui adicionar lista do result2, compor posições, eliminar code39
                    int x_39 = (int)result2.X1 * width / bmp.Width + 103;
                    int y_39 = (int)result2.Y1 * height / bmp.Height + 27;
                    int x2_39 = (int)result2.X2 * width / bmp.Width - 70;
                    int y2_39 = (int)result2.Y2 * height / bmp.Height - 34;
                    res.Add($"{x_39}:{y_39}:{x2_39}:{y2_39}");

                    foreach(var barcode in result)
                    {
                        // é necessário compor as posições do código barras pois ele tem espaço branco na imagem, os valores foram ajustados manualmente
                        // com a ajuda da criação de uma imagem auxiliar que permite a visualização dos pontos
                        int x = (int)barcode.X1 * width / bmp.Width + 97;
                        int y = (int)barcode.Y1 * height / bmp.Height + 25;
                        int x2 = (int)barcode.X2 * width / bmp.Width - 60; // se for maior que -55 vai para a direita, -70 vai para a esquerda
                        int y2 = (int)barcode.Y2 * height / bmp.Height;

                        if (file_name.Contains("scan"))
                        {
                            x = (int)barcode.X1 * width / bmp.Width + 97;
                            y = (int)barcode.Y1 * height / bmp.Height + 25;
                            x2 = (int)barcode.X2 * width / bmp.Width - 60;
                            y2 = (int)barcode.Y2 * height / bmp.Height;
                            //change
                            //fixing the values of scan image 
                            if (y >= 1000)
                                y = Math.Abs(height - y + 15);
                            if (x >= 220)
                                x = Math.Abs(width - x - 63);
                            if (x2 >= 500)
                                x2 = Math.Abs(width - x2);
                            if (y2 >= 1000)
                                y2 = Math.Abs(height - y2 + 90);
                            if (x2 <= 100)
                                x2 += width / 2 + 41;
                        }
                        res.Add($"{x}:{y}:{x2}:{y2}");
                    }
                   
                    bmp.Dispose();
                    return res;
                }
            }
            else
                trackerServices.WriteFile("erro ao obter as posições do código de barras");
            
            return res;
        }


        public void RetificateAnalise(int id_doc, SQL_connection sql, string file_name, int difference_x, int difference_y)
        {   
            List<string> returnlist = sql.Get_Values_Analise_Forense(id_doc);
            AuxFunc auxFunc = new AuxFunc(id_doc, sql, file_name);

            string img = auxFunc.DrawImage(returnlist, file_name, difference_x, difference_y);
            string[] filename = img.Split(new[] { ".png" }, StringSplitOptions.None);

            string output = filename[0] + ".pdf";
            string[] s_doc = file_name.Split(new[] { "_integrity" }, StringSplitOptions.None);

            using (FileStream sourceStream = new FileStream(s_doc[0], FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                using (FileStream outputStream = new FileStream(output, FileMode.Create, FileAccess.Write, FileShare.None))
                {
                    iTextSharp.text.pdf.PdfReader reader = new iTextSharp.text.pdf.PdfReader(sourceStream);
                    PdfStamper stamper = new PdfStamper(reader, outputStream);
                    iTextSharp.text.Image image = iTextSharp.text.Image.GetInstance(img);
                    image.SetAbsolutePosition(0, 0);
                    iTextSharp.text.Rectangle pageSize = reader.GetPageSizeWithRotation(1);
                    image.ScaleToFit(pageSize.Width, pageSize.Height);
                    PdfContentByte content = stamper.GetOverContent(1);
                    content.AddImage(image);
                    stamper.Close();
                    reader.Close();
                }
            }

            AnaliseForenseForm form = new AnaliseForenseForm(filename[0] + ".pdf");
            form.Show();
        }

        public void GetDimensionsDocument(string file_name)
        {
            if (!file_name.Contains(".pdf"))
                file_name += ".pdf";
            using (Stream inputPdfStream = new FileStream(file_name, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                var reader = new iTextSharp.text.pdf.PdfReader(inputPdfStream);
                iTextSharp.text.pdf.PdfReader.unethicalreading = true;
                width = (int)reader.GetPageSize(1).Width;
                height = (int)reader.GetPageSize(1).Height;
                reader.Close();
                reader.Dispose();
            }
        }
    }
}
