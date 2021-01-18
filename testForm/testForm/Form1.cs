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
                     textBox2.AppendText($"[{directory.Name}] {tag.Name} = {tag.Description}\n");

                if (directory.HasError)
                {
                    foreach (var error in directory.Errors)
                        Console.WriteLine($"ERROR: {error}");
                }
            }

        }
    }
}
