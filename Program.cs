namespace ConsoleSweeper;

class ConsoleSweeper
{
    static void Main()
    {
        ConsoleSweeper consoleSweeper = new ConsoleSweeper();
        
        consoleSweeper.Game();
    }

    private void Game()
    {
        int[,] filed;
        int bombs;
        int foundCells = 0;
        
        bool[,] revealed;
        bool[,] flagedCells;
        bool lost = false;

        while (true)
        {
            //TODO: Write code for different difficulties
            Console.Write("Choose the prefered difficulty:\n1. Easy\n2. Intermediate\n3. Expert\n\nFor custom difficulty, enter the number of rows and columns and bombs. Integers separated by space (3 3 3): ");
                
            var input = Console.ReadLine()?
                .Split(" ")
                .Where(n => int.TryParse(n, out _))
                .Select(s => int.Parse(s))
                .ToArray() ?? Array.Empty<int>();

            if (input.Length == 3)
            {
                filed = putBombs(input[0], input[1], input [2]);
                bombs = input[2];
                revealed = new bool[input[0], input[1]];
                flagedCells = new bool [input[0], input[1]]; 
                break;
            }
            else if (input.Length == 1)
            {
                // Beginner
                if (input[0] == 1) 
                {
                    filed = putBombs(9, 9, 10);
                    bombs = 10;
                    revealed = new bool[9, 9];
                    flagedCells = new bool[9, 9];
                    break;
                }
                // Intermediate
                else if (input[0] == 2)
                {
                    filed = putBombs(16, 16, 40);
                    bombs = 40;
                    revealed = new bool[16, 16];
                    flagedCells = new bool[16, 16];
                    break;
                }
                //Expert
                else if (input[0] == 3)
                {
                    filed = putBombs(30, 16, 99);
                    bombs = 99;
                    revealed = new bool[30, 16];
                    flagedCells = new bool[30, 16];
                    break;
                }
                else
                {
                    Console.WriteLine("Invalid input");
                }
            }
            else
            {
                Console.WriteLine("Invalid input");
            }
                
            
        }
        
        while (true)
        {
            Console.Clear();
            
            for (int i = 0; i < filed.GetLength(0); i++)
            {
                for (int j = 0; j < filed.GetLength(1); j++) //💣
                {
                    if (revealed[i, j])
                        if (filed[i,j] == -1)
                        {
                            Console.Write("💣  ");
                            lost = true;
                        }
                        else
                        {
                            Console.Write($"{filed[i, j],2} ");
                        }
                    else if (flagedCells [i, j])
                    {
                        Console.Write("🚩 ");
                    }
                    else
                        Console.Write("■  ");
                }
                Console.WriteLine();    
            }

            if (lost)
            {
                Console.WriteLine("\nYou Lost!");
                break;
            }
            
            //Win condition
            if (foundCells == filed.GetLength(0) * filed.GetLength(1) - bombs)
            {
                Console.Write("\nYou WIN!");
                break;
            }
            
            //TODO: Implement logic for choosing and reveling cells. Maybe just keep track of the cells that are reveled in the list and then when displaying the matrix, use the list instead of +10
            Console.Write("\nEnter the cell to reveal. To flag a cell add a \"flag\" or \"f\" at the start (f 3 3): ");
            string? input= Console.ReadLine();
            bool flag = false;
            
            if (input!= null)
            {
                flag = (input.ToLower().StartsWith("flag") || input.ToLower().StartsWith("f"));
            }

            
            int[] cordinates = input?
                .Split(" ")
                .Where(n => int.TryParse(n, out int b))
                .Select(n => int.Parse(n))
                .ToArray() ?? Array.Empty<int>();

            if (cordinates.Length == 2 && !flag)
            {
                //Reveal a cell and then check nearby cells, if thay are empty (0) reveal them
                revealed[cordinates[0] - 1, cordinates[1] - 1] = true;
                foundCells++;
                
                if (filed[cordinates[0] - 1, cordinates[1] - 1] != -1)
                {
                    Queue< (int row, int col)> checkCells = new Queue<(int, int)>();
                    
                    checkCells.Enqueue((cordinates[0] - 1, cordinates[1] - 1));

                    //Check the nearby cells of the newly revealed ones
                    while (checkCells.Count != 0)
                    {
                        var current = checkCells.Dequeue();

                        if (filed [current.row, current.col] == 0)
                        {
                            // Top
                            if (current.row - 1 >= 0 && !revealed[current.row - 1, current.col])
                            {
                                if (filed[current.row - 1, current.col] == 0)
                                {
                                    checkCells.Enqueue((current.row - 1, current.col));
                                }
                                revealed[current.row - 1, current.col] = true;
                                foundCells++;
                            }

                            // Left
                            if (current.col - 1 >= 0 && !revealed[current.row, current.col - 1])
                            {
                                if (filed[current.row, current.col - 1] == 0)
                                {
                                    checkCells.Enqueue((current.row, current.col - 1));
                                }
                                revealed[current.row, current.col - 1] = true;
                                foundCells++;
                            }

                            // Right
                            if (current.col + 1 < filed.GetLength(1) && !revealed[current.row, current.col + 1])
                            {
                                if (filed[current.row, current.col + 1] == 0)
                                {
                                    checkCells.Enqueue((current.row, current.col + 1));
                                }
                                revealed[current.row, current.col + 1] = true;
                                foundCells++;
                            }

                            // Bottom
                            if (current.row + 1 < filed.GetLength(0) && !revealed[current.row + 1, current.col])
                            {
                                if (filed[current.row + 1, current.col] == 0)
                                {
                                    checkCells.Enqueue((current.row + 1, current.col));
                                }
                                revealed[current.row + 1, current.col] = true;
                                foundCells++;
                            }
                            
                            //Corners
                            // Top-left corner
                            if (current.row - 1 >= 0 && current.col - 1 >= 0 && !revealed[current.row - 1, current.col - 1]) {
                                if (filed[current.row - 1, current.col -1] == 0)
                                {
                                    checkCells.Enqueue((current.row - 1, current.col - 1));
                                }
                                revealed[current.row - 1, current.col - 1] = true;
                                foundCells++;
                            }

                            // Top-right corner
                            if (current.row - 1 >= 0 && current.col + 1 < filed.GetLength(1) && !revealed[current.row - 1, current.col + 1])
                            {
                                if (filed[current.row - 1, current.col + 1] == 0)
                                {
                                    checkCells.Enqueue((current.row - 1, current.col + 1));
                                }
                                revealed[current.row - 1, current.col + 1] = true;
                                foundCells++;
                            }

                            // Bottom-left corner
                            if (current.row + 1 < filed.GetLength(0) && current.col - 1 >= 0 && !revealed[current.row + 1, current.col - 1])
                            {
                                if (filed[current.row + 1, current.col - 1] == 0)
                                {
                                    checkCells.Enqueue((current.row + 1, current.col - 1));
                                }
                                revealed[current.row + 1, current.col - 1] = true;
                                foundCells++;
                            }

                            // Bottom-right corner
                            if (current.row + 1 < filed.GetLength(0) && current.col + 1 < filed.GetLength(1) && !revealed[current.row + 1, current.col + 1])
                            {
                                if (filed[current.row + 1, current.col + 1] == 0)
                                {
                                    checkCells.Enqueue((current.row + 1, current.col + 1));
                                }
                                revealed[current.row + 1, current.col + 1] = true;
                                foundCells++;
                            } 
                        }
                        
                    }
                }
            }
            
            //Flag the cordinates
            else if (cordinates.Length == 2 && flag)
            {
                if (flagedCells [cordinates[0] - 1, cordinates[1] - 1])
                {
                    flagedCells[cordinates[0] - 1, cordinates[1] - 1] = false;
                }
                else
                {
                    flagedCells [cordinates[0] - 1, cordinates[1] - 1] = true;
                }
            }
            else
            {
                Console.WriteLine("Invalid input");
            }
        }
    }

