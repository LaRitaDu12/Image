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
    public class Histogramme
    {
        #region ATRIBUTS
        private string myFileReturn;

        private int tailleFichier;
        private int largeur;
        private int hauteur;
        private int nbBitsParCouleur;
        private int tailleOffset;
        private byte[] octetsSortie;
        private Pixel[,] matPixelSortie;
        #endregion

        #region PROPRIETES
        public string MyFileReturn { get { return this.myFileReturn; } set { this.myFileReturn = value; } }

        public int Hauteur { get { return this.hauteur; } set { this.hauteur = value; } }

        public int Largeur { get { return this.largeur; } set { this.largeur = value; } }
        #endregion

        #region CONSTRUCTEUR
        public Histogramme(string myFileReturn, int hauteur, int largeur)
        {
            this.myFileReturn = myFileReturn;
            this.hauteur = hauteur;
            this.largeur = largeur;
        }
        #endregion

        #region METHODES BASIQUES

        /// <summary>
        /// Convertit une valeur int en un tableau de bytes correspondant
        /// </summary>
        /// <param name="valeur"></param> La valeur à convertir
        /// <param name="taille"></param> La taille du tableau de bytes
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
        /// Construit un tableau d'octets contenant les headers et les informations sur l'image selon les normes du format bitmap
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
        /// Enregistre le tableau d'octets précedents sur l'ordinateur de manière à pouvoir voir l'image correctement, au format bitmap
        /// </summary>
        public void From_Histo_To_File()
        {
            From_Matrice_To_Tableau();
            //Sauvegarder le fichier de sortie
            File.WriteAllBytes(myFileReturn, octetsSortie);

            Process.Start(myFileReturn);
        }

        /// <summary>
        /// Retourne le nombre de pixel correspondant à une moyenne de couleur dans l'image
        /// </summary>
        /// <param name="val"></param> La moyenne de couleur
        /// <param name="matPixel"></param> La matrice de pixels correspondant à l'image
        /// <returns></returns>
        public static int NombrePixel(int val, Pixel[,] matPixel)
        {
            int compteur = 0;
            for (int i = 0; i < matPixel.GetLength(0); i++)
            {
                for (int j = 0; j < matPixel.GetLength(1); j++)
                {
                    int moyenne = Convert.ToInt32(matPixel[i, j].MoyennePixel());
                    if (moyenne == val)
                    {
                        compteur++;
                    }
                }
            }
            return compteur;
        }

        /// <summary>
        /// Calcul du nombre de pixels maximal associé à une seule moyenne de couleur
        /// </summary>
        /// <param name="matPixel"></param> La matrice correspondant à l'image étudiée
        /// <returns></returns> La valeur moyenne de pixel maximale de l'image
        public static int MaxNombrePixel(Pixel[,] matPixel)
        {
            int max = 0;
            for (int i = 0; i <= 255; i++)
            {
                int nombrePixel = NombrePixel(i, matPixel);
                if (nombrePixel > max)
                {
                    max = nombrePixel;
                }
            }
            return max;
        }

        /// <summary>
        /// Calcul le nombre de pixels colorés d'une certaine nuance sur l'image
        /// </summary>
        /// <param name="couleur"></param> La nuance étudiée (rouge, vert, bleu)
        /// <param name="val"></param> La valeur de la nuance étudiée
        /// <param name="matPixel"></param> La matrice de pixels associée à l'image
        /// <returns></returns> Le nombre de pixels correspondant à cette nuance présents sur l'image
        public static int NbPixelCouleur(string couleur, int val, Pixel[,] matPixel)
        {
            int compteur = 0;
            if (couleur == "rouge")
            {
                compteur = 0;
                for (int i = 0; i < matPixel.GetLength(0); i++)
                {
                    for (int j = 0; j < matPixel.GetLength(1); j++)
                    {
                        int valRouge = Convert.ToInt32(matPixel[i, j].R);
                        if (valRouge == val)
                        {
                            compteur++;
                        }
                    }
                }
            }
            if (couleur == "vert")
            {
                compteur = 0;
                for (int i = 0; i < matPixel.GetLength(0); i++)
                {
                    for (int j = 0; j < matPixel.GetLength(1); j++)
                    {
                        int valVert = Convert.ToInt32(matPixel[i, j].V);
                        if (valVert == val)
                        {
                            compteur++;
                        }
                    }
                }
            }
            if (couleur == "bleu")
            {
                compteur = 0;
                for (int i = 0; i < matPixel.GetLength(0); i++)
                {
                    for (int j = 0; j < matPixel.GetLength(1); j++)
                    {
                        int valBleu = Convert.ToInt32(matPixel[i, j].B);
                        if (valBleu == val)
                        {
                            compteur++;
                        }
                    }
                }
            }
            return compteur;
        }

        /// <summary>
        /// Calcul le nombre maximal de pixels d'une même nuance pour une couleur particulière
        /// </summary>
        /// <param name="couleur"></param> La couleur étudiée (rouge, vert, bleu)
        /// <param name="matPixel"></param> La matrice de pixels associée à l'image étudiée
        /// <returns></returns>
        public static int MaxNbPixelCouleur(string couleur, Pixel[,] matPixel)
        {
            int max = 0;
            for (int i = 0; i <= 255; i++)
            {
                int nombrePixel = NbPixelCouleur(couleur, i, matPixel);
                if (nombrePixel > max)
                {
                    max = nombrePixel;
                }
            }
            return max;
        }
        #endregion

        #region METHODES POUR L'HISTOGRAMME
        /// <summary>
        /// Etablit l'histogramme qui rendra compte de la luminosité d'une image
        /// </summary>
        /// <param name="image"></param> L'image de la classe MyImage étudiée
        public void HistoLuminosite(MyImage image)
        {
            int max = MaxNombrePixel(image.MatPixel);

            matPixelSortie = new Pixel[hauteur, largeur];
            for (int i = 0; i < matPixelSortie.GetLength(0); i++)
            {
                for (int j = 0; j < matPixelSortie.GetLength(1); j++)
                {
                    matPixelSortie[i, j] = new Pixel(255, 255, 255);
                }
            }

            //Création de l'histogramme
            for (int j = 0; j < largeur; j++)
            {
                byte lum = Convert.ToByte(j);

                int nbPixel = NombrePixel(j, image.MatPixel);
                int compteur = (nbPixel * 100) / max;

                for (int i = 0; i < compteur; i++)
                {
                    matPixelSortie[i, j] = new Pixel(lum, lum, lum);
                }
            }
        }

        /// <summary>
        /// Etablit l'histogramme qui rendra compte de la présence de rouge sur une image
        /// </summary>
        /// <param name="image"></param> L'image de la classe MyImage étudiée
        public void HistoRouge(MyImage image)
        {
            int max = MaxNbPixelCouleur("rouge", image.MatPixel);

            matPixelSortie = new Pixel[hauteur, largeur];
            for (int i = 0; i < matPixelSortie.GetLength(0); i++)
            {
                for (int j = 0; j < matPixelSortie.GetLength(1); j++)
                {
                    matPixelSortie[i, j] = new Pixel(255, 255, 255);
                }
            }

            //Construction Histogramme
            for (int j = 0; j < largeur; j++)
            {
                byte valRouge = Convert.ToByte(j);
                int nbPixel = NbPixelCouleur("rouge", j, image.MatPixel);
                int compteur = (nbPixel * 100) / max;

                for (int i = 0; i < compteur; i++)
                {
                    matPixelSortie[i, j] = new Pixel(valRouge, 0, 0);
                }
            }

        }

        /// <summary>
        /// Etablit l'histogramme qui rendra compte de la présence de vert sur une image
        /// </summary>
        /// <param name="image"></param> L'image de la classe MyImage étudiée
        public void HistoVert(MyImage image)
        {
            int max = MaxNbPixelCouleur("vert", image.MatPixel);

            matPixelSortie = new Pixel[hauteur, largeur];
            for (int i = 0; i < matPixelSortie.GetLength(0); i++)
            {
                for (int j = 0; j < matPixelSortie.GetLength(1); j++)
                {
                    matPixelSortie[i, j] = new Pixel(255, 255, 255);
                }
            }

            //Construction Histogramme
            for (int j = 0; j < largeur; j++)
            {
                byte valVert = Convert.ToByte(j);
                int nbPixel = NbPixelCouleur("vert", j, image.MatPixel);
                int compteur = (nbPixel * 100) / max;

                for (int i = 0; i < compteur; i++)
                {
                    matPixelSortie[i, j] = new Pixel(0, valVert, 0);
                }
            }
        }

        /// <summary>
        /// Etablit l'histogramme qui rendra compte de la présence de bleu sur une image
        /// </summary>
        /// <param name="image"></param> L'image de la classe MyImage étudiée
        public void HistoBleu(MyImage image)
        {
            int max = MaxNbPixelCouleur("bleu", image.MatPixel);

            matPixelSortie = new Pixel[hauteur, largeur];
            for (int i = 0; i < matPixelSortie.GetLength(0); i++)
            {
                for (int j = 0; j < matPixelSortie.GetLength(1); j++)
                {
                    matPixelSortie[i, j] = new Pixel(255, 255, 255);
                }
            }

            //Construction Histogramme
            for (int j = 0; j < largeur; j++)
            {
                byte valBleu = Convert.ToByte(j);
                int nbPixel = NbPixelCouleur("bleu", j, image.MatPixel);
                int compteur = (nbPixel * 100) / max;

                for (int i = 0; i < compteur; i++)
                {
                    matPixelSortie[i, j] = new Pixel(0, 0, valBleu);
                }
            }
        }
        #endregion
    }
}
