using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using AltoHttp;
using System.Threading;
using System.Net;
using System.Diagnostics;

namespace bulmaca1
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        Izgara bulmaca1 = new Izgara(50, 50);
        Engel bir = new Engel("açık yeşil");
        Engel iki = new Engel("yeşil");
        Engel üç = new Engel("koyu yeşil");
        Engel baslangıc = new Engel("mavi");
        Engel bitis = new Engel("kırmızı");
        public static List<Node> Grafik = new List<Node>();
        public static Node opening, goal;
        public static int open;
        public static Stack<Node> stack = new Stack<Node>();
        public static List<Node> CozumYolu = new List<Node>();
        public static List<Node> HedefArayisi = new List<Node>();
        private static readonly Random rndy = new Random();
        Stopwatch time = new Stopwatch();
        WebClient client = new WebClient();

        public static void Alan_ayarlama(Izgara e)
        {
            //hücre sayısına göre büyüklüğü ayarlama
            if (e.gridSize > 10)
            {
                e.cellSize -= e.gridSize;
            }

            // oyun alanını oluşturma
            e.gameGrid = new int[e.gridSize, e.gridSize];
            for (int i = 0; i < e.gridSize; i++)
            {
                for (int j = 0; j < e.gridSize; j++)
                {
                    e.gameGrid[i, j] = 0;
                    Grafik.Add(new Node(i, j, false));
                }
            }


            for (int i = 0; i < Grafik.Count; i++)
            {
                for (int j = 0; j < Grafik.Count; j++)
                {
                    if ((Grafik[i].x == Grafik[j].x && Math.Abs(Grafik[i].y - Grafik[j].y) == 1) || (Grafik[i].y == Grafik[j].y && Math.Abs(Grafik[i].x - Grafik[j].x) == 1))
                    {
                        Grafik[i].neighbors.Add(Grafik[j]);
                    }
                }
            }

        }

        public void Engelleri_Belirleme(Izgara e)
        {
            if (e.gameGridInfo != null)
            {
                string[] hücreler2 = e.gameGridInfo.Split('\n');

                List<int> hücreler3()
                {
                    List<int> deger = new List<int>();
                    for (int i = 0; i < hücreler2.Length; i++)
                    {
                        char[] chars = hücreler2[i].ToCharArray();

                        for (int j = 0; j < chars.Length; j++)
                        {
                            if (j != e.gridSize)
                            {
                                int a;
                                a = Convert.ToInt32(chars[j]);
                                deger.Add(a);
                            }
                        }
                    }
                    return deger;
                }


                int k = 0;
                for (int i = 0; i < e.gridSize; i++)
                {
                    for (int j = 0; j < e.gridSize; j++)
                    {
                        e.gameGrid[i, j] = hücreler3()[k];
                        k++;
                    }
                }

                int cc = hücreler3().Count;
                int c = bulmaca1.gridSize;
                int ccc = Grafik.Count;

                for (int i = 0; i < hücreler3().Count; i++)
                {
                    if (hücreler3()[i] > 48 && i <= Grafik.Count)
                    {
                        Grafik[i].engelAta();
                    }
                }

                //Başlangıç ve bitiş belirleme
                Random rnd = new Random();
                int baslangic_x, baslangic_y;
                int hedef_x, hedef_y;

                while (true)
                {
                    baslangic_x = rnd.Next(e.gridSize);
                    baslangic_y = rnd.Next(e.gridSize);
                    hedef_x = rnd.Next(e.gridSize);
                    hedef_y = rnd.Next(e.gridSize);

                    if (Math.Abs(baslangic_x - hedef_x) < e.gridSize / 2 && Math.Abs(baslangic_y - hedef_y) < e.gridSize / 2)
                    {
                        continue;
                    }

                    if (e.gameGrid[baslangic_x, baslangic_y] == 48 && e.gameGrid[hedef_x, hedef_y] == 48)
                    {
                        e.gameGrid[baslangic_x, baslangic_y] = 5;
                        e.gameGrid[hedef_x, hedef_y] = 6;
                        opening = new Node(baslangic_x, baslangic_y, false);
                        goal = new Node(hedef_x, hedef_y, false);
                        break;
                    }
                }

                foreach (Node a in Grafik)
                {
                    if (a.x == goal.x && a.y == goal.y)
                    {
                        a.goal = true;
                    }
                }
            }

        }

        public void GridCiz(Izgara e, Graphics g)
        {
            Pen p = new Pen(Color.Black, 2);
            for (int i = 0; i <= e.gridSize; i++)
            {
                g.DrawLine(p, e.margin, e.margin + i * e.cellSize, e.margin + e.gridSize * e.cellSize, e.margin + i * e.cellSize);
                g.DrawLine(p, e.margin + i * e.cellSize, e.margin, e.margin + i * e.cellSize, e.margin + e.gridSize * e.cellSize);
            }
        }

        public void HücreleriBoya(Izgara e, Graphics g)
        {
            Brush beyaz = Brushes.White;
            for (int i = 0; i < e.gridSize; i++)
            {
                for (int j = 0; j < e.gridSize; j++)
                {
                    if (e.gameGrid[i, j] == 48)
                    {
                        g.FillRectangle(beyaz, e.margin + j * e.cellSize + 1, e.margin + i * e.cellSize + 1, e.cellSize - 1, e.cellSize - 1);
                    }

                    if (e.gameGrid[i, j] == 49)
                    {
                        g.FillRectangle(bir.kalem, e.margin + j * e.cellSize + 1, e.margin + i * e.cellSize + 1, e.cellSize - 1, e.cellSize - 1);
                    }

                    if (e.gameGrid[i, j] == 50)
                    {
                        g.FillRectangle(iki.kalem, e.margin + j * e.cellSize + 1, e.margin + i * e.cellSize + 1, e.cellSize - 1, e.cellSize - 1);
                    }

                    if (e.gameGrid[i, j] == 51)
                    {
                        g.FillRectangle(üç.kalem, e.margin + j * e.cellSize + 1, e.margin + i * e.cellSize + 1, e.cellSize - 1, e.cellSize - 1);
                    }

                    if (e.gameGrid[i, j] == 5)
                    {
                        g.FillRectangle(baslangıc.kalem, e.margin + j * e.cellSize + 1, e.margin + i * e.cellSize + 1, e.cellSize - 1, e.cellSize - 1);
                    }

                    if (e.gameGrid[i, j] == 6)
                    {
                        g.FillRectangle(bitis.kalem, e.margin + j * e.cellSize + 1, e.margin + i * e.cellSize + 1, e.cellSize - 1, e.cellSize - 1);
                    }
                }
            }
        }

        public Node AlternatifHedefBul(Node Opening, Stack<Node> stack, List<Node> HedefArayisi)
        {
            Brush kalem = Brushes.YellowGreen;
            foreach (Node node in Opening.neighbors)
            {
                if (node.goal)
                {
                    return node;
                }
            }

            int choosen = 0;
            List<int> exp_neigh = new List<int>();
            int dongu_sayisi = 0;
            bool re_value = false;

            while (true)
            {
                if(Opening.neighbors != null)
                {
                    choosen = rndy.Next(Opening.neighbors.Count);
                }

                else
                {
                    break;
                }


                if (exp_neigh.Contains(choosen))
                {
                    if (Opening.neighbors[choosen].obstacle == false)
                    {
                        if (exp_neigh.Count == Opening.neighbors.Count && !stack.Peek().opening)
                        {
                            re_value = true;
                            break;
                        }
                    }

                    else
                    {
                        continue;
                    }
                }

                else
                {
                    exp_neigh.Add(choosen);
                }



                if (Opening.neighbors[choosen].obstacle == false && Opening.neighbors[choosen].exploration == false)
                {
                    if (Opening.neighbors[choosen].opening == false && Opening.neighbors[choosen].exploration == false && Opening.neighbors[choosen].goal == false)
                    {
                        HedefArayisi.Add(Opening.neighbors[choosen]);
                        Opening.neighbors[choosen].exploration = true;
                    }

                    stack.Push(Opening.neighbors[choosen]);
                    break;
                }

                dongu_sayisi++;

                if(dongu_sayisi > 4)
                {
                    re_value = true;
                    break;
                }
            }

            if (re_value)
            {
                return AlternatifHedefBul(stack.Pop(), stack, HedefArayisi);
            }

            else
            {
                return AlternatifHedefBul(Opening.neighbors[choosen], stack, HedefArayisi);
            }

        }

        public Node HedefBul(List<Node> path, Izgara e, Graphics g)
        {
            List<Node> neighbors = new List<Node>();
            Brush kalem = Brushes.YellowGreen;
            foreach (Node node in path)
            {
                foreach (Node neighbor in path)
                {
                    if ((Math.Abs(node.x - neighbor.x) == 1 && node.y == neighbor.y) ||
                        (node.x == neighbor.x && Math.Abs(node.y - neighbor.y) == 1))
                    {
                        if (!neighbor.obstacle)
                        {
                            neighbors.Add(neighbor);
                        }
                    }
                }



                for (int i = 0; i < neighbors.Count; i++)
                {
                    if (!neighbors[i].goal)
                    {
                        if (opening.x != neighbors[i].x && opening.y != neighbors[i].y)
                        {
                            int x = neighbors[i].x;
                            int y = neighbors[i].y;
                            g.FillRectangle(kalem, e.margin + x * e.cellSize + 1, e.margin + y * e.cellSize + 1, e.cellSize - 1, e.cellSize - 1);
                        }
                    }

                    else
                    {
                        return neighbors[i];
                    }
                }

                neighbors.Clear();
            }
            return null;
        }

        public void YolCiz(List<Node> path, Izgara e, Graphics g)
        {
            Brush sarı = Brushes.Yellow;

            for (int i = 1; i < path.Count - 1; i++)
            {
                int x = path[i].x;
                int y = path[i].y;
                g.FillRectangle(sarı, e.margin + y * e.cellSize + 1, e.margin + x * e.cellSize + 1, e.cellSize - 1, e.cellSize - 1);
                Thread.Sleep(30);
            }
        }

        public void HedefArayisiCiz(List<Node> path, Izgara e, Graphics g)
        {
            Brush sarı = Brushes.YellowGreen;

            for (int i = 1; i < path.Count - 1; i++)
            {
                int x = path[i].x;
                int y = path[i].y;
                g.FillRectangle(sarı, e.margin + y * e.cellSize + 1, e.margin + x * e.cellSize + 1, e.cellSize - 1, e.cellSize - 1);
                Thread.Sleep(60);
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Graphics g = this.CreateGraphics();
            //bulmaca1.gridSize = Convert.ToInt32(textBox1.Text);

            Alan_ayarlama(bulmaca1);
            Engelleri_Belirleme(bulmaca1);
            GridCiz(bulmaca1, g);
            HücreleriBoya(bulmaca1, g);
        }

        private void button3_Click(object sender, EventArgs e)
        {
            time.Start();
            Graphics g = this.CreateGraphics();
            AStarAlgorithm algorithm = new AStarAlgorithm();
            for (int i = 0; i < Grafik.Count; i++)
            {
                if (Grafik[i].x == opening.x && Grafik[i].y == opening.y)
                {
                    open = i;
                    Grafik[i].opening = true;
                    break;
                }
            }
            HedefArayisi.Add(Grafik[open]);
            Node node = AlternatifHedefBul(Grafik[open], stack, HedefArayisi);
            CozumYolu = algorithm.FindPath(opening, node, Grafik);
            //En kısa yol sarı renk ile taranıyor
            HedefArayisi.Add(node);
            HedefArayisiCiz(HedefArayisi, bulmaca1, g);
            YolCiz(CozumYolu, bulmaca1, g);
            time.Stop();
            label4.Text = (time.ElapsedMilliseconds / 1000.0).ToString() + " saniye";
            label5.Text = HedefArayisi.Count.ToString() + " kare";
            label6.Text = CozumYolu.Count.ToString() + " kare";

        }

        private void button4_Click(object sender, EventArgs e)
        {
            string adres = "http://bilgisayar.kocaeli.edu.tr/prolab2/url1.txt";
            WebRequest istek = HttpWebRequest.Create(adres);
            WebResponse cevap;
            cevap = istek.GetResponse();
            StreamReader donenbilgiler = new StreamReader(cevap.GetResponseStream());
            bulmaca1.gameGridInfo = donenbilgiler.ReadToEnd();
            string[] gamegriduzunluk = bulmaca1.gameGridInfo.Split('\n');
            bulmaca1.gridSize = gamegriduzunluk[0].Length - 1;
            button4.Enabled = false;
            button5.Enabled = false;
        }

        private void button5_Click(object sender, EventArgs e)
        {
            string adres = "http://bilgisayar.kocaeli.edu.tr/prolab2/url2.txt";
            WebRequest istek = HttpWebRequest.Create(adres);
            WebResponse cevap;
            cevap = istek.GetResponse();
            StreamReader donenbilgiler = new StreamReader(cevap.GetResponseStream());
            bulmaca1.gameGridInfo = donenbilgiler.ReadToEnd();
            string[] gamegriduzunluk = bulmaca1.gameGridInfo.Split('\n');
            bulmaca1.gridSize = gamegriduzunluk[0].Length - 1;
            button4.Enabled = false;
            button5.Enabled = false;
        }

        private void button2_Click(object sender, EventArgs e)
        {

            using (OpenFileDialog textsec = new OpenFileDialog() { Filter = "Txt|*.txt", Title = "Text Dosyaları" })
            {
                if (textsec.ShowDialog() == DialogResult.OK)
                {
                    textBox5.Text = textsec.FileName;
                    StreamReader oku = new StreamReader(textsec.FileName);
                    if (File.Exists(textsec.FileName))
                    {
                        bulmaca1.gameGridInfo = oku.ReadToEnd();
                    }
                }
            }

            string[] gamegriduzunluk = bulmaca1.gameGridInfo.Split('\n');
            bulmaca1.gridSize = gamegriduzunluk[0].Length - 1;

        }

    }

    public class Engel
    {
        public Brush kalem;
        public Engel(string renk)
        {
            if (renk.Contains("koyu yeşil"))
            {
                kalem = Brushes.DarkGreen;
            }

            else if (renk.Contains("yeşil"))
            {
                kalem = Brushes.Green;
            }

            else if (renk.Contains("açık yeşil"))
            {
                kalem = Brushes.DarkSeaGreen;
            }

            else if (renk.Contains("mavi"))
            {
                kalem = Brushes.Blue;
            }

            else if (renk.Contains("kırmızı"))
            {
                kalem = Brushes.Red;
            }
        }

    }

    public class Izgara
    {
        public int gridSize = 5; // grid boyutu
        public int cellSize = 50; // hücre boyutu
        public int margin = 50; // kenarlık
        public int[,] gameGrid; // oyun alanı
        public string gameGridInfo; // txt'den gelen alan bilgisi

        public Izgara()
        {

        }

        public Izgara(int cellSize, int margin)
        {
            this.cellSize = cellSize;
            this.margin = margin;
        }
    }

    public class Node
    {
        public int x;
        public int y;
        public int gScore;
        public int hScore;
        public int fScore;
        public bool obstacle = false;
        public bool goal = false;
        public bool exploration = false;
        public bool opening = false;
        public Node parent;
        public List<Node> neighbors = new List<Node>();

        public Node(int x, int y, bool obstacle)
        {
            this.x = x;
            this.y = y;
            this.obstacle = obstacle;
            this.gScore = 0;
            this.hScore = 0;
            this.fScore = 0;
            this.parent = null;
        }

        public void engelAta()
        {
            this.obstacle = true;
        }


    }

    public class AStarAlgorithm
    {
        public List<Node> FindPath(Node start, Node goal, List<Node> graph)
        {
            //Henüz ziyaret edilmemiş düğümler
            List<Node> openSet = new List<Node>() { start };
            //Ziyaret edilmiş düğümler
            HashSet<Node> closedSet = new HashSet<Node>();


            start.gScore = 0;
            start.hScore = Heuristic(start, goal);
            start.fScore = start.gScore + start.hScore;

            while (openSet.Count > 0)
            {
                Node current = GetNodeWithLowestFScore(openSet);


                if (current.x == goal.x && current.y == goal.y)
                {
                    return ReconstructPath(current);
                }

                openSet.Remove(current);
                closedSet.Add(current);

                foreach (Node neighbor in GetNeighbors(current, graph))
                {
                    if (closedSet.Contains(neighbor))
                    {
                        continue;
                    }

                    int tentativeGScore = current.gScore + GetDistance(current, neighbor);

                    if (!openSet.Contains(neighbor))
                    {
                        openSet.Add(neighbor);
                    }
                    else if (tentativeGScore >= neighbor.gScore)
                    {
                        continue;
                    }

                    neighbor.parent = current;
                    neighbor.gScore = tentativeGScore;
                    neighbor.hScore = Heuristic(neighbor, goal);
                    neighbor.fScore = neighbor.gScore + neighbor.hScore;
                }
            }

            return null;
        }


        //Bir düğüm ile hedef düğümü arası x ve y indis farklarının toplamını döndürür
        private static int Heuristic(Node node, Node goal)
        {
            return Math.Abs(node.x - goal.x) + Math.Abs(node.y - goal.y);
        }

        //Tüm düğümleri dolanır ve ana düğümün komşusu olanları bir liste halinde geri döndürür
        private static List<Node> GetNeighbors(Node node, List<Node> graph)
        {
            List<Node> neighbors = new List<Node>();

            foreach (Node neighbor in graph)
            {
                if ((Math.Abs(node.x - neighbor.x) == 1 && node.y == neighbor.y) ||
                    (node.x == neighbor.x && Math.Abs(node.y - neighbor.y) == 1))
                {
                    if (!neighbor.obstacle)
                    {
                        neighbors.Add(neighbor);
                    }
                }
            }

            return neighbors;
        }

        //iki düğüm arası x ve y indis farklarının toplamını döndürür
        private static int GetDistance(Node node1, Node node2)
        {
            return Math.Abs(node1.x - node2.x) + Math.Abs(node1.y - node2.y);
        }

        //Minimum maliyeti hesaplama
        private static Node GetNodeWithLowestFScore(List<Node> openSet)
        {
            Node nodeWithLowestFScore = openSet[0];

            for (int i = 1; i < openSet.Count; i++)
            {
                if (openSet[i].fScore < nodeWithLowestFScore.fScore)
                {
                    nodeWithLowestFScore = openSet[i];
                }
            }

            return nodeWithLowestFScore;
        }

        private static List<Node> ReconstructPath(Node current)
        {
            List<Node> path = new List<Node>() { current };

            while (current.parent != null)
            {
                current = current.parent;
                path.Insert(0, current);
            }

            return path;
        }
    }

}
