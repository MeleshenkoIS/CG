﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CG
{
    public class clsLoadData
    {
        // массивы для чтения координат и полигонов
        public static string[] lines;
        public static string[] words;
        public static string[] numbers;

        // вершины полигонов
        public static double x;
        public static double y;
        public static double z;
        public static List<cls3D_Point> pointsForLineTrans = new List<cls3D_Point>();
        public static List<cls3D_pointModified> pointsForTransformation = new List<cls3D_pointModified>();
        public static List<clsNorm> normsCoordinates = new List<clsNorm>();
        // сами полигоны
        public static int polygon1;
        public static int polygon2;
        public static int polygon3;
        public static List<clsPolygon> polygonsForLineTrans = new List<clsPolygon>();
        public static List<clsPolygonModified> polygonsForTransformation = new List<clsPolygonModified>();
        public static List<clsNormsPolygon> normPolygons = new List<clsNormsPolygon>();

        public static int flagNumberLW;
        // считываем и записываем вершины
        public static List<cls3D_Point> loadTopsFromObjectFileForLineTrans()
        {
            lines = File.ReadAllLines("Test.obj");                                  // считывание информации из файла
            for (int i = 0; i < lines.Length; i++)
            {
                words = lines[i].Split(' ');
                if (words[0] == "v")                                                // ищем вершины
                {
                    // CultureInfo.InvariantCulture позволяет читать десятичные числа как 0.хххх, а не 0,ххх
                    x = double.Parse(words[1], CultureInfo.InvariantCulture);       // считываем врешины
                    y = double.Parse(words[2], CultureInfo.InvariantCulture);
                    z = double.Parse(words[3], CultureInfo.InvariantCulture);
                    cls3D_Point point = new cls3D_Point(x, y, z, 5000, 600);       // инициализируем точку по координатам
                    pointsForLineTrans.Add(point);                                             // записываем в лист точку
                }
            }
            return pointsForLineTrans; // возвращаем массив точек
        }
        // считываем и записываем полигоны
        public static List<clsPolygon> loadPolygonsFromObjectFileWithOutCheck()
        {
            pointsForLineTrans = loadTopsFromObjectFileForLineTrans();                            // считываем информацию
            for (int i = 0; i < lines.Length; i++)
            {
                words = lines[i].Split(' ');
                if (words[0] == "f")
                {
                    words = lines[i].Split(' ', '/');                               // считываем координаты полигонов          
                    polygon1 = int.Parse(words[1]);
                    polygon2 = int.Parse(words[4]);
                    polygon3 = int.Parse(words[7]);
                    clsPolygon polygon = new clsPolygon();                      //  записываем полигон
                    polygon[0] = pointsForLineTrans[polygon1 - 1];
                    polygon[1] = pointsForLineTrans[polygon2 - 1];
                    polygon[2] = pointsForLineTrans[polygon3 - 1];
                    polygonsForLineTrans.Add(polygon);                                          // сохраняем в лист
                }

            }
            return polygonsForLineTrans;
        }

        public static List<clsPolygon> loadPolygonsFromObjectFileWithCheck()
        {
            pointsForLineTrans = loadTopsFromObjectFileForLineTrans();                     // считываем информацию
            for (int i = 0; i < lines.Length; i++)
            {
                words = lines[i].Split(' ');
                if (words[0] == "f")
                {
                    words = lines[i].Split(' ', '/');                               // считываем координаты полигонов          
                    polygon1 = int.Parse(words[1]);
                    polygon2 = int.Parse(words[4]);
                    polygon3 = int.Parse(words[7]);
                    clsPolygon polygon = new clsPolygon();                      //  записываем полигон
                    polygon[0] = pointsForLineTrans[polygon1 - 1];
                    polygon[1] = pointsForLineTrans[polygon2 - 1];
                    polygon[2] = pointsForLineTrans[polygon3 - 1];
                    clsBarycentricCoordinates barycentricCoordinates = new clsBarycentricCoordinates(polygon);
                    barycentricCoordinates.Calculating_lambda_coefficients(polygon[0]);
                    if (Math.Abs(1 - barycentricCoordinates.Lambda0 - barycentricCoordinates.Lambda1 -
                                 barycentricCoordinates.Lambda2) > 0.001)
                    {
                        MessageBox.Show("Сумма барицентрических координат не равна 1!", "Ошибка",
                            MessageBoxButtons.OK);
                    }
                    polygonsForLineTrans.Add(polygon);                                          // сохраняем в лист
                }
            }
            return polygonsForLineTrans;

        }

        public static List<cls3D_pointModified> loadTopsFromObjectFileForTransformation()
        {
            lines = File.ReadAllLines("Test.obj");                                  // считывание информации из файла
            for (int i = 0; i < lines.Length; i++)
            {
                words = lines[i].Split(' ');
                if (words[0] == "v")                                                // ищем вершины
                {
                    // CultureInfo.InvariantCulture позволяет читать десятичные числа как 0.хххх, а не 0,ххх
                    x = double.Parse(words[1], CultureInfo.InvariantCulture);       // считываем врешины
                    y = double.Parse(words[2], CultureInfo.InvariantCulture);
                    z = double.Parse(words[3], CultureInfo.InvariantCulture);
                    cls3D_pointModified point = new cls3D_pointModified(x, y, z);       // инициализируем точку по координатам
                    pointsForTransformation.Add(point);                                             // записываем в лист точку
                }
            }
            return pointsForTransformation; // возвращаем массив точек
        }

        public static List<clsPolygonModified> loadPolygonsFromObjectFileWithCheckForTransformation()
        {
            pointsForTransformation = loadTopsFromObjectFileForTransformation();                     // считываем информацию
            for (int i = 0; i < lines.Length; i++)
            {
                words = lines[i].Split(' ');
                if (words[0] == "f")
                {
                    words = lines[i].Split(' ', '/');                               // считываем координаты полигонов          
                    polygon1 = int.Parse(words[1]);
                    polygon2 = int.Parse(words[4]);
                    polygon3 = int.Parse(words[7]);
                    clsPolygonModified polygon = new clsPolygonModified();                      //  записываем полигон
                    polygon[0] = pointsForTransformation[polygon1 - 1];
                    polygon[1] = pointsForTransformation[polygon2 - 1];
                    polygon[2] = pointsForTransformation[polygon3 - 1];
                    clsBarycentricCoordinates barycentricCoordinates = new clsBarycentricCoordinates(polygon);
                    barycentricCoordinates.calc_lambda_for_Original(polygon[0]);
                    if (Math.Abs(1 - barycentricCoordinates.Lambda0 - barycentricCoordinates.Lambda1 -
                                 barycentricCoordinates.Lambda2) > 0.001)
                    {
                        MessageBox.Show("Сумма барицентрических координат не равна 1!", "Ошибка",
                            MessageBoxButtons.OK);
                    }
                    polygonsForTransformation.Add(polygon);                                          // сохраняем в лист
                }
            }
            return polygonsForTransformation;

        }

        public static List<clsNorm> loadCoordinatesNormsFromObjectFile()
        {
            lines = File.ReadAllLines("Test.obj");                                  // считывание информации из файла
            for (int i = 0; i < lines.Length; i++)
            {
                words = lines[i].Split(' ');
                if (words[0] == "vn")                                                // ищем вершины
                {
                    // CultureInfo.InvariantCulture позволяет читать десятичные числа как 0.хххх, а не 0,ххх
                    x = double.Parse(words[1], CultureInfo.InvariantCulture);       // считываем врешины
                    y = double.Parse(words[2], CultureInfo.InvariantCulture);
                    z = double.Parse(words[3], CultureInfo.InvariantCulture);
                    clsNorm norm = new clsNorm(x, y, z);       // инициализируем точку по координатам
                    normsCoordinates.Add(norm);                                             // записываем в лист точку
                }
            }
            return normsCoordinates; // возвращаем массив точек
        }

        public static List<clsNormsPolygon> loadNormsPolygonsFromObjectFile()
        {
            normsCoordinates = loadCoordinatesNormsFromObjectFile();                     // считываем информацию
            for (int i = 0; i < lines.Length; i++)
            {
                words = lines[i].Split(' ');
                if (words[0] == "f")
                {
                    words = lines[i].Split(' ', '/');                               // считываем координаты полигонов          
                    polygon1 = int.Parse(words[3]);
                    polygon2 = int.Parse(words[6]);
                    polygon3 = int.Parse(words[9]);
                    clsNormsPolygon normsPolygon = new clsNormsPolygon();                      //  записываем полигон
                    normsPolygon[0] = normsCoordinates[polygon1 - 1];
                    normsPolygon[1] = normsCoordinates[polygon2 - 1];
                    normsPolygon[2] = normsCoordinates[polygon3 - 1];
                    normPolygons.Add(normsPolygon);                                          // сохраняем в лист
                }
            }
            return normPolygons;

        }
    }
}
