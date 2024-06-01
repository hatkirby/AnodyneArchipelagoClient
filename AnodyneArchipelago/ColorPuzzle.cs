using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;

namespace AnodyneArchipelago
{
    public class ColorPuzzle
    {
        private Point _apartmentPos;
        private Point _circusPos;
        private Point _hotelPos;

        public Point ApartmentPos => _apartmentPos;
        public Point CircusPos => _circusPos;
        public Point HotelPos => _hotelPos;

        public void Initialize(Random rng)
        {
            HashSet<Point> alreadyChosen = new() { new Point(1, 1) };

            _apartmentPos = GetNextPoint(rng, ref alreadyChosen);
            _circusPos = GetNextPoint(rng, ref alreadyChosen);
            _hotelPos = GetNextPoint(rng, ref alreadyChosen);
        }

        private Point GetNextPoint(Random rng, ref HashSet<Point> alreadyChosen)
        {
            while (true)
            {
                Point nextPoint = new(rng.Next(6), rng.Next(6));

                if (!alreadyChosen.Contains(nextPoint))
                {
                    alreadyChosen.Add(nextPoint);
                    return nextPoint;
                }
            }
        }
    }
}
