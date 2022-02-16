using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Media;
using System.Windows.Forms;
using System.IO;
namespace final_fix
{
    public partial class Form1 : Form
    {
        string firstClicked = null;    //宣告第一次點擊的變數，null表示為空
        string secondClicked = null;    //宣告第二次點擊的變數，null表示為空
        int allcardwhitetimer; //宣告15秒後玩家沒配對成功就全部蓋牌
        int patch = 0;    //宣告使用者卡片配對次數，會記在記錄檔
        int win = 0;     //宣告使用者卡片配對成功次數
        int winforpath = 0; // 卡片遊戲配對成功次數;
        int gametimer;      //宣告遊戲時間
        int choicegametime = 0;  //宣告玩家選擇時間，會記在記錄檔
        int usershowcardtime = 3;//使用者設定開牌秒數
        int choiceshowcardtime = 3;//使用者選擇暫開卡片秒數時間，會記在記錄檔
        int mshowpicture = 0; //宣告顯示挑性歡呼照片秒數
        int again = 0;//宣告時間到重完次數(再戰300秒)，會記在記錄檔
        int issave; //檔案使否儲存，用於判斷記錄檔使否有存，不要讓使用者將相同資料重複儲存
        List<string> clicked = new List<string>();//將已按過之卡片放陣列，讓使用者不要案已翻過的卡片
        string[] mathimg = { "win1.png", "win2.png", "win3.png" }; //配對成功顯示成功照片
        string[] dmathimg = { "lose1.png", "lose3.gif", "lose2.JPG", "lose4.gif", "lose5.gif" };  //配對失敗顯示失敗照片'
        string[] gameoption = { "4*4(簡單)", "6*6(困難)" }; //遊戲項目陣列，會加入comboBox2
        System.Media.SoundPlayer winsound = new SoundPlayer("click.wav"); //點擊卡片音效
        System.Media.SoundPlayer dmatchsound = new SoundPlayer("error.wav"); //配對失敗音效
        System.Media.SoundPlayer matchsound = new SoundPlayer("correct.wav"); //配對成功音效     
        System.Media.SoundPlayer allcardwhitesound = new SoundPlayer("allcardwhite.wav"); //全部蓋牌音效     

        public Form1()
        {
            InitializeComponent();
        }
        private void Form1_Load(object sender, EventArgs e)
        {
            winsound.Load(); //載入配對成功音效
            dmatchsound.Load();//載入配對失敗音效
            matchsound.Load();//載入點擊卡片音效
            allcardwhitesound.Load();//載入全部蓋牌音效 
            this.axWindowsMediaPlayer1.URL = "game_music.wav"; //載入遊戲音效     
            this.axWindowsMediaPlayer1.Ctlcontrols.stop(); //程式一執行先不讓音樂播放
            comboBox2.Items.AddRange(gameoption); //將遊戲選項加入comboBox2
            comboBox2.SelectedIndex = 0;  //comboBox2預設為0
            comboBox2.DropDownStyle = ComboBoxStyle.DropDownList; //comboBox2文字只能唯獨
        }
        private void button8_Click(object sender, EventArgs e) //城市一開啟畫面
        {
            button8.Visible = false;
            pictureBox54.Visible = false;
        }

        private void comboBox2_SelectedIndexChanged(object sender, EventArgs e) //選擇4*4與 6*6易難度，並將所有卡片至背面
        {
            if (comboBox2.SelectedIndex == 0)  //4*4
            {
                label64.Text = "0 / 0";
                tabControl1.SelectedTab = tabPage1;
                foreach (Control ctrl in tableLayoutPanel1.Controls)
                {
                    if (ctrl is PictureBox)
                    {
                        ctrl.BackgroundImage = Image.FromFile("back.png");
                    }
                }
            }
            if (comboBox2.SelectedIndex == 1) //6*6
            {
                label64.Text = "0 / 0";
                tabControl1.SelectedTab = tabPage2;
                foreach (Control ctrl in tableLayoutPanel2.Controls)
                {
                    if (ctrl is PictureBox)
                    {
                        ctrl.BackgroundImage = Image.FromFile("back.png");
                    }
                }
            }
        }
        private void checkBox1_CheckedChanged(object sender, EventArgs e) //遊戲音樂開關
        {
            if (checkBox1.Checked == true)
            {
                this.axWindowsMediaPlayer1.Ctlcontrols.play();
            }
            else
            {
                this.axWindowsMediaPlayer1.Ctlcontrols.stop();
            }
        }
        private void trackBar1_Scroll(object sender, EventArgs e) //音樂大小設定
        {
            this.axWindowsMediaPlayer1.settings.volume = (trackBar1.Value * 10);
        }

