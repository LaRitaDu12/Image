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
    public class QrCode
    {
        #region ATTRIBUTS

        private string myFileReturn;

        private int tailleFichier;
        private int largeur;
        private int hauteur;
        private int nbBitsParCouleur;
        private int tailleOffset;
        private byte[] octetsSortie;
        private Pixel[,] matPixelSortie;
        private MyImage image;

        #endregion

        #region PROPRIETES
        public string MyFileReturn { get { return this.myFileReturn; } set { this.myFileReturn = value; } }

        public int Hauteur { get { return this.hauteur; } set { this.hauteur = value; } }

        public int Largeur { get { return this.largeur; } set { this.largeur = value; } }

        public MyImage Image { get { return this.image; } set { this.image = value; } }
        #endregion

        #region CONSTRUCTEUR

        public QrCode(string myFileReturn, int hauteur, int largeur) //Pour la création de QrCode
        {
            this.myFileReturn = myFileReturn;
            this.hauteur = hauteur;
            this.largeur = largeur;
        }

        //public QrCode(MyImage image, string myFileReturn) //Pour la lecture de QrCode
        //{
        //    this.myFileReturn = myFileReturn;
        //    this.hauteur = image.MatPixel.GetLength(0);
        //    this.largeur = image.MatPixel.GetLength(1);
        //    this.matPixelSortie = image.MatPixel;
        //}

        public QrCode(MyImage image) //Pour la lecture de QrCode
        {
            this.hauteur = image.MatPixel.GetLength(0);
            this.largeur = image.MatPixel.GetLength(1);
            this.matPixelSortie = image.MatPixel;
        }

        #endregion

        #region METHODES BASIQUES
        /// <summary>
        /// Convertit une valeur du type int en un tableau de bytes du type little endian de la taille demandée
        /// </summary>
        /// <param name="valeur"></param> Valeur à convertir
        /// <param name="taille"></param> Taille du tableau à sortir
        /// <returns></returns> Le tableau de bytes
        static public byte[] Convertir_Int_ToEndian(int valeur, int taille)
        {
            byte[] tabEndian = new byte[taille];
            for (int i = taille - 1; i >= 0; i--)
            {
                tabEndian[i] = Convert.ToByte(valeur / Convert.ToInt32(Math.Pow(256, i)));
                valeur = valeur - tabEndian[i] * Convert.ToInt32(Math.Pow(256, i));
            }
            return tabEndian;
        }

        /// <summary>
        /// Convertit une matrice de pixels en tableau d'octets
        /// </summary>
        public void From_Matrice_To_Tableau()
        {
            int nombreDeZero = ((largeur * 3) % 4);
            if (nombreDeZero != 0)
            {
                nombreDeZero = 4 - nombreDeZero;
            }
            octetsSortie = new byte[54 + hauteur * (largeur * 3 + nombreDeZero)];
            int compteur = 0;
            #region Remplir l'en-tête du fichier
            //Fichier de type bitmap ==> BM
            octetsSortie[compteur] = 66;
            compteur++;
            octetsSortie[compteur] = 77;
            compteur++;
            //Taille totale du fichier en octets
            tailleFichier = 54 + hauteur * (largeur * 3 + nombreDeZero);
            byte[] temp4 = Convertir_Int_ToEndian(tailleFichier, 4);
            for (int i = 0; i < 4; i++)
            {
                octetsSortie[compteur] = temp4[i];
                compteur++;
            }
            //Champ réservé
            temp4 = Convertir_Int_ToEndian(0, 4);
            for (int i = 0; i < 4; i++)
            {
                octetsSortie[compteur] = temp4[i];
                compteur++;
            }
            //Adresse de la zone de définition de l'image
            temp4 = Convertir_Int_ToEndian(54, 4);
            for (int i = 0; i < 4; i++)
            {
                octetsSortie[compteur] = temp4[i];
                compteur++;
            }
            #endregion
            #region Remplir l'en-tête du bitmap
            //Taille en octets de cet en-tête
            temp4 = Convertir_Int_ToEndian(40, 4);
            for (int i = 0; i < 4; i++)
            {
                octetsSortie[compteur] = temp4[i];
                compteur++;
            }
            //Largeur de l'image en pixels
            temp4 = Convertir_Int_ToEndian(largeur, 4);
            for (int i = 0; i < 4; i++)
            {
                octetsSortie[compteur] = temp4[i];
                compteur++;
            }
            //Hauteur de l'image en pixels
            temp4 = Convertir_Int_ToEndian(hauteur, 4);
            for (int i = 0; i < 4; i++)
            {
                octetsSortie[compteur] = temp4[i];
                compteur++;
            }
            //Nombre de plan
            byte[] temp2 = Convertir_Int_ToEndian(1, 2);
            for (int i = 0; i < 2; i++)
            {
                octetsSortie[compteur] = temp2[i];
                compteur++;
            }
            //Nombre de bits par pixel
            temp2 = Convertir_Int_ToEndian(8 * 3, 2);
            for (int i = 0; i < 2; i++)
            {
                octetsSortie[compteur] = temp2[i];
                compteur++;
            }
            //Compression
            temp4 = Convertir_Int_ToEndian(0, 4);
            for (int i = 0; i < 4; i++)
            {
                octetsSortie[compteur] = temp4[i];
                compteur++;
            }
            //Taille en octets des données de l'image
            temp4 = Convertir_Int_ToEndian(octetsSortie.Length - 54, 4);
            for (int i = 0; i < 4; i++)
            {
                octetsSortie[compteur] = temp4[i];
                compteur++;
            }
            //Résolution horizontale en pixels par mètre
            temp4 = Convertir_Int_ToEndian(11811, 4);
            for (int i = 0; i < 4; i++)
            {
                octetsSortie[compteur] = temp4[i];
                compteur++;
            }
            //Résolution verticale en pixels par mètre
            temp4 = Convertir_Int_ToEndian(11811, 4);
            for (int i = 0; i < 4; i++)
            {
                octetsSortie[compteur] = temp4[i];
                compteur++;
            }
            //Nombre de couleurs dans l'image
            temp4 = Convertir_Int_ToEndian(0, 4);
            for (int i = 0; i < 4; i++)
            {
                octetsSortie[compteur] = temp4[i];
                compteur++;
            }
            //Nombre de couleurs importantes
            temp4 = Convertir_Int_ToEndian(0, 4);
            for (int i = 0; i < 4; i++)
            {
                octetsSortie[compteur] = temp4[i];
                compteur++;
            }
            #endregion
            #region Construire le tableau d'octets avec les données de la matrice de pixels
            compteur = 54;
            for (int i = 0; i < hauteur; i++)
            {
                for (int j = 0; j < largeur + nombreDeZero; j++)
                {
                    if (j < matPixelSortie.GetLength(1))
                    {
                        octetsSortie[compteur] = matPixelSortie[i, j].B;
                        octetsSortie[compteur + 1] = matPixelSortie[i, j].V;
                        octetsSortie[compteur + 2] = matPixelSortie[i, j].R;
                        compteur = compteur + 3;
                    }
                    else
                    {
                        octetsSortie[compteur] = Convert.ToByte(0);
                        compteur++;
                    }
                }
            }
            #endregion
        }

        /// <summary>
        /// Enregistre sur l'ordinateur la chaîne d'octets qui permet de visionner une image du type bitmap
        /// </summary>
        public void From_QrCode_To_File()
        {
            From_Matrice_To_Tableau();
            //Sauvegarder le fichier de sortie
            File.WriteAllBytes(myFileReturn, octetsSortie);

            Process.Start(myFileReturn);
        }

        /// <summary>
        /// Convertit un caractère char en caractère écrit en AlphaNumérique
        /// </summary>
        /// <param name="a"></param> caractère à convertir
        /// <returns></returns> valeur alphanumérique correspondante
        public static int CharToAlpha(char a)
        {
            int value = 0;
            switch (a)
            {
                #region NOMBRE
                case '0':
                    value = 0;
                    break;
                case '1':
                    value = 1;
                    break;
                case '2':
                    value = 2;
                    break;
                case '3':
                    value = 3;
                    break;
                case '4':
                    value = 4;
                    break;
                case '5':
                    value = 5;
                    break;
                case '6':
                    value = 6;
                    break;
                case '7':
                    value = 7;
                    break;
                case '8':
                    value = 8;
                    break;
                case '9':
                    value = 9;
                    break;
                #endregion
                #region LETTRE
                case 'A':
                    value = 10;
                    break;
                case 'B':
                    value = 11;
                    break;
                case 'C':
                    value = 12;
                    break;
                case 'D':
                    value = 13;
                    break;
                case 'E':
                    value = 14;
                    break;
                case 'F':
                    value = 15;
                    break;
                case 'G':
                    value = 16;
                    break;
                case 'H':
                    value = 17;
                    break;
                case 'I':
                    value = 18;
                    break;
                case 'J':
                    value = 19;
                    break;
                case 'K':
                    value = 20;
                    break;
                case 'L':
                    value = 21;
                    break;
                case 'M':
                    value = 22;
                    break;
                case 'N':
                    value = 23;
                    break;
                case 'O':
                    value = 24;
                    break;
                case 'P':
                    value = 25;
                    break;
                case 'Q':
                    value = 26;
                    break;
                case 'R':
                    value = 27;
                    break;
                case 'S':
                    value = 28;
                    break;
                case 'T':
                    value = 29;
                    break;
                case 'U':
                    value = 30;
                    break;
                case 'V':
                    value = 31;
                    break;
                case 'W':
                    value = 32;
                    break;
                case 'X':
                    value = 33;
                    break;
                case 'Y':
                    value = 34;
                    break;
                case 'Z':
                    value = 35;
                    break;
                #endregion
                #region CARACTERES SPECIAUX
                case ' ':
                    value = 36;
                    break;
                case '$':
                    value = 37;
                    break;
                case '%':
                    value = 38;
                    break;
                case '*':
                    value = 39;
                    break;
                case '+':
                    value = 40;
                    break;
                case '-':
                    value = 41;
                    break;
                case '.':
                    value = 42;
                    break;
                case '/':
                    value = 43;
                    break;
                case ':':
                    value = 44;
                    break;


                #endregion
                default:
                    value = -1;
                    break;
            }
            return value;
        }

        /// <summary>
        /// Convertit une valeur en alphanumérique en une chaine de caractère du type string
        /// </summary>
        /// <param name="value"></param> valeur en alphanumérique
        /// <returns></returns> une chaîne de caractères string
        public static string AlphaToString(int value)
        {
            string a = "";
            switch (value)
            {
                #region NOMBRE
                case 0:
                    a = "0";
                    break;
                case 1:
                    a = "1";
                    break;
                case 2:
                    a = "2";
                    break;
                case 3:
                    a = "3";
                    break;
                case 4:
                    a = "4";
                    break;
                case 5:
                    a = "5";
                    break;
                case 6:
                    a = "6";
                    break;
                case 7:
                    a = "7";
                    break;
                case 8:
                    a = "8";
                    break;
                case 9:
                    a = "9";
                    break;

                #endregion

                #region LETTRE
                case 10:
                    a = "A";
                    break;
                case 11:
                    a = "B";
                    break;
                case 12:
                    a = "C";
                    break;
                case 13:
                    a = "D";
                    break;
                case 14:
                    a = "E";
                    break;
                case 15:
                    a = "F";
                    break;
                case 16:
                    a = "G";
                    break;
                case 17:
                    a = "H";
                    break;
                case 18:
                    a = "I";
                    break;
                case 19:
                    a = "J";
                    break;
                case 20:
                    a = "K";
                    break;
                case 21:
                    a = "L";
                    break;
                case 22:
                    a = "M";
                    break;
                case 23:
                    a = "N";
                    break;
                case 24:
                    a = "O";
                    break;
                case 25:
                    a = "P";
                    break;
                case 26:
                    a = "Q";
                    break;
                case 27:
                    a = "R";
                    break;
                case 28:
                    a = "S";
                    break;
                case 29:
                    a = "T";
                    break;
                case 30:
                    a = "U";
                    break;
                case 31:
                    a = "V";
                    break;
                case 32:
                    a = "W";
                    break;
                case 33:
                    a = "X";
                    break;
                case 34:
                    a = "Y";
                    break;
                case 35:
                    a = "Z";
                    break;
                #endregion

                #region CARACTERES SPECIAUX
                case 36:
                    a = " ";
                    break;
                case 37:
                    a = "$";
                    break;
                case 38:
                    a = "%";
                    break;
                case 39:
                    a = "*";
                    break;
                case 40:
                    a = "+";
                    break;
                case 41:
                    a = "-";
                    break;
                case 42:
                    a = ".";
                    break;
                case 43:
                    a = "/";
                    break;
                case 44:
                    a = ":";
                    break;

                #endregion

                default:
                    a = "FAUX";
                    break;
            }
            return a;
        }

        /// <summary>
        /// Convertit une valeur du type int en chaine de bianire
        /// </summary>
        /// <param name="nombre"></param> valeur à convertir
        /// <param name="taille"></param> taille du tableau de binaire (en général 8, pour 8 bits)
        /// <returns></returns> tableau de binaires
        public static int[] IntToBinary(int nombre, int taille)
        {
            int[] tab = new int[taille];

            //Verification de la taille 
            int nombreMax = Convert.ToInt32(Math.Pow(2, taille)) - 1;
            if (nombre > nombreMax)
            {
                tab = null;
            }
            else
            {
                for (int i = 0; i < tab.Length; i++)
                {
                    tab[i] = 0;
                }
                for (int i = 0; i < tab.Length; i++)
                {
                    int facteur = Convert.ToInt32(Math.Pow(2, taille - 1 - i));
                    if (nombre / facteur >= 1)
                    {
                        tab[i] = 1;
                        nombre -= facteur;
                    }
                }
            }

            return tab;

        }

        /// <summary>
        /// Convertit un tableau de bianires en une valeur int
        /// </summary>
        /// <param name="tab"></param> tableau de binaire à convertir
        /// <returns></returns> valeur de type int
        public static int BinaireToInt(int[] tab)
        {
            int nb = 0;
            int taille = tab.Length;
            for (int i = 0; i < taille; i++)
            {
                nb += Convert.ToInt32(tab[i] * Math.Pow(2, taille - 1 - i));
            }
            return nb;
        }

        /// <summary>
        /// Créé un tableau regroupant les tableaux, le deuxième à la suite du premier
        /// </summary>
        /// <param name="tab1"></param> Premier tableau
        /// <param name="tab2"></param> Deuxième tableau
        /// <returns></returns> Tableau qui a pour taille la somme de chacun des tableaux
        public static int[] AddTableau(int[] tab1, int[] tab2)
        {
            if (tab1 == null)
            {
                return tab2;
            }
            if (tab2 == null)
            {
                return tab1;
            }
            int taille = tab1.Length + tab2.Length;
            int[] tabTot = new int[taille];

            for (int i = 0; i < tab1.Length; i++)
            {
                tabTot[i] = tab1[i];
            }
            for (int i = 0; i < tab2.Length; i++)
            {
                tabTot[i + tab1.Length] = tab2[i];
            }

            return tabTot;
        }
        #endregion

        #region CREATION D'UN QR
        /// <summary>
        /// Créé un squelette de Qr Code qui peut être une version 1 (21*21) ou une version 2 (25*25), chaque pixel visuel est composé de 4 pixels
        /// </summary>
        /// <param name="version"></param> Version : 1 ou 2
        public void Creation(int version)
        {
            Pixel noir = new Pixel(0, 0, 0);
            Pixel blanc = new Pixel(255, 255, 255);
            Pixel gris = new Pixel(120, 120, 120);
            Pixel bleu = new Pixel(0, 0, 200);

            matPixelSortie = new Pixel[hauteur, largeur];

            //Remplir la matrice de pixel gris (ce seront les pixels à modifier);
            for (int i = 0; i < hauteur; i++)
            {
                for (int j = 0; j < largeur; j++)
                {
                    matPixelSortie[i, j] = gris;
                }
            }

            #region MOTIFS DE RECHERCHES
            for (int i = 0; i < 32; i++)
            {
                for (int j = 0; j < 32; j++)
                {
                    matPixelSortie[i, j] = blanc;
                }
            } //Premier carré en haut à gauche
            for (int i = 0; i < 28; i++)
            {
                for (int j = 0; j < 28; j++)
                {
                    matPixelSortie[i, j] = noir;
                }
            }
            for (int i = 4; i < 24; i++)
            {
                for (int j = 4; j < 24; j++)
                {
                    matPixelSortie[i, j] = blanc;
                }
            }
            for (int i = 8; i < 20; i++)
            {
                for (int j = 8; j < 20; j++)
                {
                    matPixelSortie[i, j] = noir;
                }
            }

            for (int i = hauteur - 1; i > hauteur - 1 - 32; i--)
            {
                for (int j = largeur - 1; j > largeur - 1 - 32; j--)
                {
                    matPixelSortie[i, j] = blanc;
                }
            } //Deuxième en bas à droite
            for (int i = hauteur - 1; i > hauteur - 1 - 28; i--)
            {
                for (int j = largeur - 1; j > largeur - 1 - 28; j--)
                {
                    matPixelSortie[i, j] = noir;
                }
            }
            for (int i = hauteur - 1 - 4; i > hauteur - 1 - 24; i--)
            {
                for (int j = largeur - 1 - 4; j > largeur - 1 - 24; j--)
                {
                    matPixelSortie[i, j] = blanc;
                }
            }
            for (int i = hauteur - 1 - 8; i > hauteur - 1 - 20; i--)
            {
                for (int j = largeur - 1 - 8; j > largeur - 1 - 20; j--)
                {
                    matPixelSortie[i, j] = noir;
                }
            }

            for (int i = hauteur - 1; i > hauteur - 1 - 32; i--)
            {
                for (int j = 0; j < 32; j++)
                {
                    matPixelSortie[i, j] = blanc;
                }
            } //Troisième en bas à gauche
            for (int i = hauteur - 1; i > hauteur - 1 - 28; i--)
            {
                for (int j = 0; j < 28; j++)
                {
                    matPixelSortie[i, j] = noir;
                }
            }
            for (int i = hauteur - 1 - 4; i > hauteur - 1 - 24; i--)
            {
                for (int j = 4; j < 24; j++)
                {
                    matPixelSortie[i, j] = blanc;
                }
            }
            for (int i = hauteur - 1 - 8; i > hauteur - 1 - 20; i--)
            {
                for (int j = 8; j < 20; j++)
                {
                    matPixelSortie[i, j] = noir;
                }
            }
            #endregion

            #region MOTIFS DE SYNCHRONISATION
            int compteur = 32;
            Pixel aPlacer = noir;
            while (matPixelSortie[compteur, 24] == gris) //Ligne de pixels noirs et blancs verticale, il faut se rappeler qu'un pixel à l'image vaut un carré de 4 pixels en vrai
            {
                for (int i = 0; i < 4; i++)
                {
                    for (int j = 0; j < 4; j++)
                    {
                        matPixelSortie[compteur + i, 24 + j] = aPlacer;
                    }
                }

                if (aPlacer == noir)
                {
                    aPlacer = blanc;
                }
                else
                {
                    aPlacer = noir;
                }
                compteur += 4;
            }

            compteur = 32;
            aPlacer = noir;
            while (matPixelSortie[hauteur - 28, compteur] == gris) //Ligne de pixels noirs et blancs horizontale
            {
                for (int i = 0; i < 4; i++)
                {
                    for (int j = 0; j < 4; j++)
                    {
                        matPixelSortie[hauteur - 28 + i, compteur + j] = aPlacer;
                    }
                }
                if (aPlacer == noir)
                {
                    aPlacer = blanc;
                }
                else
                {
                    aPlacer = noir;
                }
                compteur += 4;
            }
            #endregion

            #region MOTIF D'ALLIGNEMENT

            if (version == 2)
            {
                for (int i = 16; i < 36; i++)
                {
                    for (int j = largeur - 16 - 1; j > largeur - 1 - 36; j--)
                    {
                        matPixelSortie[i, j] = noir;
                    }
                }
                for (int i = 20; i < 32; i++)
                {
                    for (int j = largeur - 20 - 1; j > largeur - 1 - 32; j--)
                    {
                        matPixelSortie[i, j] = blanc;
                    }
                }
                for (int i = 24; i < 28; i++)
                {
                    for (int j = largeur - 24 - 1; j > largeur - 1 - 28; j--)
                    {
                        matPixelSortie[i, j] = noir;
                    }
                }
            }

            #endregion

            #region MOTIF SOMBRE

            for (int i = 28; i < 32; i++)
            {
                for (int j = 32; j < 36; j++)
                {
                    matPixelSortie[i, j] = noir;
                }
            }


            #endregion

            #region ZONE RESERVE AU MASQUE

            for (int i = 0; i < 28; i++)
            {
                for (int j = 32; j < 36; j++)
                {
                    matPixelSortie[i, j] = bleu;
                }
            }
            for (int i = largeur - 1 - 32; i > largeur - 1 - 36; i--)
            {
                for (int j = hauteur - 1; j > hauteur - 1 - 32; j--)
                {
                    matPixelSortie[i, j] = bleu;
                }
            }
            for (int i = hauteur - 1; i > hauteur - 1 - 36; i--)
            {
                for (int j = 0; j < 36; j++)
                {
                    if (matPixelSortie[i, j] == gris)
                    {
                        matPixelSortie[i, j] = bleu;
                    }
                }
            }

            #endregion

        }

        #endregion

        #region REMPLIR LE QR

        /// <summary>
        /// Convertit une chaine de caractères du type string en un tableau de int
        /// </summary>
        /// <param name="message"></param> Chaine de texte à convertir
        /// <returns></returns> Tableau de valeurs des poids des lettres de la chaîne
        static public int[] DonneeString(string message)
        {
            char[] tabChar = message.ToCharArray();

            //Création d'une liste avec le poid des deux lettres
            List<int> donnee = new List<int>();
            for (int i = 0; i < tabChar.Length; i += 2)
            {
                int valeur = 0;
                if (i == tabChar.Length - 1)
                {
                    valeur = CharToAlpha(tabChar[i]);
                }
                else
                {
                    valeur = CharToAlpha(tabChar[i]) * 45 + CharToAlpha(tabChar[i + 1]);
                }
                donnee.Add(valeur);
            }

            //Conversion de la liste en tableau
            int[] donneeMessage = new int[donnee.Count()];
            for (int i = 0; i < donneeMessage.Length; i++)
            {
                donneeMessage[i] = donnee[i];
            }

            return donneeMessage;
        }

        /// <summary>
        /// Convertit une chaine de caractères du type string en un tableau de binaires
        /// </summary>
        /// <param name="message"></param> Chaine de texte à convertir
        /// <returns></returns> Tableau de binaires
        static public int[] DonneeBinaire(string message)
        {
            int[] donneeString = DonneeString(message);

            //Pour la première valeur
            int nbBits = 0;
            if (donneeString[0] < 64)
            {
                nbBits = 6;
            }
            else
            {
                nbBits = 11;
            }
            int[] tab1 = IntToBinary(donneeString[0], nbBits);

            for (int i = 1; i < donneeString.Length; i++)
            {
                nbBits = 0;
                if (donneeString[i] < 64)
                {
                    nbBits = 6;
                }
                else
                {
                    nbBits = 11;
                }

                int[] tab2 = IntToBinary(donneeString[i], nbBits);
                tab1 = AddTableau(tab1, tab2);

            }
            return tab1;
        }

        /// <summary>
        /// Adapte la chaine de string entrée en un tableau de binaires adapté à la version demandée
        /// </summary>
        /// <param name="message"></param> chaine de texte à convertir
        /// <param name="version"></param> Version 1 ou 2
        /// <returns></returns>
        static public int[] DonneeTotale(string message, int version)
        {
            int[] indicateurDuMode = { 0, 0, 1, 0 };

            int nbCaractere = message.Length;
            int[] nbCar = IntToBinary(nbCaractere, 9);

            int[] doneeMessage = DonneeBinaire(message);

            int nbBits = indicateurDuMode.Length + nbCar.Length + doneeMessage.Length;
            if (nbBits % 8 != 0)
            {
                int ajout = 8 - nbBits % 8;
                int[] terminaison = new int[ajout];
                for (int i = 0; i < ajout; i++)
                {
                    terminaison[i] = 0;
                }
                doneeMessage = AddTableau(doneeMessage, terminaison);
            }

            int nbBitsTotal = 0;
            if (version == 1)
            {
                nbBitsTotal = 152;
            }
            else
            {
                nbBitsTotal = 304;
            }

            int nbOctetSup = (nbBitsTotal - indicateurDuMode.Length - nbCar.Length - doneeMessage.Length) / 8;
            int[] octet1 = IntToBinary(236, 8);
            int[] octet2 = IntToBinary(17, 8);

            int[] tabAjout = octet1;
            int[] temp = octet1;
            for (int i = 0; i < nbOctetSup; i++)
            {
                doneeMessage = AddTableau(doneeMessage, tabAjout);
                if (tabAjout == octet1)
                {
                    temp = octet2;
                }
                if (tabAjout == octet2)
                {
                    temp = octet1;
                }
                tabAjout = temp;
            }
            int[] donneeTotale = AddTableau(indicateurDuMode, nbCar);
            donneeTotale = AddTableau(donneeTotale, doneeMessage);

            return donneeTotale;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="donneeTot"></param>
        /// <param name="a"></param>
        /// <returns></returns>
        static public int[] ReedSalomon(int[] donneeTot, string a)
        {
            byte[] doneeOctets = new byte[donneeTot.Length / 8];
            int compteur = 0;
            for (int i = 0; i < donneeTot.Length; i += 8)
            {
                int[] octet = new int[8];
                for (int j = 0; j < 8; j++)
                {
                    octet[j] = donneeTot[i + j];
                }
                doneeOctets[compteur] = Convert.ToByte(BinaireToInt(octet));
                compteur++;
            }

            Encoding u8 = Encoding.UTF8;
            int iBC = u8.GetByteCount(a);
            byte[] bytesa = doneeOctets;
            byte[] result = ReedSolomonAlgorithm.Encode(bytesa, 7, ErrorCorrectionCodeType.QRCode);

            int[] test = new int[result.Length];
            for (int i = 0; i < result.Length; i++)
            {
                test[i] = Convert.ToInt32(result[i]);
            }
            int[] erreurOctet = IntToBinary(Convert.ToInt32(result[0]), 8);
            for (int i = 1; i < result.Length; i++)
            {
                int[] tab2 = IntToBinary(result[i], 8);
                erreurOctet = AddTableau(erreurOctet, tab2);
            }
            //Program.AfficherTableau(test);
            return erreurOctet;
        }

        /// <summary>
        /// Modifie la matrice de pixels de sortie pour y insérer le message selon les codifications
        /// </summary>
        /// <param name="version"></param> Version 1 ou 2
        /// <param name="message"></param> Chaine de caractère à inscrire dans le QrCode
        public void AppliquerDonnee(int version, string message)
        {
            int[] donneeMessage = DonneeTotale(message, version);
            int[] erreur = ReedSalomon(donneeMessage, message);

            int[] tot = AddTableau(donneeMessage, erreur);

            Pixel noir = new Pixel(0, 0, 0);
            Pixel blanc = new Pixel(255, 255, 255);
            Pixel gris = new Pixel(120, 120, 120);
            Pixel bleu = new Pixel(0, 0, 200);

            int compteur = 0;
            int direction = 0; // 0 = vers le haut et 1 = vers le bas
            int ligne = matPixelSortie.GetLength(1) - 1;
            int test = 1;

            //Rentrer les donéées du message
            while (compteur < tot.Length && ligne >= 3)
            {
                if (direction == 0)
                {
                    if (ligne == 3 && version == 1)
                    {
                        for (int i = 32; i < 53; i += 4)
                        {
                            for (int k = 0; k < 4; k++)
                            {
                                for (int l = 0; l < 4; l++)
                                {
                                    if ((0 + i / 4) % 2 == 0)
                                    {
                                        if (tot[compteur] == 0)
                                        {
                                            matPixelSortie[i + k, l] = noir;
                                        }
                                        if (tot[compteur] == 1)
                                        {
                                            matPixelSortie[i + k, l] = blanc;
                                        }
                                    }
                                    else
                                    {
                                        if (tot[compteur] == 0)
                                        {
                                            matPixelSortie[i + k, l] = blanc;
                                        }
                                        if (tot[compteur] == 1)
                                        {
                                            matPixelSortie[i + k, l] = noir;
                                        }
                                    }
                                }
                            }
                            compteur++;
                            if (compteur == tot.Length)
                            {
                                break;
                            }
                        }
                        break;
                    }
                    if (ligne == 3 && version == 2)
                    {
                        for (int i = 32; i < 69; i += 4)
                        {
                            for (int k = 0; k < 4; k++)
                            {
                                for (int l = 0; l < 4; l++)
                                {
                                    if ((0 + i / 4) % 2 == 0)
                                    {
                                        if (tot[compteur] == 0)
                                        {
                                            matPixelSortie[i + k, l] = noir;
                                        }
                                        if (tot[compteur] == 1)
                                        {
                                            matPixelSortie[i + k, l] = blanc;
                                        }
                                    }
                                    else
                                    {
                                        if (tot[compteur] == 0)
                                        {
                                            matPixelSortie[i + k, l] = blanc;
                                        }
                                        if (tot[compteur] == 1)
                                        {
                                            matPixelSortie[i + k, l] = noir;
                                        }
                                    }

                                }
                            }
                            compteur++;
                            if (compteur == tot.Length)
                            {
                                break;
                            }
                        }
                        break;
                    }
                    else
                    {
                        for (int i = 0; i < matPixelSortie.GetLength(0); i += 4)
                        {
                            for (int j = ligne; j > ligne - 5; j -= 4)
                            {
                                if (matPixelSortie[i, j].R == 120 && matPixelSortie[i, j].V == 120 && matPixelSortie[i, j].B == 120)
                                {
                                    for (int k = 0; k < 4; k++)
                                    {
                                        for (int l = 0; l < 4; l++)
                                        {
                                            if (((i / 4) + (j / 4)) % 2 == 0)
                                            {
                                                if (tot[compteur] == 0)
                                                {
                                                    matPixelSortie[i + k, j - l] = noir;
                                                }
                                                if (tot[compteur] == 1)
                                                {
                                                    matPixelSortie[i + k, j - l] = blanc;
                                                }
                                            }
                                            else
                                            {
                                                if (tot[compteur] == 0)
                                                {
                                                    matPixelSortie[i + k, j - l] = blanc;
                                                }
                                                if (tot[compteur] == 1)
                                                {
                                                    matPixelSortie[i + k, j - l] = noir;
                                                }
                                            }
                                        }
                                    }
                                    compteur++;
                                    if (compteur == tot.Length)
                                    {
                                        break;
                                    }
                                }
                            }
                            if (compteur == tot.Length)
                            {
                                break;
                            }
                        }

                        ligne -= 8;
                        direction = 1;
                        test++;
                    }

                }

                if (direction == 1)
                {
                    for (int i = matPixelSortie.GetLength(0) - 4; i >= 0; i -= 4)
                    {
                        for (int j = ligne; j > ligne - 5; j -= 4)
                        {
                            if (matPixelSortie[i, j].R == 120 && matPixelSortie[i, j].V == 120 && matPixelSortie[i, j].B == 120)
                            {
                                for (int k = 0; k < 4; k++)
                                {
                                    for (int l = 0; l < 4; l++)
                                    {
                                        if (((i / 4) + (j / 4)) % 2 == 0)
                                        {
                                            if (tot[compteur] == 0)
                                            {
                                                matPixelSortie[i + k, j - l] = noir;
                                            }
                                            if (tot[compteur] == 1)
                                            {
                                                matPixelSortie[i + k, j - l] = blanc;
                                            }
                                        }
                                        else
                                        {
                                            if (tot[compteur] == 0)
                                            {
                                                matPixelSortie[i + k, j - l] = blanc;
                                            }
                                            if (tot[compteur] == 1)
                                            {
                                                matPixelSortie[i + k, j - l] = noir;
                                            }
                                        }
                                    }
                                }
                                compteur++;
                                if (compteur == tot.Length)
                                {
                                    break;
                                }
                            }
                        }
                        if (compteur == tot.Length)
                        {
                            break;
                        }
                    }

                    ligne -= 8;
                    direction = 0;
                    if (compteur == tot.Length)
                    {
                        break;
                    }
                }
            }

            //Rentrer le masque 
            int[] masque = { 1, 1, 1, 0, 1, 1, 1, 1, 1, 0, 0, 0, 1, 0, 0 };

            //Masque 1
            compteur = 0;
            for (int i = 0; i < 28; i += 4)
            {
                int j = 32;
                for (int k = 0; k < 4; k++)
                {
                    for (int l = 0; l < 4; l++)
                    {
                        if (masque[compteur] == 0)
                        {
                            matPixelSortie[i + k, j + l] = blanc;
                        }
                        if (masque[compteur] == 1)
                        {
                            matPixelSortie[i + k, j + l] = noir;
                        }
                    }
                }
                compteur++;
            }
            for (int j = matPixelSortie.GetLength(1) - 32; j < matPixelSortie.GetLength(1); j += 4)
            {
                int i = matPixelSortie.GetLength(0) - 36;
                for (int k = 0; k < 4; k++)
                {
                    for (int l = 0; l < 4; l++)
                    {
                        if (masque[compteur] == 0)
                        {
                            matPixelSortie[i + k, j + l] = blanc;
                        }
                        if (masque[compteur] == 1)
                        {
                            matPixelSortie[i + k, j + l] = noir;
                        }
                    }
                }
                compteur++;
            }
            //Masque2
            compteur = 0;
            for (int j = 0; j <= 28; j += 4)
            {
                int i = matPixelSortie.GetLength(0) - 36;
                if (matPixelSortie[i, j].R == 0 && matPixelSortie[i, j].V == 0 && matPixelSortie[i, j].B == 200)
                {
                    for (int k = 0; k < 4; k++)
                    {
                        for (int l = 0; l < 4; l++)
                        {
                            if (masque[compteur] == 0)
                            {
                                matPixelSortie[i + k, j + l] = blanc;
                            }
                            if (masque[compteur] == 1)
                            {
                                matPixelSortie[i + k, j + l] = noir;
                            }
                        }
                    }
                    compteur++;
                }
            }
            for (int i = matPixelSortie.GetLength(0) - 36; i < matPixelSortie.GetLength(0); i++)
            {
                int j = 32;
                if (matPixelSortie[i, j].R == 0 && matPixelSortie[i, j].V == 0 && matPixelSortie[i, j].B == 200)
                {
                    for (int k = 0; k < 4; k++)
                    {
                        for (int l = 0; l < 4; l++)
                        {
                            if (masque[compteur] == 0)
                            {
                                matPixelSortie[i + k, j + l] = blanc;
                            }
                            if (masque[compteur] == 1)
                            {
                                matPixelSortie[i + k, j + l] = noir;
                            }
                        }
                    }
                    compteur++;
                }

            }


        }

        #endregion

        #region LECTURE D'UN QR
        /// <summary>
        /// Tire du Qr Code le message en binaire 
        /// </summary>
        /// <returns></returns> Tableau du type int contenant le message en binaire 
        public int[] DonneeDuQR()
        {
            Pixel noir = new Pixel(0, 0, 0);
            Pixel blanc = new Pixel(255, 255, 255);
            Pixel gris = new Pixel(120, 120, 120);
            Pixel bleu = new Pixel(0, 0, 200);

            int version = 0;
            if (hauteur == 84)
            {
                version = 1;
            }
            if (hauteur == 100)
            {
                version = 2;
            }

            // Je n'ai pas trouvé d'autres moyen pour contourner les motifs de recherches que des les colorées en bleu
            // Ainsi le code sera contraint de lire les pixels non colorés

            #region Cacher les motifs à contourner
            for (int i = 0; i < 32; i++)
            {
                for (int j = 0; j < 36; j++)
                {
                    matPixelSortie[i, j] = bleu;
                }
            }
            for (int i = hauteur - 1; i > hauteur - 1 - 36; i--)
            {
                for (int j = largeur - 1; j > largeur - 1 - 32; j--)
                {
                    matPixelSortie[i, j] = bleu;
                }
            }
            for (int i = hauteur - 1; i > hauteur - 1 - 36; i--)
            {
                for (int j = 0; j < 36; j++)
                {
                    matPixelSortie[i, j] = bleu;
                }
            }

            for (int i = 0; i < hauteur; i += 4)
            {
                for (int l = 0; l < 4; l++)
                {
                    for (int j = 0; j < 4; j++)
                    {
                        matPixelSortie[i + l, 24 + j] = bleu;
                    }
                }
            }
            for (int j = 32; j < largeur; j += 4)
            {
                for (int i = 0; i < 4; i++)
                {
                    for (int k = 0; k < 4; k++)
                    {
                        matPixelSortie[hauteur - 28 + i, j + k] = bleu;
                    }
                }
            }

            if (version == 2)
            {
                for (int i = 16; i < 36; i++)
                {
                    for (int j = largeur - 16 - 1; j > largeur - 1 - 36; j--)
                    {
                        matPixelSortie[i, j] = bleu;
                    }
                }
            }

            for (int i = 28; i < 32; i++)
            {
                for (int j = 32; j < 36; j++)
                {
                    matPixelSortie[i, j] = bleu;
                }
            }

            #endregion

            #region DonneeDuQR

            int[] donneeDuQr = null;
            if (version == 1)
            {
                donneeDuQr = new int[208];
            }
            if (version == 2)
            {
                donneeDuQr = new int[360];
            }

            int direction = 0; // 0 = vers le haut et 1 = vers le bas
            int ligne = matPixelSortie.GetLength(1) - 1;
            int compteur = 0;
            while (ligne >= 3 && compteur < donneeDuQr.Length)
            {
                if (direction == 0)
                {
                    if (ligne == 3 && version == 1)
                    {
                        for (int k = 0; k < matPixelSortie.GetLength(0); k += 4)
                        {
                            if (k / 4 % 2 == 0)
                            {
                                if (matPixelSortie[k, ligne].R == 255 && matPixelSortie[k, ligne].V == 255 && matPixelSortie[k, ligne].B == 255)
                                {
                                    donneeDuQr[compteur] = 0;
                                    compteur++;
                                }
                                if (matPixelSortie[k, ligne].R == 0 && matPixelSortie[k, ligne].V == 0 && matPixelSortie[k, ligne].B == 0)
                                {
                                    donneeDuQr[compteur] = 1;
                                    compteur++;
                                }
                            }
                            else
                            {
                                if (matPixelSortie[k, ligne].R == 255 && matPixelSortie[k, ligne].V == 255 && matPixelSortie[k, ligne].B == 255)
                                {
                                    donneeDuQr[compteur] = 0;
                                    compteur++;
                                }
                                if (matPixelSortie[k, ligne].R == 0 && matPixelSortie[k, ligne].V == 0 && matPixelSortie[k, ligne].B == 0)
                                {
                                    donneeDuQr[compteur] = 1;
                                    compteur++;
                                }
                            }
                        }
                    }
                    if (ligne == 3 && version == 2)
                    {
                        break;
                    }
                    if (ligne != 3)
                    {
                        for (int k = 0; k < matPixelSortie.GetLength(0); k += 4)
                        {
                            if (((ligne / 4) + (k / 4)) % 2 == 0)
                            {
                                if (matPixelSortie[k, ligne].R == 255 && matPixelSortie[k, ligne].V == 255 && matPixelSortie[k, ligne].B == 255)
                                {
                                    donneeDuQr[compteur] = 1;
                                    compteur++;
                                }
                                if (matPixelSortie[k, ligne].R == 0 && matPixelSortie[k, ligne].V == 0 && matPixelSortie[k, ligne].B == 0)
                                {
                                    donneeDuQr[compteur] = 0;
                                    compteur++;
                                }
                                if (matPixelSortie[k, ligne - 4].R == 255 && matPixelSortie[k, ligne - 4].V == 255 && matPixelSortie[k, ligne - 4].B == 255)
                                {
                                    donneeDuQr[compteur] = 0;
                                    compteur++;
                                }
                                if (matPixelSortie[k, ligne - 4].R == 0 && matPixelSortie[k, ligne - 4].V == 0 && matPixelSortie[k, ligne - 4].B == 0)
                                {
                                    donneeDuQr[compteur] = 1;
                                    compteur++;
                                }
                            }
                            else
                            {
                                if (matPixelSortie[k, ligne].R == 255 && matPixelSortie[k, ligne].V == 255 && matPixelSortie[k, ligne].B == 255)
                                {
                                    donneeDuQr[compteur] = 0;
                                    compteur++;
                                }
                                if (matPixelSortie[k, ligne].R == 0 && matPixelSortie[k, ligne].V == 0 && matPixelSortie[k, ligne].B == 0)
                                {
                                    donneeDuQr[compteur] = 1;
                                    compteur++;
                                }
                                if (matPixelSortie[k, ligne - 4].R == 255 && matPixelSortie[k, ligne - 4].V == 255 && matPixelSortie[k, ligne - 4].B == 255)
                                {
                                    donneeDuQr[compteur] = 1;
                                    compteur++;
                                }
                                if (matPixelSortie[k, ligne - 4].R == 0 && matPixelSortie[k, ligne - 4].V == 0 && matPixelSortie[k, ligne - 4].B == 0)
                                {
                                    donneeDuQr[compteur] = 0;
                                    compteur++;
                                }
                            }
                        }
                        ligne -= 8;
                        direction = 1;
                    }
                }
                if (direction == 1)
                {
                    for (int k = matPixelSortie.GetLength(0) - 1; k >= 0; k -= 4)
                    {
                        if (((ligne / 4) + (k / 4)) % 2 == 0)
                        {
                            if (matPixelSortie[k, ligne].R == 255 && matPixelSortie[k, ligne].V == 255 && matPixelSortie[k, ligne].B == 255)
                            {
                                donneeDuQr[compteur] = 1;
                                compteur++;
                            }
                            if (matPixelSortie[k, ligne].R == 0 && matPixelSortie[k, ligne].V == 0 && matPixelSortie[k, ligne].B == 0)
                            {
                                donneeDuQr[compteur] = 0;
                                compteur++;
                            }
                            if (matPixelSortie[k, ligne - 4].R == 255 && matPixelSortie[k, ligne - 4].V == 255 && matPixelSortie[k, ligne - 4].B == 255)
                            {
                                donneeDuQr[compteur] = 0;
                                compteur++;
                            }
                            if (matPixelSortie[k, ligne - 4].R == 0 && matPixelSortie[k, ligne - 4].V == 0 && matPixelSortie[k, ligne - 4].B == 0)
                            {
                                donneeDuQr[compteur] = 1;
                                compteur++;
                            }
                        }
                        else
                        {
                            if (matPixelSortie[k, ligne].R == 255 && matPixelSortie[k, ligne].V == 255 && matPixelSortie[k, ligne].B == 255)
                            {
                                donneeDuQr[compteur] = 0;
                                compteur++;
                            }
                            if (matPixelSortie[k, ligne].R == 0 && matPixelSortie[k, ligne].V == 0 && matPixelSortie[k, ligne].B == 0)
                            {
                                donneeDuQr[compteur] = 1;
                                compteur++;
                            }
                            if (matPixelSortie[k, ligne - 4].R == 255 && matPixelSortie[k, ligne - 4].V == 255 && matPixelSortie[k, ligne - 4].B == 255)
                            {
                                donneeDuQr[compteur] = 1;
                                compteur++;
                            }
                            if (matPixelSortie[k, ligne - 4].R == 0 && matPixelSortie[k, ligne - 4].V == 0 && matPixelSortie[k, ligne - 4].B == 0)
                            {
                                donneeDuQr[compteur] = 0;
                                compteur++;
                            }
                        }

                    }
                    ligne -= 8;
                    direction = 0;
                }
            }


            #endregion

            return donneeDuQr;
        }

        /// <summary>
        /// Tire d'un message en bianaire un message en alphanumérique, du type string, selon les codifications du Qr Code
        /// </summary>
        public void AfficherMessage()
        {
            int[] donnee = DonneeDuQR();

            #region Definition

            int[] tabDef = new int[4];
            for (int i = 0; i < 4; i++)
            {
                tabDef[i] = donnee[i];
            }

            #endregion

            #region Nombre De Caractère

            int[] nbCaractere = new int[9];
            for (int i = 4; i < 4 + 9; i++)
            {
                nbCaractere[i - 4] = donnee[i];
            }
            int nbCar = BinaireToInt(nbCaractere);

            #endregion

            #region Message en Binaire

            int tailleTabBinaire = 0;
            int tailleTabNum = 0;
            if (nbCar % 2 == 0)
            {
                tailleTabBinaire = (nbCar / 2) * 11;
                tailleTabNum = nbCar / 2;
            }
            if (nbCar % 2 != 0)
            {
                tailleTabBinaire = (nbCar / 2) * 11 + 6;
                tailleTabNum = (nbCar / 2) + 1;
            }

            int[] messageBinaire = new int[tailleTabBinaire];
            for (int i = 13; i < 13 + tailleTabBinaire; i++)
            {
                messageBinaire[i - 13] = donnee[i];
            }

            #endregion

            #region Message en Num (2 par 2)

            int[] messageNum = new int[tailleTabNum];
            int compteur = 0;
            if (tailleTabBinaire % 11 == 0)
            {
                for (int i = 0; i < tailleTabBinaire; i += 11)
                {
                    int[] tabAConvertir = new int[11];
                    for (int j = i; j < i + 11; j++)
                    {
                        tabAConvertir[j - i] = messageBinaire[j];
                    }
                    messageNum[compteur] = BinaireToInt(tabAConvertir);
                    compteur++;
                }
            }
            if (tailleTabBinaire % 11 != 0)
            {

                for (int j = 0; j < tailleTabBinaire - 6; j += 11)
                {
                    int[] tabAConvertir = new int[11];
                    for (int i = 0; i < 11; i++)
                    {
                        tabAConvertir[i] = messageBinaire[i + j];
                    }
                    messageNum[compteur] = BinaireToInt(tabAConvertir);
                    compteur++;
                }

                int[] tabAConvertir2 = new int[6];
                for (int i = tailleTabBinaire - 6; i < tailleTabBinaire; i++)
                {
                    tabAConvertir2[i - tailleTabBinaire + 6] = messageBinaire[i];
                }
                messageNum[compteur] = BinaireToInt(tabAConvertir2);
                compteur++;
            }

            #endregion

            #region Message en Alphanumerique

            int[] messageAlpha = new int[nbCar];
            compteur = 0;

            if (nbCar % 2 == 0)
            {
                for (int i = 0; i < messageNum.Length; i++)
                {
                    int val1 = 0;
                    int val2 = 0;
                    int nb = messageNum[i];

                    val1 = nb / 45;
                    val2 = nb - 45 * val1;

                    messageAlpha[compteur] = val1;
                    messageAlpha[compteur + 1] = val2;
                    compteur += 2;
                }
            }
            if (nbCar % 2 != 0)
            {
                for (int i = 0; i < messageNum.Length - 1; i++)
                {
                    int val1 = 0;
                    int val2 = 0;
                    int nb = messageNum[i];

                    val1 = nb / 45;
                    val2 = nb - 45 * val1;

                    messageAlpha[compteur] = val1;
                    messageAlpha[compteur + 1] = val2;
                    compteur += 2;
                }
                messageAlpha[messageAlpha.Length - 1] = messageNum[messageNum.Length - 1];


            }

            #endregion

            #region Conversion Du Message

            string message = "";
            for (int i = 0; i < messageAlpha.Length; i++)
            {
                message += AlphaToString(messageAlpha[i]);
            }

            Console.WriteLine("Voici votre message : " + message + "\n");

            #endregion


        }

        #endregion
    }
}
