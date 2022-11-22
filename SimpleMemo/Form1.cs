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

namespace SimpleMemo
{
    public partial class Memo : Form
    {
        private string filePath;//ファイルパス
        private bool textChange = false;//trueでテキストボックスが編集中

        [System.Runtime.InteropServices.DllImport("kernel32.dll")]//コンソール追加
        private static extern bool AllocConsole();//コンソール追加
        public Memo()
        {
            InitializeComponent();
            AllocConsole();//コンソール追加
        }
        private void Form1_Load(object sender, EventArgs e)
        {

        }
        private void btnNew_Click(object sender, EventArgs e)
        {
            if (this.textChange)
            {
                //保存の確認と「はい」「いいえ」のダイアログを新規作成と開くと×ボタンでチェックする
                // メッセージボックスを表示する
                // メッセージボックスのパラメーター
                string text = "変更内容を保存しますか？";
                string caption = "メモ帳";
                MessageBoxButtons button = MessageBoxButtons.YesNoCancel;
                MessageBoxIcon icon = MessageBoxIcon.Question;
                MessageBoxDefaultButton defaultButton = MessageBoxDefaultButton.Button2;
                // メッセージボックスを表示する
                DialogResult dialogResult = MessageBox.Show(this, text, caption, button, icon, defaultButton);
                if(dialogResult == DialogResult.Yes)
                {

                    btnSave_Click(sender, e);
                }
                else if(dialogResult == DialogResult.Cancel)
                {
                    return;
                }
                else
                {
                    txtMain.Text = null;
                    this.Text = "簡易メモ帳-(無題)";//Formそのもののプロパティを変更する際はthis指定
                    this.filePath = null;
                    this.textChange = false;
                }//下と全く同じだからどっちかうまく省略したい
            }
            else
            {
                txtMain.Text = null;
                this.Text = "簡易メモ帳-(無題)";//Formそのもののプロパティを変更する際はthis指定
                this.filePath = null;
                this.textChange = false;
            }
        }

        private void btnOpen_Click(object sender, EventArgs e)
        {
            if (this.textChange)
            {
                //保存の確認と「はい」「いいえ」のダイアログを新規作成と開くと×ボタンでチェックする
                // メッセージボックスを表示する
                // メッセージボックスのパラメーター
                string text = "変更内容を保存しますか？";
                string caption = "メモ帳";
                MessageBoxButtons button = MessageBoxButtons.YesNoCancel;
                MessageBoxIcon icon = MessageBoxIcon.Question;
                MessageBoxDefaultButton defaultButton = MessageBoxDefaultButton.Button2;
                // メッセージボックスを表示する
                DialogResult dr = MessageBox.Show(this, text, caption, button, icon, defaultButton);
                if (dr == DialogResult.Yes)
                {
                    btnSave_Click(sender, e);
                }
                else if (dr == DialogResult.Cancel)
                {
                    return;
                }
                else
                {
                    var dialog = new OpenFileDialog();//ダイアログボックスからファイルを選択。選択しているだけで開いてはいない
                    var dialogResult = dialog.ShowDialog();//戻り値は列挙型
                    if (dialogResult == DialogResult.OK)
                    {
                        //ダイアログで選択されているから、usingでファイルが見当たらないエラーは起きない
                        this.filePath = dialog.FileName;
                        using (var sr = new StreamReader(this.filePath))
                        {
                            txtMain.Text = null;
                            while (!sr.EndOfStream)
                            {
                                txtMain.Text += String.Concat(sr.ReadLine(), "\r\n");//Formアプリの改行コードは"\r\n"
                            }
                            //Trimで最前後の空白文字は消してしまう（意図していないがアプリ上に支障はない）
                            txtMain.Text = txtMain.Text.Substring(0, txtMain.TextLength - 1).Trim();//最後の改行を取る
                        }
                        //fileNameを切って、最後の.txtだけをthis.Textに変更する
                        var fn = this.filePath.Split('\\');
                        this.Text = String.Concat("簡易メモ帳-", fn[fn.Length - 1]);
                        this.textChange = false;
                    }
                }//下と全く同じだからどっちかうまく省略したい
            }
            else
            {
                var dialog = new OpenFileDialog();//ダイアログボックスからファイルを選択。選択しているだけで開いてはいない
                var dialogResult = dialog.ShowDialog();//戻り値は列挙型
                if (dialogResult == DialogResult.OK)
                {
                    //ダイアログで選択されているから、usingでファイルが見当たらないエラーは起きない
                    this.filePath = dialog.FileName;
                    using (var sr = new StreamReader(this.filePath))
                    {
                        txtMain.Text = null;
                        while (!sr.EndOfStream)
                        {
                            txtMain.Text += String.Concat(sr.ReadLine(), "\r\n");//Formアプリの改行コードは"\r\n"
                        }
                        //Trimで最前後の空白文字は消してしまう（意図していないがアプリ上に支障はない）
                        txtMain.Text = txtMain.Text.Substring(0, txtMain.TextLength - 1).Trim();//最後の改行を取る
                    }
                    //fileNameを切って、最後の.txtだけをthis.Textに変更する
                    var fn = this.filePath.Split('\\');
                    this.Text = String.Concat("簡易メモ帳-", fn[fn.Length - 1]);
                    this.textChange = false;
                }
            }
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            if (this.filePath==null)//新規作成時(filePathがnullの場合)
            {
                btnSaveAs_Click(sender, e);
            }
            else//上書き保存（filePathが入っている場合）
            {
                Console.WriteLine("========================");
                Console.WriteLine(txtMain.Text);
                Console.WriteLine("========================");
                using(var sw=new StreamWriter(filePath,false,Encoding.UTF8))
                {
                    sw.WriteLine(txtMain.Text);//Textは改行込みの文字列
                    this.textChange = false;
                    this.Text=this.Text.Substring(0,Text.Length - 5);//（編集中）を切り取る
                }
            }
        }