        private void button7_Click(object sender, EventArgs e) //暫開卡片時間按鈕
        {
            if (gametime.Enabled == true | showcard.Enabled == true) //遊戲進行中，按鈕無作用
            {
                return;
            }
            if (textBox2.Text == "") //表示使用者沒輸入時間
            {
                MessageBox.Show("請輸入時間", "提示訊息", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            choiceshowcardtime = int.Parse(textBox2.Text);
            label68.Text = choiceshowcardtime + "秒";
        }

        private void button2_Click(object sender, EventArgs e) //100秒遊戲時間按鈕
        {
            if (gametime.Enabled | showcard.Enabled == true)  //當遊戲進行中，按鈕無作用
            {
                return;
            }
            patch = 0;   //配對次數清除
            label20.Text = "0"; //將配對次數==0顯示在label20 上
            choicegametime = 100; //讓遊戲時間變100秒
            label18.Text = choicegametime.ToString(); //讓遊戲時間100秒顯示在label18 上
            label2.Text = choicegametime.ToString() + "秒";

        }
        private void button4_Click(object sender, EventArgs e) //200秒遊戲時間按鈕
        {
            if (gametime.Enabled | showcard.Enabled == true) //當遊戲進行中，按鈕無作用
            {
                return;
            }
            patch = 0; //配對次數清除
            label20.Text = "0"; //將配對次數0顯示在label20 上
            choicegametime = 300; //讓遊戲時間變300秒
            label18.Text = choicegametime.ToString(); //讓遊戲時間100秒顯示在label18 上
            label2.Text = choicegametime.ToString() + "秒";

        }
        private void button3_Click(object sender, EventArgs e) //本按鈕可讓使用者自訂時間
        {
            if (gametime.Enabled | showcard.Enabled == true) //當遊戲進行中，按鈕無作用
            {
                return;
            }
            if (textBox1.Text == "")  //如果textbox3為空，表示使用者沒輸入時間，系統會提示提示使用者輸入時間
            {
                MessageBox.Show("請輸入時間", "提示視窗", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            if (int.Parse(textBox1.Text) == 0)  //如果使用主輸入0秒或0秒以下的時間，要求使用者重新輸入時間
            {
                MessageBox.Show("請輸入大於0秒的時間", "提示視窗", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            patch = 0; //配對次數清除
            label20.Text = "0"; //將配對次數==0顯示在label20 上
            choicegametime = int.Parse(textBox1.Text);  //取得使用者自訂秒數的值textBox1
            label18.Text = choicegametime.ToString();//讓使用者自訂遊戲顯示在label18 上
            label2.Text = choicegametime.ToString() + "秒";

        }

        private void textBox1_KeyDown_1(object sender, KeyEventArgs e)  //當使用者對textbox1 按Enter執行button3的程式
        {
            if (e.KeyCode == Keys.Enter)
            {
                button3_Click(this, null);
            }
        }
        private void textBox2_KeyDown_1(object sender, KeyEventArgs e) ////當使用者對textbox2 按Enter執行button3的程式
        {
            if (e.KeyCode == Keys.Enter)
            {
                button7_Click(this, null);
            }
        }
        private void textBox1_KeyPress(object sender, KeyPressEventArgs e)  //限制使用者在每個textbox,只能輸入數字
        {
            if (((int)e.KeyChar < 48 | (int)e.KeyChar > 57) & (int)e.KeyChar != 8)
            {
                e.Handled = true;
            }
        }

        private void button1_Click(object sender, EventArgs e) //開始遊戲按鈕按下之行為
        {
            if (gametime.Enabled | showcard.Enabled == true) //在遊戲中，按下開始按鈕無作用
            {
                return;
            }
            if (choicegametime == 0) //當玩家沒有選擇時間，choicegametime變數為0，必須要讓使用者選擇時間
            {
                MessageBox.Show("請選擇遊戲時間", "提示視窗", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            if (comboBox2.SelectedIndex == 0) 
            {
                winforpath = 8;
                label64.Text = "0/8";
            }
            if (comboBox2.SelectedIndex == 1)
            {
                winforpath = 18;
                label64.Text = "0/18";

            }
            clicked.Clear(); //將clicked內容清除
            issave = 0;  //將檔案儲存指標變1
            again = 0;   //300秒繼續遊玩次數歸零
            patch = 0;   //按下按鈕讓配對次數(patch)歸零
            win = 0; //按下按鈕讓成功配對次數歸零
            firstClicked = null;    //讓第一次點擊卡片firstClicked清空
            secondClicked = null;   //讓第二次點擊卡片secondClicked清空
            label20.Text = patch.ToString();  //label20 顯示玩家配對次數  
            usershowcardtime = choiceshowcardtime; 
            gametimer = choicegametime;  //讓玩家選擇時間，變成系統倒數時間
            label18.Text = choicegametime.ToString(); //label18 顯示玩家所選擇遊戲時間
            checkBox2.Enabled = false; //checkbox2不能選
            comboBox2.Enabled = false; //遊戲進行時，combox2不能按
            textBox1.Text = ""; //將自訂遊戲秒數textbox清除
            textBox2.Text = ""; //將開始暫開卡片textbox清除
            label71.Visible = true;   //開始按鈕按下，顯示開牌倒數label
            label71.Text = "開牌倒數:" + usershowcardtime.ToString() + "秒"; //顯示開牌倒數
            distributiontocard();   //引用distributiontocard(將圖案分配至每個卡片)函數
            showcard.Start();  //觸發showcard(暫開卡片時間) 
        }
        private void distributiontocard() //將圖案分配至每個卡片
        {
            Random random = new Random();
            List<int> num = new List<int>();

            if (tabControl1.SelectedTab == tabPage1) //4*4
            {
                List<string> icons = new List<string>()
             {
            "1.png", "2.png", "3.png", "4.png", "5.png", "6.png", "7.png", "8.png",
            "1.png", "2.png", "3.png", "4.png", "5.png", "6.png", "7.png", "8.png"
             };
                while (num.Count() < 16)
                {
                    int n = random.Next(0, 16);
                    if (num.Contains(n))
                    {
                        continue;
                    }
                    num.Add(n);
                }
                int cardnum = 0;
                foreach (Control ctrl in tableLayoutPanel1.Controls)
                {
                    if (ctrl is PictureBox)
                    {
                        ctrl.Tag = icons[num[cardnum]];
                        ctrl.BackgroundImage = Image.FromFile("Picture//" + icons[num[cardnum]]);
                    }
                    cardnum = cardnum + 1;
                }
            }
            else //6*6
            {
                List<string> icons = new List<string>()
                 {
            "1.png", "2.png", "3.png", "4.png", "5.png", "6.png", "7.png", "8.png","9.png","10.png","11.png","12.png","13.png","14.png","15.png","16.png","17.png","18.png",
            "1.png", "2.png", "3.png", "4.png", "5.png", "6.png", "7.png", "8.png","9.png","10.png","11.png","12.png","13.png","14.png","15.png","16.png","17.png","18.png"
                 };
                while (num.Count() < 36)
                {
                    int n = random.Next(0, 36);
                    if (num.Contains(n))
                    {
                        continue;
                    }
                    num.Add(n);
                }
                int cardnum = 0;
                foreach (Control ctrl in tableLayoutPanel2.Controls)
                {
                    if (ctrl is PictureBox)
                    {
                        ctrl.Tag = icons[num[cardnum]];
                        ctrl.BackgroundImage = Image.FromFile("Picture//" + icons[num[cardnum]]);
                    }
                    cardnum = cardnum + 1;
                }
            }

        }
        private void showcard_Tick(object sender, EventArgs e) //遊戲一開始顯示卡片時間
        {
            if (usershowcardtime > 1)
            {
                usershowcardtime--;
                label71.Text = "開牌倒數:" + usershowcardtime.ToString() + "秒";
            }
            else
            {
                if (tabControl1.SelectedTab == tabPage1)
                {
                    foreach (Control ctrl in tableLayoutPanel1.Controls)
                    {
                        if (ctrl is PictureBox)
                        {
                            ctrl.BackgroundImage = Image.FromFile("back.png");
                        }
                    }
                }
                else
                {
                    foreach (Control ctrl in tableLayoutPanel2.Controls)
                    {
                        if (ctrl is PictureBox)
                        {
                            ctrl.BackgroundImage = Image.FromFile("back.png");
                        }
                    }
                }
                label71.Visible = false;
                showcard.Stop(); 
                gametime.Start();  //蓋回卡片後，開始倒數遊戲時間
            }
        }

        private void gametime_Tick(object sender, EventArgs e) //遊戲倒數時間之處理
        {
            if (gametimer > 0)
            {
                gametimer = gametimer - 1;   //將倒數時間倒數
                label18.Text = gametimer.ToString(); //將倒數時間顯示在畫面上
            }
            else //時間到之處理
            {
                gametime.Stop();   //遊戲時間結束
                allcardwhite.Stop(); //暫停蓋牌計時
                DialogResult result = MessageBox.Show("時間到，是否再戰300秒", "時間已到", MessageBoxButtons.YesNo, MessageBoxIcon.Stop);  //對話框詢問使用者是否繼續遊戲
                if (result == DialogResult.Yes) //時間到後，玩家選擇繼續遊戲
                {
                    again++;
                    gametimer = 300;
                    allcardwhite.Start();
                    gametime.Start();
                }
                if (result == DialogResult.No)//玩家在遊戲時間到選擇不要繼續玩，將顯示卡牌     
                {
                    allcardwhite.Stop();
                    label21.Visible = false;
                    comboBox2.Enabled = true;
                    checkBox2.Enabled = true;
                    if (tabControl1.SelectedTab == tabPage1)
                    {
                        foreach (Control ctrl in tableLayoutPanel1.Controls)
                        {
                            if (ctrl is PictureBox)
                            {
                                ctrl.BackgroundImage = Image.FromFile("Picture/" + ctrl.Tag);
                            }
                        }
                    }
                    else
                    {
                        foreach (Control ctrl in tableLayoutPanel2.Controls)
                        {
                            if (ctrl is PictureBox)
                            {
                                ctrl.BackgroundImage = Image.FromFile("Picture/" + ctrl.Tag);
                            }
                        }
                    }

                }
            }
        }
        private void icon_click(object sender, EventArgs e)  //點擊卡片之處理
        {
            Random pict = new Random();  //使用者配對卡片成功或失敗所"隨機"出現的照片
            PictureBox clickedLabel = sender as PictureBox;
            if (clickedLabel != null) //點擊卡片的處理
            {
                for (int i = 0; i < clicked.Count; i++) //將以點擊卡片物件存到clicked變數，如果使用者點擊卡片有在clicked變數裡，表示卡片已點擊過，不能再點
                {
                    if (((PictureBox)sender).Name == clicked[i])
                    {
                        return;
                    }
                }
                if (Nomatch.Enabled == true | showcard.Enabled == true | gametime.Enabled != true) //在系統檢查卡片是否配對成功時，遊戲一開始暫開卡片時，卡片不能點擊
                {
                    return;
                }
                if (firstClicked == null) //第一次點卡片，並將卡片存入firstClicked變數裡，並播放聲音
                {
                    clicked.Add(((PictureBox)sender).Name); //將以點擊過的picturebox名稱加入clicked陣列
                    firstClicked = clickedLabel.Tag.ToString(); //將以點擊卡片之檔名存入firstClicked變數
                    clickedLabel.BackgroundImage = Image.FromFile("Picture//" + firstClicked); //翻牌
                    winsound.Play();
                    return;
                }
                secondClicked = clickedLabel.Tag.ToString();//第二次點卡片，並將卡片存入firstClicked變數裡，並播放聲音
                clicked.Add(((PictureBox)sender).Name); //將以點擊過的picturebox名稱加入clicked陣列
                clickedLabel.BackgroundImage = Image.FromFile("Picture//" + secondClicked); //翻牌
                winsound.Play();
                if (firstClicked != null && secondClicked != null)//紀錄配對次數，並顯示在label20上
                {
                    patch++;
                    label20.Text = patch.ToString();
                }
                if (firstClicked == secondClicked) //兩張卡片配對成功處理
                {
                    allcardwhite.Start(); //觸發15秒未配對成功全部蓋回卡片
                    allcardwhitetimer = 15;
                    win = win + 1;          //win配對成功+1
                    matchsound.Play();      //播放配對成功音效
                    label64.Text = win.ToString() + "/" + winforpath; //顯示已配對次數
                    pictureBox1.Visible = true;    //讓picture可見
                    pictureBox1.Image = Image.FromFile(mathimg[pict.Next(0, mathimg.Length)]);  //隨機選擇成功配對之照片，顯示在picture1上
                    mshow.Interval = 1000;
                    mshow.Start();   //觸發mshow timer函數，為了讓選擇成功配對之照片在一秒中隱藏pictureBox1.Visible = false
                    if (win == winforpath)   //win配對成功變數為8或18，表示全部卡片配對完成，遊戲結束
                    {
                        gametime.Stop();
                        allcardwhite.Stop();
                        comboBox2.Enabled = true;
                        checkBox2.Enabled = true;
                        label21.Visible = false;
                        label68.Text = "3秒";
                        MessageBox.Show("全部配對成功", "恭喜您", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }

                    firstClicked = null;  //firstClicked(第一次點擊)變數清除
                    secondClicked = null;  //secondClicked(第二次點擊)變數清除
                    return;
                }
                //以下程式為配對卡片失敗之處理
                dmatchsound.Play();     //播放配對失敗音效
                pictureBox1.Visible = true;  //讓picture可見
                pictureBox1.Image = Image.FromFile(dmathimg[pict.Next(0, dmathimg.Length)]);  //隨機選擇失敗配對之照片，顯示在picture1上
                mshow.Interval = 500;
                mshow.Start();  //觸發dshow timer函數，為了讓選擇成功失敗之照片在一秒中隱藏pictureBox1.Visible = false
                Nomatch.Start(); //配對失敗隔幾秒後將排翻回去
            }
        }
        private void mshow_Tick(object sender, EventArgs e)  //讓配對後挑釁歡呼照片顯示
        {
            mshowpicture++;
            if (mshowpicture == 1)
            {
                pictureBox1.Visible = false;
                mshowpicture = 0;
                mshow.Stop();
            }
        }
        private void Nopatch_Tick(object sender, EventArgs e) //配對失敗處理，將卡片蓋回
        {
            if (tabControl1.SelectedTab == tabPage1)
            {
                foreach (Control ctrl in tableLayoutPanel1.Controls)
                {
                    if (ctrl is PictureBox)
                    {
                        if (ctrl.Tag.ToString() == firstClicked | ctrl.Tag.ToString() == secondClicked)
                        {
                            ctrl.BackgroundImage = Image.FromFile("back.png");
                        }

                    }
                }
            }
            else
            {
                foreach (Control ctrl in tableLayoutPanel2.Controls)
                {
                    if (ctrl is PictureBox)
                    {
                        if (ctrl.Tag.ToString() == firstClicked | ctrl.Tag.ToString() == secondClicked)
                        {
                            ctrl.BackgroundImage = Image.FromFile("back.png");
                        }
                    }
                }
            }

            firstClicked = null;   //firstClicked(第一次點擊)變數清除
            secondClicked = null;    //secondClicked(第二次點擊)變數清除
            Nomatch.Stop();

            if (clicked.Count > 1)
            {
                clicked.RemoveAt(clicked.Count - 1); //將已點擊變數移除配對對成功之卡片
                clicked.RemoveAt(clicked.Count - 1); //將已點擊變數移除配對對成功之卡片
            }
            else
            {
                return;
            }

        }
        private void button6_Click(object sender, EventArgs e)   //遊戲進行中提供中斷
        {
            if (gametime.Enabled == false)
            {
                MessageBox.Show("只有在遊戲進行時，才能使用", "提示訊息", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            if (showcard.Enabled == true)
            {
                MessageBox.Show("只有在遊戲進行時，才能使用", "提示訊息", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            gametime.Stop();
            comboBox2.Enabled = true;
            allcardwhitetimer = 15;
            allcardwhite.Stop();
            label21.Visible = false;
            checkBox2.Enabled = true;
            if (tabControl1.SelectedTab == tabPage1)
            {
                foreach (Control ctrl in tableLayoutPanel1.Controls)
                {
                    if (ctrl is PictureBox)
                    {
                        ctrl.BackgroundImage = Image.FromFile("Picture/" + ctrl.Tag);
                    }
                }
            }
            else
            {
                foreach (Control ctrl in tableLayoutPanel2.Controls)
                {
                    if (ctrl is PictureBox)
                    {
                        ctrl.BackgroundImage = Image.FromFile("Picture/" + ctrl.Tag);
                    }
                }
            }
            MessageBox.Show("遊戲已停止", "提示訊息", MessageBoxButtons.OK, MessageBoxIcon.Warning);

        }
        private void button5_Click(object sender, EventArgs e) //提供使用者關閉視窗
        {
            Close();
        }

        private void allcardwhite_Tick(object sender, EventArgs e)
        {
            if (checkBox2.Checked)
            {
                return;
            }
            if (allcardwhitetimer > 1)
            {
                label21.Visible = true;
                allcardwhitetimer--;
                label21.Text = "全部蓋牌時間:" + allcardwhitetimer + "秒";
            }
            else
            {
                allcardwhitesound.Play();
                clicked.Clear();
                firstClicked = null;
                secondClicked = null;
                allcardwhite.Stop();
                win = 0;
                label64.Text = win + "/" + winforpath;
                label21.Visible = false;
                if (tabControl1.SelectedTab == tabPage1)
                {
                    foreach (Control ctrl in tableLayoutPanel1.Controls)
                    {
                        if (ctrl is PictureBox)
                        {
                            ctrl.BackgroundImage = Image.FromFile("back.png");
                        }
                    }
                }
                else
                {
                    foreach (Control ctrl in tableLayoutPanel2.Controls)
                    {
                        if (ctrl is PictureBox)
                        {
                            ctrl.BackgroundImage = Image.FromFile("back.png");
                        }
                    }
                }
            }

        }
        private void button9_Click(object sender, EventArgs e)
        {
            if (gametime.Enabled == true | showcard.Enabled == true)
            {
                return;
            }
            if (patch == 0)
            {
                MessageBox.Show("您尚未配對過，暫時無法儲存", "提示訊息", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (issave == 1)
            {
                MessageBox.Show("您已經儲存過紀錄喔!", "提示訊息", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            if (saveFileDialog1.FileName == "")
            {
                saveFileDialog1.Filter = "txt files (*.txt)|*.txt";
                if ((saveFileDialog1.ShowDialog() == DialogResult.OK))
                {

                }

            }
            if (saveFileDialog1.FileName == "")
            {
                return;
            }
            StreamWriter r = new StreamWriter(saveFileDialog1.FileName, true);
            issave = 1;
            if (tabControl1.SelectedTab == tabPage1) //4*4
            {
                if (win == 8)
                {
                    r.WriteLine("儲存時間:" + DateTime.Now + "   遊戲易難度:4*4" + "   是否在15秒全部蓋牌模式遊玩:" + !(checkBox2.Checked) +"   玩家選擇遊戲時間:" + choicegametime + "秒    暫開卡片秒數時間:" + choiceshowcardtime + "秒   剩餘時間:" + gametimer + "秒    玩家配對次數:" + patch + "次   再戰300秒次數:" + again + "次    玩家配對成功");
                }
                else
                {
                    r.WriteLine("儲存時間:" + DateTime.Now + "   遊戲易難度:4*4" + "   是否在15秒全部蓋牌模式遊玩:" + !(checkBox2.Checked) +"   玩家選擇遊戲時間:" + choicegametime + "秒    暫開卡片秒數時間:" + choiceshowcardtime + "秒   剩餘時間:" + gametimer + "秒    玩家配對次數:" + patch + "次    再戰300秒次數:" + again + "次   玩家配對失敗");

                }
            }
            if (tabControl1.SelectedTab == tabPage2) //6*6
            {
                if (win == 18)
                {
                    r.WriteLine("儲存時間:" + DateTime.Now + "   遊戲易難度:6*6" + "   是否在15秒全部蓋牌模式遊玩:" + !(checkBox2.Checked) + "   玩家選擇遊戲時間:" + choicegametime + "秒    暫開卡片秒數時間:" + choiceshowcardtime + "秒   剩餘時間:" + gametimer + "秒    玩家配對次數:" + patch + "次   再戰300秒次數:" + again + "次    玩家配對成功");
                }
                else
                {
                    r.WriteLine("儲存時間:" + DateTime.Now + "   遊戲易難度:6*6" +"   是否在15秒全部蓋牌模式遊玩:"+ !(checkBox2.Checked)+ "   玩家選擇遊戲時間:" + choicegametime + "秒    暫開卡片秒數時間:" + choiceshowcardtime + "秒   剩餘時間:" + gametimer + "秒    玩家配對次數:" + patch + "次    再戰300秒次數:" + again + "次   玩家配對失敗");

                }
            }
            r.Close();
            MessageBox.Show("記錄檔已儲存，檔案位置在" + saveFileDialog1.FileName, "提示訊息", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }

        private void checkBox2_CheckedChanged(object sender, EventArgs e)
        {
            Console.WriteLine(checkBox2.Checked);
        }
    }
}


