using MetadataExtractor;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace testForm
{
    
    public partial class Form1 : Form
    {

        List<String> imgList;
        public Form1()
        {
            InitializeComponent();
            imgList = new List<String>();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            var fileContent = string.Empty;
            var filePath = string.Empty;
            float Latitude = 0;
            float Longitude = 0;



            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.InitialDirectory = "c:\\";
            openFileDialog.Filter = "jpeg files (*.jpg)|*.jpg|All files (*.*)|*.*";
            openFileDialog.FilterIndex = 2;
            openFileDialog.RestoreDirectory = true;
            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                
                filePath = openFileDialog.FileName;
                textBox1.Text = filePath;

                var fileStream = openFileDialog.OpenFile();

                using (StreamReader reader = new StreamReader(fileStream))
                {
                    fileContent = reader.ReadToEnd();
                }
            }
            var directories = ImageMetadataReader.ReadMetadata(filePath);

            foreach (var directory in directories)
            {
                foreach (var tag in directory.Tags)
                    if(tag.Name == "GPS Latitude" || tag.Name == "GPS Longitude")
                    {
                        if (tag.Name == "GPS Latitude")
                        {
                            Latitude = StoF(tag.Description);
                            textBox2.AppendText($"[{directory.Name}] {tag.Name} = " + Latitude + "\n");

                        }
                        else
                        {
                            Longitude = StoF(tag.Description);
                            textBox2.AppendText($"[{directory.Name}] {tag.Name} = " + Longitude + "\n");

                        }


                    }

                if (directory.HasError)
                {
                    foreach (var error in directory.Errors)
                        Console.WriteLine($"ERROR: {error}");
                }
            }




        }

        private float StoF(string L) // 도분초 -> float 으로 변환
        {
            char d = '°';
            char b = '\'';
            char c = '\"';

            float r = 0;

            string[] spstring = L.Split(d, b, c);
           
            for(int i =0; i<3; i++)
            {
                if (i == 0)
                    r += float.Parse(spstring[i]);
                else if (i == 1)
                    r += float.Parse(spstring[i]) / 60.0f;
                else
                    r += float.Parse(spstring[i]) / 3600.0f;
            }


            return r;
        }
    }
}
