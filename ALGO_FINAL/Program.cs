using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Media;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;

namespace ALGO_FINAL
{
    public class Program
    {
        /// <summary>
        /// Rend l'appelation de l'image correcte, respectant le fromat x.bmp
        /// </summary>
        /// <param name="nom"></param> Nom du fihier à vérifier
        /// <returns></returns> Nom de fichier correcte sous le format x.bmp
        static public string NomDuFichier(string nom)
        {
            char[] chaine = nom.ToCharArray();
            char[] chaineFinale;
            if (chaine.Length >= 4)
            {
                if (chaine[chaine.Length - 4] == '.' && chaine[chaine.Length - 3] == 'b' && chaine[chaine.Length - 2] == 'm' && chaine[chaine.Length - 1] == 'p')
                    chaineFinale = new char[chaine.Length];
                else
                    chaineFinale = new char[chaine.Length + 4];
            }
            else
            {
                chaineFinale = new char[chaine.Length + 4];
            }
            for (int i = 0; i < chaine.Length; i++)
            {
                chaineFinale[i] = chaine[i];
            }
            if (chaineFinale.Length != chaine.Length)
            {
                chaineFinale[chaineFinale.Length - 4] = '.';
                chaineFinale[chaineFinale.Length - 3] = 'b';
                chaineFinale[chaineFinale.Length - 2] = 'm';
                chaineFinale[chaineFinale.Length - 1] = 'p';
            }
            string nomFinal = new string(chaineFinale);
            return nomFinal;
        }

        /// <summary>
        /// Vérifie que la chaine de caractère de type string est bien en caractères alphanumériques
        /// </summary>
        /// <param name="message"></param> chaine de string
        /// <returns></returns> true si c'est bien des caractères alphanumériques, false sinon
        static public bool Alpha(string message)
        {
            char[] tabChar = message.ToCharArray();
            int val = 0;
            bool alpha = true;
            for (int i = 0; i < tabChar.Length; i++)
            {
                val = QrCode.CharToAlpha(tabChar[i]);
                if (val < 0)
                {
                    alpha = false;
                    break;
                }
            }
            return alpha;
        }

