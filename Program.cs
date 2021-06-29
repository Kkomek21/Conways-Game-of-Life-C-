using System;
using System.Collections.Generic;
using System.Threading;
using System.IO;

/*Plan
Projekt implementujacy "Gre w zycie"; Automat komorkowy stworzony przez Johna Conwaya.
Istnieje możliwość zmiany parametrów, zmieniając wartości w strukturze "parametry".
Wymagania techniczne:
1) conajmniej jedną strukturę - sensownie użytą [Spełnione]
2) conajmniej jedną funkcję - sensownie użytą [Spełnione]
3) obsługę plików - czytanie lub/i zapisywanie. [Spełnione (zapisanie stanu początkowego)] */

namespace Game
{
    public class game
    {
        //Przechowuje parametry uzyte w grze
        struct parametry
        {
            public static int symbol_alive = 64;
            public static int symbol_pusty = 39;
            public static int percent_alive = 20;
            public static int wymiar_x = 20;
            public static int wymiar_y = 20;
            public static int predkosc_wyswietlania = 500;
            public static List<int> neighbours_reproduce = new List<int> {3};
            public static List<int> neighbours_live = new List<int> {2, 3};
        }
        
        //Tworzy matrycę o danych wymiarach wypełnioną "pustymi" symbolami
        static int[,] Create_matrix(int x, int y)
        {
            int column, row;
            
            int[,] matrix = new int[x, y];
            for (row = 0; row < y; row++)
            {
                for (column = 0; column < x; column++)
                {
                    matrix[row, column] = parametry.symbol_pusty;
                }
            }
            return matrix;
        }
        
        
        // Losuje koordynaty, na ich miejscach wsadza "aktywne" symbole
        private static int[,]  Random_coordinates()
        {
            int i;
            List<int> random_coords = new List<int>();
            var rand = new Random();
            int[,] random = Create_matrix(parametry.wymiar_x, parametry.wymiar_y);
            int ilosc_zywych = ((parametry.wymiar_x * parametry.wymiar_y) / 100) * parametry.percent_alive;
            for (i = 0; i < ilosc_zywych; i++)
            {
                random_coords.Clear();
                random_coords.Add(rand.Next(parametry.wymiar_x));
                random_coords.Add(rand.Next(parametry.wymiar_y));
                random[random_coords[0], random_coords[1]] = parametry.symbol_alive;

            }
            return random;
        }
        
        //Funkcja używana do zapisu aktualnego stanu gry w Current directory
        static int[,] Zapis_stanu(int[,] uniwersum)
        {
            int i, j;
            string do_zapisu = "";
            for (i = 0; i < parametry.wymiar_x; i++)
            {
                for (j = 0; j < parametry.wymiar_y; j++)
                {
                    do_zapisu += uniwersum[i, j];
                }
            }
            File.WriteAllText("Zapis.txt", do_zapisu);
            return uniwersum;
        }
        
