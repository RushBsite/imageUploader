using MetadataExtractor;
using MySql.Data.MySqlClient;
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
using System.Security.Permissions;
using System.Runtime.InteropServices;


namespace testForm
{
    
    public partial class Form1 : Form
    {

        List<String> imgList;
        private string id = "user";
        private string pwd = "password";
        public string url = "";

        float Latitude = 0;
        float Longitude = 0;

        public Form1()
        {
            InitializeComponent();
            imgList = new List<String>();
            
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            webBrowser1.ObjectForScripting = true;
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

        public void CallForm(object Lat, object Lng)//javascript 에서 호출하는 함수
        {
            float sLat = (float)Lat;//자료형 호환위해 object 형 강제 형변환
            float sLng = (float)Lng;

            Latitude = sLat; Longitude = sLng;

            textBox2.AppendText("Lat = " + Latitude + "\n" + "Lng = " + Longitude + "\n");

        }

        private void ExecJavascript(float sVal1, float sVal2)
        {
            try
            {
                webBrowser1.Document.InvokeScript("CallScript", new object[] { sVal1, sVal2 });
            }
            catch
            {
            }
        }

        private void upBtn_Click(object sender, EventArgs e) // 1. 테이블에서 인덱스 현황 가져와서 다음 인덱스값으로 설정. 
            //2. 중복검사
            //3. 일단 이미지 자체를 blob으로 업로드.
        {
            using (MySqlConnection connection = new MySqlConnection("Server=localhost;Port=3306;Database=rv_image;Uid=root;Pwd=0000"))
            {
                string insertQuery = "INSERT INTO testtable(idx,Lat,Lng) VALUES(3,"+Latitude+","+ Longitude+ ")";
                try//예외 처리
                {
                    connection.Open();
                    MySqlCommand command = new MySqlCommand(insertQuery, connection);

                    // 만약에 내가처리한 Mysql에 정상적으로 들어갔다면 메세지를 보여주라는 뜻이다
                    if (command.ExecuteNonQuery() == 1)
                    {
                        textBox2.AppendText("인서트 성공");
                    }
                    else
                    {
                        textBox2.AppendText("인서트 실패");
                    }

                }
                catch (Exception ex)
                {
                    textBox2.AppendText("실패");
                    textBox2.AppendText(ex.ToString());
                }

            }
        }

        private void adjBtn_Click(object sender, EventArgs e)
        {
            if(url.Equals("")) { url = "http://localhost:5500/adj.html"; }
            webBrowser1.Navigate(url);
            ExecJavascript(Latitude, Longitude); //자바스크립트로 마커좌표 전달
        }
    }
}
