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
        int[,] fieled;
        int bombs;
        int foundCells = 0;
        int [] fieledParam = new int [3];
        
        bool[,] revealed;
        bool[,] flagedCells;
        bool lost = false;
        bool bombsPut = false;

        while (true)
        {
            Console.Write("Choose the prefered difficulty:\n1. Easy\n2. Intermediate\n3. Expert\n\nFor custom difficulty, enter the number of rows and columns and bombs. Integers separated by space (3 3 3): ");
                
            var input = Console.ReadLine()?
                .Split(" ")
                .Where(n => int.TryParse(n, out _))
                .Select(s => int.Parse(s))
                .ToArray() ?? Array.Empty<int>();


            //TODO: After the fix the output problem, and make it finnaly less ugly. Add numbers for the rows and columns


            if (input.Length == 3 || input.Length == 1)
            {
                if (input.Length == 1)
                {
                    // Beginner
                    if (input[0] == 1)
                    {
                        fieledParam = new int[] { 9, 9, 10 };
                    }
                    // Intermediate
                    else if (input[0] == 2)
                    {
                        fieledParam = new int[] {16, 16, 40};
                    }
                    //Expert
                    else if (input[0] == 3)
                    {
                        fieledParam = new int[] {30, 16, 99};
                    }
                }
                else
                {
                    fieledParam = new int[] {input[0], input[1], input[2]};
                }

                //Assine all the variables and temporary fieled
                fieled = new int[fieledParam[0], fieledParam[1]];
                bombs = fieledParam[2];
                revealed = new bool[fieledParam[0], fieledParam[1]];
                flagedCells = new bool[fieledParam[0], fieledParam[1]];
                break;
            }
            else
            {
                Console.WriteLine("Invalid input");
            }
        }

        
        while (true)
        {
            Console.Clear();
            
            for (int i = 0; i < fieled.GetLength(0); i++)
            {
                for (int j = 0; j < fieled.GetLength(1); j++) //💣
                {
                    if (revealed[i, j])
                        if (fieled[i,j] == -1)
                        {
                            Console.Write("💣  ");
                            lost = true;
                        }
                        else
                        {
                            Console.Write($"{fieled[i, j],2} ");
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
            if (foundCells == fieled.GetLength(0) * fieled.GetLength(1) - bombs)
            {
                Console.Write("\nYou WIN!");
                break;
            }
            
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
            

            if (cordinates.Length == 2 && !flag && cordinates[0] > 0 && cordinates[0] < fieled.GetLength(0) && cordinates[1] > 0 && cordinates[1] < fieled.GetLength(1))
            {
                if (!bombsPut)
                {
                    fieled = putBombs(fieledParam[0], fieledParam[1], fieledParam[2], cordinates[0], cordinates[1]);
                    bombsPut = true;
                }
                
                //Reveal a cell and then check nearby cells, if thay are empty (0) reveal them
                revealed[cordinates[0] - 1, cordinates[1] - 1] = true;
                foundCells++;
                
                if (fieled[cordinates[0] - 1, cordinates[1] - 1] != -1)
                {
                    Queue< (int row, int col)> checkCells = new Queue<(int, int)>();
                    
                    checkCells.Enqueue((cordinates[0] - 1, cordinates[1] - 1));

                    //Check the nearby cells of the newly revealed ones
                    while (checkCells.Count != 0)
                    {
                        var current = checkCells.Dequeue();

                        if (fieled [current.row, current.col] == 0)
                        {
                            // Top
                            if (current.row - 1 >= 0 && !revealed[current.row - 1, current.col])
                            {
                                if (fieled[current.row - 1, current.col] == 0)
                                {
                                    checkCells.Enqueue((current.row - 1, current.col));
                                }
                                revealed[current.row - 1, current.col] = true;
                                foundCells++;
                            }

                            // Left
                            if (current.col - 1 >= 0 && !revealed[current.row, current.col - 1])
                            {
                                if (fieled[current.row, current.col - 1] == 0)
                                {
                                    checkCells.Enqueue((current.row, current.col - 1));
                                }
                                revealed[current.row, current.col - 1] = true;
                                foundCells++;
                            }

                            // Right
                            if (current.col + 1 < fieled.GetLength(1) && !revealed[current.row, current.col + 1])
                            {
                                if (fieled[current.row, current.col + 1] == 0)
                                {
                                    checkCells.Enqueue((current.row, current.col + 1));
                                }
                                revealed[current.row, current.col + 1] = true;
                                foundCells++;
                            }

                            // Bottom
                            if (current.row + 1 < fieled.GetLength(0) && !revealed[current.row + 1, current.col])
                            {
                                if (fieled[current.row + 1, current.col] == 0)
                                {
                                    checkCells.Enqueue((current.row + 1, current.col));
                                }
                                revealed[current.row + 1, current.col] = true;
                                foundCells++;
                            }
                            
                            //Corners
                            // Top-left corner
                            if (current.row - 1 >= 0 && current.col - 1 >= 0 && !revealed[current.row - 1, current.col - 1]) {
                                if (fieled[current.row - 1, current.col -1] == 0)
                                {
                                    checkCells.Enqueue((current.row - 1, current.col - 1));
                                }
                                revealed[current.row - 1, current.col - 1] = true;
                                foundCells++;
                            }

                            // Top-right corner
                            if (current.row - 1 >= 0 && current.col + 1 < fieled.GetLength(1) && !revealed[current.row - 1, current.col + 1])
                            {
                                if (fieled[current.row - 1, current.col + 1] == 0)
                                {
                                    checkCells.Enqueue((current.row - 1, current.col + 1));
                                }
                                revealed[current.row - 1, current.col + 1] = true;
                                foundCells++;
                            }

                            // Bottom-left corner
                            if (current.row + 1 < fieled.GetLength(0) && current.col - 1 >= 0 && !revealed[current.row + 1, current.col - 1])
                            {
                                if (fieled[current.row + 1, current.col - 1] == 0)
                                {
                                    checkCells.Enqueue((current.row + 1, current.col - 1));
                                }
                                revealed[current.row + 1, current.col - 1] = true;
                                foundCells++;
                            }

                            // Bottom-right corner
                            if (current.row + 1 < fieled.GetLength(0) && current.col + 1 < fieled.GetLength(1) && !revealed[current.row + 1, current.col + 1])
                            {
                                if (fieled[current.row + 1, current.col + 1] == 0)
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

    private static int[,] putBombs(int rows, int columns, int bombs, int inputRow, int inputCol)
    {
        Random random = new Random();

        int[,] fieled = new int [rows, columns];

        //Place all the bombs
        while (bombs != 0)
        {
            int rowBomb = random.Next(rows);
            int colBomb = random.Next(columns);

            if (fieled[rowBomb, colBomb] != -1 && (rowBomb != inputRow || colBomb != inputCol))
            {
                //TODO: Check for bomb clusters that are too big
                fieled[rowBomb, colBomb] = -1;
                bombs--;

                //Increase the numbers in cells near bombs
                for (int i = rowBomb - 1; i <= rowBomb + 1; i++)
                {
                    //for each cell in a row and column that is not outside of matrix
                    if (i < rows && i >= 0)
                    {
                        for (int j = colBomb - 1; j <= colBomb + 1; j++)
                        {
                            if (j < columns && j >= 0 && fieled[i, j] != -1)
                            {
                                fieled[i, j] += 1;
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
        //         if (fieled[i, j] != -1)
        //         {
        //             fieled[i, j] += 10;
        //         }
        //     }
        // }
        
        // for (int i = 0; i < rows; i++)
        // {
        //     for (int j = 0; j < columns; j++)
        //     {
        //         Console.Write(fieled [i, j] + " ");
        //     }
        //     Console.WriteLine();
        // }
        // Console.WriteLine();

        return fieled;
    }
}