        //Funkcja sprawdza, czy komorka w danych koordynatach jest aktywna, czy nie.
        //Zwraca 1 jesli tak, 0 jesli nie.
        static int Alive_Check(int[,] universe, int x, int y)
        {
            if (universe[x, y] == parametry.symbol_alive)
            {
                return 1;
            }
            else
            {
                return 0;
            }
        }
        
        
        //Glowna funkcja gry, w obrebie jednego pokolenia:
        //sprawdza sasiadow kazdej z komorek, sumuje ich ilosc
        //nastepnie zgodnie z warunkami oznacza te komorke jako zywa, lub martwa na nastepne pokolenie
        //Na koniec zamienia wszystkie wyznaczone komorki
        static int[,] Generation(int[,] uniwersum, int cells_x, int cells_y, List<int> neighbours_reproduce, List<int> neighbours_live)
        {
            List<List<int>> change_to_1 = new List<List<int>>();
            List<List<int>> change_to_0 = new List<List<int>>();
            int kolumna, rzad, n, cell, i, j;
            
            for (rzad = 0; rzad < cells_y; rzad++)
            {
                for (kolumna = 0; kolumna < cells_x; kolumna++)
                {
                    cell = uniwersum[rzad, kolumna];
                    n = 0;

                    //Sprawdza ilosc sasiadow, sumuje ich ilosc
                    if (rzad == 0)
                    {
                        if (kolumna == 0)
                        {
                            n += Alive_Check(uniwersum, rzad + 1, kolumna);
                            n += Alive_Check(uniwersum, rzad, kolumna + 1);
                            n += Alive_Check(uniwersum, rzad + 1, kolumna + 1);
                        }
                        else if (kolumna == cells_x - 1)
                        {

                            n += Alive_Check(uniwersum, rzad + 1, kolumna);
                            n += Alive_Check(uniwersum, rzad + 1, kolumna - 1);
                            n += Alive_Check(uniwersum, rzad, kolumna - 1);
                        }
                        else
                        {
                            n += Alive_Check(uniwersum, rzad + 1, kolumna + 1);
                            n += Alive_Check(uniwersum, rzad + 1, kolumna);
                            n += Alive_Check(uniwersum, rzad + 1, kolumna - 1);
                            n += Alive_Check(uniwersum, rzad, kolumna + 1);
                            n += Alive_Check(uniwersum, rzad, kolumna - 1);
                        }
                    }
                    else if (rzad == cells_x - 1)
                    {

                        if (kolumna == 0)
                        {
                            n += Alive_Check(uniwersum, rzad - 1, kolumna);
                            n += Alive_Check(uniwersum, rzad - 1, kolumna + 1);
                            n += Alive_Check(uniwersum, rzad, kolumna + 1);
                        }
                        else if (kolumna == cells_x - 1)
                        {
                            n += Alive_Check(uniwersum, rzad - 1, kolumna);
                            n += Alive_Check(uniwersum, rzad - 1, kolumna - 1);
                            n += Alive_Check(uniwersum, rzad, kolumna - 1);
                        }
                        else
                        {
                            n += Alive_Check(uniwersum, rzad, kolumna + 1);
                            n += Alive_Check(uniwersum, rzad, kolumna - 1);
                            n += Alive_Check(uniwersum, rzad - 1, kolumna);
                            n += Alive_Check(uniwersum, rzad - 1, kolumna + 1);
                            n += Alive_Check(uniwersum, rzad - 1, kolumna - 1);
                        }
                    }

                    else
                    {
                        if (kolumna == 0)
                        {
                            n += Alive_Check(uniwersum, rzad, kolumna + 1);
                            n += Alive_Check(uniwersum, rzad + 1, kolumna);
                            n += Alive_Check(uniwersum, rzad + 1, kolumna + 1);
                            n += Alive_Check(uniwersum, rzad - 1, kolumna);
                            n += Alive_Check(uniwersum, rzad - 1, kolumna + 1);
                        }
                        else if (kolumna == cells_x - 1)
                        {
                            n += Alive_Check(uniwersum, rzad, kolumna - 1);
                            n += Alive_Check(uniwersum, rzad + 1, kolumna);
                            n += Alive_Check(uniwersum, rzad + 1, kolumna - 1);
                            n += Alive_Check(uniwersum, rzad - 1, kolumna);
                            n += Alive_Check(uniwersum, rzad - 1, kolumna - 1);
                        }
                        else
                        {
                            n += Alive_Check(uniwersum, rzad, kolumna + 1);
                            n += Alive_Check(uniwersum, rzad, kolumna - 1);
                            n += Alive_Check(uniwersum, rzad + 1, kolumna);
                            n += Alive_Check(uniwersum, rzad + 1, kolumna + 1);
                            n += Alive_Check(uniwersum, rzad + 1, kolumna - 1);
                            n += Alive_Check(uniwersum, rzad - 1, kolumna);
                            n += Alive_Check(uniwersum, rzad - 1, kolumna + 1);
                            n += Alive_Check(uniwersum, rzad - 1, kolumna - 1);
                        }
                    }
                    
                    
                    //Oznacza komorki na nastepne pokolenie
                    if (cell == parametry.symbol_alive)
                    {
                        if (neighbours_live.Contains(n) == false)
                        {
                            List<int> temp1 = new List<int> {rzad, kolumna};
                            change_to_0.Add(temp1);
                        }
                    }
                    if (cell == parametry.symbol_pusty)
                    {
                        if (neighbours_reproduce.Contains(n))
                        {
                            List<int> temp2 = new List<int> {rzad, kolumna};
                            change_to_1.Add(temp2);
                        }
                    }
                }
            }
            
            
            //Wyswietla aktualny stan
            Console.Write("\n");
            for (i = 0; i < cells_x; i++)
            {
                for (j = 0; j < cells_y; j++)
                {
                    Console.Write((char)uniwersum[i, j]);
                }
                Console.Write("\n");
            }
            
            
            
            //Zamienia oznaczone komorki
            List<int> coord;
            for (int l = 0; l < change_to_0.Count; l++)
            {
                coord = change_to_0[l];
                uniwersum[coord[0], coord[1]] = parametry.symbol_pusty;
            }
            for (int k = 0; k < change_to_1.Count; k++)
            {
                coord = change_to_1[k];
                uniwersum[coord[0], coord[1]] = parametry.symbol_alive;
            }
            Thread.Sleep(parametry.predkosc_wyswietlania);
            Console.Clear();
            return uniwersum;
        }
        
        
        //Główna funkcja
        static void Main()
        {
            int[,] universe = Random_coordinates(); //tworzy losowe "uniwersum"
            Zapis_stanu(universe); //zapisuje stan poczatkowy na dysku
            
            //Główna pętla, wywołuje Generation(), ktora posuwa jedno pokolenie do przodu
            while (true){
                universe = Generation(universe, parametry.wymiar_x, parametry.wymiar_y, parametry.neighbours_reproduce, parametry.neighbours_live);
            }
            
        }
    }
}