        /// <summary>
        /// On a regroupé les différents types de traitement ensemble, pour rendre le corps plus clair. Ici, il s'agit de toutes les modifications que l'on peut apporter à une image déjà existante
        /// </summary>
        static void Traitement1()
        {
            char recommencer = 'N';
            int tour = 0;
            string myFileReturn = "";
            MyImage image;
            do
            {
                tour++;
                if (tour == 1)
                {
                    Console.WriteLine("Quelle image voulez-vous traiter ? \n" + "Nous vous conseillons de consulter la liste des images disponibles dans le fichier bin>debug. \n");
                    string nom = NomDuFichier(Console.ReadLine());
                    image = new MyImage(nom);
                }
                else
                {
                    Console.WriteLine("Voulez-vous traiter l'image que vous venez de créer ? ");
                    char ancienne = Console.ReadLine().ToUpper()[0];
                    if (ancienne == 'O')
                        image = new MyImage(myFileReturn);
                    else
                    {
                        Console.WriteLine("Quelle image voulez-vous traiter ? \n" + "Nous vous conseillons de consulter la liste des images disponibles dans le fichier bin>debug. \n");
                        string nom = NomDuFichier(Console.ReadLine());
                        image = new MyImage(nom);
                    }
                }

                Console.WriteLine("Voici la liste des transformations possibles : \n" +
                        "1 : Photo en nuances de gris \n" +
                        "2 : Photo en noir et blanc \n" +
                        "3 : Photo rétrécie \n" +
                        "4 : Photo agrandie \n" +
                        "5 : Rotation avec un angle quelconque \n" +
                        "6 : Effet miroir par rapport au côté \n" +
                        "7 : Effet miroir par rapport au dessus ou au dessous \n" +
                        "8 : Symétrie par rapport aux angles \n" +
                        "9 : Application de filtres");
                int nTrans = 0;
                do
                {
                    int test = 0;
                    Console.WriteLine("Veuillez entrer le numéro de la tranformation voulue : > \n");
                    while (test == 0)
                    {
                        try
                        {
                            nTrans = Convert.ToInt32(Console.ReadLine());
                            test = 1;
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine("Veuillez entrer un numéro entier : ");
                        }
                    }
                } while (nTrans <= 0 || nTrans > 9);

                switch (nTrans)
                {
                    case 1:
                        {
                            image.NuanceDeGris();
                            Console.WriteLine("Sous quel nom voulez-vous enregistrer l'image traitée ? > \n");
                            myFileReturn = NomDuFichier(Console.ReadLine());
                            image.From_Image_To_File(myFileReturn);
                            break;
                        }
                    case 2:
                        {
                            image.NoirEtBlanc();
                            Console.WriteLine("Sous quel nom voulez-vous enregistrer l'image traitée ? > \n");
                            myFileReturn = NomDuFichier(Console.ReadLine());
                            image.From_Image_To_File(myFileReturn);
                            break;
                        }
                    case 3:
                        {
                            image.Retrecir();
                            Console.WriteLine("Sous quel nom voulez-vous enregistrer l'image traitée ? > \n");
                            myFileReturn = NomDuFichier(Console.ReadLine());
                            image.From_Image_To_File(myFileReturn);
                            break;
                        }
                    case 4:
                        {
                            image.Agrandir();
                            Console.WriteLine("Sous quel nom voulez-vous enregistrer l'image traitée ? > \n");
                            myFileReturn = NomDuFichier(Console.ReadLine());
                            image.From_Image_To_File(myFileReturn);
                            break;
                        }
                    case 5:
                        {
                            Console.WriteLine("De quel angle voulez-vous faire tourner la photo (dans le sens trigonométrique/antihoraire) ?");
                            int test = 0;
                            int degres = 0;
                            while (test == 0)
                            {
                                try
                                {
                                    degres = Convert.ToInt32(Console.ReadLine());
                                    test = 1;
                                }
                                catch (Exception e)
                                {
                                    Console.WriteLine("Veuillez entrer un nombre entier : ");
                                }
                            }
                            image.Rotation(degres);
                            Console.WriteLine("Sous quel nom voulez-vous enregistrer l'image traitée ? > \n");
                            myFileReturn = NomDuFichier(Console.ReadLine());
                            image.From_Image_To_File(myFileReturn);
                            break;
                        }
                    case 6:
                        {
                            image.Miroir_Cotes();
                            Console.WriteLine("Sous quel nom voulez-vous enregistrer l'image traitée ? > \n");
                            myFileReturn = NomDuFichier(Console.ReadLine());
                            image.From_Image_To_File(myFileReturn);
                            break;
                        }
                    case 7:
                        {
                            image.Miroir_Haut_Bas();
                            Console.WriteLine("Sous quel nom voulez-vous enregistrer l'image traitée ? > \n");
                            myFileReturn = NomDuFichier(Console.ReadLine());
                            image.From_Image_To_File(myFileReturn);
                            break;
                        }
                    case 8:
                        {
                            image.Miroir_Angles();
                            Console.WriteLine("Sous quel nom voulez-vous enregistrer l'image traitée ? > \n");
                            myFileReturn = NomDuFichier(Console.ReadLine());
                            image.From_Image_To_File(myFileReturn);
                            break;
                        }
                    case 9:
                        {
                            Console.WriteLine("Voici la liste des filtres disponibles : \n" +
                                "1 : Flou \n" +
                                "2 : Amélioration de la netteté \n" +
                                "3 : Détection des contours \n" +
                                "4 : Atténuation des bruits \n" +
                                "5 : Apparition des dimensions \n");

                            int nFiltre = 0;
                            do
                            {
                                int test = 0;
                                Console.WriteLine("Veuillez entrer le numéro du filtre que voulez appliquer : > \n");
                                while (test == 0)
                                {
                                    try
                                    {
                                        nFiltre = Convert.ToInt32(Console.ReadLine());
                                        test = 1;
                                    }
                                    catch (Exception e)
                                    {
                                        Console.WriteLine("Veuillez entrer un numéro entier : ");
                                    }
                                }
                            } while (nFiltre <= 0 || nFiltre > 5);

                            switch (nFiltre)
                            {
                                case 1:
                                    {
                                        Console.WriteLine("Voici la liste des flous que vous avez à disposition : \n" +
                                            "1 : Flou uniforme \n" +
                                            "2 : Flou de Gauss en 3*3 \n" +
                                            "3 : Flous de Gauss en 5*5 \n" +
                                            "4 : Masque de flou \n" +
                                            "5 : Flou cinétique \n");
                                        int nFlou = 0;
                                        do
                                        {
                                            int test = 0;
                                            Console.WriteLine("Veuillez entrer le numéro du flou que vous voulez appliquer : > \n");
                                            while (test == 0)
                                            {
                                                try
                                                {
                                                    nFlou = Convert.ToInt32(Console.ReadLine());
                                                    test = 1;
                                                }
                                                catch (Exception e)
                                                {
                                                    Console.WriteLine("Veuillez entrer un numéro entier : ");
                                                }
                                            }
                                        } while (nFlou <= 0 || nFlou > 5);
                                        switch (nFlou)
                                        {
                                            case 1:
                                                int[,] mat1 = { { 1, 1, 1 }, { 1, 1, 1 }, { 1, 1, 1 } };
                                                image.ApplicationFiltreUnique(mat1, 0);
                                                Console.WriteLine("Sous quel nom voulez-vous enregistrer l'image traitée ? > \n");
                                                myFileReturn = NomDuFichier(Console.ReadLine());
                                                image.From_Image_To_File(myFileReturn);
                                                break;
                                            case 2:
                                                int[,] mat2 = { { 1, 2, 1 }, { 2, 4, 2 }, { 1, 2, 1 } };
                                                image.ApplicationFiltreUnique(mat2, 0);
                                                Console.WriteLine("Sous quel nom voulez-vous enregistrer l'image traitée ? > \n");
                                                myFileReturn = NomDuFichier(Console.ReadLine());
                                                image.From_Image_To_File(myFileReturn);
                                                break;
                                            case 3:
                                                int[,] mat3 = { { 1, 4, 6, 4, 1 }, { 4, 16, 24, 16, 4 }, { 6, 24, 36, 24, 6 }, { 4, 16, 24, 16, 4 }, { 1, 4, 6, 4, 1 } };
                                                image.ApplicationFiltreUnique(mat3, 0);
                                                Console.WriteLine("Sous quel nom voulez-vous enregistrer l'image traitée ? > \n");
                                                myFileReturn = NomDuFichier(Console.ReadLine());
                                                image.From_Image_To_File(myFileReturn);
                                                break;
                                            case 4:
                                                int[,] mat4 = { { 1, 4, 6, 4, 1 }, { 4, 16, 24, 16, 4 }, { 6, 24, -476, 24, 6 }, { 4, 16, 24, 16, 4 }, { 1, 4, 6, 4, 1 } };
                                                image.ApplicationFiltreUnique(mat4, 0);
                                                Console.WriteLine("Sous quel nom voulez-vous enregistrer l'image traitée ? > \n");
                                                myFileReturn = NomDuFichier(Console.ReadLine());
                                                image.From_Image_To_File(myFileReturn);
                                                break;
                                            case 5:
                                                int[,] mat5 = { { 1, 0, 0, 0, 0, 0, 0, 0, 0 }, { 0, 1, 0, 0, 0, 0, 0, 0, 0 }, { 0, 0, 1, 0, 0, 0, 0, 0, 0 }, { 0, 0, 0, 1, 0, 0, 0, 0, 0 }, { 0, 0, 0, 0, 1, 0, 0, 0, 0 }, { 0, 0, 0, 0, 0, 1, 0, 0, 0 }, { 0, 0, 0, 0, 0, 0, 1, 0, 0 }, { 0, 0, 0, 0, 0, 0, 0, 1, 0 }, { 0, 0, 0, 0, 0, 0, 0, 0, 1 } };
                                                image.ApplicationFiltreUnique(mat5, 0);
                                                Console.WriteLine("Sous quel nom voulez-vous enregistrer l'image traitée ? > \n");
                                                myFileReturn = NomDuFichier(Console.ReadLine());
                                                image.From_Image_To_File(myFileReturn);
                                                break;
                                        }
                                        break;
                                    }
                                case 2:
                                    {
                                        Console.WriteLine("Voici la liste des possibilités concernant l'amélioration de la netteté : \n" +
                                            "1 : Faible netteté \n" +
                                            "2 : Netteté exagérée  \n" +
                                            "3 : Contour excessif \n");
                                        int nNet = 0;
                                        do
                                        {
                                            int test = 0;
                                            Console.WriteLine("Veuillez entrer le numéro de l'amélioration de netteté que vous voulez appliquer : > \n");
                                            while (test == 0)
                                            {
                                                try
                                                {
                                                    nNet = Convert.ToInt32(Console.ReadLine());
                                                    test = 1;
                                                }
                                                catch (Exception e)
                                                {
                                                    Console.WriteLine("Veuillez entrer un numéro entier : ");
                                                }
                                            }
                                        } while (nNet <= 0 || nNet > 3);
                                        switch (nNet)
                                        {
                                            case 1:
                                                int[,] mat1 = { { 0, -1, 0 }, { -1, 5, -1 }, { 0, -1, 0 } };
                                                image.ApplicationFiltreUnique(mat1, 0);
                                                Console.WriteLine("Sous quel nom voulez-vous enregistrer l'image traitée ? > \n");
                                                myFileReturn = NomDuFichier(Console.ReadLine());
                                                image.From_Image_To_File(myFileReturn);
                                                break;
                                            case 2:
                                                int[,] mat2 = { { -1, -1, -1 }, { -1, 9, -1 }, { -1, -1, -1 } };
                                                image.ApplicationFiltreUnique(mat2, 0);
                                                Console.WriteLine("Sous quel nom voulez-vous enregistrer l'image traitée ? > \n");
                                                myFileReturn = NomDuFichier(Console.ReadLine());
                                                image.From_Image_To_File(myFileReturn);
                                                break;
                                            case 3:
                                                int[,] mat3 = { { 1, 1, 1 }, { 1, -7, 1 }, { 1, 1, 1 } };
                                                image.ApplicationFiltreUnique(mat3, 0);
                                                Console.WriteLine("Sous quel nom voulez-vous enregistrer l'image traitée ? > \n");
                                                myFileReturn = NomDuFichier(Console.ReadLine());
                                                image.From_Image_To_File(myFileReturn);
                                                break;
                                        }
                                        break;
                                    }
                                case 3:
                                    {
                                        Console.WriteLine("Voici la liste des différents contours : \n" +
                                            "1 : Contour de Prewitt avec l'axe vertical ou horizontal \n" +
                                            "2 : Contour de Roberts avec l'axe à 45 degrés \n" +
                                            "3 : Contour de Kirsh avec l'axe vertical ou horizontal \n" +
                                            "4 : Contour MDIF avec l'axe vertical ou horizontal \n" +
                                            "5 : Contour du second ordre \n" +
                                            "6 : Contour détecté dans toutes les directions \n");
                                        int nCont = 0;
                                        do
                                        {
                                            int test = 0;
                                            Console.WriteLine("Veuillez entrer le numéro du contour que vous voulez appliquer : > \n");
                                            while (test == 0)
                                            {
                                                try
                                                {
                                                    nCont = Convert.ToInt32(Console.ReadLine());
                                                    test = 1;
                                                }
                                                catch (Exception e)
                                                {
                                                    Console.WriteLine("Veuillez entrer un numéro entier : ");
                                                }
                                            }
                                        } while (nCont <= 0 || nCont > 6);

                                        switch (nCont)
                                        {
                                            case 1:
                                                int[,] mat1_1 = { { -1, 0, 1 }, { -1, 0, 1 }, { -1, 0, 1 } };
                                                int[,] mat1_2 = { { -1, -1, -1 }, { 0, 0, 0 }, { 1, 1, 1 } };
                                                image.ApplicationFiltreDouble(mat1_1, mat1_2, 0, 0);
                                                Console.WriteLine("Sous quel nom voulez-vous enregistrer l'image traitée ? > \n");
                                                myFileReturn = NomDuFichier(Console.ReadLine());
                                                image.From_Image_To_File(myFileReturn);
                                                break;
                                            case 2:
                                                int[,] mat2_1 = { { 0, 0, 0 }, { 0, 0, 1 }, { 0, -1, 0 } };
                                                int[,] mat2_2 = { { 0, 0, 0 }, { 0, -1, 0 }, { 0, 0, 1 } };
                                                image.ApplicationFiltreDouble(mat2_1, mat2_2, 0, 0);
                                                Console.WriteLine("Sous quel nom voulez-vous enregistrer l'image traitée ? > \n");
                                                myFileReturn = NomDuFichier(Console.ReadLine());
                                                image.From_Image_To_File(myFileReturn);
                                                break;
                                            case 3:
                                                int[,] mat3_1 = { { -3, -3, 5 }, { -3, 0, 5 }, { -3, -3, 5 } };
                                                int[,] mat3_2 = { { -3, -3, -3 }, { -3, 0, -3 }, { 5, 5, 5 } };
                                                image.ApplicationFiltreDouble(mat3_1, mat3_2, 0, 0);
                                                Console.WriteLine("Sous quel nom voulez-vous enregistrer l'image traitée ? > \n");
                                                myFileReturn = NomDuFichier(Console.ReadLine());
                                                image.From_Image_To_File(myFileReturn);
                                                break;
                                            case 4:
                                                int[,] mat4_1 = { { 0, -1, 0, 1, 0 }, { -1, -2, 0, 2, 1 }, { -1, -3, 0, 3, 1 }, { -1, -2, 0, 2, 1 }, { 0, -1, 0, 1, 0 } };
                                                int[,] mat4_2 = { { 0, -1, -1, -1, 0 }, { -1, -2, -3, -2, -1 }, { 0, 0, 0, 0, 0 }, { 1, 2, 3, 2, 1 }, { 0, 1, 1, 1, 0 } };
                                                image.ApplicationFiltreDouble(mat4_1, mat4_2, 0, 0);
                                                Console.WriteLine("Sous quel nom voulez-vous enregistrer l'image traitée ? > \n");
                                                myFileReturn = NomDuFichier(Console.ReadLine());
                                                image.From_Image_To_File(myFileReturn);
                                                break;
                                            case 5:
                                                int[,] mat5 = { { 0, 0, 1, 1, 1, 0, 0 }, { 0, 1, 1, 1, 1, 1, 0 }, { 1, 1, -1, -4, -1, 1, 1 }, { 1, 1, -4, -8, -4, 1, 1 }, { 1, 1, -1, -4, -1, 1, 1 }, { 0, 1, 1, 1, 1, 1, 0 }, { 0, 0, 1, 1, 1, 0, 0 } };
                                                image.ApplicationFiltreUnique(mat5, 0);
                                                Console.WriteLine("Sous quel nom voulez-vous enregistrer l'image traitée ? > \n");
                                                myFileReturn = NomDuFichier(Console.ReadLine());
                                                image.From_Image_To_File(myFileReturn);
                                                break;
                                            case 6:
                                                int[,] mat6 = { { -1, -1, -1 }, { -1, 8, -1 }, { -1, -1, -1 } };
                                                image.ApplicationFiltreUnique(mat6, 0);
                                                Console.WriteLine("Sous quel nom voulez-vous enregistrer l'image traitée ? > \n");
                                                myFileReturn = NomDuFichier(Console.ReadLine());
                                                image.From_Image_To_File(myFileReturn);
                                                break;
                                        }
                                        break;
                                    }
                                case 4:
                                    {
                                        int voisinage = 0;
                                        do
                                        {
                                            int test = 0;
                                            Console.WriteLine("Veuillez entrer la taille du voisinage (impaire) : > \n");
                                            while (test == 0)
                                            {
                                                try
                                                {
                                                    voisinage = Convert.ToInt32(Console.ReadLine());
                                                    test = 1;
                                                }
                                                catch (Exception e)
                                                {
                                                    Console.WriteLine("Veuillez entrer un numéro entier et impair : ");
                                                }
                                            }
                                        } while (voisinage % 2 == 0);
                                        image.CoupeMediane(voisinage);
                                        Console.WriteLine("Sous quel nom voulez-vous enregistrer l'image traitée ? > \n");
                                        myFileReturn = NomDuFichier(Console.ReadLine());
                                        image.From_Image_To_File(myFileReturn);
                                        break;
                                    }
                                case 5:
                                    {
                                        Console.WriteLine("Voici la liste des différentes manières de faire apparaître les dimensions : \n" +
                                            "1 : Avec un angle de 45° \n" +
                                            "2 : De manière exagérée \n");
                                        int nDim = 0;
                                        do
                                        {
                                            int test = 0;
                                            Console.WriteLine("Veuillez entrer le numéro correspondant à ce que vous voulez faire : > \n");
                                            while (test == 0)
                                            {
                                                try
                                                {
                                                    nDim = Convert.ToInt32(Console.ReadLine());
                                                    test = 1;
                                                }
                                                catch (Exception e)
                                                {
                                                    Console.WriteLine("Veuillez entrer un numéro entier : ");
                                                }
                                            }
                                        } while (nDim <= 0 || nDim > 2);

                                        switch (nDim)
                                        {
                                            case 1:
                                                int[,] mat1 = { { -1, -1, 0 }, { -1, 0, 1 }, { 0, 1, 1 } };
                                                image.ApplicationFiltreUnique(mat1, 128);
                                                Console.WriteLine("Sous quel nom voulez-vous enregistrer l'image traitée ? > \n");
                                                myFileReturn = NomDuFichier(Console.ReadLine());
                                                image.From_Image_To_File(myFileReturn);
                                                break;
                                            case 2:
                                                int[,] mat2 = { { -1, -1, -1, -1, 0 }, { -1, -1, -1, 0, 1 }, { -1, -1, 0, 1, 1 }, { -1, 0, 1, 1, 1 }, { 0, 1, 1, 1, 1 } };
                                                image.ApplicationFiltreUnique(mat2, 128);
                                                Console.WriteLine("Sous quel nom voulez-vous enregistrer l'image traitée ? > \n");
                                                myFileReturn = NomDuFichier(Console.ReadLine());
                                                image.From_Image_To_File(myFileReturn);
                                                break;
                                        }

                                        break;
                                    }
                            }
                            break;
                        }
                }
                Console.WriteLine("Voulez-vous refaire une de ces transformations ? ");
                recommencer = Console.ReadLine().ToUpper()[0];
            } while (recommencer == 'O');
        }

