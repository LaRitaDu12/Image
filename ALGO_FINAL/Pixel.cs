using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ALGO_FINAL
{
    public class Pixel
    {
        #region ATTRIBUTS
        private byte rouge;
        private byte vert;
        private byte bleu;
        #endregion

        #region PROPRIETES
        public byte R
        {
            get { return this.rouge; }
            set { this.rouge = value; }
        }
        public byte V
        {
            get { return this.vert; }
            set { this.vert = value; }
        }
        public byte B
        {
            get { return this.bleu; }
            set { this.bleu = value; }
        }
        #endregion

        #region CONSTRUCTEUR
        public Pixel(Pixel pixel)
        {
            this.rouge = pixel.R;
            this.vert = pixel.V;
            this.bleu = pixel.B;
        }
        public Pixel(byte rouge, byte vert, byte bleu)
        {
            this.rouge = rouge;
            this.vert = vert;
            this.bleu = bleu;
        }
        #endregion

        #region METHODE D'INSTANCES
        /// <summary>
        /// Détermine le code de la nuance de gris correspondant à un pixel
        /// </summary>
        /// <returns></returns> Retourne la moyenne des 3 composantes d'un pixel
        public byte MoyennePixel()
        {
            int rouge = Convert.ToInt32(this.rouge);
            int vert = Convert.ToInt32(this.vert);
            int bleu = Convert.ToInt32(this.bleu);

            int moyenne = (rouge + bleu + vert) / 3;

            return Convert.ToByte(moyenne);
        }

        /// <summary>
        /// Affiche les composantes du pixel
        /// </summary>
        public void toString()
        {
            Console.Write(this.rouge + " " + this.vert + " " + this.bleu + " ");
        }

        #endregion
    }
}
