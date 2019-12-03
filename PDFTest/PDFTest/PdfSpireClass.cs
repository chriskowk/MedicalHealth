using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using Spire.Pdf;
using Spire.Pdf.Annotations;
using Spire.Pdf.Graphics;
using Spire.Pdf.Annotations.Appearance;


namespace PdfTestApp
{
    public static class PdfHelper
    {
        public static bool PrintPdfFile()
        {
            PdfDocument doc = new PdfDocument();

            //for (int i = 0; i < 100; i++)
            //{
            //    //get the page
            //    PdfPageBase page = doc.Pages.Add();
            //    //get the image
            //    PdfImage image = PdfImage.FromFile(@"D:\W\1.png");//---需要添加的图片
            //    float width = image.Width * 0.70f;
            //    float height = image.Height * 0.70f;
            //    //insert image
            //    page.Canvas.DrawImage(image, 0, 0, width, height); //---图片需要添加的位置
            //}

            //string output = @"D:\W\test.pdf";//添加图片后的新pdf文件的路径
            ////save pdf file
            //doc.SaveToFile(output);
            //doc.Close();

            doc.LoadFromFile(@"E:\MedicalHealthSY\bin\Temp\output20191114181917.pdf");
            doc.Print();

            return true;
        }

    }
}