        /// <summary>
        /// Il s'agit ici de traiter des caractéristiques d'une image en donnant ses histogrammes de couleur
        /// </summary>
        static void Traitement2()
        {
            char recommencer = 'N';
            string myFile;
            string myFileReturn = "";
            do
            {
                Console.WriteLine("Voici les histogrammes que vous pouvez réaliser : \n" +
                    "1 : Histogramme de la luminosité \n" +
                    "2 : Histogramme du rouge \n" +
                    "3 : Histogramme du vert \n" +
                    "4 : Histogramme du bleu \n");
                int nHist = 0;
                do
                {
                    Console.WriteLine("Quel histogramme voulez-vous réaliser ? \n" + "Veuillez entrer le nombre associé à cet histogramme.");
                    int test = 0;
                    while (test == 0)
                    {
                        try
                        {
                            nHist = Convert.ToInt32(Console.ReadLine());
                            test = 1;
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine("Veuillez entrer un numéro entier : ");
                        }
                    }
                } while (nHist <= 0 || nHist > 4);

                int hauteur = 0;
                do
                {
                    Console.WriteLine("Quelle hauteur (positive) voulez-vous donner à votre histogramme ? \n" + "Nous vous conseillons une hauteur de 120 pixels \n");
                    int test = 0;
                    while (test == 0)
                    {
                        try
                        {
                            hauteur = Convert.ToInt32(Console.ReadLine());
                            test = 1;
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine("Veuillez entrer un nombre entier : ");
                        }
                    }
                } while (hauteur < 100);
                int largeur = 256;
                
                Console.WriteLine("Quelle image voulez-vous traiter ? ");
                myFile = NomDuFichier(Console.ReadLine());
                MyImage image = new MyImage(myFile);
                Console.WriteLine("Sous quel nom voulez-vous enregistrer votre histogramme ?");
                myFileReturn = NomDuFichier(Console.ReadLine());
                Histogramme histogramme = new Histogramme(myFileReturn, hauteur, largeur);

                switch (nHist)
                {
                    case 1:
                        {
                            histogramme.HistoLuminosite(image);
                            histogramme.From_Histo_To_File();
                            break;
                        }
                    case 2:
                        {
                            histogramme.HistoRouge(image);
                            histogramme.From_Histo_To_File();
                            break;
                        }
                    case 3:
                        {
                            histogramme.HistoVert(image);
                            histogramme.From_Histo_To_File();
                            break;
                        }
                    case 4:
                        {
                            histogramme.HistoBleu(image);
                            histogramme.From_Histo_To_File();
                            break;
                        }
                }
                Console.WriteLine("Voulez-vous refaire un histogramme ?");
                recommencer = Console.ReadLine().ToUpper()[0];
            } while (recommencer == 'O');
        }

