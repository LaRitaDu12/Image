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
    public class MyImage
    {
        #region ATTRIBUTS 
        private string myFile;

        private string typeImage;
        private int tailleFichier;
        private int tailleDonneesImage;
        private int largeur;
        private int hauteur;
        private int nbBitsParCouleur;
        private int tailleOffset;
        private byte[] octetsArrivee;
        private byte[] octetsSortie;
        private Pixel[,] matPixel; //L'image sous sa forme en 2D
        private Pixel[,] matPixelSortie;
        #endregion

        #region PROPRIETES
        public string MyFile { get { return this.myFile; } set { this.myFile = value; } }

        public Pixel[,] MatPixel { get { return this.matPixel; } }

        public int TailleDonnees { get { return this.tailleDonneesImage; } }

        public Pixel[,] MatPixelSortie { get { return this.matPixelSortie; } }
        #endregion

        #region CONSTRUCTEURS
        public MyImage(string myFile)
        {
            this.myFile = myFile;

            octetsArrivee = File.ReadAllBytes(myFile);

            typeImage = ConvertirAsciiEnString(octetsArrivee[0]) + ConvertirAsciiEnString(octetsArrivee[1]);
            tailleFichier = Convertir_endian_ToInt(octetsArrivee, 2, 5);
            tailleOffset = Convertir_endian_ToInt(octetsArrivee, 14, 17);
            largeur = Convertir_endian_ToInt(octetsArrivee, 18, 21);
            hauteur = Convertir_endian_ToInt(octetsArrivee, 22, 25);
            nbBitsParCouleur = Convertir_endian_ToInt(octetsArrivee, 28, 29);

            matPixel = new Pixel[hauteur, largeur];
            int compteur = 0;
            for (int i = 0; i < hauteur; i++)
            {
                for (int j = 0; j < largeur; j++)
                {
                    matPixel[i, j] = new Pixel(octetsArrivee[54 + 3 * compteur], octetsArrivee[54 + 3 * compteur + 1], octetsArrivee[54 + 3 * compteur + 2]);
                    compteur++;
                }
            }
        }
        #endregion

        #region METHODES DE BASE
        /// <summary>
        /// Transforme un code ASCII en une chaine de string, pour les codes qui nous intéresse, c'est-à-dire ceux qui correspondent à des lettres en majuscule
        /// </summary>
        /// <param name="octet"></param> Un octet
        /// <returns></returns> La lettre majuscule correspondant à l'octet en ASCII
        static public string ConvertirAsciiEnString(byte octet)
        {
            if (octet < 65 || octet > 90)
            {
                return "erreur";
            }
            else
            {
                if (octet == 65) { return "A"; }
                if (octet == 66) { return "B"; }
                if (octet == 67) { return "C"; }
                if (octet == 68) { return "D"; }
                if (octet == 69) { return "E"; }
                if (octet == 70) { return "F"; }
                if (octet == 71) { return "G"; }
                if (octet == 72) { return "H"; }
                if (octet == 73) { return "I"; }
                if (octet == 74) { return "J"; }
                if (octet == 75) { return "K"; }
                if (octet == 76) { return "L"; }
                if (octet == 77) { return "M"; }
                if (octet == 78) { return "N"; }
                if (octet == 79) { return "O"; }
                if (octet == 80) { return "P"; }
                if (octet == 81) { return "Q"; }
                if (octet == 82) { return "R"; }
                if (octet == 83) { return "S"; }
                if (octet == 84) { return "T"; }
                if (octet == 85) { return "U"; }
                if (octet == 86) { return "V"; }
                if (octet == 87) { return "W"; }
                if (octet == 88) { return "X"; }
                if (octet == 89) { return "Y"; }
                else { return "Z"; }
            }
        }

        /// <summary>
        /// Transforme une partie de chaine d'octets/bytes au format little endian en entier correspondant
        /// </summary>
        /// <param name="littleEndian"></param> La chaine d'octets
        /// <param name="debut"></param> Le premier indice de la chaine à prendre en compte
        /// <param name="fin"></param> Le dernier indice de la chaine à prendre en compte
        /// <returns></returns> L'entier correspondant
        static public int Convertir_endian_ToInt(byte[] littleEndian, int debut, int fin)
        {
            int taille = fin - debut + 1;
            double nombre = 0;
            for (int i = 0; i < taille; i++)
            {
                nombre += littleEndian[i + debut] * Math.Pow(256, i);
            }
            int sortie = Convert.ToInt32(nombre);
            return sortie;
        }

        /// <summary>
        /// Transforme un entier en chaine d'octets/bytes corrspondantes au format little endian
        /// </summary>
        /// <param name="valeur"></param> L'entier à convertir
        /// <param name="taille"></param> La taille du tableau de byte à obtenir
        /// <returns></returns> Un tableau de bytes au format little endian
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
        /// Sauvegarde l'image obtenue dans un fichier de bin>debug (pas besoin de changer tous les paramètres puisqu'on part d'une image existante)
        /// </summary>
        /// <param name="myFileReturn"></param> Nom sous lequel l'image obtenue sera enregistrée
        public void From_Image_To_File(string myFileReturn)
        {
            int nombreDeZero = ((matPixelSortie.GetLength(1) * 3) % 4);
            if (nombreDeZero != 0)
            {
                nombreDeZero = 4 - nombreDeZero;
            }
            octetsSortie = new byte[54 + matPixelSortie.GetLength(0) * matPixelSortie.GetLength(1) * 3 + nombreDeZero * hauteur];

            //Initialiser le header aux valeurs d'entrée, on changera celles qu'il faut par la suite
            for (int i = 0; i < 54; i++)
            {
                octetsSortie[i] = octetsArrivee[i];
            }

            //Taille totale du fichier en octet
            tailleFichier = octetsSortie.Length;
            byte[] tabTailleFichier = Convertir_Int_ToEndian(tailleFichier, 4);
            for (int i = 0; i < tabTailleFichier.Length; i++)
            {
                octetsSortie[i + 2] = tabTailleFichier[i];
            }

            //Largeur de l'image finale
            int largeurF = matPixelSortie.GetLength(1);
            byte[] tabLargeur = Convertir_Int_ToEndian(largeurF, 4);
            for (int i = 0; i < tabLargeur.Length; i++)
            {
                octetsSortie[i + 18] = tabLargeur[i];
            }

            //Hauteur de l'image finale
            int hauteurF = matPixelSortie.GetLength(0);
            byte[] tabHauteur = Convertir_Int_ToEndian(hauteurF, 4);
            for (int i = 0; i < tabHauteur.Length; i++)
            {
                octetsSortie[i + 22] = tabHauteur[i];
            }

            //Taille des données de l'image (en octets)
            tailleDonneesImage = octetsSortie.Length - 54;
            byte[] tabTailleDI = Convertir_Int_ToEndian(tailleDonneesImage, 4);
            for (int i = 0; i < tabTailleDI.Length; i++)
            {
                octetsSortie[i + 34] = tabTailleDI[i];
            }

            //Construire le tableau d'octets avec les données de la matrice de pixels
            int compteur = 54;
            for (int i = 0; i < matPixelSortie.GetLength(0); i++)
            {
                for (int j = 0; j < matPixelSortie.GetLength(1) + nombreDeZero; j++)
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

            //Sauvegarder le fichier de sortie
            File.WriteAllBytes(myFileReturn, octetsSortie);

            Process.Start(myFileReturn);
        }

        /// <summary>
        /// Donne quelques informations sur l'image traitée
        /// </summary>
        public void toString()
        {
            Console.WriteLine("TYPE DE L'IMAGE : " + typeImage);
            Console.WriteLine("TAILLE DU FICHIER : " + tailleFichier);
            Console.WriteLine("TAILLE DU OFFSET : " + tailleOffset);
            Console.WriteLine("HAUTEUR (en pixel) : " + hauteur);
            Console.WriteLine("LARGEUR (en pixel) : " + largeur);
            Console.WriteLine("NOMBRE DE BITS PAR COULEUR : " + this.nbBitsParCouleur);
            Console.WriteLine("IMAGE" + "\n" + "\n");
        }

        /// <summary>
        /// Dis si l'image en paramètre est en couleur ou non
        /// </summary>
        /// <returns></returns> Retourne true si l'image est en couleur et retourne false si l'image est en nuances de gris
        public bool ImageCouleur()
        {
            bool couleur = true;

            for (int i = 0; i < matPixel.GetLength(0); i++)
            {
                for (int j = 0; j < matPixel.GetLength(1); j++)
                {
                    byte rouge = matPixel[i, j].R;
                    byte vert = matPixel[i, j].V;
                    byte bleu = matPixel[i, j].B;

                    if (rouge == vert && rouge == bleu && bleu == vert)
                    {
                        couleur = false;
                    }
                    else
                    {
                        couleur = true;
                        break;
                    }
                }
            }

            return couleur;
        }

        /// <summary>
        /// Trie un tableau de byte en les rangeant du plus petit au plus grand
        /// </summary>
        /// <param name="tab"></param> Tableau à trier
        /// <returns></returns> Tableau trié
        static public byte[] TriInsertion(byte[] tab)
        {
            int index;
            byte temp;
            if (tab != null)
            {
                for (int i = 0; i < tab.Length; i++)
                {
                    index = 0;
                    while (tab[i] > tab[index])
                    {
                        index++;
                    }
                    temp = tab[i];
                    for (int j = i; j > index; j--)
                    {
                        tab[j] = tab[j - 1];
                    }
                    tab[index] = temp;
                }
            }
            return tab;
        }

        /// <summary>
        /// Convertit un nombre de type byte en tableau de int représentant un nombre en binaire sous 8 bits
        /// </summary>
        /// <param name="nombre"></param> Le nombre en byte à convertir
        /// <returns></returns> Un tableau de 8 chiffres binaire
        public static int[] IntToBinary(byte nombre)
        {
            int nb = Convert.ToInt32(nombre);
            int[] tab = new int[8];
            for (int i = 0; i < tab.Length; i++)
            {
                tab[i] = 0;
            }
            for (int i = 0; i < tab.Length; i++)
            {
                int facteur = Convert.ToInt32(Math.Pow(2, 7 - i));
                if (nb / facteur >= 1)
                {
                    tab[i] = 1;
                    nb -= facteur;
                }
            }

            return tab;

        }

        /// <summary>
        /// Convertit un tableau représentant un nombre en binaire sous 8 bits en nombre de type byte
        /// </summary>
        /// <param name="tab"></param> Le tableau de 8 chiffres binaire
        /// <returns></returns> Un nombre en bytes
        public static byte BinaryToInt(int[] tab)
        {
            int nb = 0;
            for (int i = 0; i < tab.Length; i++)
            {
                nb += Convert.ToInt32(tab[i] * Math.Pow(2, 7 - i));
            }
            return Convert.ToByte(nb);
        }

        /// <summary>
        /// Retourne les 4 premiers bits d'un tableau de binaire dans un tableau de binaire de longueur 8
        /// </summary>
        /// <param name="tab"></param> Tableau de 8 bits
        /// <returns></returns> Tableau de 8 bits
        public static int[] Debut(int[] tab)
        {
            int[] debut = new int[8];
            for (int i = 0; i < tab.Length; i++)
            {
                debut[i] = 0;
            }
            for (int i = 0; i < 4; i++)
            {
                debut[i] = tab[i];
            }
            return debut;
        }

        /// <summary>
        /// Retourne les 4 derniers bits d'un tableau de binaire dans un tableau de binaire de longueur 8
        /// </summary>
        /// <param name="tab"></param> Tableau de 8 bits
        /// <returns></returns> Tableau de 8 bits
        public static int[] Fin(int[] tab)
        {
            int[] fin = new int[8];
            for (int i = 0; i < tab.Length; i++)
            {
                fin[i] = 0;
            }
            for (int i = 4; i < tab.Length; i++)
            {
                fin[i - 4] = tab[i];
            }
            return fin;
        }

        /// <summary>
        /// Fusionne deux tableaux de bits, l'un après l'autre dans un tableau de longueur 8 bits
        /// </summary>
        /// <param name="tab1"></param> Les 4 premiers bits de ce tableau seront les 4 premiers bits du tableau final
        /// <param name="tab2"></param> Les 4 premiers bits de ce tableau seront les 4 derniers bits du tableau final
        /// <returns></returns> Le tableau de 8 bits ainsi créé
        public static int[] Fusion(int[] tab1, int[] tab2)
        {
            int[] fusion = new int[8];
            for (int i = 0; i < 4; i++)
            {
                fusion[i] = tab1[i];
            }
            for (int i = 4; i < tab1.Length; i++)
            {
                fusion[i] = tab2[i - 4];
            }
            return fusion;
        }
        #endregion

        #region METHODES DE TRANSFORMATION BASIQUE
        /// <summary>
        /// Copie l'image pour l'enregistrer sous un autre nom tout en gardant la première
        /// </summary>
        public void Copie()
        {
            matPixelSortie = new Pixel[matPixel.GetLength(0), matPixel.GetLength(1)];
            for (int i = 0; i < matPixel.GetLength(0); i++)
            {
                for (int j = 0; j < matPixel.GetLength(1); j++)
                {
                    matPixelSortie[i, j] = matPixel[i, j];
                }
            }
        }

        /// <summary>
        /// Compose la matrice de pixel de sortie pour qu'elle puisse aboutir à une photo en nuances de gris
        /// </summary>
        public void NuanceDeGris()
        {
            matPixelSortie = new Pixel[matPixel.GetLength(0), matPixel.GetLength(1)];
            for (int i = 0; i < matPixelSortie.GetLength(0); i++)
            {
                for (int j = 0; j < matPixelSortie.GetLength(1); j++)
                {
                    double moyenne = (matPixel[i, j].R + matPixel[i, j].V + matPixel[i, j].B) / 3;
                    byte codeGris = Convert.ToByte(Math.Round(moyenne));
                    matPixelSortie[i, j] = new Pixel(codeGris, codeGris, codeGris);
                }
            }
        }

        /// <summary>
        /// Compose la matrice de pixel de sortie pour qu'elle puisse aboutir à une photo en noir et blanc
        /// </summary>
        public void NoirEtBlanc()
        {
            matPixelSortie = new Pixel[matPixel.GetLength(0), matPixel.GetLength(1)];
            for (int i = 0; i < matPixelSortie.GetLength(0); i++)
            {
                for (int j = 0; j < matPixelSortie.GetLength(1); j++)
                {
                    double moyenne = (matPixel[i, j].R + matPixel[i, j].V + matPixel[i, j].B) / 3;
                    int codeGris = Convert.ToInt32(Math.Round(moyenne));
                    if (codeGris < 127.5)
                    {
                        matPixelSortie[i, j] = new Pixel(0, 0, 0);
                    }
                    else
                    {
                        matPixelSortie[i, j] = new Pixel(255, 255, 255);
                    }
                }
            }
        }

        /// <summary>
        /// Compose la matrice de pixel de sortie pour qu'elle puisse aboutir à une image rétrécie (en faisant la moyenne de ce couleur de chaque carré composé de 4 pixels)
        /// </summary>
        public void Retrecir()
        {
            double hauteurD = hauteur / 2;
            double largeurD = largeur / 2;
            if (hauteur % 2 == 1)
            {
                hauteurD = hauteurD - 0.5;
            }
            if (largeur % 2 == 1)
            {
                largeurD = largeurD - 0.5;
            }
            int hauteurS = Convert.ToInt32(Math.Round(hauteurD));
            int largeurS = Convert.ToInt32(Math.Round(largeurD));

            matPixelSortie = new Pixel[hauteurS, largeurS];

            for (int i = 0; i < matPixelSortie.GetLength(0); i++)
            {
                for (int j = 0; j < matPixelSortie.GetLength(1); j++)
                {
                    double rouge = (matPixel[i * 2, j * 2].R + matPixel[i * 2, j * 2 + 1].R + matPixel[i * 2 + 1, j * 2 + 1].R + matPixel[i * 2 + 1, j * 2].R) / 4;
                    byte red = Convert.ToByte(Math.Round(rouge));
                    double vert = (matPixel[i * 2, j * 2].V + matPixel[i * 2, j * 2 + 1].V + matPixel[i * 2 + 1, j * 2 + 1].V + matPixel[i * 2 + 1, j * 2].V) / 4;
                    byte green = Convert.ToByte(Math.Round(vert));
                    double bleu = (matPixel[i * 2, j * 2].B + matPixel[i * 2, j * 2 + 1].B + matPixel[i * 2 + 1, j * 2 + 1].B + matPixel[i * 2 + 1, j * 2].B) / 4;
                    byte blue = Convert.ToByte(Math.Round(bleu));
                    matPixelSortie[i, j] = new Pixel(red, green, blue);
                }
            }
        }

        /// <summary>
        /// Compose la matrice de pixel de sortie pour qu'elle puisse aboutir à une image agrandie (en faisant des carrés de 4 pixels de la même couleur)
        /// </summary>
        public void Agrandir()
        {
            int hauteurS = hauteur * 2;
            int largeurS = largeur * 2;

            matPixelSortie = new Pixel[hauteurS, largeurS];

            for (int i = 0; i < matPixel.GetLength(0); i++)
            {
                for (int j = 0; j < matPixel.GetLength(1); j++)
                {
                    matPixelSortie[i * 2, j * 2] = matPixel[i, j];
                    matPixelSortie[i * 2 + 1, j * 2] = matPixel[i, j];
                    matPixelSortie[i * 2, j * 2 + 1] = matPixel[i, j];
                    matPixelSortie[i * 2 + 1, j * 2 + 1] = matPixel[i, j];
                }
            }
        }

        /// <summary>
        /// Compose la matrice de pixels de sortie pour qu'elle puisse aboutir à une autre image dans laquelle l'image de départ apparait tournée
        /// </summary>
        /// <param name="degres"></param> De combien de degrés on va faire tourner la photo
        public void Rotation(int degres)
        {
            degres = degres % 360;
            double rad = degres * (Math.PI / 180);

            int hauteur = matPixel.GetLength(0);
            int largeur = matPixel.GetLength(1);

            //Création de la matrice de sortie;
            int diag = Convert.ToInt32(Math.Round(Math.Sqrt(hauteur * hauteur + largeur * largeur)));
            while (diag % 4 != 0)
            {
                diag++;
            }
            Pixel blanc = new Pixel(255, 255, 255);
            matPixelSortie = new Pixel[diag, diag];
            for (int i = 0; i < diag; i++)
            {
                for (int j = 0; j < diag; j++)
                {
                    matPixelSortie[i, j] = blanc;
                }
            }
            //Recherche des différents centres
            int centreH = hauteur / 2;
            int centreL = largeur / 2;
            int centreS = matPixelSortie.GetLength(0) / 2;

            //Application de la rotation
            for (int i = 0; i < matPixel.GetLength(0); i++)
            {
                for (int j = 0; j < matPixel.GetLength(1); j++)
                {
                    int newH = Convert.ToInt32(Math.Round((j - centreL) * Math.Sin(rad) + (i - centreH) * Math.Cos(rad)));
                    int newL = Convert.ToInt32(Math.Round((j - centreL) * Math.Cos(rad) - (i - centreH) * Math.Sin(rad)));

                    matPixelSortie[centreS + newH, centreS + newL] = matPixel[i, j];
                }
            }
            //Remplir les trous blancs de l'image
            for (int i = 0; i < diag; i++)
            {
                for (int j = 0; j < diag; j++)
                {
                    if (matPixelSortie[i, j] == blanc)
                    {
                        if (i == diag - 1 || j == diag - 1)
                        {
                            matPixelSortie[i, j] = matPixelSortie[i, j];
                        }
                        else
                        {
                            matPixelSortie[i, j] = matPixelSortie[i + 1, j];
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Compose la matrice de pixels de sortie pour qu'elle puisse aboutir à une image symétrique à la première par la droite ou la gauche
        /// </summary>
        public void Miroir_Cotes()
        {
            matPixelSortie = new Pixel[matPixel.GetLength(0), matPixel.GetLength(1)];
            for (int i = 0; i < matPixelSortie.GetLength(0); i++)
            {
                for (int j = 0; j < matPixelSortie.GetLength(1); j++)
                {
                    matPixelSortie[i, j] = matPixel[i, matPixel.GetLength(1) - 1 - j];
                }
            }
        }

        /// <summary>
        /// Compose la matrice de pixels de sortie pour qu'elle puisse aboutir à une image symétrique à la première par le haut ou par le bas
        /// </summary>
        public void Miroir_Haut_Bas()
        {
            matPixelSortie = new Pixel[matPixel.GetLength(0), matPixel.GetLength(1)];
            for (int i = 0; i < matPixelSortie.GetLength(0); i++)
            {
                for (int j = 0; j < matPixelSortie.GetLength(1); j++)
                {
                    matPixelSortie[i, j] = matPixel[matPixel.GetLength(0) - 1 - i, j];
                }
            }
        }

        /// <summary>
        /// Compose la matrice de pixels de sortie pour qu'elle puisse aboutir à une image symétrique à la première par rapport à l'un de ses angles
        /// </summary>
        public void Miroir_Angles()
        {
            matPixelSortie = new Pixel[matPixel.GetLength(0), matPixel.GetLength(1)];
            for (int i = 0; i < matPixelSortie.GetLength(0); i++)
            {
                for (int j = 0; j < matPixelSortie.GetLength(1); j++)
                {
                    matPixelSortie[i, j] = matPixel[matPixel.GetLength(0) - 1 - i, matPixel.GetLength(1) - 1 - j];
                }
            }
        }
        #endregion

        #region METHODES D'APPLICATION DE FILTRES
        /// <summary>
        /// Applique un filtre à la matrice de pixels de sortie pour qu'elle aboutisse à une image modifiée selon les souhaits de l'utilisateur
        /// </summary>
        /// <param name="noyau"></param> Matrice carrée spécifique à chaque filtre. On les a trouvé sur internet à chaque fois
        /// <param name="ajout"></param> Après avoir appliqué le noyau, il faut parfois ajouter un nombre avant de diviser le tout par la normalistaion, pour chaque pixel
        public void ApplicationFiltreUnique(int[,] noyau, int ajout)
        {
            //Verification de la validité du noyau (matrice carrée et de longueur impair)
            bool verif = false;
            if (noyau.GetLength(0) == noyau.GetLength(1) && noyau.GetLength(0) % 2 != 0)
            {
                verif = true;
            }

            if (verif)
            {
                //Calcul de la normalisation
                int normalisation = 0;
                int somme = 0;
                for (int i = 0; i < noyau.GetLength(0); i++)
                {
                    for (int j = 0; j < noyau.GetLength(1); j++)
                    {
                        somme += noyau[i, j];
                    }
                }
                if (somme > 0)
                {
                    normalisation = somme;
                }
                else
                {
                    for (int i = 0; i < noyau.GetLength(0); i++)
                    {
                        for (int j = 0; j < noyau.GetLength(1); j++)
                        {
                            if (noyau[i, j] > 0)
                            {
                                normalisation += noyau[i, j];
                            }
                        }
                    }
                }

                //Application du noyau
                matPixelSortie = new Pixel[matPixel.GetLength(0), matPixel.GetLength(1)];
                int max = noyau.GetLength(0) / 2;
                for (int i = 0; i < matPixelSortie.GetLength(0); i++)
                {
                    for (int j = 0; j < matPixelSortie.GetLength(1); j++)
                    {
                        //Matrice de contour (pour le rouge, le vert, et le bleu)
                        byte[,] contourRouge = new byte[noyau.GetLength(0), noyau.GetLength(1)];
                        byte[,] contourVert = new byte[noyau.GetLength(0), noyau.GetLength(1)];
                        byte[,] contourBleu = new byte[noyau.GetLength(0), noyau.GetLength(1)];
                        for (int k = i - max; k <= i + max; k++)
                        {
                            for (int l = j - max; l <= j + max; l++)
                            {
                                if (k < 0 || l < 0 || k >= matPixel.GetLength(0) || l >= matPixel.GetLength(1))
                                {
                                    contourRouge[k - (i - max), l - (j - max)] = 0;
                                    contourVert[k - (i - max), l - (j - max)] = 0;
                                    contourBleu[k - (i - max), l - (j - max)] = 0;
                                }
                                else
                                {
                                    contourRouge[k - (i - max), l - (j - max)] = matPixel[k, l].R;
                                    contourVert[k - (i - max), l - (j - max)] = matPixel[k, l].V;
                                    contourBleu[k - (i - max), l - (j - max)] = matPixel[k, l].B;
                                }
                            }
                        }

                        //Attribution de nouvelles valeurs au pixel

                        int rougeInt = 0;
                        int vertInt = 0;
                        int bleuInt = 0;

                        for (int k = 0; k < noyau.GetLength(0); k++)
                        {
                            for (int l = 0; l < noyau.GetLength(1); l++)
                            {
                                rougeInt += noyau[k, l] * Convert.ToInt32(contourRouge[k, l]);
                                vertInt += noyau[k, l] * Convert.ToInt32(contourVert[k, l]);
                                bleuInt += noyau[k, l] * Convert.ToInt32(contourBleu[k, l]);
                            }
                        }

                        rougeInt = rougeInt + ajout / normalisation;
                        vertInt = vertInt + ajout / normalisation;
                        bleuInt = bleuInt + ajout / normalisation;

                        if (rougeInt < 0)
                        {
                            rougeInt = -rougeInt;
                        }
                        if (rougeInt > 255)
                        {
                            rougeInt = 255;
                        }
                        if (vertInt < 0)
                        {
                            vertInt = -vertInt;
                        }
                        if (vertInt > 255)
                        {
                            vertInt = 255;
                        }
                        if (bleuInt < 0)
                        {
                            bleuInt = -bleuInt;
                        }
                        if (bleuInt > 255)
                        {
                            bleuInt = 255;
                        }

                        byte rougeM = Convert.ToByte(rougeInt);
                        byte vertM = Convert.ToByte(vertInt);
                        byte bleuM = Convert.ToByte(bleuInt);

                        matPixelSortie[i, j] = new Pixel(rougeM, vertM, bleuM);

                    }
                }
            }
            else
            {
                Console.WriteLine("Le format du noyau n'est pas valide");
                Copie();
            }
        }

        /// <summary>
        /// Applique deux filtres à la matrice de pixels de sortie pour qu'elle aboutisse à une image doublement modifiée selon les les souhaits de l'utilisateur
        /// </summary>
        /// <param name="noyau1"></param> Matrice carrée spécifique à chaque filtre. On les a trouvé sur internet à chasue fois
        /// <param name="noyau2"></param> Matrice carrée spécifique à chaque filtre. On les a trouvé sur internet à chasue fois
        /// <param name="ajout1"></param> Après avoir appliqué le noyau, il faut parfois ajouter un nombre avant de diviser le tout par la normalistaion, pour chaque pixel
        /// <param name="ajout2"></param> Après avoir appliqué le noyau, il faut parfois ajouter un nombre avant de diviser le tout par la normalistaion, pour chaque pixel
        public void ApplicationFiltreDouble(int[,] noyau1, int[,] noyau2, int ajout1, int ajout2)
        {
            //Verification de la validité du noyau (matrice carrée et de longueur impair)
            bool verif = false;
            if (noyau1.GetLength(0) == noyau1.GetLength(1) && noyau1.GetLength(0) % 2 != 0 && noyau2.GetLength(0) == noyau2.GetLength(1) && noyau1.GetLength(0) % 2 != 0)
            {
                verif = true;
            }

            if (verif)
            {
                //Calcul Normalisation1
                int normalisation1 = 0;
                int somme1 = 0;
                for (int i = 0; i < noyau1.GetLength(0); i++)
                {
                    for (int j = 0; j < noyau1.GetLength(1); j++)
                    {
                        somme1 += noyau1[i, j];
                    }
                }
                if (somme1 > 0)
                {
                    normalisation1 = somme1;
                }
                else
                {
                    for (int i = 0; i < noyau1.GetLength(0); i++)
                    {
                        for (int j = 0; j < noyau1.GetLength(1); j++)
                        {
                            if (noyau1[i, j] > 0)
                            {
                                normalisation1 += noyau1[i, j];
                            }
                        }
                    }
                }

                //Calcul normalisation noyau2
                int normalisation2 = 0;
                int somme2 = 0;
                for (int i = 0; i < noyau2.GetLength(0); i++)
                {
                    for (int j = 0; j < noyau2.GetLength(1); j++)
                    {
                        somme2 += noyau2[i, j];
                    }
                }
                if (somme2 > 0)
                {
                    normalisation2 = somme2;
                }
                else
                {
                    for (int i = 0; i < noyau2.GetLength(0); i++)
                    {
                        for (int j = 0; j < noyau2.GetLength(1); j++)
                        {
                            if (noyau2[i, j] > 0)
                            {
                                normalisation2 += noyau2[i, j];
                            }
                        }
                    }
                }

                //Application du noyau
                matPixelSortie = new Pixel[matPixel.GetLength(0), matPixel.GetLength(1)];
                int max1 = noyau1.GetLength(0) / 2;
                int max2 = noyau2.GetLength(0) / 2;

                for (int i = 0; i < matPixelSortie.GetLength(0); i++)
                {
                    for (int j = 0; j < matPixelSortie.GetLength(1); j++)
                    {
                        byte[,] contourRouge1 = new byte[noyau1.GetLength(0), noyau1.GetLength(1)];
                        byte[,] contourVert1 = new byte[noyau1.GetLength(0), noyau1.GetLength(1)];
                        byte[,] contourBleu1 = new byte[noyau1.GetLength(0), noyau1.GetLength(1)];
                        byte[,] contourRouge2 = new byte[noyau2.GetLength(0), noyau2.GetLength(1)];
                        byte[,] contourVert2 = new byte[noyau2.GetLength(0), noyau2.GetLength(1)];
                        byte[,] contourBleu2 = new byte[noyau2.GetLength(0), noyau2.GetLength(1)];

                        //Pour le noyau 1
                        for (int k = i - max1; k <= i + max1; k++)
                        {
                            for (int l = j - max1; l <= j + max1; l++)
                            {
                                if (k < 0 || l < 0 || k >= matPixel.GetLength(0) || l >= matPixel.GetLength(1))
                                {
                                    contourRouge1[k - (i - max1), l - (j - max1)] = 0;
                                    contourVert1[k - (i - max1), l - (j - max1)] = 0;
                                    contourBleu1[k - (i - max1), l - (j - max1)] = 0;
                                }
                                else
                                {
                                    contourRouge1[k - (i - max1), l - (j - max1)] = matPixel[k, l].R;
                                    contourVert1[k - (i - max1), l - (j - max1)] = matPixel[k, l].V;
                                    contourBleu1[k - (i - max1), l - (j - max1)] = matPixel[k, l].B;
                                }
                            }
                        }
                        //Pour le noyau 2
                        for (int k = i - max2; k <= i + max2; k++)
                        {
                            for (int l = j - max2; l <= j + max2; l++)
                            {
                                if (k < 0 || l < 0 || k >= matPixel.GetLength(0) || l >= matPixel.GetLength(1))
                                {
                                    contourRouge2[k - (i - max2), l - (j - max2)] = 0;
                                    contourVert2[k - (i - max2), l - (j - max2)] = 0;
                                    contourBleu2[k - (i - max2), l - (j - max2)] = 0;
                                }
                                else
                                {
                                    contourRouge2[k - (i - max2), l - (j - max2)] = matPixel[k, l].R;
                                    contourVert2[k - (i - max2), l - (j - max2)] = matPixel[k, l].V;
                                    contourBleu2[k - (i - max2), l - (j - max2)] = matPixel[k, l].B;
                                }
                            }
                        }

                        //Attribution d'une nouvelle valeur au pixel

                        //Pour le noyau 1
                        int rougeInt1 = 0;
                        int vertInt1 = 0;
                        int bleuInt1 = 0;
                        for (int k = 0; k < noyau1.GetLength(0); k++)
                        {
                            for (int l = 0; l < noyau1.GetLength(1); l++)
                            {
                                rougeInt1 += noyau1[k, l] * Convert.ToInt32(contourRouge1[k, l]);
                                vertInt1 += noyau1[k, l] * Convert.ToInt32(contourVert1[k, l]);
                                bleuInt1 += noyau1[k, l] * Convert.ToInt32(contourBleu1[k, l]);
                            }
                        }
                        rougeInt1 = rougeInt1 + ajout1 / normalisation1;
                        vertInt1 = vertInt1 + ajout1 / normalisation1;
                        bleuInt1 = bleuInt1 + ajout1 / normalisation1;

                        //Pour le noyau 2
                        int rougeInt2 = 0;
                        int vertInt2 = 0;
                        int bleuInt2 = 0;
                        for (int k = 0; k < noyau2.GetLength(0); k++)
                        {
                            for (int l = 0; l < noyau2.GetLength(1); l++)
                            {
                                rougeInt2 += noyau2[k, l] * Convert.ToInt32(contourRouge2[k, l]);
                                vertInt2 += noyau2[k, l] * Convert.ToInt32(contourVert2[k, l]);
                                bleuInt2 += noyau2[k, l] * Convert.ToInt32(contourBleu2[k, l]);
                            }
                        }
                        rougeInt2 = rougeInt2 + ajout2 / normalisation2;
                        vertInt2 = vertInt2 + ajout2 / normalisation2;
                        bleuInt2 = bleuInt2 + ajout2 / normalisation2;

                        //Somme des deux filtres
                        int rougeInt = rougeInt1 + rougeInt2;
                        int vertInt = vertInt1 + vertInt2;
                        int bleuInt = bleuInt1 + bleuInt2;

                        if (rougeInt < 0)
                        {
                            rougeInt = -rougeInt;
                        }
                        if (rougeInt > 255)
                        {
                            rougeInt = 255;
                        }
                        if (vertInt < 0)
                        {
                            vertInt = -vertInt;
                        }
                        if (vertInt > 255)
                        {
                            vertInt = 255;
                        }
                        if (bleuInt < 0)
                        {
                            bleuInt = -bleuInt;
                        }
                        if (bleuInt > 255)
                        {
                            bleuInt = 255;
                        }

                        byte rougeM = Convert.ToByte(rougeInt);
                        byte vertM = Convert.ToByte(vertInt);
                        byte bleuM = Convert.ToByte(bleuInt);

                        matPixelSortie[i, j] = new Pixel(rougeM, vertM, bleuM);

                    }
                }
            }
            else
            {
                Console.WriteLine("Le format des noyaux n'est pas valide");
                Copie();
            }
        }

        /// <summary>
        /// Applique un filtre en fonction du voisinnage à une image
        /// </summary>
        /// <param name="voisinage"></param> Largeur et hauteur en pixels du carré qui entoure chaque pixel lors de son étude
        public void CoupeMediane(int voisinage) //Autre type de filtre
        {
            //Verification du paramètre
            bool verification = false;
            if (voisinage % 2 != 0 && voisinage > 0 && voisinage < matPixel.GetLength(0))
            {
                verification = true;
            }
            if (verification)
            {
                //Application du noyau
                matPixelSortie = new Pixel[matPixel.GetLength(0), matPixel.GetLength(1)];
                int max = voisinage / 2;
                for (int i = 0; i < matPixelSortie.GetLength(0); i++)
                {
                    for (int j = 0; j < matPixelSortie.GetLength(1); j++)
                    {
                        //tableau de byte contour 
                        byte[] contourRouge = new byte[voisinage * voisinage];
                        byte[] contourVert = new byte[voisinage * voisinage];
                        byte[] contourBleu = new byte[voisinage * voisinage];
                        int compteur = 0;
                        for (int k = i - max; k <= i + max; k++)
                        {
                            for (int l = j - max; l <= j + max; l++)
                            {
                                if (k < 0 || l < 0 || k >= matPixel.GetLength(0) || l >= matPixel.GetLength(1))
                                {
                                    contourRouge[compteur] = 0;
                                    contourVert[compteur] = 0;
                                    contourBleu[compteur] = 0;
                                    compteur++;
                                }
                                else
                                {
                                    contourRouge[compteur] = matPixel[k, l].R;
                                    contourVert[compteur] = matPixel[k, l].V;
                                    contourBleu[compteur] = matPixel[k, l].B;
                                    compteur++;
                                }
                            }
                        }
                        //Trie des Tableaux de Bytes
                        contourRouge = TriInsertion(contourRouge);
                        contourVert = TriInsertion(contourVert);
                        contourBleu = TriInsertion(contourBleu);

                        //Attribution de la valeur
                        byte rougeM = contourRouge[max + 1];
                        byte vertM = contourVert[max + 1];
                        byte bleuM = contourBleu[max + 1];

                        matPixelSortie[i, j] = new Pixel(rougeM, vertM, bleuM);
                    }
                }
            }
            else
            {
                Console.WriteLine("Le format des noyaux n'est pas valide");
                Copie();
            }

        }
        #endregion

        #region METHODES DE STENOGRAPHIE
        /// <summary>
        /// Pour un cas de sténographie, renvoi la matrice de pixels qui représente l'image qui en cachait une autre
        /// </summary>
        public void ImagePrincipale()
        {
            matPixelSortie = new Pixel[matPixel.GetLength(0), matPixel.GetLength(1)];
            for (int i = 0; i < matPixelSortie.GetLength(0); i++)
            {
                for (int j = 0; j < matPixelSortie.GetLength(1); j++)
                {
                    byte rouge = matPixel[i, j].R;
                    byte vert = matPixel[i, j].V;
                    byte bleu = matPixel[i, j].B;

                    int[] tabRouge = IntToBinary(rouge);
                    int[] tabVert = IntToBinary(vert);
                    int[] tabBleu = IntToBinary(bleu);

                    tabRouge = Debut(tabRouge);
                    tabVert = Debut(tabVert);
                    tabBleu = Debut(tabBleu);

                    byte rougeS = BinaryToInt(tabRouge);
                    byte vertS = BinaryToInt(tabVert);
                    byte bleuS = BinaryToInt(tabBleu);

                    matPixelSortie[i, j] = new Pixel(rougeS, vertS, bleuS);
                }
            }
        }

        /// <summary>
        /// Pour un cas de sténographie, renvoi la matrice de pixels qui représente l'image qui était cachée par une autre
        /// </summary>
        public void ImageCache()
        {
            matPixelSortie = new Pixel[matPixel.GetLength(0), matPixel.GetLength(1)];
            for (int i = 0; i < matPixelSortie.GetLength(0); i++)
            {
                for (int j = 0; j < matPixelSortie.GetLength(1); j++)
                {
                    byte rouge = matPixel[i, j].R;
                    byte vert = matPixel[i, j].V;
                    byte bleu = matPixel[i, j].B;

                    int[] tabRouge = IntToBinary(rouge);
                    int[] tabVert = IntToBinary(vert);
                    int[] tabBleu = IntToBinary(bleu);

                    tabRouge = Fin(tabRouge);
                    tabVert = Fin(tabVert);
                    tabBleu = Fin(tabBleu);

                    byte rougeS = BinaryToInt(tabRouge);
                    byte vertS = BinaryToInt(tabVert);
                    byte bleuS = BinaryToInt(tabBleu);

                    matPixelSortie[i, j] = new Pixel(rougeS, vertS, bleuS);
                }
            }
        }

        /// <summary>
        /// Conditionne la matrice de pixels de sortie pour qu'une image de la classe MyImage en cache une autre, de la même taille
        /// </summary>
        /// <param name="image"></param> Image cachée par l'image principale
        public void Fusion(MyImage image)
        {
            if (matPixel.GetLength(0) == image.matPixel.GetLength(0) && matPixel.GetLength(1) == image.matPixel.GetLength(1))
            {
                matPixelSortie = new Pixel[matPixel.GetLength(0), matPixel.GetLength(1)];
                for (int i = 0; i < matPixelSortie.GetLength(0); i++)
                {
                    for (int j = 0; j < matPixelSortie.GetLength(1); j++)
                    {
                        byte rouge1 = matPixel[i, j].R;
                        byte vert1 = matPixel[i, j].V;
                        byte bleu1 = matPixel[i, j].B;
                        byte rouge2 = image.matPixel[i, j].R;
                        byte vert2 = image.matPixel[i, j].V;
                        byte bleu2 = image.matPixel[i, j].B;

                        int[] tabRougeDevant = IntToBinary(rouge1);
                        int[] tabVertDevant = IntToBinary(vert1);
                        int[] tabBleuDevant = IntToBinary(bleu1);

                        int[] tabRougeBack = IntToBinary(rouge2);
                        int[] tabVertBack = IntToBinary(vert2);
                        int[] tabBleuBack = IntToBinary(bleu2);

                        int[] tabRougeFusion = Fusion(tabRougeDevant, tabRougeBack);
                        int[] tabVertFusion = Fusion(tabVertDevant, tabVertBack);
                        int[] tabBleuFusion = Fusion(tabBleuDevant, tabBleuBack);

                        byte rougeFusion = BinaryToInt(tabRougeFusion);
                        byte vertFusion = BinaryToInt(tabVertFusion);
                        byte bleuFusion = BinaryToInt(tabBleuFusion);

                        matPixelSortie[i, j] = new Pixel(rougeFusion, vertFusion, bleuFusion);
                    }
                }
            }
        }
        #endregion

        #region INOVATION 

        /// <summary>
        /// Compose la matrice de pixel de sortie pour qu'elle puisse aboutir à une photo en négatif
        /// </summary>
        public void Negatif()
        {
            matPixelSortie = new Pixel[hauteur, largeur];
            for (int i = 0; i < hauteur; i++)
            {
                for (int j = 0; j < largeur; j++)
                {
                    byte rougeNeg = Convert.ToByte(255 - matPixel[i, j].R);
                    byte vertNeg = Convert.ToByte(255 - matPixel[i, j].V);
                    byte bleuNeg = Convert.ToByte(255 - matPixel[i, j].B);

                    matPixelSortie[i, j] = new Pixel(rougeNeg, vertNeg, bleuNeg);
                }
            }
        }

        /// <summary>
        /// Compose la matrice de pixel de sortie pour qu'elle puisse aboutir à une photo ou seul le rouge apparait
        /// </summary>
        public void FiltreBleu()
        {
            matPixelSortie = new Pixel[hauteur, largeur];
            for (int i = 0; i < hauteur; i++)
            {
                for (int j = 0; j < largeur; j++)
                {
                    int rouge = matPixel[i, j].R;
                    int vert = matPixel[i, j].V;
                    int bleu = matPixel[i, j].B;

                    //rouge >= 2 * Math.Max(vert, bleu)
                    if (Math.Abs(vert - bleu) <= 50 && (rouge > 200 || rouge > 1.5 * Math.Max(vert, bleu)))
                    {
                        matPixelSortie[i, j] = matPixel[i, j];
                    }
                    else
                    {
                        byte moyenne = matPixel[i, j].MoyennePixel();
                        matPixelSortie[i, j] = new Pixel(moyenne, moyenne, moyenne);
                    }
                }
            }
        }

        /// <summary>
        /// Compose la matrice de pixel de sortie pour qu'elle puisse aboutir à une photo ou seul le rouge apparait
        /// </summary>
        public void FiltreVert_Jaune()
        {
            matPixelSortie = new Pixel[hauteur, largeur];
            for (int i = 0; i < hauteur; i++)
            {
                for (int j = 0; j < largeur; j++)
                {
                    int rouge = matPixel[i, j].R;
                    int vert = matPixel[i, j].V;
                    int bleu = matPixel[i, j].B;

                    if (vert > 200 || (vert > Math.Max(bleu, rouge) && Math.Abs(rouge - bleu) < 100))
                    {
                        matPixelSortie[i, j] = matPixel[i, j];
                    }
                    else
                    {
                        byte moyenne = matPixel[i, j].MoyennePixel();
                        matPixelSortie[i, j] = new Pixel(moyenne, moyenne, moyenne);
                    }
                }
            }
        }

        /// <summary>
        /// Compose la matrice de pixel de sortie pour qu'elle puisse aboutir à une photo ou seul le bleu apparait
        /// </summary>
        public void FiltreRouge()
        {
            matPixelSortie = new Pixel[hauteur, largeur];
            for (int i = 0; i < hauteur; i++)
            {
                for (int j = 0; j < largeur; j++)
                {
                    int rouge = matPixel[i, j].R;
                    int vert = matPixel[i, j].V;
                    int bleu = matPixel[i, j].B;

                    if (bleu > 200 || (bleu > Math.Max(vert, rouge) && Math.Abs(rouge - vert) < 100))
                    {
                        matPixelSortie[i, j] = matPixel[i, j];
                    }
                    else
                    {
                        byte moyenne = matPixel[i, j].MoyennePixel();
                        matPixelSortie[i, j] = new Pixel(moyenne, moyenne, moyenne);
                    }
                }
            }
        }

        /// <summary>
        /// Compose la matrice de pixel de sortie pour qu'elle puisse aboutir à une photo unicolore
        /// </summary>
        /// <param name="couleur"></param> la nuance voulue (rouge, vert, bleu, jaune)
        public void UniColore(string couleur)
        {
            byte rouge = 0;
            byte vert = 0;
            byte bleu = 0;

            matPixelSortie = new Pixel[hauteur, largeur];
            for (int i = 0; i < hauteur; i++)
            {
                for (int j = 0; j < largeur; j++)
                {
                    if (couleur == "rouge")
                    {
                        rouge = matPixel[i, j].R;
                        vert = 0;
                        bleu = 0;
                    }
                    if (couleur == "vert")
                    {
                        rouge = 0;
                        vert = matPixel[i, j].V;
                        bleu = 0;
                    }
                    if (couleur == "bleu")
                    {
                        rouge = 0;
                        vert = 0;
                        bleu = matPixel[i, j].B;
                    }
                    if (couleur == "jaune")
                    {
                        rouge = matPixel[i, j].V;
                        vert = matPixel[i, j].V;
                        bleu = 0;
                    }

                    matPixelSortie[i, j] = new Pixel(bleu, vert, rouge);
                }
            }
        }

        /// <summary>
        /// Compose la matrice de pixel de sortie pour qu'elle puisse aboutir à une mosaique
        /// </summary>
        public void Mosaique()
        {
            MyImage image1 = new MyImage(this.myFile);
            MyImage image2 = new MyImage(this.myFile);
            MyImage image3 = new MyImage(this.myFile);
            MyImage image4 = new MyImage(this.myFile);

            image1.UniColore("rouge");
            image2.UniColore("vert");
            image3.UniColore("bleu");
            image4.UniColore("jaune");


            matPixelSortie = new Pixel[hauteur * 2, largeur * 2];
            for (int i = 0; i < hauteur; i++)
            {
                for (int j = 0; j < largeur; j++)
                {
                    matPixelSortie[i, j] = new Pixel(image1.MatPixelSortie[i, j]);
                }
            }
            for (int i = hauteur; i < matPixelSortie.GetLength(0); i++)
            {
                for (int j = 0; j < largeur; j++)
                {
                    matPixelSortie[i, j] = new Pixel(image2.MatPixelSortie[i - hauteur, j]);
                }
            }
            for (int i = 0; i < hauteur; i++)
            {
                for (int j = largeur; j < matPixelSortie.GetLength(1); j++)
                {
                    matPixelSortie[i, j] = new Pixel(image3.MatPixelSortie[i, j - largeur]);
                }
            }
            for (int i = hauteur; i < matPixelSortie.GetLength(0); i++)
            {
                for (int j = largeur; j < matPixelSortie.GetLength(1); j++)
                {
                    matPixelSortie[i, j] = new Pixel(image4.MatPixelSortie[i - hauteur, j - largeur]);
                }
            }
        }

        #endregion
    }
}
