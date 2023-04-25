using System;

public class Izgara
{
    private int gridSize = 5; // grid boyutu
    private int cellSize = 50; // hücre boyutu
    private int margin = 50; // kenarlık
    private int[,] gameGrid; // oyun alanı
    private string gameGridInfo;

    public Izgara()
	{

	}

    public Izgara(int gridSize,int cellSize,int margin,int[,] gameGrid,string gameGridInfo)
    {
        this.gridSize = gridSize;
        this.cellSize = cellSize;
        this.margin = margin;
        this.gameGrid = gameGrid;
        this.gameGridInfo = gameGridInfo;
    }

    private void Alan_ayarlama(int gridSize1)
    {
        // oyun alanını oluşturma
        this.gameGrid = new int[gridSize1, gridSize1];
        for (int i = 0; i < gridSize1; i++)
        {
            for (int j = 0; j < gridSize1; j++)
            {
                this.gameGrid[i, j] = 0;
            }
        }

        // form boyutunu ayarlama
        this.ClientSize = new Size(this.cellSize * gridSize1 + this.margin * 2, this.cellSize * gridSize1 + this.margin * 2);
    }


    private void Engelleri_Belirleme(string hücreler, int gridSize0)
    {
        if (hücreler != null)
        {
            string[] hücreler2 = hücreler.Split('\n');

            List<int> hücreler3()
            {
                List<int> deger = new List<int>();
                for (int i = 0; i < hücreler2.Length; i++)
                {
                    char[] chars = hücreler2[i].ToCharArray();
                    for (int j = 0; j < chars.Length; j++)
                    {
                        if (j != gridSize0)
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
            for (int i = 0; i < gridSize0; i++)
            {
                for (int j = 0; j < gridSize0; j++)
                {
                    this.gameGrid[i, j] = hücreler3()[k];
                    k++;
                }
            }
        }

    }


}