        /// <summary>
        /// Il s'agit ici de tout ce qui concerne la création d'une fractale
        /// </summary>
        static void Traitement3()
        {
            char recommencer = 'N';
            string myFileReturn = "";
            do
            {
                Console.WriteLine("Voici les fractales que vous pouvez créer : \n" +
                    "1 : La fractale de Mandelbrot \n" +
                    "2 : La fractale de Julia \n");
                int nFra = 0;
                do
                {
                    Console.WriteLine("Quelle fractale voulez-vous créer ? \n" + "Veuillez entrer le nombre associé à cette fractale.");
                    int test = 0;
                    while (test == 0)
                    {
                        try
                        {
                            nFra = Convert.ToInt32(Console.ReadLine());
                            test = 1;
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine("Veuillez entrer un numéro entier : ");
                        }
                    }
                } while (nFra <= 0 || nFra > 2);

                int hauteur = 0;
                do
                {
                    Console.WriteLine("Quelle hauteur (positive) voulez-vous donner à votre fractale ? \n");
                    int test = 0;
                    while (test == 0)
                    {
                        try
                        {
                            hauteur = Convert.ToInt32(Console.ReadLine());
                            test = 1;
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine("Veuillez entrer un nombre entier : ");
                        }
                    }
                } while (hauteur <= 0);

                int largeur = 0;
                do
                {
                    Console.WriteLine("Quelle largeur (positive) voulez-vous donner à votre fractale ? \n");
                    int test = 0;
                    while (test == 0)
                    {
                        try
                        {
                            largeur = Convert.ToInt32(Console.ReadLine());
                            test = 1;
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine("Veuillez entrer un nombre entier : ");
                        }
                    }
                } while (largeur <= 0);

                switch (nFra)
                {
                    case 1:
                        {
                            Console.WriteLine("Sous quel nom voulez-vous enregistrer votre fractale de Mandelbrot ? ");
                            myFileReturn = NomDuFichier(Console.ReadLine());
                            Fractales fractale = new Fractales(myFileReturn, hauteur, largeur);
                            fractale.FractaleDeMandelbrot();
                            fractale.From_Fractale_To_File();
                            break;
                        }
                    case 2:
                        {
                            Console.WriteLine("Sous quel nom voulez-vous enregistrer votre fractale de Julia ? ");
                            myFileReturn = NomDuFichier(Console.ReadLine());
                            Fractales fractale = new Fractales(myFileReturn, hauteur, largeur);
                            fractale.FractaleDeJulia();
                            fractale.From_Fractale_To_File();
                            break;
                        }
                }
                Console.WriteLine("Voulez-vous refaire une fractale ?");
                recommencer = Console.ReadLine().ToUpper()[0];
            } while (recommencer == 'O');
        }

