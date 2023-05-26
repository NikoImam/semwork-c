using Aspose.Cells;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GraphPartitorApp.Models
{
    public static class ExcelToMatrixAdapter
    {
        public static int[,] GetMatrix(string path)
        {
            var wb = new Workbook(path);
            var worksheet = wb.Worksheets[0];
            var rows = worksheet.Cells.MaxDataRow+1;
            var cols = worksheet.Cells.MaxDataColumn+1;

            if (rows != cols)
            {
                throw new Exception("rows != cols");
            }

            var matrix = new int[rows, cols];

            var enumerableRows = Enumerable.Range(0, rows);
            var enumerableCols = Enumerable.Range(0, cols);


            if (!enumerableRows.All(i => 
                    enumerableCols.All(j => worksheet.Cells[i, j].Value is int))
                )
            {
                throw new Exception("Ошибка символа");
            }

            if (!enumerableRows.All(i =>
                    (int)worksheet.Cells[i, i].Value == 0
                    && enumerableCols.All(j =>
                        {
                            matrix[i, j] = (int)worksheet.Cells[i, j].Value;
                            return (int)worksheet.Cells[i, j].Value == (int)worksheet.Cells[j, i].Value;
                        }
                    )
                ))
            {
                throw new Exception("Некорректная матрица смежности");
            }

            return matrix;
        }
    }
}