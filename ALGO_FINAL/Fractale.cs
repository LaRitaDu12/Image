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
    class Fractales
    {
        #region ATTRIBUTS
        private string myFileReturn;
        private int zoom;

        private int tailleFichier;
        private int largeur;
        private int hauteur;
        private int nbBitsParCouleur;
        private int tailleOffset;
        private byte[] octetsSortie;
        private Pixel[,] matPixelSortie;
        #endregion

        #region CONSTRUCTEURS
        public Fractales(string myFileReturn, int hauteur, int largeur)
        {
            this.myFileReturn = myFileReturn;
            this.hauteur = hauteur;
            this.largeur = largeur;
        }
        #endregion

        #region PROPRIETES
        public string MyFileReturn { get { return this.myFileReturn; } set { this.myFileReturn = value; } }

        public int Hauteur { get { return this.hauteur; } set { this.hauteur = value; } }

        public int Largeur { get { return this.largeur; } set { this.largeur = value; } }
        #endregion

        #region METHODES FRACTALES
        /// <summary>
        /// Créé une fractale de Mandelbrot à partir d'une hauteur et d'une largeur choisies par l'utilisateur
        /// </summary>
        public void FractaleDeMandelbrot()
        {
            //On prend comme plan de référence une zone comprise entre les abscisses -2.1 et 0.6 et les ordonnées -1.2 et 1.2
            double x1 = -2.1;
            double x2 = 0.6;
            double y1 = -1.2;
            double y2 = 1.2;
            int iterations = 100;
            double zoomX = largeur / (x2 - x1);
            double zoomY = hauteur / (y2 - y1);
            matPixelSortie = new Pixel[hauteur, largeur];
            for (int i = 0; i < largeur; i++)
            {
                for (int j = 0; j < hauteur; j++)
                {
                    matPixelSortie[j, i] = new Pixel(Convert.ToByte(255), Convert.ToByte(255), Convert.ToByte(255));
                    double cR = i / zoomX + x1;
                    double cI = j / zoomY + y1;
                    double zR = 0;
                    double zI = 0;
                    int k = 0;
                    do
                    {
                        double zRT = zR;
                        zR = zR * zR - zI * zI + cR;
                        zI = 2 * zI * zRT + cI;
                        k = k + 1;
                    } while (zR * zR + zI * zI < 4 && k < iterations);
                    if (k == iterations)
                    {
                        matPixelSortie[j, i] = new Pixel(Convert.ToByte(0), Convert.ToByte(0), Convert.ToByte(0));
                    }
                    else
                    {
                        matPixelSortie[j, i] = new Pixel(Convert.ToByte(0), Convert.ToByte(0), Convert.ToByte(k * 255 / iterations));
                    }
                }
            }
        }

        /// <summary>
        /// Créé une fractale de Julia à partir d'une hauteur et d'une largeur choisies par l'utilisateur
        /// </summary>
        public void FractaleDeJulia()
        {
            //On prend comme plan de référence une zone comprise entre les abscisses -2.1 et 0.6 et les ordonnées -1.2 et 1.2
            double x1 = -1;
            double x2 = 1;
            double y1 = -1.2;
            double y2 = 1.2;
            int iterations = 150;
            double zoomX = largeur / (x2 - x1);
            double zoomY = hauteur / (y2 - y1);
            matPixelSortie = new Pixel[hauteur, largeur];
            for (int i = 0; i < largeur; i++)
            {
                for (int j = 0; j < hauteur; j++)
                {
                    matPixelSortie[j, i] = new Pixel(Convert.ToByte(255), Convert.ToByte(255), Convert.ToByte(255));
                    double cR = 0.285;
                    double cI = 0.01;
                    double zR = i / zoomX + x1;
                    double zI = j / zoomY + y1;
                    int k = 0;
                    do
                    {
                        double zRT = zR;
                        zR = zR * zR - zI * zI + cR;
                        zI = 2 * zI * zRT + cI;
                        k = k + 1;
                    } while (zR * zR + zI * zI < 4 && k < iterations);
                    if (k == iterations)
                    {
                        matPixelSortie[j, i] = new Pixel(Convert.ToByte(0), Convert.ToByte(0), Convert.ToByte(0));
                    }
                    else
                    {
                        matPixelSortie[j, i] = new Pixel(Convert.ToByte(0), Convert.ToByte(0), Convert.ToByte(k * 255 / iterations));
                    }
                }
            }
        }
        #endregion

        #region METHODES BASIQUES
        /// <summary>
        /// Méthode déjà décrite dans la classe MyImage, qui convertit une valeur int en tableau de bytes d'une longueur prédéfinie
        /// </summary>
        /// <param name="valeur"></param> valeur à convertir
        /// <param name="taille"></param> taille du tableau en sortie
        /// <returns></returns> tableau de bytes correspondant
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
        /// Sauvegarde la fractale dans un fichier dans bin>debug
        /// </summary>
        public void From_Fractale_To_File()
        {
            From_Matrice_To_Tableau();
            //Sauvegarder le fichier de sortie
            File.WriteAllBytes(myFileReturn, octetsSortie);

            Process.Start(myFileReturn);
        }

        /// <summary>
        /// Créé entièrement un tableau d'octets, headers compris, pour que la fractale créée puisse être enregistrée et visible
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
                        octetsSortie[compteur] = matPixelSortie[i, j].R;
                        octetsSortie[compteur + 1] = matPixelSortie[i, j].V;
                        octetsSortie[compteur + 2] = matPixelSortie[i, j].B;
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
        #endregion 
    }
}