        /// <summary>
        /// Il s'agit ici de cacher une image dans une autre ou alors de dissocier deux images mises ensemble
        /// </summary>
        static void Traitement4()
        {
            char recommencer = 'N';
            string myFile;
            string myFileReturn = "";
            do
            {
                Console.WriteLine("Ici, vous pouvez : \n" +
                    "1 : Créer une image à partir de deux images (une image en cachera une autre) \n" +
                    "2 : Extraire deux images d'une image \n");
                int nSte = 0;
                do
                {
                    Console.WriteLine("Que voulez-vous faire ? \n" + "Veuillez entrer le nombre associé.");
                    int test = 0;
                    while (test == 0)
                    {
                        try
                        {
                            nSte = Convert.ToInt32(Console.ReadLine());
                            test = 1;
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine("Veuillez entrer un numéro entier : ");
                        }
                    }
                } while (nSte <= 0 || nSte > 2);

                switch (nSte)
                {
                    case 1:
                        {
                            Console.WriteLine("Quelle image voudrez-vous mettre devant ? \n" + "Nous vous conseillons de consulter le fichier bin>debug");
                            myFile = NomDuFichier(Console.ReadLine());
                            MyImage imageDevant = new MyImage(myFile);
                            Console.WriteLine("Quelle image voulez-vous mettre derrière ? \n"
                                + "ATTENTION : Les deux images doivent avoir la même largeur et la même hauteur \n");
                            myFile = NomDuFichier(Console.ReadLine());
                            MyImage imageDerriere = new MyImage(myFile);

                            if (imageDevant.MatPixel.GetLength(0) == imageDerriere.MatPixel.GetLength(0) && imageDevant.MatPixel.GetLength(1) == imageDerriere.MatPixel.GetLength(1))
                            {
                                imageDevant.Fusion(imageDerriere);
                                Console.WriteLine("Sous quel nom voulez-vous enregistrer l'image obtenue ? ");
                                myFileReturn = NomDuFichier(Console.ReadLine());
                                imageDevant.From_Image_To_File(myFileReturn);
                            }
                            else
                                Console.WriteLine("Les deux image ne font pas la même taille, il est donc impossible de les regrouper en une seule. \n");
                            break;
                        }
                    case 2:
                        {
                            Console.WriteLine("De quelle image voulez-vous extraire des images ? \n");
                            myFile = NomDuFichier(Console.ReadLine());
                            MyImage image = new MyImage(myFile);
                            Console.WriteLine("Sous quel nom voulez-vous enregistrer l'image principale extraite ? \n");
                            myFileReturn = NomDuFichier(Console.ReadLine());
                            image.ImagePrincipale();
                            image.From_Image_To_File(myFileReturn);
                            Console.WriteLine("Sous quel nom voulez-vous enregistrer l'image cachée extraite ? \n");
                            myFileReturn = NomDuFichier(Console.ReadLine());
                            image.ImageCache();
                            image.From_Image_To_File(myFileReturn);
                            break;
                        }
                }
                Console.WriteLine("Voulez-vous rester sur ce menu ?");
                recommencer = Console.ReadLine().ToUpper()[0];
            } while (recommencer == 'O');
        }

