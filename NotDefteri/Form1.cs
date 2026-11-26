using System;
using System.IO;
using System.Windows.Forms;
using System.Drawing;

namespace NotDefteri
{
    public partial class Form1 : Form
    {
        private string dosyaYolu = string.Empty; // sürüklenen dosyanın yolunu tutan değişken
        private TextBox metinAlani;
        private PictureBox kediResmi; // :333
        private PictureBox glitter;
        private PictureBox btnSoru;
        private PictureBox btnYeniDosya;

        public Form1()
        {
            // Form ayarları
            this.Text = "Not Defterim";
            this.Width = 600;
            this.Height = 400;
            this.BackColor = Color.Pink;
            this.KeyPreview = true; // Formun klavye kısayollarını dinlemesini sağlar
            this.KeyDown += Form1_KeyDown; // Ctrl+S için

            // Kedi ayarları
            kediResmi = new PictureBox();
            kediResmi.Location = new Point(10, 3);
            kediResmi.Size = new Size(77, 77);
            kediResmi.SizeMode = PictureBoxSizeMode.Zoom; // Gifin en boy oranını koruyarak kutuya sığdırıyo
            kediResmi.Image = Image.FromFile("kedi.gif");
            this.Controls.Add(kediResmi);

            // Banner
            glitter = new PictureBox();
            glitter.Location = new Point(100, 3);
            glitter.Size = new Size(250, 70);
            glitter.SizeMode = PictureBoxSizeMode.Zoom;
            glitter.Image = Image.FromFile("glitter.gif");
            this.Controls.Add(glitter);

            // Yardım
            btnSoru = new PictureBox();
            btnSoru.Location = new Point(475, 23);
            btnSoru.Size = new Size(35, 35);
            btnSoru.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btnSoru.SizeMode = PictureBoxSizeMode.Zoom;
            btnSoru.Image = Image.FromFile("help.png");
            btnSoru.Cursor = Cursors.Hand;
            btnSoru.Click += BtnSoru_Click;
            this.Controls.Add(btnSoru);

            // Yeni Dosya Butonu
            btnYeniDosya = new PictureBox();
            btnYeniDosya.Location = new Point(520, 20);
            btnYeniDosya.Size = new Size(40, 40);
            btnYeniDosya.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btnYeniDosya.SizeMode = PictureBoxSizeMode.Zoom;
            btnYeniDosya.Image = Image.FromFile("yeni.png");
            btnYeniDosya.Cursor = Cursors.Hand;
            btnYeniDosya.Click += BtnYeniDosya_Click;
            this.Controls.Add(btnYeniDosya);

            // TextBox ayarları
            metinAlani = new TextBox();
            metinAlani.Multiline = true;
            metinAlani.Location = new Point(10, 80); // Tepede kedi için boşluk bırakacak şekilde
            metinAlani.Size = new Size(560, 270); // Kenarlardan hafif boşluklu boyutta
            metinAlani.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right; // Pencere büyütülünce onun da büyümesi için
            metinAlani.BackColor = Color.LavenderBlush;
            metinAlani.ScrollBars = ScrollBars.Both; // Kaydırma çubukları
            metinAlani.AllowDrop = true; // Sürükle-bırak özelliğini aç

            // Sürükle-Bırak Olayları
            metinAlani.DragEnter += MetinAlani_DragEnter;
            metinAlani.DragDrop += MetinAlani_DragDrop;

            // TextBox'ı forma ekleme
            this.Controls.Add(metinAlani);
        }

        private void BtnSoru_Click(object sender, EventArgs e)
        {
            string mesaj = "Cutecore Notepad Version 1.0 \n\n" +
                           "♥ Düzenlemek istediğin belgeleri bu pencereye sürükleyerek açabilirsin!! :33\n\n" +
                           "♥ Yeni boş bir sayfa açmak için sağ üstteki butonu kullanman yeterli.\n\n" +
                           "♥ Ctrl+S ile yaptığın değişiklikleri kaydedebilirsin.\n\n" +
                           "Diğer projelerimi görmek ve bana destek olmak için: https://github.com/nursezakarakaya";

            MessageBox.Show(mesaj, "Nasıl Kullanılır?", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void BtnYeniDosya_Click(object sender, EventArgs e)
        {
            DialogResult cevap = MessageBox.Show("Mevcut dosyayı kapatıp boş bir sayfa açmak istediğine emin misin?\n\n(Kaydedilmemiş değişiklikler varsa kaybolur!)", "Yeni Dosya", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (cevap == DialogResult.Yes)
            {
                metinAlani.Clear();
                dosyaYolu = string.Empty;
                this.Text = "Not Defterim";
            }
        }

        // Sürüklenen öğenin bir dosya olup olmadığını kontrol eder
        private void MetinAlani_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
                e.Effect = DragDropEffects.Copy; // İmleci kopyalama ikonuna çevir
            else
                e.Effect = DragDropEffects.None;
        }

        // Dosya pencereye bırakıldığında çalışır
        private void MetinAlani_DragDrop(object sender, DragEventArgs e)
        {
            // Sürüklenen dosyaların yollarını dizi olarak al (sadece ilkini alıcaz)
            string[] dosyalar = (string[])e.Data.GetData(DataFormats.FileDrop);

            if (dosyalar != null && dosyalar.Length > 0)
            {
                dosyaYolu = dosyalar[0]; // Dosya yolunu hafızaya al

                // Dosya içeriğini oku ve TextBox'a yazdır
                metinAlani.Text = File.ReadAllText(dosyaYolu);

                // Form başlığına dosya adını ekleme
                this.Text = "Not Defterim - " + Path.GetFileName(dosyaYolu);
            }
        }

        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            // Eğer basılan tuş Ctrl ve S ise
            if (e.Control && e.KeyCode == Keys.S)
            {
                // Ekstra: Ctrl+S dıt sesini engellemek istersen buraya "e.SuppressKeyPress = true;" yazabilirsin
                DosyayiKaydet();
            }
        }

        private void DosyayiKaydet()
        {
            // Eğer daha önceden sürüklenmiş veya kaydedilmiş bir dosya varsa, üzerine yaz
            if (!string.IsNullOrEmpty(dosyaYolu))
            {
                File.WriteAllText(dosyaYolu, metinAlani.Text);
                MessageBox.Show("Değişiklikler başarıyla kaydedildi.", "Kaydedildi", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                // Eğer yeni bir dosyaysa, kullanıcıya nereye kaydedeceğini sor
                SaveFileDialog kaydetPenceresi = new SaveFileDialog();
                kaydetPenceresi.Title = "Yeni Dosyayı Kaydet";
                kaydetPenceresi.Filter = "Metin Belgeleri (*.txt)|*.txt|Tüm Dosyalar (*.*)|*.*"; // Sadece txt ve tüm dosyalar seçeneği
                kaydetPenceresi.DefaultExt = "txt"; // Varsayılan uzantı

                // Kullanıcı pencerede "Kaydet" butonuna basarsa
                if (kaydetPenceresi.ShowDialog() == DialogResult.OK)
                {
                    dosyaYolu = kaydetPenceresi.FileName; // Seçilen yeni konumu hafızaya al
                    File.WriteAllText(dosyaYolu, metinAlani.Text); // Dosyayı oluştur ve yazıyı içine kaydet

                    // Form başlığını yeni dosyanın adıyla güncelle
                    this.Text = "Not Defterim - " + Path.GetFileName(dosyaYolu);

                    MessageBox.Show("Yeni dosya başarıyla oluşturuldu ve kaydedildi.", "Kaydedildi", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
        }
    }
}