    private static int[,] putBombs(int rows, int columns, int bombs)
    {
        Random random = new Random();

        int[,] filed = new int [rows, columns];

        //Place all the bombs
        while (bombs != 0)
        {
            int rowBomb = random.Next(rows);
            int colBomb = random.Next(columns);

            if (filed[rowBomb, colBomb] != -1)
            {
                //TODO: Check for bomb clusters that are too big
                filed[rowBomb, colBomb] = -1;
                bombs--;

                //Increase the numbers in cells near bombs
                for (int i = rowBomb - 1; i <= rowBomb + 1; i++)
                {
                    //for each cell in a row and column that is not outside of matrix
                    if (i < rows && i >= 0)
                    {
                        for (int j = colBomb - 1; j <= colBomb + 1; j++)
                        {
                            if (j < columns && j >= 0 && filed[i, j] != -1)
                            {
                                filed[i, j] += 1;
                            }
                        }
                    }
                }
            }
        }
        
        // Add +10 to each no-bomb cell to differentiate which cells are revealed and which are not
        // for (int i = 0; i < rows; i++)
        // {
        //     for (int j = 0; j < columns; j++)
        //     {
        //         if (filed[i, j] != -1)
        //         {
        //             filed[i, j] += 10;
        //         }
        //     }
        // }
        
        // for (int i = 0; i < rows; i++)
        // {
        //     for (int j = 0; j < columns; j++)
        //     {
        //         Console.Write(filed [i, j] + " ");
        //     }
        //     Console.WriteLine();
        // }
        // Console.WriteLine();

        return filed;
    }
}