        /// <summary>
        /// Il s'agit ici d'écrire ou de lire un QR Code
        /// </summary>
        static void Traitement5()
        {
            char recommencer = 'N';
            string myFile;
            string myFileReturn = "";
            do
            {
                Console.WriteLine("Ici, vous pouvez : \n" +
                    "1 : Transformer U \n" +
                    "2 : Lire un Qr Code \n");
                int nQr = 0;
                do
                {
                    Console.WriteLine("Que voulez-vous faire ? \n" + "Veuillez entrer le nombre associé.");
                    int test = 0;
                    while (test == 0)
                    {
                        try
                        {
                            nQr = Convert.ToInt32(Console.ReadLine());
                            test = 1;
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine("Veuillez entrer un numéro entier : ");
                        }
                    }
                } while (nQr <= 0 || nQr > 2);

                switch (nQr)
                {
                    case 1:
                        {
                            int test = 0;
                            string message;
                            do
                            {
                                Console.WriteLine("Veuillez saisir le message que vous voulez convertir en Qr Code. \n" + "Attention, votre message ne doit pas faire plus de 45 caractères");
                                message = Console.ReadLine();
                                message = message.ToUpper();
                                bool alpha = Alpha(message);
                                if (alpha == true)
                                {
                                    test = 1;
                                }
                                else
                                {
                                    Console.WriteLine("Le message n'est pas en alphanumérique.");
                                }
                                if (message.Length > 45 || message.Length <= 0)
                                {
                                    test = 2;
                                    Console.WriteLine("La longeur du message ne correspond à aucune version. ");
                                }

                            } while (test != 1);
                            int version = 0;
                            int taille = 0;
                            if (message.Length <= 25)
                            {
                                version = 1;
                                taille = 84;
                                Console.WriteLine("Votre Qr Code sera en version 1");
                            }
                            if (message.Length > 25 && message.Length <= 45)
                            {
                                version = 2;
                                taille = 100;
                                Console.WriteLine("Votre Qr Code sera en version 2");
                            }

                            Console.WriteLine("Sous quel nom voulez-vous enregistrer votre Qr Code ?");
                            myFileReturn = NomDuFichier(Console.ReadLine());
                            QrCode qr = new QrCode(myFileReturn, taille, taille);
                            qr.Creation(version);
                            qr.AppliquerDonnee(version, message);
                            qr.From_QrCode_To_File();
                            break;
                        }
                    case 2:
                        {
                            Console.WriteLine("Quel Qr-Code voulez-vous décripter ?");
                            myFile = NomDuFichier(Console.ReadLine());
                            MyImage image = new MyImage(myFile);
                            //Console.WriteLine("Sous quel nom voulez-vous enregistrer le message votre Qr Code ?");
                            //myFileReturn = NomDuFichier(Console.ReadLine());
                            QrCode qr = new QrCode(image);
                            qr.AfficherMessage();
                            break;
                        }
                }
                Console.WriteLine("Voulez-vous rester sur ce menu ?");
                recommencer = Console.ReadLine().ToUpper()[0];
            } while (recommencer == 'O');
        }