        private void btnSaveAs_Click(object sender, EventArgs e)
        {
            //※ダイアログとファイル名（ファイルパス）を取得するだけでファイル自体が作成（上書き）されない

            //SaveFileDialogクラスのインスタンスを作成
            SaveFileDialog sfd = new SaveFileDialog();

            //はじめのファイル名を指定する
            //はじめに「ファイル名」で表示される文字列を指定する
            sfd.FileName = "新しいファイル.txt";
            //はじめに表示されるフォルダを指定する。@"C:\"だとC:直下を参照する
            sfd.InitialDirectory = @"";
            //[ファイルの種類]に表示される選択肢を指定する
            //指定しない（空の文字列）の時は、現在のディレクトリが表示される
            sfd.Filter = "すべてのファイル(*.*)|*.*";
            //[ファイルの種類]ではじめに選択されるものを指定する
            //2番目の「すべてのファイル」が選択されているようにする
            sfd.FilterIndex = 0;
            //タイトルを設定する
            sfd.Title = "保存先のファイルを選択してください";
            //ダイアログボックスを閉じる前に現在のディレクトリを復元するようにする
            sfd.RestoreDirectory = true;
            //既に存在するファイル名を指定したとき警告する
            //デフォルトでTrueなので指定する必要はない
            sfd.OverwritePrompt = true;
            //存在しないパスが指定されたとき警告を表示する
            //デフォルトでTrueなので指定する必要はない
            sfd.CheckPathExists = true;

            //ダイアログを表示する
            if (sfd.ShowDialog() == DialogResult.OK)
            {
                this.filePath = sfd.FileName;
                using (var sw = new StreamWriter(this.filePath, false, Encoding.UTF8))
                {
                    sw.WriteLine(txtMain.Text);//Textは改行込みの文字列
                    this.textChange = false;
                    this.Text = this.Text.Substring(0,Text.Length - 5);//（編集中）を切り取る
                }
                //fileNameを切って、最後の.txtだけをthis.Textに変更する
                var fn = filePath.Split('\\');
                this.Text = String.Concat("簡易メモ帳-", fn[fn.Length - 1]);
            }
        }

        private void txtMain_TextChanged(object sender, EventArgs e)
        {
            if (!this.textChange)
            {
                this.Text = String.Concat(this.Text, "（編集中）");
                this.textChange = true;
            }
        }
    }
}