        /// <summary>
        /// Il s'agira ici de toutes les innovations majeures, en plus de celles déjà inclut dans l'autre partie du programme
        /// </summary>
        static void Traitement6()
        {
            char recommencer = 'N';
            string myFile;
            string myFileReturn = "";
            do
            {
                Console.WriteLine("Voici les innovations que vous pouvez réaliser : \n" +
                    "1 : Négatifs d'une image \n" +
                    "2 : Filtre de couleur \n" +
                    "3 : Image Unicolore \n" +
                    "4 : Mosaïque \n");
                int nIn = 0;
                do
                {
                    Console.WriteLine("Quelle innovation voulez-vous réaliser ? \n" + "Veuillez entrer le nombre associé à cette innovation.");
                    int test = 0;
                    while (test == 0)
                    {
                        try
                        {
                            nIn = Convert.ToInt32(Console.ReadLine());
                            test = 1;
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine("Veuillez entrer un numéro entier : ");
                        }
                    }
                } while (nIn <= 0 || nIn > 4);

                Console.WriteLine("Quelle image voulez-vous traiter ? ");
                myFile = NomDuFichier(Console.ReadLine());
                MyImage image = new MyImage(myFile);
                Console.WriteLine("Sous quel nom voulez-vous enregistrer votre image sortante ?");
                myFileReturn = NomDuFichier(Console.ReadLine());

                switch (nIn)
                {
                    case 1:
                        {
                            image.Negatif();
                            image.From_Image_To_File(myFileReturn);
                            break;
                        }
                    case 2:
                        {
                            Console.WriteLine("Voici la liste des filtres disponibles : \n" +
                                "1 : Rouge \n" +
                                "2 : Vert et Jaune \n" +
                                "3 : Bleu \n");

                            int nFiltre = 0;
                            do
                            {
                                int test = 0;
                                Console.WriteLine("Veuillez entrer le numéro du filtre que voulez appliquer : > \n");
                                while (test == 0)
                                {
                                    try
                                    {
                                        nFiltre = Convert.ToInt32(Console.ReadLine());
                                        test = 1;
                                    }
                                    catch (Exception e)
                                    {
                                        Console.WriteLine("Veuillez entrer un numéro entier : ");
                                    }
                                }
                            } while (nFiltre <= 0 || nFiltre > 3);

                            switch(nFiltre)
                            {
                                case 1:
                                    {
                                        image.FiltreRouge();
                                        image.From_Image_To_File(myFileReturn);
                                        break;
                                    }
                                case 2:
                                    {
                                        image.FiltreVert_Jaune();
                                        image.From_Image_To_File(myFileReturn);
                                        break;
                                    }
                                case 3:
                                    {
                                        image.FiltreBleu();
                                        image.From_Image_To_File(myFileReturn);
                                        break;
                                    }
                            }
                            break;
                        }
                    case 3:
                        {
                            Console.WriteLine("Voici la liste des couleurs disponibles : \n" +
                                "1 : Rouge \n" +
                                "2 : Vert  \n" +
                                "3 : Bleu \n" +
                                "4 : Jaune \n");

                            int nCouleur = 0;
                            do
                            {
                                int test = 0;
                                Console.WriteLine("Veuillez entrer le numéro du filtre que voulez appliquer : > \n");
                                while (test == 0)
                                {
                                    try
                                    {
                                        nCouleur = Convert.ToInt32(Console.ReadLine());
                                        test = 1;
                                    }
                                    catch (Exception e)
                                    {
                                        Console.WriteLine("Veuillez entrer un numéro entier : ");
                                    }
                                }
                            } while (nCouleur <= 0 || nCouleur > 4);

                            switch (nCouleur)
                            {
                                case 1:
                                    {
                                        image.UniColore("rouge");
                                        image.From_Image_To_File(myFileReturn);
                                        break;
                                    }
                                case 2:
                                    {
                                        image.UniColore("vert");
                                        image.From_Image_To_File(myFileReturn);
                                        break;
                                    }
                                case 3:
                                    {
                                        image.UniColore("bleu");
                                        image.From_Image_To_File(myFileReturn);
                                        break;
                                    }
                                case 4:
                                    {
                                        image.UniColore("jaune");
                                        image.From_Image_To_File(myFileReturn);
                                        break;
                                    }
                            }
                            break;
                        }
                    case 4:
                        {
                            image.Mosaique();
                            image.From_Image_To_File(myFileReturn);
                            break;
                        }
                }
                Console.WriteLine("Voulez-vous refaire une innovation ?");
                recommencer = Console.ReadLine().ToUpper()[0];
            } while (recommencer == 'O');
        }

        /// <summary>
        /// Regroupe tous les types de traitement possibles
        /// </summary>
        static void Interface_Utilisateur()
        {
            Console.WriteLine("Bonjour, dans ce programme vous allez pouvoir effectuer différents traitements d'image. ");
            char recommencer = 'N';
            do
            {
                Console.WriteLine("MENU DES TYPES DE TRAITEMENTS POSSIBLES : \n"
                    + "1 : Traitement d'une image (les filtres sont aussi dans cette section) \n"
                    + "2 : Histogramme des couleurs d'une image \n"
                    + "3 : Création d'une fractale \n"
                    + "4 : Codage et décodage d'une image dans une image \n"
                    + "5 : Encodage ou lecture d'un QR Code \n"
                    + "6 : Innovation \n");

                int nExo = 0;
                do
                {
                    Console.WriteLine("Quel traitement voulez-vous effectuer ? \n" + "Veuillez entrer le nombre associé à ce traitement.");
                    int test = 0;
                    while (test == 0)
                    {
                        try
                        {
                            nExo = Convert.ToInt32(Console.ReadLine());
                            test = 1;
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine("Veuillez entrer un numéro entier : ");
                        }
                    }
                } while (nExo <= 0 || nExo > 6);

                switch (nExo)
                {
                    case 1:
                        {
                            Traitement1();
                            break;
                        }
                    case 2:
                        {
                            Traitement2();
                            break;
                        }
                    case 3:
                        {
                            Traitement3();
                            break;
                        }
                    case 4:
                        {
                            Traitement4();
                            break;
                        }
                    case 5:
                        {
                            Traitement5();
                            break;
                        }
                    case 6:
                        {
                            Traitement6();
                            break;
                        }
                }
                Console.WriteLine("Voulez-vous effectuer un autre type de traitement ? ");
                recommencer = Console.ReadLine().ToUpper()[0];
            } while (recommencer == 'O');
        }

        static void Main(string[] args)
        {
            Interface_Utilisateur();
            Console.ReadKey();
        }
    }
}
